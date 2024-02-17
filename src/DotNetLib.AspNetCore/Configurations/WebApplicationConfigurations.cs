using DotNetLib.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;

namespace DotNetLib.AspNetCore.Extentions
{
    public static class WebApplicationConfigurations
    {

        public static IHostApplicationBuilder AddConfigurations(this IHostApplicationBuilder builder)
        {
            builder.Services.AddControllers(o =>
            {
                o.Filters.Add(typeof(ModelValidationFilter));
            });

            builder.Services.Configure<JsonOptions>(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
                options.SuppressMapClientErrors = true;
            });


            return builder;
        }

    }
}
