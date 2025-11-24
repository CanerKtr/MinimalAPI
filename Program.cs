using Microsoft.EntityFrameworkCore;
using MinimalAPI.Data;
using MinimalAPI.Models;
using MinimalAPI.Dto;
using Microsoft.AspNetCore.Mvc;

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
app.MapGet("api/coupon/{id:int}", (int id, AppDbContext db) =>
{
    var cupon = db.Coupons.FirstOrDefault(c => c.Id == id);
    return cupon != null ? Results.Ok(cupon) :Results.NotFound($"{id} not found");
});
app.MapPost("api/coupon",([FromBody]CouponDto createdCupon, AppDbContext db ) =>
{
    if (createdCupon.Percent < 0 || createdCupon.Percent > 100)
    {
        return Results.BadRequest("Percent must be between 0 and 100");
        
    }
    var cupon = new Coupon
    {
        Name = createdCupon.Name,
        Percent = createdCupon.Percent, 
        IsActive = createdCupon.IsActive,
        CreatedAt = DateTime.Now,
        UpdatedAt = null
    };
    db.Coupons.Add(cupon);
    db.SaveChanges();
    return Results.Ok(cupon);
 });

app.MapPut("api/coupon",([FromBody] CouponDto updatedCupon, AppDbContext db) =>
{
    var cupon = db.Coupons.FirstOrDefault(c => c.Id == updatedCupon.Id);
    if (cupon == null)
    {
        return Results.NotFound($"{updatedCupon.Id} not found");
    }
    if (updatedCupon.Percent < 0 || updatedCupon.Percent > 100)
    {
        return Results.BadRequest("Percent must be between 0 and 100");
    }
    cupon.Name = updatedCupon.Name != null ? updatedCupon.Name : cupon.Name;
    cupon.Percent = updatedCupon.Percent != 0 ? updatedCupon.Percent : cupon.Percent;
    cupon.IsActive = updatedCupon.IsActive == true ? true : false;
    cupon.UpdatedAt = DateTime.Now;
    db.SaveChanges();
    return Results.Ok(cupon);
});

app.MapDelete("api/coupon/{id:int}",(int id, AppDbContext db) =>
{
    var cupon = db.Coupons.FirstOrDefault(c => c.Id == id);
    if (cupon == null)
    {
        return Results.NotFound($"{id} not found");
    }
    db.Coupons.Remove(cupon);
    db.SaveChanges();
    return Results.Ok();
});
    
app.UseHttpsRedirection();
app.Run();

