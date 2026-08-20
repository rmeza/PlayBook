# AiExamApp — Spec de Producto (source of truth)

> Puente a la IA para exámenes en vivo: preguntas por voz que se auto-envían y reciben una respuesta rápida, clara y concisa. Backend local en la laptop, app móvil en el celular (mismo Wi-Fi).

---

## 1. Objetivo

Minimizar fricción en un examen en vivo: el usuario **habla**, la app **transcribe**, **auto-envía** sin botón y muestra la **mejor respuesta** del modelo en segundos.

## 2. Usuarios y contexto

- Un único usuario (el dueño del PlayBook).
- Dispositivo: **Android** (APK vía Ionic/Capacitor; desarrollo verificado en navegador).
- Examen en vivo: manos ocupadas, ruido ambiente, prisa. La app se usa mirando la pantalla, sin tocar botones.

## 3. Requisitos funcionales (FR)

| ID | Requisito | Acceptance Criteria |
|----|-----------|--------------------|
| FR-1 | Escucha continua de voz | Al abrir la pantalla, el micrófono queda activo y escuchando sin interacción previa. |
| FR-2 | Auto-envío sin botón | Al detectar pausa en el habla, la transcripción se envía automáticamente a la API. |
| FR-3 | Respuesta concisa | La API devuelve una respuesta en <5 s; renderizada clara y corta (modo examen). |
| FR-4 | Selector de modelo | La app consulta `GET /api/models` y permite cambiar el modelo activo (default: `deepseek-v4-flash-free`). |
| FR-5 | TTS opcional | La respuesta puede leerse en voz alta con un toggle. |
| FR-6 | Keep-awake | La pantalla no se apaga mientras la app está en uso. |
| FR-7 | Modo silencioso | Si no hay red local / API caída, la app muestra error claro sin colgar. |

## 4. Requisitos no funcionales (NFR)

- **Latencia:** respuesta del backend < 5 s en red local con modelo default.
- **Seguridad:** la API key de OpenCode Zen vive **solo en el backend** (variable de entorno); nunca en el APK.
- **Configuración:** agregar un modelo = editar `appsettings.json`, sin tocar código.
- **Orden:** estructura clara por capas (docs / backend / mobile), siguiendo el flujo SDD del repo.

## 5. Contratos de API (backend local)

### `POST /api/ask`
```json
{ "question": "diferencia entre abstract class e interface en C#", "model": "deepseek-v4-flash-free" }
```
`model` opcional (default: modelo por defecto del servidor). Respuesta 200:
```json
{ "answer": "respuesta concisa del LLM", "model": "deepseek-v4-flash-free", "elapsedMs": 2340 }
```

### `GET /api/models`
```json
{ "default": "deepseek-v4-flash-free", "models": [
  { "id": "deepseek-v4-flash-free", "name": "DeepSeek V4 Flash Free", "free": true },
  { "id": "deepseek-v4-flash", "name": "DeepSeek V4 Flash", "free": false }
]}
```

### `GET /api/health`
```json
{ "status": "ok" }
```

## 6. Prompt del sistema (respuesta examen)

- Responder en el idioma de la pregunta.
- Máximo 8 líneas / ~100 palabras salvo que el tema lo exija.
- Estructura: definición en 1 línea → mecanismo técnico → ejemplo mínimo → trampa común.
- Sin relleno, sin disclaimers.

## 7. Fuera de alcance (v1)

- Autenticación multi-usuario.
- Historial persistente en la nube.
- APK firmado (bloqueado hasta instalar JDK 17 + Android SDK).