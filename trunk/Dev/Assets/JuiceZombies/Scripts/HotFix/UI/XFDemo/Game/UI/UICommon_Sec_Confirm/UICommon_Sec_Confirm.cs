//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using Main;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UICommon_Sec_Confirm)]
    internal sealed class UICommon_Sec_ConfirmEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UICommon_Sec_Confirm;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UICommon_Sec_Confirm>();
        }
    }

    /// <summary>
    /// cost  get  reward
    /// </summary>
    public partial class UICommon_Sec_Confirm : UI, IAwake<Vector3, Vector3>, IAwake<Vector3, string>, IAwake
    {
        private Tblanguage tblanguage;

        public Vector3 cost;
        public Vector3 get;

        public async void Initialize()
        {
            await JiYuUIHelper.InitBlur(this);
        }

        public async void Initialize(Vector3 arg1, string arg2)
        {
            await JiYuUIHelper.InitBlur(this);

            cost = arg1;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            var KText_Return = GetFromReference(UICommon_Sec_Confirm.KText_Return);
            var KText_Buy = GetFromReference(UICommon_Sec_Confirm.KText_Buy);
            var KText = GetFromReference(UICommon_Sec_Confirm.KText);
            var KBg_Blur = GetFromReference(UICommon_Sec_Confirm.KBg_Blur);
            var KBtn_Return = GetFromReference(UICommon_Sec_Confirm.KBtn_Return);
            var KBtn_Buy = GetFromReference(UICommon_Sec_Confirm.KBtn_Buy);


            KText_Buy.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_buy").current);
            KText_Return.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_cancel").current);


            string originStr = tblanguage.Get("text_purchase_confirm").current;
            string costStr =
                $"{JiYuUIHelper.GetRewardTextIconName(cost)}{(int)cost.z}";
            string getStr = arg2;
            originStr = string.Format(originStr, costStr, getStr);

            KText.GetTextMeshPro().SetTMPText(originStr);

            KBg_Blur.GetButton().OnClick.Add(async () =>
            {
                JiYuTweenHelper.SetScaleWithBounceClose(GetFromReference(UICommon_Sec_Confirm.KBg));
                await UniTask.Delay(200);
                Close();
                return;
            });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Return, async () =>
            {
                JiYuTweenHelper.SetScaleWithBounceClose(GetFromReference(UICommon_Sec_Confirm.KBg));
                await UniTask.Delay(200);
                Close();
                return;
            });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Buy, async () =>
            {
                this.RemoveChild(UIType.UISubPanel_ResourceNotEnough);
                //Log.Error($"TryReduceReward1 {cost}");
                if (!JiYuUIHelper.IsRewardsEnough(cost))
                {
                    //Log.Error($"TryReduceReward2");
                    Vector3 v3 = cost;
                    v3.z = v3.z - JiYuUIHelper.GetRewardCount(cost);
                    var uiTip = await UIHelper.CreateAsync(this, UIType.UISubPanel_ResourceNotEnough, v3,
                        KBtn_Buy.GameObject.transform);
                    uiTip.SetParent(this, true);
                    JiYuUIHelper.SetResourceNotEnoughTip(uiTip, KBtn_Buy);

                    return;
                }
            });

            JiYuTweenHelper.SetScaleWithBounce(GetFromReference(UICommon_Sec_Confirm.KBg));
        }

        public async void Initialize(Vector3 arg1, Vector3 arg2)
        {
            await JiYuUIHelper.InitBlur(this);
            cost = arg1;
            get = arg2;

            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            var KText_Return = GetFromReference(UICommon_Sec_Confirm.KText_Return);
            var KText_Buy = GetFromReference(UICommon_Sec_Confirm.KText_Buy);
            var KText = GetFromReference(UICommon_Sec_Confirm.KText);
            var KBg_Blur = GetFromReference(UICommon_Sec_Confirm.KBg_Blur);
            var KBtn_Return = GetFromReference(UICommon_Sec_Confirm.KBtn_Return);
            var KBtn_Buy = GetFromReference(UICommon_Sec_Confirm.KBtn_Buy);


            KText_Buy.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_buy").current);
            KText_Return.GetTextMeshPro().SetTMPText(tblanguage.Get("common_state_cancel").current);


            string originStr = tblanguage.Get("text_purchase_confirm").current;
            string costStr =
                $"{JiYuUIHelper.GetRewardTextIconName(cost)}{(int)cost.z}";
            string getStr = JiYuUIHelper.GetRewardName(get);
            originStr = string.Format(originStr, costStr, getStr);

            KText.GetTextMeshPro().SetTMPText(originStr);

            KBg_Blur.GetButton().OnClick.Add(async () =>
            {
                JiYuTweenHelper.SetScaleWithBounceClose(GetFromReference(UICommon_Sec_Confirm.KBg));
                await UniTask.Delay(200);
                Close();
                return;
            });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Return, async () =>
            {
                JiYuTweenHelper.SetScaleWithBounceClose(GetFromReference(UICommon_Sec_Confirm.KBg));
                await UniTask.Delay(200);
                Close();
                return;
            });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Buy, async () =>
            {
                this.RemoveChild(UIType.UISubPanel_ResourceNotEnough);
                //Log.Error($"TryReduceReward1 {cost}");
                if (!JiYuUIHelper.IsRewardsEnough(cost))
                {
                    //Log.Error($"TryReduceReward2");
                    Vector3 v3 = cost;
                    v3.z = v3.z - JiYuUIHelper.GetRewardCount(cost);
                    var uiTip = await UIHelper.CreateAsync(this, UIType.UISubPanel_ResourceNotEnough, v3,
                        KBtn_Buy.GameObject.transform);
                    uiTip.SetParent(this, true);
                    JiYuUIHelper.SetResourceNotEnoughTip(uiTip, KBtn_Buy);

                    return;
                }
            });

            JiYuTweenHelper.SetScaleWithBounce(GetFromReference(UICommon_Sec_Confirm.KBg));
        }


        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}