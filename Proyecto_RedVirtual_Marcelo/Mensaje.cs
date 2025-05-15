using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_RedVirtual_Marcelo
{
    internal class Mensaje
    {
        #region Atributos

        public string IPOrigen { get; set; }
        public string IPDestino { get; set; }
        public string Dato { get; set; }
        public string Estado { get; set; }
        public List<Paquete> Paquetes { get; set; }
        public DateTime FechaCreacion { get; set; }

        #endregion

        #region Metodos

        public Mensaje(string ip_origen, string ip_destino, string dato)
        {
            IPOrigen = ip_origen;
            IPDestino = ip_destino;
            Dato = dato;
            Estado = "Nuevo";
            Paquetes = new List<Paquete>();
            FechaCreacion = DateTime.Now;

            for (int i = 0; i < dato.Length; i++)
            {
                Paquetes.Add(new Paquete(ip_origen, ip_destino, i + 1, dato[i]));
            }

            Paquetes.Add(new Paquete(ip_origen, ip_destino, dato.Length + 1, '\0'));
        }

        public bool VerificarIntegridad()
        {
            if (Estado == "Dañado") return false;

            var paquetes_contenido = Paquetes.Where(p => p.Dato != '\0').OrderBy(p => p.NumeroSecuencia).ToList();

            bool secuencia_correcta = true;
            for (int i = 0; i < paquetes_contenido.Count; i++)
            {
                if (paquetes_contenido[i].NumeroSecuencia != i + 1)
                {
                    secuencia_correcta = false;
                    break;
                }
            }

            string mensaje_recibido = string.Concat(paquetes_contenido.Select(p => p.Dato));
            bool contenido_correcto = mensaje_recibido == Dato;
            bool tiene_terminador = Paquetes.Any(p => p.Dato == '\0');

            Estado = (secuencia_correcta && contenido_correcto && tiene_terminador) ? "Recibido" : "Dañado";

            return Estado == "Recibido";
        }

        #endregion
    }
}
