using AeroAndes.Data;
using AeroAndes.Data.Entities;
using AeroAndes.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AeroAndes.Web.Controllers;

public class HomeController : Controller
{
    private readonly AeroAndesDbContext _db;
    private readonly ILogger<HomeController> _log;

    public HomeController(AeroAndesDbContext db, ILogger<HomeController> log)
    {
        _db = db;
        _log = log;
    }

    public async Task<IActionResult> Index()
    {
        var modelo = new HomeViewModel();
        try
        {
            // Comprobar que la aplicación puede conectarse a aeroandes_db.
            modelo.BaseDatosConectada = await _db.Database.CanConnectAsync();
            if (!modelo.BaseDatosConectada)
            {
                modelo.Error = "No se pudo conectar con la base de datos. Verifique que MySQL esté activo y que la cadena de conexión sea correcta.";
                return View(modelo);
            }

            // Consultar los registros que se mostrarán en la landing page.
            modelo.Destinos = await _db.Destinos.AsNoTracking()
                .Where(d => d.Activo).OrderBy(d => d.Ciudad).ToListAsync();

            modelo.Ofertas = await _db.Vuelos.AsNoTracking()
                .Include(v => v.Origen).Include(v => v.Destino)
                .Where(v => v.Activo && v.Estado == EstadosVuelo.Programado && v.AsientosDisponibles > 0)
                .OrderBy(v => v.FechaSalida).Take(6).ToListAsync();

            modelo.TotalVuelos = await _db.Vuelos.CountAsync(v => v.Activo);
        }
        catch (Exception ex)
        {
            // El detalle técnico va al registro; al usuario se le muestra un mensaje genérico.
            _log.LogError(ex, "Error al consultar la base de datos en la página principal.");
            modelo.BaseDatosConectada = false;
            modelo.Error = "Ocurrió un error al consultar la base de datos. Revise la conexión y que el script aeroandes_db.sql se haya ejecutado.";
        }
        return View(modelo);
    }

    public IActionResult Error() => View();
}
