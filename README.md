# AtmosphereFX v2

Control independiente de niebla y atmÃ³sfera para **Cities: Skylines**.
Desarrollo original de **juanamores98** â€” segunda versiÃ³n, con modelo de ajuste propio.

## QuÃ© hace

**Dynamic Fog** (niebla dinÃ¡mica dÃ­a/noche):
- Activar/desactivar la niebla dinÃ¡mica.
- Color decay (0â€“1), densidad (0â€“0.005), ruido (0â€“2): rangos mÃ¡s amplios que los valores por defecto del juego.
- Techo de niebla y lÃ­nea de horizonte por slider (0â€“5000).
- Distancia de inicio (0â€“10000) y velocidad de deriva (0â€“0.05, control flotante real).
- Edge fog.

**Cubemap Fog** (niebla clÃ¡sica por cubemap):
- ActivaciÃ³n manual y apagado automÃ¡tico nocturno.
- Volume fog con *scatter falloff* directo (0.5â€“10, sin transformaciones intermedias) y *scatter strength* (0â€“5).
- Color de dispersiÃ³n: automÃ¡tico, igualado al sol en tiempo real o personalizado (RGB).
- Color del volume fog: automÃ¡tico o personalizado (RGB), y distancia de inicio independiente (0â€“4000).

**General**:
- Aplicar automÃ¡ticamente al cargar un mapa (nuevo en v2).
- Restablecer todo a valores vanilla con un clic.

Todos los cambios se aplican en vivo y se guardan en `AtmosphereFX2.xml`.

## Compatibilidad

- Cities: Skylines en Windows / Linux / macOS. Sin DLCs requeridos.
- Funciona con el ciclo dÃ­a/noche activado o desactivado.
- No parchea mÃ©todos del juego: solo escribe valores en los componentes de render nativos.
- Puede solaparse con otros mods de atmÃ³sfera/luz: gana el Ãºltimo en escribir.

## Requisitos

- **No requiere Harmony** ni ninguna otra librerÃ­a externa.
- Compila contra .NET Framework 3.5 (el runtime Mono del juego lo provee).

## InstalaciÃ³n

Copiar `AtmosphereFX.dll` a:

```
%LOCALAPPDATA%\Colossal Order\Cities_Skylines\Addons\Mods\AtmosphereFX\
```

## CompilaciÃ³n

```
dotnet build -c Release
```

El build despliega el DLL automÃ¡ticamente a la carpeta de mods locales.

## Licencia

[MIT-0](https://spdx.org/licenses/MIT-0.html) (MIT No Attribution) Â© 2026 juanamores98.
Uso, copia, modificaciÃ³n, venta, distribuciÃ³n y sublicencia sin atribuciÃ³n ni condiciones.
