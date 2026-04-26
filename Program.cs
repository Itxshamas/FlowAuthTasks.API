using FlowAuthTasks.API.Data;
using FlowAuthTasks.API.Helpers;
using FlowAuthTasks.API.Mappings;
using FlowAuthTasks.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Linq;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    try
    {
        var openApiAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "Microsoft.OpenApi");

        var schemeType = openApiAssembly?.GetType("Microsoft.OpenApi.Models.OpenApiSecurityScheme")
                         ?? Type.GetType("Microsoft.OpenApi.Models.OpenApiSecurityScheme, Microsoft.OpenApi");

        if (schemeType != null)
        {
            var scheme = Activator.CreateInstance(schemeType);

            schemeType.GetProperty("Description")?.SetValue(scheme, "Enter JWT token. Example: \"Bearer eyJhbGci...\" or just the token.");
            schemeType.GetProperty("Name")?.SetValue(scheme, "Authorization");

            var paramLocationType = openApiAssembly?.GetType("Microsoft.OpenApi.Models.ParameterLocation")
                                    ?? Type.GetType("Microsoft.OpenApi.Models.ParameterLocation, Microsoft.OpenApi");
            var securitySchemeType = openApiAssembly?.GetType("Microsoft.OpenApi.Models.SecuritySchemeType")
                                    ?? Type.GetType("Microsoft.OpenApi.Models.SecuritySchemeType, Microsoft.OpenApi");

            if (paramLocationType != null)
            {
                var headerVal = Enum.Parse(paramLocationType, "Header");
                schemeType.GetProperty("In")?.SetValue(scheme, headerVal);
            }

            if (securitySchemeType != null)
            {
                var httpVal = Enum.Parse(securitySchemeType, "Http");
                schemeType.GetProperty("Type")?.SetValue(scheme, httpVal);
            }

            schemeType.GetProperty("Scheme")?.SetValue(scheme, "bearer");
            schemeType.GetProperty("BearerFormat")?.SetValue(scheme, "JWT");

            var addDef = c.GetType().GetMethod("AddSecurityDefinition", BindingFlags.Instance | BindingFlags.Public);
            addDef?.Invoke(c, new object[] { "Bearer", scheme });

            var reqType = openApiAssembly?.GetType("Microsoft.OpenApi.Models.OpenApiSecurityRequirement")
                         ?? Type.GetType("Microsoft.OpenApi.Models.OpenApiSecurityRequirement, Microsoft.OpenApi");

            if (reqType != null)
            {
                var requirement = Activator.CreateInstance(reqType);

                var addMethod = reqType.GetMethod("Add", new Type[] { schemeType, typeof(IEnumerable<string>) });
                if (addMethod != null)
                {
                    addMethod.Invoke(requirement, new object[] { scheme, Array.Empty<string>() });
                }
                else if (requirement is System.Collections.IDictionary dict)
                {
                    dict.Add(scheme, Array.Empty<string>());
                }

                var addReq = c.GetType().GetMethod("AddSecurityRequirement", BindingFlags.Instance | BindingFlags.Public);
                addReq?.Invoke(c, new object[] { requirement });
            }
        }
    }
    catch
    {
    }
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<JwtHelper>();

// Repositories & Services
builder.Services.AddScoped<FlowAuthTasks.API.Repositories.Interfaces.ITaskRepository, FlowAuthTasks.API.Repositories.Implementations.TaskRepository>();
builder.Services.AddScoped<FlowAuthTasks.API.Services.Interfaces.ITaskService, FlowAuthTasks.API.Services.Implementations.TaskService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var keyString = builder.Configuration["Jwt:Key"];
    if (string.IsNullOrWhiteSpace(keyString))
    {
        throw new InvalidOperationException("JWT key is not configured. Set 'Jwt:Key' in appsettings or user secrets.");
    }

    var key = Encoding.UTF8.GetBytes(keyString);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbInitializer.SeedAsync(services);
}

app.MapControllers();

app.Run();
