using System;
using System.Collections.Generic;
using System.Linq;
using cfg.blobstruct;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Collections;
using HotFix_UI;
using Main;
using Newtonsoft.Json;
using ProjectDawn.Navigation;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Networking;

namespace XFramework
{
    public class MenuScene : Scene
    {
        private bool isInit = false;

        async public virtual void GetObjects(ICollection<string> objKeys)
        {
            //objKeys.Add(UIPathSet.UIPanel_MonsterCollection);
            // objKeys.Add(UIPathSet.UIPanel_JiyuGame);
            // objKeys.Add(UIPathSet.UIPanel_Main);
            // objKeys.Add(UIPathSet.UIPanel_Challege);
            // objKeys.Add(UIPathSet.UIToggleItem);
            // objKeys.Add(UIPathSet.UIPanel_Task_DailyAndWeekly);
            // objKeys.Add(UIPathSet.UIPanel_Equipment);
            // var ui = await UIHelper.CreateAsync(UIType.UIPanel_MonsterCollection);
            // ui?.Dispose();
            //uuii.SetActive(false);
            // uuii.as
            //uuii?.Dispose();
            // var ui = UIHelper.Create(UIType.UIPanel_Settings);
            // ui.Dispose();
        }

        //private CancellationTokenSource cts;

        protected async override void OnCompleted()
        {
            ResourcesSingleton.Instance.isUIInit = false;
            //cts = new CancellationTokenSource();
            UnityHelper.BeginTime();
            RedDotManager.Instance.Clear();
            RedDotManager.Instance.CreateRedTreeTag();

            if (!ResourcesSingleton.Instance.FromRunTimeScene)
            {
                JiYuTweenHelper.EnableLoading(true);
            }

            AudioManager.Instance.StopFModSFX(false);
            var global = Common.Instance.Get<Global>();
            if (global.isStandAlone)
            {
                Log.Debug($"进入UIMenu场景 isStandAlone ", Color.green);
                StandAloneMode().Forget();
                return;
            }

            JiYuUIHelper.DownloadNotice().Forget();

            Log.Debug($"进入UIMenu场景 屏幕刷新率:{Screen.currentResolution.refreshRateRatio}", Color.green);

            #region 基础数据

            WebMessageHandlerOld.Instance.AddHandler(CMD.INITPLAYER, OnOpenMainPanelResponse, 1);
            WebMessageHandlerOld.Instance.AddHandler(CMD.OPENBAG, OnOpenBagPanelResponse, 1);
            //WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYPROPERTY, OnInitPlayerPropertyResponse, 1);
            WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYLEVEL, OnLevelRequsetResponse, 1);
            WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYSETTINGS, OnQuerySettingsResponse, 1);
            WebMessageHandlerOld.Instance.AddHandler(CMD.GETMAINPROPERTY, OnGetMainPropertyResponse, 1);
            WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYREDDOT, OnQueryRedDotResponse);

            #endregion

            #region 上层数据

            WebMessageHandlerOld.Instance.AddHandler(CMD.PREPAY, OnPrePayResponse);
            WebMessageHandlerOld.Instance.AddHandler(CMD.CHAPTERINFO, OnLevelInfoShowResponse, 2);
            WebMessageHandlerOld.Instance.AddHandler(CMD.SERVERTIME, OnServerTimeResponse, 2);
            WebMessageHandlerOld.Instance.AddHandler(CMD.UPDATETIME, OnQueryUpdateTimeResponse, 2);
            WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYEQUIP, OnQueryEquipResponse, 2);
            WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYMAIL, OnQueryMailResponse, 2);
            //WebMessageHandlerOld.Instance.AddHandler(11, 1, OnShopInitResponse);
            WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYSHOP, OnShopInitResponse, 2);
            WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYWEAR, OnQueryWearingEquipResponse);
            //WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYDAYANDWEEKTASK, OnDayAndWeekResponse, 2);
            //WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYACHIEVE, OnAchieveResponse, 2);
            WebMessageHandlerOld.Instance.AddHandler(12, 1, OnSignResponse, 2);
            //WebMessageHandlerOld.Instance.AddHandler(13, 1, OnNoticeResponse);
            WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYTALENT, OnQueryTalentResponse, 2);

            WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYCHALLENGE, OnQueryChallengeResponse, 2);
            WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYMONSTERCOLLECTION, OnQueryMonsterCollectionResponse, 2);
            WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYCHARGE, OnFirstChargeResponse, 2);
            WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYBANK, OnQueryBankResponse, 2);
            //WebMessageHandlerOld.Instance.AddHandler(CMD.PASSTIME, OnPassTimeResponse, 2);
            //WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYPASS, OnQueryPassResponse, 2);
            WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYACTIVITY, OnQueryActivityResponse, 2);
            //WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYACTIVITYTASK, OnQueryMonopolyTaskResponse, 2);
            WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYACTIVITYREDDOT, OnQueryActivityRedDotResponse, 2);
            WebMessageHandlerOld.Instance.AddHandler(CMD.PASSEXP, OnQueryPassExpReddot, 2);
            WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYSHARE, OnShareInfoInitResponse, 2);

            WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYDAILYREDDOT, OnQueryDailyRedDotResponse, 2);

            #endregion


            #region 广播

            WebMessageHandlerOld.Instance.AddHandler(99, 2, OnGiftChangeResponse);
            WebMessageHandlerOld.Instance.AddHandler(99, 6, OnFirstChargeChangeResponse);
            //WebMessageHandlerOld.Instance.AddHandler(CMD.BOARDCASTUPDATEPROPERTY, OnInitPlayerPropertyBoardResponse);
            WebMessageHandlerOld.Instance.AddHandler(99, 3, OnNoticeStatusChangeResponse);
            // WebMessageHandlerOld.Instance.AddHandler(100, 1, OnTaskChangeResponse);
            WebMessageHandlerOld.Instance.AddHandler(CMD.BOARDCASTMAIL, OnBoardCastMail);
            WebMessageHandlerOld.Instance.AddHandler(CMD.BOARDCASTUPDATEFUCTASK, OnBoardCastUpdateFuncTaskResponse);
            WebMessageHandlerOld.Instance.AddHandler(CMD.BOARDCASTPAYRESPONSE, OnBoardCastPaymentResponse);

            #endregion

            QueryAllRedDot();

            NetWorkManager.Instance.SendMessage(CMD.INITPLAYER);
            NetWorkManager.Instance.SendMessage(CMD.OPENBAG);
            //NetWorkManager.Instance.SendMessage(CMD.QUERYPROPERTY);
            NetWorkManager.Instance.SendMessage(CMD.QUERYLEVEL);
            NetWorkManager.Instance.SendMessage(CMD.GETMAINPROPERTY);

            //13,2 查询设置*
            NetWorkManager.Instance.SendMessage(CMD.QUERYSETTINGS);
            WebMessageHandlerOld.Instance.AddTagEvnetHandler(1, (a, b) =>
            {
                //2,4 查询章节信息 精简 *
                NetWorkManager.Instance.SendMessage(CMD.CHAPTERINFO);
                //9,5查询所有装备*
                NetWorkManager.Instance.SendMessage(CMD.QUERYEQUIP);
                //8,1 查询活动相关信息
                NetWorkManager.Instance.SendMessage(CMD.QUERYACTIVITY);
                //8,8 查询单个活动红点*
                NetWorkManager.Instance.SendMessage(CMD.QUERYACTIVITYREDDOT);

                //8,8 查询单个活动红点*
                NetWorkManager.Instance.SendMessage(CMD.PASSEXP);
                //19,1 服务器当前时间戳*
                NetWorkManager.Instance.SendMessage(CMD.SERVERTIME);
                //19,2 服务器每天更新时间*
                NetWorkManager.Instance.SendMessage(CMD.UPDATETIME);

                //5,1  查询邮件*
                NetWorkManager.Instance.SendMessage(CMD.QUERYMAIL);
                //11,10 查询商店信息*
                NetWorkManager.Instance.SendMessage(CMD.QUERYSHOP);
                //2,10查询解锁天赋*
                NetWorkManager.Instance.SendMessage(CMD.QUERYTALENT);
                //NetWorkManager.Instance.SendMessage(13, 1);
                NetWorkManager.Instance.SendMessage(12, 1);
                //NetWorkManager.Instance.SendMessage(CMD.QUERYDAYANDWEEKTASK);
                //3,4 查询成就*
                //NetWorkManager.Instance.SendMessage(CMD.QUERYACHIEVE);
                //2,13*
                NetWorkManager.Instance.SendMessage(CMD.QUERYCHALLENGE);
                //18,1 查询怪物图鉴*
                NetWorkManager.Instance.SendMessage(CMD.QUERYMONSTERCOLLECTION);
                NetWorkManager.Instance.SendMessage(17, 1);
                NetWorkManager.Instance.SendMessage(CMD.QUERYBANK);
                //14,6 查询通行证解锁时间*
                //NetWorkManager.Instance.SendMessage(CMD.PASSTIME);
                //14,2 查询通行证信息*
                //NetWorkManager.Instance.SendMessage(CMD.QUERYPASS);
                NetWorkManager.Instance.SendMessage(CMD.QUERYSHARE);
                //3-8 查询日常周常红点
                NetWorkManager.Instance.SendMessage(CMD.QUERYDAILYREDDOT);
                //NetWorkManager.Instance.SendMessage(CMD.QUERYAUTOPATROL, partorlValue);
                //Log.Error("SendMessage(CMD.CHALLENGEQUERY)");
                //Log.Debug($"进入UIMenu场景SendMessageDone", Color.green);
            });
            WebMessageHandlerOld.Instance.AddTagEvnetHandler(2, async (a, b) =>
            {
                //Log.Error("CreateUIPanel_JiyuGame");
                if (!JiYuUIHelper.TryGetUI(UIType.UISubPanel_RawBackground, out var ui))
                {
                    await UIHelper.CreateAsync(UIType.UISubPanel_RawBackground);
                }

                if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var ui1))
                {
                    //Log.Error("CreateUIPanel_JiyuGame1");
                    UIHelper.CreateAsync(UIType.UIPanel_JiyuGame, ResourcesSingleton.Instance.UserInfo).Forget();
                }
            });

            //AudioManager.Instance.PlayBgmAudio("UIBGM", false);
            //AudioManager.Instance.PlayFModAudio(1101);
        }

        private void QueryAllRedDot()
        {
            var tbtag_func = ConfigManager.Instance.Tables.Tbtag_func;
            foreach (var tagFunc in tbtag_func.DataList)
            {
                NetWorkManager.Instance.SendMessage(CMD.QUERYREDDOT, new IntValue
                {
                    Value = tagFunc.id
                });
            }
        }

        private async UniTaskVoid SetUpdateDataFromServer()
        {
            var dTime = ResourcesSingleton.Instance.serverDeltaTime;
            var uTime = ResourcesSingleton.Instance.updateTime;
            long updateTime = uTime - dTime / 1000 + 1;
            // await UniTask.Delay((int)(updateTime - TimeHelper.ClientNowSeconds()) * 1000, false,
            //     PlayerLoopTiming.Update, ResourcesSingleton.Instance.UpdateCTS.Token);
            //SendUpdateMessage();
        }

        // private void SendUpdateMessage()
        // {
        //     WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYSHOP, OnShopInitResponse);
        //     NetWorkManager.Instance.SendMessage(CMD.QUERYSHOP);
        //     WebMessageHandlerOld.Instance.AddHandler(CMD.PASSTIME, OnPassTimeResponse);
        //     NetWorkManager.Instance.SendMessage(CMD.PASSTIME);
        // }

        void OnQueryRedDotResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            StringValue stringValue = new StringValue();
            stringValue.MergeFrom(e.data);

            Log.Debug($"OnQueryRedDotResponse{stringValue.Value}", Color.green);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            if (!stringValue.Value.IsNullOrEmpty())
            {
                var strs = stringValue.Value.Split(";");
                int str0 = int.Parse(strs[0]);
                int str1 = int.Parse(strs[1]);
                if (ResourcesSingleton.Instance.redDots.ContainsKey(str0))
                {
                    ResourcesSingleton.Instance.redDots[str0] = str1;
                }
                else
                {
                    ResourcesSingleton.Instance.redDots.Add(str0, str1);
                }
            }
        }

        void OnQueryPassResponse(object sender, WebMessageHandlerOld.Execute e)
        {
        }

        async public void OnQueryMonopolyTaskResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYACTIVITYTASK, OnQueryMonopolyTaskResponse);

            ByteValueList taskList = new ByteValueList();

            taskList.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            if (!JiYuUIHelper.TryGetActivityLink(23, out var activityId, out var link))
            {
                return;
            }

            ResourcesSingleton.Instance.activity.activityTaskDic.TryRemove(activityId, out var list);

            var tasks = new List<GameTaskInfo>();
            Log.Debug($"gameTaskInfo", Color.green);
            foreach (var taskBytes in taskList.Values)
            {
                GameTaskInfo gameTaskInfo = new GameTaskInfo();
                gameTaskInfo.MergeFrom(taskBytes);
                tasks.Add(gameTaskInfo);

                Log.Debug($"{gameTaskInfo.ToString()}", Color.green);
            }

            var tbtask = ConfigManager.Instance.Tables.Tbtask;
            var tbtask_group = ConfigManager.Instance.Tables.Tbtask_group;
            var tbtask_type = ConfigManager.Instance.Tables.Tbtask_type;
            var tbmonopoly = ConfigManager.Instance.Tables.Tbmonopoly;


            bool isRedDot = false;

            var m_RedDotName = NodeNames.GetTagFuncRedDotName(tbmonopoly.Get(link).tagFunc);

            var taskListStr =
                $"{m_RedDotName}|Pos1|Task";

            foreach (var gameTask in tasks)
            {
                //RedDotManager.Instance.ClearChildrenListeners(pos1);
                var task = tbtask.Get(gameTask.Id);
                var task_group = tbtask_group.Get(task.group);
                var task_type = tbtask_type.Get(task.type);

                bool canReceive = gameTask.Para >= task.para[0] && gameTask.Status == 0;
                if (canReceive)
                {
                    isRedDot = true;
                }
            }

            Log.Debug($"isRedDot{isRedDot}", Color.green);
            RedDotManager.Instance.SetRedPointCnt(taskListStr, isRedDot ? 1 : 0);

            var node = RedDotManager.Instance.GetNode(m_RedDotName);
            node.PrintTree();

            ResourcesSingleton.Instance.activity.activityTaskDic.TryAdd(activityId, tasks);
        }

        private void OnLevelRequsetResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYLEVEL, OnLevelRequsetResponse);

            var chapterList = ConfigManager.Instance.Tables.Tbchapter.DataList;
            var config = new ConfigurationLoad();
            config.MergeFrom(e.data);

            var currentID = config.ChapterId;
            if (currentID == 0)
            {
                currentID = chapterList[currentID].levelId;
            }


            //Log.Error($"1111111111111111111111111后端返回的当前关卡id:{currentID}", Color.cyan);
            ResourcesSingleton.Instance.levelInfo.levelId = currentID;
        }

        private void OnServerTimeResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.SERVERTIME, OnServerTimeResponse);
            LongValue longValue = new LongValue();
            longValue.MergeFrom(e.data);
            Log.Debug($"服务器时间戳: {longValue}", Color.green);
            if (e.data.IsEmpty)
            {
                Log.Debug($"OnServerTimeResponse .IsEmpty", Color.red);
                return;
            }

            ResourcesSingleton.Instance.serverDeltaTime = TimeHelper.ClientNow() - longValue.Value;
            Log.Debug($"ServerDeltaTime: {ResourcesSingleton.Instance.serverDeltaTime}", Color.green);
        }

        private void OnShareInfoInitResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYSHARE, OnShareInfoInitResponse);
            var shareData = new GameShare();
            shareData.MergeFrom(e.data);
            Log.Debug($"OnShareInfoResponse {shareData}", Color.green);

            if (e.data.IsEmpty)
            {
                Log.Debug($"OnShareInfoResponse IsEmpty", Color.green);
                return;
            }

            ResourcesSingleton.Instance.gameShare = shareData;
            JiYuUIHelper.DownloadShare().Forget();
        }

        private void OnQueryPassExpReddot(object sender, WebMessageHandlerOld.Execute e)
        {
            IntValue longValue = new IntValue();
            longValue.MergeFrom(e.data);
            Log.Debug($"OnQueryPassExpReddot: {longValue}", Color.green);
            if (e.data.IsEmpty)
            {
                Log.Debug($"OnQueryPassExpReddot .IsEmpty", Color.red);
                return;
            }

            ResourcesSingleton.Instance.gamePassExp = longValue.Value == null ? 0 : longValue.Value;
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out var ui))
            {
                var uis = ui as UIPanel_Main;
                uis.ShowPassLevelExp(longValue.Value);
            }
        }

        private void OnQueryActivityRedDotResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYACTIVITYREDDOT, OnQueryActivityRedDotResponse);
            ActivityFlag activityMap = new ActivityFlag();
            activityMap.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Debug.Log("QUERYACTIVITY is empty");
                return;
            }

            Log.Debug($"OnQueryActivityResponse{activityMap}", Color.green);
            // foreach (var active in activityMap.ActivityFlagMap)
            // {
            //     Log.Debug($"{active.Key}", Color.green);
            // }

            ResourcesSingleton.Instance.activity.ActivityFlag = activityMap;
        }


        private void OnQueryDailyRedDotResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYDAILYREDDOT, OnQueryDailyRedDotResponse);
            TaskRedFlag taskRedFlag = new TaskRedFlag();
            taskRedFlag.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Debug.Log("OnQueryDailyRedDotResponse is empty");
                return;
            }

            Log.Debug($"OnQueryDailyRedDotResponse{taskRedFlag}", Color.green);
            // foreach (var active in activityMap.ActivityFlagMap)
            // {
            //     Log.Debug($"{active.Key}", Color.green);
            // }

            ResourcesSingleton.Instance.taskRedFlag = taskRedFlag;
        }

        private void OnQueryActivityResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYACTIVITY, OnQueryActivityResponse);
            ActivityMap activityMap = new ActivityMap();
            activityMap.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Debug.Log("QUERYACTIVITY is empty");
                return;
            }

            Log.Debug($"OnQueryActivityResponse", Color.green);
            foreach (var active in activityMap.ActivityMap_)
            {
                Log.Debug($"{active.Key}", Color.green);
            }

            ResourcesSingleton.Instance.activity.activityMap = activityMap;
        }


        private void OnPassTimeResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            // WebMessageHandlerOld.Instance.RemoveHandler(CMD.PASSTIME, OnPassTimeResponse);
            // if (e.data.IsEmpty)
            // {
            //     // ResourcesSingleton.Instance.passTime.id = 0;
            //     // ResourcesSingleton.Instance.passTime.gamePassStartTime = 0;
            //     // ResourcesSingleton.Instance.passTime.gamePassEndTime = 0;
            //     //
            //     ResourcesSingleton.Instance.gamePassStart = false;
            //     ResourcesSingleton.Instance.gamePassStartTime = 0;
            //     ResourcesSingleton.Instance.gamePassEndTime = 0;
            //
            //     Log.Debug($"OnPassTimeResponse empty", Color.green);
            //     return;
            // }
            //
            // var str = new StringValue();
            // str.MergeFrom(e.data);
            // /**
            //  * 查询通行证开启时间 14 -6
            //  * @return String "0;0;0" 无效配置 "通行证id;通行证startTime;通行证endTime"
            //  */
            // // @ActionMethod(PassCardModule.findPassConfig)
            // // public String findPassConfig() {
            // //     return gameUtils.activeStart(11);
            // // }
            //
            // Log.Debug($"OnPassTimeResponse {str}", Color.green);
            // var strs = str.Value.Split(";");
            //
            // if (strs.Length != 3)
            // {
            //     ResourcesSingleton.Instance.gamePassStart = false;
            //     ResourcesSingleton.Instance.gamePassStartTime = 0;
            //     ResourcesSingleton.Instance.gamePassEndTime = 0;
            // }
            // else if (int.Parse(strs[0]) == 0)
            // {
            //     ResourcesSingleton.Instance.gamePassStart = false;
            //     ResourcesSingleton.Instance.gamePassStartTime = 0;
            //     ResourcesSingleton.Instance.gamePassEndTime = 0;
            // }
            // else if (int.Parse(strs[0]) != 0)
            // {
            //     ResourcesSingleton.Instance.gamePassStartTime = long.Parse(strs[1]);
            //     ResourcesSingleton.Instance.gamePassEndTime = long.Parse(strs[2]);
            //     long nowTime = TimeHelper.ClientNowSeconds() - (ResourcesSingleton.Instance.ServerDeltaTime / 1000);
            //
            //
            //     if (nowTime >= ResourcesSingleton.Instance.gamePassStartTime &&
            //         nowTime <= ResourcesSingleton.Instance.gamePassEndTime)
            //     {
            //         ResourcesSingleton.Instance.gamePassStart = true;
            //     }
            //     else if (nowTime < ResourcesSingleton.Instance.gamePassStartTime)
            //     {
            //         Log.Debug(
            //             $"判断通行证没有开启 客户端时间戳:{nowTime} 服务器开始时间戳:{ResourcesSingleton.Instance.gamePassStartTime} 服务器结束时间戳:{ResourcesSingleton.Instance.gamePassEndTime}",
            //             Color.green);
            //         ResourcesSingleton.Instance.gamePassStart = false;
            //         if (ResourcesSingleton.Instance.gamePassStartTime < TimeHelper.GetToRecentUpdateTime())
            //         {
            //             SendPassTimeMessageDelay((int)(ResourcesSingleton.Instance.gamePassStartTime - nowTime))
            //                 .Forget();
            //         }
            //     }
            //     else
            //     {
            //         ResourcesSingleton.Instance.gamePassStart = false;
            //         if (ResourcesSingleton.Instance.gamePassEndTime < TimeHelper.GetToRecentUpdateTime())
            //         {
            //             SendPassTimeMessageDelay((int)(ResourcesSingleton.Instance.gamePassEndTime - nowTime)).Forget();
            //         }
            //     }
            // }
            // else
            // {
            //     ResourcesSingleton.Instance.gamePassStart = false;
            //     ResourcesSingleton.Instance.gamePassStartTime = 0;
            //     ResourcesSingleton.Instance.gamePassEndTime = 0;
            // }
            //
            // if (ResourcesSingleton.Instance.gamePassStart && ResourcesSingleton.Instance.gamePasses.Count == 0)
            // {
            //     WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYPASS, OnQueryPassResponse);
            //     NetWorkManager.Instance.SendMessage(CMD.QUERYPASS);
            // }
            //
            // if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out UI ui))
            // {
            //     var uis = ui as UIPanel_Main;
            //     uis.PassUpdate();
            // }
        }

        private async UniTaskVoid SendPassTimeMessageDelay(int input)
        {
            // if (input < 0) return;
            // await UniTask.Delay(input * 1000, false, PlayerLoopTiming.Update, cts.Token);
            // WebMessageHandlerOld.Instance.AddHandler(CMD.PASSTIME, OnPassTimeResponse);
            // NetWorkManager.Instance.SendMessage(CMD.PASSTIME);
        }

        private void OnQueryUpdateTimeResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.UPDATETIME, OnQueryUpdateTimeResponse);
            LongValue longValue = new LongValue();
            longValue.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Debug.Log("update time is empty");
                return;
            }

            ResourcesSingleton.Instance.updateTime = longValue.Value;
            SetUpdateDataFromServer().Forget();
        }

        private void OnQueryMonsterCollectionResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            Log.Debug($"接收到后端数据,刷新怪物图鉴数据", Color.cyan);
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYMONSTERCOLLECTION, OnQueryMonsterCollectionResponse);
            //ResourcesSingleton.Instance.resMonster.MonsterMap.Clear();
            if (e.data.IsEmpty)
            {
                Log.Debug("怪物图鉴数据为空", Color.yellow);
                //TODO:
                //return;
            }

            GameMonster resMonster = new GameMonster();
            resMonster.MergeFrom(e.data.ToByteArray());

            var tbmonster = ConfigManager.Instance.Tables.Tbmonster;
            var tbmonster_attr = ConfigManager.Instance.Tables.Tbmonster_attr;
            var tbpower = ConfigManager.Instance.Tables.Tbpower;
            var tbweapon = ConfigManager.Instance.Tables.Tbweapon;
            var tbmonster_model = ConfigManager.Instance.Tables.Tbmonster_model;

            ResourcesSingleton.Instance.resMonster = resMonster;
            //TODO:Test
            // ResourcesSingleton.Instance.resMonster.MonsterMap.Add(1011, 2);
            // ResourcesSingleton.Instance.resMonster.MonsterMap.Add(2021, 2);
            // ResourcesSingleton.Instance.resMonster.MonsterMap.Add(1031, 2);
            // ResourcesSingleton.Instance.resMonster.WeaponMap.Add(101, 2);
            // ResourcesSingleton.Instance.resMonster.WeaponMap.Add(201, 2);
            // ResourcesSingleton.Instance.resMonster.WeaponMap.Add(10201, 2);

            int tagfuncId = 5104;
            string m_RedDotName = NodeNames.GetTagFuncRedDotName(tagfuncId);

            // foreach (var item in tbpower.DataList)
            // {
            string Pos0 = $"{m_RedDotName}{RedDotManager.Instance.SplitChar}Pos0";
            RedDotManager.Instance.InsterNode(Pos0);
            string Pos1 = $"{m_RedDotName}{RedDotManager.Instance.SplitChar}Pos1";
            RedDotManager.Instance.InsterNode(Pos1);
            //}

            Log.Debug($"{resMonster.MonsterMap.ToString()}", Color.green);

            foreach (var item in ResourcesSingleton.Instance.resMonster.MonsterMap)
            {
                var monster = tbmonster_model.GetOrDefault(item.Key);

                if (monster == null)
                {
                    Log.Error($"item.Key{item.Key} tbmonster_model表没有");
                    continue;
                }

                var itemStr =
                    $"{m_RedDotName}{RedDotManager.Instance.SplitChar}Pos0{RedDotManager.Instance.SplitChar}{item.Key}";
                RedDotManager.Instance.InsterNode(itemStr);
                RedDotManager.Instance.SetRedPointCnt(itemStr, item.Value == 2 ? 1 : 0);
            }

            foreach (var item in ResourcesSingleton.Instance.resMonster.WeaponMap)
            {
                var weapon = tbweapon.GetOrDefault(item.Key);

                if (weapon == null)
                {
                    Log.Error($"item.Key{item.Key} weapon表没有");
                    continue;
                }

                var itemStr =
                    $"{m_RedDotName}{RedDotManager.Instance.SplitChar}Pos1{RedDotManager.Instance.SplitChar}{item.Key}";
                RedDotManager.Instance.InsterNode(itemStr);
                RedDotManager.Instance.SetRedPointCnt(itemStr, item.Value == 2 ? 1 : 0);
            }

            Log.Debug($"怪物图鉴红点树打印：", Color.green);
            var modelA = RedDotManager.Instance.GetNode($"{m_RedDotName}");
            modelA.PrintTree();
        }

        private void OnQueryChallengeResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            //Log.Error("QueryChallengeResponse");
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYCHALLENGE, OnQueryChallengeResponse);
            var tbChallenge = ConfigManager.Instance.Tables.Tbchallenge;
            var tbChallengeList = tbChallenge.DataList;
            Dictionary<int, int> switchID = new Dictionary<int, int>();
            for (int i = 0; i < tbChallengeList.Count; i++)
            {
                switchID.Add(tbChallengeList[i].levelId, tbChallengeList[i].id);
            }


            var challengeList = new ByteValueList();
            challengeList.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Log.Error("cmd 2,13接收到空消息");
                return;
            }

            int maxMainChallengeID = 0;
            int maxAreaChallengeID = 0;
            foreach (var challenge in challengeList.Values)
            {
                GameChallenge gameChallenge = new GameChallenge();
                gameChallenge.MergeFrom(challenge);
                var challengeMap = ResourcesSingleton.Instance.challengeInfo.challengeStateMap;

                var challengeID = switchID[gameChallenge.LevelId];
                if (challengeMap.ContainsKey(challengeID))
                {
                    challengeMap[challengeID] = gameChallenge.ChallengeStatus;
                }
                else
                {
                    challengeMap.Add(challengeID, gameChallenge.ChallengeStatus);
                }


                Log.Debug($"levelID:{challengeID},state:{gameChallenge.ChallengeStatus}", Color.cyan);
                if (tbChallenge.Get(challengeID).type == 2 && gameChallenge.ChallengeStatus != 1)
                {
                    if (maxMainChallengeID < challengeID)
                    {
                        maxMainChallengeID = challengeID;
                    }
                }

                if (tbChallenge.Get(challengeID).type == 3 && gameChallenge.ChallengeStatus != 1)
                {
                    if (maxAreaChallengeID < challengeID)
                    {
                        maxAreaChallengeID = challengeID;
                    }
                }
            }

            Log.Debug($"maxMainChallengeID:{maxMainChallengeID},maxAreaChallengeID:{maxAreaChallengeID}", Color.cyan);
            ResourcesSingleton.Instance.challengeInfo.maxAreaChallengeID = maxAreaChallengeID;
            ResourcesSingleton.Instance.challengeInfo.maxMainChallengeID = maxMainChallengeID;
        }


        async UniTaskVoid StandAloneMode()
        {
            var gameRole = new GameRole
            {
                RoleId = 0,
                UserId = 0,
                Nickname = "单机账号001",
                UpdateTimeNickname = 0,
                RoleAvatar = 1001,
                RoleAvatarFrame = 2001,
                RoleAssets = new GameRoleAssets
                {
                    AssetsId = 0,
                    RoleId = 0,
                    Level = 2,
                    Exp = 99999,
                    Energy = 99999,
                    EnergyMax = 99999,
                    //EnergyRestore = 99999,
                    EnergyUpdate = 99999,
                    EnergyCountdown = 99999,
                    Bitcoin = 99999,
                    UsBill = 99999,
                    EquipGradle = 99999,
                    Update = 99999,
                    EnergyCost = 1
                },
                PatrolGainName = 0,
                AvatarMap = new RoleAvatarMap
                {
                    RoleId = 0
                }
            };
            var tbchapter = ConfigManager.Instance.Tables.Tbchapter;
            // ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy = gameRole.RoleAssets.Energy;
            // ResourcesSingleton.Instance.playerResources.maxEnergy = gameRole.RoleAssets.EnergyMax;
            // ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin = gameRole.RoleAssets.Bitcoin;
            // ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill = gameRole.RoleAssets.UsBill;
            //var totalExp = gameRole.RoleAssets.Exp;
            //ResourcesSingleton.Instance.UserInfo.RoleAssets.Exp = totalExp;

            ResourcesSingleton.Instance.levelInfo.maxMainBlockID =
                tbchapter.DataList[tbchapter.DataList.Count - 1].blockId;
            ResourcesSingleton.Instance.levelInfo.maxUnLockChapterID =
                tbchapter.DataList[tbchapter.DataList.Count - 1].id;

            if (JsonManager.Instance.userData.chapterId == 0)
            {
                JsonManager.Instance.userData.chapterId = ResourcesSingleton.Instance.levelInfo.maxUnLockChapterID + 1;
                JsonManager.Instance.userData.blockId = ResourcesSingleton.Instance.levelInfo.maxMainBlockID + 1;
                JsonManager.Instance.userData.lastChapterId = 1;
                JsonManager.Instance.SavePlayerData(JsonManager.Instance.userData);
            }

            #region 黄金国新增

            ResourcesSingleton.Instance.UserInfo = gameRole;

            #endregion


            var tbtag = ConfigManager.Instance.Tables.Tbtag;
            var tbtag_func = ConfigManager.Instance.Tables.Tbtag_func;
            var tbguide = ConfigManager.Instance.Tables.Tbguide;
            foreach (var tag in tbtag.DataList)
            {
                ResourcesSingleton.Instance.settingData.UnlockMap.TryAdd(tag.id, 0);
            }

            foreach (var tag in tbtag_func.DataList)
            {
                ResourcesSingleton.Instance.settingData.UnlockMap.TryAdd(tag.id, 0);
            }

            // foreach (var guide in tbguide.DataList)
            // {
            //     if (!ResourcesSingleton.Instance.settingData.GuideList.Contains(guide.group))
            //     {
            //         ResourcesSingleton.Instance.settingData.GuideList.Add(guide.group);
            //     }
            // }

            ResourcesSingleton.Instance.items.TryAdd(4010001, 200);
            ResourcesSingleton.Instance.items.TryAdd(4010002, 200);
            InitSettings();
            ResourcesSingleton.Instance.UpdateResourceUI();
            //Log.Error($"单机模式：UIPanel_JiyuGame");

            await UIHelper.CreateAsync(UIType.UISubPanel_RawBackground);
            UIHelper.CreateAsync(UIType.UIPanel_JiyuGame, gameRole).Forget();
        }

        void InitSettings()
        {
            switch (ResourcesSingleton.Instance.settingData.Quality)
            {
                case (int)GraphicQuality.HD:
                    JiYuUIHelper.SetFrameRate(true);
                    break;
                case (int)GraphicQuality.Normal:
                    JiYuUIHelper.SetFrameRate(false, 60);
                    break;
                case (int)GraphicQuality.Flow:
                    JiYuUIHelper.SetFrameRate(false, 30);
                    break;
            }

            //JiYuUIHelper.InitAudioSettings();

            ConfigManager.Instance.SwitchLanguages(ResourcesSingleton.Instance.settingData.CurrentL10N);

            // if (JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var ui0))
            // {
            //     var uijiyu = ui0 as UIPanel_JiyuGame;
            //     uijiyu.RefreshToggleLanguage();
            // }
            //
            // if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Person, out var ui))
            // {
            //     var uiperson = ui as UIPanel_Person;
            //     uiperson.Initialize();
            // }
        }

        private void OnQueryEquipResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            Log.Debug($"接收到后端数据,刷新装备缓存数据", Color.cyan);
            //WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYEQUIP, OnQueryEquipResponse);
            ResourcesSingleton.Instance.equipmentData.equipments.Clear();
            ResourcesSingleton.Instance.equipmentData.isMaterials.Clear();
            ByteValueList gameEquips = new ByteValueList();
            gameEquips.MergeFrom(e.data.ToByteArray());
            var materialDic = new Dictionary<Vector3, int>();
            foreach (var gameEquip in gameEquips.Values)
            {
                EquipDto equip = new EquipDto();
                equip.MergeFrom(gameEquip);

                MyGameEquip myGameEquip = new MyGameEquip
                {
                    equip = equip,
                    isWearing = false,
                    canCompound = false
                };
                int equipId = equip.EquipId * 100 + equip.Quality;
                if (JiYuUIHelper.IsCompositeEquipReward(myGameEquip.equip))
                {
                    //Log.Error($"{item.Value.equip}");

                    var reward = new Vector3(11, equipId, 1);
                    //如果有相同种类的通用合成材料,则数量加1
                    materialDic.TryAdd(reward, myGameEquip.equip.Count);
                    // if (materialDic.ContainsKey(reward))
                    //     materialDic[reward]++;
                    // //如果没有,则视做新装备
                    // else
                    // {
                    //     materialDic.Add(reward, 1);
                    // }
                }
                else
                {
                    ResourcesSingleton.Instance.equipmentData.equipments.TryAdd(equip.PartId, myGameEquip);
                }

                Log.Debug($"接收到后端装备数据{equip}", Color.green);
            }

            ResourcesSingleton.Instance.equipmentData.isMaterials = materialDic;
            JiYuUIHelper.SetCanCompound();
            JiYuUIHelper.SortEquipments();
            //JiYuUIHelper.SetMaterialDic();
            NetWorkManager.Instance.SendMessage(CMD.QUERYWEAR);
        }


        private void OnLevelInfoShowResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.CHAPTERINFO, OnLevelInfoShowResponse);
            //Log.Error($"OnLevelInfoShowResponse {333333}", Color.green);
            var roleInfo = new RoleChapters();
            roleInfo.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug("OnLevelInfoShowResponse e.data.IsEmpty", Color.yellow);
                return;
            }

            Log.Debug($"roleInfo:{roleInfo.ToString()}", Color.yellow);

            ResourcesSingleton.Instance.levelInfo.levelBox.boxStateDic.Clear();

            var tbchapter = ConfigManager.Instance.Tables.Tbchapter;


            //拿到解锁的最大levelID 第一关默认解锁
            int maxLcokChapterID = tbchapter.Get(1).id;
            int maxPassChapterID = 0;
            for (int i = 0; i < roleInfo.ChapterList.Count; i++)
            {
                Log.Debug($"{roleInfo.ChapterList[i]}", Color.cyan);
                if (isPass(roleInfo.ChapterList[i].ChapterStatus))
                {
                    if (roleInfo.ChapterList[i].ChapterId > maxLcokChapterID)
                    {
                        maxLcokChapterID = roleInfo.ChapterList[i].ChapterId;
                    }
                }
            }

            for (int i = 0; i < roleInfo.ChapterList.Count; i++)
            {
                Log.Debug($"{roleInfo.ChapterList[i]}", Color.cyan);
                if (isPass(roleInfo.ChapterList[i].ChapterPass))
                {
                    if (roleInfo.ChapterList[i].ChapterId > maxPassChapterID)
                    {
                        maxPassChapterID = roleInfo.ChapterList[i].ChapterId;
                    }
                }
            }

            Log.Debug($"{maxLcokChapterID},pass:{maxPassChapterID}", Color.cyan);

            int minNoPassChpterID = maxPassChapterID + 1;

            for (int i = 0; i < roleInfo.ChapterList.Count; i++)
            {
                //Log.Debug($"level{roleInfo.ChapterList[i]}", Color.green);
                if (roleInfo.ChapterList[i].ChapterId == minNoPassChpterID)
                {
                    ResourcesSingleton.Instance.levelInfo.maxUnLockChapterSurviveTime =
                        (int)roleInfo.ChapterList[i].MaxLiveTime;
                    Log.Debug(
                        $"maxLcokLevelID{maxLcokChapterID},minNoPassID{minNoPassChpterID},overTime{roleInfo.ChapterList[i].MaxLiveTime}",
                        Color.cyan);
                    break;
                }
            }

            //var tbchapter = ConfigManager.Instance.Tables.Tbchapter;
            //ResourcesSingleton.Instance.levelInfo.levelId = tbchapter.Get(maxLcokChapterID).levelId;
            ResourcesSingleton.Instance.levelInfo.maxUnLockChapterID = maxLcokChapterID;
            ResourcesSingleton.Instance.levelInfo.maxPassChapterID = maxPassChapterID;


            //拿到最小未解锁的boxID
            int minNotLockBoxID = 2999;
            //拿到最小未领取的boxID
            int minNoGetBoxID = 2999;
            for (int i = 0; i < roleInfo.ChapterList.Count; i++)
            {
                for (int j = 0; j < roleInfo.ChapterList[i].BoxStatusList.Count; j++)
                {
                    string boxInput = roleInfo.ChapterList[i].BoxStatusList[j];
                    //  Log.Debug(boxInput,Color.blue);
                    if (!boxInput.Contains("["))
                    {
                        continue;
                    }

                    int startIndex = boxInput.IndexOf("[") + 1;
                    int endIndex = boxInput.IndexOf("]", startIndex);
                    string result = boxInput.Substring(startIndex, endIndex - startIndex);
                    Log.Debug(
                        $"roleInfo.ChapterList[{i}].BoxStatusList[{j}]:{roleInfo.ChapterList[i].BoxStatusList[j]}",
                        Color.yellow);
                    var boxArray = result.Split(':');

                    if (boxArray[2] == "false" ? true : false)
                    {
                        // Debug.Log($"{boxArray[0]}");
                        if (minNoGetBoxID > int.Parse(boxArray[0]))
                        {
                            minNoGetBoxID = int.Parse(boxArray[0]);
                        }
                    }

                    if (boxArray[1] == "lock" ? true : false)
                    {
                        if (minNotLockBoxID > int.Parse(boxArray[0]))
                        {
                            minNotLockBoxID = int.Parse(boxArray[0]);
                        }
                    }
                }
            }

            Log.Debug(
                $"minNotLockBoxID：{minNotLockBoxID},minNotGetBoxID:{minNoGetBoxID}", Color.blue);

            ResourcesSingleton.Instance.levelInfo.levelBox.minNotLockBoxID = minNotLockBoxID;
            ResourcesSingleton.Instance.levelInfo.levelBox.minNotGetBoxID = minNoGetBoxID;
            for (int i = 0; i < roleInfo.ChapterList.Count; i++)
            {
                for (int j = 0; j < roleInfo.ChapterList[i].BoxStatusList.Count; j++)
                {
                    string boxInput = roleInfo.ChapterList[i].BoxStatusList[j];
                    //  Log.Debug(boxInput,Color.blue);
                    if (!boxInput.Contains("["))
                    {
                        continue;
                    }

                    int startIndex = boxInput.IndexOf("[") + 1;
                    int endIndex = boxInput.IndexOf("]", startIndex);
                    string result = boxInput.Substring(startIndex, endIndex - startIndex);
                    Log.Debug(
                        $"roleInfo.ChapterList[{i}].BoxStatusList[{j}]:{roleInfo.ChapterList[i].BoxStatusList[j]}",
                        Color.yellow);
                    var boxArray = result.Split(':');
                    if (int.Parse(boxArray[0]) < minNotLockBoxID)
                    {
                        ResourcesSingleton.Instance.levelInfo.levelBox.boxStateDic.Add(int.Parse(boxArray[0]),
                            boxArray[2] == "false" ? false : true);
                    }
                }
            }

            var keys = ResourcesSingleton.Instance.levelInfo.levelBox.boxStateDic.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                Log.Debug(
                    $"boxStateDic：key:{keys[i]},value:{ResourcesSingleton.Instance.levelInfo.levelBox.boxStateDic[keys[i]]}",
                    Color.blue);
            }


            var curBlockId = tbchapter.Get(maxLcokChapterID).blockId;
            ResourcesSingleton.Instance.levelInfo.maxMainBlockID = curBlockId;

            //TODO:
            // ResourcesSingleton.Instance.levelInfo.maxMainBlockID = 2;
            // ResourcesSingleton.Instance.levelInfo.maxUnLockChapterID = 11;
            // ResourcesSingleton.Instance.levelInfo.maxPassChapterID = 10;

            Log.Debug($"curBlockId {curBlockId}", Color.red);
            if (JsonManager.Instance.userData.chapterId == 0)
            {
                JsonManager.Instance.userData.chapterId = ResourcesSingleton.Instance.levelInfo.maxUnLockChapterID + 1;
                JsonManager.Instance.userData.blockId = ResourcesSingleton.Instance.levelInfo.maxMainBlockID + 1;
                JsonManager.Instance.userData.lastChapterId = ResourcesSingleton.Instance.levelInfo.maxUnLockChapterID;
            }

            JsonManager.Instance.userData.chapterId = Mathf.Clamp(JsonManager.Instance.userData.chapterId, 1,
                ResourcesSingleton.Instance.levelInfo.maxUnLockChapterID + 1);
            JsonManager.Instance.userData.blockId = Mathf.Clamp(JsonManager.Instance.userData.blockId, 1,
                ResourcesSingleton.Instance.levelInfo.maxMainBlockID + 1);
            JsonManager.Instance.userData.lastChapterId = Mathf.Clamp(JsonManager.Instance.userData.lastChapterId, 1,
                ResourcesSingleton.Instance.levelInfo.maxUnLockChapterID);


            JsonManager.Instance.SavePlayerData(JsonManager.Instance.userData);
            // if (JsonManager.Instance.userData.chapterId == 0)
            // {
            //     JsonManager.Instance.userData.lastChapterId = ResourcesSingleton.Instance.levelInfo.maxUnLockChapterID;
            // }
            // else
            // {
            //     JsonManager.Instance.userData.chapterId = ResourcesSingleton.Instance.levelInfo.maxUnLockChapterID + 1;
            //     JsonManager.Instance.userData.blockId = ResourcesSingleton.Instance.levelInfo.maxMainBlockID + 1;
            // }
            // if (JsonManager.Instance.userData.chapterId == 0)
            // {
            //     JsonManager.Instance.userData.chapterId = ResourcesSingleton.Instance.levelInfo.maxUnLockChapterID + 1;
            //     JsonManager.Instance.userData.blockId = ResourcesSingleton.Instance.levelInfo.maxMainBlockID + 1;
            //     JsonManager.Instance.userData.lastChapterId = ResourcesSingleton.Instance.levelInfo.maxUnLockChapterID;
            // }
            // else
            // {
            //     JsonManager.Instance.userData.chapterId = ResourcesSingleton.Instance.levelInfo.maxUnLockChapterID + 1;
            //     JsonManager.Instance.userData.blockId = ResourcesSingleton.Instance.levelInfo.maxMainBlockID + 1;
            // }


            //JsonManager.Instance.userData.lastChapterId = Mathf.Max(JsonManager.Instance.userData.lastChapterId, 1);


            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Battle, out UI ui))
            {
                var ui1 = ui as UIPanel_Battle;
                //ui1.SetChapterBoxInfo(roleInfo);
                //ui1.UpdateLevelShow();
            }
            //if (roleInfo.ChapterList != null)
            //{
            //    //ac.Value.MonopolyRecord.MonopolyGridNum;
            //    //Log.Debug($"大富翁");
            //    var monopoly = tbmonopoly.Get(activity.link);
            //    var m_RedDotName = NodeNames.GetTagFuncRedDotName(monopoly.tagFunc);
            //    for (int i = 0; i < 2; i++)
            //    {
            //        var itemStr = $"{m_RedDotName}|Pos{i}";
            //        RedDotManager.Instance.InsterNode(itemStr);
            //    }

            //    var taskListStr =
            //        $"{m_RedDotName}|Pos1|Task";
            //    RedDotManager.Instance.InsterNode(taskListStr);

            //    NetWorkManager.Instance.SendMessage(CMD.QUERYACTIVITYTASK, new IntValue
            //    {
            //        Value = ac.Key
            //    });
            //}
        }

        public bool isPass(string status)
        {
            return status == "true" ? true : false;
        }

        private void OnFirstChargeChangeResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(99, 6, OnFirstChargeChangeResponse);
            WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYCHARGE, OnFirstChargeResponse);
            NetWorkManager.Instance.SendMessage(CMD.QUERYCHARGE);
        }

        private void OnQueryTalentResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(2, 10, OnQueryTalentResponse);
            ResourcesSingleton.Instance.talentID.talentPropID = 0;
            ResourcesSingleton.Instance.talentID.talentSkillID = 0;
            var tanlentInfo = new IntValueList();
            tanlentInfo.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);

                return;
            }

            var count = tanlentInfo.Values.Count;

            //for (int i = 0; i < tanlentInfo.TalentIds.Count; i++)
            //{
            //    Log.Debug(tanlentInfo.TalentIds[i].ToString(), Color.yellow);
            //}
            if (count > 0)
            {
                foreach (var item in tanlentInfo.Values)
                {
                    Log.Debug($"id00:{item}", Color.cyan);
                }

                if (tanlentInfo.Values[0] / 10000 == 2)
                {
                    ResourcesSingleton.Instance.talentID.talentSkillID = tanlentInfo.Values[0];
                }
                else
                {
                    ResourcesSingleton.Instance.talentID.talentPropID = tanlentInfo.Values[0];
                }
            }

            if (count > 1)
            {
                foreach (var item in tanlentInfo.Values)
                {
                    Log.Debug($"id:{item}", Color.cyan);
                }

                ResourcesSingleton.Instance.talentID.talentPropID = tanlentInfo.Values[0];
                ResourcesSingleton.Instance.talentID.talentSkillID = tanlentInfo.Values[1];
            }

            //if (JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out UI ui))
            //{
            //    var jiyu = ui as UIPanel_JiyuGame;
            //    //isTalentRequest = true;
            //    jiyu.RefreshTalentRedDot();
            //}
        }

        private void OnQueryWearingEquipResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            //Log.Error($"接收到后端数据,刷新装备缓存数据222", Color.green);
            //WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYWEAR, OnQueryWearingEquipResponse);
            ResourcesSingleton.Instance.equipmentData.isWearingEquipments.Clear();
            ByteValueList gameEquips = new ByteValueList();
            gameEquips.MergeFrom(e.data);
            // if (e.data.IsEmpty)
            // {
            //     Log.Debug("OnQueryWearingEquipResponse.IsEmpty", Color.red);
            //     //return;
            // }

            var equip_dataConfig = ConfigManager.Instance.Tables.Tbequip_data;

            foreach (var gameEquip in gameEquips.Values)
            {
                EquipDto equip = new EquipDto();
                equip.MergeFrom(gameEquip);
                int equipId = equip.EquipId * 100 + equip.Quality;
                var equip_data = equip_dataConfig.Get(equipId);
                MyGameEquip myGameEquip = new MyGameEquip
                {
                    equip = equip,
                    isWearing = true,
                    canCompound = false
                };
                ResourcesSingleton.Instance.equipmentData.isWearingEquipments.TryAdd(equip_data.posId, myGameEquip);
                if (ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(equip.PartId, out var wearEquip))
                {
                    wearEquip.isWearing = true;
                }


                Log.Debug($"wearequip{equip.ToString()}", Color.cyan);
            }

            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui))
            {
                var uiequip = ui as UIPanel_Equipment;

                var lastId = uiequip.lastModuleId;
                uiequip.lastModuleId = -1;
                uiequip.OnClickActionEvent(lastId, false);
                uiequip.RefreshAllWearEquip();
            }
        }

        private void OnFirstChargeResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYCHARGE, OnFirstChargeResponse);
            IntValue intValue = new IntValue();
            intValue.MergeFrom(e.data);
            Log.Debug($"OnFirstChargeResponse:{intValue.Value}", Color.green);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            ResourcesSingleton.Instance.firstChargeInt = intValue.Value;
        }

        private void OnQueryMailResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYMAIL, OnQueryMailResponse);

            var resultMailList = new ResultMailList();
            resultMailList.MergeFrom(e.data);

            ResourcesSingleton.Instance.mails.Clear();

            if (isInit)
            {
                isInit = false;
                ResourcesSingleton.Instance.mails.Add(new MailInfo
                {
                    Id = UnityEngine.Random.Range(10, 9999999),
                    MailModuleId = 30001,
                    UserId = 0,
                    RoleId = 0,
                    Status = 0,
                    SendTime = 1699260000,
                    ExpireDate = 1699265918
                });
                ResourcesSingleton.Instance.mails.Add(new MailInfo
                {
                    Id = UnityEngine.Random.Range(10, 9999999),
                    MailModuleId = 10002,
                    UserId = 0,
                    RoleId = 0,
                    Status = 0,
                    SendTime = 1699260000,
                    ExpireDate = 1728463238
                });
                ResourcesSingleton.Instance.mails.Add(new MailInfo
                {
                    Id = UnityEngine.Random.Range(10, 9999999),
                    MailModuleId = 30002,
                    UserId = 0,
                    RoleId = 0,
                    Status = 0,
                    SendTime = 1699260000,
                    ExpireDate = 1728431238
                });
                ResourcesSingleton.Instance.mails.Add(new MailInfo
                {
                    Id = UnityEngine.Random.Range(10, 9999999),
                    MailModuleId = 10002,
                    UserId = 0,
                    RoleId = 0,
                    Status = 0,
                    SendTime = 1699260000,
                    ExpireDate = 1728431238
                });
                ResourcesSingleton.Instance.mails.Add(new MailInfo
                {
                    Id = UnityEngine.Random.Range(10, 9999999),
                    MailModuleId = 20002,
                    UserId = 0,
                    RoleId = 0,
                    Status = 0,
                    SendTime = 1699260000,
                    ExpireDate = 17284363238
                });
                ResourcesSingleton.Instance.mails.Add(new MailInfo
                {
                    Id = UnityEngine.Random.Range(10, 9999999),
                    MailModuleId = 30004,
                    UserId = 0,
                    RoleId = 0,
                    Status = 0,
                    SendTime = 1699260000,
                    ExpireDate = 1728463238
                });
            }


            foreach (var mailInfo in resultMailList.MailInfoList)
            {
                ResourcesSingleton.Instance.mails.Add(mailInfo);
                Log.Debug($"{mailInfo.ToString()}", Color.green);
            }

            ResourcesSingleton.Instance.mails.Sort(new MailInfosComparer());


            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Mail, out UI ui))
            {
                Log.Debug($"刷新Mail界面", Color.green);
                UIHelper.Remove(UIType.UIPanel_Mail);
                UIHelper.CreateAsync(UIType.UIPanel_Mail).Forget();
            }


            Log.Debug($"UpdateResourceUI", Color.green);
            ResourcesSingleton.Instance.UpdateResourceUI();
        }

        void GetActivityId(int tagFunId)
        {
        }

        async UniTaskVoid HandleRedDot(int tag_funcId)
        {
            var tagFunName = NodeNames.GetTagFuncRedDotName(tag_funcId);
            await UniTask.Delay(1000);
            if (RedDotManager.Instance.GetRedPointCnt(tagFunName) == 0)
                RedDotManager.Instance.SetRedPointCnt(tagFunName, 1);
        }

        private void OnPrePayResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            StringValue stringValue = new StringValue();
            stringValue.MergeFrom(e.data);
            Log.Debug($"OnPrePayResponse:{stringValue}", Color.green);
            if (e.data.IsEmpty)
            {
                return;
            }

            //TODO:接入第三方支付
            var tempUrl = $"http://192.168.2.29:9001/pay/wxPay";

            string jsonData = JsonConvert.SerializeObject(new ResInfo
            {
                orderId = stringValue.Value,
                status = 1
            });

            // 创建 UnityWebRequest，指定为 POST 请求，并将 JSON 数据添加到请求体
            UnityWebRequest www = new UnityWebRequest(tempUrl, "POST");

            // 将数据设置到请求体中
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

            www.uploadHandler = new UploadHandlerRaw(jsonToSend);

            // 设置响应类型为 JSON
            www.SetRequestHeader("Content-Type", "application/json");

            // 使用 UnityWebRequest 的 DownloadHandler 来处理返回的数据
            //www.downloadHandler = new DownloadHandlerBuffer();

            www.SendWebRequest();
        }

        async void OnBoardCastPaymentResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            StringValue stringListValue = new StringValue();
            stringListValue.MergeFrom(e.data);
            Log.Debug($"OnBoardCastPaymentResponse:{stringListValue}", Color.green);
            if (e.data.IsEmpty)
            {
                return;
            }

            //错误码
            if (stringListValue.Value.Contains("ResponseStatus:"))
            {
                var errcodeStr = stringListValue.Value.Replace("ResponseStatus:", "");
                int errcode = int.Parse(errcodeStr);
                ErrorMsg.LogErrorMsg(errcode);
                return;
            }

            string str = $"OnShopResponse";
            JiYuEventManager.Instance.TriggerEvent(str, stringListValue.Value);


            var tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            var rewards = JiYuUIHelper.TurnShopBoardCastStrReward2List(stringListValue.Value);
            if (rewards.Count > 0)
            {
                var ui = await UIHelper.CreateAsync(UIType.UICommon_Reward, rewards);
            }
            else
            {
                JiYuUIHelper.ClearCommonResource();
                var ui = await UIHelper.CreateAsync(UIType.UICommon_Resource,
                    tblanguage.Get($"text_buy_success").current);
            }
        }

        async void OnBoardCastUpdateFuncTaskResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            //TODO:根据模块id更新对应

            var intValueList = new IntValueList();
            intValueList.MergeFrom(e.data);
            Log.Debug($"OnBoardCastUpdateFuncTaskResponse {intValueList}", Color.cyan);
            foreach (var intValue in intValueList.Values)
            {
                HandleRedDot(intValue).Forget();
                // switch (intValue)
                // {
                //     case 3301:
                //         //23:活动类型
                //         if (!JiYuUIHelper.TryGetActivityLink(21, out var activityId, out var link))
                //         {
                //             return;
                //         }
                //
                //         Log.Debug($"查询 {intValue} 所属的活动接口", Color.green);
                //
                //         // NetWorkManager.Instance.SendMessage(CMD.QUERTSINGLEACTIVITY, new IntValue
                //         // {
                //         //     Value = activityId
                //         // });
                //         break;
                // }
                //var intdd = new IntValue();
                //intdd.MergeFrom(intValue);
            }

            // WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYMAIL, OnQueryMailResponse);
            // NetWorkManager.Instance.SendMessage(CMD.QUERYMAIL);
        }

        void OnBoardCastMail(object sender, WebMessageHandlerOld.Execute e)
        {
            Log.Debug($"OnBoardCastMail", Color.cyan);
            WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYMAIL, OnQueryMailResponse);
            NetWorkManager.Instance.SendMessage(CMD.QUERYMAIL);
        }

        /// <summary>
        /// 弃用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnInitPlayerPropertyBoardResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            var stringValue = new StringValue();
            stringValue.MergeFrom(e.data);
            Log.Debug($"OnInitPlayerPropertyBoardResponse{stringValue.Value}", Color.red);
            //return;

            var battleProperty = Newtonsoft.Json.JsonConvert.DeserializeObject<BattleProperty>(stringValue.Value);
            // var battleProperty = new BattleProperty();
            // battleProperty.MergeFrom(e.data);

            Log.Debug("OnInitPlayerPropertyBoardResponse", Color.red);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            var playerData = new PlayerData();
            var chaStats = new ChaStats();
            Log.Debug($"{battleProperty.Properties}", Color.green);
            JiYuUIHelper.InitPlayerProperty(ref playerData, ref chaStats, battleProperty);

            ResourcesSingleton.Instance.playerProperty.playerData = playerData;
            ResourcesSingleton.Instance.playerProperty.chaProperty = chaStats.chaProperty;
            JiYuUIHelper.RefreshPlayerPropertyEquipPanelUI();
        }


        void OnGetMainPropertyResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            var battleProperty = new BattleProperty();
            battleProperty.MergeFrom(e.data);

            Log.Debug("OnGetMainPropertyResponse", Color.green);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            Log.Debug($"OnGetMainPropertyResponse:{battleProperty.Properties}", Color.green);
            ResourcesSingleton.Instance.mainProperty.Clear();
            foreach (var property in battleProperty.Properties)
            {
                var strings = property.Split(";");
                var id = int.Parse(strings[0]);
                var value = int.Parse(strings[1]);
                ResourcesSingleton.Instance.mainProperty.Add(id, value);
            }

            foreach (var kv in ResourcesSingleton.Instance.mainProperty)
            {
                Log.Debug($"mainProperty:{kv.Key} : {kv.Value}");
            }

            //挪到装备获取后
            JiYuUIHelper.RefreshPlayerPropertyEquipPanelUI();
            //ResourcesSingleton.Instance.mainProperty = battleProperty;
        }

        /// <summary>
        /// 弃用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnInitPlayerPropertyResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            // var stringValue = new StringValue();
            // stringValue.MergeFrom(e.data);
            // Log.Debug($"OnInitPlayerPropertyResponse{stringValue.Value}", Color.red);
            //return;

            //var battleProperty = Newtonsoft.Json.JsonConvert.DeserializeObject<BattleProperty>(stringValue.Value);
            var battleProperty = new BattleProperty();
            battleProperty.MergeFrom(e.data);

            Log.Debug("OnInitPlayerPropertyResponse", Color.red);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            var playerData = new PlayerData();
            var chaStats = new ChaStats();
            Log.Debug($"{battleProperty.Properties}", Color.green);
            JiYuUIHelper.InitPlayerProperty(ref playerData, ref chaStats, battleProperty);

            ResourcesSingleton.Instance.playerProperty.playerData = playerData;
            ResourcesSingleton.Instance.playerProperty.chaProperty = chaStats.chaProperty;
            JiYuUIHelper.RefreshPlayerPropertyEquipPanelUI();
        }

        void OnOpenBagPanelResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.OPENBAG, OnOpenBagPanelResponse);
            var valueList = new ByteValueList();
            valueList.MergeFrom(e.data);

            ResourcesSingleton.Instance.items.Clear();
            Log.Debug($"背包信息", Color.red);
            foreach (var bagItem in valueList.Values)
            {
                var bag = new BagItem();
                bag.MergeFrom(bagItem);
                Log.Debug($"{bag}", Color.red);
                if (bag.Count <= 0)
                {
                    continue;
                }

                ResourcesSingleton.Instance.items.TryAdd(bag.ItemId, bag.Count);
            }

            Tbitem item = ConfigManager.Instance.Tables.Tbitem;


            var bagList = new List<BagItem>();
            foreach (var VARIABLE in ResourcesSingleton.Instance.items)
            {
                bagList.Add(new BagItem
                {
                    ItemId = VARIABLE.Key,
                    Count = VARIABLE.Value
                });
            }

            //TODO:假数据
            // foreach (var VARIABLE in item.DataList)
            // {
            //     bagList.Add(new BagItem
            //     {
            //         ItemId = VARIABLE.id,
            //         Count = 10
            //     });
            // }

            bagList.Sort(new BagItemComparer());
            foreach (var VARIABLE in bagList)
            {
                if (!ResourcesSingleton.Instance.items.TryAdd(VARIABLE.ItemId, VARIABLE.Count))
                {
                    //Log.Debug($"{VARIABLE.ItemId}", Color.cyan);
                }
            }

            bagList.Clear();
        }

        async void OnOpenMainPanelResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            //WebMessageHandlerOld.Instance.RemoveHandler(CMD.INITPLAYER, OnOpenMainPanelResponse);
            var gameRole = new GameRole();
            gameRole.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            //var tbuserLevel = ConfigManager.Instance.Tables.Tbuser_level;
            // ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy = gameRole.RoleAssets.Energy;
            // ResourcesSingleton.Instance.playerResources.maxEnergy = gameRole.RoleAssets.EnergyMax;
            // ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin = gameRole.RoleAssets.Bitcoin;
            // ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill = gameRole.RoleAssets.UsBill;
            // ResourcesSingleton.Instance.UserInfo.RoleAssets.Level = gameRole.RoleAssets.Level;
            // ResourcesSingleton.Instance.UserInfo.RoleAssets.Exp = gameRole.RoleAssets.Exp;

            #region 黄金国新增

            //gameRole.RoleAssets.Level = 2;
            ResourcesSingleton.Instance.UserInfo = gameRole;
            //Log.Debug($"EnergyCountdown{gameRole.RoleAssets.EnergyCountdown}",Color.cyan);

            #endregion

            //gameRole.RoleAssets.
            ResourcesSingleton.Instance.UpdateResourceUI();

            //ResourcesSingleton.Instance.SetJiYuGamePanel(uiType);


            Log.Debug($"{gameRole}", Color.red);
        }


        private void OnQueryBankResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYBANK, OnQueryBankResponse);
            var gameBank = new GoldPig();
            gameBank.MergeFrom(e.data);
            Debug.Log("menu scene game bank:" + gameBank);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            gameBank.PigLevel = gameBank.PigLevel % 100;
            ResourcesSingleton.Instance.goldPig = gameBank;
        }

        private void OnGiftChangeResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            ByteValueList byteValueList = new ByteValueList();
            byteValueList.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            foreach (var item in byteValueList.Values)
            {
                IndexModule indexModule = new IndexModule();
                indexModule.MergeFrom(item);
                Debug.Log(indexModule);

                if (ResourcesSingleton.Instance.shopMap.IndexModuleMap.ContainsKey(indexModule.ModuleId))
                {
                    ResourcesSingleton.Instance.shopMap.IndexModuleMap[indexModule.ModuleId] = indexModule;
                }
                else
                {
                    ResourcesSingleton.Instance.shopMap.IndexModuleMap.Add(indexModule.ModuleId, indexModule);
                }
                //List<long> GiftHelp = new List<long>();
                //Dictionary<int, List<long>> ModuleHelp = new Dictionary<int, List<long>>();

                //if ()
                //if (ResourcesSingleton.Instance.shopInit.shopHelpDic.ContainsKey(indexModule.ModuleId))
                //{
                //    for (int i = 0; i < indexModule.GiftInfoList.Count; i++)
                //    {
                //        int ihelp = i;
                //        if (ihelp == 0)
                //        {
                //            GiftHelp.Add(indexModule.GiftInfoList[ihelp].EndTime);
                //            GiftHelp.Add(indexModule.GiftInfoList[ihelp].CreateTime);
                //            GiftHelp.Add(indexModule.GiftInfoList[ihelp].Times);
                //            //GiftHelp.Add(gift.Times);
                //            ModuleHelp.Add(indexModule.GiftInfoList[ihelp].GiftId, GiftHelp);
                //            ResourcesSingleton.Instance.shopInit.shopHelpDic.Add(indexModule.ModuleId, ModuleHelp);
                //            GiftHelp.Clear();
                //            ModuleHelp.Clear();
                //        }
                //        else
                //        {
                //            GiftHelp.Add(indexModule.GiftInfoList[ihelp].EndTime);
                //            GiftHelp.Add(indexModule.GiftInfoList[ihelp].CreateTime);
                //            GiftHelp.Add(indexModule.GiftInfoList[ihelp].Times);
                //            ResourcesSingleton.Instance.shopInit.shopHelpDic[indexModule.ModuleId]
                //                .Add(indexModule.GiftInfoList[ihelp].GiftId, GiftHelp);
                //            GiftHelp.Clear();
                //            ModuleHelp.Clear();
                //        }
                //    }
                //}
                //else
                //{
                //    for (int i = 0; i < indexModule.GiftInfoList.Count; i++)
                //    {
                //        int ihelp = i;
                //        GiftHelp.Add(indexModule.GiftInfoList[ihelp].EndTime);
                //        GiftHelp.Add(indexModule.GiftInfoList[ihelp].CreateTime);
                //        GiftHelp.Add(indexModule.GiftInfoList[ihelp].Times);
                //        if (ResourcesSingleton.Instance.shopInit.shopHelpDic.TryGetValue(indexModule.ModuleId,
                //                out var a))
                //        {
                //            ResourcesSingleton.Instance.shopInit.shopHelpDic[indexModule.ModuleId]
                //                .Add(indexModule.GiftInfoList[ihelp].GiftId, GiftHelp);
                //        }
                //        else
                //        {
                //            Dictionary<int, List<long>> h = new Dictionary<int, List<long>>();
                //            h.Add(indexModule.GiftInfoList[ihelp].GiftId, GiftHelp);
                //            ResourcesSingleton.Instance.shopInit.shopHelpDic.Add(indexModule.ModuleId, h);
                //        }

                //        GiftHelp.Clear();
                //        ModuleHelp.Clear();
                //    }
                //}
                //ResourcesSingleton.Instance.shopInit.shopHelpDic.Get()
            }

            ResourcesSingleton.Instance.UpdateResourceUI();
        }

        private void OnShopInitResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYSHOP, OnShopInitResponse);
            var shopMap = new GameShopMap();
            shopMap.MergeFrom(e.data);
            Debug.Log(shopMap);

            //GameShop gameShop = new GameShop();
            //gameShop.MergeFrom(e.data);
            //Debug.Log(e);
            //Debug.Log(gameShop);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            ResourcesSingleton.Instance.shopMap = shopMap;
            ResourcesSingleton.Instance.UpdateResourceUI();

            //Tbshop_daily tbshop_Daily = ConfigManager.Instance.Tables.Tbshop_daily;

            //foreach (var module in gameShop.IndexModules)
            //{
            //    Dictionary<int, List<long>> ModuleHelp = new Dictionary<int, List<long>>();


            //    //box的list当中0，1位是保底次数，2位是广告次数，3位是广告倒计时，4是抽取时间
            //    foreach (var box in module.BoxInfoList)
            //    {
            //        List<long> BoxIntHelp = new List<long>();
            //        for (int i = 0; i < box.Numbers.Count; i++)
            //        {
            //            BoxIntHelp.Add(box.Numbers[i]);
            //        }

            //        if (box.Numbers.Count == 1)
            //        {
            //            BoxIntHelp.Add(0);
            //        }

            //        BoxIntHelp.Add(box.AdCount);
            //        BoxIntHelp.Add(box.SumTime);
            //        BoxIntHelp.Add(box.DrawTime);
            //        ModuleHelp.Add(box.Id, BoxIntHelp);
            //    }

            //    //充值的list中0是充值次数，1是免费次数，2是广告次数
            //    foreach (var chargeInfo in module.ChargeInfoList)
            //    {
            //        List<long> ChargeIntHelp = new List<long>();
            //        ChargeIntHelp.Add(chargeInfo.ChargeSum);
            //        ChargeIntHelp.Add(chargeInfo.FreeSum);
            //        ChargeIntHelp.Add(chargeInfo.AdSum);
            //        ModuleHelp.Add(chargeInfo.Id, ChargeIntHelp);
            //    }

            //    //礼包的list中0是限时礼包倒计时，1是礼包创建时间，2是礼包可购买次数
            //    foreach (var gift in module.GiftInfoList)
            //    {
            //        List<long> GiftHelp = new List<long>();
            //        GiftHelp.Add(gift.EndTime);
            //        GiftHelp.Add(gift.CreateTime);
            //        GiftHelp.Add(gift.Times);
            //        ModuleHelp.Add(gift.GiftId, GiftHelp);
            //    }

            //    //每日商店的list中0是标识，1是购买次数，2是更新时间
            //    foreach (var daily in module.DailyBuyList)
            //    {
            //        List<long> dailyHelp = new List<long>();
            //        dailyHelp.Add(daily.Sign);
            //        dailyHelp.Add(daily.BuyCount);
            //        dailyHelp.Add(daily.UpTime);
            //        ModuleHelp.Add(tbshop_Daily.Get(daily.Sign).pos, dailyHelp);
            //    }

            //    if (ResourcesSingleton.Instance.shopInit.shopHelpDic.TryGetValue(module.ModuleId,
            //            out Dictionary<int, List<long>> a))
            //    {
            //        ResourcesSingleton.Instance.shopInit.shopHelpDic[module.ModuleId] = ModuleHelp;
            //    }
            //    else
            //    {
            //        ResourcesSingleton.Instance.shopInit.shopHelpDic.Add(module.ModuleId, ModuleHelp);
            //    }
            //}

            //ResourcesSingleton.Instance.UpdateResourceUI();
            //await UniTask.Delay(1000 * (int)TimeHelper.GetToTomorrowTime());
            //WebMessageHandlerOld.Instance.AddHandler(11, 1, OnShopInitResponse);
            //NetWorkManager.Instance.SendMessage(11, 1);
        }

        private void OnTaskChangeResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            IntValue intValue = new IntValue();
            intValue.MergeFrom(e.data);
            //Debug.Log(e);
            Log.Debug($"OnTaskChangeResponse{intValue.Value}");
            //Debug.Log(intValue);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            //Debug.Log("100 - 1 Task change");

            // WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYDAYANDWEEKTASK, OnDayAndWeekResponse);
            // NetWorkManager.Instance.SendMessage(CMD.QUERYDAYANDWEEKTASK);

            // WebMessageHandlerOld.Instance.AddHandler(3, 4, OnAchieveResponse);
            // NetWorkManager.Instance.SendMessage(3, 4);
        }

        private void OnDayAndWeekResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYDAYANDWEEKTASK, OnDayAndWeekResponse);
            RoleTaskInfo roleTaskInfo = new RoleTaskInfo();
            roleTaskInfo.MergeFrom(e.data);

            Debug.Log($"OnDayAndWeekResponse:{roleTaskInfo}");
            if (e.data.IsEmpty)
            {
                ResourcesSingleton.Instance.dayWeekTask.tasks.Clear();
                ResourcesSingleton.Instance.dayWeekTask.boxes.Clear();
                foreach (var t in ConfigManager.Instance.Tables.Tbtask.DataList)
                {
                    ResourcesSingleton.Instance.dayWeekTask.tasks.Add(t.id, new Vector2(0, 0));
                }

                foreach (var ts in ConfigManager.Instance.Tables.Tbtask_score.DataList)
                {
                    ResourcesSingleton.Instance.dayWeekTask.boxes.Add(ts.id, new Vector2(0, 0));
                }

                Log.Debug("3-1RoleTaskInfo.data.IsEmpty", Color.red);
                return;
            }

            ResourcesSingleton.Instance.dayWeekTask.tasks.Clear();
            ResourcesSingleton.Instance.dayWeekTask.boxes.Clear();

            foreach (var t in roleTaskInfo.TaskInfoList)
            {
                //status为领取状态0未领取，1领取， para任务参数
                ResourcesSingleton.Instance.dayWeekTask.tasks.Add(t.Id, new Vector2(t.Status, t.Para));
            }

            foreach (var s in roleTaskInfo.TaskScoreList)
            {
                //status为领取状态0未领取，1领取， valid生效与否0未生效1生效
                ResourcesSingleton.Instance.dayWeekTask.boxes.Add(s.Id, new Vector2(s.Status, s.Valid));
            }

            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Battle, out UI ui))
            {
                var ui1 = ui as UIPanel_Battle;
                //ui1.RedPointSetState();
            }
        }

        private void OnAchieveResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYACHIEVE, OnAchieveResponse);
            RoleTaskInfo roleTaskInfo = new RoleTaskInfo();
            roleTaskInfo.MergeFrom(e.data);
            Log.Debug($"OnAchieveResponse {roleTaskInfo.ToString()}", Color.green);
            if (e.data.IsEmpty)
            {
                // //未连接成功空数据
                // //成就空数据
                // Dictionary<int, Vector2> helpDic = new Dictionary<int, Vector2>();
                // foreach (var t in ConfigManager.Instance.Tables.Tbtask.DataList)
                // {
                //     helpDic.Add(t.id, new Vector2(0, 0));
                // }
                //
                // ResourcesSingleton.Instance.achieve.tasks = helpDic;
                //
                // //成就等级宝箱空数据
                // Dictionary<int, Vector2> scoreDic = new Dictionary<int, Vector2>();
                // foreach (var ac in ConfigManager.Instance.Tables.Tbachieve.DataList)
                // {
                //     scoreDic.Add(ac.id, new Vector2(0, 0));
                // }
                //
                // ResourcesSingleton.Instance.achieve.boxes = scoreDic;
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            Log.Debug($"接收到成就消息", Color.green);
            ResourcesSingleton.Instance.achieve.tasks.Clear();
            ResourcesSingleton.Instance.achieve.boxes.Clear();

            foreach (var t in roleTaskInfo.TaskInfoList)
            {
                //status为领取状态0未领取，1领取， para任务参数
                ResourcesSingleton.Instance.achieve.tasks.Add(t.Id, new Vector2(t.Status, t.Para));
                //Debug.Log("t.id" + t.Id + ";t.Para = " + t.Para);
            }

            // foreach (var s in roleTaskInfo.AchieveList)
            // {
            //     //status为领取状态0未领取，1领取， 0为凑数的
            //     //Debug.Log("GameAchieve.id: " + s.Id);
            //     ResourcesSingleton.Instance.achieve.boxes.Add(s.Id, new Vector2(s.Status, 0));
            // }

            //Debug.Log("menu scene roleTaskInfo.AchieveList.count:" + roleTaskInfo.AchieveList.Count);
            Debug.Log("game achieve list cont: " + ResourcesSingleton.Instance.achieve.boxes.Count);

            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Battle, out UI ui))
            {
                var ui1 = ui as UIPanel_Battle;
                //ui1.RedPointSetState();
            }
        }

        private async void OnSignResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(12, 1, OnSignResponse);
            GameCheckIn gameCheckIn = new GameCheckIn();
            gameCheckIn.MergeFrom(e.data);
            Debug.Log(gameCheckIn);
            if (e.data.IsEmpty)
            {
                ResourcesSingleton.Instance.signData = new GameCheckIn();
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            ResourcesSingleton.Instance.signData = gameCheckIn;
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Battle, out UI ui))
            {
                var ui1 = ui as UIPanel_Battle;
                //ui1.RedPointSetState();
            }

            await UniTask.Delay(1000 * (int)TimeHelper.GetToTomorrowTime());
            WebMessageHandlerOld.Instance.AddHandler(12, 1, OnSignSecResponse);
            NetWorkManager.Instance.SendMessage(12, 1);
        }

        private async void OnSignSecResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(12, 1, OnSignSecResponse);
            GameCheckIn gameCheckIn = new GameCheckIn();
            gameCheckIn.MergeFrom(e.data);
            Debug.Log(gameCheckIn);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            ResourcesSingleton.Instance.signData = gameCheckIn;
            // if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Notice, out UI uuii))
            // {
            //
            // }
            // else
            // {
            //     if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Battle, out UI ui))
            //     {
            //         var ui1 = ui as UIPanel_Battle;
            //         ui1.RedPointSetState();
            //     }
            // }
            await UniTask.Delay(1000 * (int)TimeHelper.GetToTomorrowTime());
            WebMessageHandlerOld.Instance.AddHandler(12, 1, OnSignSecResponse);
            NetWorkManager.Instance.SendMessage(12, 1);
        }

        private void OnNoticeStatusChangeResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            JiYuUIHelper.DownloadNotice().Forget();
        }

        private async void OnQuerySettingsResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYSETTINGS, OnQuerySettingsResponse);
            SettingDate settingDate = new SettingDate();
            settingDate.MergeFrom(e.data);
            Log.Debug($"OnQuerySettingsResponse{settingDate}", Color.green);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                // StandAloneMode();
                InitSettings();

                // JsonManager.Instance.sharedData.lastLoginUserId = 0;
                // await JsonManager.Instance.SaveSharedData(JsonManager.Instance.sharedData);
                // var sceneController = Common.Instance.Get<SceneController>(); // 场景控制
                // var sceneObj = sceneController.LoadSceneAsync<Login>(SceneName.Login);
                // await SceneResManager.WaitForCompleted(sceneObj); // 等待场景加载完毕
                return;
            }

            //Log.Debug($"{settingDate}", Color.cyan);
            // var settingsData = new SettingsData
            // {
            //     quality = (GraphicQuality)settingDate.Quality,
            //     enableFx = settingDate.EnableFx,
            //     enableBgm = settingDate.EnableBgm,
            //     enableShock = settingDate.EnableShock,
            //     enableWeakEffect = settingDate.EnableWeakEffect,
            //     enableShowStick = settingDate.EnableShowStick,
            //     version = ResourcesSingleton.Instance.settingsData.version,
            //     isShared = false,
            //     currentL10N = (Tblanguage.L10N)settingDate.CurrentL10N
            // };
            //ResourcesSingleton.Instance.settingsData = settingsData;

            //settingDate.CurrentL10N = Mathf.Max(1, settingDate.CurrentL10N);


            ResourcesSingleton.Instance.settingData = settingDate;


            JsonManager.Instance.sharedData.l10N = ResourcesSingleton.Instance.settingData.CurrentL10N;
            JsonManager.Instance.SaveSharedData(JsonManager.Instance.sharedData);
            //ResourcesSingleton.Instance.settingData.GuideList.Clear();
            // //TODO:改
            // ResourcesSingleton.Instance.settingData.UnlockList.Clear();
            // var tbtag = ConfigManager.Instance.Tables.Tbtag;
            // var tbtag_func = ConfigManager.Instance.Tables.Tbtag_func;
            // foreach (var tag in tbtag.DataList)
            // {
            //     ResourcesSingleton.Instance.settingData.UnlockList.Add(tag.id);
            // }
            //
            // foreach (var tag in tbtag_func.DataList)
            // {
            //     ResourcesSingleton.Instance.settingData.UnlockList.Add(tag.id);
            // }

            InitSettings();
        }

        async protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                isInit = true;
                Log.Debug($"OnBoardCast", Color.cyan);
                WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYMAIL, OnQueryMailResponse);
                NetWorkManager.Instance.SendMessage(CMD.QUERYMAIL);
            }


            if (Input.GetKeyDown(KeyCode.T))
            {
                Log.Debug($"加一封文本邮件", Color.green);
                ResourcesSingleton.Instance.mails.Add(new MailInfo
                {
                    Id = UnityEngine.Random.Range(10, 9999999),
                    MailModuleId = 10002,
                    UserId = 0,
                    RoleId = 0,
                    Status = 0,
                    SendTime = 1699260000,
                    ExpireDate = 1699265918
                });

                var uiManager = Common.Instance.Get<UIManager>();
                if (uiManager.TryGet(UIType.UIPanel_Mail, out var ui))
                {
                    UIHelper.Remove(UIType.UIPanel_Mail);
                    await UIHelper.CreateAsync(UIType.UIPanel_Mail);
                }


                ResourcesSingleton.Instance.UpdateResourceUI();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                ResourcesSingleton.Instance.mails.Clear();

                var uiManager = Common.Instance.Get<UIManager>();
                if (uiManager.TryGet(UIType.UIPanel_Mail, out var ui))
                {
                    Log.Debug($"UIPanel_Mail", Color.cyan);
                    UIHelper.Remove(UIType.UIPanel_Mail);
                    await UIHelper.CreateAsync(UIType.UIPanel_Mail);
                }

                ResourcesSingleton.Instance.UpdateResourceUI();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
            }

            base.OnUpdate();
        }


        public void HandleEvent(int args)
        {
            //Log.Debug($"TestEvent {args}");
        }

        public void HandleEvent(float args)
        {
        }


        protected override void OnDestroy()
        {
            Log.Debug("离开Menu场景");
            //cts.Cancel();
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.BOARDCASTMAIL, OnBoardCastMail);
            WebMessageHandlerOld.Instance.RemoveHandler(99, 2, OnGiftChangeResponse);
            WebMessageHandlerOld.Instance.RemoveHandler(99, 3, OnNoticeStatusChangeResponse);
            WebMessageHandlerOld.Instance.RemoveHandler(99, 6, OnFirstChargeChangeResponse);
            WebMessageHandlerOld.Instance.RemoveHandler(100, 1, OnTaskChangeResponse);
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYWEAR, OnQueryWearingEquipResponse);
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYEQUIP, OnQueryEquipResponse);
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYPROPERTY, OnInitPlayerPropertyResponse);
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.INITPLAYER, OnOpenMainPanelResponse);
            base.OnDestroy();
        }

        // 虚拟的第三方返回的结果对象（由前端现在模拟调用）后删
        [Serializable]
        public class ResInfo
        {
            /**
             * 订单
             */
            public string orderId;

            /**
             * 支付结果 1 成功 2 失败
             */
            public int status;
        }

        private class MailInfosComparer : IComparer<MailInfo>
        {
            public int Compare(MailInfo obj1, MailInfo obj2)
            {
                if (obj1.SendTime > obj2.SendTime)
                    return -1;
                else if (obj1.SendTime < obj2.SendTime)
                    return 1;

                if (obj1.MailModuleId < obj2.MailModuleId)
                    return -1;
                else if (obj1.MailModuleId > obj2.MailModuleId)
                    return 1;

                return 0;
            }
        }

        private class BagItemComparer : IComparer<BagItem>
        {
            public int Compare(BagItem obj1, BagItem obj2)
            {
                Tbitem item = ConfigManager.Instance.Tables.Tbitem;
                var item1 = item.GetOrDefault(obj1.ItemId);
                var item2 = item.GetOrDefault(obj2.ItemId);
                if (item1 == null || item2 == null)
                {
                    return 0;
                }

                var sort1 = item1.sort;
                var sort2 = item2.sort;
                if (sort1 < sort2)
                    return -1;
                else if (sort1 > sort2)
                    return 1;

                if (obj1.ItemId < obj2.ItemId)
                    return -1;
                else if (obj1.ItemId > obj2.ItemId)
                    return 1;

                return 0;
            }
        }
    }
}