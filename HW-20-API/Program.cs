using HW_20.Domain.Contract.AppService;
using HW_20.Domain.Contract.Repositoris;
using HW_20.Domain.Contract.Service;
using HW_20.Domain.Contract.Sevice;
using HW_20.Domain.Models;
using HW_20.Infrastructure.DB;
using HW_20.Infrastructure.Repositoris;
using HW_20.Service.AppService;
using HW_20.Service.Service;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;  // برای OpenAPI و Swagger






var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// بارگذاری تنظیمات از فایل پیکربندی
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// بارگذاری تنظیمات از appsettings.json
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Add services to the container.
builder.Services.AddControllers();
//builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IInspectionRequestAppService, InspectionRequestAppService>();
builder.Services.AddScoped<IInspectionRequestService, InspectionRequestService>();
builder.Services.AddScoped<IInspectionRequestRepository, InspectionRequestRepository>();
builder.Services.AddScoped<IAuthenticationAppService, AuthenticationAppService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddScoped<ICarModelSevice, CarModelSevice>();
builder.Services.AddScoped<ICarModelAppSevice, CarModelAppSevice>();
builder.Services.AddScoped<ICarModelRepository, CarModelRepository>();
builder.Services.AddControllersWithViews().AddDataAnnotationsLocalization();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}



// اضافه کردن Middleware برای احراز هویت با API Key
//app.UseMiddleware<ApiKeyMiddleware>();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
