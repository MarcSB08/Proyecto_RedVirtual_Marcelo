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

        public Cola<Paquete> ColaRecibidos { get; set; }
        public List<Mensaje> MensajesRecibidos { get; private set; }

        #endregion

        #region Metodos

        public PC(string ip) : base(ip, "PC", 10)
        {
            ColaRecibidos = new Cola<Paquete>(10);
            MensajesRecibidos = new List<Mensaje>();
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
                paquete.Estado = "Recibido";
                ColaRecibidos.Insertar(paquete);
                return true;
            }
            return false;
        }

        public void ProcesarPaquetesRecibidos()
        {
            if (ColaRecibidos.ColaVacia()) return;

            var paquetes_por_mensaje = new Dictionary<string, List<Paquete>>();

            foreach (var paquete in ColaRecibidos.ObtenerElementos())
            {
                string clave = $"{paquete.IPOrigen}-{paquete.IPDestino}";
                if (!paquetes_por_mensaje.ContainsKey(clave))
                {
                    paquetes_por_mensaje[clave] = new List<Paquete>();
                }
                paquetes_por_mensaje[clave].Add(paquete);
            }

            foreach (var grupo in paquetes_por_mensaje)
            {
                var paquetes = grupo.Value.OrderBy(p => p.NumeroSecuencia).ToList();
                bool tiene_fin = paquetes.Any(p => p.Dato == '\0');

                if (tiene_fin)
                {
                    string contenido = "";
                    foreach (var p in paquetes.Where(p => p.Dato != '\0').OrderBy(p => p.NumeroSecuencia))
                    {
                        contenido += p.Dato;
                    }

                    var mensaje = new Mensaje(paquetes.First().IPOrigen, paquetes.First().IPDestino, contenido);
                    mensaje.Paquetes = paquetes;
                    mensaje.VerificarIntegridad();
                    MensajesRecibidos.Add(mensaje);

                    var nueva_cola = new Cola<Paquete>(10);
                    foreach (var p in ColaRecibidos.ObtenerElementos())
                    {
                        if (!paquetes.Contains(p))
                        {
                            nueva_cola.Insertar(p);
                        }
                    }
                    ColaRecibidos = nueva_cola;
                }
            }
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
