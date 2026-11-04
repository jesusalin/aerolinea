using AeroAndes.Data;
using AeroAndes.Data.Entities;
using AeroAndes.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AeroAndes.Web.Controllers;

public class ContactoController : Controller
{
    private const int MaxMensajes = 3;                               // por dirección IP
    private static readonly TimeSpan Ventana = TimeSpan.FromMinutes(10);

    private readonly AeroAndesDbContext _db;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ContactoController> _log;

    public ContactoController(AeroAndesDbContext db, IMemoryCache cache, ILogger<ContactoController> log)
    {
        _db = db; _cache = cache; _log = log;
    }

    [HttpGet]
    public IActionResult Index() => View(new ContactoViewModel());

    // El token anti-CSRF se valida automáticamente (filtro global en Program.cs).
    [HttpPost]
    public async Task<IActionResult> Index(ContactoViewModel m)
    {
        // Campo trampa: si lo llenó un robot, se responde "ok" sin guardar nada.
        if (!string.IsNullOrEmpty(m.Sitio))
        {
            _log.LogWarning("Formulario de contacto descartado por el campo trampa.");
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid) return View(m);

        // Límite de envíos para evitar spam.
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";
        var clave = "contacto:" + ip;
        _cache.TryGetValue(clave, out int enviados);
        if (enviados >= MaxMensajes)
        {
            ModelState.AddModelError("", "Ha enviado varios mensajes seguidos. Espere unos minutos e inténtelo de nuevo.");
            return View(m);
        }

        try
        {
            _db.Contactos.Add(new Contacto
            {
                Nombre = m.Nombre.Trim(),
                Correo = m.Correo.Trim().ToLowerInvariant(),
                Asunto = m.Asunto.Trim(),
                Mensaje = m.Mensaje.Trim(),
                FechaCreacion = DateTime.Now,
                Estado = "pendiente"
            });
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error al guardar el mensaje de contacto.");
            ModelState.AddModelError("", "No pudimos enviar su mensaje. Inténtelo de nuevo más tarde.");
            return View(m);
        }

        _cache.Set(clave, enviados + 1, Ventana);
        TempData["Enviado"] = "¡Gracias! Recibimos su mensaje y lo atenderemos pronto.";
        return RedirectToAction(nameof(Index));   // evita reenviar el formulario al recargar
    }
}
