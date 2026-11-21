using MesaDeAyuda.Data;
using MesaDeAyuda.Models;

namespace MesaDeAyuda.Services;

/// <summary>
/// Lógica de la aplicación: coordina el modelo (reglas) y el repositorio (base de datos),
/// y traduce excepciones a un Resultado que la interfaz puede mostrar.
/// </summary>
public class TicketService
{
    private const string MensajeFallo =
        "No se pudo completar la operación por un problema con la base de datos. Intente de nuevo y, si persiste, revise la conexión.";

    private readonly TicketRepository _repositorio;
    private readonly ILogger<TicketService> _log;

    public TicketService(TicketRepository repositorio, ILogger<TicketService> log)
    {
        _repositorio = repositorio;
        _log = log;
    }

    public Task<Resultado<List<Categoria>>> ObtenerCategoriasAsync() =>
        EjecutarConValorAsync(async () => Resultado<List<Categoria>>.Correcto(await _repositorio.ObtenerCategoriasAsync()));

    public Task<Resultado<List<Ticket>>> ListarAsync(EstadoTicket? estado, int? categoriaId) =>
        EjecutarConValorAsync(async () => Resultado<List<Ticket>>.Correcto(await _repositorio.ListarAsync(estado, categoriaId)));

    public Task<Resultado<ConteoTickets>> ContarAsync() =>
        EjecutarConValorAsync(async () => Resultado<ConteoTickets>.Correcto(await _repositorio.ContarAsync()));

    public Task<Resultado<Ticket>> ObtenerAsync(int id) =>
        EjecutarConValorAsync(async () =>
        {
            Ticket? ticket = await _repositorio.ObtenerPorIdAsync(id);
            return ticket is null
                ? Resultado<Ticket>.Rechazado("El ticket seleccionado ya no existe.")
                : Resultado<Ticket>.Correcto(ticket);
        });

    public Task<Resultado> CrearAsync(string? titulo, string? descripcion, int categoriaId, string? prioridad) =>
        EjecutarAsync(async () =>
        {
            var ticket = new Ticket(titulo, descripcion, categoriaId, prioridad); // valida las reglas del modelo
            if (!await _repositorio.ExisteCategoriaAsync(categoriaId))
                return Resultado.Invalido("La categoría seleccionada no existe.");
            int id = await _repositorio.InsertarAsync(ticket);
            return Resultado.Ok($"Ticket #{id} registrado con estado Abierto.");
        });

    public Task<Resultado> EditarAsync(int id, string? titulo, string? descripcion, int categoriaId, string? prioridad) =>
        EjecutarAsync(async () =>
        {
            Ticket? ticket = await _repositorio.ObtenerPorIdAsync(id);
            if (ticket is null)
                return Resultado.Invalido("El ticket ya no existe.");
            ticket.Editar(titulo, descripcion, categoriaId, prioridad); // lanza si es inválido o está cerrado
            if (!await _repositorio.ExisteCategoriaAsync(categoriaId))
                return Resultado.Invalido("La categoría seleccionada no existe.");
            int filas = await _repositorio.ActualizarAsync(ticket);
            return filas > 0
                ? Resultado.Ok($"Ticket #{id} actualizado.")
                : Resultado.Invalido("El ticket ya no existe.");
        });

    public Task<Resultado> CerrarAsync(int id) =>
        EjecutarAsync(async () =>
        {
            Ticket? ticket = await _repositorio.ObtenerPorIdAsync(id);
            if (ticket is null)
                return Resultado.Invalido("El ticket ya no existe.");
            ticket.Cerrar(); // lanza si ya estaba cerrado
            int filas = await _repositorio.ActualizarAsync(ticket);
            return filas > 0
                ? Resultado.Ok($"Ticket #{id} cerrado.")
                : Resultado.Invalido("El ticket ya no existe.");
        });

    public Task<Resultado> EliminarAsync(int id) =>
        EjecutarAsync(async () =>
        {
            int filas = await _repositorio.EliminarAsync(id);
            return filas > 0
                ? Resultado.Ok($"Ticket #{id} eliminado.")
                : Resultado.Invalido("El ticket ya no existe.");
        });

    // Regla rechazada por el modelo => Invalido (el usuario corrige). Cualquier otra excepción => Fallo.
    private async Task<Resultado> EjecutarAsync(Func<Task<Resultado>> accion)
    {
        try
        {
            return await accion();
        }
        catch (ReglaDeNegocioException ex)
        {
            return Resultado.Invalido(ex.Message);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error de ejecución en TicketService");
            return Resultado.Fallo(MensajeFallo);
        }
    }

    private async Task<Resultado<T>> EjecutarConValorAsync<T>(Func<Task<Resultado<T>>> accion)
    {
        try
        {
            return await accion();
        }
        catch (ReglaDeNegocioException ex)
        {
            return Resultado<T>.Rechazado(ex.Message);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error de ejecución en TicketService");
            return Resultado<T>.Error(MensajeFallo);
        }
    }
}
