using Caspian.UI;
using Caspian.Common;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddScoped<BatchServiceData>();
            services.AddScoped<BasePageService>();

            services.AddScoped<FormAppState>();

            return services;
        }
    }
}