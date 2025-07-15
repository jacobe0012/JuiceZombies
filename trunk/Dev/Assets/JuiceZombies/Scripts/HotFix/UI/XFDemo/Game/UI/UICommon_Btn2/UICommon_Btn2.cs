//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using HotFix_UI;

namespace XFramework
{
    [UIEvent(UIType.UICommon_Btn2)]
    internal sealed class UICommon_Btn2Event : AUIEvent
    {
        public override string Key => UIPathSet.UICommon_Btn2;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UICommon_Btn2>();
        }
    }

    public partial class UICommon_Btn2 : UI, IAwake<int>
    {

        public int index;
        public void Initialize(int currentID)
        {
            int currentLockSkillId = ResourcesSingleton.Instance.talentID.talentSkillID;
            GetFromReference(KImg_Mask).SetActive(true);
            if (currentLockSkillId >= currentID)
            {
                GetFromReference(KImg_Mask).SetActive(false);
            }

            var tanlentMap = ConfigManager.Instance.Tables.Tbtalent.DataMap;
            var lang = ConfigManager.Instance.Tables.Tblanguage;
            GetFromReference(KImg_Prop).GetImage().SetSprite(tanlentMap[currentID].icon, false);

            GetFromReference(KText_Title).GetTextMeshPro().SetTMPText(lang.Get(tanlentMap[currentID].name).current);
            GetFromReference(KText_Level).GetTextMeshPro().SetTMPText("LV."+ tanlentMap[currentID].level.ToString());
            GetFromReference(KText_Prop).GetTextMeshPro().SetTMPText(lang.Get(tanlentMap[currentID].desc).current);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}