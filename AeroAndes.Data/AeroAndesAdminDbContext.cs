using AeroAndes.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AeroAndes.Data;

/// <summary>
/// Contexto que usa SOLO el panel de administración y el inicio de sesión.
/// Se conecta con el usuario de base de datos 'app_admin' (el único que puede leer la tabla administradores).
/// El sitio público usa AeroAndesDbContext con el usuario 'app_web', que tiene menos privilegios.
/// </summary>
public class AeroAndesAdminDbContext : DbContext
{
    public AeroAndesAdminDbContext(DbContextOptions<AeroAndesAdminDbContext> options) : base(options) { }

    public DbSet<Administrador> Administradores => Set<Administrador>();
    public DbSet<Destino> Destinos => Set<Destino>();
    public DbSet<Vuelo> Vuelos => Set<Vuelo>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<Contacto> Contactos => Set<Contacto>();
}
