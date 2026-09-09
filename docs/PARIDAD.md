# Paridad de AtmosphereFX

Revisión: 2026-09-08. Requisitos del encargo y de la matriz de comportamiento de `ModdingResearch/EncargosFX/Auditoria-20260908/INFORME.md`. No implica adopción de implementaciones GPL.

| Capacidad | Fuente de requisito | Entrada / unidad / rango | Comportamiento | API comprobada | Destino | Prueba | Estado / diferencia |
|---|---|---|---|---|---|---|---|
| Niebla y bordes | Fog Controller / auditoría H06–H10 | Interruptores independientes | Aplicar y restituir solo mientras el módulo actúa | FogProperties, FogEffect, DayNightFogEffect | SettingsApplier | A01, A03, A09 | Lógica y persistencia verificadas; imagen pendiente |
| Densidad, ruido, viento | Fog Controller | Densidad 0–0.005; ruido 0–2; viento 0–0.05 | Entrada decimal y exportación sin perder 0.0001 | FogProperties | ConfigFile / panel Fog | A01, A02 | Corregido; sin medición de coste |
| Dispersión | Fog Controller / DEFAULT | Exponente efectivo 0.5–100000; intensidad 0–100 | Usar directamente los valores del motor; DEFAULT 59049 | RenderProperties.m_inscatteringExponent | Panel Scatter | A06 | No se confunde el slider legado negativo con el exponente |
| Profundidad estática y volumen | DEFAULT de RenderIt Plus | Altura, inicio, distancia y borde; -1 = no imponer | Separar campos de los dos componentes | FogEffect y RenderProperties | ConfigFile / panel Volume | A07, A08 | Campos nuevos persistidos y aplicados |
| Color de dispersión / volumen | Fog Controller | Auto, ligado al sol, RGB 0–1 | Actualización por watcher cuando corresponde | RenderProperties / DayNightProperties | SettingsApplier / PerFrameWatcher | Inspección + build | Pendiente A/B y transiciones en Unity |
| Modos y persistencia | Encargo actual | VANILLA / OPTIMIZED / ajustes propios | Suspender escrituras, mantener modo y archivo global | XML / snapshots | QuickPresets / ConfigStore | A03–A05, A10, IO01 | Probado con dobles; reinicio real pendiente |

Las pruebas citadas están en `SceneFX/tests/Regression/Program.cs`. Firmas compiladas contra DLL reales; aserciones ejecutadas con dobles, no Unity. UI, imagen, tiempo de respuesta y rendimiento pendientes de observación.

**No se declara paridad total.** Las alturas siguen acotadas a 0–5000; la intensidad de dispersión a 0–100. No equivalen a una entrada ilimitada del legado. No hay importador del XML de Fog Controller. Los modos de color necesitan comprobación visual y medición a exponentes extremos.

Para la cobertura de lo que en Render It! Plus tiene licencia restrictiva -Relight, Fog Controller, Eyecandy X y Daylight Classic, que es GPL-3.0- el documento es `SceneFX/docs/RELEVO-RENDERIT-PLUS.md`.
