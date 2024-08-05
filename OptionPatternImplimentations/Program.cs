using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OptionPatternImplimentations.Data;
using OptionPatternImplimentations.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using OptionPatternImplimentations.Configuration;
using System.Configuration;
using FluentAssertions.Common;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.

builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddTransient<UserManager<AppUser>>();
builder.Services.AddTransient<SignInManager<AppUser>>();
//builder.Services.AddScoped<TokenService>();


//Get Configuration
builder.Services.Configure<Jwt>(builder.Configuration.GetSection("jwt"));
builder.Services.AddSingleton<IOptionsMonitor<Jwt>, OptionsMonitor<Jwt>>();

//Removed CookieAuthenticationDefaults.AuthenticationScheme from Add authentication method parameter
builder.Services.AddAuthentication(options=>
    {
        options.DefaultScheme = "MultiAuthSchemes";
        options.DefaultChallengeScheme = "MultiAuthSchemes";
    }
).AddCookie().
    AddJwtBearer( options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:jwtKey"])),
            ClockSkew = TimeSpan.Zero
        };
    }).
    AddJwtBearer("SecondJwtScheme", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bC9Shjao0rFLI9xZ+TX/WenMBCkY/N+v23uOA445Z4I=")),
            ClockSkew = TimeSpan.Zero
        };
    }).AddPolicyScheme("MultiAuthSchemes", JwtBearerDefaults.AuthenticationScheme, option =>
    {
        option.ForwardDefaultSelector = context =>
        {
            string authorization = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
            {
                var token = authorization.Substring("Bearer ".Length).Trim();
                var jwtHandler = new JwtSecurityTokenHandler();
                return (jwtHandler.CanReadToken(token) && jwtHandler.ReadJwtToken(token).Issuer.Equals("https://localhost:7018/"))
                    ? JwtBearerDefaults.AuthenticationScheme : "SecondJwtScheme";
            }
            return CookieAuthenticationDefaults.AuthenticationScheme;
        };
    });

builder.Services.AddAuthorization(options =>
{

    options.AddPolicy("OnlyAdmin", policy => policy.RequireRole("Admin"));
   
});

builder.Services.AddAuthorization(options =>
{



    var defaultAuthorizatinPolicy = new AuthorizationPolicyBuilder(
        CookieAuthenticationDefaults.AuthenticationScheme,
        JwtBearerDefaults.AuthenticationScheme, "SecondJwtScheme");





    options.DefaultPolicy = defaultAuthorizatinPolicy.RequireAuthenticatedUser().Build();

   

 


    var onlySecondJwtSchemePolicyBuilder = new AuthorizationPolicyBuilder("SecondJwtScheme");
    options.AddPolicy("OnlySecondJwtScheme", onlySecondJwtSchemePolicyBuilder
        .RequireAuthenticatedUser()
        .Build());

    var onlyCookieSchemePolicyBuilder = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme);
    options.AddPolicy("OnlyCookieScheme", onlyCookieSchemePolicyBuilder
        .RequireAuthenticatedUser()
        .Build());

});



//builder.Services
//  .Configure<AuthenticationOptions>(options => {
//      options.DefaultScheme =CookieAuthenticationDefaults.AuthenticationScheme ;
//      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//  });

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

//builder.Services.AddAuthentication(option =>
//{
//    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//           .AddJwtBearer(options =>
//           {
//               options.RequireHttpsMetadata = false;
//               options.SaveToken = true;
//               options.TokenValidationParameters = new TokenValidationParameters
//               {
//                   ValidateIssuer = true,
//                   ValidateAudience = true,
//                   ValidateLifetime = true,
//                   ValidateIssuerSigningKey = true,
//                   ValidIssuer = builder.Configuration["Jwt:Issuer"],
//                   ValidAudience = builder.Configuration["Jwt:Audience"],
//                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
//                   ClockSkew = TimeSpan.Zero
//               };
//           });






builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});



//Add Swagger Services
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Authentication",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});





builder.Services.AddControllers();
//builder.Services.AddControllersWithViews();




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
