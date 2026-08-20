# 🎨 FRONTEND CHEAT SHEET — JavaScript + Angular (examen técnico)

Condensado de frontend para repaso rápido. Mentalidad: **Angular es el consumidor de tu WebApi .NET**. Cada bloque: tabla + snippet clave.

---

## 1. JavaScript: lo esencial

### Tipos y declaración

| | `var` | `let` | `const` |
|---|---|---|---|
| Scope | función | bloque | bloque |
| Reasignar | ✅ | ✅ | ❌ (inmutable referencia) |

- `null` (ausencia intencional) vs `undefined` (no asignado)
- `===` (estricto, compara tipo+valor) vs `==` (con coerción — evítalo)
- Tipos primitivos: `string, number, boolean, null, undefined, symbol, bigint`
- **Objetos/arrays son por referencia** — copiar con spread `{ ...obj }` / `[...arr]` (copia superficial)

### Funciones modernas

```js
const suma = (a, b) => a + b;          // arrow function
const saludar = nombre => `Hola ${nombre}`;  // template literal
array.map(x => x * 2).filter(x => x > 10);   // inmutabilidad funcional
```

### Async: Promesas y async/await

```js
// fetch → Promise (paralelo conceptual a Task de .NET)
const data = await fetch('/api/patients').then(r => r.json());

async function load() {
  const [a, b] = await Promise.all([fetchA(), fetchB()]);  // en paralelo
}
```

> `async` devuelve Promise; `await` solo dentro de `async`. **Paralelo a `async/await` en C#.**

### Destructuring y spread

```js
const { id, name } = patient;          // extrae propiedades
const merged = { ...patient, active: true };  // clona + agrega
const [first, ...rest] = list;         // array destructuring
```

---

## 2. Angular: arquitectura

### Componentes (la pieza central)

```ts
@Component({
  selector: 'app-patient-card',        // <app-patient-card> en HTML
  templateUrl: './patient-card.html',
  styles: ['./patient-card.css']
})
export class PatientCardComponent {
  @Input() patient!: Patient;          // dato de entrada (padre→hijo)
  @Output() selected = new EventEmitter<number>();  // evento salida

  onClick() { this.selected.emit(this.patient.id); }
}
```

### Template y Binding (data flow)

| Binding | Sintaxis | Dirección |
|---|---|---|
| Interpolación | `{{ patient.name }}` | One-way TS→HTML |
| Propiedad | `[disabled]="!valid"` | One-way TS→HTML |
| Evento | `(click)="onClick()"` | One-way HTML→TS |
| Doble | `[(ngModel)]="search"` | Two-way (forms) |

```html
<!-- Estructuras de control -->
<div *ngFor="let p of patients">{{ p.name }}</div>
<div *ngIf="patients.length === 0">Vacío</div>
<p [class.active]="selectedId === p.id">{{ p.name }}</p>
```

### Ciclo de vida (los 2 que importan)

- `ngOnInit()` → **lógica de carga** (equivalentes de construcción). `constructor` solo para DI.
- `ngOnDestroy()` → limpiar suscripciones (RxJS) para evitar memory leaks.
- `ngOnChanges()` → reaccionar a `@Input`.

---

## 3. RxJS: flujos asíncronos

> Angular reemplaza Promises por **Observables**: emiten 0..N valores en el tiempo (stream). El HTTP de Angular devuelve Observable, no Promise.

```ts
// Servicio: expone flujo de pacientes
@Injectable({ providedIn: 'root' })
export class PatientService {
  constructor(private http: HttpClient) {}

  getPatients(): Observable<Patient[]> {
    return this.http.get<Patient[]>('/api/patients');
  }
}

// Componente: suscripción
this.patients$ = this.service.getPatients();   // guardar el observable

// Template: async pipe se suscribe/desuscribe solo (sin leaks)
<div *ngFor="let p of patients$ | async">{{ p.name }}</div>
```

### Operadores clave (pipe)

```ts
this.service.getPatients().pipe(
  map(p => p.filter(x => x.active)),     // transformar (como LINQ Select/Where)
  debounceTime(300),                     // esperar silencio (búsqueda)
  catchError(() => of([]))               // manejar error → valor por defecto
);
```

| RXJS | LINQ/C# equivalente |
|---|---|
| `map` | `Select` |
| `filter` | `Where` |
| `take(n)` / `first()` | `Take` / `First` |
| `mergeMap` / `switchMap` | `SelectMany` |
| `combineLatest` | espera último de cada fuente |
| `Subject` / `BehaviorSubject` | evento/pub-sub |

> **Mentalidad senior:** `async` pipe + `BehaviorSubject` para estado → sin `subscribe()` manual en templates.

---

## 4. Angular ↔ .NET: la integración

```
[Angular]  http.get('/api/patients')  ──HTTP/JSON──▶  [WebApi .NET]  GET api/patients
  ▼                                                                      ▼
  Patient[] (tipado TS)  ◀─────────JSON────────────  JsonResult (Model Binding)
```

### Convenciones compartidas

- **DTOs**: definí la misma forma de datos en TS (`interface Patient { id: number; name: string }`) que en tu DTO C# — mismas propiedades, camelCase JSON.
- **HttpClient** = tu `HttpClient` de .NET; usa `HttpParams` para query (`?page=1`), headers para auth (Bearer JWT).
- **CORS** habilita en WebApi: `AddCors()` + `AllowAnyOrigin`/`WithOrigins("http://localhost:4200")`.
- **Auth**: `HttpInterceptor` añade el token en cada request (equivalente a `DelegatingHandler`).
- **Señales (signal)**: la API nueva que notifica cambios → `computed()`, `signal()`. Lo equivalente a un observable de un solo valor reactivo.

### RxJS vs Task — tabla mental

| .NET | Angular |
|---|---|
| `Task<T>` (1 valor futuro) | `Observable<T>` (0..N valores) |
| `await` | `async` pipe / `subscribe()` |
| `Task.WhenAll` | `forkJoin` / `Promise.all` |
| `CancellationToken` | `takeUntil(this.destroy$)` |
| `event`/`EventHub` | `Subject` |

---

## 5. Respuestas clave (English one-liners)

| # | Pregunta | One-liner |
|---|---|---|
| 1 | Promise vs Observable | *"Promise emits once; Observable emits 0..N values over time with operators for composition."* |
| 2 | Why `async` pipe | *"It subscribes/unsubscribes automatically — no manual cleanup, no memory leaks."* |
| 3 | `@Input` vs `@Output` | *"Input flows data down from parent; Output emits events up to the parent."* |
| 4 | `*ngFor` vs `*ngIf` | *"ngFor iterates collections in the template; ngIf conditionally renders blocks."* |
| 5 | Angular DI | *"Same as .NET — constructor injection with @Injectable services and providedIn scope."* |
| 6 | Handling HTTP errors | *"catchError in the pipe to return a fallback Observable instead of throwing."* |
| 7 | Avoid memory leaks | *"takeUntil(destroy$) or the async pipe unsubscribes automatically in ngOnDestroy."* |
| 8 | Two-way binding | *"ngModel combines property binding and event binding into [(ngModel)]."* |

---

## 🔗 Relación con el PlayBook

| Backend (.NET) | Frontend (Angular) |
|---|---|
| `04-Architecture/03-DependencyInjection.md` | `@Injectable` + constructor DI |
| `05-Patterns-CQRS/05` (async) | RxJS `switchMap`/`forkJoin` |
| WebApi DTOs | TS interfaces |
| `HttpClient`/DelegatingHandler | `HttpClient`/`HttpInterceptor` |
| `CORS` en WebApi | `provideHttpClient()` |