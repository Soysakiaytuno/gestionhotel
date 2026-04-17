using HotelBackend.Repository;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. INYECCIÓN DE DEPENDENCIAS (REPOSITORIOS)
// ==========================================
// Ahora ya no hay 'AddDbContext' de Entity Framework.
// Inyectamos los repositorios y ellos mismos leerán la cadena de conexión.
builder.Services.AddScoped<RepositorioHabitacion>();
builder.Services.AddScoped<RepositorioHuesped>();
builder.Services.AddScoped<RepositorioEstadia>();

// ==========================================
// 2. SERVICIOS WEB Y CORS
// ==========================================
builder.Services.AddControllers();

builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("PermitirFrontend", politica =>
    {
        politica.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
    });
});

// ==========================================
// 3. SWAGGER (DOCUMENTACIÓN)
// ==========================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("PermitirFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();