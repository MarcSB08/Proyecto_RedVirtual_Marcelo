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
                Interfaz.Error("Se necesitan mínimo 2 subredes para crear/enviar mensajes\n");
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

        public void EnviarPaquetes()  // Opcion 3
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
                string ip_dispositivo = Console.ReadLine().Trim();

                Dispositivo dispositivo_origen = null;
                SubRed subred_origen = null;

                foreach (var subred in SubRedes)
                {
                    if (subred.PC.IP == ip_dispositivo)
                    {
                        dispositivo_origen = subred.PC;
                        subred_origen = subred;
                        break;
                    }
                    if (subred.Router.IP == ip_dispositivo)
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

                // Caso 1: Envío desde PC a Router local
                if (dispositivo_origen is PC pc_origen)
                {
                    if (pc_origen.ColaPaquetes.ColaVacia())
                    {
                        Interfaz.Error("El PC no tiene paquetes para enviar");
                        Interfaz.Continuar();
                        return;
                    }

                    Paquete paquete = pc_origen.ColaPaquetes.FrenteCola();
                    Router router_local = subred_origen.Router;

                    if (router_local.RecibirPaquete(paquete))
                    {
                        pc_origen.ColaPaquetes.Quitar();
                        Console.WriteLine($"\nPaquete enviado de {pc_origen.IP} a {router_local.IP}");

                        string clave_mensaje = $"{paquete.IPOrigen}-{paquete.NumeroSecuencia}";
                        if (pc_origen.MensajesEnProceso.TryGetValue(clave_mensaje, out Mensaje mensaje))
                        {
                            mensaje.Estado = "Enviado";
                        }
                    }
                    else
                    {
                        paquete = pc_origen.ColaPaquetes.Quitar();
                        pc_origen.ColaPaquetes.Insertar(paquete);
                        Console.WriteLine($"\n¡Cola llena en {router_local.IP}! Paquete devuelto a {pc_origen.IP}");
                    }
                }
                // Caso 2 y 3: Envío desde Router
                else if (dispositivo_origen is Router router_origen)
                {
                    if (router_origen.ColaPaquetes.ColaVacia())
                    {
                        Interfaz.Error("El Router no tiene paquetes para enviar");
                        Interfaz.Continuar();
                        return;
                    }

                    Paquete paquete = router_origen.ColaPaquetes.FrenteCola();
                    string red_destino = paquete.IPDestino.Split('.')[0];

                    // Verificar si el destino está en la misma red
                    if (router_origen.Red == red_destino)
                    {
                        // Caso 3: Envío directo a PC destino (misma red)
                        PC pc_destino = SubRedes.Where(s => s.PC.IP == paquete.IPDestino).Select(s => s.PC).FirstOrDefault();

                        if (pc_destino == null)
                        {
                            Interfaz.Error("No se encontró el PC destino en la red local");
                            Interfaz.Continuar();
                            return;
                        }

                        if (pc_destino.RecibirPaquete(paquete))
                        {
                            router_origen.ColaPaquetes.Quitar();
                            Console.WriteLine($"\nPaquete enviado de {router_origen.IP} a {pc_destino.IP}");
                            pc_destino.ProcesarPaquetesRecibidos();
                        }
                        else
                        {
                            paquete = router_origen.ColaPaquetes.Quitar();
                            router_origen.ColaPaquetes.Insertar(paquete);
                            Console.WriteLine($"\n¡Cola llena en {pc_destino.IP}! Paquete devuelto a {router_origen.IP}");
                        }
                    }
                    else
                    {
                        // Caso 2: Envío a Router de otra red
                        Router router_destino = SubRedes.Where(s => s.Router.IP.StartsWith(red_destino + ".")).Select(s => s.Router).FirstOrDefault();

                        if (router_destino == null)
                        {
                            Interfaz.Error("No se encontró router destino");
                            Interfaz.Continuar();
                            return;
                        }

                        if (router_destino.RecibirPaquete(paquete))
                        {
                            router_origen.ColaPaquetes.Quitar();
                            Console.WriteLine($"\nPaquete enviado de {router_origen.IP} a {router_destino.IP}");
                        }
                        else
                        {
                            paquete = router_origen.ColaPaquetes.Quitar();
                            router_origen.ColaPaquetes.Insertar(paquete);
                            Console.WriteLine($"\n¡Cola llena en {router_destino.IP}! Paquete devuelto a {router_origen.IP}");
                        }
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

                // Mostrar Router
                Console.WriteLine($"\n🖧 ROUTER {subred.Router.IP}");
                Console.WriteLine($"📦 Paquetes en cola ({subred.Router.ColaPaquetes.Tamano()}/4):");
                MostrarColaPaquetes(subred.Router.ColaPaquetes);

                // Mostrar PC
                Console.WriteLine($"\n💻 PC {subred.PC.IP}");
                Console.WriteLine($"📤 Paquetes para enviar ({subred.PC.ColaPaquetes.Tamano()}/10):");
                MostrarColaPaquetes(subred.PC.ColaPaquetes);

                Console.WriteLine($"\n📥 Paquetes recibidos ({subred.PC.ColaRecibidos.Tamano()}/10):");
                MostrarColaPaquetes(subred.PC.ColaRecibidos);

                Console.WriteLine($"\n✉️ Mensajes completos recibidos:");
                if (subred.PC.MensajesRecibidos.Count == 0)
                {
                    Console.WriteLine("   (No hay mensajes completos)");
                }
                else
                {
                    foreach (var mensaje in subred.PC.MensajesRecibidos)
                    {
                        Console.WriteLine($"   - {mensaje.Dato} | Estado: {mensaje.Estado} | Origen: {mensaje.IPOrigen}");
                    }
                }

                Console.WriteLine($"\n═══════════════════════════════");

                // Navegación
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

        private void MostrarColaPaquetes(Cola<Paquete> cola)
        {
            if (cola.ColaVacia())
            {
                Console.WriteLine("   (Vacía)");
                return;
            }

            foreach (var paquete in cola.ObtenerElementos())
            {
                string estado = paquete.Estado.PadRight(10).Substring(0, 10);
                Console.WriteLine($"   - Paq. #{paquete.NumeroSecuencia.ToString().PadLeft(2)}: " +
                                $"[{estado}] " +
                                $"'{VisualizarCaracter(paquete.Dato)}' | " +
                                $"{paquete.IPOrigen} → {paquete.IPDestino}");
            }
        }

        private string VisualizarCaracter(char c)
        {
            if (c == '\0')
            {
                return "\0";
            }
            else
            {
                return c.ToString();
            }
        }

        #endregion
    }
}
