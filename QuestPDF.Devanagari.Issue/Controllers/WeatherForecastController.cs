using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
namespace QuestPDF.Devanagari.Issue.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly string _fontName;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        //read file from output directory
        var  fileBytes = System.IO.File.OpenRead("NotoSansDevanagari-Regular.ttf");
        //register using font manager
        FontManager.RegisterFont(fileBytes);
        _fontName = "Noto Sans Devanagari";
        
        QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;
        QuestPDF.Settings.License = LicenseType.Community;
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
    [HttpGet("CreatePDF")]
    public  IActionResult CreatePDF()
    {
       
      var res=  Document.Create(container =>
        {
            container
                .Page(page =>
                {
                    page .DefaultTextStyle(text => text.FontFamily(_fontName));
                    page.Size(PageSizes.A4); ;
                    page.Content()
                        .Column(x =>
                        {
                            x.Item().Text("Mixed line: This 中文");
                            x.Item().Text("감사합니다 korean");
                            x.Item().Text("谢谢 chinese");
                            x.Item().Text("ありがとう ございます japanese");
                            x.Item().Text("நன்றி tamil");
                            x.Item().Text("धन्यवाद Hindi");
                            x.Item().Text("ขอบคุณ Thai");
                            x.Item().Text("teşekkürler Turkish");
                            x.Item().Text("ಧನ್ಯವಾದಗಳು Kannada");
                        });
                });
        }).GeneratePdf();
        return File(res, "application/pdf");
    }
}