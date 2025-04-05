using Microsoft.EntityFrameworkCore;
using PhoneBookApi.DContext;
using PhoneBookApi.Services;
using PhoneBookApi.Settings;
using PhoneBookApi.Settings.Mail;
using PhoneBookSharedTools.ExcelFileWorks;
using PhoneBookSharedTools.ExcelFileWorks._ClosedXML;
using PhoneBookSharedTools.ExcelFileWorks._DocumentFormatXML;

var builder = WebApplication.CreateBuilder(args);

//Eposta listesi alalým.
var _emailSettings = builder.Configuration.GetSection("EMailSettings");
EmailCounter.pbs = _emailSettings.Get<List<MailInfo>>();

//Ayarlar bölümünü alalým
var _settingsPart = builder.Configuration.GetSection("ProjSettings");
builder.Services.Configure<ProjSettings>(_settingsPart);

//Çalýþma parametresi
ProjSettings _settings = _settingsPart.Get<ProjSettings>();
WSettings.Test = (_settings.IsTest.ToUpper() == "YES") ? true : false;
_settings.ConnectionString = (WSettings.Test)
    ? new DbConnectInfo().connectionString_Test
    : new DbConnectInfo().connectionString_Local;

//Excel dosya kontrolleri için.
builder.Services.AddScoped<IExcelFileWorks, ExcelFileWorks_DocumentXML>();

//DB Context bölümü
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(_settings.ConnectionString));

//DB Baðlantým içinde bir servis aktif et.
builder.Services.AddTransient<IConnect, Connect>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
ServiceTool.Create(builder.Services);
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
