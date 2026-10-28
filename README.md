# AeroAndes

Sistema web seguro de aerolínea con ASP.NET Core MVC y MySQL.
Curso: Seguridad de la Información (Semana 7) · Estudiante: Alin Jesus

## Estructura

- `AeroAndes` → aplicación web ASP.NET Core MVC (controladores, vistas).
- `AeroAndes.Data` → biblioteca de clases (entidades y `AeroAndesDbContext`).
- `aeroandes_db.sql` → script de la base de datos.

## Requisitos

- .NET 8 SDK y Visual Studio
- XAMPP (MariaDB/MySQL)

## Cómo ejecutarlo

1. Ejecutar `aeroandes_db.sql` en MySQL/MariaDB (por ejemplo, desde phpMyAdmin > Importar).
2. Crear el usuario de la aplicación con permisos mínimos:

   ```sql
   CREATE USER 'app_aeroandes'@'localhost' IDENTIFIED BY 'SU_CLAVE';
   GRANT SELECT, INSERT, UPDATE ON aeroandes_db.* TO 'app_aeroandes'@'localhost';
   FLUSH PRIVILEGES;
   ```

3. Configurar la cadena de conexión con **user-secrets** (clic derecho en el proyecto `AeroAndes` > *Administrar secretos de usuario*):

   ```json
   {
     "ConnectionStrings": {
       "AeroAndesDb": "Server=localhost;Port=3306;Database=aeroandes_db;User=app_aeroandes;Password=SU_CLAVE"
     }
   }
   ```

4. Abrir `AeroAndes.sln` y ejecutar con F5.

## Seguridad

- La cadena de conexión no está en el código: se guarda en user-secrets.
- La aplicación usa un usuario con permisos mínimos, no `root`.
- Todos los datos de prueba son ficticios.