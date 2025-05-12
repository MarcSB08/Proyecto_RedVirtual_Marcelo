using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_RedVirtual_Marcelo
{
    internal class Router : Dispositivo
    {
        #region Atributos

        public string Red { get; private set; }

        #endregion

        #region Metodos

        public Router(string ip) : base(ip, "Router", 4)
        {
            Red = ip.Split('.')[0];
        }

        public bool PerteneceARed(string ip_destino)
        {
            var partes_destino = ip_destino.Split('.');
            if (partes_destino.Length < 1) return false;

            string red_destino = partes_destino[0];

            return red_destino == this.Red;
        }

        public override string ObtenerStatus()
        {
            var status = base.ObtenerStatus();
            status += $"\nRed asociada: {Red}";
            return status;
        }

        #endregion
    }
}
