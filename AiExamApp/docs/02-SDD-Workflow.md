# 02-SDD-Workflow.md — Flujo Spec-Driven con Agentes

Patrón para construir cada feature de `AiExamApp` (y cualquier proyecto del PlayBook) con **spec como fuente de verdad** y **agentes especializados** como pipeline.

## El bucle SDD

```text
        ┌──────────────────────────────────────────────┐
        │                                              │
        ▼                                              │
  [SPEC]  spec-planner  →  [IMPLEMENT]  implementer  → │
  (docs/*.md)           →  (código+tests)              │
        ▲                                              │
        └──────────── [VERIFY]  code-reviewer ────────┘
                      (audita contra spec + tests verdes)
```

1. **Spec** — `spec-planner` (subagent read-only) convierte la intención en `docs/*.md`: requisitos, acceptance criteria, contratos API. Solo edita docs.
2. **Implement** — `implementer` (subagent con permisos edit) construye código + tests para satisfacer la spec. No cambia la spec.
3. **Review** — `code-reviewer` (subagent read-only) compara el diff contra la spec: cubre acceptance criteria, edge cases, O(), convenciones del repo.
4. **Verify** — comandos reales (`dotnet test AiExamApp.slnx`, `npm run build`). Solo se cierra el paso si pasan.
5. **Loop** — si review/verify falla, vuelve a implement sin tocar la spec; si la spec queda obsoleta, la actualiza primero `spec-planner`.

## Roles de agentes

| Agente | Modo | Puede editar | Tarea |
|--------|------|--------------|-------|
| `spec-planner` | subagent | solo `docs/` | Escribir/refinar specs y acceptance criteria |
| `implementer` | subagent | código + tests | Implementar contra la spec |
| `code-reviewer` | subagent | nada (deny edit) | Auditar diff contra spec + convenciones |

## Reglas del flujo

- **La spec manda:** ningún agente de implementación inventa requisitos fuera de `docs/*.md`.
- **Nadie toca dos roles:** si falta spec, `spec-planner`; si falta código, `implementer`; si falta validación, `code-reviewer` + verify.
- **Una feature a la vez** (mismo espíritu que el coach de CodeChallenge).
- **Verify es innegociable:** sin `dotnet test` verde no se cierra una iteración.

## Invocación

Desde el agente principal (build) con el `task` tool, o manual con `@mention`:

- `@spec-planner redacta la spec para el endpoint /api/ask según 00-Spec.md`
- `@implementer implementa POST /api/ask según docs/00-Spec.md`
- `@code-reviewer revisa el diff de backend contra docs/00-Spec.md`

## Estructura de una spec (`docs/NN-Nombre.md`)

1. Objetivo (1 párrafo)
2. Requisitos funcionales (tabla ID + acceptance criteria)
3. Requisitos no funcionales
4. Contratos de API / interfaces (JSON/type/firma)
5. Fuera de alcance