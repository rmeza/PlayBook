# 🎤 Senior & Staff Engineer Interview Answers (English)

This document compiles high-impact, pragmatic responses for Senior and Staff Engineer technical interviews in .NET 8, Clean Architecture, CQRS, and Cloud.

---

## 1. SOLID Principles in Practice
**Question:** *"How do you apply SOLID principles in your day-to-day .NET architecture?"*

> **Answer:**  
> "In my daily workflow, SOLID principles are key to maintaining a clean and testable codebase. For instance, I enforce **SRP** by keeping controllers and command handlers thin, delegating business rules to domain services or aggregates. I rely heavily on **DIP** through Dependency Injection, allowing us to swap persistence implementations—like switching from EF Core to Dapper for performance-critical queries—without touching core business logic. Finally, I apply **ISP** to design small, focused contracts that make unit testing with mocks straightforward and resilient to breaking changes."

---

## 2. Dependency Lifetimes & Captive Dependencies
**Question:** *"What is a Captive Dependency and how do you avoid it in .NET?"*

> **Answer:**  
> "A Captive Dependency occurs when a service with a longer lifetime holds onto a service with a shorter lifetime—for example, when a **Singleton** service injects a **Scoped** service like `DbContext`. This keeps the scoped object alive indefinitely, leading to memory leaks and thread-safety bugs. To avoid this, I use the `IServiceScopeFactory` to explicitly create a scope inside the Singleton when I need to interact with scoped resources."

---

## 3. Clean Architecture vs N-Tier
**Question:** *"Why Clean Architecture over traditional N-Tier architecture?"*

> **Answer:**  
> "Traditional N-Tier architectures often tightly couple business rules to the database layer because the UI calls the Business Logic Layer, which directly references the Data Access Layer. In **Clean Architecture**, we invert that dependency. The Domain layer sits at the center with zero external dependencies. This ensures that UI frameworks, ORMs, or cloud providers are merely infrastructure details that can be evolved, tested, or swapped without impacting core business rules."

---

## 4. Repository Pattern & EF Core Redundancy
**Question:** *"Since EF Core already implements DbSet as a Repository, isn't adding another Repository layer redundant?"*

> **Answer:**  
> "It depends on the complexity of the domain. While `DbContext` and `DbSet` already implement Unit of Work and Repository patterns, introducing an explicit repository in Clean Architecture prevents EF Core primitives—like `IQueryable`—from leaking into application handlers or controllers. It establishes a clear boundary for pure domain entities and simplifies unit testing with mocks. However, for read-heavy or simple CQRS queries, bypassing custom repositories and querying EF Core or Dapper directly is often a pragmatic, high-performance trade-off."

---

## 5. CQRS Trade-offs & Transactional Boundaries
**Question:** *"When does CQRS add real value vs over-engineering? And how do you handle transactional boundaries?"*

> **Answer:**  
> "In my experience, jumping straight into CQRS with separate read/write models for simple CRUD domains is **over-engineering**. If the business rules are straightforward, standard EF Core with clean service boundaries is often enough.  
>  
> However, CQRS adds massive value when there is a clear **asymmetry between read and write workloads**. For writes, we use EF Core to enforce strong domain constraints and transactional integrity using the **Unit of Work** pattern within a single command handler. For reads, we bypass the domain model entirely and use **Dapper or lightweight EF Core projections (`AsNoTracking`)** directly to DTOs for maximum performance.  
>  
> Regarding **transactional boundaries**: Commands always execute inside an atomic transaction bound to a single Aggregate Root. If we need to propagate side effects, we publish **Domain Events via MediatR**, using the **Outbox Pattern** if we need eventual consistency across microservices."