using System.Collections.Generic;
using Common;
using HotFix_UI;

namespace XFramework
{
    public class UIBagThingList : UI, IAwake<List<BagItem>>, ILoopScrollRectProvide<UIBagItem>
    {
        private List<BagItem> bagItems;
        private UI bagItemTipPanel;

        public void Initialize(List<BagItem> args)
        {
            var config = ConfigManager.Instance.Tables.TbitemOld;
            bagItems = args;

            // foreach (var bagItem in args)
            // {
            //     item.Get(bagItem.ItemId).icon
            // }

            var loopRect = this.GetLoopScrollRect();
            loopRect.SetProvideData(UIPathSet.UIBagItem, this);


            loopRect.SetTotalCount(args.Count);
            loopRect.RefillCells();
            //var f=new Vector2(0, 0);
            loopRect.OnValueChanged.Add((v) => { DisableTipPanel(); });
        }

        public void SetBagItemTipPanel(UI ui)
        {
            bagItemTipPanel = ui;
        }

        public void DisableTipPanel()
        {
            bagItemTipPanel?.Dispose();
        }

        public long GetItemCount(int itemId)
        {
            foreach (var bagItem in bagItems)
            {
                if (itemId == bagItem.ItemId)
                {
                    return bagItem.Count;
                }
            }

            Log.Debug($"没找到id为{itemId}的item");
            return 0;
        }

        public BagItem GetItem(int index)
        {
            if (bagItems[index] != null)
            {
                return bagItems[index];
            }


            Log.Debug($"没找到下标为{index}的item");
            return null;
        }

        public void ProvideData(UIBagItem ui, int index)
        {
            //Log.Debug($"UIBagItem{index}");
            ObjectHelper.Awake(ui, index);
        }

        protected override void OnClose()
        {
            bagItems.Clear();
            base.OnClose();
        }
    }
}