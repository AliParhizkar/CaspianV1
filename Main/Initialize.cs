﻿using System.Xml.Linq;
using Caspian.Common.Extension;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Main
{
    public static class Initialize
    {
        public static void CreateFileAndFolder(this WebApplication app)
        {

            if (!app.Environment.IsDevelopment())
            {
                var path = $"{app.Environment.ContentRootPath}\\Errors";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path = $"{app.Environment.ContentRootPath}\\Data";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
        }

        public static ILoggingBuilder AddCaspianConsoleLogger(this ILoggingBuilder builder, WebApplicationBuilder web)
        {
            builder.AddConfiguration();
            
            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, CaspianConsoleLoggerProvider>(t => 
                {
                    return new CaspianConsoleLoggerProvider(web);
                }));
            return builder;
        }
    }

    public class CaspianConsoleLoggerProvider : ILoggerProvider
    {
        WebApplicationBuilder builder;

        public CaspianConsoleLoggerProvider(WebApplicationBuilder builder)
        {
            this.builder = builder;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new CaspianConsoleLogger(builder);
        }

        public void Dispose()
        {
            
        }
    }

    public class CaspianConsoleLogger : ILogger
    {
        WebApplicationBuilder builder;

        public CaspianConsoleLogger(WebApplicationBuilder builder)
        {
            this.builder = builder;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return default;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            var value = builder.Configuration.GetSection("DetailedErrors").Value?.ToLower();
            if (value == null) 
                return false;
            if (Boolean.TryParse(value, out _))
                return Convert.ToBoolean(value);
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel) && logLevel == LogLevel.Error)
            {
                string message = exception?.Message?.ToString() ?? state?.ToString();
                if (message!= null)
                {
                    var path = $"{builder.Environment.ContentRootPath}\\Errors\\{Path.GetRandomFileName()}.xml";
                    var doc = new XElement("Errors");
                    var error = new XElement("Error");
                    if (exception == null)
                        error.AddContent(message);
                    else
                    {
                        var ex = exception;
                        while(ex != null)
                        {
                            error.AddElement("Message", message).AddElement("StackTrace", exception.StackTrace);
                            doc.AddElement(error);
                            ex = ex.InnerException;
                            error = new XElement("Error");
                        }
                    }
                    doc.Save(path);
                }

            }
        }
    }

}
