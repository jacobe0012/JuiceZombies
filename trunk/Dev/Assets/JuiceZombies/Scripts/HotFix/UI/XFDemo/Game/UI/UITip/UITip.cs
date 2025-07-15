//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UITip)]
    internal sealed class UITipEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UITip;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Mid;

        public override UI OnCreate()
        {
            return UI.Create<UITip>();
        }
    }

    public partial class UITip : UI, IAwake<UIBagItem.BagItemData>
    {
        private int bagIndex;
        private int itemId;

        private Vector2 firstElem = new Vector2(-400, 685);
        private float horizontalSpacing = 200;
        private float verticalSpacing = 215;


        public void Initialize(UIBagItem.BagItemData data)
        {
            var parentUI = GetParent<UIBagThingList>();
            bagIndex = data.bagIndex;
            itemId = data.itemId;
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var item = ConfigManager.Instance.Tables.Tbitem;

            var titleText = GetFromReference(KTitleText);
            var desText = GetFromReference(KDesText);

            titleText.GetTextMeshPro().SetTMPText(language.Get(item.Get(itemId)?.name).current);
            desText.GetTextMeshPro().SetTMPText(language.Get(item.Get(itemId)?.desc).current);


            int row = bagIndex / 5;
            int column = bagIndex % 5;
            float xOffset = column * horizontalSpacing;
            float yOffset = row * verticalSpacing;

            this.GetRectTransform().SetAnchoredPosition(firstElem + new Vector2(xOffset, -yOffset));
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}