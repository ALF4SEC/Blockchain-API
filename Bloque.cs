using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Blockchain
{
    public class Bloque
    {
        public int Indice { get; set; }
        public DateTime Momento { get; set; }
        public List<Transaccion> Transacciones { get; set; } = new();
        public string AnteriorHash { get; set; } = "";
        public string Hash { get; set; } = "";
        public int Nonce { get; set; }

        public Bloque(int indice, List<Transaccion> transacciones, string anteriorHash)
        {
            Indice = indice;
            Momento = DateTime.Now;
            Transacciones = transacciones;
            AnteriorHash = anteriorHash;
            Nonce = 0;
            Hash = CalcularHash();
        }

        // https://www.daniweb.com/programming/software-development/threads/540385/how-would-you-implement-a-simple-blockchain-in-a-programming-language-of-yo -> funciones en Python ( orientativo )

        //HAY QUE PONER UN CONSTRUCTOR VACÍO PARA CONVERTIR EL JSON DE SWAGGER EN OBJETO 
        public Bloque() { }

        public string CalcularHash()
        {
            // https://dev.to/aygun_zarbaliyeva/implementing-a-simple-blockchain-in-c-3n62 -> generar hash y clases básicas

            SHA256 sha256 = SHA256.Create();
            string texto = Indice.ToString() + Momento.ToString() + AnteriorHash + Nonce.ToString();
            byte[] textoCoded = new UTF8Encoding().GetBytes(texto);
            byte[] hashBytes = sha256.ComputeHash(textoCoded);
            string hashCalculado = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

            return hashCalculado;
        }

        public void MinarBloque(int dificultad)
        {
            string objetivo = new string('0', dificultad);
            while (!Hash.StartsWith(objetivo))
            {
                Nonce++;
                Hash = CalcularHash();
            }

        }
    }
}
