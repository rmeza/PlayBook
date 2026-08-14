# OOP Principles: Interfaces vs Abstract Classes

🎯 **Objective:** Entender cuándo usar Interfaces vs Clases Abstractas en el diseño del dominio y Clean Architecture.

❓ **What problem does it solve?**
Evita el acoplamiento rígido, la duplicación de código y las jerarquías de herencia frágiles (Fragile Base Class Problem).

🧠 **Core Concept:**
* **Interface:** Define capacidades y contratos (Qué hace). Sin estado.
* **Abstract Class:** Define identidad y reutilización de comportamiento (Qué es). Con estado.

💻 **C# Example:**
\\\csharp
public abstract class ClinicalDocument
{
    public Guid PatientId { get; private set; }
    public abstract bool ValidateNom004Rules();
}

public interface IPrintable
{
    byte[] RenderPdf();
}
\\\

⚖️ **Trade-offs:**
* Favorecer Composición sobre Herencia.
* Máximo 1 nivel de herencia de clases abstractas.

🎤 **Senior Answer:**
"Interfaces are stateless contracts for capabilities across decoupled layers, while abstract classes provide shared state and behavior for closely related domain entities."
