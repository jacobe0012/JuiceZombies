//---------------------------------------------------------------------
// UnicornStudio
// Author: huangjinguo
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Google.Protobuf;
using HotFix_UI;
using UnityEngine;


namespace XFramework
{
    [UIEvent(UIType.UIPanel_Person)]
    internal sealed class UIPanel_PersonEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Person;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Person>();
        }
    }

    public partial class UIPanel_Person : UI, IAwake
    {
        #region

        private GameRole UserInfo;
        private Tblanguage tblanguage;
        private Tbuser_avatar tbuser_Avatar;
        private Tbtag_func tbtag_Func;
        private List<tag_func> tgfList1 = new List<tag_func>();
        private List<tag_func> tgfList2 = new List<tag_func>();
        private Dictionary<int, UI> UIDic1 = new Dictionary<int, UI>();
        private Dictionary<int, UI> UIDic2 = new Dictionary<int, UI>();
        public string Person_Red_Point_Root = "Person_Red_Point_Root";
        private CancellationTokenSource cts;

        #endregion

        public int tagId = 5;
        private string m_RedDotName;

        public void Initialize()
        {
            cts = new CancellationTokenSource();
            WebMessageHandlerOld.Instance.AddHandler(99, 3, OnHttpTestResponse);
            //RedPointMgr.instance.Add(Person_Red_Point_Root, null, null, RedPointType.Enternal);
            InitRedDot();
            DataInit();
            HeadImgInit();
            TextInit();
            BtnInit();
            InitRedDot();
            RefreshResourceUI();
            //RefreshPersonRedPoint();
        }

        private void InitRedDot()
        {
        }

        public void RefreshResourceUI()
        {
            var KText_Gem = GetFromReference(UIPanel_Person.KText_Gem);
            var KText_Gold = GetFromReference(UIPanel_Person.KText_Gold);
            var KText_Energy = GetFromReference(UIPanel_Person.KText_Energy);

            var icon = UnicornUIHelper.GetRewardTextIconName(UnicornUIHelper.GetVector3(UnicornUIHelper.Vector3Type.BITCOIN));
            // icon= UnityHelper.RichTextSize(icon, 50);
            KText_Gem.GetTextMeshPro().SetTMPText(icon + UnicornUIHelper.ReturnFormatResourceNum(1));

            icon = UnicornUIHelper.GetRewardTextIconName(UnicornUIHelper.GetVector3(UnicornUIHelper.Vector3Type.DOLLARS));
            /// icon = UnityHelper.RichTextSize(icon, 50);
            KText_Gold.GetTextMeshPro().SetTMPText(icon + UnicornUIHelper.ReturnFormatResourceNum(2));

            icon = UnicornUIHelper.GetRewardTextIconName(UnicornUIHelper.GetVector3(UnicornUIHelper.Vector3Type.ENEERGY));
            // icon = UnityHelper.RichTextSize(icon, 50);
            KText_Energy.GetTextMeshPro()
                .SetTMPText(icon + UnicornUIHelper.ReturnFormatResourceNum(3) + "/" +
                            $"{ResourcesSingletonOld.Instance.UserInfo.RoleAssets.EnergyMax}");
        }

        private void OnHttpTestResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            Debug.Log("OnHttpTestResponse receive");
        }

        private void DataInit()
        {
            UserInfo = ResourcesSingletonOld.Instance.UserInfo;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbtag_Func = ConfigManager.Instance.Tables.Tbtag_func;
            tbuser_Avatar = ConfigManager.Instance.Tables.Tbuser_avatar;
            tgfList1 = new List<tag_func>();
            tgfList2 = new List<tag_func>();
            foreach (var tag_func in tbtag_Func.DataList)
            {
                if (tag_func.tagId == 5)
                {
                    if (tag_func.posType == 1)
                    {
                        tgfList1.Add(tag_func);
                    }

                    if (tag_func.posType == 2 && tag_func.id != 5203)
                    {
                        Log.Debug($"id11111:{tag_func}", Color.cyan);
                        tgfList2.Add(tag_func);
                    }
                }
            }

            tgfList1.Sort(delegate(tag_func tf1, tag_func tf2) { return tf1.sort.CompareTo(tf2.sort); });
            tgfList2.Sort(delegate(tag_func tf1, tag_func tf2) { return tf1.sort.CompareTo(tf2.sort); });


            for (int i = 0; i < tgfList2.Count; i++)
            {
                Log.Debug($"id2222:{tgfList2[i]}", Color.cyan);
            }
        }

        private void TextInit()
        {
            this.GetFromReference(KText_Name).GetTextMeshPro().SetTMPText(UserInfo.Nickname);
            this.GetFromReference(KText_ID).GetTextMeshPro().SetTMPText("ID:" + UserInfo.UserId.ToString());
        }

        private async void OnChangeNameStatusResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.CHANGESTATUS, OnChangeNameStatusResponse);
            var checkResult = new CheckResult();
            checkResult.MergeFrom(e.data);
            Debug.Log(checkResult);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }


            var ui = await UIHelper.CreateAsync(UIType.UISubPanel_Person_ChangeName, checkResult, 0);
            // var ui = await UIHelper.CreateAsync<CheckResult>(UIType.UISubPanel_Person_ChangeName, checkResult,
            //     UILayer.Mid);
            ui.SetParent(this, false);
        }

        private async void CreatePrompt(string str)
        {
            UnicornUIHelper.ClearCommonResource();
            await UIHelper.CreateAsync(UIType.UICommon_Resource, str);
        }

        private void BtnInit()
        {
            // var achiveID = 5105;
            var curUI = this.GetFromReference(KTxt_Achievement);
            var curImg = this.GetFromReference(KImg_Achievement);
            var curBtn = this.GetFromReference(KBtn_Achievement);
            string strName = default;
            // curUI.GetTextMeshPro().SetTMPText(tblanguage.Get(strName).current);
            // BtnImgSet(tbtag_Func.Get(achiveID), curImg, false);


            // UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(curBtn,
            //     async () => { await UIHelper.CreateAsync(UIType.UIPanel_Achieve); });


            //#region 临时
            //UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_Settings), () => { UIHelper.CreateAsync(UIType.UIPanel_Settings); });
            ////await UIHelper.CreateAsync(UIType.UIPanel_Settings);
            //#endregion

            UIDic2.Clear();
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_Name), () =>
            {
                WebMessageHandlerOld.Instance.AddHandler(CMDOld.CHANGESTATUS, OnChangeNameStatusResponse);
                NetWorkManager.Instance.SendMessage(CMDOld.CHANGESTATUS);
            },1104);

            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_ID), () =>
            {
                GUIUtility.systemCopyBuffer = UserInfo.UserId.ToString();
                CreatePrompt(tblanguage.Get("text_copy_success").current);
            });


            this.GetFromReference(KTxtChangeZhuangban).GetTextMeshPro()
                .SetTMPText(tblanguage.Get(tblanguage.Get("func_5101_name").current).current);
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_ChangeZhuangban),
                () =>
                {
                    //UIHelper.Create(UIType.UISubPanel_Person_UserInfo);
                });


            foreach (var tf in tgfList1)
            {
                //Debug.LogError("fffffffffffffffffffffffffff");

                var path = NodeNames.GetTagFuncRedDotName(tf.id);

                if (ResourcesSingletonOld.Instance.redDots.TryGetValue(tf.id, out var value))
                {
                }

                switch (tf.id)
                {
                    case 5102:
                        curUI = this.GetFromReference(KTxt_Gonghui);
                        curImg = this.GetFromReference(KImg_Gonghui);
                        curBtn = this.GetFromReference(KBtn_Gonghui);
                        strName = tf.name;
                        curUI.GetTextMeshPro().SetTMPText(tblanguage.Get(strName).current);
                        BtnImgSet(tf, curImg, false);
                        UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(curBtn, () => { Btn51OnClick(tf.id); },1104);
                        break;
                    case 5103:

                        curUI = this.GetFromReference(KTxt_Friend);
                        curImg = this.GetFromReference(KImg_Friend);
                        curBtn = this.GetFromReference(KBtn_Friend);
                        strName = tf.name;
                        curUI.GetTextMeshPro().SetTMPText(tblanguage.Get(strName).current);
                        BtnImgSet(tf, curImg, false);
                        UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(curBtn, () => { Btn51OnClick(tf.id); },1104);
                        break;
                    case 5104:
                        curUI = this.GetFromReference(KTxt_Monster);
                        curImg = this.GetFromReference(KImg_Monster);
                        curBtn = this.GetFromReference(KBtn_Monster);
                        strName = tf.name;
                        curUI.GetTextMeshPro().SetTMPText(tblanguage.Get(strName).current);
                        BtnImgSet(tf, curImg, false);


                        UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(curBtn, () => { Btn51OnClick(tf.id); });

                        var KImg_MonsterRedDot = GetFromReference(UIPanel_Person.KImg_MonsterRedDot);

                        KImg_MonsterRedDot.SetActive(value > 0);
                        RedDotManager.Instance.AddListener(path, a => { KImg_MonsterRedDot.SetActive(a > 0); });

                        break;
                    case 5105:
                        curUI = this.GetFromReference(KTxt_Achievement);
                        curImg = this.GetFromReference(KImg_Achievement);
                        curBtn = this.GetFromReference(KBtn_Achievement);
                        strName = tf.name;
                        curUI.GetTextMeshPro().SetTMPText(tblanguage.Get(strName).current);
                        BtnImgSet(tf, curImg, false);


                        UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(curBtn, () => { Btn51OnClick(tf.id); },1104);

                        var KImg_AchieveRedDot = GetFromReference(UIPanel_Person.KImg_AchieveRedDot);

                        KImg_AchieveRedDot.SetActive(value > 0);
                        RedDotManager.Instance.AddListener(path, a => { KImg_AchieveRedDot.SetActive(a > 0); });

                        break;
                }
            }


            //var KImg_AchieveRedDot = GetFromReference(UIPanel_Person.KImg_AchieveRedDot);
            //UIDic1.Add(tf.id, curUI);

            //UnicornUIHelper.ForceRefreshLayout(this.GetFromReference(KPos_Btn1));


            #region 格伦新增

            //TODO:
            //int funcId = tf.id;
            //var KImg_RedDot = curUI.GetFromReference(UISubPanel_Person_Btn51.KImg_RedDot);
            //KImg_RedDot.SetActive(false);

            // int monsterCollectionId = 5104;
            // if (funcId == monsterCollectionId)
            // {
            //     var itemStr =
            //         $"{NodeNames.GetFuncRedDotName(monsterCollectionId)}";
            //     KImg_RedDot.SetActive(RedDotManager.Instance.GetRedPointCnt(itemStr) > 0);
            //     RedDotManager.Instance.AddListener(itemStr, (num) =>
            //     {
            //         Log.Error($"Person {itemStr} {num}");
            //         KImg_RedDot.SetActive(num > 0);
            //     });
            // }

            #endregion

            var uiBtnList52 = this.GetFromReference(KPos_Btn2).GetList();
            uiBtnList52.Clear();
            foreach (var tf in tgfList2)
            {
                Log.Debug($"id:{tf.id}");
                var ui = uiBtnList52.CreateWithUIType(UIType.UISubPanel_Person_Btn52, false);
                //UI ui = UIHelper.Create(UIType.UISubPanel_Person_Btn52, this.GetFromReference(KPos_Btn2).GameObject.transform, true);
                UIDic2.Add(tf.id, ui);
                var KBtn = ui.GetFromReference(UISubPanel_Person_Btn52.KBtn);
                var KText = ui.GetFromReference(UISubPanel_Person_Btn52.KText);
                var KBg_Btn = ui.GetFromReference(UISubPanel_Person_Btn52.KBg_Btn);
                var KImg_RedPoint = ui.GetFromReference(UISubPanel_Person_Btn52.KImg_RedPoint);
                KBg_Btn.GetRectTransform().SetAnchoredPositionY(0);
                //KImg_RedPoint.GetRectTransform().SetAnchoredPositionY(0);
                KBtn.GetRectTransform().SetAnchoredPositionY(0);
                //KText.GetRectTransform().SetAnchoredPositionY(-15);
                BtnImgSet(tf, ui.GetFromReference(UISubPanel_Person_Btn52.KBg_Btn), true);
                ui.GetFromReference(UISubPanel_Person_Btn52.KText).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get(tf.name).current);
                var btn52 = ui.GetFromReference(UISubPanel_Person_Btn52.KBtn);
                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(btn52, () => { Btn52OnClick(tf.id); }, 1104);
                KImg_RedPoint.SetActive(false);
                if (ResourcesSingletonOld.Instance.redDots.TryGetValue(tf.id, out var value))
                {
                    KImg_RedPoint.SetActive(value > 0);
                }

                if (tf.id == 5202)
                {
                    if (JsonManager.Instance.sharedData?.noticesList?.notices?.Where(a => a.readStatus == 0)
                            .Count() > 0)
                    {
                        KImg_RedPoint.SetActive(true);
                    }
                    else
                    {
                        KImg_RedPoint.SetActive(false);
                    }
                }

                m_RedDotName = NodeNames.GetTagFuncRedDotName(tf.id);
                RedDotManager.Instance.AddListener(m_RedDotName, (num) => { KImg_RedPoint?.SetActive(num > 0); });
            }

            UnicornUIHelper.ForceRefreshLayout(this.GetFromReference(KPos_Btn2));
            //PosSet();
        }

        private void Btn51OnClick(int input)
        {
            //ccccttttssss = new CancellationTokenSource();
            switch (input)
            {
                case 5101:
                    var ui = UIHelper.Create(UIType.UISubPanel_Person_UserInfo);
                    ui.SetParent(this, false);
                    break;
                case 5102:
                    //工会
                    //WebMessageHandlerOld.Instance.AddHandler(8, 1, Ontttt);
                    //NetWorkManager.Instance.SendMessage(8, 1);
                    Debug.Log("5102");
                    break;
                case 5103:
                    //好友
                    //WebMessageHandlerOld.Instance.AddHandler(15, 2, OntestR);
                    //NetWorkManager.Instance.SendMessage(15, 2);
                    //UIHelper.Create(UIType.UIPanel_Activity_SevenDays);
                    Debug.Log("5103");
                    break;
                case 5104:
                    UIHelper.CreateAsync(UIType.UIPanel_MonsterCollection);
                    //WebMessageHandlerOld.Instance.AddHandler(17, 1, Ontttt);
                    //NetWorkManager.Instance.SendMessage(17, 1);
                    Debug.Log("5104");
                    break;
                case 5105:
                    UIHelper.CreateAsync(UIType.UIPanel_Achieve);
                    //WebMessageHandlerOld.Instance.AddHandler(17, 1, Ontttt);
                    //NetWorkManager.Instance.SendMessage(17, 1);
                    Debug.Log("5104");
                    break;
                default:
                    Debug.Log("OnClick is wrong");
                    break;
            }
        }

        //private void DebugFundList()
        //{

        //}

        //private void 


        private void Ontttt(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(8, 1, Ontttt);
            //BoolValue boolValue = new BoolValue();
            //boolValue.MergeFrom(e.data);
            //Debug.Log(boolValue.Value);
            ActivityMap activityMap = new ActivityMap();
            activityMap.MergeFrom(e.data);
            Debug.Log(activityMap);
        }

        private void OntestR(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(15, 2, OntestR);
            Debug.Log(e.data);
        }

        private void OnShopTestResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.QUERYBANK, OnShopTestResponse);
            var shopMap = new GoldPig();
            shopMap.MergeFrom(e.data);
            Debug.Log(shopMap);

            Debug.Log(TimeHelper.ClientNowSeconds().ToString());
            Debug.Log("GoldBankTime" + shopMap.GoldBankTime.ToString());

            if (e.data.IsEmpty)
            {
                Debug.Log("e is empty");
                return;
            }
        }

        public void SetUpdateDataDelay()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            //UpdateDataByServerOneDay(cts.Token).Forget();
        }

        //public async UniTaskVoid UpdateDataByServerOneDay(CancellationToken cancellationToken)
        //{
        //    //CancellationTokenSource cts = new CancellationTokenSource();
        //    var dTime = ResourcesSingletonOld.Instance.ServerDeltaTime;
        //    var uTime = ResourcesSingletonOld.Instance.UpdateTime;
        //    long updateTime = uTime - dTime / 1000 + 1;
        //    await UniTask.Delay((int)(updateTime - TimeHelper.ClientNowSeconds()) * 1000, false, PlayerLoopTiming.Update, cancellationToken);
        //    SetTest();
        //}

        public void SetTest()
        {
            WebMessageHandlerOld.Instance.AddHandler(CMDOld.QUERYBANK, OnShopTestResponse);
            NetWorkManager.Instance.SendMessage(CMDOld.QUERYBANK);
        }

        private async void Btn52OnClick(int input)
        {
            switch (input)
            {
                case 5201:
                    var ui = await UIHelper.CreateAsync(UIType.UIPanel_Mail);
                    UnicornTweenHelper.SetEaseAlphaAndPosB2U(ui.GetFromReference(UIPanel_Mail.KMain), 0, 100, cts.Token,
                        0.15f, false);
                    UnicornTweenHelper.SetEaseAlphaAndPosLtoR(ui.GetFromReference(UIPanel_Mail.KMain), 0, 100, cts.Token,
                        0.15f, false);
                    ui.GetFromReference(UIPanel_Mail.KMain).GetComponent<CanvasGroup>().alpha = 0f;
                    ui.GetFromReference(UIPanel_Mail.KMain).GetComponent<CanvasGroup>().DOFade(1, 0.3f)
                        .SetEase(Ease.InQuad);

                    break;
                case 5202:


                    Debug.Log("5202");
                    ui = UIHelper.Create(UIType.UIPanel_Notice);
                    UnicornTweenHelper.SetEaseAlphaAndPosB2U(ui.GetFromReference(UIPanel_Notice.KImg_Bg), 0, 100, cts.Token,
                        0.15f,
                        false);
                    UnicornTweenHelper.SetEaseAlphaAndPosLtoR(ui.GetFromReference(UIPanel_Notice.KImg_Bg), 0, 100, cts.Token,
                        0.15f,
                        false);
                    ui.GetFromReference(UIPanel_Notice.KImg_Bg).GetComponent<CanvasGroup>().alpha = 0f;
                    ui.GetFromReference(UIPanel_Notice.KImg_Bg).GetComponent<CanvasGroup>().DOFade(1, 0.3f)
                        .SetEase(Ease.InQuad);
                    break;
                case 5204:
                    Debug.Log("5204");
                    ui = await UIHelper.CreateAsync(UIType.UIPanel_Settings);
                    UnicornTweenHelper.SetEaseAlphaAndPosB2U(ui.GetFromReference(UIPanel_Settings.KMid), 0, 100, cts.Token,
                        0.15f,
                        false);
                    UnicornTweenHelper.SetEaseAlphaAndPosLtoR(ui.GetFromReference(UIPanel_Settings.KMid), 0, 100, cts.Token,
                        0.15f,
                        false);
                    ui.GetFromReference(UIPanel_Settings.KMid).GetComponent<CanvasGroup>().alpha = 0f;
                    ui.GetFromReference(UIPanel_Settings.KMid).GetComponent<CanvasGroup>().DOFade(1, 0.3f)
                        .SetEase(Ease.InQuad);
                    break;
                default:
                    Debug.Log("52OnClick is wrong");
                    break;
            }
        }

        private void PosSet()
        {
            Debug.Log("PosSet1");
            float PosH = 0;

            foreach (var ui1 in UIDic1)
            {
                ui1.Value.GetRectTransform().SetAnchoredPositionY(PosH -
                                                                  ui1.Value.GetFromReference(UISubPanel_Person_Btn51
                                                                      .KBtn).GetRectTransform().Height() / 2);
                PosH -= ui1.Value.GetRectTransform(UISubPanel_Person_Btn51.KBtn).Height();
                PosH -= 15;
            }

            Debug.Log("PosSet2");
        }

        private async void HeadImgInit()
        {
            await this.GetFromReference(KImg_Head).GetImage()
                .SetSpriteAsync(tbuser_Avatar[ResourcesSingletonOld.Instance.UserInfo.RoleAvatar].icon, false);
            //await this.GetFromReference(KBg_Head).GetImage()
            //    .SetSpriteAsync(tbuser_Avatar[ResourcesSingletonOld.Instance.UserInfo.RoleAvatarFrame].icon, false);
        }

        private void BtnImgSet(tag_func tf, UI ui, bool isNactiveSize = false)
        {
            ui.GetImage().SetSprite(tf.icon, isNactiveSize);
        }

        public void UpdatePerson()
        {
            HeadImgInit();
            TextInit();
        }

        private void DestroyUI()
        {
            var uiBtnList52 = this.GetFromReference(KPos_Btn2).GetList();
            uiBtnList52.Clear();
        }

        public void RefreshPersonRedPoint()
        {
            //InitRedPoint();
            //SetRedPoint();
            ResourcesSingletonOld.Instance.UpdateResourceUI();
        }

        //private void InitRedPoint()
        //{
        //    RedPointMgr.instance.Remove(Person_Red_Point_Root, "notice");
        //    RedPointMgr.instance.Add(Person_Red_Point_Root, "notice", Person_Red_Point_Root, RedPointType.Enternal);
        //    RedPointMgr.instance.Init(Person_Red_Point_Root, "notice",
        //        (RedPointState state, int data) =>
        //        {
        //            UIDic2.Get(5202).GetFromReference(UISubPanel_Person_Btn52.KImg_RedPoint)
        //                .SetActive(state == RedPointState.Show);
        //        });
        //    //UIDic2.Get(5202).GetFromReference(UISubPanel_Person_Btn52.KImg_RedPoint).SetActive(true);
        //    if (JsonManager.Instance.sharedData.noticesList == null)
        //    {
        //    }
        //    else
        //    {
        //        if (JsonManager.Instance.sharedData.noticesList.notices == null)
        //        {
        //        }
        //        else
        //        {
        //            if (JsonManager.Instance.sharedData.noticesList.notices.Count == 0)
        //            {
        //            }
        //            else
        //            {
        //                foreach (var nt in JsonManager.Instance.sharedData.noticesList.notices)
        //                {
        //                    RedPointMgr.instance.Add(Person_Red_Point_Root, "notice" + nt.id.ToString(), "notice",
        //                        RedPointType.None);
        //                }
        //            }
        //        }
        //    }
        //}

        //private void SetRedPoint()
        //{
        //    if (JsonManager.Instance.sharedData.noticesList == null)
        //    {
        //    }
        //    else
        //    {
        //        if (JsonManager.Instance.sharedData.noticesList.notices == null)
        //        {
        //        }
        //        else
        //        {
        //            if (JsonManager.Instance.sharedData.noticesList.notices.Count == 0)
        //            {
        //            }
        //            else
        //            {
        //                foreach (var nt in JsonManager.Instance.sharedData.noticesList.notices)
        //                {
        //                    if (nt.readStatus == 0)
        //                    {
        //                        //unread
        //                        RedPointMgr.instance.SetState(Person_Red_Point_Root, "notice" + nt.id.ToString(),
        //                            RedPointState.Show);
        //                    }
        //                    else
        //                    {
        //                        //read
        //                        RedPointMgr.instance.SetState(Person_Red_Point_Root, "notice" + nt.id.ToString(),
        //                            RedPointState.Hide);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //private void RemoveRedPoint()
        //{
        //    RedPointMgr.instance.Remove(Person_Red_Point_Root, Person_Red_Point_Root);
        //}


        protected override void OnClose()
        {
            #region 格伦新增

            //移除该模块及其子树所有红点回调

            RedDotManager.Instance.ClearChildrenListeners(NodeNames.GetTagRedDotName(5));

            #endregion

            //RemoveRedPoint();
            //DestroyUI();
            cts.Cancel();
            base.OnClose();
        }
    }
}