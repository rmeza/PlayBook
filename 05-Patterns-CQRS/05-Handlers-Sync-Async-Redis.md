# ⚡ Handlers, Asincronía y Caché Distribuida (Redis)

**Dominio de referencia:** Dental Clinic (Sistema Dental)

---

## 01. Function Handlers (Handlers de Funciones / Invocación)

Un **Handler** es una abstracción (generalmente una clase o función) encargada de procesar una **única unidad de trabajo bien definida** dentro del sistema. En arquitecturas modernas (.NET / Clean Architecture / CQRS / Serverless), los Handlers representan la capa de ejecución principal de la lógica de aplicación.

### Tipos de Handlers en el Ecosistema

1. **MediatR / CQRS Handlers:** Reciben un Command o Query y ejecutan la orquestación correspondiente.
2. **Event Handlers:** Reaccionan a un evento de dominio (`INotificationHandler`) o integración (`IConsumer`).
3. **AWS Lambda / Azure Function Handlers:** Punto de entrada cuando se trabaja con arquitectura Serverless.

### Principio Clave: Single Responsibility

Un Handler nunca debe contener lógica de infraestructura directa (armar un SQL a mano o conectar un Socket) ni validar esquemas directamente. Su única responsabilidad es: **recibir la petición, pedir datos a los agregados/repositorios, invocar comportamiento de dominio y retornar una respuesta.**

```csharp
// Handler enfocado: Solo orquesta, el dominio ejecuta la regla
public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, bool>
{
    private readonly IAppointmentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelAppointmentCommandHandler(IAppointmentRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CancelAppointmentCommand command, CancellationToken ct)
    {
        var appointment = await _repository.GetByIdAsync(command.AppointmentId, ct);
        if (appointment == null) throw new NotFoundException();

        // Regla de negocio encapsulada dentro del Agregado
        appointment.Cancel(command.Reason);

        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
```

---

## 02. Synchronous vs. Asynchronous (Sync / Async)

En aplicaciones de alto tráfico en .NET, la diferencia entre código síncrono y asíncrono **no es la velocidad de ejecución de una sola tarea**, sino el **aprovechamiento de hilos del Thread Pool** y la escalabilidad del sistema.

### Operaciones I/O Bound vs. CPU Bound

- **I/O Bound** (Lectura/Escritura de Disco, Base de Datos, Red): Aquí se usa `async / await`. Mientras SQL Server responde, el hilo de .NET se libera para atender a otros usuarios en lugar de quedarse bloqueado esperando.
- **CPU Bound** (Cálculos matemáticos complejos, procesamiento de imágenes): Se ejecutan en hilos secundarios con `Task.Run()` para no bloquear la interfaz o el hilo principal.

```text
SÍNCRONO (Bloqueante):
[Hilo 1] ---> Espera SQL (Inactivo 200ms) ---> Procesa respuesta ---> [Disponible]

ASÍNCRONO (No Bloqueante):
[Hilo 1] ---> Inicia SQL ---> [Hilo 1 Liberado al Thread Pool]
                                ... SQL trabajando ...
[Hilo 2 (cualquiera)] <--- Recibe resultado de SQL <--- Procesa respuesta
```

### Reglas de Oro en .NET

1. **Never use `Task.Result` or `.Wait()`:** Provoca Thread Starvation (agotamiento de hilos) y puede causar Deadlocks.
2. **Usa `CancellationToken`:** Siempre propaga el token hasta la base de datos. Si el usuario cancela la petición HTTP, la consulta a SQL Server se detiene inmediatamente, ahorrando recursos.

---

## 03. Redis (Remote Dictionary Server)

Redis es un almacén de datos clave-valor **en memoria** (*In-Memory Data Structure Store*), de ultra baja latencia (tiempos de respuesta sub-milisegundo). Se utiliza como base de datos en memoria, broker de mensajes o capa de caché.

### Estructuras de Datos Más Utilizadas

- **Strings:** Guarda JSONs serializados (ideal para DTOs de lectura).
- **Hashes:** Guarda objetos estructurados permitiendo actualizar un solo campo sin serializar todo.
- **Sorted Sets (ZSET):** Mantiene listas ordenadas por puntuación (ej. cola de prioridades de urgencias en la clínica).

### Redis vs. In-Memory Cache (MemoryCache)

| Característica | IMemoryCache (Local) | Redis (Distribuido) |
|---|---|---|
| Ubicación | Memoria RAM del proceso de la App | Servidor / Cluster dedicado |
| Velocidad | Nanosegundos (Ultra rápida) | Milisegundos (Latencia de red) |
| Escalabilidad | Limitada a un solo pod/servidor | Compartido entre múltiples pods/instancias |
| Invalidez | Desincronizado si tienes N servidores | Estado único y consistente para todos los nodos |

---

## 04. Cache Patterns & Estrategias de Caching

Implementar caché no es solo guardar datos en RAM; es gestionar el **ciclo de vida del dato** para evitar servir información obsoleta (*Stale Data*).

### Patrón 1: Cache-Aside (Lazy Loading) - El estándar

La aplicación es la responsable de coordinar la caché y la base de datos:

1. La app busca el dato en Redis.
2. **Cache Hit:** Si existe, lo devuelve inmediatamente.
3. **Cache Miss:** Si no existe, lee de la base de datos SQL, guarda el resultado en Redis con un tiempo de expiración (TTL) y lo devuelve.

```csharp
public class GetDentalCatalogQueryHandler : IRequestHandler<GetDentalCatalogQuery, List<TreatmentDto>>
{
    private readonly IDistributedCache _cache;
    private readonly ApplicationDbContext _db;

    public async Task<List<TreatmentDto>> Handle(GetDentalCatalogQuery query, CancellationToken ct)
    {
        string cacheKey = "catalog:treatments";

        // 1. Intentar leer de Redis
        var cachedData = await _cache.GetStringAsync(cacheKey, ct);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<List<TreatmentDto>>(cachedData)!; // Cache Hit
        }

        // 2. Cache Miss: Leer de SQL Server
        var catalog = await _db.Treatments
            .AsNoTracking()
            .Select(t => new TreatmentDto(t.Id, t.Name, t.Price))
            .ToListAsync(ct);

        // 3. Guardar en Redis con expiración de 1 hora (TTL)
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(catalog), options, ct);

        return catalog;
    }
}
```

### Estrategias de Invalidation & TTL

- **Absolute Expiration:** El dato expira estrictamente pasados N minutos/horas.
- **Sliding Expiration:** El tiempo de vida se renueva cada vez que alguien consulta el dato (ideal para sesiones activas).
- **Write-Through / Cache Invalidation:** Cuando un Command modifica los precios de los tratamientos, publica un evento que elimina la clave `catalog:treatments` de Redis inmediatamente (Cache Eviction).

---

## 🎤 Respuesta Senior (English)

> **1. Function Handlers & Clean Architecture**
> "Function Handlers act as the primary orchestration layer in CQRS and Clean Architecture. A handler should be lean and adhere strictly to the Single Responsibility Principle. It receives a Command or Query, fetches the Aggregate from a repository, delegates business invariants to the domain model, and persists the state using Unit of Work without leaking infrastructure details."

> **2. Async / Await & Threading**
> "In .NET, async/await is crucial for I/O-bound operations like database queries and network calls. It prevents thread blocking by returning the execution thread to the Thread Pool while waiting for I/O completion. This maximizes throughput and application scalability under heavy concurrent load. We always pass CancellationTokens to gracefully abort database queries if a client disconnects."

> **3. Redis & Caching Strategies**
> "We use Redis as a distributed cache to maintain consistent state across scaled-out microservice instances. By implementing the Cache-Aside pattern with TTLs, we significantly reduce load on our primary SQL database for read-heavy operations like product or treatment catalogs. For cache invalidation, we emit domain events upon write operations to clear or update affected keys explicitly."

---

## 📝 Puntos clave para recordar

- Handler = unidad de trabajo bien definida; orquesta, no implementa infraestructura.
- `async/await` para I/O-bound (libera el hilo del Thread Pool); `Task.Run()` para CPU-bound.
- Nunca `.Result`/`.Wait()`: Thread Starvation y Deadlocks.
- Redis = caché distribuido compartido entre instancias; MemoryCache = local.
- Cache-Aside + TTL (absolute/sliding) + invalidación por eventos en escrituras.

> Delegados `Func`/`Action`/`Predicate` en `01-OOP-Fundamentals/07-FuncActionPredicate-Delegates.md`.