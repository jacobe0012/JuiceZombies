//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using Common;
using Google.Protobuf;
using HotFix_UI;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Person_ChangeName)]
    internal sealed class UISubPanel_Person_ChangeNameEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UISubPanel_Person_ChangeName;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Person_ChangeName>();
        }
    }

    public partial class UISubPanel_Person_ChangeName : UI, IAwake<CheckResult, int>
    {
        #region

        private Tblanguage tblanguage;
        private Tbconstant tbconstant;
        private Tbitem tbitem;
        private Tbuser_variable tbuser_Variable;

        //0是改名进入冷却状态并显示冷却时间，1是免费修改，2是改名卡修改，3是付费资产改名且资产充足，4是付费资产改名且资产不充足
        private int isFree = 0;

        private long timerId;
        private long CDS = 86400;
        private bool inCD = false;
        private bool isSent = false;
        private int timeHelp = 0;

        string isFreeButtonStr = "icon_btn_yellow";
        string isNotFreeButtonStr = "icon_btn_green";
        string inCDButtonStr = "pic_buttonGray";
        private int _guideId = 0;

        #endregion

        public async void Initialize(CheckResult checkResult, int guideId = 0)
        {
            await UnicornUIHelper.InitBlur(this);
            isFree = int.Parse(checkResult.Status);
            _guideId = guideId;
            if (isFree == 0)
            {
                CDS = checkResult.Value;
            }

            DataInit();
            NetInit();
            BtnInit();
            TextInit();
            UpdateInit();
            SelectStatus();

            this.GetFromReference(KInputField).GetComponent<TMP_InputField>().ActivateInputField();
            UnicornTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(UISubPanel_Person_ChangeName.KBg), 0);
        }

        private void DataInit()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbconstant = ConfigManager.Instance.Tables.Tbconstant;
            tbitem = ConfigManager.Instance.Tables.Tbitem;
            tbuser_Variable = ConfigManager.Instance.Tables.Tbuser_variable;
        }

        private void UpdateInit()
        {
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.StartRepeatedTimer(20, this.Update);
        }

        private void NetInit()
        {
            WebMessageHandlerOld.Instance.AddHandler(1, 6, OnChangeNameResponse);
            WebMessageHandlerOld.Instance.AddHandler(1, 5, OnChangeStatusResponse);
        }

        private void TextInit()
        {
            this.GetFromReference(KText_Desc).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("user_info_name_change_desc").current);
            this.GetFromReference(KPlaceholder).GetTextMeshPro()
                .SetTMPText(ResourcesSingletonOld.Instance.UserInfo.Nickname);
            this.GetFromReference(KText_Input).GetTextMeshPro().SetTMPText("");
            this.GetFromReference(KInputField).GetComponent<TMP_InputField>().text = "";
            this.GetFromReference(KText_Free).GetTextMeshPro().SetTMPText(
                tblanguage.Get("common_free_text").current + tblanguage.Get("user_info_name_change_text").current);
            this.GetFromReference(KText_Change).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("user_info_name_change_text").current);
        }

        private void BtnInit()
        {
            this.GetFromReference(KBtn_BgClose).GetXButton().OnClick.Add(Close);
            var ClearBtn = this.GetFromReference(KBtn_Clear);
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(ClearBtn, () =>
            {
                this.GetFromReference(KInputField).GetComponent<TMP_InputField>().text = "";
                this.GetFromReference(KText_Input).GetTextMeshPro().SetTMPText("");
            });

            var changeBtn = this.GetFromReference(KBtn_ChangeName);
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(changeBtn, () =>
            {
                string nameStr = this.GetFromReference(KText_Input).GetTextMeshPro().Content;
                nameStr = CleanStr(nameStr);

                bool state = JudgeStatus(nameStr);

                if (state)
                {
                    //打开更改名称的二次确认，暂时先不加二次确认
                    ResourcesSingletonOld.Instance.UserInfo.Nickname = nameStr;
                    var str = new StringValue();
                    str.Value = nameStr;
                    Debug.Log(nameStr);
                    NetWorkManager.Instance.SendMessage(1, 6, str);

                    if(UnicornUIHelper.TryGetUI(UIType.UIPanel_Main,out  UI ui)){
                        var uis=ui as UIPanel_Main;
                        uis.ChangePlayerInfo();
                    }

                }
                else
                {
                    CreatePrompt(tblanguage.Get("user_info_name_change_fail_2").current);
                    //打开通用UI提示包含限制词汇
                    Debug.Log("打开通用UI提示包含限制词汇");
                }
            });
        }

        private async void SelectStatus()
        {
            //isFree = 2;
            switch (isFree)
            {
                case 0:
                    //进入改名冷却时间
                    inCD = true;
                    this.GetFromReference(KText_InCD).GetTextMeshPro().SetTMPText(
                        tblanguage.Get("user_info_name_change_cd").current
                        + UnicornUIHelper.GeneralTimeFormat(new int4(2, 3, 1, 1), CDS));
                    this.GetFromReference(KBtn_ChangeName).SetActive(false);
                    this.GetFromReference(KText_InCD).SetActive(true);
                    this.GetFromReference(KImg_CardOrDiamond).SetActive(false);
                    this.GetFromReference(KText_Num).SetActive(false);
                    this.GetFromReference(KText_Change).SetActive(false);
                    this.GetFromReference(KText_Free).SetActive(false);
                    break;

                case 1:
                    //可以免费修改
                    await this.GetFromReference(KIcon_Btn).GetImage().SetSpriteAsync(isFreeButtonStr, false);
                    this.GetFromReference(KBtn_ChangeName).SetActive(true);
                    this.GetFromReference(KText_InCD).SetActive(false);
                    this.GetFromReference(KImg_CardOrDiamond).SetActive(false);
                    this.GetFromReference(KText_Num).SetActive(false);
                    this.GetFromReference(KText_Change).SetActive(false);
                    this.GetFromReference(KText_Free).SetActive(true);
                    break;

                case 2:
                    //可以改名卡修改
                    await this.GetFromReference(KIcon_Btn).GetImage().SetSpriteAsync(isNotFreeButtonStr, false);
                    if (tbitem.Get(1010001) != null)
                    {
                        await this.GetFromReference(KImg_CardOrDiamond).GetImage()
                            .SetSpriteAsync(tbitem.Get(1010001).icon, false);
                    }

                    this.GetFromReference(KText_Num).GetTextMeshPro().SetTMPText("1");

                    this.GetFromReference(KBtn_ChangeName).SetActive(true);
                    this.GetFromReference(KText_InCD).SetActive(false);
                    this.GetFromReference(KImg_CardOrDiamond).SetActive(true);
                    this.GetFromReference(KText_Num).SetActive(true);
                    this.GetFromReference(KText_Change).SetActive(true);
                    this.GetFromReference(KText_Free).SetActive(false);
                    break;

                case 3:
                    //付费资产改名且资产充足
                    await this.GetFromReference(KImg_CardOrDiamond).GetImage()
                        .SetSpriteAsync(tbuser_Variable.Get(2).icon, false);
                    this.GetFromReference(KText_Num).GetTextMeshPro()
                        .SetTMPText(tbconstant.Get("change_name_cost").constantValue.ToString());

                    this.GetFromReference(KBtn_ChangeName).SetActive(true);
                    this.GetFromReference(KText_InCD).SetActive(false);
                    this.GetFromReference(KImg_CardOrDiamond).SetActive(true);
                    this.GetFromReference(KText_Num).SetActive(true);
                    this.GetFromReference(KText_Change).SetActive(true);
                    this.GetFromReference(KText_Free).SetActive(false);
                    break;

                case 4:
                    //付费资产改名但是资产不充足
                    await this.GetFromReference(KIcon_Btn).GetImage().SetSpriteAsync(isNotFreeButtonStr, false);
                    await this.GetFromReference(KImg_CardOrDiamond).GetImage()
                        .SetSpriteAsync(tbuser_Variable.Get(2).icon, false);
                    this.GetFromReference(KText_Num).GetTextMeshPro()
                        .SetTMPText(tbconstant.Get("change_name_cost").constantValue.ToString());

                    this.GetFromReference(KBtn_ChangeName).SetActive(true);
                    this.GetFromReference(KText_InCD).SetActive(false);
                    this.GetFromReference(KImg_CardOrDiamond).SetActive(true);
                    this.GetFromReference(KText_Num).SetActive(true);
                    this.GetFromReference(KText_Change).SetActive(true);
                    this.GetFromReference(KText_Free).SetActive(false);
                    break;

                default:
                    Debug.Log("changename is wrong");
                    break;
            }
        }

        private string CleanStr(string input)
        {
            int strL = input.Length;
            return input.Substring(0, strL - 1);
        }

        /// <summary>
        /// 判断输入名称是否可更改，1是可以更改，0是包含限制词汇
        /// </summary>
        private bool JudgeStatus(string namestr)
        {
            if (namestr == "test")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private async void CreatePrompt(string str)
        {
            UnicornUIHelper.ClearCommonResource();
            await UIHelper.CreateAsync(UIType.UICommon_Resource, str);
        }

        /// <summary>
        /// 回调gameRole后进行对应操作
        /// </summary>
        public void OnChangeNameResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            var changeSuccess = new BoolValue();
            changeSuccess.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }


            if (changeSuccess.Value)
            {
                if (isFree == 2)
                {
                    if (ResourcesSingletonOld.Instance.items.TryGetValue(1010001, out long value))
                    {
                        ResourcesSingletonOld.Instance.items[1010001] -= 1;
                    }
                    else
                    {
                        Debug.Log("Change Name Card id WRONG");
                    }
                }
                else if (isFree == 3)
                {
                    ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin -=
                        tbconstant.Get("change_name_cost").constantValue;
                }

                CreatePrompt(tblanguage.Get("user_info_name_change_success").current);
                string namestr = this.GetFromReference(KText_Input).GetTextMeshPro().Content;
                ResourcesSingletonOld.Instance.UserInfo.Nickname = namestr;
                var parent = this.GetParent<UIPanel_Person>();
                parent.UpdatePerson();
                ResourcesSingletonOld.Instance.UpdateResourceUI();
                Close();
            }
        }

        private void Update()
        {
            if (inCD)
            {
                timeHelp++;
                if (timeHelp == 3000) //每过一分钟
                {
                    CDS -= 60; //减去60秒
                    SelectStatus();
                    timeHelp = 0;
                }

                if (CDS <= 0)
                {
                    inCD = false;
                    isSent = false;
                }
            }
            else
            {
                if (!isSent)
                {
                    NetWorkManager.Instance.SendMessage(1, 5);
                    isSent = true;
                }
            }
        }

        /// <summary>
        /// 回调改名状态后进行对应操作
        /// </summary>
        public void OnChangeStatusResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            var checkResult = new CheckResult();
            checkResult.MergeFrom(e.data);
            Debug.Log(checkResult);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            isFree = int.Parse(checkResult.Status);

            if (isFree == 0)
            {
                CDS = checkResult.Value;
            }

            SelectStatus();
        }

        protected override void OnClose()
        {
            WebMessageHandlerOld.Instance.RemoveHandler(1, 5, OnChangeStatusResponse);
            WebMessageHandlerOld.Instance.RemoveHandler(1, 6, OnChangeNameResponse);

            if (_guideId > 0 && UnicornUIHelper.TryGetUI(UIType.UIPanel_Main, out UI ui))
            {
                var uis = ui as UIPanel_Main;
                uis.OnGuideIdFinished(_guideId);
            }

            //_guideId>0?
            base.OnClose();
        }
    }
}