-- Script de base de datos: Mesa de ayuda (Hito 2)
-- Ejecutar con:  mysql -u root -p < Database/mesa_ayuda.sql
-- Crea la base, las tablas y los datos de demostración.
-- Las tablas usan IF NOT EXISTS; los tickets de ejemplo se insertan solo si la tabla está vacía.

CREATE DATABASE IF NOT EXISTS mesa_ayuda
    CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE mesa_ayuda;

CREATE TABLE IF NOT EXISTS categorias (
    id     INT          NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(50)  NOT NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_categorias_nombre (nombre)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tickets (
    id             INT          NOT NULL AUTO_INCREMENT,
    titulo         VARCHAR(100) NOT NULL,
    descripcion    TEXT         NOT NULL,
    categoria_id   INT          NOT NULL,
    prioridad      VARCHAR(10)  NOT NULL,
    estado         VARCHAR(10)  NOT NULL DEFAULT 'Abierto',
    fecha_creacion DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    CONSTRAINT fk_tickets_categoria FOREIGN KEY (categoria_id) REFERENCES categorias (id),
    CONSTRAINT ck_tickets_prioridad CHECK (prioridad IN ('Baja', 'Media', 'Alta')),
    CONSTRAINT ck_tickets_estado    CHECK (estado IN ('Abierto', 'Cerrado'))
) ENGINE=InnoDB;

-- Datos de demostración
INSERT IGNORE INTO categorias (nombre) VALUES
    ('Hardware'), ('Software'), ('Red'), ('Accesos');

INSERT INTO tickets (titulo, descripcion, categoria_id, prioridad, estado)
SELECT t.titulo, t.descripcion, c.id, t.prioridad, t.estado
FROM (
    SELECT 'Impresora no imprime'   AS titulo, 'La impresora del segundo piso no responde.' AS descripcion, 'Hardware' AS categoria, 'Media' AS prioridad, 'Abierto' AS estado
    UNION ALL
    SELECT 'No abre el correo',     'Outlook se cierra al iniciar.',                         'Software', 'Alta',  'Abierto'
    UNION ALL
    SELECT 'Wi-Fi intermitente',    'La red de la sala de reuniones se cae cada hora.',      'Red',      'Alta',  'Cerrado'
    UNION ALL
    SELECT 'Restablecer contraseña','El usuario olvidó su contraseña del sistema.',          'Accesos',  'Baja',  'Cerrado'
) AS t
INNER JOIN categorias c ON c.nombre = t.categoria
WHERE NOT EXISTS (SELECT 1 FROM tickets);
