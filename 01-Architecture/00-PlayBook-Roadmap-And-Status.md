# 🗺️ PlayBook Master: Plan de Estudio y Estado de Avance

---

## 📍 ¿En qué parte vamos?

Actualmente nos encontramos en el **MÓDULO 3**, habiendo completado tanto los **conceptos teóricos y de arquitectura** como la **profundización técnica detallada sobre `IQueryable` vs `IEnumerable`**. 

Tu archivo `03-IQueryable-vs-IEnumerable-Pushdown.md` ya cubre el comportamiento en memoria, la compilación de *Expression Trees* y la optimización de consultas SQL Server.

---

## 🎯 Plan de Estudio Global (Roadmap de Arquitectura .NET 8)

| Módulo | Estado | Descripción del Contenido |
| :--- | :---: | :--- |
| **Módulo 1: Composición vs. Herencia (DDD)** | `[100% COMPLETADO]` | Value Objects (`record`), Agregados, `.ComplexProperty()` de EF Core 8 y mapeo plano en SQL Server sin JOINs. |
| **Módulo 2: EF Core Performance & N+1** | `[100% COMPLETADO]` | Diagnóstico del problema N+1, `AsNoTracking`, proyecciones con `.Select()` y mapa comparativo de estrategias. |
| **Módulo 3: `IQueryable` vs `IEnumerable`** | `[100% COMPLETADO]` | Explicación técnica de *Expression Trees*, evaluación en Servidor vs Cliente y eliminación de *Memory Leaks*. |
| **Módulo 4: CQRS Read/Write Separation** | `[PENDIENTE]` | Separación de pipelines de Lectura (Queries directas a DTO) y Escritura (Commands cargando Agregados). |
| **Módulo 5: Clean Architecture Structure** | `[PENDIENTE]` | Estructura enterprise de la solución `.sln` en .NET 8 y aislamiento de capas sin dependencias circulares. |
| **Módulo 6: Testing & Validation** | `[PENDIENTE]` | Unit Tests de reglas de dominio e Integration Tests para EF Core con Respawn / Testcontainers. |
| **Módulo 7: Domain Events & Outbox Pattern** | `[PENDIENTE]` | Desacoplamiento de lógica asíncrona y garantía de consistencia eventual con el patrón Transactional Outbox. |

---

## 📄 Detalle Extensivo de Temas por Módulo

### 🧠 MÓDULO 1: Composición vs Herencia en DDD
* **1.1 Anti-Patrones de Dominio:** Por qué NO usar clases abstractas `Person` ni interfaces `IPerson`. Problemas con TPH (columnas `NULL`) y TPT (`JOINs` innecesarios).
* **1.2 Principio Has-A (Composición):** Modelado de `Patient` y `Doctor` con Value Objects independientes (`PersonName`, `ContactInfo`).
* **1.3 C# .NET 8 Value Objects:** Implementación con `record` inmutables y validaciones en constructor.
* **1.4 Mapeo Fluent API:** Uso de `.ComplexProperty()` en EF Core 8.
* **1.5 Tabla SQL Resultante:** Estructura de base de datos plana sin tablas hijas para Value Objects.

### 🗄️ MÓDULO 2: EF Core Performance & Diagnóstico N+1
* **2.1 Diagrama de Secuencia N+1:** Cómo se disparan $1 + N$ consultas en bucle por el acceso a propiedades de navegación.
* **2.2 Estrategias de Carga:** Comparación de *Lazy Loading*, *Eager Loading* (`.Include()`) y *DTO Projection* (`.Select()`).
* **2.3 Proyección DTO con `AsNoTracking`:** Reducción de overhead por deshabilitación del *Change Tracker*.
* **2.4 Guía de Entrevistas:** Respuestas estructuradas para roles Senior y Staff / Architect.

### ⚡ MÓDULO 3: `IQueryable` vs `IEnumerable` (Pushdown Execution)
* **3.1 Arquitectura de Expresiones:** Funcionamiento de los *Expression Trees* en C# vs *Delegados IL*.
* **3.2 Server-Side Evaluation vs Client-Side Evaluation:** Impacto en consumo de memoria RAM y CPU al filtrar.
* **3.3 Anti-Patrón In-Memory Filtering:** Errores al usar `.ToList()` antes de filtrar o paginar (`Skip`/`Take`).
* **3.4 SQL Pushdown:** Traducción de LINQ a `WHERE`, `LIKE` y `OFFSET...FETCH` en SQL Server.

### 🔀 MÓDULO 4: CQRS & Separación Lectura/Escritura
* **4.1 Pipeline de Lectura (Read Side):** Bypassing de entidades de dominio. Consultas directas a DTOs con `IQueryable` para máximo rendimiento.
* **4.2 Pipeline de Escritura (Write Side):** Carga estricta de Agregados para asegurar invariantes de negocio antes de persistir.
* **4.3 Patrones con MediatR / C# Handlers:** Implementación limpia en .NET 8.

### 🏗️ MÓDULO 5: Estructura Enterprise de Clean Architecture
* **5.1 Inversión de Dependencias:** Reglas de acoplamiento (Domain $\leftarrow$ Application $\leftarrow$ Infrastructure / WebAPI).
* **5.2 Organización de Archivos:** Estructura modular de carpetas para soluciones enterprise en C#.

---

## 📌 Próximo Paso Sugerido
Ahora que tenemos este roadmap claro y los módulos 1, 2 y 3 cerrados, estamos listos para construir el **Módulo 4: CQRS & Separación Lectura/Escritura** en su respectivo archivo `04-CQRS-Read-Write-Separation.md`.