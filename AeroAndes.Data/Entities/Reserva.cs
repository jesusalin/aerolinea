using System;
using System.Collections.Generic;

namespace AeroAndes.Data.Entities;

public partial class Reserva
{
    public uint Id { get; set; }

    public string CodigoReserva { get; set; } = null!;

    public uint VueloId { get; set; }

    public string NombrePasajero { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string? Telefono { get; set; }

    public byte CantidadPasajeros { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public virtual Vuelo Vuelo { get; set; } = null!;
}
