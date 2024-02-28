
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NSwag;
using NSwag.Generation.Processors.Security;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Http.Features;
using CapSinhPham.Extensions;
using Data.DataAccess;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("firebase.json"),
});

builder.Services.ConfigMongoDb(builder.Configuration["AppDatabaseSettings:ConnectionString"], builder.Configuration["AppDatabaseSettings:DatabaseName"]);

builder.Services.AddControllers();


builder.Services.AddCors();

//add FCM
builder.Services.AddFCM((global::Microsoft.Extensions.Configuration.ConfigurationSection)builder.Configuration.GetSection("FcmNotification"));
//builder.Services.ConfigCors();


builder.Services.AddOpenApiDocument(document =>
{
    document.Title = "Capsinhpham";
    document.Version = "1.0.1";
    document.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Type into the textbox: Bearer {your JWT token}."
    });

    document.OperationProcessors.Add(
        new AspNetCoreOperationSecurityScopeProcessor("JWT"));

});

builder.Services.ConfigMinIO(
    builder.Configuration["MinIO:Endpoint"],
    builder.Configuration["MinIO:AccessKey"],
    builder.Configuration["MinIO:SecretKey"]);

builder.Services.AddBusinessServices();

builder.Services.AddStackExchangeRedisCache(options =>
{
   options.Configuration = builder.Configuration["Redis"];
    //options.Configuration = "localhost:6379";

    options.InstanceName = "my-release-redis-master";
});

//add session
builder.Services.AddDistributedMemoryCache();

//add FCM
builder.Services.AddFCM((global::Microsoft.Extensions.Configuration.ConfigurationSection)builder.Configuration.GetSection("FcmNotification"));

//add masstransit
//var connection = new MassTransitConnection();
//builder.Configuration.Bind("MassTranfit", connection);
//builder.Services.AddSingleton(connection);
//builder.Services.AddMassTransit(x =>
//{
//    x.AddConsumer<MemberConsumer>();
//    x.AddConsumer<OrganizationConsumer>();
//    x.AddConsumer<PersonConsumer>();
//    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
//    {
//        cfg.Host(new Uri(builder.Configuration["MassTranfit:Host"]), h =>
//        {
//            h.Username(builder.Configuration["MassTranfit:Username"]);
//            h.Password(builder.Configuration["MassTranfit:Password"]);
//        });
//        cfg.ReceiveEndpoint("organization-sync", ep =>
//        {
//            ep.PrefetchCount = 16;
//            ep.UseMessageRetry(r => r.Interval(2, 100));
//            ep.ConfigureConsumer<OrganizationConsumer>(provider);
//        });
//        cfg.ReceiveEndpoint("member-sync", ep =>
//        {
//            ep.PrefetchCount = 16;
//            ep.UseMessageRetry(r => r.Interval(2, 100));
//            ep.ConfigureConsumer<MemberConsumer>(provider);
//        });
//        cfg.ReceiveEndpoint("person-sync", ep =>
//        {
//            ep.PrefetchCount = 16;
//            ep.UseMessageRetry(r => r.Interval(2, 100));
//            ep.ConfigureConsumer<PersonConsumer>(provider);
//        });
//    }));
//});



// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});


// Register the Swagger services


var app = builder.Build();

// Configure the HTTP request pipeline.


// Create collections in Database
var appDbContext = builder.Services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();

if (appDbContext != null)
{
    appDbContext.CreateCollectionsIfNotExists();
}
app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi3();

app.Run();


