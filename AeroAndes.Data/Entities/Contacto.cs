using System;
using System.Collections.Generic;

namespace AeroAndes.Data.Entities;

public partial class Contacto
{
    public uint Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Asunto { get; set; } = null!;

    public string Mensaje { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public string Estado { get; set; } = null!;
}
