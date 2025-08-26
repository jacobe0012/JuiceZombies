//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using cfg.config;
using HotFix_UI;

namespace XFramework
{
    [UIEvent(UIType.UIContainer_Bar)]
    internal sealed class UIContainer_BarEvent : AUIEvent
    {
        public override string Key => UIPathSet.UIContainer_Bar;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UIContainer_Bar>();
        }
    }

    public partial class UIContainer_Bar : UI, IAwake<int>
    {
        public int index;
        private List<talent> talentList;
        private Tbtalent_level talentLevelMap;
        private Tblanguage lang;

        public struct ParamterBtn
        {
            public int talentID;
            public bool isDisplayArrow;
            public bool isOnlyDisplay;
        }

        public void Initialize()
        {
            int currentID = ResourcesSingletonOld.Instance.talentID.talentPropID;
        }

        public async void Initialize(int level)
        {
            InitConfig();
            GetFromReference(KTxt_Level).GetTextMeshPro().SetTMPText(level.ToString());
            GetFromReference(KTxt_Name).GetTextMeshPro().SetTMPText(lang.Get(talentLevelMap[level].name).current);


            var containerList = GetFromReference(KContainer_Prop).GetList();
            containerList.Clear();
            for (int i = 0; i < talentList.Count; i++)
            {
                if (talentList[i].type == 1 && talentList[i].level == level)
                {
                    int index = i;
                    // Log.Debug($"tanlentList[i]:{tanlentList[i]}");
                    ParamterBtn paramterBtn;
                    paramterBtn.talentID = talentList[i].id;
                    paramterBtn.isDisplayArrow = true;
                    paramterBtn.isOnlyDisplay = true;
                    var ui = await containerList.CreateWithUITypeAsync<UIContainer_Bar.ParamterBtn>(
                        UIType.UICommon_Btn1, paramterBtn, false) as UICommon_Btn1;
                    ui.index = index;
                }
            }

            containerList.Sort((a, b) =>
            {
                var uia = a as UICommon_Btn1;
                var uib = b as UICommon_Btn1;
                return uia.index.CompareTo(uib.index);
            });
        }

        private void InitConfig()
        {
            talentList = ConfigManager.Instance.Tables.Tbtalent.DataList;
            talentLevelMap = ConfigManager.Instance.Tables.Tbtalent_level;
            lang = ConfigManager.Instance.Tables.Tblanguage;
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}