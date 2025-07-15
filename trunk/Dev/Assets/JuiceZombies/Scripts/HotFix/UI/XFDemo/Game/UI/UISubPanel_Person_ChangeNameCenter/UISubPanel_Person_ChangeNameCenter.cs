//---------------------------------------------------------------------
// JiYuStudio
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
    [UIEvent(UIType.UISubPanel_Person_ChangeNameCenter)]
    internal sealed class UISubPanel_Person_ChangeNameCenterEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UISubPanel_Person_ChangeNameCenter;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Person_ChangeNameCenter>();
        }
    }

    public partial class UISubPanel_Person_ChangeNameCenter : UI, IAwake<CheckResult, int>
    {
        #region

        private Tblanguage tblanguage;
        private Tbconstant tbconstant;
        private Tbitem tbitem;
        private Tbuser_variable tbuser_Variable;

        //0�Ǹ���������ȴ״̬����ʾ��ȴʱ�䣬1������޸ģ�2�Ǹ������޸ģ�3�Ǹ����ʲ��������ʲ����㣬4�Ǹ����ʲ��������ʲ�������
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
            await JiYuUIHelper.InitBlur(this);
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
            JiYuTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(UISubPanel_Person_ChangeNameCenter.KBg), 0);
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
            WebMessageHandler.Instance.AddHandler(1, 6, OnChangeNameResponse);
            WebMessageHandler.Instance.AddHandler(1, 5, OnChangeStatusResponse);
        }

        private void TextInit()
        {
            this.GetFromReference(KText_Desc).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("user_info_name_change_desc").current);
            this.GetFromReference(KPlaceholder).GetTextMeshPro()
                .SetTMPText(ResourcesSingleton.Instance.UserInfo.Nickname);
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
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(ClearBtn, () =>
            {
                this.GetFromReference(KInputField).GetComponent<TMP_InputField>().text = "";
                this.GetFromReference(KText_Input).GetTextMeshPro().SetTMPText("");
            });

            var changeBtn = this.GetFromReference(KBtn_ChangeName);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(changeBtn, () =>
            {
                string nameStr = this.GetFromReference(KText_Input).GetTextMeshPro().Content;
                nameStr = CleanStr(nameStr);

                bool state = JudgeStatus(nameStr);

                if (state)
                {
                    //�򿪸������ƵĶ���ȷ�ϣ���ʱ�Ȳ��Ӷ���ȷ��
                    ResourcesSingleton.Instance.UserInfo.Nickname = nameStr;
                    var str = new StringValue();
                    str.Value = nameStr;
                    Debug.Log(nameStr);
                    NetWorkManager.Instance.SendMessage(1, 6, str);

                    if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out UI ui))
                    {
                        var uis = ui as UIPanel_Main;
                        uis.ChangePlayerInfo();
                    }

                }
                else
                {
                    CreatePrompt(tblanguage.Get("user_info_name_change_fail_2").current);
                    //��ͨ��UI��ʾ�������ƴʻ�
                    Debug.Log("��ͨ��UI��ʾ�������ƴʻ�");
                }
            });
        }

        private async void SelectStatus()
        {
            //isFree = 2;
            switch (isFree)
            {
                case 0:
                    //���������ȴʱ��
                    inCD = true;
                    this.GetFromReference(KText_InCD).GetTextMeshPro().SetTMPText(
                        tblanguage.Get("user_info_name_change_cd").current
                        + JiYuUIHelper.GeneralTimeFormat(new int4(2, 3, 1, 1), CDS));
                    this.GetFromReference(KBtn_ChangeName).SetActive(false);
                    this.GetFromReference(KText_InCD).SetActive(true);
                    this.GetFromReference(KImg_CardOrDiamond).SetActive(false);
                    this.GetFromReference(KText_Num).SetActive(false);
                    this.GetFromReference(KText_Change).SetActive(false);
                    this.GetFromReference(KText_Free).SetActive(false);
                    break;

                case 1:
                    //��������޸�
                    await this.GetFromReference(KIcon_Btn).GetImage().SetSpriteAsync(isFreeButtonStr, false);
                    this.GetFromReference(KBtn_ChangeName).SetActive(true);
                    this.GetFromReference(KText_InCD).SetActive(false);
                    this.GetFromReference(KImg_CardOrDiamond).SetActive(false);
                    this.GetFromReference(KText_Num).SetActive(false);
                    this.GetFromReference(KText_Change).SetActive(false);
                    this.GetFromReference(KText_Free).SetActive(true);
                    break;

                case 2:
                    //���Ը������޸�
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
                    //�����ʲ��������ʲ�����
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
                    //�����ʲ����������ʲ�������
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
        /// �ж����������Ƿ�ɸ��ģ�1�ǿ��Ը��ģ�0�ǰ������ƴʻ�
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
            JiYuUIHelper.ClearCommonResource();
            await UIHelper.CreateAsync(UIType.UICommon_Resource, str);
        }

        /// <summary>
        /// �ص�gameRole����ж�Ӧ����
        /// </summary>
        public void OnChangeNameResponse(object sender, WebMessageHandler.Execute e)
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
                    if (ResourcesSingleton.Instance.items.TryGetValue(1010001, out long value))
                    {
                        ResourcesSingleton.Instance.items[1010001] -= 1;
                    }
                    else
                    {
                        Debug.Log("Change Name Card id WRONG");
                    }
                }
                else if (isFree == 3)
                {
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin -=
                        tbconstant.Get("change_name_cost").constantValue;
                }

                CreatePrompt(tblanguage.Get("user_info_name_change_success").current);
                string namestr = this.GetFromReference(KText_Input).GetTextMeshPro().Content;
                ResourcesSingleton.Instance.UserInfo.Nickname = namestr;
                var parent = this.GetParent<UIPanel_Person>();
                parent.UpdatePerson();
                ResourcesSingleton.Instance.UpdateResourceUI();
                Close();
            }
        }

        private void Update()
        {
            if (inCD)
            {
                timeHelp++;
                if (timeHelp == 3000) //ÿ��һ����
                {
                    CDS -= 60; //��ȥ60��
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
        /// �ص�����״̬����ж�Ӧ����
        /// </summary>
        public void OnChangeStatusResponse(object sender, WebMessageHandler.Execute e)
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
            WebMessageHandler.Instance.RemoveHandler(1, 5, OnChangeStatusResponse);
            WebMessageHandler.Instance.RemoveHandler(1, 6, OnChangeNameResponse);

            if (_guideId > 0 && JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out UI ui))
            {
                var uis = ui as UIPanel_Main;
                uis.OnGuideIdFinished(_guideId);
            }

            //_guideId>0?
            base.OnClose();
        }
    }
}