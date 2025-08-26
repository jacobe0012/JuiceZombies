//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Text.RegularExpressions;
using cfg.config;
using HotFix_UI;
using Main;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UICommon_Btn1)]
    internal sealed class UICommon_Btn1Event : AUIEvent
    {
        public override string Key => UIPathSet.UICommon_Btn1;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UICommon_Btn1>();
        }
    }

    public partial class UICommon_Btn1 : UI, IAwake<UIContainer_Bar.ParamterBtn>
    {
        UI tipUI;
        public int index;

        public struct Parameter
        {
            public int talentID;
            public UI parentUI;
        }


        public void UpdateBtnState(UI thisUi, int talentID, bool isDisplayArrow, bool isOnlyDisplay)
        {
            int currentUnlocked = ResourcesSingletonOld.Instance.talentID.talentPropID;
            var tanlentMap = ConfigManager.Instance.Tables.Tbtalent;
            var tanlentList = ConfigManager.Instance.Tables.Tbtalent.DataList;
            var lang = ConfigManager.Instance.Tables.Tblanguage;
            string imgPath = tanlentMap[talentID].icon;
            thisUi.GetButton(KBtn_All)?.OnClick.Clear();
            thisUi.GetFromReference(KImg_Up).SetActive(false);
            thisUi.GetFromReference(KImg_Mask).SetActive(true);
            thisUi.GetFromReference(KImg_Prop).GetImage().SetSprite(imgPath, false);

            string input = lang.Get(tanlentMap[talentID].name).current;
            //Log.Debug(input);
            //string replace = tanlentMap[talentID].para[0].ToString();
            var descStr = string.Format(input, UnityHelper.RichTextColor(tanlentMap[talentID].para[0].ToString(), "72fa52"));
            thisUi.GetFromReference(KText_Prop).GetTextMeshPro().SetTMPText(descStr);

            thisUi.GetFromReference(KLine_Green).SetActive(false);
            //if (currentUnlocked == 0)
            //{
            //    currentUnlocked = tanlentList[0].id - 1;
            //}
            //已解锁
            if (tanlentMap[talentID].id <= currentUnlocked)
            {
                thisUi.GetFromReference(KImg_Mask).SetActive(false);
                int lastId = -1;
                foreach(var talent in tanlentList)
                {
                    if (talent.preId == talentID)
                    {
                        lastId = talent.id;break;
                    }
                }

                if(lastId<=currentUnlocked&&tanlentMap.Get(lastId).level== tanlentMap.Get(talentID).level)
                {
                    thisUi.GetFromReference(KLine_Green).SetActive(true) ;
                }
                return;
            }

            else
            {
                int lockType = isCanLock(talentID, currentUnlocked);
                if (lockType == 1 && !isDisplayArrow)
                {
                    thisUi.GetFromReference(KImg_Up).SetActive(true);
                }

                if (!isOnlyDisplay)
                    thisUi.GetButton(KBtn_All)?.OnClick.Add(() => OnClickBtn(talentID, lockType));
            }
        }

        string ReplaceBetweenBraces(string input, string replacement)
        {
            Regex regex = new Regex("{(.*?)}");
            MatchCollection matches = regex.Matches(input);

            foreach (Match match in matches)
            {
                string content = match.Groups[1].Value;
                input = input.Replace(match.Value, replacement);
            }

            return input;
        }
        //string ReplaceBetweenBraces(string input, string replacement)
        //{

        //    string output = Regex.Replace(input, @"（.*?）", replacement);
        //    output = output.Replace("(", "").Replace(")", "");

        //    return input;
        //}
        private async void OnClickBtn(int talentId, int lockType)
        {
            if (lockType == 1)
            {
                Parameter currentPara;
                currentPara.talentID = talentId;
                currentPara.parentUI = this;
                tipUI = await UIHelper.CreateAsync<Parameter>(UIType.UISubPanel_Purchase, currentPara);
                UnicornUIHelper.SetTipPos(this, tipUI, UISubPanel_Purchase.KContent, UISubPanel_Purchase.KImg_Arrow,
                    UISubPanel_Purchase.KImg_Arrow);
                var pos = new Vector2(-tipUI.GetRectTransform().AnchoredPosition().x,
                    -tipUI.GetRectTransform().AnchoredPosition().y);
                tipUI.GetFromReference(UISubPanel_Purchase.KBtn_Close).GetRectTransform().SetAnchoredPosition(pos);
                tipUI.GetFromReference(UISubPanel_Purchase.KImg_Arrow).SetActive(true);
                // tipUI.GetButton(UISubPanel_Purchase.KBtn_Close)?.OnClick.Add(()=>CloseTip(tipUI));
            }
            else if (lockType == 2)
            {
                tipUI = await UIHelper.CreateAsync<int>(UIType.UISubPanel_NotPurchase, talentId);
                UnicornUIHelper.SetTipPos(this, tipUI, UISubPanel_NotPurchase.KContent, UISubPanel_NotPurchase.KImg_Arrow,
                    UISubPanel_NotPurchase.KImg_Arrow);
                var pos = new Vector2(-tipUI.GetRectTransform().AnchoredPosition().x,
                    -tipUI.GetRectTransform().AnchoredPosition().y);
                tipUI.GetFromReference(UISubPanel_NotPurchase.KBtn_Close).GetRectTransform().SetAnchoredPosition(pos);
                // tipUI.GetButton(UISubPanel_NotPurchase.KBtn_Close)?.OnClick.Add(() => CloseTip(tipUI));
            }
            else
            {
                var lang = ConfigManager.Instance.Tables.Tblanguage;
                UnicornUIHelper.ClearCommonResource();

                await UIHelper.CreateAsync(UIType.UICommon_Resource, lang.Get("talent_tip_unlock").current);
            }
        }

    

        private void CloseTip(UI tipUI)
        {
            tipUI.Dispose();

        }

        /// <summary>
        /// 返回三种解锁状态 1可解锁 2不可解锁缺钱 3 不可解锁前置不对 
        /// </summary>
        /// <param name="talentId"></param>
        /// <param name="currentUnlocked"></param>
        /// <returns></returns>
        private int isCanLock(int talentId, int currentUnlocked)
        {
            var tanlentMap = ConfigManager.Instance.Tables.Tbtalent;

            if (currentUnlocked < tanlentMap[talentId].preId)
                return 3;

            var costs = tanlentMap[talentId].cost;
            for (int i = 0; i < costs.Count; i++)
            {
                if (!JudgeCosts(costs[i]))
                {
                    return 2;
                }
            }

            return 1;
        }

        private bool JudgeCosts(Vector3 vector3)
        {
            switch (vector3.x)
            {
                case 1:
                    return ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Energy >= vector3.z ? true : false;
                case 2:
                    return ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin >= vector3.z ? true : false;
                case 3:

                    return ResourcesSingletonOld.Instance.UserInfo.RoleAssets.UsBill >= vector3.z ? true : false;
            }

            return false;
        }

        protected override void OnClose()
        {
            //tipUI.ClosePanel();
            base.OnClose();
        }

        public void Initialize(UIContainer_Bar.ParamterBtn args)
        {
            tipUI = default;
            UpdateBtnState(this, args.talentID, args.isDisplayArrow, args.isOnlyDisplay);

            //UnicornTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(UICommon_Btn1.KMid), 0, 20, 0.35f, false, false);


            //UnicornUIHelper.ChangePaddingLR(this, 50, 0.2f);
            //UnicornTweenHelper.ChangeSoftness(this, 300, 0.35f);
        }
    }
}