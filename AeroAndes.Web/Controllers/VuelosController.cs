using AeroAndes.Data;
using AeroAndes.Data.Entities;
using AeroAndes.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AeroAndes.Web.Controllers;

public class VuelosController : Controller
{
    // Lista blanca: solo se aceptan estos criterios de orden.
    private static readonly string[] OrdenesValidos = { "precio", "salida", "duracion" };

    private readonly AeroAndesDbContext _db;
    private readonly ILogger<VuelosController> _log;

    public VuelosController(AeroAndesDbContext db, ILogger<VuelosController> log)
    {
        _db = db;
        _log = log;
    }

    // Ejemplo: /Vuelos/Buscar?origenId=1&destinoId=2&fecha=2026-11-10&pasajeros=2&orden=precio
    public async Task<IActionResult> Buscar(int? origenId, int? destinoId, DateTime? fecha, int pasajeros = 1, string orden = "precio")
    {
        pasajeros = Math.Clamp(pasajeros, 1, 9);
        if (!OrdenesValidos.Contains(orden)) orden = "precio";

        var modelo = new BusquedaViewModel
        {
            OrigenId = origenId, DestinoId = destinoId, Fecha = fecha, Pasajeros = pasajeros, Orden = orden
        };

        try
        {
            modelo.Destinos = await _db.Destinos.AsNoTracking().Where(d => d.Activo).OrderBy(d => d.Ciudad).ToListAsync();

            // Todas las consultas usan parámetros (Entity Framework): no hay concatenación de SQL.
            var baseQ = _db.Vuelos.AsNoTracking()
                .Where(v => v.Activo && v.Estado == EstadosVuelo.Programado && v.AsientosDisponibles >= pasajeros);
            if (origenId.HasValue) baseQ = baseQ.Where(v => v.OrigenId == origenId.Value);
            if (destinoId.HasValue) baseQ = baseQ.Where(v => v.DestinoId == destinoId.Value);

            IQueryable<Vuelo> consulta = baseQ.Include(v => v.Origen).Include(v => v.Destino);

            if (fecha.HasValue)
            {
                var dia = fecha.Value.Date;
                var siguiente = dia.AddDays(1);
                consulta = consulta.Where(v => v.FechaSalida >= dia && v.FechaSalida < siguiente);

                // Barra de fechas: 3 días antes y 3 después, con el precio más bajo de cada día.
                var ini = dia.AddDays(-3);
                var fin = dia.AddDays(4);
                var cercanos = await baseQ.Where(v => v.FechaSalida >= ini && v.FechaSalida < fin)
                    .Select(v => new { v.FechaSalida, v.PrecioDesde })
                    .ToListAsync();
                for (var i = 0; i < 7; i++)
                {
                    var d = ini.AddDays(i);
                    var delDia = cercanos.Where(c => c.FechaSalida.Date == d).ToList();
                    modelo.Dias.Add(new DiaTarifa
                    {
                        Dia = d,
                        PrecioMinimo = delDia.Count > 0 ? delDia.Min(c => c.PrecioDesde) : (decimal?)null
                    });
                }
            }

            var lista = await consulta.OrderBy(v => v.FechaSalida).Take(50).ToListAsync();

            modelo.Vuelos = orden switch
            {
                "salida" => lista.OrderBy(v => v.FechaSalida).ToList(),
                "duracion" => lista.OrderBy(v => v.FechaLlegada - v.FechaSalida).ThenBy(v => v.PrecioDesde).ToList(),
                _ => lista.OrderBy(v => v.PrecioDesde).ThenBy(v => v.FechaSalida).ToList()
            };

            if (lista.Count > 1)
            {
                modelo.IdMasBarato = lista.OrderBy(v => v.PrecioDesde).First().Id;
                modelo.IdMasRapido = lista.OrderBy(v => v.FechaLlegada - v.FechaSalida).First().Id;
            }
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error al buscar vuelos.");
            modelo.Error = "No se pudo consultar la base de datos. Inténtelo de nuevo más tarde.";
        }
        return View(modelo);
    }
}
