using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroAndes.Data.Entities;

[Table("contactos")]
public class Contacto
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("nombre")] public string Nombre { get; set; } = "";
    [Column("correo")] public string Correo { get; set; } = "";
    [Column("asunto")] public string Asunto { get; set; } = "";
    [Column("mensaje")] public string Mensaje { get; set; } = "";
    [Column("fecha_creacion")] public DateTime FechaCreacion { get; set; }
    [Column("estado")] public string Estado { get; set; } = "pendiente";
}
