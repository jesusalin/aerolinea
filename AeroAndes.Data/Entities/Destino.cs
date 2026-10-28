using System;
using System.Collections.Generic;

namespace AeroAndes.Data.Entities;

public partial class Destino
{
    public uint Id { get; set; }

    public string Ciudad { get; set; } = null!;

    public string Pais { get; set; } = null!;

    public string CodigoAeropuerto { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? ImagenUrl { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Vuelo> VueloDestinos { get; set; } = new List<Vuelo>();

    public virtual ICollection<Vuelo> VueloOrigens { get; set; } = new List<Vuelo>();
}
