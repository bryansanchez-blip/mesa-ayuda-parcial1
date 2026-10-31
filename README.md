# Mesa de Ayuda - Hito 1

Sistema de mesa de ayuda (Help Desk) desarrollado en C# / .NET 8 como proyecto de consola para la primera evaluación parcial.

## Requisitos de Ejecución
- **.NET 8.0 SDK** instalado en el sistema.
- Terminal / Consola (PowerShell, Command Prompt, Bash).

## Pasos para Ejecutar
1. Clonar o descargar el repositorio.
2. Navegar a la carpeta raíz del proyecto:
   ```bash
   cd MesaDeAyuda
   ```
3. Ejecutar la aplicación mediante CLI:
   ```bash
   dotnet run
   ```

## Funciones Disponibles
- **Registro de Tickets:** Permite crear tickets asignando ID autoincremental, título, descripción y prioridad (Baja, Media, Alta) en estado inicial "Abierto".
- **Validaciones de Registro:** Impide campos de título/descripción vacíos y prioridades no válidas.
- **Listado y Filtros:** Permite ver todos los tickets registrados, solo los abiertos o solo los cerrados.
- **Consulta de Detalle:** Permite seleccionar un ticket por su ID para ver la información completa.
- **Cambio de Estado:** Permite cerrar tickets abiertos. Valida que un ticket previamente cerrado no vuelva a cerrarse.
- **Contadores en Tiempo Real:** Muestra en el menú principal las cantidades totales, de abiertos y de cerrados.

## Documentación del Proyecto
- [hito-0.md](hito-0.md): Propuesta inicial del modelo e interfaz.
- [hito-1.md](hito-1.md): Registro de cambios, reglas de negocio y ajustes implementados.

## Limitaciones Conocidas
- Los datos se almacenan en memoria durante la ejecución (no persisten en base de datos al cerrar la consola).
