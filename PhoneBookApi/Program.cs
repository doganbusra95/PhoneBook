using Microsoft.EntityFrameworkCore;
using PhoneBookApi.DContext;
using PhoneBookApi.Settings;

var builder = WebApplication.CreateBuilder(args);

//Ayarlar b�l�m�n� alal�m
var ayarlarBolumu = builder.Configuration.GetSection("ProjSettings");
builder.Services.Configure<ProjSettings>(ayarlarBolumu);

//�al��ma parametresi
ProjSettings ayarlar = ayarlarBolumu.Get<ProjSettings>();
WSettings.Test = (ayarlar.IsTest.ToUpper() == "YES") ? true : false;
ayarlar.ConnectionString = (WSettings.Test)
    ? new DbConnectInfo().connectionString_Test
    : new DbConnectInfo().connectionString_Local;

//DB Context b�l�m�
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(ayarlar.ConnectionString));

//DB Ba�lant�m i�inde bir servis aktif et.
builder.Services.AddTransient<IConnect, Connect>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
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
