//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using cfg.config;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using Unity.Physics;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UICommon_Reward)]
    internal sealed class UICommon_RewardEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UICommon_Reward;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Mid;

        public override UI OnCreate()
        {
            return UI.Create<UICommon_Reward>();
        }
    }

    public partial class UICommon_Reward : UI, IAwake<List<Vector3>>
    {
        private long timerId;

        private CancellationTokenSource cts = new CancellationTokenSource();

        public async void Initialize(List<Vector3> args)
        {
            await UnicornUIHelper.InitBlur(this);
            UnicornUIHelper.MergeRewardList(args);
            //UnicornTweenHelper.EnableLoading(true);
            Tblanguage tblanguage = ConfigManager.Instance.Tables.Tblanguage;

            var list = new List<Vector3>(args.Count);
            list.AddRange(args);
            var KBg_Img = GetFromReference(UICommon_Reward.KBg_Img);
            var KBtn_Close = GetFromReference(UICommon_Reward.KBtn_Close);
            var KBg_MidImg = GetFromReference(UICommon_Reward.KBg_MidImg);

            KBg_Img.SetActive(true);
            KBtn_Close.SetActive(true);
            //开始定时器
            StartTimer();

            GetFromReference(KText_Tips).GetTextMeshPro().SetTMPText(tblanguage.Get("text_gain_reward").current);
            GetFromReference(KText_Close).GetTextMeshPro().SetTMPText(tblanguage.Get("text_window_close").current);

            KBg_MidImg.GetButton().OnClick.Add(UnicornUIHelper.DestoryAllTips);
            var KScrollView = this.GetFromReference(UICommon_Reward.KScrollView);
            KScrollView.GetXScrollRect().OnBeginDrag.Add(() => { UnicornUIHelper.DestoryAllTips(); });

            KBtn_Close.GetButton()?.OnClick.AddListener(async () =>
            {
                UnicornTweenHelper.SetScaleWithBounceClose(GetFromReference(UICommon_Reward.KBg_MidImg),
                    cancellationToken: cts.Token);
                await UniTask.Delay(200, true, cancellationToken: cts.Token);
                OnClickReward(list);
                Close();
                return;
            });

            KBg_Img.GetButton()?.OnClick.AddListener(async () =>
            {
                UnicornTweenHelper.SetScaleWithBounceClose(GetFromReference(UICommon_Reward.KBg_MidImg),
                    cancellationToken: cts.Token);
                await UniTask.Delay(200, true, cancellationToken: cts.Token);

                OnClickReward(list);
                Close();
                return;
            });

            //实际奖励初始化
            InitRewardItems(list, cts.Token).Forget();
            //args2 = args;
            UnicornTweenHelper.SetScaleWithBounce(GetFromReference(UICommon_Reward.KBg_MidImg),
                cancellationToken: cts.Token);
        }

        /// <summary>
        /// 开启定时器
        /// </summary>
        public void StartTimer()
        {
            //开启一个每帧执行的任务，相当于Update
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.StartRepeatedTimer(2500, this.Update);
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

        void Update()
        {
            var KTxt_CloseTip = GetFromReference(KText_Close);

            KTxt_CloseTip.GetTextMeshPro().DoFade(1, 0.1f, 1f).AddOnCompleted(() =>
            {
                KTxt_CloseTip.GetTextMeshPro().DoFade(0.1f, 1, 1f);
            });
        }

        protected override void OnClose()
        {
            cts.Cancel();
            cts.Dispose();

            RemoveTimer();
            base.OnClose();
        }

        private void OnClickReward(List<Vector3> args)
        {
            bool has2 = args.Where(a => (int)a.x == 2).Count() > 0;
            bool has3 = args.Where(a => (int)a.x == 3).Count() > 0;
            bool has4 = args.Where(a => (int)a.x == 11).Count() > 0;
            bool has5 = args.Where(a => (int)a.x == 5).Count() > 0;
            if (has2)
            {
                AudioManager.Instance.PlayFModAudio(1301);
            }

            if (has3)
            {
                AudioManager.Instance.PlayFModAudio(1302);
            }

            if (has4)
            {
                AudioManager.Instance.PlayFModAudio(1304);
            }

            if (has5)
            {
                AudioManager.Instance.PlayFModAudio(1303);
            }

            UnicornUIHelper.AddReward(args, true);
            //await UniTask.Delay(200);
        }

        public async UniTask InitRewardItems(List<Vector3> args, CancellationToken cct)
        {
            this.SetActive(false);
            var KMid_Pos = GetFromReference(UICommon_Reward.KMid_Pos);


            // List<Vector4> result = args
            //     .GroupBy(v => new { v.x, v.y, v.z }) // 按照 x, y, z 分组
            //     .Select(g => new Vector4(g.Key.x, g.Key.y, g.Key.z, g.Count())) // 生成 Vector4，其中 w 是计数
            //     .ToList();


            var list = KMid_Pos.GetList();
            list.Clear();

            foreach (var item in args)
            {
                var ui = await list.CreateWithUITypeAsync(UIType.UICommon_RewardItem, item, false, cct);
                UnicornUIHelper.SetRewardOnClick(item, ui);
                //ui.GetComponent<RectTransform>().SetScale3(0.75f);
            }

            list.Sort(UnicornUIHelper.RewardUIComparer);
            //KMid_Pos.SetActive(true);
            this.SetActive(true);
            var bg = GetFromReference(KBg_MidImg);
            float addition = Mathf.Ceil(args.Count / 6f) - 1;
            var initHeight = 430;
            float maxHeight = 900f;
            bg.GetRectTransform().SetHeight(initHeight);
            Log.Debug($"InitReWardItem,addition:{addition}");
            if (addition >= 0 && addition <= 4)
            {
                bg.GetRectTransform().SetHeight(initHeight + addition * 166);
            }
            else if (addition > 4)
            {
                bg.GetRectTransform().SetHeight(maxHeight);
            }


            // list.Sort((obj11, obj21) =>
            // {
            //     Tblanguage language = ConfigManager.Instance.Tables.Tblanguage;
            //     Tbuser_variable user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
            //     Tbitem item = ConfigManager.Instance.Tables.Tbitem;
            //     Tbequip_data equip_data = ConfigManager.Instance.Tables.Tbequip_data;
            //     Tbequip_quality equip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
            //     Tbquality quality = ConfigManager.Instance.Tables.Tbquality;
            //
            //     var obj111 = obj11 as UICommon_RewardItem;
            //     var obj211 = obj21 as UICommon_RewardItem;
            //
            //     var obj1 = new Vector3(obj111.trueReward.x, obj111.trueReward.y, obj111.trueReward.z);
            //     var obj2 = new Vector3(obj211.trueReward.x, obj211.trueReward.y, obj211.trueReward.z);
            //     var obj1rewardx = (int)obj1.x;
            //     var obj1rewardy = (int)obj1.y;
            //     var obj1rewardz = (int)obj1.z;
            //     var obj2rewardx = (int)obj2.x;
            //     var obj2rewardy = (int)obj2.y;
            //     var obj2rewardz = (int)obj2.z;
            //
            //     if (UnicornUIHelper.IsResourceReward(obj1) && UnicornUIHelper.IsResourceReward(obj2))
            //     {
            //         if (obj1rewardx < obj2rewardx)
            //             return -1;
            //         else if (obj1rewardx > obj2rewardx)
            //             return 1;
            //     }
            //
            //     if (UnicornUIHelper.IsResourceReward(obj1) && !UnicornUIHelper.IsResourceReward(obj2))
            //         return -1;
            //     else if (!UnicornUIHelper.IsResourceReward(obj1) && UnicornUIHelper.IsResourceReward(obj2))
            //         return 1;
            //     // if (obj1rewardx != 5 && obj2rewardx == 5)
            //     //     return -1;
            //     // else if (obj1rewardx == 5 && obj2rewardx != 5)
            //     //     return 1;
            //
            //     if (obj1rewardx == 11 && obj2rewardx != 11)
            //         return 1;
            //     else if (obj1rewardx != 11 && obj2rewardx == 11)
            //         return -1;
            //
            //     if (obj1rewardx == 5 && obj2rewardx == 5)
            //     {
            //         if (item.Get(obj1rewardy).sort < item.Get(obj2rewardy).sort)
            //             return -1;
            //         else if (item.Get(obj1rewardy).sort > item.Get(obj2rewardy).sort)
            //             return 1;
            //         if (obj1rewardy < obj2rewardy)
            //             return -1;
            //         else if (obj1rewardy > obj2rewardy)
            //             return 1;
            //     }
            //
            //
            //     if (obj1rewardx == 11 && obj2rewardx == 11)
            //     {
            //         if (!UnicornUIHelper.IsCompositeEquipReward(obj1) &&
            //             UnicornUIHelper.IsCompositeEquipReward(obj2))
            //             return -1;
            //         else if (UnicornUIHelper.IsCompositeEquipReward(obj1) &&
            //                  !UnicornUIHelper.IsCompositeEquipReward(obj2))
            //             return 1;
            //
            //         if (equip_data.Get(obj1rewardy).quality >
            //             equip_data.Get(obj2rewardy).quality)
            //             return -1;
            //         else if (equip_data.Get(obj1rewardy).quality <
            //                  equip_data.Get(obj2rewardy).quality)
            //             return 1;
            //
            //         if (equip_data.Get(obj1rewardy).sYn == 1 &&
            //             equip_data.Get(obj2rewardy).sYn != 1)
            //             return -1;
            //         else if (equip_data.Get(obj1rewardy).sYn != 1 &&
            //                  equip_data.Get(obj2rewardy).sYn == 1)
            //             return 1;
            //
            //
            //         if (equip_data.Get(obj1rewardy).posId <
            //             equip_data.Get(obj2rewardy).posId)
            //             return -1;
            //         else if (equip_data.Get(obj1rewardy).posId >
            //                  equip_data.Get(obj2rewardy).posId)
            //             return 1;
            //
            //         if (obj1rewardy > obj2rewardy)
            //             return -1;
            //         else if (obj1rewardy < obj2rewardy)
            //             return 1;
            //     }
            //
            //
            //     return 0;
            // });
            //UnicornTweenHelper.EnableLoading(false);
            //UnicornUIHelper.ForceRefreshLayout(KMid_Pos);
        }
    }
}