using System.Globalization;
using AeroAndes.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

// Cultura invariable: formatos de fecha y decimales consistentes.
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

// Las cadenas de conexión vienen de user-secrets o de variables de entorno, nunca del código fuente.
// Se usan DOS usuarios de base de datos con privilegios distintos (mínimo privilegio):
//   AeroAndesWeb   -> usuario 'app_web'   : sitio público (leer vuelos y destinos, guardar contactos)
//   AeroAndesAdmin -> usuario 'app_admin' : inicio de sesión y panel de administración
string Conexion(string nombre) =>
    builder.Configuration.GetConnectionString(nombre)
    ?? throw new InvalidOperationException(
        $"Falta la cadena de conexión '{nombre}'. Configúrela con 'dotnet user-secrets' (ver LEEME.md).");

var csWeb = Conexion("AeroAndesWeb");
var csAdmin = Conexion("AeroAndesAdmin");

builder.Services.AddDbContext<AeroAndesDbContext>(o => o.UseMySql(csWeb, ServerVersion.AutoDetect(csWeb)));
builder.Services.AddDbContext<AeroAndesAdminDbContext>(o => o.UseMySql(csAdmin, ServerVersion.AutoDetect(csAdmin)));
// Valida automáticamente el token anti-CSRF en todos los POST.
builder.Services.AddControllersWithViews(o =>
    o.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

builder.Services.AddMemoryCache();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Cuenta/Login";
        o.AccessDeniedPath = "/Cuenta/AccesoDenegado";
        o.Cookie.Name = "AeroAndes.Auth";
        o.Cookie.HttpOnly = true;                           // JavaScript no puede leerla
        o.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // solo viaja por HTTPS
        o.Cookie.SameSite = SameSiteMode.Strict;            // protección CSRF
        o.ExpireTimeSpan = TimeSpan.FromMinutes(20);        // expira por inactividad
        o.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Cabeceras de seguridad HTTP.
app.Use(async (context, next) =>
{
    var h = context.Response.Headers;
    h["Content-Security-Policy"] =
        "default-src 'self'; img-src 'self' data:; style-src 'self'; script-src 'self'; " +
        "frame-ancestors 'none'; form-action 'self'; base-uri 'self'";
    h["X-Content-Type-Options"] = "nosniff";
    h["X-Frame-Options"] = "DENY";
    h["Referrer-Policy"] = "strict-origin-when-cross-origin";
    await next();
});

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
