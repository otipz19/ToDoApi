using Microsoft.EntityFrameworkCore;
using WebAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<TodoContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("localhost"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
