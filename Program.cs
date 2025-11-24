using Microsoft.EntityFrameworkCore;
using MinimalAPI.Data;
using MinimalAPI.Models;
using MinimalAPI.Dto;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI;
using AutoMapper;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
var app = builder.Build();
var logger = app.Logger;
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("api/coupon", (AppDbContext db) =>
{
    logger.LogInformation("Getting all coupons");
    return Results.Ok(db.Coupons.ToList());
}
);
app.MapGet("api/coupon/{id:int}", (int id, AppDbContext db) =>
{
    logger.LogInformation($"Getting coupon with id {id}");
    var cupon = db.Coupons.FirstOrDefault(c => c.Id == id);
    return cupon is not null ? Results.Ok(cupon) : Results.NotFound($"{id} not found");
});
app.MapPost("api/coupon", async (IMapper _mapper, IValidator <CouponDtoWithoutId> _validation
    , [FromBody] CouponDtoWithoutId createdCupon, AppDbContext db) =>
{
    var validationResult = await _validation.ValidateAsync(createdCupon);
    logger.LogInformation("Creating a new coupon"); 
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors.FirstOrDefault().ToString());

    }
    var cupon = _mapper.Map<Coupon>(createdCupon);
    cupon.CreatedAt = DateTime.Now;
    db.Coupons.Add(cupon);
    db.SaveChanges();
    return Results.Created($"api/coupon/{cupon.Id}", cupon);
});

app.MapPut("api/coupon", async (IMapper _mapper, IValidator<CouponDto> _validation, [FromBody] CouponDto updatedCupon, AppDbContext db) =>
{
    var validationResult = await  _validation.ValidateAsync(updatedCupon);
    logger.LogInformation($"Updating coupon with id {updatedCupon.Id}");
    if (!validationResult.IsValid)
    {
        return Results.NotFound(validationResult.Errors.FirstOrDefault().ToString());
    }
    var cupon = await db.Coupons.FindAsync(updatedCupon.Id);
    if (cupon == null)
    {
        return Results.NotFound($"Coupon with ID {updatedCupon.Id} not found.");
    }
    _mapper.Map(updatedCupon, cupon);
    cupon.UpdatedAt = DateTime.Now;
    await db.SaveChangesAsync();
    return Results.Ok(cupon);
});

app.MapDelete("api/coupon/{id:int}", (int id, AppDbContext db) =>
{
    logger.LogInformation($"Deleting coupon with id {id}");
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

