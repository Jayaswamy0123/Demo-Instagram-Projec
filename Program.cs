using Instagram.Entities;
using Instagram.Repository.Iservices;
using Instagram.Repository.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<InstagramContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), ServerVersion.Parse("8.0.28"))
           .EnableSensitiveDataLogging()
           .EnableDetailedErrors();

});
builder.Services.AddHttpClient();  //This adds the HttpClient service to the dependency injection container. It allows the application to make HTTP requests to other services or APIs.
builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.ContractResolver = new DefaultContractResolver();
    o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    o.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
    o.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ";
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("Instagram",
    new OpenApiInfo
    {
        Title = "Instagram",
        Version = "1.1",
        Description = "Instagram using ASP.NET CORE 7",
        Contact = new OpenApiContact
        {
            Name = "Jayaswamy N",
            Email = "njayaswamy0123@gmail.com",
        },
    });
    c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
    });
    c.OperationFilter<AuthOperationFilter>();
});

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration.GetValue<string>("JwtOptions:Issuer"),
            ValidAudience = builder.Configuration.GetValue<string>("JwtOptions:Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtOptions:SecurityKey")))
        };
    });
builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
{
    builder
        .AllowAnyMethod()
        .AllowAnyHeader()
          .WithOrigins(
                      "http://localhost:4200",
                       "https://localhost:4200",
                       "http://localhost:4400",
                       "https://localhost:4400",
                       "http://developer.promena.in",
                       "http://3.237.193.202",
                       "https://3.237.193.202",
                       "http://100.24.114.194",
                       "https://100.24.114.194",
                       "http://bosi.beyondacademics.com",
                       "https://bosi.beyondacademics.com",
                       "https://development.fastark.in",
                       "http://development.fastark.in")
          .SetIsOriginAllowedToAllowWildcardSubdomains()
        .AllowCredentials();
}));

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IAccountServices, AccountServices>();
builder.Services.AddTransient<IEmailServices, EmailServices>();
builder.Services.AddTransient<IUserServices, UserServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStatusCodePages("text/plain", "Status code page, status code: {0}");

app.UseHttpsRedirection();

app.UseCookiePolicy();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.UseDefaultFiles();

app.UseHsts();

app.UseSwagger();

app.UseSwaggerUI(c =>
{

    c.SwaggerEndpoint(".././swagger/Instagram/swagger.json", "Instagram");
    c.DocExpansion(DocExpansion.None);
    c.DefaultModelsExpandDepth(-1);
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

