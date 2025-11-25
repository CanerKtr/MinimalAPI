using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalAPI;
using MinimalAPI.Data;
using MinimalAPI.Dto;
using MinimalAPI.Models;
using MinimalAPI.Responses;

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

app.MapGet("api/coupon", async (HttpRequest req,AppDbContext db) =>
{
    logger.LogInformation("Getting coupons with filter");
    var query = db.Coupons.AsQueryable();

    // Tüm query string parametreleri oku
    foreach (var (key, value) in req.Query)
    {
        if (key.Equals("id", StringComparison.OrdinalIgnoreCase) && int.TryParse(value, out int id))
            query = query.Where(c => c.Id == id);

        if (key.Equals("name", StringComparison.OrdinalIgnoreCase))
            query = query.Where(c => c.Name.Contains(value, StringComparison.OrdinalIgnoreCase));

        if (key.Equals("percent", StringComparison.OrdinalIgnoreCase) && int.TryParse(value, out int percent))
            query = query.Where(c => c.Percent == percent);

        if (key.Equals("isActive", StringComparison.OrdinalIgnoreCase) && bool.TryParse(value, out bool isActive))
            query = query.Where(c => c.IsActive == isActive);
    }

    var result = await query.ToListAsync();
    return Results.Ok(ApiResponse<List<Coupon>>.Ok(result));
}).WithName("GetCouponsWithFilter")
.WithTags("Coupon")
.WithSummary("Get coupons with optional filters")
.WithDescription("Filter coupons by Id, Name, Percent or IsActive using query parameters")
.Produces<ApiResponse<List<Coupon>>>(StatusCodes.Status200OK);





app.MapGet("api/coupon/{id:int}", async (int id, AppDbContext db) =>
{
    logger.LogInformation($"Getting coupon with id {id}");
    var cupon = await db.Coupons.FindAsync(id);
    return cupon is not null
        ? Results.Ok(ApiResponse<Coupon>.Ok(cupon))
        : Results.NotFound(ApiResponse<Coupon>.Fail($"{id} not found"));
}).WithName("GetCouponById")
.WithTags("Coupon")
.WithSummary("Gets a coupon by Id")
.WithDescription("Returns a single coupon if found, otherwise returns 404")
.Produces<ApiResponse<Coupon>>(StatusCodes.Status200OK)
.Produces<ApiResponse<Coupon>>(StatusCodes.Status404NotFound);





app.MapPost("api/coupon", async (IMapper _mapper, 
    IValidator <CouponDtoWithoutId> _validation,
    [FromBody] CouponDtoWithoutId createdCupon, 
    AppDbContext db) =>
{
    var validationResult = await _validation.ValidateAsync(createdCupon);
    logger.LogInformation("Creating a new coupon"); 
    if (!validationResult.IsValid)
        return Results.BadRequest(ApiResponse<Coupon>.Fail(validationResult.Errors.FirstOrDefault()?.ToString()));

    var cupon = _mapper.Map<Coupon>(createdCupon);
    cupon.CreatedAt = DateTime.Now;
    db.Coupons.Add(cupon);
    await db.SaveChangesAsync();
    return Results.Created($"api/coupon/{cupon.Id}", ApiResponse<Coupon>.Ok(cupon));
}).WithName("CreateCoupon")
.WithTags("Coupon")
.WithSummary("Creates a new coupon")
.WithDescription("Creates a new coupon and returns the created coupon")
.Produces<ApiResponse<Coupon>>(StatusCodes.Status201Created)
.Produces<ApiResponse<Coupon>>(StatusCodes.Status400BadRequest);





app.MapPut("api/coupon", async (IMapper _mapper, 
    IValidator<CouponDto> _validation, 
    [FromBody] CouponDto updatedCupon, 
    AppDbContext db) =>
{
    var validationResult = await  _validation.ValidateAsync(updatedCupon);
    logger.LogInformation($"Updating coupon with id {updatedCupon.Id}");
    if (!validationResult.IsValid)
        return Results.BadRequest(ApiResponse<Coupon>.Fail(validationResult.Errors.FirstOrDefault()?.ToString()));
    
    var cupon = await db.Coupons.FindAsync(updatedCupon.Id);
    if (cupon == null)
        return Results.NotFound(ApiResponse<Coupon>.Fail($"Coupon with ID {updatedCupon.Id} not found."));
    
    _mapper.Map(updatedCupon, cupon);
    cupon.UpdatedAt = DateTime.Now;
    await db.SaveChangesAsync();
    return Results.Ok(ApiResponse<Coupon>.Ok(cupon));
}).WithName("UpdateCoupon")
.WithTags("Coupon")
.WithSummary("Updates a coupon")
.WithDescription("Updates the coupon with given Id and returns the updated coupon")
.Produces<ApiResponse<Coupon>>(StatusCodes.Status200OK)
.Produces<ApiResponse<Coupon>>(StatusCodes.Status404NotFound)
.Produces<ApiResponse<Coupon>>(StatusCodes.Status400BadRequest);






app.MapDelete("api/coupon/{id:int}", async (int id, AppDbContext db) =>
{
    logger.LogInformation($"Deleting coupon with id {id}");
    var cupon = await db.Coupons.FindAsync(id);
    if (cupon == null)
        return Results.NotFound(ApiResponse<Coupon>.Fail($"{id} not found"));
    
    db.Coupons.Remove(cupon);
    await db.SaveChangesAsync();
    return Results.Ok(ApiResponse<Coupon>.Ok(cupon));
}).WithName("DeleteCoupon")
.WithTags("Coupon")
.WithSummary("Deletes a coupon")
.WithDescription("Deletes the coupon with given Id and returns the deleted coupon")
.Produces<ApiResponse<Coupon>>(StatusCodes.Status200OK)
.Produces<ApiResponse<Coupon>>(StatusCodes.Status404NotFound);



app.UseHttpsRedirection();
app.Run();

