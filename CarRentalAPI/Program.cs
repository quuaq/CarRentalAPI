using Microsoft.EntityFrameworkCore;
using CarRentalAPI.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Veritaban� ba�lant�s� ekleme
builder.Services.AddDbContext<Context>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); //Swagger'� etkinle�tirir
    app.UseSwaggerUI(); //SwaggerUI aray�z�n� etkinle�tirir
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
