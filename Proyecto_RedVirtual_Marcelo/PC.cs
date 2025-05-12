using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_RedVirtual_Marcelo
{
    internal class PC : Dispositivo
    {
        #region Atributos

        public Cola<Paquete> ColaRecibidos { get; private set; }
        public List<Mensaje> MensajesRecibidos { get; private set; }
        public Dictionary<string, Mensaje> MensajesEnProceso { get; private set; }

        #endregion

        #region Metodos

        public PC(string ip) : base(ip, "PC", 10)
        {
            ColaRecibidos = new Cola<Paquete>(10);
            MensajesRecibidos = new List<Mensaje>();
            MensajesEnProceso = new Dictionary<string, Mensaje>();
        }

        public void CrearMensaje(string ip_destino, string contenido)
        {
            var mensaje = new Mensaje(this.IP, ip_destino, contenido);

            foreach (var paquete in mensaje.Paquetes)
            {
                if (!ColaPaquetes.ColaLlena())
                {
                    ColaPaquetes.Insertar(paquete);
                }
                else
                {
                    throw new Exception("La cola de envío está llena. No se pueden agregar más paquetes");
                }
            }
        }

        public override bool RecibirPaquete(Paquete paquete)
        {
            if (!ColaRecibidos.ColaLlena())
            {
                ColaRecibidos.Insertar(paquete);
                ProcesarPaquetesRecibidos();
                return true;
            }
            return false;
        }

        public void ProcesarPaquetesRecibidos()
        {
            while (!ColaRecibidos.ColaVacia())
            {
                var paquete = ColaRecibidos.FrenteCola();

                var clave = $"{paquete.IPOrigen}-{paquete.NumeroSecuencia}";
                if (!MensajesEnProceso.ContainsKey(clave))
                {
                    var mensaje = new Mensaje(paquete.IPOrigen, this.IP, "");
                    MensajesEnProceso[clave] = mensaje;
                }

                var mensaje_actual = MensajesEnProceso[clave];
                mensaje_actual.Paquetes.Add(ColaRecibidos.Quitar());

                if (paquete.Dato == '\0')
                {
                    mensaje_actual.VerificarIntegridad();
                    MensajesRecibidos.Add(mensaje_actual);
                    MensajesEnProceso.Remove(clave);
                }
            }
        }

        public string ObtenerMensajesRecibidos()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Mensajes recibidos en PC {IP}:");

            if (MensajesRecibidos.Count == 0)
            {
                sb.AppendLine("No hay mensajes recibidos");
            }
            else
            {
                foreach (var mensaje in MensajesRecibidos)
                {
                    sb.AppendLine(mensaje.ToString());
                }
            }

            return sb.ToString();
        }

        public override string ObtenerStatus()
        {
            var status = base.ObtenerStatus();
            status += $"\nPaquetes recibidos: {ColaRecibidos.Tamano()}";
            status += $"\nMensajes completos recibidos: {MensajesRecibidos.Count}";
            return status;
        }

        #endregion
    }
}
