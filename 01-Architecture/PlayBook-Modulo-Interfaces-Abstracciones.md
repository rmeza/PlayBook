# 🧩 PlayBook: Interfaces, Abstracciones & Fundamentos OOP
## Dominio de referencia: Dental Clinic (Patient, Dentist, Appointment, Odontogram)

---

## 📍 Estado General

| # | Tema | Estado | Score (1-10) | Fecha |
|---|---|:---:|:---:|:---:|
| 1 | Interfaces | 🔵 En progreso | | |
| 2 | Abstract classes | ⏳ Pendiente | | |
| 3 | Interface vs Abstract class | ⏳ Pendiente | | |
| 4 | Virtual / Override / Overload | ⏳ Pendiente | | |
| 5 | Readonly / Constant | ⏳ Pendiente | | |
| 6 | Multiple interface implementation | ⏳ Pendiente | | |
| 7 | Default interface members | ⏳ Pendiente | | |
| 8 | Programming against abstractions | ⏳ Pendiente | | |
| 9 | Dependency Inversion | ⏳ Pendiente | | |
| 10 | Interface Segregation | ⏳ Pendiente | | |
| 11 | Testability / Mocking | ⏳ Pendiente | | |
| 12 | Abstraction leakage | ⏳ Pendiente | | |
| 13 | When interfaces are unnecessary / Over-abstraction | ⏳ Pendiente | | |
| 14 | 🏗️ Estructura de proyecto Dental Clinic (código) | ⏳ Pendiente | | |

**Leyenda:** ⏳ Pendiente · 🔵 En progreso · ✅ Completado

---

## 📄 Detalle por Tema

> Cada sección se completa después de terminar el tema: tu respuesta original, el score, lo que faltó, y la respuesta Senior/Architect final en inglés.

### 1. Interfaces
- **Concepto clave:** Contrato de comportamiento sin estado, permite depender de abstracciones (`INotificationSender`).
- **Mi respuesta original:**
- **Score:**
- **Qué faltó:**
- **Respuesta Senior (EN):**

---

### 2. Abstract classes
- **Concepto clave:**
- **Mi respuesta original:**
- **Score:**
- **Qué faltó:**
- **Respuesta Senior (EN):**

---

### 3. Interface vs Abstract class
- **Concepto clave:**
- **Mi respuesta original:**
- **Score:**
- **Qué faltó:**
- **Respuesta Senior (EN):**

---

### 4. Virtual / Override / Overload
- **Concepto clave:**
- **Mi respuesta original:**
- **Score:**
- **Qué faltó:**
- **Respuesta Senior (EN):**

---

### 5. Readonly / Constant
- **Concepto clave:**
- **Mi respuesta original:**
- **Score:**
- **Qué faltó:**
- **Respuesta Senior (EN):**

---

### 6. Multiple interface implementation
- **Concepto clave:**
- **Mi respuesta original:**
- **Score:**
- **Qué faltó:**
- **Respuesta Senior (EN):**

---

### 7. Default interface members
- **Concepto clave:**
- **Mi respuesta original:**
- **Score:**
- **Qué faltó:**
- **Respuesta Senior (EN):**

---

### 8. Programming against abstractions
- **Concepto clave:**
- **Mi respuesta original:**
- **Score:**
- **Qué faltó:**
- **Respuesta Senior (EN):**

---

### 9. Dependency Inversion
- **Concepto clave:**
- **Mi respuesta original:**
- **Score:**
- **Qué faltó:**
- **Respuesta Senior (EN):**

---

### 10. Interface Segregation
- **Concepto clave:**
- **Mi respuesta original:**
- **Score:**
- **Qué faltó:**
- **Respuesta Senior (EN):**

---

### 11. Testability / Mocking
- **Concepto clave:**
- **Mi respuesta original:**
- **Score:**
- **Qué faltó:**
- **Respuesta Senior (EN):**

---

### 12. Abstraction leakage
- **Concepto clave:**
- **Mi respuesta original:**
- **Score:**
- **Qué faltó:**
- **Respuesta Senior (EN):**

---

### 13. When interfaces are unnecessary / Over-abstraction
- **Concepto clave:**
- **Mi respuesta original:**
- **Score:**
- **Qué faltó:**
- **Respuesta Senior (EN):**

---

## 🏗️ Tema 14: Estructura de Proyecto Dental Clinic (Clean Architecture)

Objetivo: aplicar todo lo aprendido (interfaces, DI, abstracciones) en la estructura real de carpetas/proyectos .NET.

```text
DentalClinic/
├── DentalClinic.Domain/
│   ├── Aggregates/
│   │   └── PatientAggregate/
│   ├── ValueObjects/
│   ├── Entities/
│   ├── Interfaces/          <- Contratos del dominio (IRepository, etc.)
│   └── Events/
├── DentalClinic.Application/
│   ├── Appointments/
│   │   ├── Commands/
│   │   └── Queries/
│   ├── Common/
│   │   └── Interfaces/      <- INotificationSender, IApplicationDbContext
│   └── Behaviors/
├── DentalClinic.Infrastructure/
│   ├── Persistence/
│   ├── Notifications/       <- Implementaciones concretas de INotificationSender
│   └── Interceptors/
└── DentalClinic.WebApi/
    ├── Controllers/
    └── Program.cs
```

*(Pendiente: llenar con el detalle de cada capa una vez completado el bloque de interfaces)*

---

## 📝 Notas Generales de la Sesión

-
-
-
