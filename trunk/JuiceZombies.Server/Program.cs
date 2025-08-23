using System.Net;
using JuiceZombies.Server.Datas.Config.Scripts;
using JuiceZombies.Server.Services;
using HotFix_UI;
using JuiceZombies.Server.Datas;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore; // 新增：引入 EF Core


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

//
builder.Services.AddSingleton<HttpClient>();

builder.Services.AddSingleton<MyConfig>();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = new ConfigurationOptions
    {
        AbortOnConnectFail = false
    };
    config.EndPoints.Add(IPAddress.Loopback, 6379); // 默认 Redis 端口是 6379
    config.SetDefaultPorts();
    var connection = ConnectionMultiplexer.Connect(config);
    connection.ConnectionFailed += (_, e) => { Console.WriteLine("Connection to Redis failed."); };

    if (!connection.IsConnected)
    {
        Console.WriteLine("Did not connect to Redis.");
    }

    return connection;
});
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

var postgresConnectionString = builder.Configuration.GetConnectionString("PostgresConnection");
// 注册 DbContext 并配置使用 Npgsql
builder.Services.AddDbContext<MyPostDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));

var app = builder.Build();
// <snippet_UseWebSockets>
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);
// </snippet_UseWebSockets>

MyConfig.InitConfig();

Console.WriteLine($"test config:{MyConfig.Tables.Tbitem.Get(1001).id}");
//app.UseDefaultFiles();
//app.UseStaticFiles();

app.MapControllers();
var url = $"http://{MyUrl.urlipv4}";
app.Run(url);