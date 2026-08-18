# 🎯 Delegados Genéricos en C#: `Func<>`, `Action<>` y `Predicate<>`

**Dominio de referencia:** Dental Clinic (Sistema Dental)

---

## 🎯 Concepto

Un **Delegado** en C# es un tipo de dato fuertemente tipado que almacena una referencia a un método. Te permite **pasar métodos como parámetros** a otras funciones (programación funcional / Higher-Order Functions).

Antes de C# 3.0, tenías que declarar delegados personalizados a mano (`delegate void MyDelegate(...)`). Con la llegada de `Func<>` y `Action<>`, esto se estandarizó en todo el framework.

---

## 1. `Action<T...>` (Métodos que NO devuelven nada → `void`)

`Action` representa un método que acepta de **0 a 16 parámetros** y devuelve `void`.

**Uso principal:** Notificaciones, callbacks, ejecutar acciones colaterales, pipelines de configuración (`builder => builder.UseX()`).

```csharp
// Acción simple sin parámetros
Action logHeader = () => Console.WriteLine("=== SISTEMA DENTAL ===");

// Acción con parámetros (Input: PatientId, DentalStatus) -> Output: void
Action<Guid, string> logTreatmentToAudit = (patientId, status) =>
{
    Console.WriteLine($"Audit Log: Patient {patientId} updated to status {status}");
};

// Invocar la acción
logHeader();
logTreatmentToAudit(Guid.NewGuid(), "Cleaning Completed");
```

---

## 2. `Func<T..., TResult>` (Métodos que SI devuelven un valor)

`Func` representa un método que acepta de **0 a 16 parámetros de entrada** y **SIEMPRE devuelve un valor**. El último tipo genérico especificado es el tipo de retorno.

> Fórmula: `Func<Parametro1, ..., ParametroN, TipoDeRetorno>`

**Uso principal:** Proyecciones, transformaciones, LINQ (`.Select()`, `.Where()`), cálculos financieros o de negocio.

```csharp
// Func sin parámetros de entrada, devuelve decimal: () => decimal
Func<decimal> getEmergencyBaseFee = () => 150.00m;

// Func con 2 entradas (BasePrice, Discount) y 1 retorno (FinalPrice)
Func<decimal, decimal, decimal> calculateDiscountedPrice = (price, discountPercent) =>
{
    return price - (price * (discountPercent / 100));
};

decimal finalAmount = calculateDiscountedPrice(200.00m, 15.0m); // Retorna 170.00
```

---

## 3. `Predicate<T>` (Casos especiales de evaluación Booleana)

Es equivalente a `Func<T, bool>`. Acepta un objeto de tipo `T` y devuelve `bool` (true/false). Se utiliza internamente en métodos de colecciones como `List<T>.Find()`, `List<T>.RemoveAll()`.

```csharp
Predicate<int> isToothAdultNumber = toothNumber => toothNumber >= 11 && toothNumber <= 48;
bool isValid = isToothAdultNumber(18); // true
```

---

## 🆚 Cuadro Comparativo Express

| Delegado | ¿Devuelve valor? | Firma Típica | Caso de Uso Principal en C# |
|---|---|---|---|
| `Action` | ❌ No (void) | `Action<T1, T2>` | Callbacks, logging, middleware `options => { ... }` |
| `Func` | ✅ Sí (Cualquier tipo) | `Func<T1, T2, TResult>` | Expresiones LINQ, reglas de cálculo, mapeos |
| `Predicate` | ✅ Sí (Siempre bool) | `Predicate<T>` | Filtros en memoria, validaciones rápidas |

---

## 🏛️ Patrones Avanzados de Arquitectura usando Func y Action

Como Arquitecto, no usas `Func` y `Action` solo para lambdas simples de LINQ, sino para diseñar **patrones de diseño reutilizables**:

### A. Patrón Decorador / Resilience Wrapper (Interceptores)

Puedes usar `Func<Task<T>>` para envolver ejecuciones de código en bloques de **Retry, Logging o Manejo de Transacciones**:

```csharp
public class TransactionExecutor
{
    private readonly ApplicationDbContext _db;

    public TransactionExecutor(ApplicationDbContext db) => _db = db;

    // Recibe cualquier función asíncrona y la ejecuta dentro de una transacción SQL
    public async Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<Task<TResult>> actionToExecute,
        CancellationToken ct)
    {
        using var transaction = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            // Ejecutamos la función pasada por parámetro
            TResult result = await actionToExecute();

            await transaction.CommitAsync(ct);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
```

---

## ⚡ Function Handlers vs. Azure Functions / Serverless

### 1. In-Process Function Handlers (C# Clean Architecture / CQRS)

Es la abstracción donde un objeto o función C# procesa un solo evento o comando.

- **MediatR:** `IRequestHandler<TRequest, TResponse>` implementa un método `Handle()` que internamente actúa como una `Func<TRequest, Task<TResponse>>`.
- **Desacoplamiento:** La capa web (API Controllers / Minimal APIs) no sabe cómo se procesa la solicitud, solo se la delega al Handler.

### 2. Serverless Function Handlers (Azure Functions / AWS Lambda)

En arquitectura cloud Serverless, la "Función" es la unidad mínima de despliegue.

- **Triggers:** Una Azure Function responde a eventos externos (Petición HTTP, mensaje en RabbitMQ/ServiceBus, cron job).
- **Handler:** El método decorado con `[Function]` actúa como el Handler de entrada.

```csharp
// Azure Function Handler (Serverless)
public class ProcessAppointmentPaymentFunction
{
    private readonly IMediator _mediator;

    public ProcessAppointmentPaymentFunction(IMediator mediator) => _mediator = mediator;

    [Function("ProcessAppointmentPayment")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
        FunctionContext executionContext)
    {
        var command = await req.ReadFromJsonAsync<ProcessPaymentCommand>();

        // Delegamos directamente al Handler interno de CQRS
        var result = await _mediator.Send(command);

        return new OkObjectResult(result);
    }
}
```

---

## 🎤 Respuesta Senior (English)

> **Q:** *"What is the difference between Func and Action in C#, and how do you leverage them in architecture?"*
>
> **A:** "In C#, `Action` and `Func` are built-in generic delegates. An `Action` represents a method that returns void, typically used for callbacks, side-effects, and configuration pipelines. A `Func` represents a method that returns a value, making it the foundation for LINQ queries, transformations, and functional patterns. Architecturally, we leverage `Func` to build dynamic execution wrappers — such as generic transaction handlers or resiliency execution pipelines using Polly — allowing us to inject behavior cleanly without violating the Open/Closed Principle."

---

## 📝 Puntos clave para recordar

- `Action<T...>` → `void` (callbacks, side-effects, config).
- `Func<T..., TResult>` → devuelve valor (LINQ, cálculos, mapeos).
- `Predicate<T>` → `Func<T, bool>` (filtros y validaciones).
- `Func<Task<T>>` habilita wrappers de transacción / resilience (Polly).
- Handlers de MediatR internamente son `Func<TRequest, Task<TResponse>>`.