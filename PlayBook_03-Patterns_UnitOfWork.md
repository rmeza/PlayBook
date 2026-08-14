# 03-Patterns / Unit of Work Pattern

---

## 1. Propósito

Garantizar que múltiples operaciones en repositorios individuales se confirmen en **una única transacción atómica (ACID)**.

---

## 2. Código C#

```csharp
public interface IUnitOfWork : IDisposable
{
    IOrderRepository Orders { get; }
    ICustomerRepository Customers { get; }
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
```

---

## 3. 🎤 Respuesta de Entrevista en Inglés (Senior/Staff Level)

> **Interviewer:** *"How does Unit of Work ensure transactional integrity in .NET?"*

> **Your Answer:**  
> "The Unit of Work pattern coordinates writes across multiple repositories sharing the same `DbContext` session. Instead of each repository calling `SaveChangesAsync()` individually—which risks partial writes—the application handler executes business logic across multiple repositories and issues a single `CommitAsync()` at the end. This guarantees atomic, transactional integrity across the operation."