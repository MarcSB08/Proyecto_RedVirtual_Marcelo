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

            var paquetes_validos = Paquetes.Where(p => p.Dato != '\0').ToList();
            var paquetes_ordenados = paquetes_validos.OrderBy(p => p.NumeroSecuencia).ToList();

            bool secuencia_correcta = true;
            for (int i = 0; i < paquetes_ordenados.Count; i++)
            {
                if (paquetes_ordenados[i].NumeroSecuencia != i + 1)
                {
                    secuencia_correcta = false;
                    break;
                }
            }

            string mensaje_reconstruido = "";
            foreach (var p in paquetes_ordenados)
            {
                mensaje_reconstruido += p.Dato;
            }

            if (!secuencia_correcta || mensaje_reconstruido != Dato || !Paquetes.Any(p => p.Dato == '\0'))
            {
                Estado = "Dañado";
                return false;
            }

            Estado = "Recibido";
            return true;
        }

        public override string ToString()
        {
            return $"Mensaje: {Dato} | {IPOrigen} -> {IPDestino} | Estado: {Estado}";
        }

        #endregion
    }
}
