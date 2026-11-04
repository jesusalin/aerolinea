using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AeroAndes.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AeroAndes.Web.Controllers;

public class LoginViewModel
{
    [Required(ErrorMessage = "Ingrese su correo.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(150)]
    public string Correo { get; set; } = "";

    [Required(ErrorMessage = "Ingrese su contraseña.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";
}

public class CuentaController : Controller
{
    private const int MaxIntentos = 5;
    private static readonly TimeSpan Bloqueo = TimeSpan.FromMinutes(5);
    // Hash falso: se verifica cuando el correo no existe, para que la respuesta tarde igual.
    private const string HashFalso = "$2b$12$AL54.0AbT7IAYVBEg2n0kOUs34iGEI66qHX547qn4xbzA/CXvcJZ.";

    private readonly AeroAndesDbContext _db;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CuentaController> _log;

    public CuentaController(AeroAndesDbContext db, IMemoryCache cache, ILogger<CuentaController> log)
    {
        _db = db; _cache = cache; _log = log;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Admin");
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel m, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(m);

        var correo = m.Correo.Trim().ToLowerInvariant();
        var clave = "login:" + correo;

        if (_cache.TryGetValue(clave, out int intentos) && intentos >= MaxIntentos)
        {
            _log.LogWarning("Cuenta bloqueada temporalmente por intentos fallidos: {Correo}", correo);
            ModelState.AddModelError("", "Demasiados intentos fallidos. Espere 5 minutos e inténtelo de nuevo.");
            return View(m);
        }

        var admin = await _db.Administradores.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Correo == correo && a.Activo);

        var hashValido = BCrypt.Net.BCrypt.Verify(m.Password, admin?.PasswordHash ?? HashFalso);

        if (admin is null || !hashValido)
        {
            _cache.Set(clave, intentos + 1, Bloqueo);
            _log.LogWarning("Inicio de sesión fallido para {Correo}", correo);
            // Mismo mensaje exista o no el correo: no revela qué cuentas hay.
            ModelState.AddModelError("", "Correo o contraseña incorrectos.");
            return View(m);
        }

        _cache.Remove(clave);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, admin.Id.ToString()),
            new(ClaimTypes.Name, admin.Nombre),
            new(ClaimTypes.Email, admin.Correo),
            new(ClaimTypes.Role, "Administrador")
        };
        var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identidad));

        _log.LogInformation("Inicio de sesión correcto: {Correo}", correo);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);   // evita redirecciones a sitios externos
        return RedirectToAction("Index", "Admin");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccesoDenegado() => View();
}