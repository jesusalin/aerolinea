using AeroAndes.Data;
using AeroAndes.Data.Entities;
using AeroAndes.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AeroAndes.Web.Controllers;

public class VuelosController : Controller
{
    private readonly AeroAndesDbContext _db;
    private readonly ILogger<VuelosController> _log;

    public VuelosController(AeroAndesDbContext db, ILogger<VuelosController> log)
    {
        _db = db;
        _log = log;
    }

    // Ejemplo: /Vuelos/Buscar?origenId=1&destinoId=2&fecha=2026-11-10&pasajeros=2
    public async Task<IActionResult> Buscar(int? origenId, int? destinoId, DateTime? fecha, int pasajeros = 1)
    {
        pasajeros = Math.Clamp(pasajeros, 1, 9);
        var modelo = new BusquedaViewModel { OrigenId = origenId, DestinoId = destinoId, Fecha = fecha, Pasajeros = pasajeros };
        try
        {
            // Todas las consultas usan parámetros (Entity Framework): no hay concatenación de SQL.
            var consulta = _db.Vuelos.AsNoTracking()
                .Include(v => v.Origen).Include(v => v.Destino)
                .Where(v => v.Activo && v.Estado == EstadosVuelo.Programado && v.AsientosDisponibles >= pasajeros);

            if (origenId.HasValue) consulta = consulta.Where(v => v.OrigenId == origenId.Value);
            if (destinoId.HasValue) consulta = consulta.Where(v => v.DestinoId == destinoId.Value);
            if (fecha.HasValue)
            {
                var desde = fecha.Value.Date;
                consulta = consulta.Where(v => v.FechaSalida >= desde);
            }

            modelo.Destinos = await _db.Destinos.AsNoTracking().Where(d => d.Activo).OrderBy(d => d.Ciudad).ToListAsync();
            modelo.Vuelos = await consulta.OrderBy(v => v.FechaSalida).Take(50).ToListAsync();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error al buscar vuelos.");
            modelo.Error = "No se pudo consultar la base de datos. Inténtelo de nuevo más tarde.";
        }
        return View(modelo);
    }
}
