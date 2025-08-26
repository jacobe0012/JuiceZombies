//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_MailItem)]
    internal sealed class UISubPanel_MailItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_MailItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_MailItem>();
        }
    }

    public partial class UISubPanel_MailItem : UI, IAwake<MailInfo>
    {
        private long timerId;
        private Tbmail tbmail;
        private Tblanguage tblanguage;
        public MailInfo _mailInfo;
        private mail mail;

        void Init(MailInfo mailInfo)
        {
            mail = null;

            if (mailInfo.Type == 1)
            {
                var rewards = new List<Vector3>();
                var paraList = mailInfo.Para.Split(";").ToList();
                var rewardList = mailInfo.RewardList;

                foreach (var reward in rewardList)
                {
                    var rewardSplit = reward.Split(";");

                    rewards.Add(new Vector3(int.Parse(rewardSplit[0]), int.Parse(rewardSplit[1]),
                        int.Parse(rewardSplit[2])));
                }

                //TODO:发件人 邮件id
                mail = new mail(mailInfo.MailModuleId, mailInfo.Type + 1, mailInfo.Title, mailInfo.Content, paraList,
                    "Jiyu-Studio",
                    99, rewards);
            }
            else
            {
                mail = tbmail.GetOrDefault(mailInfo.MailModuleId);
            }

            if (mail == null)
            {
                Log.Error($"id:{mailInfo.MailModuleId} 表中邮件拿不到");
                return;
            }
        }

        public void Initialize(MailInfo mailInfo)
        {
            _mailInfo = mailInfo;
            tbmail = ConfigManager.Instance.Tables.Tbmail;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;

            Init(mailInfo);


            var KImg_Selected = GetFromReference(UISubPanel_MailItem.KImg_Selected);
            var KImg_MailItemLeftIcon = GetFromReference(UISubPanel_MailItem.KImg_MailItemLeftIcon);
            var KTxt_MailItemTitle = GetFromReference(UISubPanel_MailItem.KTxt_MailItemTitle);
            var KTxt_MailItemAuthor = GetFromReference(UISubPanel_MailItem.KTxt_MailItemAuthor);

            var KTxt_MailItemTime = GetFromReference(UISubPanel_MailItem.KTxt_MailItemTime);
            var KTxt_MailItemWarning = GetFromReference(UISubPanel_MailItem.KTxt_MailItemWarning);
            var KWarning = GetFromReference(UISubPanel_MailItem.KWarning);
            var KImg_GiftIcon = GetFromReference(UISubPanel_MailItem.KImg_GiftIcon);
            var KImg_RedDot = GetFromReference(UISubPanel_MailItem.KImg_RedDot);
            var KRight = GetFromReference(UISubPanel_MailItem.KRight);
            var KBg_Mask = GetFromReference(UISubPanel_MailItem.KBg_Mask);
            var KBtn_MailItem = GetFromReference(UISubPanel_MailItem.KBtn_MailItem);

            InitPanel(mailInfo);

            KBtn_MailItem.GetXButton().OnClick.Add(() => { OnClickMailList(mailInfo); });
            StartTimer();
        }

        public MailInfo GetMailInfo()
        {
            return _mailInfo;
        }

        public async void OnReceiveGift()
        {
            var rewards = tbmail.Get(_mailInfo.MailModuleId).reward;

            //var rewards = UnicornUIHelper.MergeRewardList(rewards);
            if (_mailInfo.Status != 1)
            {
                NetWorkManager.Instance.SendMessage(5, 2, new LongValue
                {
                    Value = _mailInfo.Id
                });
                _mailInfo.Status = 1;
                SetReadOrNot(_mailInfo, mail);
                foreach (var reward in rewards)
                {
                    UnicornUIHelper.AddReward(reward, true);
                    await Task.Delay(500);
                }
            }
        }

        public void OnClickMailList(MailInfo mailInfo)
        {
            UnicornUIHelper.DestoryAllTips();
            //var type = tbmail.Get(mailInfo.MailModuleId).type;

            var KImg_Selected = GetFromReference(UISubPanel_MailItem.KImg_Selected);
            KImg_Selected.SetActive(true);

            switch (mail.type)
            {
                case 1:
                    if (mailInfo.Status != 1)
                    {
                        NetWorkManager.Instance.SendMessage(5, 2, new LongValue
                        {
                            Value = mailInfo.Id
                        });
                        mailInfo.Status = 1;
                    }

                    SetReadOrNot(mailInfo, mail);
                    GetParent<UIPanel_Mail>().InitTopContent(mailInfo);

                    break;
                case 2:

                    GetParent<UIPanel_Mail>().InitTopContent(mailInfo);

                    break;
            }
        }

        private void Update()
        {
            InitPanel(_mailInfo);
        }

        /// <summary>
        /// 开启定时器
        /// </summary>
        public void StartTimer()
        {
            //开启一个每帧执行的任务，相当于Update
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.StartRepeatedTimer(2000, this.Update);
        }

        /// <summary>
        /// 移除定时器
        /// </summary>
        public void RemoveTimer()
        {
            var timerMgr = TimerManager.Instance;
            timerMgr?.RemoveTimerId(ref this.timerId);
            this.timerId = 0;
        }

        void SetReadOrNot(MailInfo mailInfo, mail mail)
        {
            var KRight = GetFromReference(UISubPanel_MailItem.KRight);
            var KBg_Mask = GetFromReference(UISubPanel_MailItem.KBg_Mask);
            var KImg_RedDot = GetFromReference(UISubPanel_MailItem.KImg_RedDot);
            var KImg_GiftIcon = GetFromReference(UISubPanel_MailItem.KImg_GiftIcon);
            var KWarning = GetFromReference(UISubPanel_MailItem.KWarning);
            var KTxt_MailItemTitle= GetFromReference(UISubPanel_MailItem.KTxt_MailItemTitle);
            var KTxt_MailItemAuthor= GetFromReference(UISubPanel_MailItem.KTxt_MailItemAuthor);
            var KTxt_MailItemTime= GetFromReference(UISubPanel_MailItem.KTxt_MailItemTime);

            KWarning.SetActive(false);
            switch (mail.type)
            {
                case 1:
                    KImg_GiftIcon.SetActive(false);
                    if (mailInfo.Status == null || mailInfo.Status == 0)
                    {
                        KRight.SetActive(true);
                        KImg_RedDot.SetActive(true);
                        KBg_Mask.SetActive(false);
                        KTxt_MailItemTitle.GetTextMeshPro().SetAlpha(1);
                        KTxt_MailItemAuthor.GetTextMeshPro().SetAlpha(1);
                        KTxt_MailItemTime.GetTextMeshPro().SetAlpha(1);
                    }
                    else
                    {
                        KRight.SetActive(false);
                        KImg_RedDot.SetActive(false);

                        KBg_Mask.SetActive(true);
                        KTxt_MailItemTitle.GetTextMeshPro().SetAlpha(0.3686f);
                        KTxt_MailItemAuthor.GetTextMeshPro().SetAlpha(0.3686f);
                        KTxt_MailItemTime.GetTextMeshPro().SetAlpha(0.3686f);
                    }

                    break;
                case 2:
                    KImg_GiftIcon.SetActive(true);
                    if (mailInfo.Status == null || mailInfo.Status == 0)
                    {
                        KRight.SetActive(true);
                        KImg_RedDot.SetActive(true);

                        KBg_Mask.SetActive(false);
                        KTxt_MailItemTitle.GetTextMeshPro().SetAlpha(1);
                        KTxt_MailItemAuthor.GetTextMeshPro().SetAlpha(1);
                        KTxt_MailItemTime.GetTextMeshPro().SetAlpha(1);
                    }
                    else
                    {
                        KRight.SetActive(false);
                        KImg_RedDot.SetActive(false);

                        KBg_Mask.SetActive(true);
                        KTxt_MailItemTitle.GetTextMeshPro().SetAlpha(0.3686f);
                        KTxt_MailItemAuthor.GetTextMeshPro().SetAlpha(0.3686f);
                        KTxt_MailItemTime.GetTextMeshPro().SetAlpha(0.3686f);
                    }

                    break;
            }
        }

        void InitPanel(MailInfo mailInfo)
        {
            //var reward = mail.Get(mailInfo.MailModuleId).reward;
            var KImg_MailItemLeftIcon = GetFromReference(UISubPanel_MailItem.KImg_MailItemLeftIcon);
            var KRight = GetFromReference(UISubPanel_MailItem.KRight);
            var KBg_Mask = GetFromReference(UISubPanel_MailItem.KBg_Mask);

            var KImg_RedDot = GetFromReference(UISubPanel_MailItem.KImg_RedDot);
            var KImg_GiftIcon = GetFromReference(UISubPanel_MailItem.KImg_GiftIcon);
            var KWarning = GetFromReference(UISubPanel_MailItem.KWarning);
            var KTxt_MailItemWarning = GetFromReference(UISubPanel_MailItem.KTxt_MailItemWarning);
            // var KImg_Selected = GetFromReference(UISubPanel_MailItem.KImg_Selected);
            // KImg_Selected.SetActive(true);

            switch (mail.type)
            {
                case 1:
                    KImg_MailItemLeftIcon.GetImage().SetSpriteAsync("pic_mail_mail_icon", false).Forget();

                    SetReadOrNot(mailInfo, mail);

                    break;
                case 2:
                    KImg_MailItemLeftIcon.GetImage().SetSpriteAsync("pic_mail_gift", false).Forget();

                    SetReadOrNot(mailInfo, mail);
                    var expireTime = mailInfo.ExpireDate;
                    var remainingTime = expireTime - TimeHelper.ClientNowSeconds();
                    if (remainingTime < 345600 && (mailInfo.Status == null || mailInfo.Status == 0))
                    {
                        KWarning.SetActive(true);
                        var warningText = string.Format(tblanguage.Get("mail_list_delay_text").current,
                            ExpireTime(mailInfo));
                        KTxt_MailItemWarning.GetTextMeshPro().SetTMPText(warningText);
                    }

                    break;
            }

            var KTxt_MailItemTitle = GetFromReference(UISubPanel_MailItem.KTxt_MailItemTitle);
            var KTxt_MailItemAuthor = GetFromReference(UISubPanel_MailItem.KTxt_MailItemAuthor);
            var KTxt_MailItemTime = GetFromReference(UISubPanel_MailItem.KTxt_MailItemTime);


            KTxt_MailItemTitle.GetTextMeshPro()
                .SetTMPText(tblanguage.Get(mail.title).current);
            KTxt_MailItemAuthor.GetTextMeshPro()
                .SetTMPText(tblanguage.Get(mail.from).current);


            var sendTime = mailInfo.SendTime;
            var remaining = TimeHelper.ClientNowSeconds() - sendTime;
            if (remaining > 345600)
            {
                KTxt_MailItemTime.GetTextMeshPro()
                    .SetTMPText($"{MailTimeShow(mailInfo)}");
            }
            else
            {
                var str = string.Format(tblanguage.Get("mail_list_time_text").current, MailTimeShow(mailInfo));
                KTxt_MailItemTime.GetTextMeshPro()
                    .SetTMPText(str);
                // KTxt_MailItemTime.GetTextMeshPro()
                //     .SetTMPText($"{MailTimeShow(mailInfo)}{tblanguage.Get("mail_list_time_text").current}");
                
                
            }


            //Log.Debug($"{MailTimeShow(mailInfo)}{language.Get("mail_list_time_text").current}", Color.green);
        }

        private string ExpireTime(MailInfo mailInfo)
        {
            string time_minute_2 = "time_minute_2";
            string time_hour_2 = "time_hour_2";
            string time_day_2 = "time_day_2";

            var expireTime = mailInfo.ExpireDate;
            var remainingTime = expireTime - TimeHelper.ClientNowSeconds();

            string outstring = default;
            if (remainingTime < 60)
            {
                outstring = $"{1}{tblanguage.Get(time_minute_2).current}";
            }
            else if (remainingTime < 3600)
            {
                int minutes = (int)(remainingTime / 60);
                outstring = $"{minutes}{tblanguage.Get(time_minute_2).current}";
            }
            else if (remainingTime < 86400)
            {
                int hours = (int)(remainingTime / 3600);
                int minutes = (int)((remainingTime % 3600) / 60);

                if (minutes == 0)
                {
                    outstring = $"{hours}{tblanguage.Get(time_hour_2).current}";
                }
                else
                {
                    outstring =
                        $"{hours}{tblanguage.Get(time_hour_2).current}{minutes}{tblanguage.Get(time_minute_2).current}";
                }
            }
            else if (remainingTime < 345600) // 4 days
            {
                int days = (int)(remainingTime / 86400);

                outstring = $"{days}{tblanguage.Get(time_day_2).current}";
            }
            else
            {
                DateTime dateTime = TimeHelper.ToDateTime(expireTime * 1000);

                outstring = dateTime.ToString("MM/dd");
            }

            return outstring;
        }

        private string MailTimeShow(MailInfo mailInfo)
        {
            var language = ConfigManager.Instance.Tables.Tblanguage;
            string time_minute_2 = "time_minute_2";
            string time_hour_2 = "time_hour_2";
            string time_day_2 = "time_day_2";

            var sendTime = mailInfo.SendTime;
            var remainingTime = TimeHelper.ClientNowSeconds() - sendTime;

            string outstring = default;
            if (remainingTime < 60)
            {
                outstring = $"{1}{language.Get(time_minute_2).current}";
            }
            else if (remainingTime < 3600)
            {
                int minutes = (int)(remainingTime / 60);
                outstring = $"{minutes}{language.Get(time_minute_2).current}";
            }
            else if (remainingTime < 86400)
            {
                int hours = (int)(remainingTime / 3600);
                int minutes = (int)((remainingTime % 3600) / 60);

                if (minutes == 0)
                {
                    outstring = $"{hours}{language.Get(time_hour_2).current}";
                }
                else
                {
                    outstring =
                        $"{hours}{language.Get(time_hour_2).current}{minutes}{language.Get(time_minute_2).current}";
                }
            }
            else if (remainingTime < 345600) // 4 days
            {
                int days = (int)(remainingTime / 86400);

                outstring = $"{days}{language.Get(time_day_2).current}";
            }
            else
            {
                DateTime dateTime = TimeHelper.ToDateTime(sendTime * 1000);

                outstring = dateTime.ToString("MM/dd");
            }

            return outstring;
        }


        protected override void OnClose()
        {
            RemoveTimer();
            //UnityEngine.GameObject.Destroy(this.GameObject);
            base.OnClose();
        }
    }
}