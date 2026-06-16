using System.Xml.Linq;
using Caspian.Common.Extension;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Main
{
    public static class Initialize
    {
        public static void CreateFileAndFolder(this IWebHostEnvironment environment)
        {
            if (!environment.IsDevelopment())
            {
                var foldersName = new string[] { "Errors", "Data", "Report", "Report\\Images", "Report\\Print", "Report\\View" };
                foreach (var folderName in foldersName)
                {
                    var path = $"{environment.ContentRootPath}\\{folderName}";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                }
            }
        }
    }

    public class CaspianConsoleLoggerProvider : ILoggerProvider
    {
        WebApplicationBuilder builder;

        public CaspianConsoleLoggerProvider(WebApplicationBuilder builder)
        {
            File.WriteAllText("D:\\Test\\c.txt", "Step1");
            this.builder = builder;
        }

        public ILogger CreateLogger(string categoryName)
        {
            File.WriteAllText("D:\\Test\\b.txt", "Step2");
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
            File.WriteAllText("D:\\Test\\c.txt", "Step3");
            //var value = builder.Configuration.GetSection("DetailedErrors").Value?.ToLower();
            //if (value == null) 
            //    return false;
            //if (Boolean.TryParse(value, out _))
            //    return Convert.ToBoolean(value);
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            File.WriteAllText("D:\\Test\\d.txt", "Message");
            if (IsEnabled(logLevel) && logLevel == LogLevel.Error)
            {
                File.WriteAllText("D:\\Test\\a.txt", "Message");
                string message = exception?.Message?.ToString() ?? state?.ToString();
                if (message!= null)
                {
                    var path = $"{builder.Environment.ContentRootPath}\\Errors\\{Path.GetRandomFileName()}.xml";
                    var doc = new XElement("Errors");
                    var error = new XElement("Error");
                    doc.AddElement(error);
                    if (exception == null)
                        error.AddContent(message);
                    else
                    {
                        var ex = exception;
                        while(ex != null)
                        {
                            error.AddElement("Message", message).AddElement("StackTrace", exception.StackTrace);
                            ex = ex.InnerException;
                            error = new XElement("Error");
                        }
                    }
                    doc.Save("D:\\Test");
                }

            }
        }
    }
}
