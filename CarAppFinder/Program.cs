using CarAppFinder.Models;
using CarAppFinder.Services;
using CarAppFinder.Services.Bug;
using CarAppFinder.Services.Setting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Setting Setting = new();
builder.Configuration.Bind(nameof(Setting), Setting);

AddServices(builder.Services);

builder.Services.AddControllers(config =>
{
    config.Filters.Add<HttpResponseExceptionFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

var connectionString = new ConnectionString();
builder.Configuration.Bind(nameof(connectionString), connectionString);

builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(Setting.ConnectionString));

builder.Services.AddIdentity<User, IdentityRole>(opts =>
{
    opts.User.RequireUniqueEmail = Setting.IsProduction;
    opts.Password.RequiredLength = 4;
    opts.Password.RequireNonAlphanumeric = Setting.IsProduction;
    opts.Password.RequireLowercase = Setting.IsProduction;
    opts.Password.RequireUppercase = Setting.IsProduction;
    opts.Password.RequireDigit = Setting.IsProduction;
    opts.Password.RequireDigit = Setting.IsProduction;
}).AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();

AddAuthentication(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
   
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void AddServices(IServiceCollection services)
{
    services.AddSwaggerGen(c => {
        c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
        c.IgnoreObsoleteActions();
        c.IgnoreObsoleteProperties();
        c.CustomSchemaIds(type => type.FullName);
    });
    services.AddSingleton(Setting);
    services.AddScoped<IUserService, UserService>();
    services.AddTransient<IErrorLogService, ErrorLogService>();
    services.AddTransient<ICarService, CarService>();
}

void AddAuthentication(IServiceCollection services)
{
    var tokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        //ValidateActor=true,
        ValidIssuer = Setting.JwtSetting.Issuer,
        ValidAudience = Setting.JwtSetting.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Setting.JwtSetting.IssuerSigningKey))
    };
    //services.AddSingleton(tokenValidationParameters);

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

    }).AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = tokenValidationParameters;
    });
}
