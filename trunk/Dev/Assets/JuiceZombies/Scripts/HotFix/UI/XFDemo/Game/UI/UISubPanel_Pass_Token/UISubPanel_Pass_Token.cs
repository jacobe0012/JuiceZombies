//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Pass_Token)]
    internal sealed class UISubPanel_Pass_TokenEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UISubPanel_Pass_Token;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Pass_Token>();
        }
    }

    public partial class UISubPanel_Pass_Token : UI, IAwake<List<GamePass>>
    {
        #region properties

        private Tblanguage tblanguage;
        private Tbbattlepass tbbattlepass;
        private Tbprice tbprice;
        private Tbbattlepass_reward tbbattlepass_Reward;
        private Tbbattlepass_exp tbbattlepass_Exp;
        private Tbactivity tbActivity;
        private Dictionary<int, battlepass_reward> rewardDic = new Dictionary<int, battlepass_reward>();

        #endregion

        public async void Initialize(List<GamePass> gamePassList)
        {
            await JiYuUIHelper.InitBlur(this);
            this.GetFromReference(KBtn_Close).GetXButton().OnClick?.Add(() => { Close(); });
            DataInit(gamePassList);
            TextInit(gamePassList);
            ImgInit().Forget();
            CreateItem(gamePassList).Forget();
            BtnCloseTipSet();
        }

        private void CloseTip()
        {
            JiYuUIHelper.DestoryAllTips();
            //UIHelper.Remove(UIType.UICommon_ItemTips);
        }

        private void BtnCloseTipSet()
        {
            this.GetFromReference(KBg).GetXButton()?.OnClick?.Add(() => { CloseTip(); });
            this.GetFromReference(KImg_Icon_Activation).GetXButton()?.OnClick?.Add(() => { CloseTip(); });
            this.GetFromReference(KScrollView).GetScrollRect().OnValueChanged.Add((a) => { CloseTip(); });
        }

        private void DataInit(List<GamePass> gamePassList)
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbbattlepass = ConfigManager.Instance.Tables.Tbbattlepass;
            tbprice = ConfigManager.Instance.Tables.Tbprice;
            tbbattlepass_Reward = ConfigManager.Instance.Tables.Tbbattlepass_reward;
            tbbattlepass_Exp = ConfigManager.Instance.Tables.Tbbattlepass_exp;
            tbActivity = ConfigManager.Instance.Tables.Tbactivity;
            foreach (var tbbpRe in tbbattlepass_Reward.DataList)
            {
                if (tbbpRe.id == tbActivity.Get(gamePassList[0].Version).link)
                {
                    rewardDic.Add(tbbpRe.level, tbbpRe);
                }
            }
        }

        private void TextInit(List<GamePass> gamePassList)
        {
            this.GetFromReference(KText_Title).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("battlepass_activate_name").current);
            this.GetFromReference(KText_Introduce).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("battlepass_activate_text").current);

            var version = tbActivity.Get(gamePassList[0].Version).link;
            var price = tbbattlepass.Get(version).price;
            this.GetFromReference(KText_Btn).GetTextMeshPro().SetTMPText(
                tbprice.Get(price).rmb
                + tblanguage.Get("common_coin_unit").current + " " +
                tblanguage.Get("battlepass_activiate_button").current);
        }

        private async UniTaskVoid ImgInit()
        {
            this.GetFromReference(KImg_Icon_Activation).GetImage()
                .SetSpriteAsync("icon_battlepass_activate", true).Forget();
        }

        private async UniTaskVoid CreateItem(List<GamePass> gamePassList)
        {
            List<Vector3> reImmediately = new List<Vector3>();
            List<Vector3> reList = new List<Vector3>();
            int gamePassCount = gamePassList.Count;
            int passExp = gamePassList[gamePassList.Count - 1].Exp;
            int level = PassLevel(passExp);
            for (int i = 1; i <= level; i++)
            {
                reImmediately.Add(rewardDic[i].reward2[0]);
            }

            int maxLevel = tbbattlepass_Exp.DataList.Count;
            if (level < maxLevel)
            {
                for (int i = level + 1; i <= maxLevel; i++)
                {
                    reList.Add(rewardDic[i].reward2[0]);
                }
            }

            JiYuUIHelper.MergeRewardList(reImmediately);
            JiYuUIHelper.MergeRewardList(reList);
            JiYuUIHelper.SortRewards(reImmediately);
            JiYuUIHelper.SortRewards(reList);
            // int allNum = reImmediately.Count + reList.Count;
            // int rowNum = allNum / 5;
            // if (allNum % 5 == 0)
            // {
            // }
            // else
            // {
            //     rowNum += 1;
            // }

            //this.GetFromReference(KContent).GetRectTransform().SetHeight(rowNum * 180 + (rowNum - 1) * 27);
            // this.GetFromReference(KContent).GetRectTransform().SetOffsetWithLeft(0);
            // this.GetFromReference(KContent).GetRectTransform().SetOffsetWithRight(0);
            var contentList = this.GetFromReference(KContent).GetList();
            contentList.Clear();
            foreach (var r in reImmediately)
            {
                var item = await contentList.CreateWithUITypeAsync<Vector3>(UIType.UISubPanel_Pass_Token_Item, r,
                    false);
                var reItem = await item.GetFromReference(UISubPanel_Pass_Token_Item.KPos_Item).GetList()
                    .CreateWithUITypeAsync<Vector3>(UIType.UICommon_RewardItem, r, false);
                reItem.GetRectTransform().SetAnchoredPositionX(0);
                reItem.GetRectTransform().SetAnchoredPositionY(0);
                JiYuUIHelper.SetRewardOnClick(r, reItem);
                item.GetFromReference(UISubPanel_Pass_Token_Item.KImg_Can_Claim).SetActive(true);
            }

            foreach (var r in reList)
            {
                var item = await contentList.CreateWithUITypeAsync<Vector3>(UIType.UISubPanel_Pass_Token_Item, r,
                    false);
                var reItem = await item.GetFromReference(UISubPanel_Pass_Token_Item.KPos_Item).GetList()
                    .CreateWithUITypeAsync<Vector3>(UIType.UICommon_RewardItem, r, false);
                reItem.GetRectTransform().SetAnchoredPositionX(0);
                reItem.GetRectTransform().SetAnchoredPositionY(0);
                JiYuUIHelper.SetRewardOnClick(r, reItem);
                item.GetFromReference(UISubPanel_Pass_Token_Item.KImg_Can_Claim).SetActive(false);
            }

            JiYuUIHelper.ForceRefreshLayout(this.GetFromReference(KContent));
        }

        private int PassLevel(int exp)
        {
            int level = 1;
            foreach (var bpExp in tbbattlepass_Exp.DataList)
            {
                if (exp >= bpExp.exp)
                {
                    level = bpExp.id;
                }
                else
                {
                    break;
                }
            }

            return level;
        }

        protected override void OnClose()
        {
            CloseTip();
            base.OnClose();
        }
    }
}