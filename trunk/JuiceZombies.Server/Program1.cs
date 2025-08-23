// using System.Net;
// using JuiceZombies.Server.Hubs;
// using HotFix_UI;
// using StackExchange.Redis;
//
// var builder = WebApplication.CreateBuilder(args);
//
// // 添加 Redis 配置
// builder.Services.AddSingleton<HttpClient>();
// builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
// {
//     var config = new ConfigurationOptions
//     {
//         AbortOnConnectFail = false
//     };
//     config.EndPoints.Add(IPAddress.Loopback, 6379); // 默认 Redis 端口是 6379
//     config.SetDefaultPorts();
//     var connection = ConnectionMultiplexer.Connect(config);
//     connection.ConnectionFailed += (_, e) => { Console.WriteLine("Connection to Redis failed."); };
//
//     if (!connection.IsConnected)
//     {
//         Console.WriteLine("Did not connect to Redis.");
//     }
//
//     return connection;
// });
//
// builder.Services.AddSignalR()
//     .AddStackExchangeRedis(o =>
//     {
//         o.ConnectionFactory = async writer =>
//         {
//             var config = new ConfigurationOptions
//             {
//                 AbortOnConnectFail = false
//             };
//             config.EndPoints.Add(IPAddress.Loopback, 6379);
//             config.SetDefaultPorts();
//             var connection = await ConnectionMultiplexer.ConnectAsync(config, writer);
//             connection.ConnectionFailed += (_, e) => { Console.WriteLine("Connection to Redis failed."); };
//
//             if (!connection.IsConnected)
//             {
//                 Console.WriteLine("Did not connect to Redis.");
//             }
//
//             var db = connection.GetDatabase();
//             await Task.Delay(1000);
//             var playerdata = db.StringGet(2.ToString());
//             Console.WriteLine($"{playerdata}");
//             return connection;
//         };
//     });
//
// var app = builder.Build();
//
// // 配置中间件等
// app.MapHub<LoginHub>("/LoginHub");
// var url = $"http://{MyUrl.urlipv4}";
// app.Run(url);