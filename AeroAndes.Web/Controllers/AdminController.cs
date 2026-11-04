using AeroAndes.Data;
using AeroAndes.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AeroAndes.Web.Controllers;

public class AdminResumen
{
    public int Vuelos { get; set; }
    public int Reservas { get; set; }
    public List<Contacto> Contactos { get; set; } = new();
}

[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly AeroAndesDbContext _db;
    public AdminController(AeroAndesDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var m = new AdminResumen
        {
            Vuelos = await _db.Vuelos.CountAsync(),
            Reservas = await _db.Reservas.CountAsync(),
            Contactos = await _db.Contactos.AsNoTracking()
                .OrderByDescending(c => c.FechaCreacion).Take(10).ToListAsync()
        };
        return View(m);
    }
}