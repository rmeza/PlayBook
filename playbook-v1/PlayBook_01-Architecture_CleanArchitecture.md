# 01-Architecture / Clean Architecture

---

## 1. Regla de Dependencia

Las dependencias apuntan **únicamente hacia adentro**. El Dominio no conoce nada de la Base de Datos, de la Web, ni de las APIs Externas.

---

## 2. Estructura de Capas

```mermaid
graph TD
    UI[Presentation / Controllers] --> App[Application Layer]
    Infra[Infrastructure / EF Core] --> App
    App --> Domain[Domain Layer - Core]
    Infra --> Domain
```

---

## 3. 🎤 Respuesta de Entrevista en Inglés (Senior/Staff Level)

> **Interviewer:** *"Why Clean Architecture over traditional N-Tier architecture?"*

> **Your Answer:**  
> "Traditional N-Tier architectures often tightly couple business rules to the database layer because the UI calls the Business Logic Layer, which directly references the Data Access Layer. In **Clean Architecture**, we invert that dependency. The Domain layer sits at the center with zero external dependencies. This ensures that UI frameworks, ORMs, or cloud providers are merely infrastructure details that can be evolved, tested, or swapped without impacting core business rules."