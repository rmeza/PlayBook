# 🛠️ Event Sourcing & Proyecciones (Read Models)

**Dominio de referencia:** Dental Clinic (Odontograma / Historia Clínica)

---

## 1. El Reto: Consistencia Eventual (Eventual Consistency)

Cuando separas el lado de escritura (Event Store) del lado de lectura (Read Models en SQL/Mongo), la sincronización no siempre ocurre en la misma transacción síncrona.

```mermaid
flowchart LR
    C[Cliente] -->|Command| AR[Aggregate Root] --> ES[Event Store (Appended)]
    ES -.Asynchronous Event Listener.-> P[Update Read Model (SQL)]
```

### Estrategias de Sincronización

1. **In-Process (Síncrono):** El evento se despacha en la misma transacción HTTP/SQL antes de responder al cliente.
   - Pro: Consistencia inmediata (Strong Consistency).
   - Contra: Si falla la actualización de la proyección, falla el Command. Menor rendimiento.

2. **Out-of-Process / Outbox Pattern (Asíncrono):** El evento se guarda en la base de datos junto con el cambio de estado (o log) y un worker en segundo plano (ej. Worker Service, RabbitMQ, Kafka) procesa las proyecciones.
   - Pro: Altísimo rendimiento y alta disponibilidad.
   - Contra: Existe un pequeño desfase (milisegundos a segundos) en la proyección (Eventual Consistency).

---

## 2. Definición Técnica: Un Projector (Read Model Generator)

Un **Projector** es un manejador de eventos especializado en escuchar la secuencia inmutable de eventos y actualizar la vista de lectura denormalizada.

```csharp
// Read Model denormalizado optimizado para consultas directas en UI
public class PatientOdontogramReadModel
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; }
    public Dictionary<int, string> ToothStatuses { get; set; } = new();
    public decimal TotalTreatmentCost { get; set; }
    public DateTime LastUpdated { get; set; }
}
```

```csharp
// Handler / Projector de Eventos para mantener la vista de lectura
public class OdontogramProjectionHandler :
    INotificationHandler<PatientRegisteredDomainEvent>,
    INotificationHandler<ToothTreatmentAppliedDomainEvent>
{
    private readonly IDocumentSession _readSession; // Ej. Marten, MongoDB o DbContext

    public OdontogramProjectionHandler(IDocumentSession readSession)
    {
        _readSession = readSession;
    }

    public async Task Handle(PatientRegisteredDomainEvent @event, CancellationToken ct)
    {
        var model = new PatientOdontogramReadModel
        {
            PatientId = @event.PatientId,
            PatientName = @event.FullName,
            LastUpdated = @event.OccurredOn
        };

        _readSession.Store(model);
        await _readSession.SaveChangesAsync(ct);
    }

    public async Task Handle(ToothTreatmentAppliedDomainEvent @event, CancellationToken ct)
    {
        var model = await _readSession.LoadAsync<PatientOdontogramReadModel>(@event.PatientId, ct);

        if (model != null)
        {
            model.ToothStatuses[@event.ToothNumber] = @event.NewStatus.ToString();
            model.TotalTreatmentCost += @event.Cost;
            model.LastUpdated = @event.OccurredOn;

            _readSession.Update(model);
            await _readSession.SaveChangesAsync(ct);
        }
    }
}
```

---

## 3. Manejo de Desafíos en Event Sourcing

En producción, implementar Event Sourcing requiere resolver dos problemas fundamentales:

### 1. Snapshots (Capturas de Estado)

- **Problema:** Si un Agregado tiene 10,000 eventos acumulados en 5 años, reconstruirlo leyendo evento por evento toma demasiado tiempo.
- **Solución:** Cada N eventos (ej. cada 100 eventos), se guarda un **Snapshot** (una foto del estado actual). Al hidratar el Agregado, se carga la última foto y solo se aplican los eventos posteriores.

### 2. Evolución de Eventos (Versioning / Upcasters)

- **Problema:** Un evento guardado en 2024 tenía la estructura `{ ToothNumber, Cost }`, pero en 2026 la regla de negocio exige `{ ToothNumber, Cost, Currency, DentistId }`.
- **Solución:** Se implementan **Upcasters**, componentes que transforman en memoria la estructura JSON antigua de un evento a la nueva versión antes de rehidratar el Agregado.

---

## 🎤 Respuesta Senior (English)

> **Q:** *"How do you handle projections and eventual consistency in Event Sourcing?"*
>
> **A:** "In Event Sourcing, projections transform the stream of immutable events into optimized Read Models. Since event processing can be asynchronous using patterns like Outbox or message brokers, the Read Side operates under eventual consistency. To optimize aggregate hydration from long event streams, we use **Snapshots** every N events. For handling schema changes over time without mutating past events, we employ **Upcasters** to map legacy event payloads to modern domain definitions in memory."

---

## 📝 Puntos clave para recordar

- Projections = transformar eventos inmutables en Read Models optimizados.
- Consistencia eventual cuando la proyección es asíncrona (Outbox / brokers).
- Snapshots cada N eventos para hidratación rápida de Agregados largos.
- Upcasters para migrar payloads viejos sin mutar eventos pasados.