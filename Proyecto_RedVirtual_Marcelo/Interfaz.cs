using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_RedVirtual_Marcelo
{
    public static class Interfaz
    {
        #region Metodos

        public static string Menu()
        {
            Console.WriteLine("=== SIMULADOR DE RED VIRTUAL ===");
            Console.WriteLine("1. Configurar red de comunicación");
            Console.WriteLine("2. Crear un mensaje");
            Console.WriteLine("3. Enviar un mensaje");
            Console.WriteLine("4. Mostrar el status de la red");
            Console.WriteLine("5. Mostrar el status de la subred");
            Console.WriteLine("6. Mostrar el status de un equipo");
            Console.WriteLine("7. Eliminar un paquete de la cola");
            Console.WriteLine("8. Visualizar mensajes recibidos");
            Console.WriteLine("9. Consultar información de un paquete");
            Console.WriteLine("0. Salir");
            Console.Write("\n-Opción: ");

            string opcion = Console.ReadLine();
            return opcion;
        }

        public static void Continuar()
        {
            Console.Write("\n\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        public static void Error(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"ERROR: {mensaje}");
            Console.ResetColor();
        }

        #endregion
    }
}
