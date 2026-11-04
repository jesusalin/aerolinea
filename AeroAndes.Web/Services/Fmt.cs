namespace AeroAndes.Web.Services;

/// <summary>Funciones de formato para las vistas (cultura invariable).</summary>
public static class Fmt
{
    public static string Fecha(DateTime d) => d.ToString("dd/MM/yyyy");
    public static string Hora(DateTime d) => d.ToString("HH:mm");
    public static string Precio(decimal p) => "S/ " + p.ToString("N2");

    public static string Duracion(DateTime salida, DateTime llegada)
    {
        var t = llegada - salida;
        return $"{(int)t.TotalHours} h {t.Minutes:00} min";
    }
}
