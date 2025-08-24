# Instrucciones para GitHub Copilot

## Guías Prioritarias

Al generar código para este repositorio:

1. Compatibilidad de Versiones: Detecta y respeta siempre las versiones exactas de lenguajes, frameworks y librerías usadas en este proyecto.
2. Archivos de Contexto: Prioriza los patrones y estándares definidos en este archivo y cualquier otro en `.github/copilot`.
3. Patrones de la Base de Código: Cuando los archivos de contexto no den una guía específica, escanea la base de código para identificar patrones establecidos.
4. Consistencia Arquitectónica: Mantén nuestro estilo arquitectónico Por Capas (MVVM sobre Avalonia) y los límites establecidos entre UI (XAML), ViewModels, Logic y Models.
5. Calidad de Código: Prioriza mantenibilidad y testabilidad en todo el código generado, respetando las convenciones existentes.

## Detección de Versiones Tecnológicas

Antes de generar código, respeta estas versiones detectadas explícitamente del proyecto:

1. Versiones de Lenguaje
   - C# de acuerdo a `net8.0` (usa solo características válidas para C# compatible con .NET 8; no asumas previsualizaciones ni nuevas APIs no referenciadas).

2. Versiones de Frameworks
   - .NET Target Framework: `net8.0` (ver `MusicTaggingLight.csproj`).
   - Avalonia UI: `11.0.7` (paquetes: `Avalonia`, `Avalonia.Desktop`, `Avalonia.Themes.Fluent`, `Avalonia.Fonts.Inter`, `Avalonia.ReactiveUI`, `Avalonia.Controls.DataGrid`).

3. Versiones de Librerías
   - `CommunityToolkit.Mvvm` v8.2.2
   - `taglib-sharp-netstandard2.0` v2.1.0
   - `System.Drawing.Common` v6.0.0
   - `System.Configuration.ConfigurationManager` v6.0.0

Nunca uses APIs o características no disponibles en estas versiones.

## Archivos de Contexto

Prioriza los archivos dentro de `.github/copilot` si existen:
- `architecture.md`, `tech-stack.md`, `coding-standards.md`, `folder-structure.md`, `exemplars.md`.
- Si no existen, usa este archivo y los patrones reales del código como fuente de verdad.

## Instrucciones de Escaneo de la Base de Código

Cuando no haya guía específica:
1. Identifica archivos similares al que se modifica o crea.
2. Analiza patrones de:
   - Convenciones de nombres: Clases en PascalCase, propiedades en PascalCase, campos privados con guion bajo (`_campo`).
   - Organización: Capas en carpetas `UI/`, `ViewModels/`, `Logic/`, `Models/` y raíz para `App`, `Program`, `MainWindow`.
   - Manejo de errores: Uso de clase `Result`/`Result<T>` y `Status` enum; captura de excepciones específicas (`CorruptFileException` en `TaggingLogic`). Notificaciones de UI a través de `MainWindowViewModel.SetNotification` con texto y color.
   - Comandos: `RelayCommand`/`AsyncRelayCommand` del `CommunityToolkit.Mvvm`.
   - Vistas: XAML con Avalonia 11; `DataGrid` de `Avalonia.Controls.DataGrid`; enlaces con `{Binding ...}`.
   - Documentación: Comentarios XML en modelos y lógica cuando aporta claridad (seguir estilo existente, no sobre-documentar).
3. Sigue los patrones más consistentes. No introduzcas patrones que no existan (p. ej., no introducir DI frameworks, logging externos, ni navegación compleja).

## Estándares de Calidad de Código

### Mantenibilidad
- Nombres claros y autoexplicativos, en PascalCase para clases/propiedades y camelCase para variables locales.
- Mantén funciones enfocadas; extrae métodos si exceden responsabilidad única.
- Mantén la separación de capas: UI (XAML), `ViewModels` para estado y comandos, `Logic` para operaciones de dominio/IO, `Models` para datos y conversión con TagLib.

### Rendimiento
- Carga de archivos: procesa solo `*.mp3` como en `TaggingLogic` y filtra usando `Regex` existente; evita cargas innecesarias.
- Evita asignaciones repetidas dentro de bucles; usa `AddRange` ya provisto para colecciones.

### Seguridad
- No manipules rutas sin validación básica cuando se escriben cambios (renombrado de archivos respeta directorio actual).
- No introduzcas manejo de credenciales ni red a menos que se implemente explícitamente.

### Testabilidad
- Métodos con salidas claras usando `Result`/`Result<T>` para reportar estado.
- Evita acoplar UI directamente con lógica nueva; expón acciones vía comandos y delegados como ya se hace en `MainWindowViewModel`.

## Requisitos de Documentación
- Igualar el estilo de comentarios XML en `Models` y `Logic` donde clarifica intención pública.
- Documentar parámetros y retornos solo cuando el comportamiento no sea trivial.

## Enfoque de Pruebas
- No existen pruebas en el repositorio; al agregar código lógico no trivial, estructura métodos para facilitar pruebas unitarias aisladas (por ejemplo, pure functions y `Result`).

## Guías .NET
- Alinea el código a .NET 8 con Avalonia 11.0.7.
- Usa `CommunityToolkit.Mvvm` para propiedades observables (`ObservableObject`, `SetProperty`) y comandos (`RelayCommand`, `AsyncRelayCommand`).
- Mantén el patrón de comandos: inicialización en `InitCommands()` y binding en XAML.
- En `ViewModels`, expón propiedades con `SetProperty` y evita lógica pesada en getters.

## Guías Específicas de Avalonia/MVVM
- Vistas (`*.axaml`) definen layout y bindings; `Window.DataContext` establece el `ViewModel`.
- Usa `GridLength` y bindings como en `DetColWidth` para mostrar/ocultar paneles.
- Maneja interacciones de UI que requieren APIs de plataforma (diálogos) mediante delegados expuestos por el VM y ejecutados en la Vista (como `SelectRootFolderFunc`).

## Guías de Control de Versiones
- Mantén compatibilidad con las versiones listadas del `.csproj` y evita nuevas dependencias sin actualizar este archivo.
- Si actualizas paquetes, fija versiones explícitas y actualiza esta sección.

## Mejores Prácticas Generales
- Sigue la organización en carpetas existente.
- Reutiliza `Result`/`Result<T>` para errores y estados.
- Evita introducir frameworks de DI, ORMs u otras librerías no presentes.
- No uses características no presentes en Avalonia 11.0.7 (p. ej., controles o APIs de versiones posteriores).

## Guía Específica del Proyecto
- Respeta estrictamente los límites entre `UI`, `ViewModels`, `Logic` y `Models`.
- Cuando agregues nuevos comandos, sigue el patrón de `MainWindowViewModel` (propiedades `ICommand` + inicialización en `InitCommands`).
- Para nuevas operaciones sobre tags, extiende `TaggingLogic` y retorna `Result/Result<T>`.
- Para conversión TagLib, usa métodos estáticos en `MusicFileTag` como referencia de estilo.
