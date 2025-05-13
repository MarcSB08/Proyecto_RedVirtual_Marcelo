using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Proyecto_RedVirtual_Marcelo
{
    public static class Interfaz
    {
        #region Metodos

        public static string Menu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Borde();
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            Titulo();
            Console.ResetColor();
            xy(15, 11); Console.WriteLine("1. Configurar red de comunicación");
            xy(15, 12); Console.WriteLine("2. Crear un mensaje");
            xy(15, 13); Console.WriteLine("3. Enviar un mensaje");
            xy(15, 14); Console.WriteLine("4. Mostrar el status de la red");
            xy(15, 15); Console.WriteLine("5. Mostrar el status de la subred");
            xy(15, 16); Console.WriteLine("6. Mostrar el status de un equipo");
            xy(15, 17); Console.WriteLine("7. Eliminar un paquete de la cola");
            xy(15, 18); Console.WriteLine("8. Visualizar mensajes recibidos");
            xy(15, 19); Console.WriteLine("9. Consultar información de un paquete");
            xy(15, 20); Console.WriteLine("0. Salir");
            xy(8, 23); Console.Write("-Opción: ");

            string opcion = Console.ReadLine();
            return opcion;
        }

        public static void Borde()
        {
            int ancho = 100, alto = 25;

            Console.SetCursorPosition(6, 2);
            Console.Write("╔");

            Console.SetCursorPosition(ancho + 8, 2);
            Console.Write("╗");

            Console.SetCursorPosition(6, alto - 1);
            Console.Write("╚");

            Console.SetCursorPosition(ancho + 8, alto - 1);
            Console.Write("╝");

            for (int i = 8; i < ancho + 9; i++)
            {
                Console.SetCursorPosition(i - 1, 2);
                Console.Write("═");

                Console.SetCursorPosition(i - 1, alto - 1);
                Console.Write("═");
            }

            for (int i = 3; i < alto - 1; i++)
            {
                Console.SetCursorPosition(6, i);
                Console.Write("║");

                Console.SetCursorPosition(ancho + 8, i);
                Console.Write("║");
            }
        }

        public static void Titulo()
        {
            xy(16, 4); Console.WriteLine("██████╗ ███████╗██████╗      ██╗   ██╗██╗██████╗ ████████╗██╗   ██╗ █████╗ ██╗     ");
            xy(16, 5); Console.WriteLine("██╔══██╗██╔════╝██╔══██╗     ██║   ██║██║██╔══██╗╚══██╔══╝██║   ██║██╔══██╗██║     ");
            xy(16, 6); Console.WriteLine("██████╔╝█████╗  ██║  ██║     ██║   ██║██║██████╔╝   ██║   ██║   ██║███████║██║     ");
            xy(16, 7); Console.WriteLine("██╔══██╗██╔══╝  ██║  ██║     ╚██╗ ██╔╝██║██╔══██╗   ██║   ██║   ██║██╔══██║██║     ");
            xy(16, 8); Console.WriteLine("██║  ██║███████╗██████╔╝      ╚████╔╝ ██║██║  ██║   ██║   ╚██████╔╝██║  ██║███████╗");
            xy(16, 9); Console.WriteLine("╚═╝  ╚═╝╚══════╝╚═════╝        ╚═══╝  ╚═╝╚═╝  ╚═╝   ╚═╝    ╚═════╝ ╚═╝  ╚═╝╚══════╝");
        }

        public static void Adios()
        {
            int x = 15, y = 10;

            Console.Clear();
            Thread.Sleep(50); xy(x, y++); Console.WriteLine("    _       _ _               ____  ");
            Thread.Sleep(50); xy(x, y++); Console.WriteLine("   / \\   __| (_) ___  ___   _|  _ \\ ");
            Thread.Sleep(50); xy(x, y++); Console.WriteLine("  / _ \\ / _` | |/ _ \\ __| (_) | | |");
            Thread.Sleep(50); xy(x, y++); Console.WriteLine(" / ___ \\ (_| | | (_) \\__\\  _| |_| |");
            Thread.Sleep(50); xy(x, y++); Console.WriteLine("/_/     \\__,_|_||___/|___/ (_)____/ ");
            Thread.Sleep(50); xy(x, y++); Console.ResetColor();
        }

        public static void ImprimirLogoUSM()
        {
            int x = 60, y = 7;
            Console.ForegroundColor = ConsoleColor.Blue;
            Borde();

            Thread.Sleep(50); xy(x, y); Console.WriteLine("         .+%%%%%%+.=#%%%%#:+%%%%%%%%%%%-        ");
            Thread.Sleep(50); xy(x, y++); Console.WriteLine("      .+%%%%%%*.-#%%%%%:=%%%%%%%%%%%%#-.      ");
            Thread.Sleep(50); xy(x, y++); Console.WriteLine("     .+%%%%%%%::*%%%%%--%%%%%%%%%%%%%%#-.     ");
            Thread.Sleep(50); xy(x, y++); Console.WriteLine("    .+%%%%%%%-.*%%%%%=:%%%%#==++++++++++:.    ");
            Thread.Sleep(50); xy(x, y++); Console.WriteLine("   .+%%%%%%%+.+%%%%%+:#%%%%==%%%%%%%%%%%#-.   ");
            Thread.Sleep(50); xy(x, y++); Console.WriteLine("  .=%%%%%%%%++%%%%%*:*%%%%+-%%%%%%%%%%%%%%-.  ");
            Thread.Sleep(50); xy(x, y++); Console.WriteLine(" .:*%%%%%%%%%%%%%%#:+%%%%*-#%%%%%%%%%%%%%%#:. ");
            Thread.Sleep(50); xy(x, y++); Console.WriteLine("  .:*%%%%%%%%%%%%#-+%%%%#:*%%%%%%%%%%%%%%%=.  ");
            Thread.Sleep(50); xy(x, y++); Console.WriteLine("    .+%%%%%%%%%%#=-#%%%%-+%%*+%#=*%%%%%%%=.   ");
            Thread.Sleep(50); xy(x, y++); Console.WriteLine("     .:==========+#%%%%-=%%*-*%=:#%%%%%%+.    ");
            Thread.Sleep(50); xy(x, y++); Console.WriteLine("      :*%%%%%%%%%%%%%%=:%%#-+%*:*%%%%%%+.     ");
            Thread.Sleep(50); xy(x, y++); Console.WriteLine("       .+%%%%%%%%%%%%*:%%%=-%%--%%%%%%*.      ");
            Thread.Sleep(50); xy(x, y++); Console.WriteLine("        .=%%%%%%%%%%#:#%%+:#%+:*%%%%%*.       ");

            Console.ResetColor();
            xy(x, y++); Console.ReadKey();
            Console.Clear();
        }

        public static void xy(int x, int y)
        {
            Console.SetCursorPosition(x, y);
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
