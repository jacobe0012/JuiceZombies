//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using HotFix_UI;

namespace XFramework
{
    [UIEvent(UIType.UIBagItem)]
    internal sealed class UIBagItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UIBagItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UIBagItem>();
        }
    }


    public partial class UIBagItem : UI, IAwake<int>
    {
        private long timerId;
        private int bagIndex;

        private int itemId;
        private UIBagThingList parentUI => Parent.GetParent<UIBagThingList>();


        public struct BagItemData
        {
            public int bagIndex;

            public int itemId;
        }

        public void Initialize(int index)
        {
            var item = ConfigManager.Instance.Tables.TbitemOld;

            bagIndex = index;
            itemId = parentUI.GetItem(index).ItemId;

            var icon = GetFromReference(KItemIcon);
            var count = GetFromReference(KItemCountText);
            var itemBtn = GetFromReference(KItemBtn);


            count.GetTextMeshPro().SetTMPText($"x {parentUI.GetItemCount(itemId).ToString()}");

            icon.GetImage().SetSpriteAsync(item.Get(itemId).icon, false);


            itemBtn.GetXButton().OnClick.Add(async () =>
            {
                switch (item.Get(itemId).useYn)
                {
                    case 0:

                        var ui = await UIHelper.CreateAsync(UIType.UITip, new BagItemData
                        {
                            bagIndex = bagIndex,
                            itemId = itemId
                        });
                        parentUI?.AddChild(ui);

                        parentUI.DisableTipPanel();
                        parentUI.SetBagItemTipPanel(ui);

                        break;
                    case 1:
                        // tipPanel.SetActive(true);
                        //parentUI.DisableTipPanel();

                        break;
                }
            });
        }


        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}