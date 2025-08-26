//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using cfg.config;
using Common;
using Google.Protobuf;
using HotFix_UI;
using Unity.Mathematics;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Person_UserInfo)]
    internal sealed class UISubPanel_Person_UserInfoEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UISubPanel_Person_UserInfo;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Person_UserInfo>();
        }
    }

    public partial class UISubPanel_Person_UserInfo : UI, IAwake
    {
        #region

        private UI LastBottom;
        private UI BottomUI;
        private List<UI> BottomList = new List<UI>();
        private List<UI> UIList = new List<UI>();
        private Tblanguage tblanguage;
        private Tbuser_avatar tbuser_Avatar;
        private Tbquality tbquality;
        private Dictionary<int, int> HeadLockDic = new Dictionary<int, int>();
        private Dictionary<int, int> FrameLockDic = new Dictionary<int, int>();
        private UI LastUI;
        private int HeadHelp = 1001;
        private int FrameHelp = 2001;
        private bool isHead = false;
        private bool isFrame = false;

        #endregion

        public void Initialize()
        {
            NetInit();
            DataInit();
            BtnNameAndIDInit();
            BottomInit();
            HeadImgInit();
            TextInit();
            HeadInit();
        }

        private void DataInit()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbuser_Avatar = ConfigManager.Instance.Tables.Tbuser_avatar;
            tbquality = ConfigManager.Instance.Tables.Tbquality;
            foreach (user_avatar ua in tbuser_Avatar.DataList)
            {
                if (ua.type == 1)
                {
                    HeadLockDic.Add(ua.id, 0);
                }

                if (ua.type == 2)
                {
                    FrameLockDic.Add(ua.id, 0);
                }
            }

            foreach (var state in ResourcesSingletonOld.Instance.UserInfo.AvatarMap.AvatarList)
            {
                if (state.Type == 1)
                {
                    HeadLockDic[state.Id] = 1;
                }

                if (state.Type == 2)
                {
                    FrameLockDic[state.Id] = 1;
                }
            }

            HeadHelp = ResourcesSingletonOld.Instance.UserInfo.RoleAvatar;
            FrameHelp = ResourcesSingletonOld.Instance.UserInfo.RoleAvatarFrame;

            #region ������

            //HeadLockDic[1004] = 1;
            //HeadLockDic[1007] = 1;
            //FrameLockDic[2005] = 1;
            //FrameLockDic[2006] = 1;

            #endregion

            LockSorting();
        }

        private void BtnNameAndIDInit()
        {
            var nameBtn = this.GetFromReference(KBtn_Name);
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(nameBtn, () =>
            {
                //NetWorkManager.Instance.SendMessage(CMDOld.CHANGESTATUS);
                OpenChangeName();
            });

            var copyBtn = this.GetFromReference(KBtn_ID);
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(copyBtn,
                () => { GUIUtility.systemCopyBuffer = ResourcesSingletonOld.Instance.UserInfo.UserId.ToString(); });
        }

        private void NetInit()
        {
            WebMessageHandlerOld.Instance.AddHandler(1, 2, OnChangeAvaterResponse);
        }

        private void LockSorting()
        {
            HeadLockDic = SortingHelp(HeadLockDic);
            FrameLockDic = SortingHelp(FrameLockDic);
        }

        private Dictionary<int, int> SortingHelp(Dictionary<int, int> LockDic)
        {
            List<int2> helpList1 = new List<int2>();
            List<int2> helpList0 = new List<int2>();

            foreach (var hd in LockDic)
            {
                if (hd.Value == 1)
                {
                    helpList1.Add(new int2(hd.Key, hd.Value));
                }

                if (hd.Value == 0)
                {
                    helpList0.Add(new int2(hd.Key, hd.Value));
                }
            }

            helpList1.Sort(delegate(int2 a, int2 b)
            {
                return tbuser_Avatar[a.x].sort.CompareTo(tbuser_Avatar[b.x].sort);
            });
            helpList0.Sort(delegate(int2 a, int2 b)
            {
                return tbuser_Avatar[a.x].sort.CompareTo(tbuser_Avatar[b.x].sort);
            });

            Dictionary<int, int> helpDic = new Dictionary<int, int>();

            for (int i = 0; i < helpList1.Count; i++)
            {
                helpDic.Add(helpList1[i].x, helpList1[i].y);
            }

            for (int i = 0; i < helpList0.Count; i++)
            {
                helpDic.Add(helpList0[i].x, helpList0[i].y);
            }

            return helpDic;
        }

        private void HeadInit()
        {
            foreach (var hd in HeadLockDic)
            {
                var ui = UIHelper.Create(UIType.UISubPanel_Person_Head,
                    this.GetFromReference(KScrollView).GetScrollRect().Content.GameObject.transform, true);
                UIList.Add(ui);

                ui.GetFromReference(UISubPanel_Person_Head.KBtn_Use).SetActive(false);

                ui.GetFromReference(UISubPanel_Person_Head.KText_Quality).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get(tbquality[tbuser_Avatar[hd.Key].quality].name).current);
                ui.GetFromReference(UISubPanel_Person_Head.KText_Desc).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get(tbuser_Avatar[hd.Key].desc).current);
                ui.GetFromReference(UISubPanel_Person_Head.KText_HeadName).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get(tbuser_Avatar[hd.Key].name).current);
                ui.GetFromReference(UISubPanel_Person_Head.KText_Use).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("item_state_use").current);
                ui.GetFromReference(UISubPanel_Person_Head.KText_Using).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("common_state_using").current + "...");


                HeadImageSet(ui, hd);
                if (hd.Key == ResourcesSingletonOld.Instance.UserInfo.RoleAvatar)
                {
                    LastUI = ui;
                    ui.GetFromReference(UISubPanel_Person_Head.KBg_NotSelect).SetActive(false);
                    ui.GetFromReference(UISubPanel_Person_Head.KBg_Select).SetActive(true);
                    ui.GetFromReference(UISubPanel_Person_Head.KText_Using).SetActive(true);
                }
                else
                {
                    ui.GetFromReference(UISubPanel_Person_Head.KBg_NotSelect).SetActive(true);
                    ui.GetFromReference(UISubPanel_Person_Head.KBg_Select).SetActive(false);
                    ui.GetFromReference(UISubPanel_Person_Head.KText_Using).SetActive(false);
                }

                if (hd.Value == 1)
                {
                    ui.GetFromReference(UISubPanel_Person_Head.KIcon_Lock).SetActive(false);
                    ui.GetFromReference(UISubPanel_Person_Head.KImg_Lock).SetActive(false);
                    ui.GetFromReference(UISubPanel_Person_Head.KText_Desc).SetActive(false);
                    if (hd.Key != ResourcesSingletonOld.Instance.UserInfo.RoleAvatar)
                    {
                        ui.GetFromReference(UISubPanel_Person_Head.KBtn_Use).SetActive(true);
                    }
                }
                else
                {
                    ui.GetFromReference(UISubPanel_Person_Head.KIcon_Lock).SetActive(true);
                    ui.GetFromReference(UISubPanel_Person_Head.KImg_Lock).SetActive(true);
                    ui.GetFromReference(UISubPanel_Person_Head.KText_Desc).SetActive(true);
                }

                ui.GetFromReference(UISubPanel_Person_Head.KBtn_Select).GetXButton().OnClick.Add(() =>
                {
                    if (LastUI != ui)
                    {
                        ui.GetFromReference(UISubPanel_Person_Head.KBg_NotSelect).SetActive(true);
                        LastUI.GetFromReference(UISubPanel_Person_Head.KBg_Select).SetActive(false);
                        LastUI.GetFromReference(UISubPanel_Person_Head.KBg_NotSelect).SetActive(true);
                        HeadSet(hd.Key);

                        LastUI = ui;
                    }
                });

                var ChangeBtn = ui.GetFromReference(UISubPanel_Person_Head.KBtn_Use);
                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(ChangeBtn, () =>
                {
                    if (LastUI != ui && hd.Value == 1)
                    {
                        isHead = true;
                        isFrame = false;

                        GameRole gameRole = ResourcesSingletonOld.Instance.UserInfo;
                        gameRole.RoleAvatar = hd.Key;
                        NetWorkManager.Instance.SendMessage(1, 2, gameRole);
                    }
                });
            }

            HeadPos();
        }

        private async void HeadImageSet(UI ui, KeyValuePair<int, int> hd)
        {
            await ui.GetFromReference(UISubPanel_Person_Head.KImg_Head).GetImage()
                .SetSpriteAsync(tbuser_Avatar[hd.Key].icon, false);
            //TODO:
            ui.GetFromReference(UISubPanel_Person_Head.KText_Quality).GetTextMeshPro().SetColor(Color.blue);
        }

        private void FrameInit()
        {
            foreach (var hd in FrameLockDic)
            {
                var ui = UIHelper.Create(UIType.UISubPanel_Person_Frame,
                    this.GetFromReference(KScrollView).GetScrollRect().Content.GameObject.transform, true);
                UIList.Add(ui);

                ui.GetFromReference(UISubPanel_Person_Frame.KText_Desc).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get(tbuser_Avatar[hd.Key].desc).current);
                ui.GetFromReference(UISubPanel_Person_Frame.KText_Name).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get(tbuser_Avatar[hd.Key].name).current);
                ui.GetFromReference(UISubPanel_Person_Frame.KText_Quality).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get(tbquality[tbuser_Avatar[hd.Key].quality].name).current);
                ui.GetFromReference(UISubPanel_Person_Frame.KText_Use).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("item_state_use").current);
                ui.GetFromReference(UISubPanel_Person_Frame.KText_Using).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("common_state_using").current);


                FrameImageSet(ui, hd);

                if (hd.Key == ResourcesSingletonOld.Instance.UserInfo.RoleAvatarFrame)
                {
                    LastUI = ui;
                    ui.GetFromReference(UISubPanel_Person_Frame.KBg_Select).SetActive(true);


                    ui.GetFromReference(UISubPanel_Person_Frame.KText_Using).SetActive(true);
                    ui.GetFromReference(UISubPanel_Person_Frame.KBtn_Use).SetActive(false);
                    ui.GetFromReference(UISubPanel_Person_Frame.KText_Desc).SetActive(false);
                }
                else
                {
                    ui.GetFromReference(UISubPanel_Person_Frame.KBg_Select).SetActive(false);
                    ui.GetFromReference(UISubPanel_Person_Frame.KText_Using).SetActive(false);

                    if (hd.Value == 1)
                    {
                        ui.GetFromReference(UISubPanel_Person_Frame.KBtn_Use).SetActive(true);
                        ui.GetFromReference(UISubPanel_Person_Frame.KText_Desc).SetActive(false);
                    }
                    else
                    {
                        ui.GetFromReference(UISubPanel_Person_Frame.KBtn_Use).SetActive(false);
                        ui.GetFromReference(UISubPanel_Person_Frame.KText_Desc).SetActive(true);
                    }
                }

                if (hd.Value == 1)
                {
                    ui.GetFromReference(UISubPanel_Person_Frame.KImg_Lock).SetActive(false);
                    ui.GetFromReference(UISubPanel_Person_Frame.KIcon_Lock).SetActive(false);
                }
                else
                {
                    ui.GetFromReference(UISubPanel_Person_Frame.KImg_Lock).SetActive(true);
                    ui.GetFromReference(UISubPanel_Person_Frame.KIcon_Lock).SetActive(true);
                }

                ui.GetFromReference(UISubPanel_Person_Frame.KBtn_Select).GetXButton().OnClick.Add(() =>
                {
                    if (LastUI != ui)
                    {
                        LastUI.GetFromReference(UISubPanel_Person_Frame.KBg_Select).SetActive(false);
                        ui.GetFromReference(UISubPanel_Person_Head.KBg_NotSelect).SetActive(true);
                        ui.GetFromReference(UISubPanel_Person_Head.KBg_NotSelect).SetActive(false);
                        ui.GetFromReference(UISubPanel_Person_Frame.KBg_Select).SetActive(true);
                        FrameSet(hd.Key);

                        LastUI = ui;
                    }
                });

                var changeBtn = ui.GetFromReference(UISubPanel_Person_Frame.KBtn_Use);
                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(changeBtn, () =>
                {
                    if (LastUI != ui && hd.Value == 1)
                    {
                        isHead = false;
                        isFrame = true;

                        GameRole gameRole = ResourcesSingletonOld.Instance.UserInfo;
                        gameRole.RoleAvatarFrame = hd.Key;
                        NetWorkManager.Instance.SendMessage(1, 2, gameRole);
                    }
                });
            }

            FramePos();
        }

        private async void FrameImageSet(UI ui, KeyValuePair<int, int> hd)
        {
            //TODO:
            //await ui.GetFromReference(UISubPanel_Person_Frame.KImg_Quality).GetImage()
            //    .SetSpriteAsync(tbquality[tbuser_Avatar[hd.Key].quality].bg, false);
            ui.GetFromReference(UISubPanel_Person_Frame.KImg_Frame).GetImage()
                .SetSprite(tbuser_Avatar[hd.Key].icon, true);
            float iconW = ui.GetFromReference(UISubPanel_Person_Frame.KImg_Frame).GetRectTransform().Width();
            float scrollW = ui.GetFromReference(UISubPanel_Person_Frame.KScrollView).GetScrollRect().Content
                .GetRectTransform().Width();
            ui.GetFromReference(UISubPanel_Person_Frame.KImg_Frame).GetRectTransform()
                .SetScale(new Vector2(scrollW / iconW, scrollW / iconW));
        }

        private void HeadPos()
        {
            if (UIList.Count > 0)
            {
                float itemH = UIList[0].GetFromReference(UISubPanel_Person_Head.KBg_Select).GetRectTransform().Height();
                float ScrollW = this.GetFromReference(KScrollView).GetRectTransform().Width() - 80;
                int HeadNum = UIList.Count;
                int HeadH = HeadNum / 2 + HeadNum % 2;
                float ContentH = itemH * HeadH + (HeadH - 1) * 30;
                this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetHeight(ContentH);
                this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetOffsetWithLeft(0);
                this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetOffsetWithRight(0);
                for (int i = 0; i < HeadNum; i++)
                {
                    int ihelp = i;
                    UIList[ihelp].GetRectTransform()
                        .SetAnchoredPositionY(-(itemH / 2 + (ihelp / 2) * (itemH + 30)) + ContentH / 2);
                    if (ihelp % 2 == 0)
                    {
                        UIList[ihelp].GetRectTransform().SetAnchoredPositionX(-ScrollW / 4);
                    }
                    else
                    {
                        UIList[ihelp].GetRectTransform().SetAnchoredPositionX(ScrollW / 4);
                    }
                }
            }

            this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetAnchoredPositionY(0);
        }

        private void FramePos()
        {
            if (UIList.Count > 0)
            {
                float itemH = UIList[0].GetFromReference(UISubPanel_Person_Frame.KBg_Select).GetRectTransform()
                    .Height();
                int FrameNum = UIList.Count;
                float ContentH = itemH * FrameNum + (FrameNum - 1) * 30;
                this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetHeight(ContentH);
                this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetOffsetWithLeft(0);
                this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetOffsetWithRight(0);
                for (int i = 0; i < FrameNum; i++)
                {
                    int ihelp = i;
                    UIList[ihelp].GetRectTransform().SetOffsetWithLeft(50);
                    UIList[ihelp].GetRectTransform().SetOffsetWithRight(-50);

                    UIList[ihelp].GetRectTransform().SetAnchoredPositionY(-ihelp * (itemH + 30));
                }
            }

            this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetAnchoredPositionY(0);
        }

        private async void BottomInit()
        {
            BottomUI = await UIHelper.CreateAsync(this, UIType.UICommon_Bottom, this.GameObject.transform);
            var backUi = BottomUI.GetFromReference(UICommon_Bottom.KBtn_Close);
            BottomUI.GetRectTransform().SetAnchoredPosition(Vector2.zero);
            //BottomUI.GetImage().SetAlpha(0);
            var scrollView = BottomUI.GetFromReference(UICommon_Bottom.KScrollView_Item0);
            var horizontalUi = scrollView.GetScrollRect().Content;
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(backUi, () => { Close(); });

            for (int i = 0; i < 2; i++)
            {
                int ihelp = i;
                var itemUI =
                    await UIHelper.CreateAsync(this, UIType.UICommon_BottomBtn, horizontalUi.GameObject.transform);
                BottomList.Add(itemUI);
                if (i == 0)
                {
                    LastBottom = itemUI;
                    itemUI.GetRectTransform().SetScale(new Vector2(1.1f, 1.1f));
                }

                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(itemUI, () =>
                {
                    if (LastBottom != itemUI)
                    {
                        LastBottom.GetRectTransform().DoScale2(new Vector2(1f, 1f), 0.2f);
                        itemUI.GetRectTransform().DoScale2(new Vector2(1.1f, 1.1f), 0.2f);
                        LastBottom = itemUI;
                        DestroyList();
                        HeadOrFrameInit(ihelp);
                    }
                });
            }

            BottomList[0].GetFromReference(UICommon_BottomBtn.KText_Btn).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("user_info_avatar_name").current);
            BottomList[1].GetFromReference(UICommon_BottomBtn.KText_Btn).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("user_info_bg_name").current);
        }

        private void HeadOrFrameInit(int input)
        {
            if (input == 0)
            {
                HeadInit();
            }
            else if (input == 1)
            {
                FrameInit();
            }
            else
            {
                Debug.Log("head or frame intit is wrong");
            }
        }

        private async void HeadImgInit()
        {
            await this.GetFromReference(KImg_Head).GetImage()
                .SetSpriteAsync(tbuser_Avatar[ResourcesSingletonOld.Instance.UserInfo.RoleAvatar].icon, false);
            //await this.GetFromReference(KBg_Head).GetImage()
            //    .SetSpriteAsync(tbuser_Avatar[ResourcesSingletonOld.Instance.UserInfo.RoleAvatarFrame].icon, false);
        }

        private async void HeadSet(int headID)
        {
            await this.GetFromReference(KImg_Head).GetImage().SetSpriteAsync(tbuser_Avatar[headID].icon, false);
        }

        private async void FrameSet(int frameID)
        {
            //await this.GetFromReference(KBg_Head).GetImage().SetSpriteAsync(tbuser_Avatar[frameID].icon, false);
        }

        private void TextInit()
        {
            this.GetFromReference(KText_Name).GetTextMeshPro()
                .SetTMPText(ResourcesSingletonOld.Instance.UserInfo.Nickname);
            this.GetFromReference(KText_ID).GetTextMeshPro()
                .SetTMPText("ID:" + ResourcesSingletonOld.Instance.UserInfo.UserId.ToString());
        }

        private void DestroyList()
        {
            foreach (var VARIABLE in UIList)
            {
                VARIABLE.Dispose();
            }


            UIList.Clear();
        }

        private void DestroyBottomUI()
        {
            BottomUI.Dispose();
            int BottomListCount = BottomList.Count;
            foreach (var VARIABLE in BottomList)
            {
                VARIABLE.Dispose();
            }

            BottomList.Clear();
        }

        private void OnChangeAvaterResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            var gameRole = new GameRole();
            gameRole.MergeFrom(e.data);
            Debug.Log(e);
            Debug.Log(gameRole);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                ResourcesSingletonOld.Instance.UserInfo.RoleAvatar = HeadHelp;
                ResourcesSingletonOld.Instance.UserInfo.RoleAvatarFrame = FrameHelp;
                DestroyList();
                if (isHead)
                {
                    HeadInit();
                }

                if (isFrame)
                {
                    FrameInit();
                }

                HeadImgInit();
                return;
            }

            ResourcesSingletonOld.Instance.UserInfo.RoleAvatar = gameRole.RoleAvatar;
            ResourcesSingletonOld.Instance.UserInfo.RoleAvatarFrame = gameRole.RoleAvatarFrame;

            DestroyList();
            if (isHead)
            {
                HeadInit();
            }

            if (isFrame)
            {
                FrameInit();
            }

            HeadImgInit();

            var parent = this.GetParent<UIPanel_Person>();
            if (parent != null)
            {
                parent.UpdatePerson();
            }
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

         

            var ui = await UIHelper.CreateAsync(UIType.UISubPanel_Person_ChangeName, checkResult,0);
            // var ui = await UIHelper.CreateAsync<CheckResult>(UIType.UISubPanel_Person_ChangeName, checkResult,
            //     UILayer.Mid);
            ui.SetParent(this, false);
        }

        public void SetName()
        {
            TextInit();
            var parent = this.GetParent<UIPanel_Person>();
            if (parent != null)
            {
                parent.UpdatePerson();
            }
        }

        public void OpenChangeName()
        {
            WebMessageHandlerOld.Instance.AddHandler(CMDOld.CHANGESTATUS, OnChangeNameStatusResponse);
            NetWorkManager.Instance.SendMessage(CMDOld.CHANGESTATUS);
        }

        protected override void OnClose()
        {
            WebMessageHandlerOld.Instance.RemoveHandler(1, 2, OnChangeAvaterResponse);
            DestroyList();
            DestroyBottomUI();
            base.OnClose();
        }
    }
}