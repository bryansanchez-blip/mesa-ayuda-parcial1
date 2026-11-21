namespace MesaDeAyuda.Services;

public enum TipoResultado
{
    Exito,     // la operación se realizó
    Invalido,  // la entrada o la operación fue rechazada por una regla (el usuario puede corregir)
    Fallo      // error de ejecución (base de datos caída, etc.)
}

/// <summary>Resultado de una operación sin valor de retorno.</summary>
public class Resultado
{
    public TipoResultado Tipo { get; }
    public string Mensaje { get; }
    public bool EsExito => Tipo == TipoResultado.Exito;

    protected Resultado(TipoResultado tipo, string mensaje)
    {
        Tipo = tipo;
        Mensaje = mensaje;
    }

    public static Resultado Ok(string mensaje) => new(TipoResultado.Exito, mensaje);
    public static Resultado Invalido(string mensaje) => new(TipoResultado.Invalido, mensaje);
    public static Resultado Fallo(string mensaje) => new(TipoResultado.Fallo, mensaje);
}

/// <summary>Resultado de una operación que devuelve un valor cuando tiene éxito.</summary>
public class Resultado<T> : Resultado
{
    public T? Valor { get; }

    private Resultado(TipoResultado tipo, string mensaje, T? valor) : base(tipo, mensaje)
    {
        Valor = valor;
    }

    public static Resultado<T> Correcto(T valor) => new(TipoResultado.Exito, "", valor);
    public static Resultado<T> Rechazado(string mensaje) => new(TipoResultado.Invalido, mensaje, default);
    public static Resultado<T> Error(string mensaje) => new(TipoResultado.Fallo, mensaje, default);
}
