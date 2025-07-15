//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using HotFix_UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UIAwardGet)]
    internal sealed class UIAwardGetEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIAwardGet;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.High;

        public override UI OnCreate()
        {
            return UI.Create<UIAwardGet>();
        }
    }

    public partial class UIAwardGet : UI, IAwake<int>
    {
        public List<GameObject> rewardItems;

        public void Initialize(int boxID)
        {
            var boxTable = ConfigManager.Instance.Tables.Tbchapter_box.DataMap;
            rewardItems = new List<GameObject>(boxTable[boxID].reward.Count);
            var language = ConfigManager.Instance.Tables.Tblanguage;
            Log.Debug(boxID.ToString(), Color.blue);

            this.GetFromReference(KCloseText).GetComponent<TMP_Text>().SetText(language["text_window_close"].current);
            this.GetFromReference(KTextInfo).GetComponent<TMP_Text>().SetText(language["text_gain_reward"].current);
            this.GetButton(KCloseBack)?.OnClick.Add(Close);

            var rewardArray = boxTable[boxID].reward.ToArray();


            foreach (var reward in rewardArray)
            {
                float rewardID = reward.x;
                float rewardCount = reward.z;
                InitAwardItem(rewardID, rewardCount);
            }
        }

        void InitAwardItem(float rewardID, float rewardCount)
        {
            var itemMap = ConfigManager.Instance.Tables.Tbuser_variable.DataMap;
            var parentUI = GetFromReference(KAwardmageRoot);
            var item = UIHelper.Create(UIType.UIAward, parentUI,
                GetFromReference(KAwardmageRoot).GetComponent<Transform>());
            //var item= GameObject.Instantiate(awardGo, GetFromReference(KLevelAwardGroup).GetRectTransform().Get()); 
            Image awardImage = item.GetComponent<Transform>().Find("AwardTypeImage").GetComponent<Image>();
            Text awardCount = item.GetComponent<Transform>().Find("AwardNumText").GetComponent<Text>();
            awardCount.text = rewardCount.ToString();
            int itemID = (int)rewardID;
            string spritePath = itemMap[itemID].icon.ToString();

            awardImage.sprite = ResourcesManager.LoadAsset<Sprite>(spritePath);
            rewardItems.Add(item.GameObject);
        }

        protected override void OnClose()
        {
            //rewardItems.Clear();
            //if (this.GetFromReference(KAwardmageRoot).Children.Count > 0)
            //{
            //    this.GetFromReference(KAwardmageRoot).RemoveChild("Award");
            //}
            base.OnClose();
        }
    }
}