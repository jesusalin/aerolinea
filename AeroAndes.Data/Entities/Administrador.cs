using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroAndes.Data.Entities;

[Table("administradores")]
public class Administrador
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("nombre")] public string Nombre { get; set; } = "";
    [Column("correo")] public string Correo { get; set; } = "";
    /// <summary>Hash bcrypt de la contraseña. Nunca se guarda texto plano.</summary>
    [Column("password_hash")] public string PasswordHash { get; set; } = "";
    [Column("activo")] public bool Activo { get; set; } = true;
    [Column("fecha_creacion")] public DateTime FechaCreacion { get; set; }
}
