﻿
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QRA.JWT.contracts;
using QRA.JWT.jwt;
using QRA.Persistence;
using QRA.UseCases.commands;
using QRA.UseCases.Commands;
using QRA.UseCases.contracts;
using QRA.UseCases.Helpers;
using QRA.UseCases.Mapper;
using QRA.UseCases.Queries;
using System.Text;

namespace QRA.API
{

    public class Startup
    {

        public readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
       
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

      

        public void ConfigureServices(IServiceCollection services)
        {
            //Database Configuration
            string QRAConnectionString = Configuration.GetConnectionString(name: "QRAChallenge");
            int timeout = Configuration.GetValue<int>("Timeout");

            services.AddDbContext<QRAchallengeContext>(options =>
                options.UseSqlServer(QRAConnectionString, sql => sql.CommandTimeout(timeout)).UseLazyLoadingProxies());

            //cors --url allows to access
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      //  builder.AllowCredentials().WithOrigins("http://localhost:4200");
                                      builder.AllowAnyOrigin();
                                      builder.AllowAnyHeader();
                                      builder.AllowAnyMethod();
                                  });
            });

            //configure swagger documentation
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "SingleSignOn.API", Version = "v1" }));

           

            //allow DI
            services.AddControllers();
             services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<ITenantsLoginService, TenantsLoginService>();
            services.AddScoped<IToken,Token>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IModelValidation, UseCases.commands.ModelValidation>();

            //add queries 
            services.AddScoped<GetTenantQueries>();

            //Jwt authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options=>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["JWT:Audience"],
                    ValidIssuer = Configuration["JWT:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]))
                };
            });

            //add auto mapper
            services.AddAutoMapper(typeof(RegisterProfile).Assembly);

            services.AddHealthChecks();
            services.AddLogging();
            services.AddMvc();
        }

      
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SingleSignOn.Api v1");
                    c.RoutePrefix = string.Empty;
                });
            }
            else
            {
                app.UseHsts();
            }
            app.UseAuthentication();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
