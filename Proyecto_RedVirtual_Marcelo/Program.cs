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
                Console.Clear();
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
                        break;
                    case "6":
                        break;
                    case "7":
                        break;
                    case "8":
                        break;
                    case "9":
                        break;
                    case "0":
                        Console.WriteLine("Saliendo del programa...");
                        Interfaz.Continuar();
                        break;
                    default:
                        Interfaz.Error("Opción no válida. Intente nuevamente.");
                        Interfaz.Continuar();
                        break;
                }
            } while (opcion != "0");
        }
    }
}
