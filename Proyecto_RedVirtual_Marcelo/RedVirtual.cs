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
            Console.WriteLine("=== CONFIGURACIÓN DE LA RED ===");

            int cantidad_subredes = 0;
            bool key = false;

            do
            {
                try
                {
                    Console.Write("\nIngrese cantidad de subredes (mínimo 1): ");
                    cantidad_subredes = int.Parse(Console.ReadLine());

                    if (cantidad_subredes < 1)
                    {
                        Interfaz.Error("La cantidad de subredes debe ser al menos 1\n");
                        key = true;
                    }
                    else key = false;
                }
                catch (FormatException)
                {
                    key = true;
                    Interfaz.Error("Entrada no válida. Debe ingresar un número entero\n");
                }
            } while (key);

            SubRedes.Clear();  // Borra la red anterior

            for (int i = 1; i <= cantidad_subredes; i++)
            {
                Console.WriteLine($"\nConfiguración de la SubRed #{i}:");

                string numero_red;
                bool red_valida;

                do
                {
                    Console.Write("Ingrese número de red (X) para la subred (ej: 180): ");
                    numero_red = Console.ReadLine().Trim();

                    red_valida = ValidarNumeroRed(numero_red);

                    if (red_valida && SubRedes.Any(s => s.Router.IP.StartsWith(numero_red + ".")))
                    {
                        Interfaz.Error("Esta red ya existe\n");
                        red_valida = false;
                    }

                } while (!red_valida);

                string ip_router = $"{numero_red}.0";
                string ip_PC = $"{numero_red}.01";

                SubRedes.Add(new SubRed(i, ip_router, ip_PC));
                Console.WriteLine($"SubRed {i} configurada:");
                Console.WriteLine($"- Router: {ip_router}");
                Console.WriteLine($"- PC: {ip_PC}");
            }

            Console.WriteLine("\n¡Red configurada exitosamente!");
            Interfaz.Continuar();
        }

        private bool ValidarNumeroRed(string numero_red)
        {
            if (string.IsNullOrWhiteSpace(numero_red))
            {
                Interfaz.Error("El número de red no puede estar vacío\n");
                return false;
            }

            if (!int.TryParse(numero_red, out int red) || red < 0 || red > 255)
            {
                Interfaz.Error("El número de red debe ser entre 0 y 255\n");
                return false;
            }

            return true;
        }

        public void CrearMensaje()  // Opcion 2
        {
            Console.Clear();
            Console.WriteLine("=== CREAR MENSAJE ===");

            if (SubRedes.Count < 2)
            {
                Interfaz.Error("Se necesitan mínimo 2 subredes para crear mensajes\n");
                Interfaz.Continuar();
                return;
            }

            try
            {
                Console.WriteLine("\nPCs disponibles en la red:");
                foreach (var subred in SubRedes)
                {
                    Console.WriteLine($"- PC: {subred.PC.IP}");
                }

                Console.Write("\nIngrese IP origen (formato X.01): ");
                string ip_origen = Console.ReadLine().Trim();

                var pc_origen = SubRedes.Select(s => s.PC).FirstOrDefault(pc => pc.IP == ip_origen);
                if (pc_origen == null)
                {
                    Interfaz.Error("No existe un PC con esa IP origen");
                    Interfaz.Continuar();
                    return;
                }

                Console.Write("Ingrese IP destino (formato X.01): ");
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

                Console.Write("Ingrese el mensaje a enviar: ");
                string contenido = Console.ReadLine().Trim();

                if (string.IsNullOrEmpty(contenido))
                {
                    Interfaz.Error("El mensaje no puede estar vacío");
                    Interfaz.Continuar();
                    return;
                }

                pc_origen.CrearMensaje(ip_destino, contenido);
                Console.WriteLine("\nMensaje creado y paquetes encolados correctamente!");
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
            Console.WriteLine("=== ENVIAR PAQUETES ===");

            if (SubRedes.Count < 2)
            {
                Interfaz.Error("Se necesitan al menos 2 subredes configuradas");
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
                Console.WriteLine("=== ESTADO DE LA RED ===");
                Console.WriteLine("\nNo hay subredes configuradas.");
                Interfaz.Continuar();
                return;
            }

            int indice_subred = 0;
            while (indice_subred < SubRedes.Count)
            {
                Console.Clear();
                Console.WriteLine("=== ESTADO DE LA RED ===");
                Console.WriteLine($"\nMostrando SubRed {indice_subred + 1} de {SubRedes.Count}\n");

                var subred = SubRedes[indice_subred];

                Console.WriteLine($"═══════════════════════════════");
                Console.WriteLine($"       [ SUBRED {subred.Numero} ]");
                Console.WriteLine($"═══════════════════════════════");

                Console.WriteLine($"\nROUTER {subred.Router.IP}");
                Console.WriteLine($"Paquetes en cola ({subred.Router.ColaPaquetes.Tamano()}/4):");
                MostrarColaPaquetes(subred.Router.ColaPaquetes);

                Console.WriteLine($"\nPC {subred.PC.IP}");
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
            Console.WriteLine("=== ESTADO DE SUBRED ===");

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
            Console.WriteLine("=== ESTADO DE EQUIPO ===");

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
            Console.WriteLine("=== ELIMINAR PAQUETE (SIMULAR PÉRDIDA EN TRANSMISIÓN) ===");

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

            Console.WriteLine($"\nPaquetes en cola de envío de {equipo.Tipo} {equipo.IP}:");
            if (equipo.ColaPaquetes.ColaVacia())
            {
                Interfaz.Error("El equipo no tiene paquetes en cola de envío.");
                Interfaz.Continuar();
                return;
            }

            MostrarColaPaquetes(equipo.ColaPaquetes);

            Console.Write("\nIngrese el número de secuencia del paquete a eliminar: ");
            if (!int.TryParse(Console.ReadLine(), out int numero_secuencia) || numero_secuencia <= 0)
            {
                Interfaz.Error("Número de secuencia inválido.");
                Interfaz.Continuar();
                return;
            }

            bool eliminado = false;
            int capacidad_cola = (equipo is PC) ? 10 : 4;
            var nueva_cola = new Cola<Paquete>(capacidad_cola);

            foreach (var paquete in equipo.ColaPaquetes.ObtenerElementos())
            {
                if (paquete.NumeroSecuencia != numero_secuencia)
                {
                    nueva_cola.Insertar(paquete);
                }
                else
                {
                    eliminado = true;
                    Console.WriteLine($"\nPaquete {numero_secuencia} eliminado de {equipo.IP} (simulando pérdida en transmisión)");
                }
            }

            if (!eliminado)
            {
                Interfaz.Error($"No se encontró el paquete {numero_secuencia} en la cola de envío de {equipo.IP}");
                Interfaz.Continuar();
                return;
            }

            equipo.ColaPaquetes = nueva_cola;

            Console.WriteLine("\nEstado actualizado de la cola de envío:");
            MostrarColaPaquetes(equipo.ColaPaquetes);

            Interfaz.Continuar();
        }

        #endregion
    }
}
