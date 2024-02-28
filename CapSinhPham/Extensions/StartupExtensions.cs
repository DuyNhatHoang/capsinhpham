
using CorePush.Apple;
using CorePush.Google;
using Data.DataAccess;
using Data.ViewModels;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Minio;
using MongoDB.Driver;
using Services.Core;
using System.Configuration;

namespace CapSinhPham.Extensions
{
    public static class StartupExtensions
    {
        public static void AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IOrderItemService, OrderItemService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();

            //// for masstransit
            //services.AddScoped<IPersonProducer, PersonProducer>();

        }

        public static void ConfigCors(this IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("MyAllowSpecificOrigins",
                      builder =>
                      {
                          builder.WithOrigins("https://localhost:3000",
                              "https://myawesomesite")
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                      }));
        }

        public static void ConfigMongoDb(this IServiceCollection services, string connectionString, string databaseName)
        {
            services.AddSingleton<IMongoClient>(s => new MongoClient(connectionString));
            services.AddScoped(s => new AppDbContext(s.GetRequiredService<IMongoClient>(), databaseName));
        }

        public static void ConfigMinIO(this IServiceCollection services, string endpoint, string accessKey, string secretKey)
        {
            //MinioClient minio = new MinioClient()
            //                            .WithEndpoint(endpoint)
            //                            .WithCredentials(accessKey, secretKey)
            //                            .WithSSL(false)
            //                            .Build();

            //services.AddSingleton<MinioClient>(s => minio);
        }


        public static void AddFCM(this IServiceCollection services, Microsoft.Extensions.Configuration.ConfigurationSection configurationSection)
        {
            services.AddTransient<IFCMService, FCMService>();
            services.AddHttpClient<FcmSender>();
            services.AddHttpClient<ApnSender>();

            // Configure strongly typed settings objects
            services.Configure<FcmNotificationSetting>(configurationSection);
        }

        public static void ConfigJwt(this IServiceCollection services, string key, string issuer, string audience)
        {
            //services.AddAuthentication(x =>
            //    {
            //        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    })
            //    .AddJwtBearer(jwtconfig =>
            //    {
            //        jwtconfig.SaveToken = true;
            //        jwtconfig.TokenValidationParameters = new TokenValidationParameters()
            //        {
            //            ValidateIssuer = true,
            //            ValidateAudience = true,
            //            ValidateLifetime = true,
            //            ValidateIssuerSigningKey = false,
            //            RequireSignedTokens = true,
            //            ValidIssuer = issuer,
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            //            ValidAudience = string.IsNullOrEmpty(audience) ? issuer : audience,
            //        };

            //    });
        }
    }
}
