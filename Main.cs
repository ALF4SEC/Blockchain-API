using Blockchain;

// https://www.youtube.com/watch?v=V9Kr2SujqHw&t=982s -> entendimiento de blockchain

//https://www.youtube.com/watch?v=C3BBNIo4cKk -> 5 vídeos tutoriales de blockchain en C# para hacer el código y entender los conceptos de manera práctica

//https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.4.md -> Git de Swagger

//https://www.youtube.com/watch?v=i0Ybse1sqak&t=58s -> Utilizar swagger con API y exponer endpoints

var builder = WebApplication.CreateBuilder(args);

//REGISTRAR EL SERVICIO DE BLOCKCHAIN
builder.Services.AddSingleton<BlockchainRed>();

//REGISTRAR PAQUETE CONTROLADORES
builder.Services.AddControllers();

//REGISTRAR SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ACTIVAR SWAGGER PARA PODER VER LOS ENDPOINTS EN LA WEB Y QUE SEA MÁS SENCILLO
app.UseSwagger();
app.UseSwaggerUI();

//ACTIVAR ENDPOINTS
app.MapControllers();

//CORRRER LA APLICACIÓN
app.Run();
