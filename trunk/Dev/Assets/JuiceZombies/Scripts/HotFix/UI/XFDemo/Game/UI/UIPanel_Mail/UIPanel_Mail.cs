//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Google.Protobuf.Collections;
using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Mail)]
    internal sealed class UIPanel_MailEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Mail;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Mail>();
        }
    }

    public partial class UIPanel_Mail : UI, IAwake
    {
        //{ "id": "1717430398578896896", "MailModuleId": 10001, "userId": "1", "roleId": "1", "sendTime": "1698302289", "expireDate": "1698388689" }

        private Tbmail tbmail;
        private long timerId;
        private Dictionary<long, UISubPanel_MailItem> mailItemsDic = new Dictionary<long, UISubPanel_MailItem>();

        private Tblanguage tblanguage;

        //private List<UICommon_RewardItem> rewardItems = new List<UICommon_RewardItem>();
        private long lastUI = 0;

        private UICommon_ItemTips tipPanel;
        private int tagFunc = 5201;

        private string m_RedDotName;

        private CancellationTokenSource cts = new CancellationTokenSource();

        public async void Initialize()
        {
            await UnicornUIHelper.InitBlur(this);
            tbmail = ConfigManager.Instance.Tables.Tbmail;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            InitRedDot();
            var KMaskBtn = GetFromReference(UIPanel_Mail.KMaskBtn);
            var KTxt_Title = GetFromReference(UIPanel_Mail.KTxt_Title);
            var KBtn_Close = GetFromReference(UIPanel_Mail.KBtn_Close);
            var KBg_TopTitle = GetFromReference(UIPanel_Mail.KBg_TopTitle);
            var KTxt_MailTitle = GetFromReference(UIPanel_Mail.KTxt_MailTitle);
            var KTxt_MailAuthor = GetFromReference(UIPanel_Mail.KTxt_MailAuthor);
            var KTxt_MailTime = GetFromReference(UIPanel_Mail.KTxt_MailTime);
            var KCommon_Btn = GetFromReference(UIPanel_Mail.KCommon_Btn);

            var KTxt_Content = GetFromReference(UIPanel_Mail.KTxt_Content);
            var KTxt_ListName = GetFromReference(UIPanel_Mail.KTxt_ListName);
            var KCommon_CloseInfo = GetFromReference(UIPanel_Mail.KCommon_CloseInfo);
            var KEmpty2 = GetFromReference(UIPanel_Mail.KEmpty2);
            var KImg_MailEmpty2 = GetFromReference(UIPanel_Mail.KImg_MailEmpty2);
            var KTxt_MailEmpty2 = GetFromReference(UIPanel_Mail.KTxt_MailEmpty2);
            var KEmpty1 = GetFromReference(UIPanel_Mail.KEmpty1);
            var KImg_MailEmpty1 = GetFromReference(UIPanel_Mail.KImg_MailEmpty1);
            var KTxt_MailEmpty1 = GetFromReference(UIPanel_Mail.KTxt_MailEmpty1);
            var KTopContent = GetFromReference(UIPanel_Mail.KTopContent);
            var KTxt_BiggerContent = GetFromReference(UIPanel_Mail.KTxt_BiggerContent);
            var KScroller_BiggerContent = GetFromReference(UIPanel_Mail.KScroller_BiggerContent);
            var KScroller_Content = GetFromReference(UIPanel_Mail.KScroller_Content);
            var KScroller_MailList = GetFromReference(UIPanel_Mail.KScroller_MailList);
            var KScroller_Reward = GetFromReference(UIPanel_Mail.KScroller_Reward);
            var KRewardPos = GetFromReference(UIPanel_Mail.KRewardPos);

            var KBg_Main = GetFromReference(UIPanel_Mail.KBg_Main);
            var KImg_Title = GetFromReference(UIPanel_Mail.KImg_Title);


            KMaskBtn.GetRectTransform().SetScale(Vector2.one);
            KBtn_Close.GetRectTransform().SetScale(Vector2.one);


            KMaskBtn.GetXButton().OnClick.Add(async () => { await ClosePanel(); });
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Close, async () => await ClosePanel());
            await InitMailPanel();

            FirstMailRead();

            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KCommon_Btn, () =>
            {
                UnicornUIHelper.DestoryAllTips();
                OnClickReceiveBtn();
            });
            KImg_Title.GetXButton().OnClick.Add(() => { UnicornUIHelper.DestoryAllTips(); });
            KBg_Main.GetXButton().OnClick.Add(() => { UnicornUIHelper.DestoryAllTips(); });
        }

        void InitRedDot()
        {
            m_RedDotName = NodeNames.GetTagFuncRedDotName(tagFunc);
            // for (int i = 0; i < 2; i++)
            // {
            //     var itemStr = $"{m_RedDotName}|Pos{i}";
            //     RedDotManager.Instance.InsterNode(itemStr);
            // }
        }

        private async UniTask ClosePanel()
        {
            UnicornTweenHelper.SetEaseAlphaAndPosUtoB(GetFromReference(UIPanel_Mail.KMain), 0 - 100, 100, cts.Token, 0.15f,
                false);
            UnicornTweenHelper.SetEaseAlphaAndPosRtoL(GetFromReference(UIPanel_Mail.KMain), 0 - 100, 100, cts.Token, 0.15f,
                false);
            GetFromReference(UIPanel_Mail.KMain).GetComponent<CanvasGroup>().alpha = 1f;
            GetFromReference(UIPanel_Mail.KMain).GetComponent<CanvasGroup>().DOFade(0, 0.3f).SetEase(Ease.InQuad);
            await UniTask.Delay(150, cancellationToken: cts.Token);
            Close();
        }

        public void SetItemTipPanel(UICommon_ItemTips ui)
        {
            //Log.Debug($"SetItemTipPanel", Color.green);
            tipPanel = ui;
        }


        private void OnClickReceiveBtn()
        {
            mailItemsDic[lastUI].OnReceiveGift();

            SetReceiveBtn(mailItemsDic[lastUI].GetMailInfo());
            // var KBtn_Receive = GetFromReference(UIPanel_Mail.KBtn_Receive);
            // var KTxt_Receive = GetFromReference(UIPanel_Mail.KTxt_Receive);
            // KBtn_Receive.GetXButton().SetEnabled(false);
            // KBtn_Receive.GetImage().SetEnabled(false);
            // KTxt_Receive.GetTextMeshPro().SetTMPText(language.Get("common_state_gained").current);
            // KTxt_Receive.GetTextMeshPro().SetColor(Color.black);
        }

        void SetReceiveBtn(MailInfo mailInfo)
        {
            var KCommon_Btn = GetFromReference(UIPanel_Mail.KCommon_Btn);
            var KText_GotReward = GetFromReference(UIPanel_Mail.KText_GotReward);
            //var KTxt_Receive = GetFromReference(UIPanel_Mail.KTxt_Receive);
            var KText_Btn = KCommon_Btn.GetFromReference(UICommon_Btn.KText_Btn);
            KText_Btn.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_gain").current);
            KText_GotReward.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_gained").current);

            if (mailInfo.Status != 1)
            {
                KCommon_Btn.SetActive(true);
                KText_GotReward.SetActive(false);
                // KText_GotReward.SetActive(false);
                // KCommon_Btn.GetXButton().SetEnabled(true);
                // KCommon_Btn.GetImage().SetEnabled(true);

                //KText_Btn.GetTextMeshPro().SetColor(Color.white);
            }
            else
            {
                KCommon_Btn.SetActive(false);
                KText_GotReward.SetActive(true);

                //KText_Btn.GetTextMeshPro().SetColor(Color.black);
            }
        }

        public void DisableAllIsSelected()
        {
            var KScroller_MailList = GetFromReference(UIPanel_Mail.KScroller_MailList);
            var list = KScroller_MailList.GetScrollRect().Content.GetList();
            foreach (var ui in list.Children)
            {
                var uis = ui as UISubPanel_MailItem;
                var KImg_Selected = uis.GetFromReference(UISubPanel_MailItem.KImg_Selected);
                KImg_Selected.SetActive(false);
            }
        }

        public async void InitTopContent(MailInfo mailInfo)
        {
            mail mail = null;

            if (mailInfo.Type == 1)
            {
                var rewards = new List<Vector3>();
                var paraList = mailInfo.Para.Split(";").ToList();
                RepeatedField<string> rewardList = mailInfo.RewardList;

                foreach (var reward in rewardList)
                {
                    var rewardSplit = reward.Split(";");

                    rewards.Add(new Vector3(int.Parse(rewardSplit[0]), int.Parse(rewardSplit[1]),
                        int.Parse(rewardSplit[2])));
                }

                //TODO:������ �ʼ�id
                mail = new mail(mailInfo.MailModuleId, mailInfo.Type + 1, mailInfo.Title, mailInfo.Content, paraList,
                    "Jiyu-Studio",
                    99, rewards);
            }
            else
            {
                //Debug.LogError($"111111");
                mail = tbmail.GetOrDefault(mailInfo.MailModuleId);
            }

            if (mail == null)
            {
                Log.Error($"id:{mailInfo.MailModuleId} �����ʼ��ò���");
                return;
            }


            var KScroller_Reward = GetFromReference(UIPanel_Mail.KScroller_Reward);
            var KScroller_Content = GetFromReference(UIPanel_Mail.KScroller_Content);
            var KScroller_BiggerContent = GetFromReference(UIPanel_Mail.KScroller_BiggerContent);
            var KTxt_BiggerContent = GetFromReference(UIPanel_Mail.KTxt_BiggerContent);
            var KTxt_Content = GetFromReference(UIPanel_Mail.KTxt_Content);

            var KTxt_MailTitle = GetFromReference(UIPanel_Mail.KTxt_MailTitle);
            var KTxt_MailAuthor = GetFromReference(UIPanel_Mail.KTxt_MailAuthor);
            var KTxt_MailTime = GetFromReference(UIPanel_Mail.KTxt_MailTime);
            //var KTxt_Receive = GetFromReference(UIPanel_Mail.KTxt_Receive);
            var KRewardPos = GetFromReference(UIPanel_Mail.KRewardPos);

            var scrollRect_Reward = KScroller_Reward.GetXScrollRect();

            var scrollRect_Content = KScroller_Content.GetXScrollRect();

            var scrollRect_BiggerContent = KScroller_BiggerContent.GetXScrollRect();

            KTxt_MailTitle.GetTextMeshPro().SetTMPText(tblanguage.Get(mail.title).current);
            KTxt_MailAuthor.GetTextMeshPro().SetTMPText(tblanguage.Get(mail.from).current);
            var dateTime = TimeHelper.ToDateTime(mailInfo.SendTime * 1000);

            string formattedDate = dateTime.ToString($"MM/dd HH:mm");
            KTxt_MailTime.GetTextMeshPro().SetTMPText(formattedDate);

            if (lastUI == mailInfo.Id)
                return;
            if (lastUI != 0)
            {
                DisableAllIsSelected();
                var KImg_Selected = mailItemsDic[mailInfo.Id].GetFromReference(UISubPanel_MailItem.KImg_Selected);
                KImg_Selected.SetActive(true);
            }

            lastUI = mailInfo.Id;

            string contentText = tblanguage.Get(mail.content).current;

            if (mail.para.Count > 0)
            {
                contentText = string.Format(contentText, mail.para.ToArray());
            }

            switch (mail.type)
            {
                case 1:
                    KRewardPos.SetActive(false);
                    KScroller_Content.SetActive(false);
                    KScroller_BiggerContent.SetActive(true);
                    KTxt_BiggerContent.GetTextMeshPro().SetTMPText(contentText);

                    break;
                case 2:
                    var rewards = mail.reward;
                    //rewards.Add(new Vector3(5f, 5012001f, 1f));
                    KRewardPos.SetActive(true);
                    KScroller_Content.SetActive(true);
                    KScroller_BiggerContent.SetActive(false);
                    KTxt_Content.GetTextMeshPro().SetTMPText(contentText);

                    scrollRect_Reward.OnDrag.Add((a) => { UnicornUIHelper.DestoryAllTips(); });
                    scrollRect_Content.OnDrag.Add((a) => { UnicornUIHelper.DestoryAllTips(); });

                    SetReceiveBtn(mailInfo);

                    var list = scrollRect_Reward.Content.GetList();
                    list.Clear();
                    foreach (var reward in rewards)
                    {
                        var common_RewardItem =
                            await list.CreateWithUITypeAsync(UIType.UICommon_RewardItem, reward,
                                    false) as
                                UICommon_RewardItem;


                        //ui.SetParent(this, false);
                        common_RewardItem.GetRectTransform().SetScale(new Vector2(1f, 1f));
                        UnicornUIHelper.SetRewardOnClick(reward, common_RewardItem);
                    }

                    list.Sort(UnicornUIHelper.RewardUIComparer);
                    break;
            }

            ResourcesSingletonOld.Instance.UpdateResourceUI();
        }


        void FirstMailRead()
        {
            if (ResourcesSingletonOld.Instance.mails.Count < 1)
                return;
            mailItemsDic[ResourcesSingletonOld.Instance.mails[0].Id].OnClickMailList(ResourcesSingletonOld.Instance.mails[0]);
        }


        async UniTask InitMailPanel()
        {
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var KTxt_Title = GetFromReference(UIPanel_Mail.KTxt_Title);
            KTxt_Title.GetTextMeshPro().SetTMPText(language.Get("func_3102_name").current);

            var KTxt_ListName = GetFromReference(UIPanel_Mail.KTxt_ListName);
            KTxt_ListName.GetTextMeshPro().SetTMPText(language.Get("mail_list_name").current);
            var KBg_TopTitle = GetFromReference(UIPanel_Mail.KBg_TopTitle);
            KBg_TopTitle.GetImage().SetSpriteAsync("pic_mail_detail_background", false).Forget();

            var KImg_MailEmpty1 = GetFromReference(UIPanel_Mail.KImg_MailEmpty1);
            var KTxt_MailEmpty1 = GetFromReference(UIPanel_Mail.KTxt_MailEmpty1);
            //KImg_MailEmpty1.GetImage().SetSpriteAsync("pic_mail_detail_blank", false).Forget();
            KTxt_MailEmpty1.GetTextMeshPro().SetTMPText(language.Get("mail_detail_blank_text").current);

            var KImg_MailEmpty2 = GetFromReference(UIPanel_Mail.KImg_MailEmpty2);
            var KTxt_MailEmpty2 = GetFromReference(UIPanel_Mail.KTxt_MailEmpty2);
            //KImg_MailEmpty2.GetImage().SetSpriteAsync("pic_mail_list_blank", false).Forget();
            KTxt_MailEmpty2.GetTextMeshPro().SetTMPText(language.Get("mail_list_blank_text").current);


            var KCommon_CloseInfo = GetFromReference(UIPanel_Mail.KCommon_CloseInfo);
            KCommon_CloseInfo.GetTextMeshPro().SetTMPText(language.Get("text_window_close").current);

            StartTimer();


            if (ResourcesSingletonOld.Instance.mails.Count < 1)
            {
                var KScroller_MailList = GetFromReference(UIPanel_Mail.KScroller_MailList);
                KScroller_MailList.SetActive(false);
                var KEmpty2 = GetFromReference(UIPanel_Mail.KEmpty2);
                KEmpty2.SetActive(true);
                var KTopContent = GetFromReference(UIPanel_Mail.KTopContent);
                KTopContent.SetActive(false);
                var KEmpty1 = GetFromReference(UIPanel_Mail.KEmpty1);
                KEmpty1.SetActive(true);
                return;
            }
            else
            {
                var KScroller_MailList = GetFromReference(UIPanel_Mail.KScroller_MailList);
                KScroller_MailList.SetActive(true);
                var KEmpty2 = GetFromReference(UIPanel_Mail.KEmpty2);
                KEmpty2.SetActive(false);
                var KTopContent = GetFromReference(UIPanel_Mail.KTopContent);
                KTopContent.SetActive(true);
                var KEmpty1 = GetFromReference(UIPanel_Mail.KEmpty1);
                KEmpty1.SetActive(false);
            }


            var scroller_BiggerContent = GetFromReference(UIPanel_Mail.KScroller_BiggerContent);
            var scroller_Content = GetFromReference(UIPanel_Mail.KScroller_Content);
            var scroller_MailList = GetFromReference(UIPanel_Mail.KScroller_MailList);
            var scroller_Reward = GetFromReference(UIPanel_Mail.KScroller_Reward);

            var scrollRect = scroller_MailList.GetScrollRect();
            var list = scrollRect.Content.GetList();
            list.Clear();
            foreach (var mailInfo in ResourcesSingletonOld.Instance.mails)
            {
                var ui =
                    await list.CreateWithUITypeAsync(UIType.UISubPanel_MailItem, mailInfo,
                            false) as
                        UISubPanel_MailItem;

                ui.SetParent(this, false);
                mailItemsDic.Add(mailInfo.Id, ui);
            }

            list.Sort((a, b) =>
            {
                var uia = a as UISubPanel_MailItem;
                var uib = b as UISubPanel_MailItem;
                var obj1 = uia._mailInfo;
                var obj2 = uib._mailInfo;

                if (obj1.SendTime > obj2.SendTime)
                    return -1;
                else if (obj1.SendTime < obj2.SendTime)
                    return 1;

                if (obj1.MailModuleId < obj2.MailModuleId)
                    return -1;
                else if (obj1.MailModuleId > obj2.MailModuleId)
                    return 1;
                return 0;
            });
            DisableAllIsSelected();
        }


        void Update()
        {
            var KCommon_CloseInfo = GetFromReference(UIPanel_Mail.KCommon_CloseInfo);

            KCommon_CloseInfo.GetTextMeshPro().DoFade(1, 0.1f, 1f).AddOnCompleted(() =>
            {
                KCommon_CloseInfo.GetTextMeshPro().DoFade(0.1f, 1, 1f);
            });
        }

        /// <summary>
        /// ������ʱ��
        /// </summary>
        public void StartTimer()
        {
            //����һ��ÿִ֡�е������൱��Update
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.StartRepeatedTimer(2500, this.Update);
        }

        /// <summary>
        /// �Ƴ���ʱ��
        /// </summary>
        public void RemoveTimer()
        {
            var timerMgr = TimerManager.Instance;
            timerMgr?.RemoveTimerId(ref this.timerId);
            this.timerId = 0;
        }


        protected override void OnClose()
        {
            RemoveTimer();

            mailItemsDic.Clear();

            lastUI = 0;
            UnicornUIHelper.DestoryAllTips();
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}