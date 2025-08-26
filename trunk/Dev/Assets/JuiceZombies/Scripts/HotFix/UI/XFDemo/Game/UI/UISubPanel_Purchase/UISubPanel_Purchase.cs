//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using Common;
using Google.Protobuf;
using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Purchase)]
    internal sealed class UISubPanel_PurchaseEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UISubPanel_Purchase;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Mid;

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Purchase>();
        }
    }

    public partial class UISubPanel_Purchase : UI, IAwake<UICommon_Btn1.Parameter>
    {
        public int currentTalentID;
        bool isLockSucess;
        private Tbtalent talentMap;
        private Tblanguage lang;

        protected override void OnClose()
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.LOCKTALENT, JudegeLockTalent);
            Log.Debug("ClosePanel");
            base.OnClose();
        }

        private void JudegeLockTalent(object sender, WebMessageHandlerOld.Execute e)
        {
            var value = new StringValue();
            value.MergeFrom(e.data);
            Log.Debug(value.Value, Color.cyan);
            if (value.Value != "true")
            {
                return;
            }

            isLockSucess = true;
            if (isLockSucess)
            {
                //NetWorkManager.Instance.SendMessage(CMDOld.QUERYPROPERTY);
                Log.Debug("wewqeeeeeeeeeeeeeeeeeee");
                ResourcesSingletonOld.Instance.talentID.talentPropID = currentTalentID;

                var cost = new Vector3(talentMap[currentTalentID].cost[0].x, talentMap[currentTalentID].cost[0].y,
                    talentMap[currentTalentID].cost[0].z);
                UnicornUIHelper.TryReduceReward(cost);

                if (UnicornUIHelper.TryGetUI(UIType.UIPanel_Talent, out UI ui))
                {
                    var uiPanel_Talent = ui as UIPanel_Talent;
                    uiPanel_Talent?.UpdateContainer();
                }
            }

            Close();
        }

        private void OnButtonLockClick()
        {
            //�����츳
            NetWorkManager.Instance.SendMessage(CMDOld.LOCKTALENT, new IntValue { Value = currentTalentID });
        }


        public void Initialize(UICommon_Btn1.Parameter args)
        {
            InitJson();
            isLockSucess = false;
            currentTalentID = args.talentID;
            this.SetParent(args.parentUI, false);
            GetFromReference(KImg_Left).SetActive(true);
            GetFromReference(KText_Right).SetActive(true);
            GetFromReference(KText_Mid).SetActive(true);
            GetFromReference(KImg_Arrow).SetActive(true);
            GetFromReference(KImg_Left).GetImage().SetSprite("icon_money", false);
            if (talentMap.GetOrDefault(args.talentID).cost.Count > 0)
            {
                var cost = talentMap.GetOrDefault(args.talentID).cost[0].z;
                GetFromReference(KText_Mid).GetTextMeshPro().SetTMPText(cost.ToString());
                GetFromReference(KText_Right).GetTextMeshPro().SetTMPText(lang.Get("talent_attr_unlock").current);
                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(this.GetFromReference(KBtn_Common), () => OnButtonLockClick());
                WebMessageHandlerOld.Instance.AddHandler(CMDOld.LOCKTALENT, JudegeLockTalent);
                this.GetFromReference(KBtn_Close)?.GetComponent<XButton>()?.onClick.Add(Close);
            }
            else
            {
                Debug.LogError("�������ã�������������������������������������������������������������");
                return;
            }
        }

        private void InitJson()
        {
            talentMap = ConfigManager.Instance.Tables.Tbtalent;
            lang = ConfigManager.Instance.Tables.Tblanguage;
        }
    }
}