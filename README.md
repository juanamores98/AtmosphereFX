# AtmosphereFX

Control de niebla dinÃ¡mica, clÃ¡sica y volumÃ©trica, bordes, dispersiÃ³n y profundidad para Cities: Skylines 1.

## Uso

- Abrir con **Ctrl+Alt+A**, o UUI opcional.
- Panel nativo preferido: **360 Ã— 680**, mÃ­nimo 280 Ã— 260. Secciones: Fog, Scatter, Volume.
- **VANILLA** libera las modificaciones del mÃ³dulo y guarda ese modo. Restaura la referencia capturada respetando compaÃ±eros detectados; un tema u otro mod puede hacer que difiera del vanilla puro.
- **OPTIMIZED** aplica la parte de este mÃ³dulo del **Default personal de RenderIt Plus**. Es una receta de aspecto, no de rendimiento.
- Editar controles guarda un estado personalizado. El pie distingue VANILLA, OPTIMIZED y CUSTOM segÃºn los valores configurados.
- Los sliders incluyen entrada decimal y refresco sin escrituras por repaint.

Niebla dinÃ¡mica apagada; clÃ¡sica activada solo de dÃ­a; niebla volumÃ©trica de RenderProperties apagada, volumen estÃ¡tico de FogEffect activado. Densidad 0.00006, inicio 2852, exponente efectivo 59049, intensidad 1.72. Profundidades estÃ¡ticas y volumÃ©tricas independientes.

## Persistencia

Archivos globales: **AtmosphereFX2.xml**, bajo `%LOCALAPPDATA%\Colossal Order\Cities_Skylines`. Independientes de la partida. Temporal y reemplazo con copia `.bak`, pendientes que se reintentan y guardado al cerrar el anfitriÃ³n. Se conserva lectura desde la ubicaciÃ³n histÃ³rica cuando procede. Un cierre forzado durante el intervalo de guardado puede perder el Ãºltimo cambio pendiente.

Los built-ins no sobrescriben presets ya extraÃ­dos del usuario. Los botones de modo leen la receta incorporada; un antiguo archivo llamado Optimized puede contener valores distintos.

## IncrustaciÃ³n futura

`FxModule.CreatePanel(parent, width, height)` crea el panel dentro de un `UIComponent`. Con padre no tiene arrastre ni botÃ³n de ventana. Ofrece `ReadState`, `ApplyState`, `Release`, `ApplyOptimized`, `Flush`, `Mode` y `Status`. Ver [arquitectura](ARQUITECTURA.md).

No se ha integrado con RenderIt Plus ni Arrebol; tampoco hay dependencia de esos productos.

## Compilar y verificar

```powershell
dotnet build AtmosphereFX.csproj -c Release
```

Target **net35 / C# 7.3**, referencias de CS1 instalado. El build normal genera `bin/Release/net35` y **no instala**. El target de despliegue requiere `DeployMod=true`; solo debe utilizarse con autorizaciÃ³n, juego cerrado y respaldo.

Regresiones conjuntas: `SceneFX/tests/Regression/Regression.csproj`, que enlaza el cÃ³digo actual de los cuatro repos hermanos. Prueba lÃ³gica en .NET 8 con dobles del motor; no valida render ni interacciÃ³n visual.

## LÃ­mites

Las alturas siguen acotadas a 0â€“5000; la intensidad de dispersiÃ³n a 0â€“100. No equivalen a una entrada ilimitada del legado. No hay importador del XML de Fog Controller. Los modos de color necesitan comprobaciÃ³n visual y mediciÃ³n a exponentes extremos.

[Paridad](docs/PARIDAD.md) Â· [Estado](docs/ESTADO-SESION.md) Â· [Procedencia](PROCEDENCIA.md). `DESIGN.md` se conserva como referencia histÃ³rica.

CÃ³digo propio bajo **MIT-0**, [LICENSE](LICENSE).
