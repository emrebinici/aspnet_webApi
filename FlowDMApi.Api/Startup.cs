using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Globalization;
using FlowDMApi.Api.Extensions;
using FlowDMApi.Api.Filter;
using FlowDMApi.Api.Handler;
using FlowDMApi.Api.Helper;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FlowDMApi.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(); // Make sure you call this previous to AddMvc
            services.AddControllers(ops =>
            {
                ops.Filters.Add(new TransactionScopeFilter());
                ops.Filters.Add(new TokenAuthorizeFilter());
            }).AddJsonOptions(joptions =>
            {
                joptions.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
            }); services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddHttpContextAccessor();
            services.AddScoped<TransactionScopeFilter>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FlowDMApi.Api", Version = "v1" });
            });
           
            services.AddSwaggerDocumentation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Bu bölüm UseMvc()’ den önce eklenecektir.
            // Uygulamamız içerisinde destek vermemizi istediğimiz dilleri tutan bir liste oluşturuyoruz.
            // SupportedCultures ve SupportedUICultures’a yukarıda oluşturduğumuz dil listesini tanımlıyoruz.
            // DefaultRequestCulture’a varsayılan olarak uygulamamızın hangi dil ile çalışması gerektiğini tanımlıyoruz.
            var cultureInfo = new CultureInfo("tr-TR");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FlowDMApi.Api v1"));
            }
            app.UseExceptionHandler("/token/handleerrors");
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors();
            app.UseCorsMiddleware();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwaggerDocumentation();
        }
    }
}
