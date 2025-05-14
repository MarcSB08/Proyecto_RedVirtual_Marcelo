using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_RedVirtual_Marcelo
{
    internal class RedVirtual
    {
        #region Atributos

        public List<SubRed> SubRedes { get; private set; }

        #endregion

        #region Metodos

        public RedVirtual()
        {
            SubRedes = new List<SubRed>();
        }

        public void ConfigurarRed()  //Opcion 1
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("=== CONFIGURACIÓN DE LA RED ===");
            Console.ResetColor();

            int cantidad_subredes = 0;
            string opcion = "";
            bool key = false;

            if (SubRedes.Count != 0)
            {
                do
                {
                    Console.WriteLine("\nYa hay una red existente, desea eliminarla y crear una nueva?");
                    Console.Write("-Opción (Si/No): ");

                    opcion = Console.ReadLine().Trim().ToLower();
                    if (opcion == "si")
                    {
                        SubRedes.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nRed eliminada exitosamente!");
                        Console.ResetColor();
                    }
                    else if (opcion == "no")
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nRed no eliminada, volviendo al menú principal...");
                        Console.ResetColor();
                        Interfaz.Continuar();
                        return;
                    }
                    else
                    {
                        Interfaz.Error("Opción no válida");
                    }
                } while (opcion != "si" && opcion != "no");
            }

            do
            {
                try
                {
                    Console.Write("\nIngrese cantidad de subredes (mínimo 1): ");
                    cantidad_subredes = int.Parse(Console.ReadLine());

                    if (cantidad_subredes < 1)
                    {
                        Interfaz.Error("La cantidad de subredes debe ser al menos 1.\n");
                        key = true;
                    }
                    else key = false;
                }
                catch (FormatException)
                {
                    key = true;
                    Interfaz.Error("Entrada no válida. Debe ingresar un número entero.\n");
                }
            } while (key);

            for (int i = 1; i <= cantidad_subredes; i++)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"\nConfiguración de la SubRed #{i}:");
                Console.ResetColor();

                string numero_red;
                bool red_valida;

                do
                {
                    Console.Write("Ingrese número de red para la subred (ej: 180): ");
                    numero_red = Console.ReadLine().Trim();

                    red_valida = ValidarNumeroRed(numero_red);

                    if (red_valida && SubRedes.Any(s => s.Router.IP.StartsWith(numero_red + ".")))
                    {
                        Interfaz.Error("Esta red ya existe\n\n");
                        red_valida = false;
                    }

                } while (!red_valida);

                string ip_router = $"{numero_red}.0";
                string ip_PC = $"{numero_red}.01";

                SubRedes.Add(new SubRed(i, ip_router, ip_PC));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nSubRed {i} configurada:");
                Console.ResetColor();
                Console.WriteLine($"- Router: {ip_router}");
                Console.WriteLine($"- PC: {ip_PC}");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n¡Red configurada exitosamente!");
            Console.ResetColor();
            Interfaz.Continuar();
        }

        private bool ValidarNumeroRed(string numero_red)
        {
            if (string.IsNullOrWhiteSpace(numero_red))
            {
                Interfaz.Error("El número de red no puede estar vacío\n\n");
                return false;
            }

            if (!int.TryParse(numero_red, out int red) || red < 0 || red > 255)
            {
                Interfaz.Error("El número de red debe ser entre 0 y 255\n\n");
                return false;
            }

            return true;
        }

        public void CrearMensaje()  // Opcion 2
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("=== CREAR MENSAJE ===");
            Console.ResetColor();

            if (SubRedes.Count < 2)
            {
                Console.WriteLine();
                Interfaz.Error("Se necesitan mínimo 2 subredes para crear mensajes\n");
                Interfaz.Continuar();
                return;
            }

            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nPCs disponibles en la red:");
                Console.ResetColor();

                foreach (var subred in SubRedes)
                {
                    Console.WriteLine($"- PC: {subred.PC.IP}");
                }

                Console.Write("\nIngrese IP de origen (formato X.01): ");
                string ip_origen = Console.ReadLine().Trim();

                var pc_origen = SubRedes.Select(s => s.PC).FirstOrDefault(pc => pc.IP == ip_origen);
                if (pc_origen == null)
                {
                    Interfaz.Error("No existe un PC con esa IP origen");
                    Interfaz.Continuar();
                    return;
                }

                Console.Write("\nIngrese IP de destino (formato X.01): ");
                string ip_destino = Console.ReadLine().Trim();

                if (ip_destino == ip_origen)
                {
                    Interfaz.Error("No puede enviar mensajes al mismo PC origen");
                    Interfaz.Continuar();
                    return;
                }

                var pc_destino = SubRedes.Select(s => s.PC).FirstOrDefault(pc => pc.IP == ip_destino);
                if (pc_destino == null)
                {
                    Interfaz.Error("No existe un PC con esa IP destino");
                    Interfaz.Continuar();
                    return;
                }

                Console.Write("\nIngrese el mensaje a enviar (máx 9 caracteres): ");
                string contenido = Console.ReadLine().Trim();

                if (string.IsNullOrEmpty(contenido))
                {
                    Interfaz.Error("El mensaje no puede estar vacío");
                    Interfaz.Continuar();
                    return;
                }

                if (contenido.Length > 9)
                {
                    Interfaz.Error("El mensaje no puede tener más de 9 caracteres");
                    Interfaz.Continuar();
                    return;
                }

                pc_origen.CrearMensaje(ip_destino, contenido);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nMensaje creado y paquetes encolados correctamente!");
                Console.ResetColor();
                Console.WriteLine($"Total de paquetes generados: {pc_origen.ColaPaquetes.Tamano()}");
            }
            catch (Exception ex)
            {
                Interfaz.Error($"{ex.Message}");
            }
            Interfaz.Continuar();
        }

        public void EnviarPaquetes()  //Opcion 3
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("=== ENVIAR PAQUETES ===");
            Console.ResetColor();


            if (SubRedes.Count < 2)
            {
                Console.WriteLine();
                Interfaz.Error("Se necesitan al menos 2 subredes configuradas\n");
                Interfaz.Continuar();
                return;
            }

            try
            {
                Console.WriteLine("\nDispositivos en la red:");
                foreach (var subred in SubRedes)
                {
                    Console.WriteLine($"- PC: {subred.PC.IP}");
                    Console.WriteLine($"- Router: {subred.Router.IP}");
                }

                Console.Write("\nIngrese IP del dispositivo que enviará (ej: 190.01 o 190.0): ");
                string ip_origen = Console.ReadLine().Trim();

                Dispositivo dispositivo_origen = null;
                SubRed subred_origen = null;

                foreach (var subred in SubRedes)
                {
                    if (subred.PC.IP == ip_origen)
                    {
                        dispositivo_origen = subred.PC;
                        subred_origen = subred;
                        break;
                    }
                    if (subred.Router.IP == ip_origen)
                    {
                        dispositivo_origen = subred.Router;
                        subred_origen = subred;
                        break;
                    }
                }

                if (dispositivo_origen == null)
                {
                    Interfaz.Error("No se encontró el dispositivo");
                    Interfaz.Continuar();
                    return;
                }

                if (dispositivo_origen.ColaPaquetes.ColaVacia())
                {
                    Interfaz.Error("El dispositivo no tiene paquetes para enviar");
                    Interfaz.Continuar();
                    return;
                }

                Paquete paquete = dispositivo_origen.ColaPaquetes.FrenteCola();
                Dispositivo siguiente_dispositivo = null;
                string accion = "";

                if (dispositivo_origen is PC)
                {
                    siguiente_dispositivo = subred_origen.Router;
                    accion = $"enviado desde PC {ip_origen} a Router local {siguiente_dispositivo.IP}";
                }
                else if (dispositivo_origen is Router router_origen)
                {
                    var ip_destino = paquete.IPDestino;
                    var subred_destino = SubRedes.FirstOrDefault(s => s.PC.IP == ip_destino);

                    if (subred_destino == null)
                    {
                        Interfaz.Error($"No existe destino {ip_destino}");
                        paquete.Estado = "Dañado";
                        var paquete_dañado = dispositivo_origen.ColaPaquetes.Quitar();
                        dispositivo_origen.ColaPaquetes.Insertar(paquete_dañado);
                        Interfaz.Continuar();
                        return;
                    }

                    if (router_origen.Red == subred_destino.Router.Red)
                    {
                        siguiente_dispositivo = subred_destino.PC;
                        accion = $"enviado desde Router {ip_origen} a PC destino {siguiente_dispositivo.IP}";
                    }
                    else
                    {
                        siguiente_dispositivo = subred_destino.Router;
                        accion = $"enrutado desde Router {ip_origen} a Router {siguiente_dispositivo.IP}";
                    }
                }

                if (siguiente_dispositivo != null)
                {
                    bool envio_exitoso = false;

                    if (siguiente_dispositivo is PC pc_destino)
                    {
                        envio_exitoso = pc_destino.RecibirPaquete(paquete);
                        if (envio_exitoso)
                        {
                            dispositivo_origen.ColaPaquetes.Quitar();
                            pc_destino.ProcesarPaquetesRecibidos();
                            paquete.Estado = "Enviado";
                            Console.WriteLine($"\nPaquete {paquete.NumeroSecuencia} {accion}");
                        }
                    }
                    else
                    {
                        envio_exitoso = siguiente_dispositivo.RecibirPaquete(paquete);
                        if (envio_exitoso)
                        {
                            dispositivo_origen.ColaPaquetes.Quitar();
                            paquete.Estado = "Enviado";
                            Console.WriteLine($"\nPaquete {paquete.NumeroSecuencia} {accion}");
                        }
                    }

                    if (!envio_exitoso)
                    {
                        var paquete_devuelto = dispositivo_origen.ColaPaquetes.Quitar();
                        paquete_devuelto.Estado = "Devuelto";
                        dispositivo_origen.ColaPaquetes.Insertar(paquete_devuelto);

                        Console.WriteLine($"\n¡Cola llena en {siguiente_dispositivo.IP}! " +
                                        $"Paquete {paquete_devuelto.NumeroSecuencia} devuelto a {ip_origen}");
                    }
                }
            }
            catch (Exception ex)
            {
                Interfaz.Error($"Error: {ex.Message}");
            }

            Interfaz.Continuar();
        }

        public void MostrarStatusRed()  // Opcion 4
        {
            if (SubRedes.Count == 0)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("=== ESTADO DE LA RED ===");
                Console.ResetColor();
                Console.WriteLine("\nNo hay subredes configuradas.");
                Interfaz.Continuar();
                return;
            }

            int indice_subred = 0;
            while (indice_subred < SubRedes.Count)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("=== ESTADO DE LA RED ===");
                Console.ResetColor();
                Console.WriteLine($"\nMostrando SubRed {indice_subred + 1} de {SubRedes.Count}\n");

                var subred = SubRedes[indice_subred];

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"═══════════════════════════════");
                Console.WriteLine($"       [ SUBRED {subred.Numero} ]");
                Console.WriteLine($"═══════════════════════════════");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\nROUTER {subred.Router.IP}");
                Console.ResetColor();
                Console.WriteLine($"Paquetes en cola ({subred.Router.ColaPaquetes.Tamano()}/4):");
                MostrarColaPaquetes(subred.Router.ColaPaquetes);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\nPC {subred.PC.IP}");
                Console.ResetColor();
                Console.WriteLine($"Paquetes para enviar ({subred.PC.ColaPaquetes.Tamano()}/10):");
                MostrarColaPaquetes(subred.PC.ColaPaquetes);

                Console.WriteLine($"\nPaquetes recibidos ({subred.PC.ColaRecibidos.Tamano()}/10):");
                if (subred.PC.ColaRecibidos.ColaVacia())
                {
                    Console.WriteLine("   (Vacía)");
                }
                else
                {
                    foreach (var paquete in subred.PC.ColaRecibidos.ObtenerElementos()
                        .OrderBy(p => p.IPOrigen)
                        .ThenBy(p => p.NumeroSecuencia))
                    {
                        string estado = paquete.Estado.PadRight(8);
                        string dato = paquete.Dato == '\0' ? "[FIN]" : $"'{paquete.Dato}'";
                        Console.WriteLine($"   - Paquete {paquete.NumeroSecuencia.ToString().PadLeft(2)} " +
                                        $"[{estado}] {dato} " +
                                        $"{paquete.IPOrigen} → {paquete.IPDestino}");
                    }
                }

                Console.WriteLine($"\nMensajes completos recibidos:");
                if (subred.PC.MensajesRecibidos.Count == 0)
                {
                    Console.WriteLine("   (No hay mensajes completos)");
                }
                else
                {
                    foreach (var mensaje in subred.PC.MensajesRecibidos
                        .OrderBy(m => m.FechaCreacion))
                    {
                        string estado = mensaje.Estado == "Recibido" ? "Recibido" : "Dañado";
                        Console.WriteLine($"   - [{mensaje.FechaCreacion:HH:mm:ss}] " +
                                        $"{mensaje.IPOrigen} -> {mensaje.Dato} " +
                                        $"(Paquetes: {mensaje.Paquetes.Count - 1}) - Estado: {estado}");
                    }
                }

                Console.WriteLine($"\n═══════════════════════════════");

                if (indice_subred < SubRedes.Count - 1)
                {
                    Console.WriteLine("\nPresione cualquier tecla para ver la siguiente subred...");
                    Console.WriteLine("Presione 'Q' para salir");
                }
                else
                {
                    Console.WriteLine("\nFin del reporte. Presione cualquier tecla para volver al menú...");
                }

                var tecla = Console.ReadKey(true);
                if (tecla.Key == ConsoleKey.Q)
                {
                    break;
                }

                indice_subred++;
            }
        }

        public void MostrarStatusSubRed()  // Opcion 5
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("=== ESTADO DE SUBRED ===");
            Console.ResetColor();

            if (SubRedes.Count == 0)
            {
                Interfaz.Error("No hay subredes configuradas.");
                Interfaz.Continuar();
                return;
            }

            Console.WriteLine("\nSubRedes disponibles:");
            foreach (var subred in SubRedes)
            {
                Console.WriteLine($"- SubRed ({subred.Numero}): Router {subred.Router.IP}, PC {subred.PC.IP}");
            }

            Console.Write("\nIngrese el número de la SubRed a mostrar (1, 2, 3, etc): ");
            if (!int.TryParse(Console.ReadLine(), out int numero_subred) || numero_subred <= 0 || numero_subred > SubRedes.Count)
            {
                Interfaz.Error("Número de SubRed inválido.");
                Interfaz.Continuar();
                return;
            }

            var subred_seleccionada = SubRedes[numero_subred - 1];

            Console.Clear();
            Console.WriteLine($"=== ESTADO DE SUBRED {subred_seleccionada.Numero} ===");
            Console.WriteLine($"Router: {subred_seleccionada.Router.IP}");
            Console.WriteLine($"PC: {subred_seleccionada.PC.IP}");

            Console.WriteLine("\n[ROUTER]");
            Console.WriteLine($"Paquetes en cola ({subred_seleccionada.Router.ColaPaquetes.Tamano()}/4):");
            MostrarColaPaquetes(subred_seleccionada.Router.ColaPaquetes);

            Console.WriteLine("\n[PC]");
            Console.WriteLine($"Paquetes para enviar ({subred_seleccionada.PC.ColaPaquetes.Tamano()}/10):");
            MostrarColaPaquetes(subred_seleccionada.PC.ColaPaquetes);

            Console.WriteLine($"\nPaquetes recibidos ({subred_seleccionada.PC.ColaRecibidos.Tamano()}/10):");
            MostrarColaPaquetes(subred_seleccionada.PC.ColaRecibidos);

            Console.WriteLine($"\nMensajes completos recibidos:");
            if (subred_seleccionada.PC.MensajesRecibidos.Count == 0)
            {
                Console.WriteLine("(No hay mensajes completos)");
            }
            else
            {
                foreach (var mensaje in subred_seleccionada.PC.MensajesRecibidos)
                {
                    Console.WriteLine($"- {mensaje.Dato} | Origen: {mensaje.IPOrigen} | Estado: {mensaje.Estado}");
                }
            }

            Interfaz.Continuar();
        }

        private void MostrarColaPaquetes(Cola<Paquete> cola)
        {
            if (cola.ColaVacia())
            {
                Console.WriteLine("   (Vacía)");
                return;
            }

            foreach (var paquete in cola.ObtenerElementos())
            {
                string estado = paquete.Estado.PadRight(10);
                string dato = paquete.Dato == '\0' ? "[FIN]" : $"'{paquete.Dato}'";
                Console.WriteLine($"   - Paquete {paquete.NumeroSecuencia.ToString().PadLeft(2)} " +
                                $"[{estado}] {dato} | " +
                                $"{paquete.IPOrigen} → {paquete.IPDestino}");
            }
        }

        public void MostrarStatusEquipo()  // Opcion 6
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("=== ESTADO DE EQUIPO ===");
            Console.ResetColor();

            if (SubRedes.Count == 0)
            {
                Interfaz.Error("No hay equipos configurados.");
                Interfaz.Continuar();
                return;
            }

            Console.WriteLine("\nEquipos disponibles:");
            foreach (var subred in SubRedes)
            {
                Console.WriteLine($"- PC: {subred.PC.IP}");
                Console.WriteLine($"- Router: {subred.Router.IP}");
            }

            Console.Write("\nIngrese la IP del equipo a mostrar (ej: 192.01 o 192.0): ");
            string ip_equipo = Console.ReadLine().Trim();

            Dispositivo equipo = null;
            foreach (var subred in SubRedes)
            {
                if (subred.PC.IP == ip_equipo)
                {
                    equipo = subred.PC;
                    break;
                }
                if (subred.Router.IP == ip_equipo)
                {
                    equipo = subred.Router;
                    break;
                }
            }

            if (equipo == null)
            {
                Interfaz.Error("No se encontró el equipo con esa IP.");
                Interfaz.Continuar();
                return;
            }

            Console.Clear();
            Console.WriteLine($"=== ESTADO DE EQUIPO {equipo.IP} ===");
            Console.WriteLine($"Tipo: {equipo.Tipo}");
            Console.WriteLine($"SubRed: {SubRedes.First(s => s.Router.IP == ip_equipo || s.PC.IP == ip_equipo).Numero}");

            if (equipo is PC pc)
            {
                Console.WriteLine($"\n[COLA DE ENVÍO] ({pc.ColaPaquetes.Tamano()}/10):");
                MostrarColaPaquetes(pc.ColaPaquetes);

                Console.WriteLine($"\n[COLA DE RECIBIDOS] ({pc.ColaRecibidos.Tamano()}/10):");
                MostrarColaPaquetes(pc.ColaRecibidos);

                Console.WriteLine($"\n[MENSAJES COMPLETOS] ({pc.MensajesRecibidos.Count}):");
                if (pc.MensajesRecibidos.Count == 0)
                {
                    Console.WriteLine("(No hay mensajes completos)");
                }
                else
                {
                    foreach (var mensaje in pc.MensajesRecibidos)
                    {
                        Console.WriteLine($"- {mensaje.Dato} | Origen: {mensaje.IPOrigen} | Estado: {mensaje.Estado}");
                    }
                }
            }
            else if (equipo is Router router)
            {
                Console.WriteLine($"\n[COLA DE PAQUETES] ({router.ColaPaquetes.Tamano()}/4):");
                MostrarColaPaquetes(router.ColaPaquetes);

                Console.WriteLine($"\n[RED ASOCIADA]: {router.Red}");
            }

            Interfaz.Continuar();
        }

        public void EliminarPaquete()  // Opcion 7
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("=== ELIMINAR PAQUETE ===");
            Console.ResetColor();

            if (SubRedes.Count == 0)
            {
                Interfaz.Error("No hay equipos configurados.");
                Interfaz.Continuar();
                return;
            }

            Console.WriteLine("\nEquipos disponibles:");
            foreach (var subred in SubRedes)
            {
                Console.WriteLine($"- PC: {subred.PC.IP}");
                Console.WriteLine($"- Router: {subred.Router.IP}");
            }

            Console.Write("\nIngrese la IP del equipo: ");
            string ip_equipo = Console.ReadLine().Trim();

            Dispositivo equipo = null;
            PC pc = null;
            SubRed subred_seleccionada = null;

            foreach (var subred in SubRedes)
            {
                if (subred.PC.IP == ip_equipo)
                {
                    equipo = subred.PC;
                    pc = subred.PC;
                    subred_seleccionada = subred;
                    break;
                }
                if (subred.Router.IP == ip_equipo)
                {
                    equipo = subred.Router;
                    subred_seleccionada = subred;
                    break;
                }
            }

            if (equipo == null)
            {
                Interfaz.Error("No se encontró el equipo con esa IP.");
                Interfaz.Continuar();
                return;
            }

            Cola<Paquete> cola_seleccionada = null;
            string tipo_cola = "envío";

            if (equipo is PC)
            {
                Console.WriteLine("\nSeleccione qué cola desea modificar:");
                Console.WriteLine("1. Cola de Envío");
                Console.WriteLine("2. Cola de Recibidos");
                Console.Write("Opción: ");

                string opcion_cola = Console.ReadLine().Trim();

                switch (opcion_cola)
                {
                    case "1":
                        cola_seleccionada = pc.ColaPaquetes;
                        tipo_cola = "envío";
                        break;

                    case "2":
                        cola_seleccionada = pc.ColaRecibidos;
                        tipo_cola = "recibidos";
                        break;

                    default:
                        Interfaz.Error("Opción no válida.");
                        Interfaz.Continuar();
                        return;
                }
            }
            else
            {
                cola_seleccionada = equipo.ColaPaquetes;
            }

            Console.WriteLine($"\nPaquetes en cola de {tipo_cola} de {equipo.Tipo} {equipo.IP}:");
            if (cola_seleccionada.ColaVacia())
            {
                Interfaz.Error($"El equipo no tiene paquetes en cola de {tipo_cola}.");
                Interfaz.Continuar();
                return;
            }

            MostrarColaPaquetes(cola_seleccionada);

            Console.Write("\nIngrese el número de secuencia del paquete a eliminar: ");
            if (!int.TryParse(Console.ReadLine(), out int numero_secuencia) || numero_secuencia <= 0)
            {
                Interfaz.Error("Número de secuencia inválido.");
                Interfaz.Continuar();
                return;
            }

            Paquete paquete_a_eliminar = cola_seleccionada.ObtenerElementos().FirstOrDefault(p => p.NumeroSecuencia == numero_secuencia);

            if (paquete_a_eliminar == null)
            {
                Interfaz.Error($"No se encontró el paquete {numero_secuencia} en la cola de {tipo_cola} de {equipo.IP}");
                Interfaz.Continuar();
                return;
            }

            Console.WriteLine("\nInformación del paquete a eliminar:");
            Console.WriteLine("----------------------------------");
            Console.WriteLine($"- Número de secuencia: {paquete_a_eliminar.NumeroSecuencia}");
            Console.WriteLine($"- Dato: {(paquete_a_eliminar.Dato == '\0' ? "[FIN]" : $"'{paquete_a_eliminar.Dato}'")}");
            Console.WriteLine($"- Estado: {paquete_a_eliminar.Estado}");
            Console.WriteLine($"- Origen: {paquete_a_eliminar.IPOrigen}");
            Console.WriteLine($"- Destino: {paquete_a_eliminar.IPDestino}");
            Console.WriteLine("----------------------------------");

            Console.Write("\n¿Está seguro que desea eliminar este paquete? (Si/No): ");
            string confirmacion = Console.ReadLine().Trim().ToLower();

            if (confirmacion == "no")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nOperación cancelada. El paquete no fue eliminado.");
                Console.ResetColor();
                Interfaz.Continuar();
                return;
            }
            else if (confirmacion == "si")
            {
                int capacidad_cola = (equipo is PC) ? 10 : 4;
                var nueva_cola = new Cola<Paquete>(capacidad_cola);
                bool eliminado = false;

                foreach (var paquete in cola_seleccionada.ObtenerElementos())
                {
                    if (paquete.NumeroSecuencia != numero_secuencia)
                    {
                        nueva_cola.Insertar(paquete);
                    }
                    else
                    {
                        eliminado = true;
                    }
                }

                if (equipo is PC && tipo_cola == "recibidos")
                {
                    pc.ColaRecibidos = nueva_cola;
                }
                else
                {
                    equipo.ColaPaquetes = nueva_cola;
                }

                if (eliminado)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nPaquete {numero_secuencia} eliminado de {equipo.IP} (simulando pérdida en transmisión)");
                    Console.ResetColor();

                    Console.WriteLine("\nEstado actualizado de la cola:");
                    MostrarColaPaquetes(nueva_cola);
                }
            }
            else
            {
                Interfaz.Error("Opción no válida. El paquete no fue eliminado.");
            }
            Interfaz.Continuar();
        }

        public void VisualizarMensajesRecibidos()  // Opcion 8
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("=== MENSAJES RECIBIDOS ===");
            Console.ResetColor();

            if (SubRedes.Count == 0)
            {
                Interfaz.Error("No hay equipos configurados.");
                Interfaz.Continuar();
                return;
            }

            Console.WriteLine("\nPCs disponibles:");
            foreach (var subred in SubRedes)
            {
                Console.WriteLine($"- PC: {subred.PC.IP}");
            }

            Console.Write("\nIngrese la IP del PC (formato X.01): ");
            string ip_PC = Console.ReadLine().Trim();

            PC pc = null;
            foreach (var subred in SubRedes)
            {
                if (subred.PC.IP == ip_PC)
                {
                    pc = subred.PC;
                    break;
                }
            }

            if (pc == null)
            {
                Interfaz.Error("No se encontró un PC con esa IP.");
                Interfaz.Continuar();
                return;
            }

            Console.Clear();
            Console.WriteLine($"=== MENSAJES RECIBIDOS EN PC {pc.IP} ===");

            if (pc.MensajesRecibidos.Count == 0)
            {
                Console.WriteLine("\nNo hay mensajes recibidos.");
            }
            else
            {
                Console.WriteLine("\nLista de mensajes recibidos:");
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine("| Fecha Recepción | Origen   | Contenido           | Estado  |");
                Console.WriteLine("------------------------------------------------------------");

                foreach (var mensaje in pc.MensajesRecibidos.OrderBy(m => m.FechaCreacion))
                {
                    string contenido = mensaje.Dato.Length > 20 ? mensaje.Dato.Substring(0, 17) + "..." : mensaje.Dato;
                    string estado = mensaje.Estado == "Recibido" ? "Recibido" : "Dañado  ";

                    Console.WriteLine($"| {mensaje.FechaCreacion:yyyy-MM-dd HH:mm} | {mensaje.IPOrigen.PadRight(8)} | {contenido.PadRight(20)} | {estado} |");
                }

                Console.WriteLine("------------------------------------------------------------");

                int recibidos = pc.MensajesRecibidos.Count(m => m.Estado == "Recibido");
                int danados = pc.MensajesRecibidos.Count(m => m.Estado == "Dañado");

                Console.WriteLine($"\nEstadísticas: {recibidos} recibidos correctamente | {danados} dañados");
            }

            Interfaz.Continuar();
        }

        public void ConsultarPaquete()  // Opcion 9
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("=== CONSULTAR PAQUETE ===");
            Console.ResetColor();

            if (SubRedes.Count == 0)
            {
                Interfaz.Error("No hay equipos configurados.");
                Interfaz.Continuar();
                return;
            }

            Console.Write("\nIngrese el número de secuencia del paquete: ");
            if (!int.TryParse(Console.ReadLine(), out int numero_secuencia) || numero_secuencia <= 0)
            {
                Interfaz.Error("Número de secuencia inválido.");
                Interfaz.Continuar();
                return;
            }

            bool encontrado = false;

            Console.WriteLine("\nBuscando paquete en la red...");

            foreach (var subred in SubRedes)
            {
                var info_router = BuscarPaqueteEnDispositivo(subred.Router, numero_secuencia);
                if (info_router.Encontrado)
                {
                    MostrarInfoPaquete(info_router, subred.Router);
                    encontrado = true;
                }

                var info_PCEnvio = BuscarPaqueteEnDispositivo(subred.PC, numero_secuencia);
                if (info_PCEnvio.Encontrado)
                {
                    MostrarInfoPaquete(info_PCEnvio, subred.PC, "Cola de Envío");
                    encontrado = true;
                }

                var info_PCRecibidos = BuscarPaqueteEnCola(subred.PC.ColaRecibidos, numero_secuencia, subred.PC);
                if (info_PCRecibidos.Encontrado)
                {
                    MostrarInfoPaquete(info_PCRecibidos, subred.PC, "Cola de Recibidos");
                    encontrado = true;
                }
            }

            if (!encontrado)
            {
                Console.WriteLine("\nEl paquete no se encuentra en ningún equipo de la red.");
            }

            Interfaz.Continuar();
        }

        private (bool Encontrado, int Posicion, Paquete Paquete) BuscarPaqueteEnDispositivo(Dispositivo dispositivo, int numero_secuencia)
        {
            return BuscarPaqueteEnCola(dispositivo.ColaPaquetes, numero_secuencia, dispositivo);
        }

        private (bool Encontrado, int Posicion, Paquete Paquete) BuscarPaqueteEnCola(Cola<Paquete> cola, int numero_secuencia, Dispositivo dispositivo)
        {
            int posicion = 1;
            foreach (var paquete in cola.ObtenerElementos())
            {
                if (paquete.NumeroSecuencia == numero_secuencia)
                {
                    return (true, posicion, paquete);
                }
                posicion++;
            }
            return (false, 0, null);
        }

        private void MostrarInfoPaquete((bool Encontrado, int Posicion, Paquete Paquete) info, Dispositivo dispositivo, string tipoCola = "Cola de Paquetes")
        {
            Console.WriteLine("\n══════════════════════════════════════");
            Console.WriteLine("  INFORMACIÓN DEL PAQUETE ENCONTRADO");
            Console.WriteLine("══════════════════════════════════════");
            Console.WriteLine($"- Número de secuencia: {info.Paquete.NumeroSecuencia}");
            Console.WriteLine($"- Dato: {(info.Paquete.Dato == '\0' ? "[FIN]" : $"'{info.Paquete.Dato}'")}");
            Console.WriteLine($"- Estado: {info.Paquete.Estado}");
            Console.WriteLine($"- Origen: {info.Paquete.IPOrigen}");
            Console.WriteLine($"- Destino: {info.Paquete.IPDestino}");
            Console.WriteLine($"\n- Ubicación actual:");
            Console.WriteLine($"  > Equipo: {dispositivo.IP} ({dispositivo.Tipo})");
            Console.WriteLine($"  > Cola: {tipoCola}");
            Console.WriteLine($"  > Posición: {info.Posicion} de {dispositivo.ColaPaquetes.Tamano()}");
            Console.WriteLine("══════════════════════════════════════\n");
        }

        public void VaciarColaDispositivo()  // Opcion 10 (extra)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("=== VACIAR COLA DE DISPOSITIVO ===");
            Console.ResetColor();

            if (SubRedes.Count == 0)
            {
                Interfaz.Error("No hay equipos configurados.");
                Interfaz.Continuar();
                return;
            }

            Console.WriteLine("\nDispositivos disponibles:");
            foreach (var subred in SubRedes)
            {
                Console.WriteLine($"- PC: {subred.PC.IP}");
                Console.WriteLine($"- Router: {subred.Router.IP}");
            }

            Console.Write("\nIngrese la IP del dispositivo: ");
            string ip_dispositivo = Console.ReadLine().Trim();

            Dispositivo dispositivo = null;
            SubRed subred_seleccionada = null;

            foreach (var subred in SubRedes)
            {
                if (subred.PC.IP == ip_dispositivo)
                {
                    dispositivo = subred.PC;
                    subred_seleccionada = subred;
                    break;
                }
                if (subred.Router.IP == ip_dispositivo)
                {
                    dispositivo = subred.Router;
                    subred_seleccionada = subred;
                    break;
                }
            }

            if (dispositivo == null)
            {
                Interfaz.Error("No se encontró el dispositivo con esa IP.");
                Interfaz.Continuar();
                return;
            }

            if (dispositivo is PC pc)
            {
                Console.WriteLine("\nSeleccione qué cola desea vaciar:");
                Console.WriteLine("1. Cola de Envío");
                Console.WriteLine("2. Cola de Recibidos");
                Console.Write("Opción: ");

                string opcion_cola = Console.ReadLine().Trim();
                Cola<Paquete> cola_seleccionada = null;
                string nombre_cola = "";

                switch (opcion_cola)
                {
                    case "1":
                        cola_seleccionada = pc.ColaPaquetes;
                        nombre_cola = "envío";
                        break;

                    case "2":
                        cola_seleccionada = pc.ColaRecibidos;
                        nombre_cola = "recibidos";
                        break;

                    default:
                        Interfaz.Error("Opción no válida.");
                        Interfaz.Continuar();
                        return;
                }

                Console.WriteLine($"\nContenido de la cola de {nombre_cola} ({cola_seleccionada.Tamano()} paquetes):");

                if (cola_seleccionada.ColaVacia())
                {
                    Console.WriteLine("(La cola está vacía)");
                    Interfaz.Continuar();
                    return;
                }
                else
                {
                    MostrarColaPaquetes(cola_seleccionada);
                }

                Console.Write("\n¿Está seguro que desea vaciar esta cola? (Si/No): ");
                string confirmar = Console.ReadLine().Trim().ToLower();

                if (confirmar == "si")
                {
                    cola_seleccionada.BorrarCola();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nCola de {nombre_cola} vaciada correctamente.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nOperación cancelada. La cola no se ha modificado.");
                    Console.ResetColor();
                }
            }
            else if (dispositivo is Router router)
            {
                Console.WriteLine("\nContenido de la cola del router:");

                if (router.ColaPaquetes.ColaVacia())
                {
                    Console.WriteLine("(La cola está vacía)");
                    Interfaz.Continuar();
                    return;
                }
                else
                {
                    MostrarColaPaquetes(router.ColaPaquetes);
                }

                Console.Write("\n¿Está seguro que desea vaciar esta cola? (Si/No): ");
                string confirmar = Console.ReadLine().Trim().ToLower();

                if (confirmar == "si")
                {
                    router.ColaPaquetes.BorrarCola();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nCola del router vaciada correctamente.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nOperación cancelada. La cola no se ha modificado.");
                    Console.ResetColor();
                }
            }

            Interfaz.Continuar();
        }

        public void EliminarSubred()  // Opcion 11 (extra)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("=== ELIMINAR SUBRED ===");
            Console.ResetColor();

            if (SubRedes.Count == 0)
            {
                Interfaz.Error("No hay subredes configuradas.");
                Interfaz.Continuar();
                return;
            }

            Console.WriteLine("\nSubredes disponibles:");
            foreach (var subred in SubRedes)
            {
                Console.WriteLine($"- Subred {subred.Numero}: Router {subred.Router.IP}, PC {subred.PC.IP}");
            }

            Console.Write("\nIngrese el número de la subred a eliminar: ");
            if (!int.TryParse(Console.ReadLine(), out int numero_subred) || numero_subred <= 0 || numero_subred > SubRedes.Count)
            {
                Interfaz.Error("Número de subred inválido.");
                Interfaz.Continuar();
                return;
            }

            var subred_a_eliminar = SubRedes[numero_subred - 1];

            Console.WriteLine("\nInformación de la SubRed a eliminar:");
            Console.WriteLine("----------------------------------");
            Console.WriteLine($"- Número: {subred_a_eliminar.Numero}");
            Console.WriteLine($"- Router: {subred_a_eliminar.Router.IP}");
            Console.WriteLine($"- PC: {subred_a_eliminar.PC.IP}");
            Console.WriteLine($"- Paquetes en router: {subred_a_eliminar.Router.ColaPaquetes.Tamano()}");
            Console.WriteLine($"- Paquetes en PC (envío): {subred_a_eliminar.PC.ColaPaquetes.Tamano()}");
            Console.WriteLine($"- Paquetes en PC (recibidos): {subred_a_eliminar.PC.ColaRecibidos.Tamano()}");
            Console.WriteLine("----------------------------------");

            Console.Write("\n¿Está SEGURO que desea eliminar esta subred y TODOS sus paquetes? (Si/No): ");
            string confirmar = Console.ReadLine().Trim().ToLower();

            if (confirmar == "no")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nOperación cancelada. La SubRed no fue eliminada.");
                Console.ResetColor();
                Interfaz.Continuar();
                return;
            }
            else if (confirmar == "si")
            {

                SubRedes.RemoveAt(numero_subred - 1);

                for (int i = 0; i < SubRedes.Count; i++)
                {
                    SubRedes[i].Numero = i + 1;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nSubred {numero_subred} eliminada correctamente con todos sus dispositivos y paquetes.");
                Console.ResetColor();
            }
            else
            {
                Interfaz.Error("Opción no válida. La subred no fue eliminada.");
            }
            Interfaz.Continuar();
        }

        public void EliminarTodaLaRed()  // Opcion 12 (extra)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("=== ELIMINAR TODA LA RED ===");
            Console.ResetColor();

            if (SubRedes.Count == 0)
            {
                Interfaz.Error("No hay red configurada para eliminar.");
                Interfaz.Continuar();
                return;
            }

            Console.WriteLine("\nResumen de la red actual:");
            Console.WriteLine("-------------------------");
            Console.WriteLine($"- Total de Subredes: {SubRedes.Count}");

            int total_paquetes = 0;
            foreach (var subred in SubRedes)
            {
                total_paquetes += subred.Router.ColaPaquetes.Tamano();
                total_paquetes += subred.PC.ColaPaquetes.Tamano();
                total_paquetes += subred.PC.ColaRecibidos.Tamano();
            }
            Console.WriteLine($"- Total de paquetes en toda la red: {total_paquetes}");
            Console.WriteLine("-------------------------");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\n¿Está ABSOLUTAMENTE SEGURO que desea eliminar TODA LA RED? (Si/No): ");
            Console.ResetColor();
            string confirmar = Console.ReadLine().Trim().ToLower();

            if (confirmar == "no")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nOperación cancelada. La red no fue modificada.");
                Console.ResetColor();
                Interfaz.Continuar();
                return;
            }
            else if (confirmar == "si")
            {
                SubRedes.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n¡Toda la red ha sido eliminada correctamente!");
                Console.ResetColor();
                Interfaz.Continuar();
            }
            else
            {
                Interfaz.Error("Opción no válida. La red no fue eliminada.");
                Interfaz.Continuar();
            }
        }

        #endregion
    }
}
