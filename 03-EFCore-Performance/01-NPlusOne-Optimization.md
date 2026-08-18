# 🗄️ EF Core Performance: El Problema N+1

**Dominio de referencia:** Dental Clinic / Sistema Dental

---

## 🎯 Executive Summary

El problema **N+1** es la causa #1 de degradación de rendimiento en aplicaciones .NET con EF Core. Ocurre al acceder a propiedades de navegación en bucle, disparando `1 + N` consultas SQL. La solución pasa por **reducir el I/O de red** y forzar la **evaluación en el motor SQL (Server Pushdown)**.

---

## 2.1 El Problema N+1

Se dispara **1 consulta inicial** y luego **N consultas en bucle** por acceder perezosamente a propiedades de navegación:

```
Consulta 1:  SELECT * FROM Appointments WHERE ScheduledAt = hoy     → N citas
Consulta 2..N: SELECT * FROM Patients WHERE Id = @Appointment.PatientId  (xN veces)
```

---

## 2.2 Cuadro Comparativo de Estrategias

| Métrica | Lazy Loading (N+1) ❌ | Eager Loading (`.Include`) 🟡 | DTO Projection (`.Select`) 🟢 |
|---|---|---|---|
| Consultas SQL | 1 + N | 1 Query | 1 Query |
| Hops de Red | N+1 Viajes (Lento) | 1 Viaje | 1 Viaje (Mínimo) |
| Change Tracker | Habilitado | Habilitado | Deshabilitado (`AsNoTracking`) |
| Campos traídos | SELECT * (sobrecarga) | SELECT * (sobrecarga) | Solo los del DTO |

---

## 2.3 `IQueryable` vs `IEnumerable` (Pushdown en SQL)

**❌ Mal (`IEnumerable`):** Carga todos los registros a la RAM antes de filtrar. Causa `OutOfMemoryException` en tablas grandes.

```csharp
// FILTRADO EN RAM (INCORRECTO)
var all = await context.Patients.ToListAsync();
var filtered = all.Where(p => p.Name.FirstName.Contains("Carlos")).Skip(10).Take(10);
```

**✅ Bien (`IQueryable`):** Traduce la consulta a SQL `WHERE`, `OFFSET` y `FETCH NEXT`.

```csharp
// FILTRADO EN SQL SERVER (OPTIMO)
var filtered = await context.Patients.AsNoTracking()
    .Where(p => p.Name.FirstName.Contains("Carlos"))
    .Skip(10).Take(10)
    .Select(p => new PatientDto(p.Id, p.Name.FirstName + " " + p.Name.LastName))
    .ToListAsync();
```

> Detalle técnico completo en `03-EFCore-Performance/02-IQueryable-vs-IEnumerable-Pushdown.md`.

---

## 2.4 Solución al Problema N+1 (Proyección DTO)

```csharp
public async Task<List<TodayAppointmentDto>> GetTodayAppointmentsOptimizedAsync(DbContext context)
{
    return await context.Set<Appointment>()
        .AsNoTracking()
        .Where(a => a.ScheduledAt.Date == DateTime.UtcNow.Date)
        .Select(a => new TodayAppointmentDto(
            a.Id, a.ScheduledAt,
            a.Patient.Name.FirstName + " " + a.Patient.Name.LastName,
            a.Patient.Contact.Email
        )).ToListAsync();
}
```

---

## 🎤 Guía de Entrevistas

### 3.1 Respuesta de 30 Segundos (Senior)

> "The N+1 problem occurs when an initial query fetches N parent records, and subsequent code accesses navigation properties triggering N additional database queries in a loop. I prevent it using DTO Projections via `.Select()` and Eager Loading via `.Include()`, while disabling Lazy Loading across the `DbContext`."

### 3.2 Respuesta de 2 Minutos (Staff / Architect)

> "We set `ChangeTracker.LazyLoadingEnabled = false` across our DbContext to fail fast in integration tests. On the read side of CQRS, we strictly enforce `IQueryable` composition with `.AsNoTracking().Select(...)`, ensuring SQL engine pushdown (JOINs, WHERE, OFFSET/FETCH) with minimal RAM overhead."

---

## 📝 Puntos clave para recordar

- N+1 = 1 consulta + N consultas en bucle por navegación perezosa.
- Evitar `Lazy Loading` en producción; deshabilitarlo para fallar rápido en tests.
- `.Select()` a DTO con `AsNoTracking()` = 1 consulta SQL, mínima RAM, sin Change Tracker.
- Nunca `.ToList()` antes de filtrar o paginar: obliga a traer la tabla completa a RAM.