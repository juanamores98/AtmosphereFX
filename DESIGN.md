# Diseño de AtmosphereFX v2

Especificación funcional de la segunda versión. El modelo de ajuste, los rangos y
el esquema de persistencia son propios de esta versión.

## Objetivo

Dar control directo sobre los dos sistemas de niebla del juego (dinámica día/noche
y clásica por cubemap) y sobre el volumen de dispersión, sin transformaciones
intermedias: cada control escribe su valor final en el componente de render.

## Modelo de ajuste (v2)

### Niebla dinámica (`FogProperties` + `DayNightFogEffect`)
| Parámetro | Rango v2 | Paso | Valor vanilla |
|---|---|---|---|
| Color decay | 0–1 | 0.01 | 0.2 |
| Densidad | 0–0.005 | 0.00005 | 0.00223 |
| Ruido | 0–2 | 0.02 | 1.0 |
| Techo de niebla | 0–5000 | 25 | 1000 |
| Línea de horizonte | 0–5000 | 25 | 800 |
| Distancia de inicio | 0–10000 | 25 | 194 |
| Velocidad de deriva | 0–0.05 (flotante) | 0.001 | 0.001 |

Cambios de diseño frente a una versión anterior: alturas por slider (no campos de
texto), velocidad de deriva flotante real, y `m_edgeFog` compartido por ambos
sistemas de niebla para mantenerlos coherentes.

### Niebla clásica (`FogEffect`) y volumen (`RenderProperties`)
| Parámetro | Rango v2 | Nota |
|---|---|---|
| Cubemap fog | on/off | con apagado nocturno automático opcional |
| Scatter falloff | 0.5–10 | se aplica **directo** como exponente del juego |
| Scatter strength | 0–5 | aplicación directa |
| Color de dispersión | auto / sol / custom (RGB) | el modo "sol" se actualiza por tick |
| Color del volumen | auto / custom (RGB) | |
| Volume start | 0–4000 | independiente del edge fog |

### Comportamiento
- `applyOnLoad`: aplica el perfil completo al cargar un mapa (on por defecto).
- Reset a vanilla: restaura todos los parámetros a los valores de un juego sin
  modificar y los aplica en vivo.

## Persistencia

XML propio (`AtmosphereFX2.xml`, raíz `atmosphereFx`, `schema="2"`) con validación
de rango en cada propiedad al deserializar. Archivo situado junto al ejecutable
del juego (directorio de trabajo del proceso).

## Arquitectura

- `Config/ModConfig` — modelo estático de valores + resolución de colores.
- `Config/ConfigFile` — esquema XML v2 + almacenamiento.
- `Runtime/SettingsApplier` — única vía de escritura hacia los componentes.
- `Runtime/PerFrameWatcher` — comportamientos por tick (color igualado al sol,
  apagado nocturno).
- `Options/` — panel de opciones y fábrica de controles.
