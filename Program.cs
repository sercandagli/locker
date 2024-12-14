using System.Text;
using Kargomat.ExternalServices.CabinetService;
using Locker;
using Locker.ExternalServices;
using Locker.Middlewares;
using locker.Pages;
using Locker.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WkHtmlToPdfDotNet.Contracts;
using WkHtmlToPdfDotNet;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddAuthorization();


var services = builder.Services;

services.AddScoped<IOrderService,OrderService>();
services.AddScoped<INotificationService,NotificationService>();
services.AddScoped<IAuthService, AuthService>();

    services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

services.AddScoped<IBestWondService,BestWondService>();
services.AddScoped<ISmsService,SmsService>();
services.AddScoped<WorkContext>();
services.AddScoped<BasePageModel>();
services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
services.AddSession();
services.AddHttpClient("Cabinet", cfg => {
    var url = builder.Configuration.GetSection("BestWondServiceSettings:BaseUrl").Value;
    cfg.BaseAddress = new Uri(url ?? string.Empty);
});
services.AddTransient<AuthenticationMiddleware>();
services.AddHttpContextAccessor();

var app = builder.Build();
app.UseSession();

app.UseAuthenticationMiddleware();





// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();



