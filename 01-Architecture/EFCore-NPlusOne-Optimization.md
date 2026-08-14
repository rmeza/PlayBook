# 🏛️ Master Playbook: Domain-Driven Design & EF Core Performance Optimization

---

## 🎯 Executive Summary
Guía arquitectónica integral para aplicaciones enterprise en **.NET 8**. Cubre desde el modelado de dominio con **DDD (Composición vs Herencia, Value Objects, Complex Types)** hasta la optimización avanzada de persistencia en **Entity Framework Core 8 (Problema N+1, `IQueryable` vs `IEnumerable`, Proyecciones y Rendimiento en SQL Server)**.

---

# 🧠 MÓDULO 1: Composición vs. Herencia en DDD

## 1.1 ¿Por qué NO usar Clase Abstracta `Person` ni Interfaz `IPerson`?

### ❌ 1. Clase Abstracta `Person` (Herencia de Datos)
* **Invariantes de Dominio:** `Patient` y `Doctor` no son variantes de persona; son **ROLES y AGREGADOS independientes**. Meclar sus atributos destruye la encapsulación del dominio.
* **Degradación en SQL Server (Mapeos EF Core):**
  * **TPH (Table Per Hierarchy):** Genera una sola tabla gigante `People` colmada de columnas `NULL` (ej. `MedicalHistoryNumber` queda NULL en doctores; `MedicalLicense` queda NULL en pacientes).
  * **TPT (Table Per Type):** Genera múltiples tablas enlazadas por `JOINs` obligatorios en cada lectura, degradando severamente la velocidad de consulta.

### ❌ 2. Interfaz `IPerson`
* Las interfaces definen **contratos de comportamiento** (*can-do*), no estado persistible.
* Crear `IPerson` sin un caso de uso polimórfico real en la capa de aplicación representa sobre-ingeniería (Violación del principio YAGNI).

---

## 1.2 Arquitectura por Composición

Aplicamos el principio **"Tiene-un" (*Has-a*)**:
* `Patient` **TIENE UN** `PersonName` y **TIENE UN** `ContactInfo`.
* `Doctor` **TIENE UN** `PersonName` y **TIENE UN** `ContactInfo`.

```mermaid
classDiagram
    class Patient {
        +Guid Id
        +PersonName Name
        +ContactInfo Contact
        +DateOnly DateOfBirth
        +string MedicalHistoryNumber
        +Create(Name, Contact, DateOfBirth, HistoryNumber)
    }

    class Doctor {
        +Guid Id
        +PersonName Name
        +ContactInfo Contact
        +string MedicalLicense
        +Create(Name, Contact, MedicalLicense)
    }

    class PersonName {
        +string FirstName
        +string LastName
        +string FullName
    }

    class ContactInfo {
        +string Email
        +string PhoneNumber
    }

    Patient *-- PersonName : Value Object
    Patient *-- ContactInfo : Value Object
    Doctor *-- PersonName : Value Object
    Doctor *-- ContactInfo : Value Object