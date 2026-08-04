using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IOrderStatusHistoryRepository, OrderStatusHistoryRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IOrderStatusHistoryService, OrderStatusHistoryService>();

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.Use(async(Context, Next) =>
{
    Console.WriteLine($"Start Endpoint : {Context.Request.Method} {Context.Request.Path}");
    await Next();
    Console.WriteLine("Finish Endpoint");
});
app.Use(async(Context, Next) =>
{
    var sw = System.Diagnostics.Stopwatch.StartNew();
    await Next();
    sw.Stop();
    Console.WriteLine($"Request completed in {sw.ElapsedMilliseconds} ms");
});
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
