using System.ComponentModel.DataAnnotations;
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
    public string Orden { get; set; } = "precio";
    public int? IdMasBarato { get; set; }
    public int? IdMasRapido { get; set; }
    public List<DiaTarifa> Dias { get; set; } = new();
    public List<Destino> Destinos { get; set; } = new();
    public List<Vuelo> Vuelos { get; set; } = new();
    public string? Error { get; set; }
}

/// <summary>Un día de la barra de fechas con el precio más bajo disponible (null = sin vuelos).</summary>
public class DiaTarifa
{
    public DateTime Dia { get; set; }
    public decimal? PrecioMinimo { get; set; }
}

/// <summary>Formulario de contacto: las reglas se validan siempre en el servidor.</summary>
public class ContactoViewModel
{
    [Required(ErrorMessage = "Ingrese su nombre.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    [RegularExpression(@"^[\p{L}\p{M}][\p{L}\p{M}' .\-]*$", ErrorMessage = "El nombre solo puede tener letras, espacios, apóstrofes, puntos y guiones.")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "Ingrese su correo.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(150, ErrorMessage = "El correo no puede superar 150 caracteres.")]
    public string Correo { get; set; } = "";

    [Required(ErrorMessage = "Ingrese el asunto.")]
    [StringLength(150, MinimumLength = 5, ErrorMessage = "El asunto debe tener entre 5 y 150 caracteres.")]
    public string Asunto { get; set; } = "";

    [Required(ErrorMessage = "Escriba su mensaje.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "El mensaje debe tener entre 10 y 2000 caracteres.")]
    [DataType(DataType.MultilineText)]
    public string Mensaje { get; set; } = "";

    /// <summary>Campo trampa (honeypot): una persona real no lo ve ni lo llena.</summary>
    public string? Sitio { get; set; }
}
