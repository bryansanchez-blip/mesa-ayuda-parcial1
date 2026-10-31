using System;
using System.Collections.Generic;
using System.Linq;

namespace MesaDeAyuda
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Prioridad { get; set; }
        public string Estado { get; set; } = "Abierto";

        public bool CerrarTicket()
        {
            if (Estado == "Cerrado")
            {
                return false;
            }
            Estado = "Cerrado";
            return true;
        }
    }

    class Program
    {
        static List<Ticket> tickets = new List<Ticket>();
        static int contadorId = 1;

        static void Main(string[] args)
        {
            int opcion = 0;
            do
            {
                Console.Clear();
                Console.WriteLine("==================================================");
                Console.WriteLine("    SISTEMA DE MESA DE AYUDA - HITO 1 (C#)");
                Console.WriteLine("==================================================");
                Console.WriteLine($"Total Tickets: {tickets.Count} | Abiertos: {tickets.Count(t => t.Estado == "Abierto")} | Cerrados: {tickets.Count(t => t.Estado == "Cerrado")}");
                Console.WriteLine("--------------------------------------------------\n");

                Console.WriteLine("1. Registrar nuevo ticket");
                Console.WriteLine("2. Listar tickets (con filtro de estado)");
                Console.WriteLine("3. Consultar detalle de un ticket");
                Console.WriteLine("4. Cambiar estado de un ticket (Cerrar)");
                Console.WriteLine("5. Salir");
                Console.Write("\nSeleccione una opción (1-5): ");

                if (int.TryParse(Console.ReadLine(), out opcion))
                {
                    switch (opcion)
                    {
                        case 1:
                            RegistrarTicket();
                            break;
                        case 2:
                            ListarTickets();
                            break;
                        case 3:
                            ConsultarTicket();
                            break;
                        case 4:
                            CambiarEstadoTicket();
                            break;
                        case 5:
                            Console.WriteLine("\nSaliendo del sistema...");
                            break;
                        default:
                            Console.WriteLine("\n[Error] Opción no válida.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("\n[Error] Ingrese un número válido.");
                }

                if (opcion != 5)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            } while (opcion != 5);
        }

        static void RegistrarTicket()
        {
            Console.Clear();
            Console.WriteLine("=== REGISTRO DE NUEVO TICKET ===");

            Console.Write("Título: ");
            string titulo = Console.ReadLine();

            Console.Write("Descripción: ");
            string descripcion = Console.ReadLine();

            Console.Write("Prioridad (Baja / Media / Alta): ");
            string prioridad = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(descripcion))
            {
                Console.WriteLine("\n[ERROR DE VALIDACIÓN] El título y la descripción no pueden estar vacíos.");
                return;
            }

            if (string.IsNullOrWhiteSpace(prioridad) ||
               (!prioridad.Equals("Baja", StringComparison.OrdinalIgnoreCase) &&
                !prioridad.Equals("Media", StringComparison.OrdinalIgnoreCase) &&
                !prioridad.Equals("Alta", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("\n[ERROR DE VALIDACIÓN] Prioridad no permitida. Debe ser: Baja, Media o Alta.");
                return;
            }

            string prioridadNormalizada = char.ToUpper(prioridad[0]) + prioridad.Substring(1).ToLower();

            Ticket nuevoTicket = new Ticket
            {
                Id = contadorId++,
                Titulo = titulo.Trim(),
                Descripcion = descripcion.Trim(),
                Prioridad = prioridadNormalizada,
                Estado = "Abierto"
            };

            tickets.Add(nuevoTicket);
            Console.WriteLine($"\n[ÉXITO] Ticket #{nuevoTicket.Id} registrado exitosamente con estado Abierto.");
        }

        static void ListarTickets()
        {
            Console.Clear();
            Console.WriteLine("=== LISTADO DE TICKETS ===");
            Console.WriteLine("Seleccione una vista:");
            Console.WriteLine("1. Mostrar todos los tickets");
            Console.WriteLine("2. Solamente tickets ABIERTOS");
            Console.WriteLine("3. Solamente tickets CERRADOS");
            Console.Write("\nOpción: ");
            string filtro = Console.ReadLine();

            IEnumerable<Ticket> listaFiltrada = tickets;
            string tituloFiltro = "TODOS LOS TICKETS";

            if (filtro == "2")
            {
                listaFiltrada = tickets.Where(t => t.Estado == "Abierto");
                tituloFiltro = "TICKETS ABIERTOS";
            }
            else if (filtro == "3")
            {
                listaFiltrada = tickets.Where(t => t.Estado == "Cerrado");
                tituloFiltro = "TICKETS CERRADOS";
            }

            Console.WriteLine($"\n--- {tituloFiltro} ---");
            if (!listaFiltrada.Any())
            {
                Console.WriteLine("No hay tickets para mostrar en esta lista.");
                return;
            }

            Console.WriteLine("{0,-5} | {1,-30} | {2,-10} | {3,-10}", "ID", "TÍTULO", "PRIORIDAD", "ESTADO");
            Console.WriteLine(new string('-', 65));

            foreach (var t in listaFiltrada)
            {
                string tituloCorto = t.Titulo.Length > 28 ? t.Titulo.Substring(0, 25) + "..." : t.Titulo;
                Console.WriteLine("{0,-5} | {1,-30} | {2,-10} | {3,-10}", t.Id, tituloCorto, t.Prioridad, t.Estado);
            }
        }

        static void ConsultarTicket()
        {
            Console.Clear();
            Console.WriteLine("=== CONSULTA DE DETALLE DE TICKET ===");
            Console.Write("Ingrese el ID del ticket a consultar: ");

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var ticket = tickets.FirstOrDefault(t => t.Id == id);
                if (ticket != null)
                {
                    Console.WriteLine("\n==================================================");
                    Console.WriteLine($" DETALLE DEL TICKET #{ticket.Id}");
                    Console.WriteLine("==================================================");
                    Console.WriteLine($"ID         : {ticket.Id}");
                    Console.WriteLine($"Título     : {ticket.Titulo}");
                    Console.WriteLine($"Descripción: {ticket.Descripcion}");
                    Console.WriteLine($"Prioridad  : {ticket.Prioridad}");
                    Console.WriteLine($"Estado     : {ticket.Estado}");
                    Console.WriteLine("==================================================");
                }
                else
                {
                    Console.WriteLine("\n[ERROR] No se encontró ningún ticket con el ID ingresado.");
                }
            }
            else
            {
                Console.WriteLine("\n[ERROR] Ingrese un ID numérico válido.");
            }
        }

        static void CambiarEstadoTicket()
        {
            Console.Clear();
            Console.WriteLine("=== CAMBIO DE ESTADO DE TICKET ===");
            Console.Write("Ingrese el ID del ticket que desea cerrar: ");

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var ticket = tickets.FirstOrDefault(t => t.Id == id);
                if (ticket != null)
                {
                    bool cambiado = ticket.CerrarTicket();
                    if (cambiado)
                    {
                        Console.WriteLine($"\n[ÉXITO] El estado del ticket #{ticket.Id} ha sido cambiado a 'Cerrado'.");
                    }
                    else
                    {
                        Console.WriteLine($"\n[AVISO] El ticket #{ticket.Id} ya se encuentra 'Cerrado' y no puede volver a cerrarse.");
                    }
                }
                else
                {
                    Console.WriteLine("\n[ERROR] No se encontró ningún ticket con el ID ingresado.");
                }
            }
            else
            {
                Console.WriteLine("\n[ERROR] Ingrese un ID numérico válido.");
            }
        }
    }
}