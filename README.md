# Blockchain API

Práctica de **Sistemas Distribuidos**: una blockchain sencilla expuesta como API REST con ASP.NET Core y Swagger.

La práctica está pensada para ejecutarse en **2 o más ordenadores**. Cada ordenador ejecuta un nodo con su propia copia de la cadena, los nodos se registran entre sí y llegan a un consenso con la regla de la **cadena válida más larga**.

## Funcionalidades

- Bloque génesis creado al arrancar el nodo.
- Transacciones pendientes que se incluyen en el siguiente bloque minado.
- Minado por **prueba de trabajo** (SHA-256, el hash debe empezar por `000`) con recompensa de 10 unidades para el minero.
- Validación de la cadena comparando hashes y enlaces con el bloque anterior.
- Registro de nodos vecinos y resolución de conflictos: cada nodo descarga la cadena de sus vecinos y adopta la más larga si es válida.
- Consulta del saldo de una dirección recorriendo todas las transacciones.

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Los ordenadores deben estar en la misma red y verse entre sí (revisar el firewall).

## Ejecución

En **cada ordenador**:

```bash
dotnet run --urls "http://0.0.0.0:5000"
```

Se escucha en `0.0.0.0` para que el nodo acepte peticiones de los demás ordenadores y no solo de `localhost`. La interfaz de Swagger queda en `http://<ip>:5000/swagger`.

## Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/blockchain/cadena` | Devuelve la cadena completa |
| GET | `/blockchain/validar` | Comprueba si la cadena es válida |
| POST | `/blockchain/transaccion/nueva` | Añade una transacción pendiente |
| GET | `/blockchain/transaccion/pendientes` | Lista las transacciones pendientes |
| POST | `/blockchain/minar` | Mina un bloque con las transacciones pendientes |
| POST | `/blockchain/nodos/registrar` | Registra nodos vecinos |
| GET | `/blockchain/nodos/resolverConflictos` | Aplica el consenso de la cadena más larga |
| GET | `/blockchain/saldo/{direccion}` | Devuelve el saldo de una dirección |

## Ejemplo con dos ordenadores

Suponiendo que el nodo A está en `192.168.1.10` y el nodo B en `192.168.1.20`:

1. Registrar cada nodo en el otro. En A:
   ```bash
   curl -X POST http://192.168.1.10:5000/blockchain/nodos/registrar \
        -H "Content-Type: application/json" \
        -d '{"nodos": ["http://192.168.1.20:5000"]}'
   ```
   Repetir en B con la IP de A.
2. Crear una transacción y minar en A:
   ```bash
   curl -X POST http://192.168.1.10:5000/blockchain/transaccion/nueva \
        -H "Content-Type: application/json" \
        -d '{"remitente": "alice", "receptor": "bob", "cantidad": 5}'

   curl -X POST http://192.168.1.10:5000/blockchain/minar \
        -H "Content-Type: application/json" \
        -d '{"minero": "nodoA"}'
   ```
3. Sincronizar B, que adopta la cadena de A por ser más larga:
   ```bash
   curl http://192.168.1.20:5000/blockchain/nodos/resolverConflictos
   ```

## Estructura

- `Main.cs`: arranque de la aplicación, registro de servicios y Swagger.
- `Swagger.cs`: controlador con los endpoints REST.
- `BlockchainRed.cs`: lógica de la cadena, minado, validación y consenso.
- `Bloque.cs`: bloque, cálculo del hash y prueba de trabajo.
- `Transaccion.cs`: modelo de transacción.
- `INFORME.pdf`: memoria de la práctica.
