using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_RedVirtual_Marcelo
{
    internal abstract class Dispositivo
    {
        #region Atributos

        public string IP { get; protected set; }
        public string Tipo { get; protected set; }
        public Cola<Paquete> ColaPaquetes { get; set; }

        #endregion

        #region Metodos

        protected Dispositivo(string ip, string tipo, int capacidad_cola)
        {
            IP = ip;
            Tipo = tipo;
            ColaPaquetes = new Cola<Paquete>(capacidad_cola);
        }

        public virtual bool RecibirPaquete(Paquete paquete)
        {
            if (!ColaPaquetes.ColaLlena())
            {
                paquete.Estado = "Recibido";
                ColaPaquetes.Insertar(paquete);
                return true;
            }
            return false;
        }

        public virtual string ObtenerStatus()
        {
            var status = new StringBuilder();
            status.AppendLine($"Dispositivo: {IP} ({Tipo})");
            status.AppendLine($"Paquetes en cola: {ColaPaquetes.Tamano()}");

            if (!ColaPaquetes.ColaVacia())
            {
                status.AppendLine("Contenido de la cola:");
                foreach (var p in ColaPaquetes)
                {
                    status.AppendLine($"- {p}");
                }
            }
            return status.ToString();
        }

        #endregion
    }
}
