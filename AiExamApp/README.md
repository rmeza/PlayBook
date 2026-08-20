# AiExamApp — Puente a la IA para exámenes en vivo

App manos-libres: **hablas → se transcribe → auto-envía (sin botón) → respuesta IA concisa** para exámenes en vivo.

> **Web-first:** la v1 se corre y prueba en el navegador (Chrome/Edge) contra el backend local, para afinar la lógica antes del APK. El mismo código Ionic se empaqueta después como APK Android (requiere JDK 17 + Android SDK).

## Arquitectura

```
AiExamApp/
├── docs/                 <- Specs (source of truth SDD)
│   ├── 00-Spec.md        <- Spec de producto + contratos de API
│   └── 02-SDD-Workflow.md<- Flujo spec→implement→review→verify
├── backend/              <- .NET Minimal API (net10.0)
│   ├── AiExamApp.Api/    <- POST /api/ask, GET /api/models, GET /api/health
│   └── AiExamApp.Api.Tests/  <- xUnit v3 + FluentAssertions
└── mobile/               <- Ionic + Angular 21 + Capacitor (web + Android)
    ├── src/app/services/ <- voice (web SpeechRecognition / nativo Capacitor), api, tts
    └── src/app/home/     <- pantalla hands-free (auto-envío por silencio)
```

## Backend (local)

```powershell
cd backend
$env:ZEN_API_KEY = "tu-api-key"   # solo en servidor, nunca en el APK
dotnet run --project AiExamApp.Api   # escucha en http://0.0.0.0:5000
dotnet test AiExamApp.slnx
```

Modelos configurados en `backend/AiExamApp.Api/appsettings.json` bajo `Ai:Models`
(default: `deepseek-v4-flash-free`). Agregar uno = editar ese archivo, sin tocar código.

## Frontend — modo web (recomendado para iterar)

```powershell
cd mobile
npm install
npm run dev        # http://localhost:4200 (Chrome/Edge, micrófono)
```

La API base se configura con `AI_EXAM_API_URL` (global `window.AI_EXAM_API_URL` en el
HTML) o usa `http://localhost:5000/api` por defecto.

## APK Android (fase futura)

```powershell
cd mobile
npx cap add android
npm run build
npx cap sync android
npx cap open android   # requiere JDK 17 + Android SDK instalados
```

## Flujo SDD

Cada feature se construye con el bucle del PlayBook (`docs/02-SDD-Workflow.md`):
`spec-planner` → `implementer` → `code-reviewer` → verify (tests verdes).
Agentes en `.opencode/agents/`.

## Estado

- [x] Backend: endpoints + tests (6/6 verdes)
- [x] Frontend web-first: escucha continua, auto-envío por silencio, selector de modelo, TTS
- [x] Revisión SDD con code-reviewer (hallazgos corregidos)
- [ ] Probar con API key real (pendiente `ZEN_API_KEY`)
- [ ] APK Android (requiere JDK 17 + Android SDK)