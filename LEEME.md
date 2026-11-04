# AeroAndes – Semana 8: desarrollo inicial del proyecto web con C# y MySQL

Proyecto académico de Seguridad de la Información (aerolínea ficticia). Crea el proyecto ASP.NET Core, configura una conexión protegida con MySQL y muestra en la página principal los datos de la base `aeroandes_db` de la semana 7.

## Estructura de la solución

```
AeroAndes.sln
├── AeroAndes.Data   Biblioteca de clase: entidades (Destino, Vuelo, ...) y AeroAndesDbContext (Entity Framework Core + MySQL)
├── AeroAndes.Web    Aplicación ASP.NET Core MVC: controladores, vistas y estilos
└── database/        aeroandes_db.sql (script de la semana 7)
```

## Requisitos

- .NET SDK 8.0
- MySQL 8.0.16 o superior (o MariaDB 10.5 o superior)
- Visual Studio 2022 o Visual Studio Code

## Pasos para ejecutarlo

### 1. Crear la base de datos
Ejecute `database/aeroandes_db.sql` en MySQL Workbench (File > Open SQL Script > rayo) o con:

```
mysql -u root -p < database/aeroandes_db.sql
```

### 2. (Recomendado) Crear un usuario con privilegios mínimos
La aplicación no debe usar la cuenta `root`. En esta etapa solo necesita leer datos:

```sql
CREATE USER 'app_aeroandes'@'localhost' IDENTIFIED BY 'ESCRIBA_AQUI_UNA_CLAVE_PROPIA';
GRANT SELECT ON aeroandes_db.* TO 'app_aeroandes'@'localhost';
```

### 3. Guardar la cadena de conexión de forma protegida
La cadena de conexión NO está en el código ni en `appsettings.json`: se guarda en los secretos de usuario (fuera del proyecto y de GitHub). Desde la carpeta de la solución:

```
dotnet user-secrets set "ConnectionStrings:AeroAndes" "Server=localhost;Port=3306;Database=aeroandes_db;User=app_aeroandes;Password=SU_CLAVE" --project AeroAndes.Web
```

Cambie `SU_CLAVE` por la clave del paso 2. En Visual Studio también puede usar clic derecho sobre `AeroAndes.Web` > "Administrar secretos de usuario" y pegar:

```json
{ "ConnectionStrings": { "AeroAndes": "Server=localhost;Port=3306;Database=aeroandes_db;User=app_aeroandes;Password=SU_CLAVE" } }
```

### 4. Ejecutar
Los paquetes se descargan solos (Pomelo.EntityFrameworkCore.MySql 8.0.2). Para confiar en el certificado HTTPS de desarrollo (una sola vez) y ejecutar:

```
dotnet dev-certs https --trust
dotnet run --project AeroAndes.Web --launch-profile https
```

Abra la dirección de la consola (por ejemplo, https://localhost:7225). En Visual Studio basta con abrir `AeroAndes.sln` y presionar F5.

### 5. Qué debe ver (y capturar para la entrega)
- Una insignia verde: "Base de datos aeroandes_db: conectada · 6 destinos · 7 vuelos".
- "Próximos vuelos" con tarjetas de los vuelos programados.
- "Destinos" con una tarjeta por cada ciudad.
- En el menú, "Vuelos" permite buscar por origen, destino y fecha.

Tome la captura de la página principal mostrando la insignia y las tarjetas.

## Cómo funciona (resumen para la exposición)

1. `AeroAndes.Data/Entities/*.cs`: una clase por tabla; `[Table]` y `[Column]` las asocian a la base de datos.
2. `AeroAndes.Data/AeroAndesDbContext.cs`: contexto de Entity Framework Core con un `DbSet` por tabla.
3. `AeroAndes.Web/Program.cs`: lee la cadena de conexión de la configuración protegida y registra el contexto con `UseMySql`.
4. `HomeController`: comprueba la conexión (`CanConnectAsync`), consulta destinos y vuelos y los envía a la vista. Si hay un error, muestra un mensaje genérico y deja el detalle técnico en el registro.
5. `Views/Home/Index.cshtml`: muestra los datos en tarjetas.

## Medidas de seguridad aplicadas en esta etapa

- Cadena de conexión fuera del código (user-secrets o variables de entorno).
- Usuario de base de datos con privilegios mínimos (solo lectura).
- Consultas con Entity Framework (parametrizadas): sin concatenar SQL.
- HTTPS con redirección; HSTS fuera de desarrollo.
- Cabeceras: Content-Security-Policy, X-Frame-Options, X-Content-Type-Options y Referrer-Policy.
- Mensajes de error genéricos para el usuario.
- Datos de prueba ficticios.

## Solución de problemas

| Mensaje o síntoma | Causa probable | Qué hacer |
|---|---|---|
| "Falta la cadena de conexión 'AeroAndes'" | No se configuró el secreto | Repita el paso 3 |
| Insignia roja "sin conexión" | MySQL apagado, clave o puerto incorrectos | Verifique el servicio MySQL y la cadena de conexión |
| "Access denied for user" | Usuario o clave incorrectos | Revise el usuario del paso 2 y la clave del secreto |
| "Unknown database 'aeroandes_db'" | No se ejecutó el script | Repita el paso 1 |
| La página sale con 0 destinos | El script no insertó datos | Ejecute de nuevo el script completo |
| Error de certificado SSL con MySQL | MySQL con SSL sin certificado confiable | Agregue `SslMode=None` a la cadena, solo en desarrollo local |
| Aviso de certificado HTTPS del navegador | No se confió en el certificado de desarrollo | Ejecute `dotnet dev-certs https --trust` |
| Tiene .NET 9 o 10 | El proyecto apunta a net8.0 | Instale el SDK 8.0, o cambie `TargetFramework` y las versiones de los paquetes |

Los mensajes técnicos completos aparecen en la consola donde ejecutó `dotnet run`.

## Verificación realizada

El código C# y las vistas compilaron sin errores ni advertencias, y las páginas respondieron correctamente en pruebas locales con componentes simulados de Entity Framework. **No se probó contra un servidor MySQL real ni con los paquetes oficiales de NuGet**, porque el entorno de desarrollo no tenía acceso a ellos. Si al ejecutarlo aparece algún error, conserve el mensaje completo para corregirlo.
