using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroAndes.Data.Entities;

[Table("vuelos")]
public class Vuelo
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("codigo_vuelo")] public string CodigoVuelo { get; set; } = "";
    [Column("origen_id")] public int OrigenId { get; set; }
    [Column("destino_id")] public int DestinoId { get; set; }
    [Column("fecha_salida")] public DateTime FechaSalida { get; set; }
    [Column("fecha_llegada")] public DateTime FechaLlegada { get; set; }
    [Column("precio_desde")] public decimal PrecioDesde { get; set; }
    [Column("asientos_disponibles")] public int AsientosDisponibles { get; set; }
    [Column("estado")] public string Estado { get; set; } = EstadosVuelo.Programado;
    [Column("activo")] public bool Activo { get; set; } = true;

    [ForeignKey(nameof(OrigenId))] public Destino? Origen { get; set; }
    [ForeignKey(nameof(DestinoId))] public Destino? Destino { get; set; }
}
