using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_RedVirtual_Marcelo
{
    internal class Paquete
    {
        #region Atributos

        public string IPOrigen { get; set; }
        public string IPDestino { get; set; }
        public int NumeroSecuencia { get; set; }
        public char Dato { get; set; }
        public string Estado { get; set; }

        #endregion

        #region Metodos

        public Paquete(string ip_origen, string ip_destino, int numero_secuencia, char dato)
        {
            IPOrigen = ip_origen;
            IPDestino = ip_destino;
            NumeroSecuencia = numero_secuencia;
            Dato = dato;
            Estado = "Nuevo";
        }

        public override string ToString()
        {
            return $"Paquete {NumeroSecuencia}: {IPOrigen} -> {IPDestino}, Dato: '{Dato}', Estado: {Estado}";
        }

        #endregion
    }
}
