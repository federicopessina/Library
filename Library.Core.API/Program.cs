
using Library.Core;
using Library.Core.API.Controllers;
using Library.Core.Stores;
using Library.Interfaces;
using Library.Interfaces.Controllers;
using Library.Interfaces.Stores;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add dependencies.
builder.Services.AddSingleton<IBookStore, BookStore>();
builder.Services.AddSingleton<IPersonStore, PersonStore>();
builder.Services.AddSingleton<ICardStore, CardStore>();
builder.Services.AddSingleton<IUserStore, UserStore>();
builder.Services.AddSingleton<IReservationStore, ReservationStore>();

// TODO Add controller ? 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
