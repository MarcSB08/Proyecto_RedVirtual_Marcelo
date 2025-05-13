using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_RedVirtual_Marcelo
{
    internal class Cola<T> : IEnumerable<T>
    {
        private int Frente;
        private int Final;
        private readonly T[] ListaCola;
        public readonly int MaxTam;

        public Cola(int tamano = 10)
        {
            MaxTam = tamano;
            ListaCola = new T[MaxTam];
            Frente = 0;
            Final = -1;
        }

        public void Insertar(T elemento)
        {
            if (!ColaLlena())
            {
                ListaCola[++Final] = elemento;
            }
            else
            {
                throw new OverflowException("Overflow en la cola");
            }
        }

        public T Quitar()
        {
            if (!ColaVacia())
            {
                T elemento = ListaCola[Frente];
                ListaCola[Frente] = default;

                if (Frente == Final)
                {
                    Frente = 0;
                    Final = -1;
                }
                else
                {
                    Frente++;
                }

                return elemento;
            }
            throw new InvalidOperationException("Cola vacía");
        }

        public void BorrarCola()
        {
            Array.Clear(ListaCola, 0, ListaCola.Length);
            Frente = 0;
            Final = -1;
        }

        public T FrenteCola()
        {
            if (!ColaVacia()) return ListaCola[Frente];
            throw new InvalidOperationException("Cola vacía");
        }

        public bool ColaVacia() => Frente > Final;

        public bool ColaLlena() => Final == MaxTam - 1;

        public int Tamano()
        {
            if (ColaVacia()) return 0;
            return Final - Frente + 1;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = Frente; i <= Final; i++)
            {
                yield return ListaCola[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contiene(Func<T, bool> predicado)
        {
            for (int i = Frente; i <= Final; i++)
            {
                if (predicado(ListaCola[i])) return true;
            }
            return false;
        }

        public List<T> ObtenerElementos()
        {
            var elementos = new List<T>();
            for (int i = Frente; i <= Final; i++)
            {
                elementos.Add(ListaCola[i]);
            }
            return elementos;
        }
    }
}
