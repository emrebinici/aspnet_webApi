using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace FlowDMApi.Api.Helper
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                var contact = new OpenApiContact()
                {
                    Name = "Prestij Yazılım",
                    Email = "destek@prestijsoftware.com",
                    Url = new Uri("http://www.prestijsoftware.com/tr/")
                };
                var license = new OpenApiLicense()
                {
                    Name = "My License",
                    Url = new Uri("http://www.prestijsoftware.com/tr/")
                };
                var info = new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "Yoğun Bakım API",
                    Description = "Yoğun Bakım projesi için HBYS entegrasyonu.",
                    TermsOfService = new Uri("http://www.prestijsoftware.com/tr/"),
                    Contact = contact,
                    License = license
                };
                var info2 = new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "Mobil API",
                    Description = "Mobil projesi için HBYS entegrasyonu.",
                    TermsOfService = new Uri("http://www.prestijsoftware.com/tr/"),
                    Contact = contact,
                    License = license
                };
                c.SwaggerDoc("yogunbakim", info);
                c.SwaggerDoc("mobil", info2);
                c.AddSecurityDefinition("Authorization", new OpenApiSecurityScheme
                {
                    Description = @"Authorization Header Token Example: '12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Authorization"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Authorization"
                            },
                            Scheme = "oauth2",
                            Name = "Authorization",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Prestij Common API dökümanı";
                c.SwaggerEndpoint("/swagger/yogunbakim/swagger.json", "Yoğun Bakım API v1");
                c.SwaggerEndpoint("/swagger/mobil/swagger.json", "Mobil API v1");
                c.RoutePrefix = "help";
            });

            return app;
        }
    }
}
