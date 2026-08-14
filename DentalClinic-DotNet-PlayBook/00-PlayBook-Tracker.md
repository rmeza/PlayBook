# 🧩 PlayBook: Interfaces, Abstracciones & Fundamentos OOP
## Dominio de referencia: Dental Clinic (Patient, Dentist, Appointment, Odontogram)

---

## 📍 Estado General (Avance parcial)

| # | Tema | Estado | Score |
|---|---|:---:|:---:|
| 1 | Interfaces | ✅ Completo | 4→3→resuelto (mecanismo de mocking) |
| 2 | Abstract classes | ✅ Completo | 5/10 |
| 3 | Interface vs Abstract class | ✅ Completo | — |
| 4 | Virtual / Override / Overload | ✅ Completo | 7/10 |
| 5 | Readonly / Constant | ✅ Completo | 8/10 |
| 6 | Multiple interface implementation | ✅ Completo | 7/10 |
| 7 | Default interface members | 🔵 En progreso | — |
| 8 | Programming against abstractions | ⏳ Pendiente | |
| 9 | Dependency Inversion | ⏳ Pendiente | |
| 10 | Interface Segregation | ⏳ Pendiente | |
| 11 | Testability / Mocking | ⏳ Pendiente | |
| 12 | Abstraction leakage | ⏳ Pendiente | |
| 13 | When interfaces are unnecessary / Over-abstraction | ⏳ Pendiente | |
| 14 | 🏗️ Estructura de proyecto Dental Clinic (código) | ⏳ Pendiente | |

**Leyenda:** ⏳ Pendiente · 🔵 En progreso · ✅ Completado

---

## 📄 Archivos individuales generados

Cada tema completado tiene su propio archivo `.md` con: concepto, problema, solución con código,
diagrama Mermaid, trade-offs, y respuesta Senior en inglés.

- `01-Interfaces.md`
- `02-AbstractClasses.md`
- `03-InterfaceVsAbstractClass.md`
- `04-VirtualOverrideOverload.md`
- `05-ReadonlyConstant.md`
- `06-MultipleInterfaceImplementation.md`

---

## 🧵 Hilo conductor de conceptos ya conectados

- **Interfaces (1)** → habilita mocking real vía dynamic proxy (solo funciona con métodos `virtual`/interfaz)
- **Abstract Classes (2)** → Template Method Pattern para compartir código real (retry logic)
- **Interface vs Abstract (3)** → decisión basada en relación entre clases, no solo sintaxis
- **Virtual/Override/Overload (4)** → mecanismo técnico que hace posible #1 y #2
- **Readonly/Const (5)** → conecta con inmutabilidad de Value Objects (`ContactInfo`, `init`)
- **Multiple Interfaces (6)** → evita interfaces "gordas", conecta directo con Interface Segregation (Tema 10)

---

## 🏗️ Tema 14: Estructura de Proyecto Dental Clinic (Clean Architecture)

*(Pendiente — se desarrollará al cerrar el bloque de 13 temas)*

```text
DentalClinic/
├── DentalClinic.Domain/
│   ├── Aggregates/
│   ├── ValueObjects/
│   ├── Entities/
│   ├── Interfaces/
│   └── Events/
├── DentalClinic.Application/
├── DentalClinic.Infrastructure/
└── DentalClinic.WebApi/
```

---

## 📝 Notas Generales de la Sesión

- Usuario prefiere explicaciones pausadas, sin saltar directo a nivel "Arquitecto" — ritmo Senior claro.
- Feedback de scores tiende a detectar respuestas "genéricas" que repiten la conclusión sin mecanismo técnico — patrón a vigilar en próximos temas.
- Flujo de Claude Code documentado en `00-Guia-StudyMode-ClaudeCode.md` para replicar este Study Mode con ahorro de tokens (Sonnet para enseñanza, Haiku para exportar archivos).
