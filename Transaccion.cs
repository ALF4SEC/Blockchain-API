using System.Runtime.CompilerServices;

namespace Blockchain
{
    public class Transaccion
    {
        public string Remitente { get; set; } = "";
        public string Receptor { get; set; } = "";
        public decimal Cantidad { get; set; }
        public DateTime Momento { get; set; } = DateTime.Now;

        public Transaccion(string remitente, string receptor, decimal cantidad)
        {
            Remitente = remitente;
            Receptor = receptor;
            Cantidad = cantidad;
            Momento = DateTime.Now;
        }
    }

    
}
