using System.Net;
using System.Text;
using System.Text.Json;

namespace Blockchain
{
    public class BlockchainRed
    {
        private List<Bloque> cadena = new();
        private List<Transaccion> transaccionesPendientes = new();
        private HashSet<string> nodos = new();
        private int Ceros = 3; //CEROS AL PRINCIPIO DE LA SOLUCIÓN, COMO OCURRE CON BITCOIN
        private decimal Recompensa = 10m;

        public BlockchainRed()
        {
            CrearBloqueGenesis(); 
        }

        private void CrearBloqueGenesis()
        {
            Bloque genesis = new Bloque(0, new List<Transaccion>(), "0");
            genesis.Hash = genesis.CalcularHash();
            cadena.Add(genesis);

        }

        //https://www.reddit.com/r/csharp/comments/xfr69h/last_item_in_c/?tl=es-es -> ultimo elemento de cadena
        public Bloque UltimoBloque => cadena[^1]; // COGE EL ÚLTIMO ELEMENTO, Y ESTO SE CALCULA SÓLO CUANDO SE USA, SINO DA ERROR
        public List<Bloque> Cadena => cadena;
        public List<Transaccion> TransaccionesPendientes => transaccionesPendientes;
        public HashSet<string> Nodos => nodos;


        public int AñadirTransaccion(Transaccion nueva)
        {
            // AÑADIMOS LA TRANSACCION A LAS PENDIENTES, DEVOLVEMOS EL ÍNDICE PARA SABER EN QUÉ BLOQUE VA CUANDO SE MINE

            TransaccionesPendientes.Add(nueva);
            return UltimoBloque.Indice + 1; 
        }

        public Bloque Minar(string nombreMinador)
        {

            TransaccionesPendientes.Add(new Transaccion("SISTEMA", nombreMinador, Recompensa));

            Bloque bloque = new Bloque(cadena.Count,new List<Transaccion>(TransaccionesPendientes),UltimoBloque.Hash);

            bloque.MinarBloque(Ceros);
            cadena.Add(bloque);
            transaccionesPendientes.Clear();

            return bloque;
        }


        public bool Validez(List<Bloque> cadena)
        {
            //VAMOS A RECORRRE TODOS LOS BLOQUES, EMPEZANDO POR EL 1, YA QUE EL GÉNESIS NO TIENE ANTERIOR
            //SE COMPARA HACE LA COMPARACION DE HASHES. SE CALCULA EL HASH DEL BLOQUE Y SE COMPARA CON EL QUE TENGA ALMACENADO PARA 
            //PODER COMPROBAR SI ALGUIEN HA MODIFICADO EL CONTENIDO DEL BLOQUE. ADEMÁS POR EL FUNCIONAMIENTO DE BLOCKCHAIN
            //ES SEGURO COMPROBAR EL HASH ANTERIOR ALMACENADO CON EL HASH ANTERIOR REAL, HACIENDO QUE SE ENCADENEN

            //TAMBIÉN SIRVE PARA LA COMPROBACION QUE HAY QUE REALIZAR PARA RESOLVER LOS CONFLICTOS
            //COMO ESTA SE HACE A TRAVÉS DE QUIEN TIENE LA CADENA MÁS LARGA, HAY QUE VERIFICAR QUE DICHA CADENA ESTÉ CORRECTA
            //EN CASO DE NO HACERLO, SE PODRÍA PASAR UNA CADENA MUY LARGA FALSA Y ASÍ PODER AÑADIR EL BLOQUE


            for (int i = 1; i < cadena.Count; i++)
            {
                var actual = cadena[i];
                var anterior = cadena[i - 1];

                if (actual.Hash != actual.CalcularHash())
                {
                    return false; 
                }

                if (actual.AnteriorHash != anterior.Hash)
                {
                    return false;
                }
            }
            return true;
        }

        public void RegistrarNodo(string url)
        {
            //PARA PODER REGISTRAR LOS NODOS VECINOS, SE AÑADE A LA LISTA DE NODOS LA URL ( http://localhost:5000 ) 
            //HAY QUE QUITAR LA BARRA DEL FINAL
            // ESTO SIRVE PARA CUANDO SE QUIERE LLEGAR A UN CONSENSO, QUE CADA NODO SEPA A QUIEN PREGUNTAR
            // https://learn.microsoft.com/es-es/dotnet/standard/base-types/trimming -> eliminar barra al final de url
            nodos.Add(url.TrimEnd('/'));
        }


        public bool ResolverConflictos()
        {
            bool reemplazada = false;
            int maximaLongitud = cadena.Count;
            List<Bloque> nuevaCadena = new();

            foreach (string nodo in nodos)
            {
                try
                {
                    using var cliente = new WebClient();
                    string respuesta = cliente.DownloadString($"{nodo}/blockchain/cadena");

                    List<Bloque>? cadenaExterna = JsonSerializer.Deserialize<List<Bloque>>(respuesta, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (cadenaExterna != null && cadenaExterna.Count > maximaLongitud && Validez(cadenaExterna))
                    {
                        maximaLongitud = cadenaExterna.Count;
                        nuevaCadena = cadenaExterna;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR AL CREAR EN EL CONSENSO");
                }
            }

            // SI NO HA SUPERADO LA MAXIMA LONGITUD, ESTA ESTARÁ VACÍA, POR LO QUE NO ENTRARÁ NI REEMPLAZARÁ NADA
            if (nuevaCadena.Count > 0) 
            {
                cadena.Clear();

                foreach (Bloque bloque in nuevaCadena)
                {
                    cadena.Add(bloque);
                }

                reemplazada = true;
            }

            return reemplazada;
        }

        public decimal Dinero(string direccion)
        {

            //SE RECORRE BLOQUE A BLOQUE CADA UNA DE LAS TRANSACCIONES. SI SE ES EL RECEPTOR, SE SUMA CANTIDAD. SI ERES REMITENTE, SE RESTA
            decimal saldo = 0;
            foreach (Bloque bloque in cadena)
                foreach (Transaccion transaccion in bloque.Transacciones)
                {
                    if (transaccion.Receptor == direccion) saldo += transaccion.Cantidad;
                    if (transaccion.Remitente == direccion) saldo -= transaccion.Cantidad;
                }
            return saldo;
        }
    }
}
