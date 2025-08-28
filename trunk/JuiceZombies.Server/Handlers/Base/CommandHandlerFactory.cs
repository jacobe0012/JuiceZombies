using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Reflection;
using AutoMapper;
using HotFix_UI;
using JuiceZombies.Server.Datas;
using StackExchange.Redis;

namespace JuiceZombies.Server.Handlers;

public class CommandHandlerFactory
{
    // 缓存：消息类型 -> 对应的处理程序类型
    //private readonly ConcurrentDictionary<string, Type> _handlerTypes = new ConcurrentDictionary<string, Type>();

    // 注入必要的依赖
    //private readonly IConnectionMultiplexer _redis;

    //private readonly IConnections _connections;
    private readonly IMapper _mapper;
    private readonly MyPostgresDbContext _context;

    public CommandHandlerFactory(IMapper mapper, MyPostgresDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    /// <summary>
    /// 根据消息类型创建处理程序实例
    /// </summary>
    /// <param name="commandType">消息类型（如：typeof(C2S_Login)）</param>
    /// <param name="serviceProvider">依赖注入容器</param>
    /// <returns>处理程序实例</returns>
    public ICommandHandler CreateHandler(int command)
    {
        return command switch
        {
            CMD.Auth.C2S_LOGIN => new LoginCommandHandler(_mapper, _context),
            CMD.Shop.C2S_QUERYSHOP => new QueryGameShopHandler(_mapper, _context),
            CMD.Shop.C2S_DRAWS => new GachaHandler(_mapper, _context),
            
            // CMD.QUERYRESOURCE => new QueryPlayerResourceHandler(_redis, _connections),
            // CMD.RECEIVEDAILYSIGN => new ReceiveDailySignHandler(_redis, _connections),
            // CMD.RECEIVEACHIEVEITEM => new ReceiveAchieveItemHandler(_redis, _connections),
            // CMD.RECEIVEACHIEVEBOX => new ReceiveAchieveBoxHandler(_redis, _connections),
            // CMD.RECEIVEMAILITEM => new ReceiveMailItemHandler(_redis, _connections),
            // CMD.RECEIVEDAILYSIGN7 => new ReceiveDailySign7Handler(_redis, _connections),
            // 类似的命令可以继续添加
            _ => throw new NotImplementedException($"未实现命令 {command} 的处理程序。")
        };
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