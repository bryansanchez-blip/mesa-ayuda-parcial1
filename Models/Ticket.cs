namespace MesaDeAyuda.Models;

public enum EstadoTicket
{
    Abierto,
    Cerrado
}

/// <summary>Se lanza cuando un cambio viola una regla del modelo (dato inválido o estado no permitido).</summary>
public class ReglaDeNegocioException : Exception
{
    public ReglaDeNegocioException(string mensaje) : base(mensaje) { }
}

public record Categoria(int Id, string Nombre);

public record ConteoTickets(int Total, int Abiertos, int Cerrados);

/// <summary>
/// Modelo del ticket. Los setters son privados: la única forma de cambiar el estado
/// es mediante el constructor, Editar y Cerrar, que aplican las reglas.
/// </summary>
public class Ticket
{
    public const int LongitudMaximaTitulo = 100;
    public const int LongitudMaximaDescripcion = 500; // NUEVO
    public static readonly IReadOnlyList<string> PrioridadesPermitidas = new[] { "Baja", "Media", "Alta" };

    public int Id { get; private set; }
    public string Titulo { get; private set; } = "";
    public string Descripcion { get; private set; } = "";
    public int CategoriaId { get; private set; }
    public string? CategoriaNombre { get; private set; }
    public string Prioridad { get; private set; } = "";
    public EstadoTicket Estado { get; private set; } = EstadoTicket.Abierto;

    public bool EstaCerrado => Estado == EstadoTicket.Cerrado;

    /// <summary>Crea un ticket nuevo: siempre nace Abierto y con datos válidos.</summary>
    public Ticket(string? titulo, string? descripcion, int categoriaId, string? prioridad)
    {
        AplicarDatos(titulo, descripcion, categoriaId, prioridad);
        Estado = EstadoTicket.Abierto;
    }

    private Ticket() { }

    /// <summary>Reconstruye un ticket leído de la base de datos (ya fue validado al guardarse).</summary>
    public static Ticket Reconstruir(int id, string titulo, string descripcion, int categoriaId,
                                     string? categoriaNombre, string prioridad, EstadoTicket estado)
    {
        return new Ticket
        {
            Id = id,
            Titulo = titulo,
            Descripcion = descripcion,
            CategoriaId = categoriaId,
            CategoriaNombre = categoriaNombre,
            Prioridad = prioridad,
            Estado = estado
        };
    }

    public void Editar(string? titulo, string? descripcion, int categoriaId, string? prioridad)
    {
        if (EstaCerrado)
            throw new ReglaDeNegocioException("Un ticket cerrado no puede editarse.");
        AplicarDatos(titulo, descripcion, categoriaId, prioridad);
    }

    public void Cerrar()
    {
        if (EstaCerrado)
            throw new ReglaDeNegocioException("El ticket ya está cerrado y no puede cerrarse otra vez.");
        Estado = EstadoTicket.Cerrado;
    }

    // Valida TODO antes de asignar nada, para que un cambio inválido no deje el objeto a medias.
    private void AplicarDatos(string? titulo, string? descripcion, int categoriaId, string? prioridad)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ReglaDeNegocioException("El título no puede estar vacío.");
        if (titulo.Trim().Length > LongitudMaximaTitulo)
            throw new ReglaDeNegocioException($"El título no puede superar {LongitudMaximaTitulo} caracteres.");
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ReglaDeNegocioException("La descripción no puede estar vacía.");
        if (descripcion.Trim().Length > LongitudMaximaDescripcion) // NUEVO
            throw new ReglaDeNegocioException($"La descripción no puede superar {LongitudMaximaDescripcion} caracteres."); // NUEVO
        if (categoriaId <= 0)
            throw new ReglaDeNegocioException("Seleccione una categoría.");

        string? prioridadValida = PrioridadesPermitidas
            .FirstOrDefault(p => p.Equals(prioridad?.Trim(), StringComparison.OrdinalIgnoreCase));
        if (prioridadValida is null)
            throw new ReglaDeNegocioException("Prioridad no permitida. Debe ser: Baja, Media o Alta.");

        Titulo = titulo.Trim();
        Descripcion = descripcion.Trim();
        CategoriaId = categoriaId;
        CategoriaNombre = null; // el nombre se obtiene de la base al recargar
        Prioridad = prioridadValida;
    }
}