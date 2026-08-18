# 🎤 Senior & Staff Engineer Interview Answers (English)

Índice rápido de Q&A en inglés para entrevistas. Las respuestas completas viven en cada archivo de tema (enlaces abajo). Solo se mantiene aquí el texto íntegro de preguntas sin archivo dedicado.

---

## 📑 Índice de Preguntas

| # | Pregunta | Respuesta completa |
|---|---|---|
| 1 | How do you apply SOLID principles in your day-to-day .NET architecture? | `04-Architecture/01-SOLID.md` |
| 2 | What is a Captive Dependency and how do you avoid it in .NET? | `04-Architecture/03-DependencyInjection.md` |
| 3 | Why Clean Architecture over traditional N-Tier architecture? | `04-Architecture/02-CleanArchitecture.md` |
| 4 | Since EF Core already implements DbSet as a Repository, isn't adding another Repository layer redundant? | `04-Architecture/04-Repository-Pattern.md` |
| 5 | When does CQRS add real value vs over-engineering? And how do you handle transactional boundaries? | *(respuesta completa abajo — única)* |
| 6 | Difference between IQueryable and IEnumerable, and why it matters for performance | `03-EFCore-Performance/02-IQueryable-vs-IEnumerable-Pushdown.md` |
| 7 | What is the N+1 problem and how do you prevent it in EF Core? | `03-EFCore-Performance/01-NPlusOne-Optimization.md` |
| 8 | Interfaces / Abstract Classes / Interface vs Abstract (Q&A completo en inglés) | `07-Interview/01-Pitch-Sheet-Senior.md` (Módulo B) |

---

## 5. CQRS Trade-offs & Transactional Boundaries

**Question:** *"When does CQRS add real value vs over-engineering? And how do you handle transactional boundaries?"*

> **Answer:**  
> "In my experience, jumping straight into CQRS with separate read/write models for simple CRUD domains is **over-engineering**. If the business rules are straightforward, standard EF Core with clean service boundaries is often enough.
>
> However, CQRS adds massive value when there is a clear **asymmetry between read and write workloads**. For writes, we use EF Core to enforce strong domain constraints and transactional integrity using the **Unit of Work** pattern within a single command handler. For reads, we bypass the domain model entirely and use **Dapper or lightweight EF Core projections (`AsNoTracking`)** directly to DTOs for maximum performance.
>
> Regarding **transactional boundaries**: Commands always execute inside an atomic transaction bound to a single Aggregate Root. If we need to propagate side effects, we publish **Domain Events via MediatR**, using the **Outbox Pattern** if we need eventual consistency across microservices."