using Microsoft.EntityFrameworkCore;
using PharmaChain.Application.Interfaces;
using PharmaChain.Infrastructure.Models;
using PharmaChain.Infrastructure.BackendServices;
using PharmaChain.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt")
);
var key = Encoding.ASCII.GetBytes(jwt["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
 .AddJwtBearer(options =>
 {
     options.Events = new JwtBearerEvents
     {
         OnMessageReceived = context =>
         {
             context.Token = context.Request.Cookies["jwtToken"];
             return Task.CompletedTask;
         },

         OnChallenge = context =>
         {
             context.HandleResponse(); 

             context.Response.StatusCode = 302;
             context.Response.Headers["Location"] = "/";

             return Task.CompletedTask;
         }
     };

     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwt["Issuer"],
         ValidAudience = jwt["Audience"],
         IssuerSigningKey = new SymmetricSecurityKey(key),
         ClockSkew = TimeSpan.Zero
     };
 });
builder.Services.AddAuthorization();

// MVC + API
builder.Services.AddControllersWithViews();

// Database
builder.Services.AddDbContext<PharmaChainDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<IPharmaChainDbContext, PharmaChainDbContext>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IOtpService, OtpService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IMedicineService, MedicineService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IBatchService, BatchService>();
builder.Services.AddScoped<IPurchaseStockService, PurchaseStockService>();
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();