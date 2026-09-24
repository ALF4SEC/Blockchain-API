using Microsoft.AspNetCore.Mvc;

namespace Blockchain
{
    //HAY QUE ESPECIFICAR QUE ES UN CONTROLADOR
    [ApiController]

    //RECORD SIRVE PARA NO TENER QUE CREAR LA CLASE COMO TAL, YA QUE NO SE UTILIZA MÁS QUE AQUÍ. SE CREA CON SUS PROPIEDAES Y CONSTRUCTOR CORRESPONDIENTES

    [Route("blockchain")]
    public class Swagger : ControllerBase
    {
        private BlockchainRed blockchain;

        public Swagger(BlockchainRed blockchain)
        {
            this.blockchain = blockchain;
        }

        [HttpGet("cadena")]

        //CON EL IACTIONRESULT, PERMITIMOS PODER DEVOLVER OK,BADREQUEST Y NOTFOUND, PARA LOS DISTINTOS CASOS A LAS PETICIONES REALIZADAS
        public IActionResult ObtenerCadena()
        {
            return Ok(blockchain.Cadena);
        }

        [HttpGet("validar")]
        public IActionResult Validar()
        {
            bool valida = blockchain.Validez(blockchain.Cadena);
            return Ok(new { valida, mensaje = valida ? "LA CADENA INTRODUCIDA ES CORRECTA." : "LA CADENA NO ES VÁLIDA." });
        }

        [HttpPost("transaccion/nueva")]
        public IActionResult NuevaTransaccion([FromBody] Transaccion transaccion)
        {
            int indiceBloque = blockchain.AñadirTransaccion(transaccion);
            return Ok(new { mensaje = $"LA TRANSACCION SE ENCUENTRA EN EL BLOQUE {indiceBloque}." });
        }

        [HttpGet("transaccion/pendientes")]
        public IActionResult ObtenerPendientes()
        {
            return Ok(blockchain.TransaccionesPendientes);
        }

        [HttpPost("minar")]

        //FROMBODY HACE QUE PODAMOS OBTENER EL NOMBRE DEL MINERO DEL CUERPO INTRODUCIDO
        //https://learn.microsoft.com/es-es/aspnet/web-api/overview/formats-and-model-binding/parameter-binding-in-aspnet-web-api
        public IActionResult Minar([FromBody] PeticionMinado peticion)
        {
            if (string.IsNullOrWhiteSpace(peticion.Minero))
                return BadRequest(new { error = "ERROR EN LA DIRECCIÓN DEL NODO MINERO" });

            if (!blockchain.TransaccionesPendientes.Any())
                return BadRequest(new { error = "TODAVÍA NO HAY NUEVAS TRANSACCIONES, NO SE PUEDE MINAR" });

            Bloque bloque = blockchain.Minar(peticion.Minero);
            return Ok(new{mensaje = "BLOQUE MINADO",indice = bloque.Indice,hash = bloque.Hash,transacciones = bloque.Transacciones});
        }

        [HttpPost("nodos/registrar")]
        public IActionResult RegistrarNodos([FromBody] PeticionNodos peticion)
        {
            if (peticion.Nodos == null || !peticion.Nodos.Any())
                return BadRequest(new { error = "ERROR AL REGISTRAR EL NODO" });

            foreach (string nodo in peticion.Nodos)
                blockchain.RegistrarNodo(nodo);

            return Ok(new { mensaje = "NODOS REGISTRADOS:", nodos = blockchain.Nodos });
        }

        [HttpGet("nodos/resolverConflictos")]
        public IActionResult ResolverConflictos()
        {
            bool reemplazada = blockchain.ResolverConflictos();
            if (reemplazada)
                return Ok(new { mensaje = "LA CADENA HA SIDO REEMPLAZADA POR UNA MÁS LARGA.", cadena = blockchain.Cadena });
            else
                return Ok(new { mensaje = "SE TENÍA LA CADENA MÁS LARGA, NO HA HABIDO REEMPLAZO", cadena = blockchain.Cadena });
        }

        [HttpGet("saldo/{direccion}")]
        public IActionResult ObtenerSaldo(string direccion)
        {
            decimal saldo = blockchain.Dinero(direccion);
            return Ok(new { direccion, saldo });
        }
    }

    public record PeticionMinado(string Minero);
    public record PeticionNodos(List<string> Nodos);

}