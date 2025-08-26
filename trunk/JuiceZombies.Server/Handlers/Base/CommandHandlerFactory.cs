using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Reflection;
using HotFix_UI;
using JuiceZombies.Server.Datas;
using StackExchange.Redis;

namespace JuiceZombies.Server.Handlers;

public class CommandHandlerFactory
{
    // 缓存：消息类型 -> 对应的处理程序类型
    private readonly ConcurrentDictionary<string, Type> _handlerTypes = new ConcurrentDictionary<string, Type>();

    // 注入必要的依赖
    //private readonly IConnectionMultiplexer _redis;

    //private readonly IConnections _connections;

    //private readonly MyPostgresDbContext _context;

    public CommandHandlerFactory()
    {
        // 1. 定义要查找的泛型接口类型
        var handlerInterfaceType = typeof(ICommandHandler<>);
        var handlerAssembly = Assembly.GetExecutingAssembly();

        // 2. 遍历程序集中的所有类型
        var handlerImplementations = handlerAssembly.GetTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType));

        foreach (var implementation in handlerImplementations)
        {
            // 3. 获取处理程序实现的泛型参数（即消息类型）
            var commandType = implementation.GetInterfaces()
                .Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
                .GetGenericArguments()[0];

            // 4. 将消息类型的名称（字符串）和处理程序类型添加到字典中
            //    例如："C2S_Login" -> typeof(LoginCommandHandler)
            _handlerTypes.TryAdd(commandType.Name, implementation);
        }
    }

    /// <summary>
    /// 根据消息类型创建处理程序实例
    /// </summary>
    /// <param name="commandType">消息类型（如：typeof(C2S_Login)）</param>
    /// <param name="serviceProvider">依赖注入容器</param>
    /// <returns>处理程序实例</returns>
    public ICommandHandler CreateHandler(string command, IServiceProvider serviceProvider)
    {
        if (_handlerTypes.TryGetValue(command, out var handlerType))
        {
            // 直接获取非泛型接口类型
            return (ICommandHandler)serviceProvider.GetRequiredService(handlerType);
        }

        throw new InvalidOperationException($"未找到命令 '{command}' 的处理程序。");
    }
    // public ICommandHandler CreateHandler(Type command)
    // {
    //     // return command switch
    //     // {
    //     //     CMD.Auth.C2S_LOGIN => new LoginCommandHandler(_context, _redis, _connections),
    //     //     CMD.Shop.C2S_QUERYSHOP => new QueryGameShopHandler(_context, _redis, _connections),
    //     //     // CMD.QUERYRESOURCE => new QueryPlayerResourceHandler(_redis, _connections),
    //     //     // CMD.RECEIVEDAILYSIGN => new ReceiveDailySignHandler(_redis, _connections),
    //     //     // CMD.RECEIVEACHIEVEITEM => new ReceiveAchieveItemHandler(_redis, _connections),
    //     //     // CMD.RECEIVEACHIEVEBOX => new ReceiveAchieveBoxHandler(_redis, _connections),
    //     //     // CMD.RECEIVEMAILITEM => new ReceiveMailItemHandler(_redis, _connections),
    //     //     // CMD.RECEIVEDAILYSIGN7 => new ReceiveDailySign7Handler(_redis, _connections),
    //     //     // 类似的命令可以继续添加
    //     //     _ => throw new NotImplementedException($"未实现命令 {command} 的处理程序。")
    //     // };
    // }
}