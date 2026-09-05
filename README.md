# AtmosphereFX v2

Control independiente de niebla y atmósfera para **Cities: Skylines**.
Desarrollo original de **juanamores98** — segunda versión, con modelo de ajuste propio.

## Qué hace

**Dynamic Fog** (niebla dinámica día/noche):
- Activar/desactivar la niebla dinámica.
- Color decay (0–1), densidad (0–0.005), ruido (0–2): rangos más amplios que los valores por defecto del juego.
- Techo de niebla y línea de horizonte por slider (0–5000).
- Distancia de inicio (0–10000) y velocidad de deriva (0–0.05, control flotante real).
- Edge fog.

**Cubemap Fog** (niebla clásica por cubemap):
- Activación manual y apagado automático nocturno.
- Volume fog con *scatter falloff* directo (0.5–10, sin transformaciones intermedias) y *scatter strength* (0–5).
- Color de dispersión: automático, igualado al sol en tiempo real o personalizado (RGB).
- Color del volume fog: automático o personalizado (RGB), y distancia de inicio independiente (0–4000).

**General**:
- Aplicar automáticamente al cargar un mapa (nuevo en v2).
- Restablecer todo a valores vanilla con un clic.

Todos los cambios se aplican en vivo y se guardan en `AtmosphereFX2.xml`.

## Compatibilidad

- Cities: Skylines en Windows / Linux / macOS. Sin DLCs requeridos.
- Funciona con el ciclo día/noche activado o desactivado.
- No parchea métodos del juego: solo escribe valores en los componentes de render nativos.
- Puede solaparse con otros mods de atmósfera/luz: gana el último en escribir.

## Requisitos

- **No requiere Harmony** ni ninguna otra librería externa.
- Compila contra .NET Framework 3.5 (el runtime Mono del juego lo provee).

## Instalación

Copiar `AtmosphereFX.dll` a:

```
%LOCALAPPDATA%\Colossal Order\Cities_Skylines\Addons\Mods\AtmosphereFX\
```

## Compilación

```
dotnet build -c Release
```

El build despliega el DLL automáticamente a la carpeta de mods locales.

## Licencia

[MIT-0](https://spdx.org/licenses/MIT-0.html) (MIT No Attribution) © 2026 juanamores98.
Uso, copia, modificación, venta, distribución y sublicencia sin atribución ni condiciones.
