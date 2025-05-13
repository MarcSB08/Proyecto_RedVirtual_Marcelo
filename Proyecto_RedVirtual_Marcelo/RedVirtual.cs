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

        public void EnviarPaquetes()  // Opción 3 del menú
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
                string ipOrigen = Console.ReadLine().Trim();

                Dispositivo dispositivoOrigen = null;
                SubRed subRedOrigen = null;

                foreach (var subred in SubRedes)
                {
                    if (subred.PC.IP == ipOrigen)
                    {
                        dispositivoOrigen = subred.PC;
                        subRedOrigen = subred;
                        break;
                    }
                    if (subred.Router.IP == ipOrigen)
                    {
                        dispositivoOrigen = subred.Router;
                        subRedOrigen = subred;
                        break;
                    }
                }

                if (dispositivoOrigen == null)
                {
                    Interfaz.Error("No se encontró el dispositivo");
                    Interfaz.Continuar();
                    return;
                }

                // Obtener paquete a enviar
                Paquete paquete = null;
                if (!dispositivoOrigen.ColaPaquetes.ColaVacia())
                {
                    paquete = dispositivoOrigen.ColaPaquetes.FrenteCola();
                }
                else
                {
                    Interfaz.Error("El dispositivo no tiene paquetes para enviar");
                    Interfaz.Continuar();
                    return;
                }

                Dispositivo siguienteDispositivo = null;
                string accion = "";

                if (dispositivoOrigen is PC)
                {
                    // Envío desde PC a Router local
                    siguienteDispositivo = subRedOrigen.Router;
                    accion = $"enviado desde PC {ipOrigen} a Router local {siguienteDispositivo.IP}";
                }
                else if (dispositivoOrigen is Router routerOrigen)
                {
                    var ipDestino = paquete.IPDestino;
                    var subRedDestino = SubRedes.FirstOrDefault(s => s.PC.IP == ipDestino);

                    if (subRedDestino == null)
                    {
                        Interfaz.Error($"No existe destino {ipDestino}");
                        paquete.Estado = "Dañado";
                        dispositivoOrigen.ColaPaquetes.Insertar(dispositivoOrigen.ColaPaquetes.Quitar());
                        Interfaz.Continuar();
                        return;
                    }

                    if (routerOrigen.Red == subRedDestino.Router.Red)
                    {
                        // Misma red - enviar directamente al PC destino
                        siguienteDispositivo = subRedDestino.PC;
                        accion = $"enviado desde Router {ipOrigen} a PC destino {siguienteDispositivo.IP}";
                    }
                    else
                    {
                        // Red diferente - enviar al router destino
                        siguienteDispositivo = subRedDestino.Router;
                        accion = $"enrutado desde Router {ipOrigen} a Router {siguienteDispositivo.IP}";
                    }
                }

                // Procesar el envío
                if (siguienteDispositivo != null)
                {
                    bool envioExitoso = false;

                    if (siguienteDispositivo is PC pcDestino)
                    {
                        envioExitoso = pcDestino.RecibirPaquete(paquete);
                        if (envioExitoso)
                        {
                            pcDestino.ProcesarPaquetesRecibidos();
                            paquete.Estado = "Recibido";
                            dispositivoOrigen.ColaPaquetes.Quitar(); // Solo quitamos si se envió con éxito
                        }
                    }
                    else
                    {
                        envioExitoso = siguienteDispositivo.RecibirPaquete(paquete);
                        if (envioExitoso)
                        {
                            paquete.Estado = "Enviado";
                            dispositivoOrigen.ColaPaquetes.Quitar(); // Solo quitamos si se envió con éxito
                        }
                    }

                    if (envioExitoso)
                    {
                        Console.WriteLine($"\nPaquete {paquete.NumeroSecuencia} {accion}");
                    }
                    else
                    {
                        paquete.Estado = "Devuelto";
                        // El paquete ya está en la cola (no se quitó), solo se actualiza el estado
                        Interfaz.Error($"Cola llena en {siguienteDispositivo.IP}, paquete devuelto a {ipOrigen}");
                    }
                }
            }
            catch (Exception ex)
            {
                Interfaz.Error($"Error: {ex.Message}");
            }

            Interfaz.Continuar();
        }

        public void MostrarStatusRed()
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
                string dato = paquete.Dato == '\0' ? "[FIN]" : $"'{paquete.Dato}'";
                Console.WriteLine($"   - Paquete {paquete.NumeroSecuencia.ToString().PadLeft(2)} " +
                                $"[{estado}] {dato} | " +
                                $"{paquete.IPOrigen} → {paquete.IPDestino}");
            }
        }

        #endregion
    }
}
