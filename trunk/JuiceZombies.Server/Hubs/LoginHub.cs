﻿using JuiceZombies.Server.Datas;
using HotFix_UI;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace JuiceZombies.Server.Hubs;

public class LoginHub : Hub
{
    private readonly IConnectionMultiplexer _redis;
    private readonly HttpClient _httpClient;

    public LoginHub(IConnectionMultiplexer redis, HttpClient httpClient)
    {
        _redis = redis;
        _httpClient = httpClient;
    }

    public async Task<string> GetSessionJson(string jsCode)
    {
        string appId = AppKeys.AppID;
        string appSecret = AppKeys.AppSecret;
        // 构建请求的URL
        string url =
            $"https://api.weixin.qq.com/sns/jscode2session?appid={appId}&secret={appSecret}&js_code={jsCode}&grant_type=authorization_code";
        string responseBody = default;
        // 发起GET请求
        HttpResponseMessage response = await _httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            // 读取响应内容
            responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"responseBody:{responseBody}");
        }
        else
        {
            Console.WriteLine($"response from server: {response.ReasonPhrase}");
        }

        return responseBody;
    }

    public async void Login(S2C_UserResData player)
    {
        // Console.WriteLine($"serverLogin:{player}");
        // var db = _redis.GetDatabase();
        //
        // //long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        // //var playerdata = await db.StringGetAsync(player.Id.ToString());
        // var userInfoJson = await GetSessionJson(player.OtherData.Code);
        // Console.WriteLine($"userInfoJson:{userInfoJson}");
        // //var userData = JsonConvert.DeserializeObject<S2C_UserResData>(jsonData);
        // string jsonData = JsonConvert.SerializeObject(player);
        // await db.StringSetAsync(player.ThirdId, jsonData);

        //Clients.Caller.SendAsync("", player);
        //Console.WriteLine(timestamp);
        // await db.StringSetAsync(name.FirstName, timestamp.ToString());
        // db.KeyDelete("abc");
        // var value = await db.StringGetAsync("abc"); // 
        //
        // Console.WriteLine($"{name} is called Login");
    }

    public void Test()
    {
        Console.WriteLine($"Test:1");
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var url = httpContext.Request.GetEncodedUrl();

        // 你可以在这里记录URL或者执行其他操作
        Console.WriteLine("Client connected with ConnectionId: " + Context.ConnectionId);
        Console.WriteLine("Client connected with URL: " + url);

        await base.OnConnectedAsync();
    }
}