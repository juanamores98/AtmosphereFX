# AtmosphereFX — arquitectura y cambios

Documento ejecutivo. Estado a día de hoy. `DESIGN.md` es la especificación
funcional original; este documento describe cómo está construido el mod hoy y
qué cambió en el último ciclo.

## Qué manda este mod

Dentro de la suite FX cada propiedad del juego tiene **un solo dueño**.
AtmosphereFX es el dueño de **toda la niebla**:

| Materia | Campos del juego |
|---|---|
| Niebla de escena | `FogProperties` — densidad, color decay, ruido, altura, horizonte, inicio |
| Niebla dinámica día/noche | `FogEffect`, `DayNightFogEffect` |
| Niebla volumétrica | `RenderProperties.m_volumeFogDensity`, `m_volumeFogStart`, `m_volumeFogDistance`, `m_volumeFogColor` |
| Niebla de borde | `m_edgeFogDistance`, y por separado su variante de cubemap |
| Inscattering | `m_inscatteringColor`, sincronizado con el sol cada fotograma |

SceneFX no escribe niebla: cuando este mod está cargado se la pide por
`ApplySuiteSection`. Si no lo está, SceneFX la escribe por su cuenta.

## Piezas

```
Source/
  AtmosphereFXMod.cs           IUserMod + API de suite (25 etiquetas)
  Runtime/
    AtmosphereEngine.cs        MonoBehaviour anfitrión, Ctrl+Alt+A
    PerFrameWatcher.cs         lo que hay que reescribir cada fotograma
    SettingsApplier.cs         lleva la configuración a los componentes
    VanillaSnapshot.cs         la foto del juego sin tocar, para poder volver
  Config/
    ModConfig.cs               el estado completo, campos estáticos
    ConfigFile.cs              persistencia XML con recorte de rangos
    QuickPresets.cs            Vanilla y Optimized de un clic
  UI/FogWindow.cs              ventana IMGUI de 2 pestañas
  Options/OptionsPanel.cs      opciones dentro del menú del juego
```

### Por qué hay un vigilante por fotograma

El juego reescribe algunos de estos campos en su propio bucle. `PerFrameWatcher`
vuelve a poner los valores del mod después, y de paso sincroniza el color de
inscattering con la posición del sol —que es lo que hace que la dispersión
cambie de tono al amanecer en vez de quedarse fija.

### Los dos bordes de niebla

`edgeFogDynamic` y `edgeFogCubemap` son dos cosas distintas que el mod original
mezclaba en un solo interruptor: la niebla de borde de la escena y la del
cubemap del cielo. Se pueden apagar por separado. La etiqueta `edgefog` sigue
aceptándose al aplicar como alias que mueve las dos a la vez —no se exporta,
porque exportar las dos mitades es más preciso.

## API de suite

`ApplySuiteSection` / `ExportSuiteSection`, públicas y estáticas. 25 etiquetas
exportadas, más el alias `edgefog` de solo entrada:

```
dynamicFog colorDecay density noise fogHeight horizonHeight startDistance
windSpeed edgeFogDynamic edgeFogCubemap cubemapFog offAtNight volumeFog
scatterFalloff scatterStrength scatterColorMode scatterR scatterG scatterB
autoVolumeColor volumeR volumeG volumeB volumeStart vanillaMode
```

`scatterColorMode`: 0 automático, 1 igualado al sol, 2 personalizado.

## Dónde guarda las cosas

`%LOCALAPPDATA%\Colossal Order\Cities_Skylines\AtmosphereFX2.xml`. Ruta
completa, no relativa: un nombre suelto acababa dentro de Archivos de Programa.
Si queda un archivo en el sitio antiguo y todavía no hay ninguno en el nuevo, se
lee el antiguo.

## Qué cambió en este ciclo

**Dos pestañas en vez de un muro.** *Dynamic fog & horizon* y *Volumetrics &
scattering*, cada una con su propio scroll. La altura del contenido se mide
dentro del área desplazable, que es donde tiene sentido medirla.

**Botón de un clic para quitar la neblina azul.** Pone la fuerza de dispersión a
cero y el color de inscattering en un gris neutro (0,5 / 0,5 / 0,5) en modo
personalizado. Es el tinte violáceo del horizonte que a mucha gente le sobra, y
quitarlo a mano son cuatro controles en dos sitios distintos.

**Vanilla y Optimized** de un clic, por el mismo camino que un perfil de suite
guardado —con sus validaciones y sus efectos inmediatos—, no por un atajo que se
desincronizaría en cuanto se añadiera un ajuste nuevo.

## Correcciones de la revisión

- **Sin emoji.** La fuente Arial de Unity 5.6 no lleva pictogramas, y los del
  plano astral llegan como pares sustitutos que IMGUI de esa versión no compone.
  Comprobado sobre 12.960 ficheros `.cs` de mods que ya funcionan: ninguno los
  usa; `▼` y `►` sí, en 79 y 54 ficheros.
- **La ventana vuelve a estar entera en inglés.** Se habían colado diecisiete
  literales en castellano junto a los deslizadores en inglés que nadie tocó.
- **`ToggleWindow()`** — método público y estático, para que SceneFX pueda abrir
  este panel desde su pestaña de clima. El botón que lo intentaba apuntaba a un
  método que no existía y anunciaba un atajo equivocado; el atajo es Ctrl+Alt+A.
- **La aislación de los deslizadores sigue en pie** (verificado): cada
  deslizador guarda `GUI.changed`, lo pone a falso, dibuja y lo restaura. Sin
  eso, redondear al paso reescribía la configuración sin que el usuario tocara
  nada.

## Atajos

- `Ctrl+Alt+A` — ventana del mod.

## Licencia

MIT-0 © 2026 juanamores98. Sin atribución ni condiciones.
