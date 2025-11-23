using Microsoft.EntityFrameworkCore;
using MinimalAPI.Data;
using MinimalAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("api/coupon", (AppDbContext db) => Results.Ok(db.Coupons.ToList()));
app.MapGet("api/coupon/{id}", (int id, AppDbContext db) =>
{
    var coupon = db.Coupons.Find(id);
    return coupon != null ? Results.Ok(coupon) : Results.NotFound();
});


app.UseHttpsRedirection();
app.Run();

