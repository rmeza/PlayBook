# 🧠 CQRS vs. Event Sourcing: Desmitificando la relación

**Dominio de referencia:** Dental Clinic (Sistema Dental)

---

## 1. ¿Cuál es la diferencia?

En muchas entrevistas para roles de Arquitecto, se comete el error de asumir que CQRS y Event Sourcing son lo mismo o que siempre deben ir juntos.

| | CQRS | Event Sourcing |
|---|---|---|
| **Tipo de patrón** | Arquitectura de software | Persistencia de datos |
| **Qué hace** | Separa la vía de modificación de estado (Commands) de la vía de consulta (Queries) | En lugar de guardar el **estado actual** de una entidad en una fila, guarda la **secuencia inmutable de todos los eventos** que le han ocurrido desde su creación |
| **Soporte** | Se implementa perfectamente sobre una base relacional tradicional (SQL Server) con tablas normadas | Requiere un Event Store (solo INSERT, append-only) |

---

## 2. Caso Práctico en el Sistema Dental

### A) Persistencia Tradicional (Estado Actual)

Cuando el dentista aplica un tratamiento, la fila en SQL Server cambia directamente:

```sql
-- Fila en la tabla ToothConditions
-- Id: "d3b07384..." | PatientId: "p1" | ToothNumber: 18 | Status: "Filled" | LastUpdated: "2026-08-11"
```

**Problema:** Perdiste el historial. No sabes qué estado tenía el diente antes de hoy a menos que mantengas tablas de auditoría manuales.

### B) Persistencia con Event Sourcing (Event Store)

En lugar de una tabla `ToothConditions`, tienes un **Event Stream inmutable** (solo INSERT):

| Sequence | AggregateId | EventType | Payload | Timestamp |
|---|---|---|---|---|
| 1 | Patient-101 | PatientRegistered | { Name: "Carlos" } | 2024-01-10 |
| 2 | Patient-101 | CariesDiagnosed | { ToothNumber: 18 } | 2025-05-12 |
| 3 | Patient-101 | ToothTreatmentApplied | { ToothNumber: 18, Status: "Filled" } | 2026-08-11 |

Para saber el estado actual del diente 18 del paciente, **reproduces (re-play)** todos los eventos en orden cronológico.

---

## 3. ¿Por qué Event Sourcing y CQRS hacen una combinación poderosa?

Si guardas solo una lista infinita de eventos, hacer una consulta como *"Tráeme la lista de todos los pacientes que tienen caries actualmente"* requeriría leer millones de eventos en RAM. Sería extremadamente lento.

Aquí es donde entra CQRS:

1. **Write Side (Event Store):** Los Commands guardan eventos inmutables en una base de datos optimizada para escritura rápida (ej. EventStoreDB, CosmosDB).
2. **Read Side (Read Model / Projections):** Un proceso de fondo escucha esos eventos y actualiza tablas SQL o Documentos Mongo optimizados exclusivamente para lectura instantánea (Read Projections).

---

## 🎤 Respuesta Senior (English)

> **Q:** *"Is Event Sourcing required to implement CQRS, and what are the trade-offs?"*
>
> **A:** "No, Event Sourcing is not required for CQRS. CQRS is an architectural pattern that separates read and write pathways, which can easily run on top of a standard relational database. Event Sourcing, on the other hand, is a persistence strategy where state is stored as an append-only log of immutable events rather than mutating current-state records. While combining them provides auditability, temporal querying, and high throughput on writes, it introduces significant complexity — such as eventual consistency, snapshotting for long streams, and schema evolution handling. In most enterprise applications, starting with CQRS on a traditional relational database is the pragmatic approach, reserving Event Sourcing only for domains where audit trails are a core business requirement."

---

## 📝 Puntos clave para recordar

- CQRS = patrón de **arquitectura**; Event Sourcing = estrategia de **persistencia**.
- CQRS funciona perfectamente sobre SQL tradicional sin Event Sourcing.
- Event Sourcing = append-only log de eventos inmutables (auditoría completa).
- Combinados: Write Side (Event Store) + Read Side (Projections), con consistencia eventual.
- Uso pragmático: reserva Event Sourcing para dominios donde la auditoría es requisito central.

> Detalle técnico de proyecciones en `05-Patterns-CQRS/04-EventSourcing-Projections.md`.