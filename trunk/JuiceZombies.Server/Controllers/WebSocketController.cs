using System.Collections.Concurrent;
using System.Net.WebSockets;
using JuiceZombies.Server.Services;
using HotFix_UI;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Diagnostics;
using AutoMapper;
using JuiceZombies.Server.Datas;
using JuiceZombies.Server.Handlers;
using JuiceZombies.Server.Log;


namespace JuiceZombies.Server.Controllers;

public class WebSocketController : ControllerBase
{
    private readonly IConnectionMultiplexer _redis;
    private readonly HttpClient _httpClient;
    private readonly IRedisCacheService _redisCache;
    private readonly MyPostgresDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;

    private static readonly MessagePackSerializerOptions _options =
        MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block);

    //链接 openid
    private static readonly ConcurrentDictionary<WebSocket, uint> _connections = new();

    public WebSocketController(MyPostgresDbContext context, IConnectionMultiplexer redis, HttpClient httpClient,
        IRedisCacheService redisCache, IServiceProvider serviceProvider, IMapper mapper)
    {
        _context = context;
        _redis = redis;
        _httpClient = httpClient;
        _redisCache = redisCache;
        _serviceProvider = serviceProvider;
        _mapper = mapper;
    }


    [Route("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            Console.WriteLine(
                $"ConnectionId:{HttpContext.Connection.Id} Ip:{HttpContext.Connection.RemoteIpAddress}");

            await Echo(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }


    private async Task Echo(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult receiveResult;

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                // 移除超时 CancellationToken，让 ReceiveAsync 无限期等待新消息
                receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (receiveResult.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine($"ConnectionId:{HttpContext.Connection.Id} Connection Closed");
                    _connections.TryRemove(webSocket, out _);
                    await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription,
                        CancellationToken.None);
                    return;
                }

                // 处理消息并生成回复
                var receivedMessage =
                    MessagePackSerializer.Deserialize<MyMessage>(
                        new ReadOnlyMemory<byte>(buffer, 0, receiveResult.Count), _options);
                var responseMessage = await ProcessMessage(receivedMessage, webSocket);

                // 使用 MessagePack 序列化回复消息
                var responseBuffer = MessagePackSerializer.Serialize(responseMessage, _options);

                // 将回复发送回客户端
                await webSocket.SendAsync(
                    new ArraySegment<byte>(responseBuffer, 0, responseBuffer.Length),
                    receiveResult.MessageType,
                    receiveResult.EndOfMessage,
                    CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            // 处理所有其他类型的异常，包括网络中断
            Console.WriteLine($"ConnectionId:{HttpContext.Connection.Id} Error: {ex.Message}");
            _connections.TryRemove(webSocket, out _);
            try
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Error occurred",
                    CancellationToken.None);
            }
            catch
            {
                // Ignore exceptions during close
            }
        }
    }

    public void AddConn(WebSocket webSocket, uint userId)
    {
        _connections.TryAdd(webSocket, userId);
    }

    private async Task<MyMessage> ProcessMessage(MyMessage message, WebSocket webSocket)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        _connections.TryGetValue(webSocket, out var userId);
        var commandHandlerFactory = new CommandHandlerFactory(_mapper, _context);
        var handler = commandHandlerFactory.CreateHandler(message.Cmd);
        var context = await handler.HandleAsync(new Context
        {
            Message = message,
            Controller = this,
            webSocket = webSocket,
            UserId = userId
        });

        string errorStr = message.ErrorCode != 0 ? $"ErrorCode:{message.ErrorCode},Content:" : "";
        stopwatch.Stop();

        MyLogger.Log(message.Cmd.ToString(), userId.ToString(), context.inputContentStr,
            $"{errorStr}{context.outputContentStr}",
            stopwatch);
        // 调用处理器的 HandleAsync 方法
        return context.message;
    }


    #region BoardCast

    private async Task NotifyUserAsync(uint openId, MyMessage message)
    {
        var ws = _connections.Where(a => a.Value == openId).FirstOrDefault();
        if (ws.Key != null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var webSocket = ws.Key;
            // 使用 MessagePack 序列化消息
            var responseBuffer = MessagePackSerializer.Serialize(message, _options);

            // 向客户端推送消息
            await webSocket.SendAsync(
                new ArraySegment<byte>(responseBuffer, 0, responseBuffer.Length),
                WebSocketMessageType.Binary,
                true, // 表示结束消息
                CancellationToken.None);

            stopwatch.Stop();
            string errorStr = message.ErrorCode != 0 ? $"ErrorCode:{message.ErrorCode}" : "";

            MyLogger.Log(message.Cmd.ToString(), _connections[webSocket].ToString(), $"ToUser:{openId}",
                $"ErrorCode:{errorStr},Content:",
                stopwatch);
        }
        else
        {
            // WebSocket 连接不可用或未找到该用户
            Console.WriteLine($"User {openId} is not connected or WebSocket is not open.");
        }
    }

    #endregion
}