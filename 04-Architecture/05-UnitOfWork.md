# 03-Patterns / Unit of Work Pattern & Gestión de Transacciones

**Dominio de referencia:** Dental Clinic (citas, odontograma, facturación)

---

## 1. Propósito

Garantizar que múltiples operaciones en repositorios individuales se confirmen en **una única transacción atómica (ACID)**. El patrón mantiene una lista de objetos afectados por una transacción de negocio y coordina la escritura de cambios y la resolución de concurrencia en una sola operación *All-or-Nothing* (Todo o Nada).

### Escenario de negocio en el sistema dental

Un paciente asiste a una cita. El dentista registra la finalización del tratamiento (Treatment Completed), genera un cargo financiero (Invoice / Billing Record) y actualiza el estado del diente en el Odontograma.

Si procesamos estas operaciones de forma aislada:

1. Se guarda la actualización del odontograma. 🟢
2. Se guarda el cambio de estado de la cita. 🟢
3. Falla la conexión a la base de datos o el servicio al intentar crear la factura. 🔴

**El problema:** Tu base de datos quedó en un estado inconsistente. La cita aparece completada y el diente marcado como tratado, pero la clínica nunca registró la deuda/pago del paciente.

---

## 2. ¿Cómo encaja con Entity Framework Core?

En .NET, la clase `DbContext` **ya es una implementación nativa del Unit of Work Pattern**.

Cada vez que haces:

```csharp
context.Appointments.Update(appointment);
context.Invoices.Add(invoice);
context.Odontograms.Update(odontogram);
```

EF Core no ejecuta ningún SQL en la base de datos en ese momento. En su lugar, el **Change Tracker** de `DbContext` registra cada entidad en un estado (Added, Modified, Deleted).

Únicamente cuando llamas a:

```csharp
await context.SaveChangesAsync();
```

`DbContext` inicia una transacción explícita en SQL Server, traduce todos los cambios acumulados a sentencias `INSERT`, `UPDATE` y `DELETE`, y las ejecuta dentro de ese bloque transaccional. Si cualquiera de las sentencias falla, SQL Server realiza un **ROLLBACK automático**.

---

## 3. Ejemplo Práctico: Caso "Atender Cita Dental"

```csharp
namespace DentalClinic.Application.Appointments.Commands.CompleteAppointment;

public record CompleteAppointmentCommand(
    Guid AppointmentId,
    List<Guid> CompletedTreatmentIds,
    decimal TotalAmountCharged) : IRequest<bool>;

public class CompleteAppointmentCommandHandler : IRequestHandler<CompleteAppointmentCommand, bool>
{
    private readonly IApplicationDbContext _context; // DbContext como Unit of Work

    public CompleteAppointmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CompleteAppointmentCommand request, CancellationToken ct)
    {
        // 1. Cargar la Cita y el Paciente
        var appointment = await _context.Appointments
            .Include(a => a.Treatments)
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, ct);

        if (appointment == null) return false;

        // 2. Regla de Dominio: Marcar cita como completada
        appointment.MarkAsCompleted();

        // 3. Regla de Dominio: Generar la Factura/Cobro al Paciente
        var invoice = Invoice.Create(
            patientId: appointment.PatientId,
            appointmentId: appointment.Id,
            amount: request.TotalAmountCharged
        );

        _context.Invoices.Add(invoice); // Acumula en el Change Tracker (Estado: Added)

        // 4. ATOMICIDAD (Unit of Work): Un solo SaveChangesAsync persiste la Cita Y la Factura.
        //    Si la Factura falla en SQL, la Cita tampoco cambia de estado.
        await _context.SaveChangesAsync(ct);

        return true;
    }
}
```

---

## 4. Transacciones Explícitas (`BeginTransactionAsync`)

`SaveChangesAsync` por sí solo crea una transacción implícita para todo lo que esté dentro de ese `DbContext`. Pero, ¿qué pasa si necesitas coordinar **múltiples `SaveChangesAsync`** o llamar a un **Stored Procedure externo** dentro de la misma transacción?

Para eso usamos una **Transacción Explícita**:

```csharp
using var transaction = await _context.Database.BeginTransactionAsync(ct);

try
{
    // Operación 1
    appointment.MarkAsCompleted();
    await _context.SaveChangesAsync(ct);

    // Operación 2 (Ej. llamada a un servicio de integración o procedimiento)
    await _externalBillingProcedure.ExecuteAsync(invoice);
    await _context.SaveChangesAsync(ct);

    // Si todo salió bien, confirmamos permanentemente los cambios
    await transaction.CommitAsync(ct);
}
catch (Exception)
{
    // Si algo falla en cualquier punto, revertimos todo
    await transaction.RollbackAsync(ct);
    throw;
}
```

---

## 5. 🎤 Respuesta de Entrevista en Inglés (Senior/Staff Level)

> **Interviewer:** *"How does Unit of Work ensure transactional integrity in .NET?"*
>
> **Your Answer:**  
> "The Unit of Work pattern coordinates writes across multiple repositories sharing the same `DbContext` session. Instead of each repository calling `SaveChangesAsync()` individually—which risks partial writes—the application handler executes business logic across multiple repositories and issues a single commit at the end. This guarantees atomic, transactional integrity across the operation."

> **Interviewer:** *"What is the Unit of Work pattern, and how do you implement it in EF Core?"*
>
> **Your Answer (versión Architect):**  
> "The Unit of Work pattern maintains a list of business transactions and coordinates the writing of changes to ensure data consistency through atomicity—the ACID 'A'. In .NET, `DbContext` natively implements Unit of Work. Through its Change Tracker, it aggregates entity modifications in memory. When `SaveChangesAsync()` is invoked, EF Core wraps all pending INSERT, UPDATE, and DELETE operations inside a single database transaction. In my architectures, I rely on `DbContext`'s built-in Unit of Work for single-context operations to avoid unnecessary abstractions like `IUnitOfWork`. However, when coordinating operations across multiple aggregates or explicit steps, I use `IDbContextTransaction` via `BeginTransactionAsync()` to ensure strict transactional boundaries."

---

## 📝 Puntos clave para recordar

- UoW = atomicidad (ACID 'A'): todas las operaciones se confirman o ninguna.
- `DbContext` + Change Tracker + `SaveChangesAsync()` = UoW nativo.
- Transacciones explícitas (`BeginTransactionAsync`) para coordinar múltiples `SaveChangesAsync` o Stored Procedures externos.
- Anti-patrón: llamar `SaveChanges()` dentro de cada repositorio (rompe la atomicidad).
- Interfaz `IUnitOfWork` solo cuando necesitas compartir contexto entre repositorios en Clean Architecture.