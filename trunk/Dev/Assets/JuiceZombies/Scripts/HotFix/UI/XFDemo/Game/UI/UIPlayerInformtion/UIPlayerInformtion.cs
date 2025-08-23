//---------------------------------------------------------------------
// JiYuStudio
// Author: �ƽ��
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using Common;
using Google.Protobuf;
using HotFix_UI;
using Unity.Mathematics;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UIPlayerInformtion)]
    internal sealed class UIPlayerInformtionEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPlayerInformtion;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Mid;

        public override UI OnCreate()
        {
            return UI.Create<UIPlayerInformtion>();
        }
    }

    public partial class UIPlayerInformtion : UI, IAwake<GameRole>
    {
        public Dictionary<int, GameObject> UIHeadBtnDic = new Dictionary<int, GameObject>(10);
        public int UIHeadBtnlastIndex = -1;
        public Dictionary<int, GameObject> UIPIHeadFrameDic = new Dictionary<int, GameObject>(10);
        public int UIPIHeadFramelastIndex = -1;

        public Dictionary<int, GameObject> UIItemDic = new Dictionary<int, GameObject>(10);
        public int lastIndex = -1;
        private UI lastUI;

        private int HeadInt = 9;
        private int FrameInt = 9;
        public int HeadID = 1001;
        public int FrameID = 2001;

        private string UserName;
        private long UserID;

        public List<int2> HeadLockList = new List<int2>();
        public List<int2> FrameLockList = new List<int2>();

        //�����ɫ��Ϣ
        private GameRole role;

        private Transform itemPos;

        public struct ChangeNameHelp
        {
            public GameRole gameRole;
            public CheckResult checkResult;
        };

        public async void Initialize(GameRole _gameRole)
        {
            itemPos = this.GetFromReference(KItemPos).GameObject.transform;
            // JiYuTweenHelper.OpenPanelScale(this.GetFromReference(KBack));
            // JiYuTweenHelper.OpenPanelScale(this.GetFromReference(KItemPos));
            role = _gameRole;
            WebMessageHandlerOld.Instance.AddHandler(1, 2, OnChangeAvaterResponse);

            Log.Debug($"{_gameRole}");
            //����
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var userHead = ConfigManager.Instance.Tables.Tbuser_avatar.DataMap;
            var quality = ConfigManager.Instance.Tables.Tbquality.DataMap;

            //����ѡ��״̬��ʼ��
            bool isLeft = true;
            bool isRight = false;

            //��˶�ȡͷ���ID��ͷ��ID���ǳƺ��˺�ID
            HeadID = _gameRole.RoleAvatar;
            FrameID = _gameRole.RoleAvatarFrame;
            UserName = _gameRole.Nickname;
            UserID = _gameRole.UserId;
            //0��δ������1�ǽ���
            for (int i = 0; i < HeadInt; i++)
            {
                HeadLockList.Add(new int2(1001 + i, 0));
            }

            for (int i = 0; i < FrameInt; i++)
            {
                FrameLockList.Add(new int2(2001 + i, 0));
            }

            //���ݺ�����ݶ�ȡ����״̬
            foreach (var i in _gameRole.AvatarMap.AvatarList)
            {
                int indexHelper = i.Id % 1000 - 1;
                if (i.Type == 1)
                {
                    HeadLockList[indexHelper] = new int2(HeadLockList[indexHelper].x, 1);
                }

                if (i.Type == 2)
                {
                    FrameLockList[indexHelper] = new int2(FrameLockList[indexHelper].x, 1);
                }
            }

            //HeadLockList[3] = new int2(HeadLockList[3].x, 1);
            //HeadLockList[5] = new int2(HeadLockList[5].x, 1);

            //FrameLockList[2] = new int2(FrameLockList[2].x, 1);
            //FrameLockList[3] = new int2(FrameLockList[3].x, 1);


            //�Ժ�˴����Ľ���״̬���ݽ�������
            HeadLockList = LockSorting(HeadLockList, HeadInt);
            FrameLockList = LockSorting(FrameLockList, FrameInt);

            #region Init

            //�����ʼ��
            //��ʼ��ѡ��ť�����ı�
            //this.GetFromReference(KLeftNameTxt).GetTextMeshPro().SetTMPText(language["user_info_profile_name"].current);
            //this.GetFromReference(KRightNameTxt).GetTextMeshPro()
            //    .SetTMPText(language["user_info_profileframe_name"].current);
            //��ʼ������ǳ�
            this.GetFromReference(KNameTxt).GetTextMeshPro().SetTMPText(UserName);
            //��ʼ�����ID
            this.GetFromReference(KIDTxt).GetTextMeshPro()
                .SetTMPText(language["user_info_userid_name"].current + ": " + UserID.ToString());
            //��ʼ��ѡ��ť״̬
            GetFromReference(KLeftImage).GetImage().SetAlpha(255);
            GetFromReference(KRightImage).GetImage().SetAlpha(0);
            //��ʼ��ʹ����ͷ����ͷ���
            await this.GetFromReference(KHeadImage).GetImage().SetSpriteAsync(userHead[HeadID].icon, false);
            await this.GetFromReference(KFrame).GetImage().SetSpriteAsync(userHead[FrameID].icon, false);
            //��ʼ��ͷ��ѡ�����
            InitHeadItem(UIType.UIHeadBtn, UIItemDic, HeadInt, HeadID, HeadLockList,
                language["common_state_using"].current);
            //��ʼ��ʹ����ͷ������
            this.GetFromReference(KDescTxt).GetTextMeshPro().SetTMPText(language[userHead[HeadID].desc].current);
            this.GetFromReference(KQualityTxt).GetTextMeshPro()
                .SetTMPText(language[quality[userHead[HeadID].quality].name].current);
            this.GetFromReference(KQualityTxt).GetTextMeshPro()
                .SetColor("#" + quality[userHead[HeadID].quality].fontColor);
            // await this.GetFromReference(KQuality).GetImage()
            //     .SetSpriteAsync(quality[userHead[HeadID].quality].bg, false);
            //��ʼ��ʹ�ð�ť
            this.GetFromReference(KUseTxt).GetTextMeshPro().SetTMPText(language["item_state_use"].current);
            this.GetFromReference(KUseBtn).SetActive(false);
            this.GetFromReference(KStateTxt).GetTextMeshPro().SetTMPText(language["common_state_using"].current);

            #endregion

            var CloseBack = this.GetFromReference(KCloseBack);
            var CloseBtn = this.GetFromReference(KCloseBtn);
            var NameChange = this.GetFromReference(KNameChange);
            var UseBtn = this.GetFromReference(KUseBtn);

            //�ر������Ϣ����
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(CloseBack, () =>
            {
                CloseLastItemView(UIItemDic);
                Close();
            });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(CloseBtn, () =>
            {
                CloseLastItemView(UIItemDic);
                Close();
            });

            //�򿪸�������
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(NameChange, async () =>
            {
                WebMessageHandlerOld.Instance.AddHandler(CMD.CHANGESTATUS, OnChangeNameStatusResponse);
                NetWorkManager.Instance.SendMessage(CMD.CHANGESTATUS);
            });

            //��ͷ��ѡ�����
            this.GetButton(KLeftBtn)?.OnClick.Add(async () =>
            {
                GetFromReference(KLeftBtn);
                GetFromReference(KLeftImage).GetImage().SetAlpha(255);
                GetFromReference(KRightImage).GetImage().SetAlpha(0);

                if (isLeft) return;

                CloseLastItemView(UIItemDic);
                InitHeadItem(UIType.UIHeadBtn, UIItemDic, HeadInt, HeadID, HeadLockList,
                    language["common_state_using"].current);
                isLeft = true;
                isRight = false;

                //����ͷ���ָ�Ĭ��
                await this.GetFromReference(KFrame).GetImage().SetSpriteAsync(userHead[FrameID].icon, false);
                //����ͷ�������ָ�Ĭ��
                this.GetFromReference(KDescTxt).GetTextMeshPro().SetTMPText(language[userHead[HeadID].desc].current);
                this.GetFromReference(KQualityTxt).GetTextMeshPro()
                    .SetTMPText(language[quality[userHead[HeadID].quality].name].current);
                this.GetFromReference(KQualityTxt).GetTextMeshPro()
                    .SetColor("#" + quality[userHead[HeadID].quality].fontColor);
                // await this.GetFromReference(KQuality).GetImage()
                //     .SetSpriteAsync(quality[userHead[HeadID].quality].bg, false);
                //�ָ�ͷ��ʹ��״̬
                this.GetFromReference(KUseBtn).SetActive(false);
                this.GetFromReference(KStateTxt).GetTextMeshPro().SetTMPText(language["common_state_using"].current);
                this.GetFromReference(KStateTxt).SetActive(true);
            });

            //��ͷ���ѡ�����
            this.GetButton(KRightBtn)?.OnClick.Add(async () =>
            {
                GetFromReference(KRightBtn);
                GetFromReference(KRightImage).GetImage().SetAlpha(255);
                GetFromReference(KLeftImage).GetImage().SetAlpha(0);
                if (isRight) return;


                CloseLastItemView(UIItemDic);
                InitFrameItem(UIType.UIHeadFrameBtn, UIItemDic, FrameInt, FrameID, FrameLockList,
                    language["common_state_using"].current);

                isRight = true;
                isLeft = false;

                //����ͷ��ָ�Ĭ��
                await this.GetFromReference(KHeadImage).GetImage().SetSpriteAsync(userHead[HeadID].icon, false);
                //����ͷ��������ָ�Ĭ��
                this.GetFromReference(KDescTxt).GetTextMeshPro().SetTMPText(language[userHead[FrameID].desc].current);
                this.GetFromReference(KQualityTxt).GetTextMeshPro()
                    .SetTMPText(language[quality[userHead[FrameID].quality].name].current);
                this.GetFromReference(KQualityTxt).GetTextMeshPro()
                    .SetColor("#" + quality[userHead[FrameID].quality].fontColor);
                // await this.GetFromReference(KQuality).GetImage()
                //     .SetSpriteAsync(quality[userHead[FrameID].quality].bg, false);
                //�ָ�ͷ���ʹ��״̬
                this.GetFromReference(KUseBtn).SetActive(false);
                this.GetFromReference(KStateTxt).GetTextMeshPro().SetTMPText(language["common_state_using"].current);
                this.GetFromReference(KStateTxt).SetActive(true);
            });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(UseBtn, async () =>
            {
                //����ͷ��
                if (isLeft)
                {
                    HeadID = HeadLockList[lastIndex].x;
                    await this.GetFromReference(KHeadImage).GetImage().SetSpriteAsync(userHead[HeadID].icon, false);
                    this.GetFromReference(KUseBtn).SetActive(false);
                    this.GetFromReference(KStateTxt).GetTextMeshPro()
                        .SetTMPText(language["common_state_using"].current);
                    this.GetFromReference(KStateTxt).SetActive(true);
                    CloseLastItemView(UIItemDic);
                    _gameRole.RoleAvatar = HeadID;
                    NetWorkManager.Instance.SendMessage(1, 2, _gameRole);

                    InitHeadItem(UIType.UIHeadBtn, UIItemDic, HeadInt, HeadID, HeadLockList,
                        language["common_state_using"].current);
                }

                //����ͷ���
                if (isRight)
                {
                    FrameID = FrameLockList[lastIndex].x;
                    await this.GetFromReference(KFrame).GetImage().SetSpriteAsync(userHead[FrameID].icon, false);
                    this.GetFromReference(KUseBtn).SetActive(false);
                    this.GetFromReference(KStateTxt).GetTextMeshPro()
                        .SetTMPText(language["common_state_using"].current);
                    this.GetFromReference(KStateTxt).SetActive(true);
                    CloseLastItemView(UIItemDic);
                    _gameRole.RoleAvatarFrame = FrameID;
                    NetWorkManager.Instance.SendMessage(1, 2, _gameRole);

                    InitFrameItem(UIType.UIHeadFrameBtn, UIItemDic, FrameInt, FrameID, FrameLockList,
                        language["common_state_using"].current);
                }
            });

            //��������
            this.GetFromReference(KTitleTxt).GetTextMeshPro().SetTMPText(language["user_info_title"].current);
        }

        public void SetName(string name)
        {
            this.GetFromReference(KNameTxt).GetTextMeshPro().SetTMPText(name);
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out UI ui))
            {
                var uis = ui as UIPanel_Main;
                uis?.SetPlayerName(name);
            }
        }

        void CloseLastItemView(Dictionary<int, GameObject> itemDic)
        {
            lastUI.Dispose();

            foreach (var item in itemDic)
            {
                //item.Value.Dispose();
                //UnityEngine.GameObject.Destroy(item.Value);
            }

            itemDic.Clear();
        }

        async void InitHeadItem(string uiType, Dictionary<int, GameObject> itemDic, int length, int usingID,
            List<int2> HeadLock, string inUsing)
        {
            var userHead = ConfigManager.Instance.Tables.Tbuser_avatar.DataMap;
            //itemPos = GetFromReference(KItemPos);
            lastUI = await UIHelper.CreateAsync(this, UIType.UIItemView, itemPos, true);
            var loopRect = lastUI.GetScrollRect();

            lastIndex = -1;
            for (int i = 0; i < length; i++)
            {
                var ui = UIHelper.Create(uiType, i,
                    loopRect.Content.GameObject.transform);
                ui.SetParent(this, false);

                itemDic.Add(i, ui.GameObject);

                //����ͼƬ
                int Index = HeadLock[i].x;
                await ui.GetFromPath("Button/HeadBack/HeadImage").GetImage()
                    .SetSpriteAsync(userHead[Index].icon, false);

                //���ý���
                if (HeadLock[i].y == 0 ? false : true)
                {
                    ui.GetFromPath("Button/Lock").SetActive(false);
                }

                //����ʹ����
                if (Index == usingID)
                {
                    ui.GetFromPath("Button/TxtBack").SetActive(true);
                    ui.GetFromPath("Button/TxtBack/UsingTxt").GetTextMeshPro().SetTMPText(inUsing);
                    ui.GetFromPath("Button/HeadSelect").SetActive(true);
                    lastIndex = i;
                }
            }
        }

        async void InitFrameItem(string uiType, Dictionary<int, GameObject> itemDic, int length, int usingID,
            List<int2> FrameLock, string inUsing)
        {
            var userHead = ConfigManager.Instance.Tables.Tbuser_avatar.DataMap;
            //itemPos = GetFromReference(KItemPos);
            lastUI = await UIHelper.CreateAsync(this, UIType.UIItemView, itemPos, true);
            var loopRect = lastUI.GetScrollRect();

            lastIndex = -1;
            for (int i = 0; i < length; i++)
            {
                var ui = UIHelper.Create(uiType, i,
                    loopRect.Content.GameObject.transform);
                ui.SetParent(this, false);

                itemDic.Add(i, ui.GameObject);

                //����ͼƬ
                int Index = FrameLock[i].x;
                await ui.GetFromPath("Button/Frame").GetImage().SetSpriteAsync(userHead[Index].icon, false);

                //���ý���
                if (FrameLock[i].y == 0 ? false : true)
                {
                    ui.GetFromPath("Button/Lock").SetActive(false);
                }

                //����ʹ����
                if (Index == usingID)
                {
                    ui.GetFromPath("Button/TxtBack/UsingTxt").GetTextMeshPro().SetTMPText(inUsing);
                    ui.GetFromPath("Button/TxtBack").SetActive(true);
                    ui.GetFromPath("Button/HeadSelect").SetActive(true);
                    lastIndex = i;
                }
            }
        }

        //���ݽ����������
        private List<int2> LockSorting(List<int2> input, int length)
        {
            int i = 0;
            List<int2> sortingHelp = new List<int2>();

            for (int j = 0; j < length; j++)
            {
                if (input[j].y == 1)
                {
                    sortingHelp.Add(input[j]);
                    i++;
                }
            }

            for (int j = 0; j < length; j++)
            {
                if (input[j].y == 0)
                {
                    sortingHelp.Add(input[j]);
                    i++;
                }
            }

            return sortingHelp;
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
                return;
            }

            HeadID = gameRole.RoleAvatar;
            FrameID = gameRole.RoleAvatarFrame;
        }

        private async void OnChangeNameStatusResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            var checkResult = new CheckResult();
            checkResult.MergeFrom(e.data);
            Debug.Log(checkResult);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            // var ui = await UIHelper.CreateAsync(UIType.UIChangeName,
            //     new ChangeNameHelp { gameRole = role, checkResult = checkResult }, UILayer.Mid);
            // ui.SetParent(this, true);
            // //var ui = await UIHelper.CreateAsync(UIType.UICommon_TopTab);
            // //UIHelper.Create(UIType.UIRedPointT);
            // WebMessageHandlerOld.Instance.RemoveHandler(CMD.CHANGESTATUS, OnChangeNameStatusResponse);
        }

        protected override void OnClose()
        {
            CloseLastItemView(UIItemDic);
            WebMessageHandlerOld.Instance.RemoveHandler(1, 2, OnChangeAvaterResponse);
            base.OnClose();
        }
    }
}