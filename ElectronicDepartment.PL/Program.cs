using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ElectronicDepartment.BLL;
using ElectronicDepartment.BLL.Services;
using ElectronicDepartment.BLL.Services.Interfaces;
using ElectronicDepartment.DAL.Context;
using ElectronicDepartment.DAL.UnitOfWork;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;
using ElectronicDepartment.Common.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

builder.Services.AddDbContext<ElectronicDepartmentContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.MigrationsAssembly("ElectronicDepartment.DAL")
    )
);

// Authentication & Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IManagerService, ManagerService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IGroupService, GroupService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Electronic Department API",
        Version = "v1",
        Description = "API для управління електронною кафедрою"
    });

    c.EnableAnnotations();

    // Додаємо підтримку JWT в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Введіть 'Bearer' [space] та ваш токен.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

    // Додаємо підтримку коментарів XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.SchemaFilter<EnumSchemaFilter>();
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Electronic Department API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ініціалізація бази даних
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ElectronicDepartmentContext>();
    context.Database.EnsureCreated();
}

app.Run();

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            Enum.GetNames(context.Type)
                .ToList()
                .ForEach(name => schema.Enum.Add(new OpenApiString($"{Convert.ToInt32(Enum.Parse(context.Type, name))} - {name}")));
        }
    }
}
