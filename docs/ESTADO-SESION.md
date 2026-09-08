# Estado de sesión — AtmosphereFX

- Fecha: 2026-09-08.
- Repo: `juanamores98/AtmosphereFX`, rama `main`.
- HEAD de partida: `e5a897e906ea4021ba30a232788d557d9a639a04`.
- Cambios preparados para commit y push a main por autorización del usuario. Árbol de partida limpio; esta entrega conserva las pruebas offline y no instala el mod.
- Encargo: correcciones, VANILLA/OPTIMIZED y panel estrecho incrustable; MIT/MIT-0; sin integración.
- NeuralFX y SkyFX excluidos.

## Entregado

Guardado completo y atómico, precisión de viento, validación antes de aplicar, restitución real al pulsar VANILLA y ampliación del exponente efectivo hasta 100000. Se separan las profundidades de FogEffect y RenderProperties que necesita DEFAULT.

Receta del perfil activo Default. SHA256 del archivo fuente: `71062f3ac84872ca305db47237e5fd7168bf5b5b629cfff9b24eae45ad6bbfba`. Referencia y limitaciones en `SceneFX/docs/VALIDACION.md`.

## Comprobado

- Build net35/C# 7.3: cero errores.
- Cuatro advertencias MSB3245: referencias implícitas System.Data, System.Drawing, System.Runtime.Serialization y System.Xml.Linq no resueltas en este entorno.
- Suite conjunta: **60 PASS, 0 FAIL**, con .NET 8 y dobles. Código y resultados en `SceneFX/tests/Regression`.
- Build sin instalación; DLL instaladas fuera de este cambio.

## Pendiente y próximo paso

Las alturas siguen acotadas a 0–5000; la intensidad de dispersión a 0–100. No equivalen a una entrada ilimitada del legado. No hay importador del XML de Fog Controller. Los modos de color necesitan comprobación visual y medición a exponentes extremos.

Prueba agrupada en el juego con los cuatro binarios de esta revisión, después de autorizar despliegue con respaldo y juego cerrado. Matriz en `SceneFX/docs/VALIDACION.md`: primera/segunda ciudad, reinicio, modos repetidos, LUT presente/ausente, UI estrecha, convivencia y comparación con DEFAULT. Registrar observaciones y medidas antes de aprobar integración o afirmar «100 %».
