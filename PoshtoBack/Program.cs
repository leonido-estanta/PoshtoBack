using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PoshtoBack.Data;
using PoshtoBack.Hubs;
using PoshtoBack.Services;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddScoped<SeedService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IJwtService, JwtService>();

var hubTypePathMappings = new Dictionary<Type, string>
{
    { typeof(ChatHub), "/chatHub" },
    { typeof(VoiceHub), "/voiceHub" },
    { typeof(UserHub), "/userHub" }
};

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey("b6e9c882-4278-4eb5-8753-c0e8e64de461".ToCharArray().Select(c => (byte)c).ToArray()),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && hubTypePathMappings.Any(p => path.StartsWithSegments(p.Value)))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNgrok",
        corsPolicyBuilder => corsPolicyBuilder
            .SetIsOriginAllowed(origin => new Uri(origin).Host.EndsWith("ngrok-free.app"))
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddDbContext<PoshtoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PoshtoConnectionString")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
// app.UseHttpsRedirection(); // Comment this line out if testing without HTTPS
app.UseCors("AllowNgrok");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

foreach (var mapping in hubTypePathMappings)
{
    var method = typeof(HubEndpointRouteBuilderExtensions)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .FirstOrDefault(m => m.Name == "MapHub" && m.GetParameters().Length == 2);

    var genericMethod = method?.MakeGenericMethod(mapping.Key);
    genericMethod?.Invoke(null, [app, mapping.Value]);
}

app.Run();
