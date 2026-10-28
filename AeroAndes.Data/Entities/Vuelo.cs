using System;
using System.Collections.Generic;

namespace AeroAndes.Data.Entities;

public partial class Vuelo
{
    public uint Id { get; set; }

    public string CodigoVuelo { get; set; } = null!;

    public uint OrigenId { get; set; }

    public uint DestinoId { get; set; }

    public DateTime FechaSalida { get; set; }

    public DateTime FechaLlegada { get; set; }

    public decimal PrecioDesde { get; set; }

    public ushort AsientosDisponibles { get; set; }

    public string Estado { get; set; } = null!;

    public bool? Activo { get; set; }

    public virtual Destino Destino { get; set; } = null!;

    public virtual Destino Origen { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
