using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using log4net;
using log4net.Config;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configuração programática do log4net
        ConfigureLog4Net();

        // Adicionar serviços
        builder.Services.AddControllers();
        builder.Services.AddLogging(configure => 
        {
            configure.AddLog4Net();
        });

        var app = builder.Build();
        app.MapControllers();
        app.Run();
    }

    private static void ConfigureLog4Net()
    {
        var hierarchy = (Hierarchy)LogManager.GetRepository(typeof(Program).Assembly);
        hierarchy.Clear();

        // Configurar layout
        var patternLayout = new PatternLayout
        {
            ConversionPattern = "%date [%thread] %-5level %logger - %message%newline"
        };
        patternLayout.ActivateOptions();

        // Configurar console appender
        var consoleAppender = new ConsoleAppender
        {
            Layout = patternLayout
        };
        consoleAppender.ActivateOptions();

        // Configurar file appender
        var fileAppender = new RollingFileAppender
        {
            File = "logs/application.log",
            AppendToFile = true,
            RollingStyle = RollingFileAppender.RollingMode.Size,
            MaxSizeRollBackups = 5,
            MaximumFileSize = "10MB",
            Layout = patternLayout
        };
        fileAppender.ActivateOptions();

        // Adicionar appenders ao repositório
        hierarchy.Root.AddAppender(consoleAppender);
        hierarchy.Root.AddAppender(fileAppender);

        // Configurar nível de log
        hierarchy.Root.Level = log4net.Core.Level.Info;
        hierarchy.Configured = true;
    }
}

// Exemplo de uso em um controller
[ApiController]
[Route("[controller]")]
public class ExemploController : ControllerBase
{
    private static readonly ILog _logger = LogManager.GetLogger(typeof(ExemploController));

    [HttpGet]
    public IActionResult Get()
    {
        _logger.Info("Método Get foi chamado");
        return Ok();
    }
}
