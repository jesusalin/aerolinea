using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroAndes.Data.Entities;

[Table("reservas")]
public class Reserva
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("codigo_reserva")] public string CodigoReserva { get; set; } = "";
    [Column("vuelo_id")] public int VueloId { get; set; }
    [Column("nombre_pasajero")] public string NombrePasajero { get; set; } = "";
    [Column("correo")] public string Correo { get; set; } = "";
    [Column("telefono")] public string? Telefono { get; set; }
    [Column("cantidad_pasajeros")] public int CantidadPasajeros { get; set; } = 1;
    [Column("estado")] public string Estado { get; set; } = "pendiente";
    [Column("fecha_creacion")] public DateTime FechaCreacion { get; set; }

    [ForeignKey(nameof(VueloId))] public Vuelo? Vuelo { get; set; }
}
