//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using HotFix_UI;

namespace XFramework
{
    [UIEvent(UIType.UIHeadBtn)]
    internal sealed class UIHeadBtnEvent : AUIEvent
    {
        public override string Key => UIPathSet.UIHeadBtn;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;


        public override UI OnCreate()
        {
            return UI.Create<UIHeadBtn>();
        }
    }

    public partial class UIHeadBtn : UI, IAwake<int>
    {
        public void Initialize(int args)
        {
            
            var userHead = ConfigManager.Instance.Tables.Tbuser_avatar.DataMap;
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var quality = ConfigManager.Instance.Tables.Tbquality.DataMap;
            var headSelect = GetFromReference(KHeadSelect);
            GetFromReference(KButton).GetXButton().OnClick.Add(async () =>
            {
                var parent = this.GetParent<UIPlayerInformtion>();


                if (parent.lastIndex == args)
                {
                    return;
                }

                headSelect.SetActive(true);

                if (parent.lastIndex != -1)
                {
                    parent.UIItemDic[parent.lastIndex]?.transform.GetChild(0).GetChild(0).gameObject
                        .SetActive(false);
                }

                parent.lastIndex = args;

                await parent.GetFromPath("Back/Up/Head/HeadImage").GetImage()
                    .SetSpriteAsync(userHead[parent.HeadLockList[args].x].icon, false);
                parent.GetFromPath("Back/Down/DescBackground/DescTxt").GetTextMeshPro()
                    .SetTMPText(language[userHead[parent.HeadLockList[args].x].desc].current);
                parent.GetFromPath("Back/Down/DescBackground/Quality/QualityTxt").GetTextMeshPro()
                    .SetTMPText(language[quality[userHead[parent.HeadLockList[args].x].quality].name].current);
                parent.GetFromPath("Back/Down/DescBackground/Quality/QualityTxt").GetTextMeshPro()
                    .SetColor("#" + quality[userHead[parent.HeadLockList[args].x].quality].fontColor);
                // await parent.GetFromPath("Back/Down/DescBackground/Quality").GetImage()
                //     .SetSpriteAsync(quality[userHead[parent.HeadLockList[args].x].quality].bg, false);

                if (parent.HeadLockList[args].x == parent.HeadID && parent.HeadLockList[args].y == 1)
                {
                    parent.GetFromPath("Back/Down/StateOrBtn/StateTxt").GetTextMeshPro()
                        .SetTMPText(language["common_state_using"].current);
                    parent.GetFromPath("Back/Down/StateOrBtn/StateTxt").SetActive(true);
                    parent.GetFromPath("Back/Down/StateOrBtn/UseBtn").SetActive(false);
                }

                if (parent.HeadLockList[args].x != parent.HeadID && parent.HeadLockList[args].y == 0)
                {
                    parent.GetFromPath("Back/Down/StateOrBtn/StateTxt").GetTextMeshPro()
                        .SetTMPText(language["common_state_locking"].current);
                    parent.GetFromPath("Back/Down/StateOrBtn/StateTxt").SetActive(true);
                    parent.GetFromPath("Back/Down/StateOrBtn/UseBtn").SetActive(false);
                }

                if (parent.HeadLockList[args].x != parent.HeadID && parent.HeadLockList[args].y == 1)
                {
                    parent.GetFromPath("Back/Down/StateOrBtn/StateTxt").SetActive(false);
                    parent.GetFromPath("Back/Down/StateOrBtn/UseBtn").SetActive(true);
                }
            });
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}