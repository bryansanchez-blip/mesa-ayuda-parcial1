using MesaDeAyuda.Models;
using MySqlConnector;

namespace MesaDeAyuda.Data;

/// <summary>Acceso a datos con ADO.NET. Todos los comandos usan parámetros (@nombre), nunca concatenan valores.</summary>
public class TicketRepository
{
    private const string SelectTickets =
        @"SELECT t.id, t.titulo, t.descripcion, t.categoria_id, c.nombre, t.prioridad, t.estado
          FROM tickets t
          INNER JOIN categorias c ON c.id = t.categoria_id";

    private readonly string? _cadenaConexion;

    public TicketRepository(IConfiguration configuracion)
    {
        _cadenaConexion = configuracion.GetConnectionString("MesaAyuda");
    }

    private async Task<MySqlConnection> AbrirConexionAsync()
    {
        if (string.IsNullOrWhiteSpace(_cadenaConexion))
            throw new InvalidOperationException("Falta la cadena de conexión 'MesaAyuda'.");
        var conexion = new MySqlConnection(_cadenaConexion);
        await conexion.OpenAsync();
        return conexion;
    }

    public async Task<List<Categoria>> ObtenerCategoriasAsync()
    {
        var lista = new List<Categoria>();
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new MySqlCommand("SELECT id, nombre FROM categorias ORDER BY nombre", conexion);
        await using var lector = await comando.ExecuteReaderAsync();
        while (await lector.ReadAsync())
            lista.Add(new Categoria(lector.GetInt32(0), lector.GetString(1)));
        return lista;
    }

    public async Task<bool> ExisteCategoriaAsync(int categoriaId)
    {
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new MySqlCommand("SELECT COUNT(*) FROM categorias WHERE id = @id", conexion);
        comando.Parameters.AddWithValue("@id", categoriaId);
        object? resultado = await comando.ExecuteScalarAsync();
        return Convert.ToInt32(resultado) > 0;
    }

    public async Task<List<Ticket>> ListarAsync(EstadoTicket? estado, int? categoriaId)
    {
        var lista = new List<Ticket>();
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new MySqlCommand(
            SelectTickets + @" WHERE (@estado IS NULL OR t.estado = @estado)
                               AND (@categoriaId IS NULL OR t.categoria_id = @categoriaId)
                               ORDER BY t.id", conexion);
        comando.Parameters.AddWithValue("@estado", estado is null ? DBNull.Value : estado.ToString());
        comando.Parameters.AddWithValue("@categoriaId", categoriaId is null ? DBNull.Value : categoriaId.Value);
        await using var lector = await comando.ExecuteReaderAsync();
        while (await lector.ReadAsync())
            lista.Add(Mapear(lector));
        return lista;
    }

    public async Task<Ticket?> ObtenerPorIdAsync(int id)
    {
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new MySqlCommand(SelectTickets + " WHERE t.id = @id", conexion);
        comando.Parameters.AddWithValue("@id", id);
        await using var lector = await comando.ExecuteReaderAsync();
        return await lector.ReadAsync() ? Mapear(lector) : null;
    }

    public async Task<ConteoTickets> ContarAsync()
    {
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new MySqlCommand(
            @"SELECT COUNT(*),
                     COALESCE(SUM(estado = 'Abierto'), 0),
                     COALESCE(SUM(estado = 'Cerrado'), 0)
              FROM tickets", conexion);
        await using var lector = await comando.ExecuteReaderAsync();
        await lector.ReadAsync();
        return new ConteoTickets(
            Convert.ToInt32(lector.GetValue(0)),
            Convert.ToInt32(lector.GetValue(1)),
            Convert.ToInt32(lector.GetValue(2)));
    }

    /// <summary>Inserta el ticket y devuelve el identificador generado por MySQL.</summary>
    public async Task<int> InsertarAsync(Ticket ticket)
    {
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new MySqlCommand(
            @"INSERT INTO tickets (titulo, descripcion, categoria_id, prioridad, estado)
              VALUES (@titulo, @descripcion, @categoriaId, @prioridad, @estado)", conexion);
        comando.Parameters.AddWithValue("@titulo", ticket.Titulo);
        comando.Parameters.AddWithValue("@descripcion", ticket.Descripcion);
        comando.Parameters.AddWithValue("@categoriaId", ticket.CategoriaId);
        comando.Parameters.AddWithValue("@prioridad", ticket.Prioridad);
        comando.Parameters.AddWithValue("@estado", ticket.Estado.ToString());
        await comando.ExecuteNonQueryAsync();
        return (int)comando.LastInsertedId;
    }

    /// <summary>Actualiza datos y estado. Devuelve las filas afectadas (0 = el ticket ya no existe).</summary>
    public async Task<int> ActualizarAsync(Ticket ticket)
    {
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new MySqlCommand(
            @"UPDATE tickets
              SET titulo = @titulo, descripcion = @descripcion, categoria_id = @categoriaId,
                  prioridad = @prioridad, estado = @estado
              WHERE id = @id", conexion);
        comando.Parameters.AddWithValue("@titulo", ticket.Titulo);
        comando.Parameters.AddWithValue("@descripcion", ticket.Descripcion);
        comando.Parameters.AddWithValue("@categoriaId", ticket.CategoriaId);
        comando.Parameters.AddWithValue("@prioridad", ticket.Prioridad);
        comando.Parameters.AddWithValue("@estado", ticket.Estado.ToString());
        comando.Parameters.AddWithValue("@id", ticket.Id);
        return await comando.ExecuteNonQueryAsync();
    }

    public async Task<int> EliminarAsync(int id)
    {
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new MySqlCommand("DELETE FROM tickets WHERE id = @id", conexion);
        comando.Parameters.AddWithValue("@id", id);
        return await comando.ExecuteNonQueryAsync();
    }

    private static Ticket Mapear(MySqlDataReader lector)
    {
        return Ticket.Reconstruir(
            id: lector.GetInt32(0),
            titulo: lector.GetString(1),
            descripcion: lector.GetString(2),
            categoriaId: lector.GetInt32(3),
            categoriaNombre: lector.GetString(4),
            prioridad: lector.GetString(5),
            estado: Enum.Parse<EstadoTicket>(lector.GetString(6)));
    }
}
