# Mesa de Ayuda - Hito 2 (persistencia con MySQL)

Aplicación web de mesa de ayuda hecha con **Blazor (Interactive Server)**, **.NET 10** y **MySQL**, con acceso a datos mediante **ADO.NET** (paquete `MySqlConnector`). Amplía el Hito 1 (consola, datos en memoria): ahora los tickets se guardan en la base de datos y sobreviven a cerrar y volver a ejecutar la aplicación.

## Requisitos
- .NET 10 SDK
- MySQL 8.0.16 o superior (local o alojado)
- Cliente `mysql` o MySQL Workbench para ejecutar el script

## Base de datos
El script está en `Database/mesa_ayuda.sql`. Crea la base `mesa_ayuda`, las tablas `categorias` y `tickets`, y datos de demostración (4 categorías y 4 tickets; los tickets de ejemplo solo se insertan si la tabla está vacía).

```bash
mysql -u TU_USUARIO -p < Database/mesa_ayuda.sql
```

Comprobación opcional:
```sql
USE mesa_ayuda;
SELECT COUNT(*) FROM tickets;
```

## Configuración de la conexión
La cadena de conexión se llama `MesaAyuda`. En `appsettings.json` solo hay un valor de ejemplo (`Password=CAMBIAR`). **No escriba su contraseña real en el repositorio.** Use *user secrets*, desde la carpeta del proyecto:

```bash
dotnet user-secrets set "ConnectionStrings:MesaAyuda" "Server=localhost;Port=3306;Database=mesa_ayuda;User ID=TU_USUARIO;Password=TU_CONTRASEÑA;"
```

También sirve una variable de entorno: `ConnectionStrings__MesaAyuda`.

## Ejecución
```bash
dotnet run
```
Abrir http://localhost:5080

## Funciones
- Registrar tickets (título, descripción, categoría y prioridad); nacen en estado Abierto con ID único.
- Listado con detalle completo; al seleccionar otro ticket el detalle cambia.
- Editar (solo tickets abiertos), sin duplicar ni modificar otros.
- Cerrar ticket; un ticket cerrado no puede cerrarse otra vez.
- Eliminar con confirmación (cancelar conserva el ticket).
- Muestra el nombre de la categoría (JOIN) y filtra por estado y/o categoría, con mensaje cuando no hay coincidencias y botón "Quitar filtros".
- Cantidades total / abiertos / cerrados del conjunto completo (no cambian con el filtro).
- Validación (título y descripción obligatorios, prioridad y categoría válidas) y mensajes distintos para éxito, entrada inválida y fallo de base de datos.
- Persistencia: lo registrado, editado, cerrado o eliminado se conserva al reiniciar.

## Estructura
- `Models/`: `Ticket` (reglas y encapsulamiento), `Categoria`, `ConteoTickets`.
- `Data/`: `TicketRepository` (ADO.NET, comandos parametrizados).
- `Services/`: `TicketService` (coordina modelo y repositorio) y `Resultado`.
- `Components/Pages/Tickets.razor`: interfaz Blazor.
- `Database/mesa_ayuda.sql`: script de la base de datos.

## Limitaciones conocidas
- Un ticket cerrado no puede editarse ni reabrirse.
- No hay autenticación de usuarios.
- No hay paginación del listado.
- Volver a ejecutar el script no duplica categorías ni tickets de ejemplo, pero tampoco restaura tickets que se hayan eliminado.
