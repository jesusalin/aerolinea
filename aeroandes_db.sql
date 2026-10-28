-- =====================================================================
-- Proyecto: Sistema web seguro de aerolínea con C# y MySQL
-- Aerolínea ficticia: AeroAndes
-- Curso: Seguridad de la Información (Semana 7) - Estudiante: Alin Jesus
-- Base de datos: aeroandes_db
-- Compatible con MySQL 8.0.16 o superior (y MariaDB 10.5 o superior)
-- IMPORTANTE: todos los datos son FICTICIOS. No usar datos personales
-- ni contraseñas reales.
-- =====================================================================

CREATE DATABASE IF NOT EXISTS aeroandes_db
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE aeroandes_db;

-- Permite volver a ejecutar el script desde cero
SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS reservas;
DROP TABLE IF EXISTS vuelos;
DROP TABLE IF EXISTS destinos;
DROP TABLE IF EXISTS contactos;
DROP TABLE IF EXISTS administradores;
SET FOREIGN_KEY_CHECKS = 1;

-- ---------------------------------------------------------------------
-- 1. administradores: cuentas autorizadas para ingresar al panel
-- ---------------------------------------------------------------------
CREATE TABLE administradores (
  id              INT UNSIGNED  NOT NULL AUTO_INCREMENT,
  nombre          VARCHAR(100)  NOT NULL,
  correo          VARCHAR(150)  NOT NULL,
  password_hash   VARCHAR(255)  NOT NULL,   -- nunca texto plano (bcrypt/Argon2id)
  activo          TINYINT(1)    NOT NULL DEFAULT 1,
  fecha_creacion  DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT pk_administradores PRIMARY KEY (id),
  CONSTRAINT uq_administradores_correo UNIQUE (correo),
  CONSTRAINT ck_administradores_activo CHECK (activo IN (0, 1))
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- 2. destinos: ciudades y aeropuertos que se muestran en la landing page
-- ---------------------------------------------------------------------
CREATE TABLE destinos (
  id                 INT UNSIGNED  NOT NULL AUTO_INCREMENT,
  ciudad             VARCHAR(80)   NOT NULL,
  pais               VARCHAR(60)   NOT NULL,
  codigo_aeropuerto  CHAR(3)       NOT NULL,   -- código IATA de 3 letras
  descripcion        VARCHAR(300)  NULL,
  imagen_url         VARCHAR(255)  NULL,
  activo             TINYINT(1)    NOT NULL DEFAULT 1,
  CONSTRAINT pk_destinos PRIMARY KEY (id),
  CONSTRAINT uq_destinos_codigo UNIQUE (codigo_aeropuerto),
  CONSTRAINT ck_destinos_activo CHECK (activo IN (0, 1))
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- 3. vuelos: vuelos programados que se publican en la landing page
-- ---------------------------------------------------------------------
CREATE TABLE vuelos (
  id                    INT UNSIGNED   NOT NULL AUTO_INCREMENT,
  codigo_vuelo          VARCHAR(10)    NOT NULL,
  origen_id             INT UNSIGNED   NOT NULL,
  destino_id            INT UNSIGNED   NOT NULL,
  fecha_salida          DATETIME       NOT NULL,
  fecha_llegada         DATETIME       NOT NULL,
  precio_desde          DECIMAL(10,2)  NOT NULL,
  asientos_disponibles  SMALLINT UNSIGNED NOT NULL DEFAULT 0,
  estado                ENUM('programado','retrasado','cancelado','finalizado')
                        NOT NULL DEFAULT 'programado',
  activo                TINYINT(1)     NOT NULL DEFAULT 1,
  CONSTRAINT pk_vuelos PRIMARY KEY (id),
  CONSTRAINT uq_vuelos_codigo UNIQUE (codigo_vuelo),
  CONSTRAINT fk_vuelos_origen  FOREIGN KEY (origen_id)  REFERENCES destinos (id),
  CONSTRAINT fk_vuelos_destino FOREIGN KEY (destino_id) REFERENCES destinos (id),
  CONSTRAINT ck_vuelos_ruta    CHECK (origen_id <> destino_id),
  CONSTRAINT ck_vuelos_fechas  CHECK (fecha_llegada > fecha_salida),
  CONSTRAINT ck_vuelos_precio  CHECK (precio_desde >= 0),
  CONSTRAINT ck_vuelos_activo  CHECK (activo IN (0, 1))
) ENGINE=InnoDB;

CREATE INDEX ix_vuelos_ruta_fecha ON vuelos (origen_id, destino_id, fecha_salida);

-- ---------------------------------------------------------------------
-- 4. reservas: solicitudes de reserva enviadas desde el formulario público
-- ---------------------------------------------------------------------
CREATE TABLE reservas (
  id                  INT UNSIGNED  NOT NULL AUTO_INCREMENT,
  codigo_reserva      VARCHAR(10)   NOT NULL,
  vuelo_id            INT UNSIGNED  NOT NULL,
  nombre_pasajero     VARCHAR(120)  NOT NULL,
  correo              VARCHAR(150)  NOT NULL,
  telefono            VARCHAR(20)   NULL,
  cantidad_pasajeros  TINYINT UNSIGNED NOT NULL DEFAULT 1,
  estado              ENUM('pendiente','confirmada','cancelada')
                      NOT NULL DEFAULT 'pendiente',
  fecha_creacion      DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT pk_reservas PRIMARY KEY (id),
  CONSTRAINT uq_reservas_codigo UNIQUE (codigo_reserva),
  CONSTRAINT fk_reservas_vuelo FOREIGN KEY (vuelo_id) REFERENCES vuelos (id),
  CONSTRAINT ck_reservas_cantidad CHECK (cantidad_pasajeros BETWEEN 1 AND 9)
) ENGINE=InnoDB;

CREATE INDEX ix_reservas_vuelo  ON reservas (vuelo_id);
CREATE INDEX ix_reservas_correo ON reservas (correo);

-- ---------------------------------------------------------------------
-- 5. contactos: mensajes enviados desde el formulario de contacto
-- ---------------------------------------------------------------------
CREATE TABLE contactos (
  id              INT UNSIGNED  NOT NULL AUTO_INCREMENT,
  nombre          VARCHAR(100)  NOT NULL,
  correo          VARCHAR(150)  NOT NULL,
  asunto          VARCHAR(150)  NOT NULL,
  mensaje         TEXT          NOT NULL,
  fecha_creacion  DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
  estado          ENUM('pendiente','atendido','cerrado')
                  NOT NULL DEFAULT 'pendiente',
  CONSTRAINT pk_contactos PRIMARY KEY (id)
) ENGINE=InnoDB;

-- =====================================================================
-- DATOS FICTICIOS DE PRUEBA
-- =====================================================================

-- Administrador de prueba. El valor de password_hash es un hash bcrypt
-- de una contraseña FICTICIA de demostración; no se guarda texto plano.
INSERT INTO administradores (nombre, correo, password_hash, activo) VALUES
('Administrador de Prueba', 'admin@aeroandes.example', '$2b$12$OmxX7Bkea1rp.NroAISfeO4Ju5u7pFMCNxiUhRiCr0vwkPDXq3oMG', 1);

INSERT INTO destinos (ciudad, pais, codigo_aeropuerto, descripcion, imagen_url, activo) VALUES
('Lima',      'Perú',  'LIM', 'Capital del Perú y principal centro de conexiones de la aerolínea.', '/img/destinos/lima.jpg', 1),
('Cusco',     'Perú',  'CUZ', 'Puerta de entrada a Machu Picchu y al Valle Sagrado.',                '/img/destinos/cusco.jpg', 1),
('Arequipa',  'Perú',  'AQP', 'La Ciudad Blanca, rodeada de volcanes.',                              '/img/destinos/arequipa.jpg', 1),
('Piura',     'Perú',  'PIU', 'Ciudad del norte, cercana a playas de aguas cálidas.',                '/img/destinos/piura.jpg', 1),
('Iquitos',   'Perú',  'IQT', 'Ciudad amazónica accesible principalmente por vía aérea.',            '/img/destinos/iquitos.jpg', 1),
('Bogotá',    'Colombia', 'BOG', 'Destino internacional hacia el centro de Colombia.',               '/img/destinos/bogota.jpg', 1);

INSERT INTO vuelos (codigo_vuelo, origen_id, destino_id, fecha_salida, fecha_llegada, precio_desde, asientos_disponibles, estado, activo) VALUES
('AA101', 1, 2, '2026-11-10 06:30:00', '2026-11-10 07:55:00', 189.90, 120, 'programado', 1),
('AA102', 2, 1, '2026-11-10 09:00:00', '2026-11-10 10:25:00', 189.90, 118, 'programado', 1),
('AA201', 1, 3, '2026-11-11 08:15:00', '2026-11-11 09:55:00', 169.50, 150, 'programado', 1),
('AA202', 3, 1, '2026-11-11 11:00:00', '2026-11-11 12:40:00', 169.50, 145, 'programado', 1),
('AA301', 1, 4, '2026-11-12 14:20:00', '2026-11-12 16:05:00', 159.00, 130,  'programado', 1),
('AA401', 1, 5, '2026-11-13 05:45:00', '2026-11-13 07:50:00', 249.00, 100, 'programado', 1),
('AA501', 1, 6, '2026-11-15 22:10:00', '2026-11-16 01:20:00', 489.00, 160, 'programado', 1),
('AA109', 1, 2, '2026-11-09 06:30:00', '2026-11-09 07:55:00', 189.90, 0,   'finalizado', 0);

INSERT INTO reservas (codigo_reserva, vuelo_id, nombre_pasajero, correo, telefono, cantidad_pasajeros, estado) VALUES
('RSV-0001', 1, 'Pasajero de Prueba Uno',  'pasajero1@example.com', '999000001', 2, 'pendiente'),
('RSV-0002', 3, 'Pasajero de Prueba Dos',  'pasajero2@example.com', '999000002', 1, 'confirmada'),
('RSV-0003', 7, 'Pasajero de Prueba Tres', 'pasajero3@example.com', NULL,        3, 'cancelada');

INSERT INTO contactos (nombre, correo, asunto, mensaje, estado) VALUES
('Contacto de Prueba Uno', 'contacto1@example.com', 'Consulta sobre equipaje',    'Mensaje ficticio para probar el formulario de contacto.', 'pendiente'),
('Contacto de Prueba Dos', 'contacto2@example.com', 'Cambio de fecha de vuelo',   'Mensaje ficticio para probar el panel de administración.', 'atendido');

-- =====================================================================
-- (OPCIONAL) Usuario de aplicación con privilegios mínimos
-- La aplicación C# NO debe conectarse con la cuenta root.
-- Descomente, cambie la contraseña y NO la suba a repositorios públicos.
-- =====================================================================
-- CREATE USER IF NOT EXISTS 'app_aeroandes'@'localhost' IDENTIFIED BY 'CAMBIAR_ESTA_CLAVE';
-- GRANT SELECT, INSERT, UPDATE ON aeroandes_db.* TO 'app_aeroandes'@'localhost';
-- (sin DROP, ALTER ni GRANT OPTION)

-- =====================================================================
-- CONSULTAS DE VERIFICACIÓN (para tomar las capturas)
-- =====================================================================
SHOW TABLES;

SELECT 'administradores' AS tabla, COUNT(*) AS registros FROM administradores
UNION ALL SELECT 'destinos',  COUNT(*) FROM destinos
UNION ALL SELECT 'vuelos',    COUNT(*) FROM vuelos
UNION ALL SELECT 'reservas',  COUNT(*) FROM reservas
UNION ALL SELECT 'contactos', COUNT(*) FROM contactos;

SELECT v.codigo_vuelo, o.ciudad AS origen, d.ciudad AS destino,
       v.fecha_salida, v.precio_desde, v.asientos_disponibles, v.estado
FROM vuelos v
JOIN destinos o ON o.id = v.origen_id
JOIN destinos d ON d.id = v.destino_id
ORDER BY v.fecha_salida;

SELECT id, nombre, correo, LEFT(password_hash, 7) AS inicio_hash, activo
FROM administradores;
