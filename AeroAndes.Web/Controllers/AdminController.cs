using AeroAndes.Data;
using AeroAndes.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AeroAndes.Web.Controllers;

public class AdminResumen
{
    public int VuelosActivos { get; set; }
    public int ReservasTotal { get; set; }
    public int ReservasPendientes { get; set; }
    public int ContactosPendientes { get; set; }
    public List<Vuelo> ProximosVuelos { get; set; } = new();
    public List<Reserva> UltimasReservas { get; set; } = new();
    public List<Contacto> UltimosContactos { get; set; } = new();
    public string? Error { get; set; }
}

[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly AeroAndesAdminDbContext _db;
    private readonly ILogger<AdminController> _log;

    public AdminController(AeroAndesAdminDbContext db, ILogger<AdminController> log)
    {
        _db = db;
        _log = log;
    }

    public async Task<IActionResult> Index()
    {
        var m = new AdminResumen();
        try
        {
            m.VuelosActivos = await _db.Vuelos.CountAsync(v => v.Activo && v.Estado == EstadosVuelo.Programado);
            m.ReservasTotal = await _db.Reservas.CountAsync();
            m.ReservasPendientes = await _db.Reservas.CountAsync(r => r.Estado == "pendiente");
            m.ContactosPendientes = await _db.Contactos.CountAsync(c => c.Estado == "pendiente");

            m.ProximosVuelos = await _db.Vuelos.AsNoTracking()
                .Include(v => v.Origen).Include(v => v.Destino)
                .Where(v => v.Activo)
                .OrderBy(v => v.FechaSalida).Take(5).ToListAsync();

            m.UltimasReservas = await _db.Reservas.AsNoTracking()
                .Include(r => r.Vuelo)
                .OrderByDescending(r => r.FechaCreacion).Take(5).ToListAsync();

            m.UltimosContactos = await _db.Contactos.AsNoTracking()
                .OrderByDescending(c => c.FechaCreacion).Take(5).ToListAsync();
        }
        catch (Exception ex)
        {
            // El detalle técnico va al registro; al usuario, un mensaje genérico.
            _log.LogError(ex, "Error al cargar el panel de administración.");
            m.Error = "No se pudo cargar la información del panel. Inténtelo de nuevo más tarde.";
        }
        return View(m);
    }
}