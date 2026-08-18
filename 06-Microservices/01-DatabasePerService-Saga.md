# 📦 Microservicios: Database per Service & Sagas

**Dominio de referencia:** Sistema Dental (Appointments, Billing, Clinical)

---

## 01. De Bounded Contexts a Microservicios: Database per Service

Uno de los errores más comunes en la transición a microservicios es mantener una **base de datos compartida (Shared Database)**. Si dos servicios leen y escriben en las mismas tablas SQL, se acoplan en el esquema de datos, perdiendo **independencia de despliegue y escalabilidad**.

### El Patrón Database per Service

Cada microservicio debe ser **dueño absoluto de sus datos**. Ningún microservicio puede acceder directamente a la base de datos de otro. Toda interacción debe ocurrir a través de **APIs públicas (REST/gRPC)** o **eventos asíncronos**.

```mermaid
graph LR
    subgraph Appointments_MS["Microservicio de Citas (Appointments)"]
        AppDB[(SQL Server: AppointmentsDb)]
    end
    subgraph Billing_MS["Microservicio de Facturación (Billing)"]
        BillDB[(PostgreSQL: BillingDb)]
    end
    Appointments_MS <-->|REST / Eventos (RabbitMQ)| Billing_MS
```

### Límites de Dominio (Dental Bounded Contexts)

1. **Clinical / Odontogram Domain:** Gestiona historias clínicas, piezas dentales, diagnósticos y tratamientos. (Alta complejidad de negocio, baja concurrencia).
2. **Scheduling / Appointments Domain:** Gestiona la agenda de dentistas, salas y citas. (Alta concurrencia, lectura intensiva).
3. **Billing / Financial Domain:** Gestiona facturas, cobros y planes de pago. (Consistencia estricta, integraciones de pago).

---

## 02. Transacciones Distribuidas: El Patrón Saga

Al eliminar las bases de datos compartidas, **perdemos las transacciones ACID relacionales** (`BEGIN TRANSACTION ... COMMIT`).

**Caso de Uso:** Agendar Cita + Reservar Tratamiento + Generar Cobro.

En una arquitectura relacional monolítica, esto se solucionaba en una sola transacción SQL. En microservicios, necesitamos garantizarlo mediante **Consistencia Eventual** utilizando el **Patrón Saga**.

> Una **Saga** es una secuencia de transacciones locales. Cada transacción local actualiza la base de datos de un microservicio y publica un evento. Si una transacción local falla, la Saga ejecuta **Pasos Compensatorios (Compensating Transactions)** para revertir los cambios de los pasos anteriores.

### Tipos de Saga: Orquestada vs. Coreografiada

| Característica | Coreografía (Choreography) | Orquestación (Orchestration) |
|---|---|---|
| Mecanismo | Basado en eventos. Los servicios reaccionan a eventos de otros | Un componente central (Orchestrator) dirige el flujo |
| Acoplamiento | Muy bajo. Los servicios solo conocen sus eventos | Moderado. El orquestador conoce todos los pasos |
| Complejidad | Difícil de rastrear y depurar en flujos complejos | Centralizada y fácil de visualizar/auditar |
| Uso Recomendado | Flujos simples de 2 a 3 pasos | Flujos de negocio complejos con múltiples compensaciones |

---

## 03. Implementación de una Saga Orquestada en C# (MassTransit)

En el ecosistema .NET, el framework estándar de la industria para implementar Sagas es **MassTransit** utilizando **State Machines** sobre brokers como RabbitMQ o Azure Service Bus.

**Ejemplo:** Estado y Flujo de la Saga `AppointmentBookingSaga`.

```csharp
using MassTransit;

// Estado persistido de la Saga (State Machine Instance)
public class AppointmentSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public Guid PatientId { get; set; }
    public Guid AppointmentId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

```csharp
// Definición de la Máquina de Estados
public class AppointmentBookingSaga : MassTransitStateMachine<AppointmentSagaState>
{
    // Estados
    public State Scheduled { get; private set; }
    public State Billed { get; private set; }

    // Eventos
    public Event<AppointmentRequestedEvent> AppointmentRequested { get; private set; }
    public Event<InvoiceCreatedEvent> InvoiceCreated { get; private set; }
    public Event<InvoiceCreationFailedEvent> InvoiceCreationFailed { get; private set; }

    public AppointmentBookingSaga()
    {
        InstanceState(x => x.CurrentState);

        // Correlación de eventos basada en un Identificador Único de Saga
        Event(() => AppointmentRequested, x => x.CorrelateById(m => m.Message.SagaId));
        Event(() => InvoiceCreated, x => x.CorrelateById(m => m.Message.SagaId));
        Event(() => InvoiceCreationFailed, x => x.CorrelateById(m => m.Message.SagaId));

        Initially(
            When(AppointmentRequested)
                .Then(context =>
                {
                    context.Saga.PatientId = context.Message.PatientId;
                    context.Saga.AppointmentId = context.Message.AppointmentId;
                    context.Saga.Amount = context.Message.Amount;
                })
                .Publish(context => new CreateInvoiceCommand(context.Saga.CorrelationId, context.Saga.PatientId, context.Saga.Amount))
                .TransitionTo(Scheduled)
        );

        During(Scheduled,
            When(InvoiceCreated)
                .TransitionTo(Billed)
                .Then(context => Console.WriteLine($"Saga completada con éxito para la cita: {context.Saga.AppointmentId}")),
            // Acción Compensatoria si falla la facturación
            When(InvoiceCreationFailed)
                .Publish(context => new CancelAppointmentCommand(context.Saga.AppointmentId, "Falló la generación de factura"))
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }
}
```

---

## 🎤 Respuesta Senior (English)

> **Q:** *"How do you handle distributed transactions and consistency across Microservices?"*
>
> **A:** "In a microservices architecture, each service strictly owns its database to ensure autonomy. To manage business processes spanning multiple services without 2-Phase Commit protocols, we use the **Saga Pattern**. We implement Orchestrated Sagas using **MassTransit State Machines** for complex workflows where explicit state tracking and compensating transactions are required. If a step fails — such as a payment or invoice generation — the orchestrator emits compensating commands to revert previous states, guaranteeing eventual consistency across the system."

---

## 📝 Puntos clave para recordar

- Database per Service: cada microservicio es dueño absoluto de sus datos.
- Sin base compartida no hay ACID distribuido → Sagas + consistencia eventual.
- Coreografía (eventos, simple) vs Orquestación (central, compleja).
- MassTransit State Machines = estándar para Sagas en .NET.
- Pendiente (roadmap): Transactional Outbox & Inbox, API Gateway, Resilience (Polly), Service Discovery, Observabilidad.