# 🗺️ PlayBook: Plan de Estudio y Estado de Avance

PlayBook personal para entrevistas técnicas **Senior / Staff .NET** — C#, .NET 8, Clean Architecture, DDD, EF Core, SQL Server, CQRS, Microservices, Cloud & Agentic AI.

**Dominio de referencia:** Dental Clinic / Sistema Dental (Patient, Dentist, Appointment, Odontogram, Billing).

---

## 📂 Estructura del PlayBook

```text
PlayBook/
├── README.md                        <- Dashboard único (este archivo)
├── 00-Meta/
│   ├── Senior_DotNet_Interview_Coach_Master_Prompt.md
│   └── Guia-StudyMode-ClaudeCode.md
├── 01-OOP-Fundamentals/
│   ├── 01-Interfaces.md
│   ├── 02-AbstractClasses.md
│   ├── 03-InterfaceVsAbstractClass.md
│   ├── 04-VirtualOverrideOverload.md
│   ├── 05-ReadonlyConstant.md
│   ├── 06-MultipleInterfaceImplementation.md
│   └── 07-FuncActionPredicate-Delegates.md
├── 02-DDD/
│   ├── 01-Composition-vs-Inheritance-ValueObjects.md
│   ├── 02-Entities-ValueObjects-AggregateRoots.md
│   └── 03-DomainEvents.md
├── 03-EFCore-Performance/
│   ├── 01-NPlusOne-Optimization.md
│   └── 02-IQueryable-vs-IEnumerable-Pushdown.md
├── 04-Architecture/
│   ├── 01-SOLID.md
│   ├── 02-CleanArchitecture.md
│   ├── 03-DependencyInjection.md
│   ├── 04-Repository-Pattern.md
│   └── 05-UnitOfWork.md
├── 05-Patterns-CQRS/
│   ├── 01-CQRS-Read-Write-Separation.md
│   ├── 02-MediatR-PipelineBehaviors.md
│   ├── 03-CQRS-vs-EventSourcing.md
│   ├── 04-EventSourcing-Projections.md
│   └── 05-Handlers-Sync-Async-Redis.md
├── 06-Microservices/
│   └── 01-DatabasePerService-Saga.md
├── 07-Interview/
│   ├── 00-InterviewAnswers-CheatSheet.md
│   └── 01-Pitch-Sheet-Senior.md
└── _archivo-pdfs/                  <- PDFs originales (respaldo, contenido ya en .md)
```

---

## 🎯 Plan de Estudio Global (Roadmap de Arquitectura .NET 8)

| Módulo | Estado | Descripción del Contenido | Archivo |
| :--- | :---: | :--- | :--- |
| **Módulo 1: Composición vs. Herencia (DDD)** | ✅ Completado | Value Objects (`record`), Agregados, `.ComplexProperty()` de EF Core 8 y mapeo plano en SQL Server sin JOINs. | `02-DDD/01` |
| **Módulo 2: EF Core Performance & N+1** | ✅ Completado | Diagnóstico del problema N+1, `AsNoTracking`, proyecciones con `.Select()` y mapa comparativo de estrategias. | `03-EFCore-Performance/01` |
| **Módulo 3: `IQueryable` vs `IEnumerable`** | ✅ Completado | Expression Trees, evaluación en Servidor vs Cliente y eliminación de Memory Leaks. | `03-EFCore-Performance/02` |
| **Módulo 4: CQRS Read/Write Separation** | 🔵 En progreso | Separación de pipelines de Lectura (Queries directas a DTO) y Escritura (Commands cargando Agregados). | `05-Patterns-CQRS/01` |
| **Módulo 5: Clean Architecture Structure** | 🔵 En progreso | Estructura enterprise de la solución `.sln` en .NET 8 y aislamiento de capas. | `04-Architecture/02` |
| **Módulo 6: Testing & Validation** | ⏳ Pendiente | Unit Tests de reglas de dominio e Integration Tests para EF Core (Respawn / Testcontainers). | — |
| **Módulo 7: Domain Events & Outbox Pattern** | 🔵 En progreso | Desacoplamiento asíncrono y consistencia eventual con Transactional Outbox. | `02-DDD/03` |

**Leyenda:** ⏳ Pendiente · 🔵 En progreso · ✅ Completado

---

## 📊 Dashboard de Temas Avanzados

### 🧩 OOP Fundamentals (bloque Interfaces/Abstracciones — 14 temas)

| # | Tema | Estado | Score | Archivo |
|---|---|:---:|:---:|:---|
| 1 | Interfaces | ✅ Completo | 4→3→resuelto (mecanismo de mocking) | `01-OOP-Fundamentals/01` |
| 2 | Abstract classes | ✅ Completo | 5/10 | `01-OOP-Fundamentals/02` |
| 3 | Interface vs Abstract class | ✅ Completo | — | `01-OOP-Fundamentals/03` |
| 4 | Virtual / Override / Overload | ✅ Completo | 7/10 | `01-OOP-Fundamentals/04` |
| 5 | Readonly / Constant | ✅ Completo | 8/10 | `01-OOP-Fundamentals/05` |
| 6 | Multiple interface implementation | ✅ Completo | 7/10 | `01-OOP-Fundamentals/06` |
| 7 | Default interface members | 🔵 En progreso | — | *(pitch en `07-Interview/01` Módulo B #7)* |
| 8 | Programming against abstractions | ⏳ Pendiente | | *(pitch en `07-Interview/01` Módulo B #8)* |
| 9 | Dependency Inversion | ⏳ Pendiente | | *(pitch en `07-Interview/01` Módulo B #9)* |
| 10 | Interface Segregation | ⏳ Pendiente | | *(enlaza a `01-OOP-Fundamentals/06`)* |
| 11 | Testability / Mocking | ⏳ Pendiente | | *(enlaza a `01-OOP-Fundamentals/01`)* |
| 12 | Abstraction leakage | ⏳ Pendiente | | — |
| 13 | When interfaces are unnecessary / Over-abstraction | ⏳ Pendiente | | — |
| 14 | 🏗️ Estructura de proyecto Dental Clinic (código) | ⏳ Pendiente | | `04-Architecture/02` |

### 🏛️ Arquitectura & Patrones (bloque playbook enterprise)

| Módulo | Tema | Estado | Score | Archivo |
| :--- | :--- | :---: | :---: | :--- |
| 01-Architecture | SOLID Principles | ✅ | 9.5 / 10 | `04-Architecture/01` |
| 01-Architecture | Dependency Injection & Lifetimes | ✅ | 9.5 / 10 | `04-Architecture/03` |
| 01-Architecture | Clean Architecture | ✅ | 9.0 / 10 | `04-Architecture/02` |
| 02-DDD | Entities, Value Objects & Aggregates | 🔜 Por revisar | | `02-DDD/02` |
| 02-DDD | Domain Events | 🔜 Por revisar | | `02-DDD/03` |
| 03-Patterns | Repository Pattern | ✅ | 9.5 / 10 | `04-Architecture/04` |
| 03-Patterns | Unit of Work | ✅ | 9.0 / 10 | `04-Architecture/05` |
| 03-Patterns | CQRS | 🔜 Por revisar | | `05-Patterns-CQRS/01` |
| 03-Patterns | MediatR & Pipeline Behaviors | 🔜 Por revisar | | `05-Patterns-CQRS/02` |
| 04-CQRS | CQRS vs Event Sourcing | 🔜 Por revisar | | `05-Patterns-CQRS/03` |
| 04-CQRS | Event Sourcing & Projections | 🔜 Por revisar | | `05-Patterns-CQRS/04` |
| 04-CQRS | Handlers, Async & Redis/Caching | 🔜 Por revisar | | `05-Patterns-CQRS/05` |
| 05-Microservices | Database per Service & Saga | 🔜 Por revisar | | `06-Microservices/01` |

---

## 🧵 Hilo conductor de conceptos ya conectados

- **Interfaces (1)** → habilita mocking real vía dynamic proxy (solo con métodos `virtual`/interfaz)
- **Abstract Classes (2)** → Template Method Pattern para compartir código real (retry logic)
- **Interface vs Abstract (3)** → decisión basada en relación entre clases, no solo sintaxis
- **Virtual/Override/Overload (4)** → mecanismo técnico que hace posible #1 y #2
- **Readonly/Const (5)** → conecta con inmutabilidad de Value Objects (`ContactInfo`, `init`)
- **Multiple Interfaces (6)** → evita interfaces "gordas", conecta con Interface Segregation (Tema 10)
- **Composición + Value Objects** → rechazo de `Person`/`IPerson`; TPH/TPT (Módulo 1)
- **N+1 / IQueryable** → pushdown SQL y proyecciones DTO en el lado de lectura (Módulos 2-3)

---

## 🎤 Entrevistas

- `07-Interview/00-InterviewAnswers-CheatSheet.md` — Q&A rápidos en inglés con enlaces a cada tema.
- `07-Interview/01-Pitch-Sheet-Senior.md` — Pitch de 30 segundos por tema + cheat sheet de decisión.

---

## 📝 Próximos Pasos Sugeridos

1. Cerrar el **Módulo 4: CQRS** con ejercicios y score (archivo ya con contenido completo).
2. Completar temas OOP 7-13 (pitch ya disponible en `07-Interview/01`).
3. Construir el **Módulo 6: Testing & Validation** (testado de reglas de dominio e integración EF Core).
4. Profundizar Microservices: Transactional Outbox & Inbox, Resilience (Polly), API Gateway, Observabilidad.

---

## 🔗 Referencias

- Flujo de coaching con Claude Code: `00-Meta/Guia-StudyMode-ClaudeCode.md`
- Prompt maestro del coach de entrevistas: `00-Meta/Senior_DotNet_Interview_Coach_Master_Prompt.md`
- PDFs originales (respaldo): `_archivo-pdfs/`