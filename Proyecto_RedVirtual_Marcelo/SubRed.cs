using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_RedVirtual_Marcelo
{
    internal class SubRed
    {
        #region Atributos

        public int Numero { get; set; }
        public PC PC { get; private set; }
        public Router Router { get; private set; }

        #endregion

        #region Metodos

        public SubRed(int numero, string ip_router, string ip_PC)
        {
            Numero = numero;
            Router = new Router(ip_router);
            PC = new PC(ip_PC);
        }

        #endregion
    }
}
