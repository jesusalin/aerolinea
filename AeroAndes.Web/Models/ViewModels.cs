using AeroAndes.Data.Entities;

namespace AeroAndes.Web.Models;

/// <summary>Datos de la página principal. Si hay un problema de base de datos, Error trae un mensaje amigable.</summary>
public class HomeViewModel
{
    public List<Destino> Destinos { get; set; } = new();
    public List<Vuelo> Ofertas { get; set; } = new();
    public bool BaseDatosConectada { get; set; }
    public int TotalVuelos { get; set; }
    public string? Error { get; set; }
}

public class BusquedaViewModel
{
    public int? OrigenId { get; set; }
    public int? DestinoId { get; set; }
    public DateTime? Fecha { get; set; }
    public int Pasajeros { get; set; } = 1;
    public List<Destino> Destinos { get; set; } = new();
    public List<Vuelo> Vuelos { get; set; } = new();
    public string? Error { get; set; }
}
