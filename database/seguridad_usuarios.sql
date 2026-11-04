-- =====================================================================
-- AeroAndes - Semana 10: usuarios de base de datos con MÍNIMO PRIVILEGIO
-- Ejecutar como root (phpMyAdmin > pestaña SQL).
-- ANTES de ejecutar: reemplace las tres claves CAMBIAR_... por claves
-- propias, largas y distintas entre sí. No las suba a repositorios.
-- =====================================================================

CREATE USER IF NOT EXISTS 'app_web'@'localhost'      IDENTIFIED BY 'CAMBIAR_CLAVE_WEB';
CREATE USER IF NOT EXISTS 'app_admin'@'localhost'    IDENTIFIED BY 'CAMBIAR_CLAVE_ADMIN';
CREATE USER IF NOT EXISTS 'app_respaldo'@'localhost' IDENTIFIED BY 'CAMBIAR_CLAVE_RESPALDO';

-- ---------------------------------------------------------------------
-- app_web: sitio público (visitantes). NO puede ver administradores,
-- reservas ni mensajes de contacto; solo leer el catálogo y registrar mensajes.
-- ---------------------------------------------------------------------
GRANT SELECT ON aeroandes_db.destinos TO 'app_web'@'localhost';
GRANT SELECT ON aeroandes_db.vuelos   TO 'app_web'@'localhost';
GRANT INSERT ON aeroandes_db.contactos TO 'app_web'@'localhost';
-- Entity Framework recupera el id recién creado con un SELECT sobre esa columna.
-- Se concede SOLO la columna id, no el contenido de los mensajes.
GRANT SELECT (id) ON aeroandes_db.contactos TO 'app_web'@'localhost';

-- ---------------------------------------------------------------------
-- app_admin: inicio de sesión y panel. Sin DELETE, DROP, ALTER ni GRANT.
-- Solo puede cambiar el estado de contactos y reservas (no sus datos personales).
-- ---------------------------------------------------------------------
GRANT SELECT ON aeroandes_db.administradores TO 'app_admin'@'localhost';
GRANT SELECT ON aeroandes_db.destinos        TO 'app_admin'@'localhost';
GRANT SELECT ON aeroandes_db.vuelos          TO 'app_admin'@'localhost';
GRANT SELECT ON aeroandes_db.reservas        TO 'app_admin'@'localhost';
GRANT SELECT ON aeroandes_db.contactos       TO 'app_admin'@'localhost';
GRANT UPDATE (estado) ON aeroandes_db.contactos TO 'app_admin'@'localhost';
GRANT UPDATE (estado) ON aeroandes_db.reservas  TO 'app_admin'@'localhost';
-- Para el CRUD de vuelos (cuando se implemente), descomente:
-- GRANT INSERT, UPDATE ON aeroandes_db.vuelos   TO 'app_admin'@'localhost';
-- GRANT INSERT, UPDATE ON aeroandes_db.destinos TO 'app_admin'@'localhost';
-- (el borrado se hace de forma lógica con la columna activo, no con DELETE)

-- ---------------------------------------------------------------------
-- app_respaldo: solo para copias de seguridad (lectura).
-- ---------------------------------------------------------------------
GRANT SELECT, LOCK TABLES, SHOW VIEW ON aeroandes_db.* TO 'app_respaldo'@'localhost';

FLUSH PRIVILEGES;

-- ---------------------------------------------------------------------
-- Cuando confirme que la aplicación funciona con los usuarios nuevos,
-- elimine el usuario anterior que tenía permisos de más:
-- DROP USER 'app_aeroandes'@'localhost';
-- ---------------------------------------------------------------------

-- Verificación (para las capturas):
SHOW GRANTS FOR 'app_web'@'localhost';
SHOW GRANTS FOR 'app_admin'@'localhost';
SHOW GRANTS FOR 'app_respaldo'@'localhost';
SELECT user, host FROM mysql.user WHERE user LIKE 'app\_%';
