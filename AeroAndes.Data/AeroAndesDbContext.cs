using AeroAndes.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AeroAndes.Data;

/// <summary>
/// Contexto de Entity Framework Core para la base de datos aeroandes_db.
/// Las tablas y columnas se asignan mediante atributos en las entidades.
/// </summary>
public class AeroAndesDbContext : DbContext
{
    public AeroAndesDbContext(DbContextOptions<AeroAndesDbContext> options) : base(options) { }

    public DbSet<Administrador> Administradores => Set<Administrador>();
    public DbSet<Destino> Destinos => Set<Destino>();
    public DbSet<Vuelo> Vuelos => Set<Vuelo>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<Contacto> Contactos => Set<Contacto>();
}
