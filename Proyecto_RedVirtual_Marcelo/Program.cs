using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_RedVirtual_Marcelo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            RedVirtual red = new RedVirtual();

            string opcion;
            do
            {
                opcion = Interfaz.Menu();
                switch (opcion)
                {
                    case "1":
                        red.ConfigurarRed();
                        break;
                    case "2":
                        red.CrearMensaje();
                        break;
                    case "3":
                        red.EnviarPaquetes();
                        break;
                    case "4":
                        red.MostrarStatusRed();
                        break;
                    case "5":
                        red.MostrarStatusSubRed();
                        break;
                    case "6":
                        red.MostrarStatusEquipo();
                        break;
                    case "7":
                        red.EliminarPaquete();
                        break;
                    case "8":
                        red.VisualizarMensajesRecibidos();
                        break;
                    case "9":
                        red.ConsultarPaquete();
                        break;
                    case "0":
                        Interfaz.Adios();
                        Interfaz.ImprimirLogoUSM();
                        break;
                    default:
                        Interfaz.xy(17, 23); Interfaz.Error("Opción no válida. Intente nuevamente.");
                        Console.ReadKey();
                        break;
                }
            } while (opcion != "0");
        }
    }
}
