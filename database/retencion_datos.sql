-- =====================================================================
-- AeroAndes - Semana 10: conservación y eliminación segura de datos
-- Lo ejecuta el administrador de la base (root) de forma periódica.
-- La aplicación NO tiene permiso DELETE, así que no puede borrar datos.
-- Los plazos son una PROPUESTA del proyecto: justifíquelos en el informe.
-- ---------------------------------------------------------------------
-- 1) Vista previa: qué se eliminaría o anonimizaría (ejecutar primero)
-- =====================================================================
USE aeroandes_db;

SELECT id, correo, estado, fecha_creacion
FROM contactos
WHERE estado IN ('atendido','cerrado')
  AND fecha_creacion < (NOW() - INTERVAL 12 MONTH);

SELECT r.id, r.codigo_reserva, v.fecha_salida
FROM reservas r
JOIN vuelos v ON v.id = r.vuelo_id
WHERE v.fecha_salida < (NOW() - INTERVAL 24 MONTH)
  AND r.nombre_pasajero <> 'ANONIMIZADO';

-- =====================================================================
-- 2) Aplicar (descomente después de revisar la vista previa)
-- =====================================================================
-- Mensajes de contacto ya atendidos y con más de 12 meses: se eliminan.
-- DELETE FROM contactos
-- WHERE estado IN ('atendido','cerrado')
--   AND fecha_creacion < (NOW() - INTERVAL 12 MONTH);

-- Reservas de vuelos con más de 24 meses: se conserva el registro del viaje
-- pero se quitan los datos personales (anonimización).
-- UPDATE reservas r
-- JOIN vuelos v ON v.id = r.vuelo_id
-- SET r.nombre_pasajero = 'ANONIMIZADO',
--     r.correo = CONCAT('anonimo-', r.id, '@eliminado.invalid'),
--     r.telefono = NULL
-- WHERE v.fecha_salida < (NOW() - INTERVAL 24 MONTH)
--   AND r.nombre_pasajero <> 'ANONIMIZADO';
