# 🎯 CodeChallenge PlayBook — Tracker de Progreso

## Dominio: Problemas clásicos de entrevistas .NET (net10.0 · xUnit v3 · FluentAssertions)

---

## 🧑‍🏫 Metodología de estudio (modo coach)

Por cada problema se sigue este flujo, uno a la vez:

1. **Reto primero:** enunciado + pausa para intentarlo (patrón entrevista real).
2. **Discusión:** análisis de tu enfoque, score honesto (1-10) y qué faltó.
3. **Solución guiada:** código correcto, complejidad O() y por qué es la vía óptima.
4. **Pregunta de seguimiento:** variante típica de entrevista (índice en vez de char, O(1) memoria, etc.).
5. **Respuesta Senior en inglés:** lista para la entrevista real, razonando paso a paso.
6. **Modo "dame la solución"** solo si se pide explícitamente.

**Reglas del coach:**
- No se avanza de problema sin cerrar el anterior.
- Scores honestos; no aceptar respuestas genéricas sin mecanismo técnico.
- Cada problema = clase `public static` con comentario O() + suite de tests xUnit.

---

## 📍 Estado General

| # | Problema | Categoría | Estado | Score |
|---|----------|:---:|:---:|:---:|
| 1 | Anagram | Strings | ✅ Implementado + Testeado | — |
| 2 | Palindrome | Strings | ✅ Implementado + Testeado | — |
| 3 | Reverse (string / por palabra) | Strings | ✅ Implementado + Testeado | — |
| 4 | Balanced parentheses (simple / múltiple) | Strings | ✅ Implementado + Testeado | — |
| 5 | First non-repeating char | Strings | ✅ Implementado + Testeado | — |
| 6 | String compression (`aaabb` → `a3b2`) | Strings | ✅ Completado | 4/10 → resuelto |
| 7 | Longest substring sin repetir | Strings | ⏳ Pendiente | |
| 8 | Title case sin built-ins | Strings | ⏳ Pendiente | |
| 9 | Remove duplicate characters | Strings | ⏳ Pendiente | |
| 10 | Count char frequency | Strings | ⏳ Pendiente | |
| 11 | Second largest element | Arrays | ⏳ Pendiente | |
| 12 | Move zeros to end | Arrays | ⏳ Pendiente | |
| 13 | Missing number (1..n) | Arrays | ⏳ Pendiente | |
| 14 | Find duplicates | Arrays | ⏳ Pendiente | |
| 15 | Merge two sorted arrays | Arrays | ⏳ Pendiente | |
| 16 | Sort sin built-in | Arrays | ⏳ Pendiente | |
| 17 | Group by department (LINQ) | LINQ | ⏳ Pendiente | |
| 18 | Top 3 salaries (LINQ) | LINQ | ⏳ Pendiente | |
| 19 | Remove duplicates (LINQ) | LINQ | ⏳ Pendiente | |
| 20 | Pagination (LINQ) | LINQ | ⏳ Pendiente | |
| 21 | Dynamic sorting asc/desc | LINQ | ⏳ Pendiente | |
| 22 | Common elements entre listas | LINQ | ⏳ Pendiente | |
| 23 | Factorial (recursión) | Recursión | ⏳ Pendiente | |
| 24 | Fibonacci | Recursión | ⏳ Pendiente | |
| 25 | Sum of digits | Recursión | ⏳ Pendiente | |
| 26 | Binary search recursivo | Recursión | ⏳ Pendiente | |
| 27 | Tower of Hanoi | Recursión | ⏳ Pendiente | |
| 28 | Palindrome (recursión) | Recursión | ⏳ Pendiente | |
| 29 | Parallel processing | Multithreading | ⏳ Pendiente | |
| 30 | Thread-safe singleton | Multithreading | ⏳ Pendiente | |
| 31 | CancellationToken | Multithreading | ⏳ Pendiente | |
| 32 | Async file processing | Multithreading | ⏳ Pendiente | |
| 33 | Deadlock demo | Multithreading | ⏳ Pendiente | |
| 34 | Stack con array | Data Structures | ⏳ Pendiente | |
| 35 | Queue con dos stacks | Data Structures | ⏳ Pendiente | |
| 36 | Reverse linked list | Data Structures | ⏳ Pendiente | |
| 37 | Detect loop en linked list | Data Structures | ⏳ Pendiente | |
| 38 | BST insert | Data Structures | ⏳ Pendiente | |
| 39 | Graph traversal (BFS/DFS) | Data Structures | ⏳ Pendiente | |

**Leyenda:** ⏳ Pendiente · 🔵 En progreso · ✅ Completado

---

## 📋 Comandos útiles

```powershell
dotnet test CodeChallenge.slnx                 # todos los tests
dotnet test --filter "FullyQualifiedName~Nombre"  # filtrar por suite
```