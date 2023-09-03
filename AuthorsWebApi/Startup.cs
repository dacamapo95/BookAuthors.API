using AuthorsWebApi.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;

namespace AuthorsWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            // Evitar ocultamiento de información de claims de JWT
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(jsonOptions => //Ignora ciclos entre relaciones de objetos persistentes.
                                jsonOptions.JsonSerializerOptions
                                           .ReferenceHandler = ReferenceHandler.IgnoreCycles)
                .AddNewtonsoftJson();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            // Inyección dbcontext sqlServer
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options => options.TokenValidationParameters =
                                                // Cfg firma del token
                                                new TokenValidationParameters
                                                {
                                                    ValidateIssuer = false,
                                                    ValidateAudience = false,
                                                    ValidateLifetime = true,
                                                    ValidateIssuerSigningKey = true,
                                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["keyJwt"]))
                                                });
            services.AddResponseCaching();
            // Add services to the container.
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                //se utiliza para agregar definiciones de seguridad a la documentación de Swagger.
                options.AddSecurityDefinition(
                    "Bearer",
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header
                    });

                // se utiliza para especificar que los endpoints requieren la autenticación definida previamente.
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


            //Cfg automapper
            services.AddAutoMapper(typeof(Startup));

            //Configurar Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.AddAuthorization(options =>
            {
                // Añadir Autorización basada en claims
                options.AddPolicy("IsAdmin", policy => policy.RequireClaim("IsAdmin"));
            });

            // Permitir hacer peticiones desde un navegador.
            //services.AddCors(options =>
            //{
            //    options.AddDefaultPolicy(builder =>
            //    {
            //        builder.WithOrigins("https://apirequest.io").AllowAnyMethod().AllowAnyHeader();
            //    });
            //});

            // Inyectar encriptador
            services.AddDataProtection();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Redirige peticiones http a https
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();  
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching(); //Utilizar cache

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
