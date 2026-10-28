using System;
using System.Collections.Generic;

namespace AeroAndes.Data.Entities;

public partial class Administradore
{
    public uint Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool? Activo { get; set; }

    public DateTime FechaCreacion { get; set; }
}
