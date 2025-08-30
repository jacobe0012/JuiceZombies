//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HotFix_UI;
using Newtonsoft.Json;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Networking;


namespace XFramework
{
    [UIEvent(UIType.UIPanel_Login)]
    internal sealed class UIPanel_LoginEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Login;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Mid;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Login>();
        }
    }

    public partial class UIPanel_Login : UI, IAwake
    {
        public void Initialize()
        {
            Init().Forget();
        }

        async UniTaskVoid Init()
        {
            var KInputTextField = GetFromReference(UIPanel_Login.KInputTextField);
            var KLoginBtn = GetFromReference(UIPanel_Login.KLoginBtn);
            var KIsStandAlone = GetFromReference(UIPanel_Login.KIsStandAlone);
            var KInputText = GetFromReference(UIPanel_Login.KInputText);
            var KBtnText = GetFromReference(UIPanel_Login.KBtnText);
            var KIsGuide = GetFromReference(UIPanel_Login.KIsGuide);
            var global = Common.Instance.Get<Global>();

            //Game.Instance.Restart();


            var language = ConfigManager.Instance.Tables.Tblanguage;


            // WebMessageHandlerOld.Instance.AddHandler(CMDOld.LOGIN, OnLoginResponse);
            WebMsgHandler.Instance.AddHandler(CMD.Auth.C2S_LOGIN, C2S_LOGINResponse);
            KBtnText.GetTextMeshPro().SetTMPText(language.Get("common_state_confirm").current);


            KIsStandAlone.GetToggle().OnValueChanged.Add((isOn) => { global.isStandAlone = isOn; });
            KIsGuide.GetToggle().OnValueChanged.Add((isOn) => { global.isIntroGuide = isOn; });
            //KIsStandAlone.GetToggle().SetIsOn(false);
            //global.isStandAlone = true;
            KInputTextField.SetActive(false);
            KLoginBtn.SetActive(false);
            KIsStandAlone.SetActive(false);
            KLoginBtn.GetXButton().RemoveAllListeners();
            var sharedData = await JsonManager.Instance.LoadSharedData();
            //TODO:
            sharedData.lastLoginUserId = 0;
            if (sharedData.lastLoginUserId == 0)
            {
                //TODO:
                sharedData.quickLoginUserIds.Clear();
                if (sharedData.quickLoginUserIds.Count > 0)
                {
                    //TODO:选择账号弹窗：

                    //TODO:登录弹窗：
                    KInputTextField.SetActive(true);
                    KLoginBtn.SetActive(true);
                    KIsStandAlone.SetActive(true);


                    UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KLoginBtn,
                        () =>
                        {
                            global.isStandAlone = KIsStandAlone.GetToggle().IsOn;
                            if (!global.isStandAlone)
                            {
                                var userName = UnicornUIHelper.HandleStr(KInputText.GetTextMeshPro().Content);
                                Log.Debug($"KInputText:{userName}");
                                UnicornUIHelper.LoginRequest(1, "", userName);
                            }
                            else
                            {
                                Log.Error($"进入单机模式");

                                //NetWorkManager.Instance.Close();
                                var gameUser = new GameUserOld
                                {
                                    Id = -1,
                                    UserName = "单机账号001",
                                    UserPass = "",
                                    SourceTable = "",
                                    PrivateKey = ""
                                };
                                var userName = UnicornUIHelper.HandleStr(gameUser.UserName);
                                var data = JsonManager.Instance.LoadPlayerData(gameUser.Id, userName);


                                var sceneController = Common.Instance.Get<SceneController>(); // 场景控制
                                var sceneObj = sceneController.LoadSceneAsync<MenuScene>(SceneName.UIMenu);
                                SceneResManager.WaitForCompleted(sceneObj).ToCoroutine(); // 等待场景加载完毕
                            }
                        });
                }
                else
                {
                    //TODO:登录弹窗：
                    KInputTextField.SetActive(true);
                    KLoginBtn.SetActive(true);
                    KIsStandAlone.SetActive(true);


                    UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KLoginBtn,
                        () =>
                        {
                            global.isStandAlone = KIsStandAlone.GetToggle().IsOn;
                            if (!global.isStandAlone)
                            {
                                var userName = UnicornUIHelper.HandleStr(KInputText.GetTextMeshPro().Content);
                                Log.Debug($"KInputText:{userName}");
                                UnicornUIHelper.LoginRequest(1, "", userName);
                            }
                            else
                            {
                                Log.Error($"进入单机模式");

                                //NetWorkManager.Instance.Close();
                                var gameUser = new GameUserOld
                                {
                                    Id = -1,
                                    UserName = "单机账号001",
                                    UserPass = "",
                                    SourceTable = "",
                                    PrivateKey = ""
                                };

                                var userName = UnicornUIHelper.HandleStr(gameUser.UserName);

                                var data = JsonManager.Instance.LoadPlayerData(gameUser.Id, userName);

                                var sceneController = Common.Instance.Get<SceneController>(); // 场景控制
                                var sceneObj = sceneController.LoadSceneAsync<MenuScene>(SceneName.UIMenu);
                                SceneResManager.WaitForCompleted(sceneObj).ToCoroutine(); // 等待场景加载完毕
                            }
                        });
                }
            }
            else if (sharedData.lastLoginUserId == -1)
            {
                //TODO:单机模式
                Log.Error($"进入单机模式");
                //NetWorkManager.Instance.Close();
                global.isStandAlone = true;
                var gameUser = new GameUserOld
                {
                    Id = -1,
                    UserName = "单机账号001",
                    UserPass = "",
                    SourceTable = "",
                    PrivateKey = ""
                };

                var userName = UnicornUIHelper.HandleStr(gameUser.UserName);
                var data = JsonManager.Instance.LoadPlayerData(gameUser.Id, userName);

                var sceneController = Common.Instance.Get<SceneController>(); // 场景控制
                var sceneObj = sceneController.LoadSceneAsync<MenuScene>(SceneName.UIMenu);
                SceneResManager.WaitForCompleted(sceneObj).ToCoroutine(); // 等待场景加载完毕
            }
            else
            {
                //sharedData.quickLoginUserIds
                var playerData = await JsonManager.Instance.LoadPlayerData(sharedData.lastLoginUserId);
                Log.Debug($"privateKey:{playerData.privateKey}");
                UnicornUIHelper.LoginRequest(2, playerData.privateKey, playerData.nickName);
            }

            GetLocationInfoNew();
            //Debug.Log($"GetLocalIPAddress:{}");
            //global.isStandAlone =
        }

        async void OnQueryLoginSettingsResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.QUERYSETTINGS, OnQueryLoginSettingsResponse);
            SettingDate settingDate = new SettingDate();
            settingDate.MergeFrom(e.data);
            Log.Debug($"OnQueryLoginSettingsResponse{settingDate}", Color.green);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);

                return;
            }


            ResourcesSingletonOld.Instance.settingData = settingDate;

            var sceneController = Common.Instance.Get<SceneController>(); // 场景控制
            var sceneObj = sceneController.LoadSceneAsync<IntroGuide>(SceneName.IntroGuide);
            SceneResManager.WaitForCompleted(sceneObj).ToCoroutine(); // 等待场景加载完毕
            // JsonManager.Instance.sharedData.l10N = ResourcesSingletonOld.Instance.settingData.CurrentL10N;
            // JsonManager.Instance.SaveSharedData(JsonManager.Instance.sharedData);
            //ResourcesSingletonOld.Instance.settingData.GuideList.Clear();
            // //TODO:改
            // ResourcesSingletonOld.Instance.settingData.UnlockList.Clear();
            // var tbtag = ConfigManager.Instance.Tables.Tbtag;
            // var tbtag_func = ConfigManager.Instance.Tables.Tbtag_func;
            // foreach (var tag in tbtag.DataList)
            // {
            //     ResourcesSingletonOld.Instance.settingData.UnlockList.Add(tag.id);
            // }
            //
            // foreach (var tag in tbtag_func.DataList)
            // {
            //     ResourcesSingletonOld.Instance.settingData.UnlockList.Add(tag.id);
            // }

            //InitSettings();
        }

        void C2S_LOGINResponse(object sender, WebMsgHandler.Execute e)
        {
            var data = NetWorkManager.Instance.UnPackMsg<S2C_UserResData>(e, out var args);
            Log.Debug($"data.UserName:{data.UserName}");
            var sceneController = Common.Instance.Get<SceneController>(); // 场景控制
            var sceneObj = sceneController.LoadSceneAsync<MenuScene>(SceneName.UIMenu);
            SceneResManager.WaitForCompleted(sceneObj).ToCoroutine(); // 等待场景加载完毕
        }

        async void OnLoginResponse()
        {
            //var gameUserRes = NetWorkManager.Instance.UnPackMsg<GameUserRes>(e);

            var sceneController = Common.Instance.Get<SceneController>(); // 场景控制
            var sceneObj = sceneController.LoadSceneAsync<MenuScene>(SceneName.UIMenu);
            SceneResManager.WaitForCompleted(sceneObj).ToCoroutine(); // 等待场景加载完毕
        }

        async void OnLoginResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            var gameUser = new GameUserOld();
            gameUser.MergeFrom(e.data);

            Log.Debug($"接收到登录消息:{gameUser}", Color.green);

            var global = Common.Instance.Get<Global>();
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);

                var KInputTextField = GetFromReference(UIPanel_Login.KInputTextField);
                var KLoginBtn = GetFromReference(UIPanel_Login.KLoginBtn);
                var KIsStandAlone = GetFromReference(UIPanel_Login.KIsStandAlone);
                var KInputText = GetFromReference(UIPanel_Login.KInputText);
                var KBtnText = GetFromReference(UIPanel_Login.KBtnText);

                KInputTextField.SetActive(true);
                KLoginBtn.SetActive(true);
                KIsStandAlone.SetActive(true);

                KLoginBtn.GetXButton().RemoveAllListeners();
                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KLoginBtn,
                    async () =>
                    {
                        global.isStandAlone = KIsStandAlone.GetToggle().IsOn;
                        if (!global.isStandAlone)
                        {
                            JsonManager.Instance.sharedData.lastLoginUserId = 0;
                            await JsonManager.Instance.SaveSharedData(JsonManager.Instance.sharedData);

                            UnicornUIHelper.LoginRequest(1, "", KInputText.GetTextMeshPro().Content);
                        }
                        else
                        {
                            Log.Error($"进入单机模式");

                            //NetWorkManager.Instance.Close();
                            var gameUser = new GameUserOld
                            {
                                Id = -1,
                                UserName = "单机账号001",
                                UserPass = "",
                                SourceTable = "",
                                PrivateKey = ""
                            };
                            var userName = UnicornUIHelper.HandleStr(gameUser.UserName);
                            var data = JsonManager.Instance.LoadPlayerData(gameUser.Id, userName);

                            var sceneController = Common.Instance.Get<SceneController>(); // 场景控制
                            var sceneObj = sceneController.LoadSceneAsync<MenuScene>(SceneName.UIMenu);
                            SceneResManager.WaitForCompleted(sceneObj).ToCoroutine(); // 等待场景加载完毕
                        }
                    });

                return;
            }

            var userName = UnicornUIHelper.HandleStr(gameUser.UserName);
            var privateKey = UnicornUIHelper.HandleStr(gameUser.PrivateKey);

            var data = JsonManager.Instance.LoadPlayerData(gameUser.Id, privateKey, userName);
            // await data.AsUniTask();
            // if (JsonManager.Instance.sharedData.noticesList == null)
            // {
            //     JsonManager.Instance.sharedData.noticesList = new NoticeList();
            //     //JsonManager.Instance.SavePlayerData(JsonManager.Instance.userData);
            // }

            //Log.Error($"gameUser{gameUser}");
            //await UniTask.Delay(3000);
            //gameUser.IntroGuideFinished = 2;
            var KIsGuide = GetFromReference(UIPanel_Login.KIsGuide);
            global.isIntroGuide = KIsGuide.GetToggle().IsOn;

            if (gameUser.IntroGuideFinished == 2)
            {
                // gameUser.IntroGuideFinished = global.isIntroGuide ? 2 : 1;
                // Debug.Log($"isIntroGuide{global.isIntroGuide}");
                if (global.isIntroGuide)
                {
                    Log.Debug($"global.isIntroGuide{global.isIntroGuide}");
                    WebMessageHandlerOld.Instance.AddHandler(CMDOld.QUERYSETTINGS, OnQueryLoginSettingsResponse);
                    NetWorkManager.Instance.SendMessage(CMDOld.QUERYSETTINGS);
                }
                else
                {
                    Log.Debug($"global.isIntroGuide{global.isIntroGuide}");
                    var sceneController = Common.Instance.Get<SceneController>(); // 场景控制
                    var sceneObj = sceneController.LoadSceneAsync<MenuScene>(SceneName.UIMenu);
                    SceneResManager.WaitForCompleted(sceneObj).ToCoroutine(); // 等待场景加载完毕
                }

                // UnicornTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionFXEnter);
                // await UniTask.Delay(1000, true);
                //global.isIntroGuide = true;
                //global.isStandAlone = KIsStandAlone.GetToggle().IsOn;
            }
            else
            {
                //global.isIntroGuide = false;
                var sceneController = Common.Instance.Get<SceneController>(); // 场景控制
                var sceneObj = sceneController.LoadSceneAsync<MenuScene>(SceneName.UIMenu);
                SceneResManager.WaitForCompleted(sceneObj).ToCoroutine(); // 等待场景加载完毕
            }

            //this.Close();
            Log.Debug($"{gameUser} {TimeHelper.ToDateTime(gameUser.LastLoginTime * 1000)}", Color.red);
        }


        /// <summary>
        /// 利用bilibili的接口通过ip直接获取城市信息
        /// </summary>
        async UniTaskVoid GetLocationInfoNew()
        {
            //UnityWebRequest publicIpReq = UnityWebRequest.Get(@"https://api.live.bilibili.com/client/v1/Ip/getInfoNew");

            var publicIpReq = new UnityWebRequest("https://api.live.bilibili.com/client/v1/Ip/getInfoNew",
                UnityWebRequest.kHttpVerbGET);
            publicIpReq.downloadHandler = new DownloadHandlerBuffer();

            await publicIpReq.SendWebRequest().ToUniTask();

            if (!string.IsNullOrEmpty(publicIpReq.error))
            {
                Debug.Log($"获取城市信息失败：{publicIpReq.error}");
                //yield break;
            }

            var info = publicIpReq.downloadHandler.text;
            //Debug.Log(info);

            //将json解析为object
            var resData = JsonUtility.FromJson<ResponseRootData>(info);
            //Debug.Log($"address:{resData.data.addr}|province:{resData.data.province}|city:{resData.data.city}");
        }

        #region 用于接收返回值json的反序列化数据

        [System.Serializable]
        public class ResponseRootData
        {
            public int code;
            public string message;
            public ResponseData data;
        }

        [System.Serializable]
        public class ResponseData
        {
            public string addr;
            public string country;
            public string province;
            public string city;
            public string isp;
            public string latitude;
            public string longitude;
        }

        #endregion

        protected override void OnClose()
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.LOGIN, OnLoginResponse);

            base.OnClose();
        }
    }
}