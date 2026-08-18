# ⚡ `IQueryable` vs `IEnumerable` & SQL Pushdown Execution

**Dominio de referencia:** Dental Clinic / Sistema Dental

---

## 🎯 Executive Summary

Análisis técnico y profundo sobre la diferencia entre **`IQueryable<T>`** y **`IEnumerable<T>`** en Entity Framework Core. Se detalla el impacto en memoria RAM, tiempo de CPU, la compilación de *Expression Trees* y cómo garantizar que el filtrado, ordenamiento y paginación se ejecuten en el motor de **SQL Server (Pushdown Execution)** para evitar filtrados masivos en memoria (*Memory Leaks* / *OutOfMemoryException*).

---

## 3.1 Anatomía Técnica y Diferencias Fundamentales

La diferencia crítica radica en **DÓNDE y CUÁNDO** se procesan las operaciones LINQ:

- **`IQueryable<T>` (Evaluación en Servidor / Server-Side):**
  - Modifica un **Árbol de Expresiones (`Expression Tree`)** en C#.
  - **No ejecuta nada en la base de datos** mientras se encadenan métodos como `.Where()`, `.OrderBy()`, `.Skip()` o `.Take()`.
  - Traduce todo el pipeline LINQ a **una sola consulta SQL optimizada** al momento de materializar.

- **`IEnumerable<T>` (Evaluación en Cliente / Client-Side):**
  - Trabaja con **delegados C# (`Func<T, bool>`)** sobre objetos ya cargados en la memoria RAM del proceso .NET.
  - Si conviertes un `DbSet` o `IQueryable` a `IEnumerable` antes de filtrar, obligas a EF Core a traer **TODOS los registros de la tabla desde la base de datos hacia la aplicación** para luego filtrarlos localmente.

```mermaid
flowchart TD
    subgraph IQueryable_Server_Side [IQueryable - SQL Server Pushdown Execution]
        A[LINQ Query Expression Tree] -->|EF Core Provider Translation| B[SQL Engine Server]
        B -->|WHERE + JOIN + OFFSET/FETCH| C[Filter & Paginate in Database]
        C -->|Network Transfer| D[Return ONLY 10 Rows to RAM]
    end

    subgraph IEnumerable_Client_Side [IEnumerable - In-Memory Processing / Anti-Pattern]
        E[LINQ Query / ToListAsync] -->|Fetch Entire Table| F[SQL Engine Server]
        F -->|Network Transfer| G[Return ALL 100,000 Rows]
        G -->|Load into App Process| H[RAM Spike / High Garbage Collection]
        H -->|C# In-Memory Filter| I[Return 10 Rows to UI]
    end
```

---

## 3.2 Ejemplos de Código

**❌ Anti-Patrón In-Memory Filtering** — `.ToList()` antes de filtrar/paginar:

```csharp
var all = await context.Patients.ToListAsync();
var filtered = all.Where(p => p.Name.FirstName.Contains("Carlos")).Skip(10).Take(10);
// Trae la tabla completa a RAM: OutOfMemoryException en tablas grandes
```

**✅ SQL Pushdown** — LINQ traducido a `WHERE`, `LIKE` y `OFFSET...FETCH`:

```csharp
var filtered = await context.Patients.AsNoTracking()
    .Where(p => p.Name.FirstName.Contains("Carlos"))
    .Skip(10).Take(10)
    .Select(p => new PatientDto(p.Id, p.Name.FirstName + " " + p.Name.LastName))
    .ToListAsync();
```

---

## 🎤 Respuesta Senior (English)

> **Q:** *"What's the difference between IQueryable and IEnumerable, and why does it matter for performance?"*
>
> **A:** "`IQueryable` builds an expression tree that gets translated into a single optimized SQL query at materialization time — filtering, sorting, and pagination all execute inside SQL Server. `IEnumerable` operates on objects already loaded into application memory using compiled C# delegates. Converting an `IQueryable` to `IEnumerable` before filtering forces EF Core to pull the entire table into RAM first, which is a common cause of `OutOfMemoryException` on large tables. On the read side of CQRS, I strictly enforce `IQueryable` composition with `.AsNoTracking().Select(...)`, ensuring SQL engine pushdown with minimal RAM overhead."

---

## 📝 Puntos clave para recordar

- `IQueryable` = *Expression Tree* → se traduce a SQL y se evalúa en el servidor.
- `IEnumerable` = delegados C# sobre objetos ya en RAM → evaluación en cliente.
- Convertir a `IEnumerable`/`ToList()` antes de filtrar = anti-patrón que rompe el pushdown.
- `AsNoTracking()` + `.Select()` + `Skip/Take` = consulta mínima, sin Memory Leaks.
- Relacionado: estrategias N+1 en `03-EFCore-Performance/01-NPlusOne-Optimization.md`.