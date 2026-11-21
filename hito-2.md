# Hito 2: Persistencia de datos

## Cambios respecto al Hito 1
- La aplicación pasó de consola a **Blazor (Interactive Server)** sobre .NET 10.
- Los tickets se guardan en **MySQL** con **ADO.NET**; ya no se pierden al cerrar.
- Nueva tabla `categorias` y relación con `tickets` (llave foránea) para mostrar el nombre de la categoría.
- Nuevas operaciones: editar y eliminar con confirmación.

## Decisiones de diseño
- **Responsabilidades separadas:** interfaz (`Tickets.razor`), lógica (`TicketService`), modelo (`Ticket`) y acceso a datos (`TicketRepository`).
- **Encapsulamiento:** `Ticket` tiene setters privados; solo el constructor, `Editar` y `Cerrar` cambian su estado y validan antes de asignar.
- **Seguridad:** todos los comandos SQL usan parámetros; la contraseña va en user secrets, no en el repositorio.
- **Resultados:** `Resultado` distingue Éxito, Inválido (regla rechazada) y Fallo (error de ejecución).

## Guía de demostración (3 minutos)
1. Mostrar listado y cantidades. Seleccionar dos tickets y ver que el detalle cambia.
2. Intentar guardar un ticket con el título vacío (rechazo) y luego crear uno válido.
3. Editar ese ticket y cerrarlo; el botón de cerrar queda deshabilitado.
4. Filtrar por estado y por categoría; filtrar algo sin resultados; quitar filtros. Las cantidades no cambian.
5. Eliminar: cancelar, y luego confirmar.
6. Detener la aplicación y volver a ejecutarla: los datos siguen ahí.
7. Error previsible: detener el servicio de MySQL y pulsar un botón; la interfaz muestra el fallo sin dar éxito.
