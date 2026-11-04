using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroAndes.Data.Entities;

[Table("destinos")]
public class Destino
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("ciudad")] public string Ciudad { get; set; } = "";
    [Column("pais")] public string Pais { get; set; } = "";
    [Column("codigo_aeropuerto")] public string CodigoAeropuerto { get; set; } = "";
    [Column("descripcion")] public string? Descripcion { get; set; }
    [Column("imagen_url")] public string? ImagenUrl { get; set; }
    [Column("activo")] public bool Activo { get; set; } = true;

    [NotMapped] public string Etiqueta => $"{Ciudad} ({CodigoAeropuerto})";
}
