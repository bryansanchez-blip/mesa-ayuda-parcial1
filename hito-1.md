# Hito 1: Primer Sistema Funcional de Mesa de Ayuda

## 1. Cambios y Ajustes respecto al Hito 0
- **Implementación del Modelo:** Se creó la clase `Ticket` encapsulando la lógica del cambio de estado mediante el método `CerrarTicket()`.
- **Integración del Flujo Completo:** Se desarrollaron las cuatro operaciones principales (Registro, Listado, Consulta por ID y Cambio de Estado) en una aplicación de consola en C#.
- **Validaciones de Entrada:** Se implementó la verificación obligatoria para evitar registros con títulos/descripciones vacíos o prioridades fuera del catálogo (Baja, Media, Alta).
- **Control de Estado:** Se agregó la regla de negocio en el modelo para impedir el doble cierre de tickets.
- **Filtros y Métricas:** Se implementó el filtrado dinámico (Todos/Abiertos/Cerrados) y contadores en el encabezado principal del menú.

## 2. Decisiones de Diseño
- **Estructura en memoria:** Manejo de lista `List<Ticket>` para conservar el estado durante la sesión de uso.
- **Trazabilidad:** División modular de responsabilidades mediante métodos estáticos por cada opción funcional.

## 3. Guía de Prueba / Demostración en Clase
1. **Registro:** Intentar registrar un ticket vacío (demostrar validación) y luego registrar 2 tickets válidos.
2. **Listado:** Ver la lista de tickets completa y probar los filtros.
3. **Consulta:** Buscar el ticket #1 por ID para mostrar el detalle.
4. **Cambio de estado:** Cerrar el ticket #1 y re-intentar cerrarlo para evidenciar la regla de negocio.
