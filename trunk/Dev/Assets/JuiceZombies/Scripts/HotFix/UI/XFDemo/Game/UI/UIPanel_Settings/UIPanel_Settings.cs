//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Reflection;
using System.Threading;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Google.Protobuf;
using HotFix_UI;
using JuiceZombies.Main;
using Main;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Settings)]
    internal sealed class UIPanel_SettingsEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Settings;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Settings>();
        }
    }

    public partial class UIPanel_Settings : UI, IAwake
    {
        private int qualityBtnIndex = -1;
        Tbattr_variable attr_variableConfig = ConfigManager.Instance.Tables.Tbattr_variable;

        Tbequip_level equip_levelConfig = ConfigManager.Instance.Tables.Tbequip_level;

        //var player_skill = ConfigManager.Instance.Tables.Tbplayer_skill;
        Tblanguage languageConfig = ConfigManager.Instance.Tables.Tblanguage;
        Tbuser_variable user_variblesConfig = ConfigManager.Instance.Tables.Tbuser_variable;
        Tbequip_data equip_dataConfig = ConfigManager.Instance.Tables.Tbequip_data;
        Tbequip_quality equip_qualityConfig = ConfigManager.Instance.Tables.Tbequip_quality;
        Tbequip_pos equip_posConfig = ConfigManager.Instance.Tables.Tbequip_pos;
        Tbquality qualityConfig = ConfigManager.Instance.Tables.Tbquality;
        Tbsetting settingConfig = ConfigManager.Instance.Tables.Tbsetting;
        Tbsetting_language setting_languageConfig = ConfigManager.Instance.Tables.Tbsetting_language;
        Tbconstant tbconstant = ConfigManager.Instance.Tables.Tbconstant;
        CancellationTokenSource cts = new CancellationTokenSource();
        private long timerId0;

        public async void Initialize()
        {
            await JiYuUIHelper.InitBlur(this);
            SetUpdate();
            WebMessageHandler.Instance.AddHandler(CMD.SENDGIFTCODE, OnSendGiftCodeResponse);
            OnClickEvent();
            Init().Forget();
        }

        private void OnSendGiftCodeResponse(object sender, WebMessageHandler.Execute e)
        {
            //WebMessageHandler.Instance.RemoveHandler(CMD.SENDGIFTCODE, OnSendGiftCodeResponse);
            var strValue = new StringValue();
            strValue.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Debug.Log($"{MethodBase.GetCurrentMethod().Name} is empty");
                return;
            }

            var str = strValue.Value.DeleteEmtypStr();

            if (str.Contains("fail"))
            {
                JiYuUIHelper.ClearCommonResource();
                UIHelper.CreateAsync(UIType.UICommon_Resource,
                    languageConfig.Get("test_exchange_fail").current).Forget();
            }
            else if (str.Contains("success"))
            {
                var KGiftCode = GetFromReference(UIPanel_Settings.KGiftCode);
                var KInput_GiftCodeInput = GetFromReference(UIPanel_Settings.KInput_GiftCodeInput);
                KInput_GiftCodeInput.GetTMP_InputField().SetText(languageConfig.Get("setting_giftid_text").current);
                NetWorkManager.Instance.SendMessage(CMD.INITPLAYER);
                JiYuUIHelper.ClearCommonResource();
                UIHelper.CreateAsync(UIType.UICommon_Resource,
                    languageConfig.Get("test_exchange_success").current).Forget();

                KGiftCode.SetActive(false);
            }


            Log.Debug($"{MethodBase.GetCurrentMethod().Name} {strValue.Value}");
        }

        async UniTaskVoid Init()
        {
            var KText_Title = GetFromReference(UIPanel_Settings.KText_Title);
            var KText_ShareTitle = GetFromReference(UIPanel_Settings.KText_ShareTitle);
            var KText_IsGot = GetFromReference(UIPanel_Settings.KText_IsGot);
            var KText_IsNotGetLeft = GetFromReference(UIPanel_Settings.KText_IsNotGetLeft);

            var KText_Quality = GetFromReference(UIPanel_Settings.KText_Quality);
            var KQualityHorizontal = GetFromReference(UIPanel_Settings.KQualityHorizontal);
            var KMidHorizontal = GetFromReference(UIPanel_Settings.KMidHorizontal);
            var KBottomHorizontal = GetFromReference(UIPanel_Settings.KBottomHorizontal);
            var KText_Verson = GetFromReference(UIPanel_Settings.KText_Verson);
            var KText_Protocol = GetFromReference(UIPanel_Settings.KText_Protocol);
            var KText_PersonalInfo = GetFromReference(UIPanel_Settings.KText_PersonalInfo);
            var KText_Probability = GetFromReference(UIPanel_Settings.KText_Probability);
            var KBtn_SharePos = GetFromReference(UIPanel_Settings.KBtn_SharePos);
            var KIsNotGet = GetFromReference(UIPanel_Settings.KIsNotGet);
            var KLanguage = GetFromReference(UIPanel_Settings.KLanguage);
            var KLanguageListPos = GetFromReference(UIPanel_Settings.KLanguageListPos);
            var KSortList = GetFromReference(UIPanel_Settings.KSortList);
            var KBg = GetFromReference(UIPanel_Settings.KBg);

            var KGiftCode = GetFromReference(UIPanel_Settings.KGiftCode);
            var KInput_GiftCodeInput = GetFromReference(UIPanel_Settings.KInput_GiftCodeInput);
            var KBtn_GiftCode = GetFromReference(UIPanel_Settings.KBtn_GiftCode);
            var KText_BtnGiftCode = GetFromReference(UIPanel_Settings.KText_BtnGiftCode);

            AudioManager.Instance.PlayFModAudio(1241);
            //setting_giftid_text
            JiYuUIHelper.LoadImage($"share_icon{ResourcesSingleton.Instance.gameShare.Id}.png", KBtn_SharePos);

            KInput_GiftCodeInput.GetTMP_InputField().SetText(languageConfig.Get("setting_giftid_text").current);
            KInput_GiftCodeInput.GetTMP_InputField().OnSelect.Add((a) =>
            {
                //KInput_GiftCodeInput.GetTMP_InputField().SetText("");
                KInput_GiftCodeInput.GetTMP_InputField().Get().Select();
                KInput_GiftCodeInput.GetTMP_InputField().Get().ActivateInputField();
            });
            KText_BtnGiftCode.GetTextMeshPro().SetTMPText(languageConfig.Get("common_state_exchange").current);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_GiftCode, () =>
            {
                //TODO���һ�
                var str = KInput_GiftCodeInput.GetTMP_InputField().Content.DeleteEmtypStr();
                str = JiYuUIHelper.HandleStr(str);
                var sendStr = new StringValue();
                sendStr.Value = str;

                NetWorkManager.Instance.SendMessage(CMD.SENDGIFTCODE, sendStr);
            });
            KGiftCode.SetActive(false);
            var content = KSortList.GetScrollRect().Content;
            KLanguageListPos.SetActive(false);
            var languageSortList = content.GetList();
            languageSortList.Clear();
            for (int i = 0; i < setting_languageConfig.DataList.Count; i++)
            {
                int index = i;
                var ui =
                    languageSortList.CreateWithUIType(UIType.UISubPanel_CommonBtn, index,
                        false) as UISubPanel_CommonBtn;
                ui.GetRectTransform().SetScale2(0.85f);
                ui.GetFromReference(UISubPanel_CommonBtn.KImg_RedDot).SetActive(false);

                var KText_Mid = ui.GetFromReference(UISubPanel_CommonBtn.KText_Mid);
                var KBtn_Common = ui.GetFromReference(UISubPanel_CommonBtn.KBtn_Common);
                var KImg_Btn = ui.GetFromReference(UISubPanel_CommonBtn.KImg_Btn);
                KText_Mid.SetActive(true);
                ui.GetRectTransform().SetWidth(240f);
                //TODO:


                KText_Mid.GetTextMeshPro()
                    .SetTMPText(JiYuUIHelper.GetRewardTextIconName("img_chineseflag") +
                                languageConfig.Get(setting_languageConfig.DataList[i].name).current);
                if ((int)ResourcesSingleton.Instance.settingData.CurrentL10N == index + 1)
                {
                    KImg_Btn.GetImage().SetAlpha(1f);
                }
                else
                {
                    KImg_Btn.GetImage().SetAlpha(0.5f);
                }

                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Common,
                    () =>
                    {
                        if ((int)ResourcesSingleton.Instance.settingData.CurrentL10N == index + 1)
                        {
                            KLanguageListPos.SetActive(false);
                            return;
                        }

                        languageSortList.GetChildAt((int)ResourcesSingleton.Instance.settingData.CurrentL10N - 1)
                            .GetFromReference(UISubPanel_CommonBtn.KImg_Btn)
                            .GetImage()
                            .SetAlpha(1);
                        KImg_Btn.GetImage().SetAlpha(0.5f);
                        //Log.Error($"{(Tblanguage.L10N)(index + 1)}");
                        JiYuUIHelper.RefreshAllPanelL10N(index + 1);
                        RemoveTimer();
                        this.Initialize();
                        KLanguageListPos.SetActive(false);
                    });
            }

            languageSortList.Sort((obj1, obj2) =>
            {
                var ui1 = obj1 as UISubPanel_CommonBtn;
                var ui2 = obj2 as UISubPanel_CommonBtn;
                return ui1.index.CompareTo(ui2.index);
            });

            KText_Title.GetTextMeshPro().SetTMPText(languageConfig.Get("func_3101_name").current);
            KText_ShareTitle.GetTextMeshPro().SetTMPText(languageConfig.Get("setting_share_title").current);
            KText_Protocol.GetTextMeshPro().SetTMPText(languageConfig.Get("setting_privacy_name").current);
            KText_PersonalInfo.GetTextMeshPro().SetTMPText(languageConfig.Get("setting_info_name").current);
            KText_Probability.GetTextMeshPro().SetTMPText(languageConfig.Get("setting_probability_name").current);

            KText_Quality.GetTextMeshPro().SetTMPText(languageConfig.Get(settingConfig.Get(6).name).current);


            var count = tbconstant.Get("share_reward").constantValue;
            var maxCount = tbconstant.Get("share_reward_limit").constantValue;
            var numStr =
                $"{languageConfig.Get("setting_share_no_text").current} {JiYuUIHelper.GetRewardTextIconName($"icon_diamond")}{count}";
            KText_IsNotGetLeft.GetTextMeshPro().SetTMPText(numStr);
            KText_IsGot.GetTextMeshPro().SetTMPText(languageConfig.Get("setting_share_yes_text").current);

            var shareTime = ResourcesSingleton.Instance.gameShare.IsShare ? 1 : 0;
            if (shareTime < maxCount)
            {
                //can get
                KText_IsNotGetLeft.SetActive(true);

                KText_IsGot.SetActive(false);
            }
            else
            {
                //can not get
                KText_IsNotGetLeft.SetActive(false);

                KText_IsGot.SetActive(true);
            }

            KText_IsGot.SetActive(false);
            KIsNotGet.SetActive(true);

            //TODO:
            KText_Verson.GetTextMeshPro()
                .SetTMPText(
                    $"{languageConfig.Get("setting_version_name").current}:{LoadDllMono.Verson.subVerson}");
            var qualityList = KQualityHorizontal.GetList();
            qualityList.Clear();

            for (int i = 0; i < 3; i++)
            {
                var index = i;
                var ui =
                    qualityList.CreateWithUIType(UIType.UISubPanel_SettingsBtn, false) as
                        UISubPanel_SettingsBtn;
                var KText_AllText = ui.GetFromReference(UISubPanel_SettingsBtn.KText_AllText);
                var KBtn_Common = ui.GetFromReference(UISubPanel_SettingsBtn.KBtn_Common);

                KBtn_Common.GetImage().SetAlpha(0.5f);
                KText_AllText.SetActive(true);

                ui.GetRectTransform().SetWidth(223f);
                ui.GetRectTransform().SetHeight(82f);
                var btnRec = KBtn_Common.GetRectTransform();
                btnRec.SetWidth(280f);
                btnRec.SetHeight(82f);
                btnRec.SetAnchoredPosition(0, 0);
                //TODO:
                if ((int)ResourcesSingleton.Instance.settingData.Quality == index)
                {
                    KBtn_Common.GetImage().SetAlpha(1);
                }

                switch (index)
                {
                    case 0:
                        KText_AllText.GetTextMeshPro().SetTMPText(languageConfig.Get("setting_6_1_name").current);
                        JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Common, () =>
                        {
                            if ((int)ResourcesSingleton.Instance.settingData.Quality == index)
                                return;
                            foreach (var VARIABLE in qualityList.Children)
                            {
                                var KBtn_Common = VARIABLE.GetFromReference(UISubPanel_SettingsBtn.KBtn_Common);

                                KBtn_Common.GetImage().SetAlpha(0.5f);
                            }

                            ResourcesSingleton.Instance.settingData.Quality = index;
                            KBtn_Common.GetImage().SetAlpha(1);

                            JiYuUIHelper.SetFrameRate(true);
                        });
                        break;
                    case 1:
                        KText_AllText.GetTextMeshPro().SetTMPText(languageConfig.Get("setting_6_2_name").current);
                        JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Common, () =>
                        {
                            if ((int)ResourcesSingleton.Instance.settingData.Quality == index)
                                return;
                            foreach (var VARIABLE in qualityList.Children)
                            {
                                var KBtn_Common = VARIABLE.GetFromReference(UISubPanel_SettingsBtn.KBtn_Common);

                                KBtn_Common.GetImage().SetAlpha(0.5f);
                            }

                            ResourcesSingleton.Instance.settingData.Quality = index;
                            KBtn_Common.GetImage().SetAlpha(1);
                            JiYuUIHelper.SetFrameRate(false, 60);
                        });
                        break;
                    case 2:
                        KText_AllText.GetTextMeshPro().SetTMPText(languageConfig.Get("setting_6_3_name").current);
                        JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Common, () =>
                        {
                            if ((int)ResourcesSingleton.Instance.settingData.Quality == index)
                                return;
                            foreach (var VARIABLE in qualityList.Children)
                            {
                                var KBtn_Common = VARIABLE.GetFromReference(UISubPanel_SettingsBtn.KBtn_Common);

                                KBtn_Common.GetImage().SetAlpha(0.5f);
                            }

                            ResourcesSingleton.Instance.settingData.Quality = index;
                            KBtn_Common.GetImage().SetAlpha(1);
                            JiYuUIHelper.SetFrameRate(false, 30);
                        });
                        break;
                }
            }

            var midList = KMidHorizontal.GetList();
            midList.Clear();


            for (int i = 0; i < 5; i++)
            {
                var index = i;
                var ui =
                    midList.CreateWithUIType(UIType.UISubPanel_SettingsItem, false) as
                        UISubPanel_SettingsItem;
                var KText = ui.GetFromReference(UISubPanel_SettingsItem.KText);
                var KIcon = ui.GetFromReference(UISubPanel_SettingsItem.KIcon);
                var KImg_IconBgClose = ui.GetFromReference(UISubPanel_SettingsItem.KImg_IconBgClose);
                var KImg_IconBgOpen = ui.GetFromReference(UISubPanel_SettingsItem.KImg_IconBgOpen);
                var KBtn_Item = ui.GetFromReference(UISubPanel_SettingsItem.KBtn_Item);
                var setting = settingConfig.Get(index + 1);
                KText.GetTextMeshPro().SetTMPText(languageConfig.Get(setting.name).current);

                ui.GetRectTransform().SetWidth(294f);
                // if (setting.init == 1)
                // {
                //     KIcon.GetImage().SetSpriteAsync(setting.pic1, false).Forget();
                //     ui.SetEnable(true);
                // }
                // else
                // {
                //     KIcon.GetImage().SetSpriteAsync(setting.pic2, false).Forget();
                //     ui.SetEnable(false);
                // }
                KImg_IconBgOpen.SetActive(false);
                KImg_IconBgClose.SetActive(false);
                if (JiYuUIHelper.InitEnableSettings(index))
                {
                    KIcon.GetImage().SetSpriteAsync(setting.pic1, false).Forget();
                    KImg_IconBgOpen.SetActive(true);
                    // KIcon.GetImage().SetAlpha(1);
                    // KText.GetTextMeshPro().SetAlpha(1);
                }
                else
                {
                    KIcon.GetImage().SetSpriteAsync(setting.pic2, false).Forget();
                    KImg_IconBgClose.SetActive(true);
                    // KIcon.GetImage().SetAlpha(0.5f);
                    // KText.GetTextMeshPro().SetAlpha(0.5f);
                }


                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Item, () =>
                {
                    KImg_IconBgOpen.SetActive(false);
                    KImg_IconBgClose.SetActive(false);

                    if (JiYuUIHelper.InitEnableSettings(index, true))
                    {
                        KIcon.GetImage().SetSpriteAsync(setting.pic1, false).Forget();
                        KImg_IconBgOpen.SetActive(true);
                    }
                    else
                    {
                        KIcon.GetImage().SetSpriteAsync(setting.pic2, false).Forget();
                        KImg_IconBgClose.SetActive(true);
                    }

                    switch (index)
                    {
                        case 0:


                            break;
                        case 1:

                            break;
                        case 2:
                            break;
                        case 3:

                            break;
                        case 4:

                            break;
                    }
                });
            }

            var bottomList = KBottomHorizontal.GetList();
            bottomList.Clear();
            for (int i = 0; i < 4; i++)
            {
                var index = i;
                var ui =
                    bottomList.CreateWithUIType(UIType.UISubPanel_SettingsBtn, false) as
                        UISubPanel_SettingsBtn;
                var KText_AllText = ui.GetFromReference(UISubPanel_SettingsBtn.KText_AllText);
                var KBtn_Common = ui.GetFromReference(UISubPanel_SettingsBtn.KBtn_Common);


                ui.GetRectTransform().SetWidth(200f);
                KBtn_Common.GetImage().SetAlpha(1);
                switch (index)
                {
                    case 0:
                        KBtn_Common.GetImage().SetSpriteAsync($"icon_btn_red_3", true).Forget();
                        KText_AllText.GetTextMeshPro().SetTMPText(languageConfig.Get("setting_quit_name").current);
                        JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Common,
                            async () =>
                            {
                                NetWorkManager.Instance.SendMessage(CMD.SWITCHACCOUNT);
                                // NetWorkManager.Instance.SendMessage(CMD.SWITCHACCOUNT, new StringValue
                                // {
                                //     Value = JsonManager.Instance.userData.privateKey
                                // });
                                JsonManager.Instance.sharedData.lastLoginUserId = 0;
                                await JsonManager.Instance.SaveSharedData(JsonManager.Instance.sharedData);
                                //UIHelper.CreateAsync(UIType.UILogin);
                                Close();
                                var sceneController = Common.Instance.Get<SceneController>();
                                var sceneObj = sceneController.LoadSceneAsync<Login>(SceneName.Login);
                                SceneResManager.WaitForCompleted(sceneObj).ToCoroutine();
                            });
                        break;
                    case 1:
                        KBtn_Common.GetImage().SetSpriteAsync($"icon_btn_red_3", true).Forget();
                        KText_AllText.GetTextMeshPro().SetTMPText(languageConfig.Get("setting_fix_name").current);
                        JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Common,
                            async () =>
                            {
                                await ResourcesManager.Instance.Loader.ClearPackageAllCacheBundleFiles();
                                JiYuSceneHelper.RestartApplication();
                            });
                        break;
                    case 2:

                        KBtn_Common.GetImage().SetSpriteAsync($"icon_btn_blue_33", true).Forget();
                        KText_AllText.GetTextMeshPro().SetTMPText(languageConfig.Get("setting_feedback_name").current);
                        JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Common,
                            async () => { UIHelper.CreateAsync(UIType.UIPanel_Quest); });
                        break;
                    case 3:
                        ClearAllTips();
                        KBtn_Common.GetImage().SetSpriteAsync($"icon_btn_yellow_3", true).Forget();
                        KBtn_Common.GetRectTransform().SetAnchoredPositionX(37f);
                        KText_AllText.GetTextMeshPro().SetTMPText(languageConfig.Get("setting_giftid_name").current);
                        JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Common,
                            () =>
                            {
                                KGiftCode.SetActive(!KGiftCode.GameObject.activeSelf);
                                KLanguageListPos.SetActive(false);
                                //var KInput_GiftCodeInput = GetFromReference(UIPanel_Settings.KInput_GiftCodeInput);
                                KInput_GiftCodeInput.GetTMP_InputField()
                                    .SetText(languageConfig.Get("setting_giftid_text").current);
                            });
                        break;
                }
            }

            var languageList = KLanguage.GetList();
            languageList.Clear();

            for (int i = 0; i < 1; i++)
            {
                var index = i;
                var ui =
                    languageList.CreateWithUIType(UIType.UISubPanel_SettingsBtn, false) as
                        UISubPanel_SettingsBtn;
                var KText_AllText = ui.GetFromReference(UISubPanel_SettingsBtn.KText_AllText);
                var KBtn_Common = ui.GetFromReference(UISubPanel_SettingsBtn.KBtn_Common);
                var btnRec = KBtn_Common.GetRectTransform();
                btnRec.SetWidth(280f);
                btnRec.SetHeight(82f);
                btnRec.SetAnchoredPosition(0, 0);
                btnRec.SetHeight(208);
                btnRec.SetWidth(111);
                ui.GetRectTransform().SetWidth(200f);
                KBtn_Common.GetImage().SetAlpha(1);
                KBtn_Common.GetImage().SetSpriteAsync($"icon_btn_blue_33", true).Forget();
                KText_AllText.GetTextMeshPro().SetTMPText(languageConfig.Get("setting_7_name").current);

                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Common,
                    () =>
                    {
                        ClearAllTips();
                        KLanguageListPos.SetActive(!KLanguageListPos.GameObject.activeSelf);
                    });
            }

            JiYuUIHelper.ForceRefreshLayout(content);
            JiYuUIHelper.ForceRefreshLayout(KQualityHorizontal);
            JiYuUIHelper.ForceRefreshLayout(KMidHorizontal);
            JiYuUIHelper.ForceRefreshLayout(KBottomHorizontal);
            JiYuUIHelper.ForceRefreshLayout(KLanguage);
        }

        private void SetUpdate()
        {
            var timerMgr = TimerManager.Instance;
            timerId0 = timerMgr.StartRepeatedTimer(2000, Update);
        }

        private void Update()
        {
            var KCommon_CloseInfo = GetFromReference(UIPanel_Settings.KCommon_CloseInfo);
            KCommon_CloseInfo?.GetTextMeshPro()?.SetTMPText(languageConfig.Get("text_window_close").current);

            KCommon_CloseInfo?.GetTextMeshPro()?.DoFade(1, 0.1f, 1f)?.AddOnCompleted(() =>
            {
                KCommon_CloseInfo.GetTextMeshPro().DoFade(0.1f, 1, 1f);
            });
        }

        private void ClearAllTips()
        {
            GetFromReference(KLanguageListPos).SetActive(false);
            GetFromReference(KGiftCode).SetActive(false);
        }

        void DisableLanguageList()
        {
            var KLanguageListPos = GetFromReference(UIPanel_Settings.KLanguageListPos);

            KLanguageListPos.SetActive(false);
        }

        private void RemoveTimer()
        {
            var timerMgr = TimerManager.Instance;
            timerMgr?.RemoveTimerId(ref this.timerId0);
            this.timerId0 = 0;
            //FrameNum = 49;
        }

        void OnClickEvent()
        {
            var KBtn_Close = GetFromReference(UIPanel_Settings.KBtn_Close);
            var KMask = GetFromReference(UIPanel_Settings.KMask);
            var KBtn_SharePos = GetFromReference(UIPanel_Settings.KBtn_SharePos);
            var KText_Protocol = GetFromReference(UIPanel_Settings.KText_Protocol);
            var KText_PersonalInfo = GetFromReference(UIPanel_Settings.KText_PersonalInfo);
            var KText_Probability = GetFromReference(UIPanel_Settings.KText_Probability);
            var KBg = GetFromReference(UIPanel_Settings.KBg);

            KBg.GetXButton().OnClick.Add(DisableLanguageList);
            KMask.GetXButton().OnClick.Add(async () => ClosePanel());
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Close, async () => ClosePanel());
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_SharePos,
                () => { UIHelper.CreateAsync(UIType.UIPanel_Share); });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KText_Protocol);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KText_PersonalInfo);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KText_Probability);
        }

        private async UniTask ClosePanel()
        {
            JiYuTweenHelper.SetEaseAlphaAndPosUtoB(GetFromReference(UIPanel_Settings.KMid), 0 - 100, 100, cts.Token,
                0.15f,
                false);
            JiYuTweenHelper.SetEaseAlphaAndPosRtoL(GetFromReference(UIPanel_Settings.KMid), 0 - 100, 100, cts.Token,
                0.15f,
                false);
            GetFromReference(UIPanel_Settings.KMid).GetComponent<CanvasGroup>().alpha = 1f;
            GetFromReference(UIPanel_Settings.KMid).GetComponent<CanvasGroup>().DOFade(0, 0.3f).SetEase(Ease.InQuad);
            await UniTask.Delay(150);
            Close();
        }


        protected override void OnClose()
        {
            RemoveTimer();
            cts.Cancel();
            cts.Dispose();
            // NetWorkManager.Instance.SendMessage(CMD.CHANGESETTINGS, new SettingDate
            // {
            //     Quality = (int)ResourcesSingleton.Instance.settingsData.quality,
            //     EnableFx = ResourcesSingleton.Instance.settingsData.enableFx,
            //     EnableBgm = ResourcesSingleton.Instance.settingsData.enableBgm,
            //     EnableShock = ResourcesSingleton.Instance.settingsData.enableShock,
            //     EnableWeakEffect = ResourcesSingleton.Instance.settingsData.enableWeakEffect,
            //     EnableShowStick = ResourcesSingleton.Instance.settingsData.enableShowStick,
            //     CurrentL10N = (int)ResourcesSingleton.Instance.settingsData.currentL10N,
            // });

            if (!(ResourcesSingleton.Instance.settingData.CurrentL10N == 0))
            {
                JsonManager.Instance.sharedData.l10N = ResourcesSingleton.Instance.settingData.CurrentL10N;
            }

            JsonManager.Instance.SaveSharedData(JsonManager.Instance.sharedData);
            var settings = new SettingDate
            {
                RoleId = ResourcesSingleton.Instance.settingData.RoleId,
                Quality = ResourcesSingleton.Instance.settingData.Quality,
                EnableFx = ResourcesSingleton.Instance.settingData.EnableFx,
                EnableBgm = ResourcesSingleton.Instance.settingData.EnableBgm,
                EnableShock = ResourcesSingleton.Instance.settingData.EnableShock,
                EnableWeakEffect = ResourcesSingleton.Instance.settingData.EnableWeakEffect,
                EnableShowStick = ResourcesSingleton.Instance.settingData.EnableShowStick,
                CurrentL10N = ResourcesSingleton.Instance.settingData.CurrentL10N,
                GuideList = { },
                UnlockMap = { }
            };

            NetWorkManager.Instance.SendMessage(CMD.CHANGESETTINGS, settings);
            WebMessageHandler.Instance.RemoveHandler(CMD.SENDGIFTCODE, OnSendGiftCodeResponse);

            base.OnClose();
        }
    }
}