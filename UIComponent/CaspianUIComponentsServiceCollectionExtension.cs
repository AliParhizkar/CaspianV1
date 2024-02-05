using Caspian.UI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponent
{
    public static class CaspianUIComponentsServiceCollectionExtension
    {
        /// <summary>
        /// This method registers all services which exists in <b>UIComponent project</b> into <see cref="IServiceCollection"/> 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCaspianUIComponentsServices(this IServiceCollection services)
        {
            services.AddTransient<FileUploadService>();
            services.AddTransient<CascadeService>();
            services.AddScoped<BatchService>();
            services.AddScoped<BasePageService>();

            services.AddSingleton<FormAppState>();

            return services;
        }
    }
}