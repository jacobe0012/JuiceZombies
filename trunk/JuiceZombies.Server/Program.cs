using System.Net;
using JuiceZombies.Server.Datas.Config.Scripts;
using JuiceZombies.Server.Services;
using HotFix_UI;
using JuiceZombies.Server.Datas;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using JuiceZombies.Server.Handlers; // 新增：引入你的处理程序所在的命名空间
using System.Reflection; // 新增：引入反射命名空间


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
builder.Services.AddDbContext<MyPostgresDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));

// === 新增：依赖注入注册逻辑 ===

// 注册 CommandHandlerFactory 为 Singleton
// 它的构造函数会执行一次性反射扫描
builder.Services.AddSingleton<CommandHandlerFactory>();

// 自动扫描并注册所有 ICommandHandler<T> 的实现
var handlerInterfaceType = typeof(ICommandHandler<>);
var handlerAssembly = typeof(JuiceZombies.Server.Handlers.LoginCommandHandler).Assembly; // 替换为你的处理程序所在的程序集

var handlerImplementations = handlerAssembly.GetTypes()
    .Where(t => !t.IsAbstract && !t.IsInterface && t.GetInterfaces()
        .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType));

foreach (var implementation in handlerImplementations)
{
    // 注册为 Transient 生命周期，以确保每次请求都有新的实例
    builder.Services.AddTransient(implementation);
}

// === 新增注册逻辑结束 ===

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