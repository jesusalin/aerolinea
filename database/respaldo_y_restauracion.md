# Respaldo y restauración de aeroandes_db (XAMPP / MariaDB en Windows)

Ajuste `C:\xampp` si instaló XAMPP en otra ruta. Use PowerShell o CMD.

## 1. Crear la carpeta de respaldos
```powershell
New-Item -ItemType Directory -Force C:\Respaldos
```
Guarde los respaldos fuera de la carpeta del proyecto y fuera de GitHub:
contienen datos personales y hashes de contraseñas.

## 2. Hacer la copia de seguridad (usuario app_respaldo, solo lectura)
```powershell
cd C:\xampp\mysql\bin
.\mysqldump.exe -u app_respaldo -p --single-transaction --result-file=C:\Respaldos\aeroandes_2026-11-04.sql aeroandes_db
```
- Use `--result-file` y no `>`: en Windows PowerShell el `>` guarda el archivo
  en UTF-16 y se daña la restauración.
- Cambie la fecha del nombre en cada copia.

## 3. Comprobar que se generó
```powershell
Get-Item C:\Respaldos\aeroandes_2026-11-04.sql | Select-Object Name, Length, LastWriteTime
Select-String -Path C:\Respaldos\aeroandes_2026-11-04.sql -Pattern "CREATE TABLE" | Measure-Object
```
Debe mostrar un tamaño mayor que 0 y 5 líneas `CREATE TABLE`.

## 4. Restaurar en una base de PRUEBA (nunca sobre la real)
```powershell
.\mysql.exe -u root -p -e "CREATE DATABASE aeroandes_prueba CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci"
.\mysql.exe -u root -p aeroandes_prueba -e "source C:/Respaldos/aeroandes_2026-11-04.sql"
```

## 5. Verificar la restauración
En phpMyAdmin > SQL:
```sql
SELECT 'administradores' AS tabla, COUNT(*) AS registros FROM aeroandes_prueba.administradores
UNION ALL SELECT 'destinos',  COUNT(*) FROM aeroandes_prueba.destinos
UNION ALL SELECT 'vuelos',    COUNT(*) FROM aeroandes_prueba.vuelos
UNION ALL SELECT 'reservas',  COUNT(*) FROM aeroandes_prueba.reservas
UNION ALL SELECT 'contactos', COUNT(*) FROM aeroandes_prueba.contactos;
```
Los conteos deben coincidir con los de `aeroandes_db`. Después puede eliminar la base de prueba:
```sql
DROP DATABASE aeroandes_prueba;
```

## 6. Protección del respaldo (recomendado)
- Restrinja los permisos de la carpeta `C:\Respaldos` (clic derecho > Propiedades > Seguridad).
- Comprima con contraseña (por ejemplo 7-Zip, cifrado AES-256) antes de copiarlo a un USB o a la nube.
- Mantenga al menos una copia fuera del equipo y pruebe la restauración periódicamente.
