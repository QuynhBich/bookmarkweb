using System.Text;
using BookmarkWeb.API.Commons;
using BookmarkWeb.API.Database;
using BookmarkWeb.API.Hubs;
using BookmarkWeb.API.Models;
using BookmarkWeb.API.Models.Bookmarks;
using BookmarkWeb.API.Models.Chats;
using BookmarkWeb.API.Models.Common.Schemas;
using BookmarkWeb.API.Models.Folders;
using BookmarkWeb.API.Models.Login;
using BookmarkWeb.API.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptions();
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
    hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(Constants.API_EXPIRES_MINUTE);
    hubOptions.ClientTimeoutInterval = TimeSpan.FromMinutes(Constants.API_EXPIRES_MINUTE);
    hubOptions.MaximumReceiveMessageSize = Int16.MaxValue;
    hubOptions.StreamBufferCapacity = Int16.MaxValue;
    hubOptions.MaximumParallelInvocationsPerClient = 3;
});
var services = builder.Services;
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
// services.AddCors(options =>
// {
//     options.AddPolicy(name: "BookmarkWeb",
//         builder =>
//         {
//             builder.WithOrigins("http://localhost:3000")
//             .AllowAnyHeader()
//             .AllowAnyMethod()
//             .AllowCredentials();
//         });
// });
services.AddCors(options =>
{
    var allowHostsConfig = builder.Configuration["AllowedHosts"] ?? "";
    options.AddPolicy("CorsPolicy",
        builder =>
        {
        if (allowHostsConfig.Equals("*"))
        {
            builder.SetIsOriginAllowed((origin) => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
        else
        {
            var allowHosts = allowHostsConfig.Split(";");
            builder.WithOrigins(allowHosts)
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
        });
});
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"]!))
    };
});
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API",
        Version = "v1"
    });
    // To Enable authorization using Swagger (JWT)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});
builder.Services.AddHttpClient();

var connectionString = builder.Configuration.GetConnectionString("Default");
var serverVersion = new MySqlServerVersion(new Version(8, 0, 34));
services.AddDbContext<DataContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(connectionString, serverVersion)
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);
services.AddScoped<IBookmarkModel, BookmarksModel>();
services.AddScoped<IFolderModel, FoldersModel>();
services.AddScoped<ILoginModel, LoginModel>();
services.AddScoped<IChatsModel, ChatsModel>();
services.AddScoped<IAppStateModel, AppStateModel>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IIdentityService, IdentityService>();
builder.Services.AddTransient<IApiService, ApiService>();
builder.Services.AddTransient<IChatService, ChatService>();

var app = builder.Build();
AppState.Instance.Configuration = app.Configuration;
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors(Constants.CorsPolicy);
app.MapControllers();
app.MapHub<NotificationHub>("/hub/notification");
app.Run();
