//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Threading.Tasks;
using cfg.config;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using TMPro;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UICommon_ItemTips)]
    internal sealed class UICommon_ItemTipsEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UICommon_ItemTips;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Mid;

        public override UI OnCreate()
        {
            return UI.Create<UICommon_ItemTips>();
        }
    }


    public partial class UICommon_ItemTips : UI,
        IAwake<UICommon_ItemTips.TipsData>
    {
        public enum ArrowType
        {
            Default,
            Up,
            Down
        }

        public struct ItemTipsData
        {
            public UI itemUI;
            public string KTxt_Title;
            public string KTxt_Des;
            public ArrowType ArrowType;
        }

        /// <summary>
        /// 通用tips参数
        /// </summary>
        public struct TipsData
        {
            public UI itemUI;
            /// <summary>
            /// 可选 2选1
            /// </summary>
            public Vector3 reward;
            /// <summary>
            /// 可选 2选1
            /// </summary>
            public string title;
            public string desc;
        }


        private Tblanguage tblanguage;

        public async void Initialize(TipsData args)
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            var reward = args.reward;
            bool isReward = false;
            if (args.reward.x > 0)
            {
                isReward = true;
            }

            //this.SetActive(false);
            var KTxt_Title = GetFromReference(UICommon_ItemTips.KTxt_Title);
            var KTxt_Des = GetFromReference(UICommon_ItemTips.KTxt_Des);
            var KContent = GetFromReference(UICommon_ItemTips.KContent);
            var KImg_ArrowDown = GetFromReference(UICommon_ItemTips.KImg_ArrowDown);
            var KImg_ArrowUp = GetFromReference(UICommon_ItemTips.KImg_ArrowUp);
            var KBg_Down = GetFromReference(UICommon_ItemTips.KBg_Down);
            var KBg_Up = GetFromReference(UICommon_ItemTips.KBg_Up);
            var KVerticalLayout = GetFromReference(UICommon_ItemTips.KVerticalLayout);
            var KTxt_Num = GetFromReference(UICommon_ItemTips.KTxt_Num);
            if (isReward)
            {
                var countStr = $"{tblanguage.Get("common_have").current}{UnicornUIHelper.GetRewardCountFormat(reward)}";
                KTxt_Num.GetTextMeshPro().SetTMPText(countStr);
                UnicornUIHelper.SetRewardOnClickTipTitleDes(reward, KTxt_Title, KTxt_Des);
            }
            else
            {
                if (args.title != null)
                {
                    KTxt_Title?.SetActive(true);
                    KTxt_Title?.GetTextMeshPro().SetTMPText(args.title);
                }
                else
                {
                    KTxt_Title?.SetActive(false);
                }

                if (args.desc != null)
                {
                    KTxt_Des?.SetActive(true);
                    KTxt_Des?.GetTextMeshPro().SetTMPText(args.desc);
                }
                else
                {
                    KTxt_Des?.SetActive(false);
                }
            }

            //UnicornUIHelper.SetTipPosNew(args.itemUI, this, UICommon_ItemTips.KContent,
            //    UICommon_ItemTips.KImg_ArrowDown, UICommon_ItemTips.KImg_ArrowUp);
            var itemRec = args.itemUI.GetRectTransform();
            var pivot = itemRec.Pivot();
            pivot.x -= 0.5f;
            pivot.y -= 0.5f;
            Vector2 pivotOffset = new Vector2(pivot.x * itemRec.Width() * itemRec.Scale().x,
                pivot.y * itemRec.Height() * itemRec.Scale().y);

            var itemPos = UnicornUIHelper.GetUIPos(args.itemUI);

            var pos = new Vector2(itemPos.x - pivotOffset.x, itemPos.y - pivotOffset.y);
            Log.Debug($"itemPos{itemPos}");
            var tipsRec = this.GetRectTransform();
            var isUp = itemPos.y > 0 ? false : true;

            var halfTipHeight = tipsRec.Height() * tipsRec.Scale().y / 2f;

            var halfItemHeight = itemRec.Height() * itemRec.Scale().y / 2f;
            pos.y += isUp ? halfTipHeight + halfItemHeight : -(halfTipHeight + halfItemHeight);
            KImg_ArrowDown.SetActive(isUp);
            KImg_ArrowUp.SetActive(!isUp);
            KBg_Up.SetActive(!isUp);
            KBg_Down.SetActive(isUp);


            this.GetRectTransform().SetAnchoredPosition(pos);
            await UniTask.Yield();
            var contentRect = KContent.GetRectTransform();
            contentRect.SetHeight(KVerticalLayout.GetRectTransform().Height());
            //content上下调整
            if (isUp)
            {
                contentRect.SetAnchorMin(new Vector2(0.5f, 0));
                contentRect.SetAnchorMax(new Vector2(0.5f, 0));
                contentRect.SetPivot(new Vector2(0.5f, 0));
                contentRect.SetAnchoredPositionY(KImg_ArrowDown.GetRectTransform().Height());
            }
            else
            {
                contentRect.SetAnchorMin(new Vector2(0.5f, 1));
                contentRect.SetAnchorMax(new Vector2(0.5f, 1));
                contentRect.SetPivot(new Vector2(0.5f, 1));
                contentRect.SetAnchoredPositionY(-40f);
            }

            //content左右调整
            const float Interval = 30f;
            var maxScreenX = Screen.width / 2f;
            var contentWidth = contentRect.Width();
            contentRect.SetAnchoredPositionX(0);

            if (itemPos.x + contentWidth / 2f > maxScreenX)
            {
                var moveX = (itemPos.x + contentWidth / 2f) - maxScreenX;
                contentRect.SetAnchoredPositionX(-moveX - Interval);
            }
            else if (itemPos.x - contentWidth / 2f < -maxScreenX)
            {
                var moveX = (itemPos.x - contentWidth / 2f) - (-maxScreenX);
                contentRect.SetAnchoredPositionX(-moveX + Interval);
            }

            //await UniTask.Yield();
            //this.SetActive(true);
            //this.SetActive(true);
        }

        // public void Initialize(ItemTipsData args)
        // {
        //     bool titleNull = false;
        //     if (args.KTxt_Title == null)
        //     {
        //         titleNull = true;
        //     }
        //
        //     const float DefalutContentHeitgt = 156f;
        //     const float DefalutDesPosY = -65f;
        //     const float DefaultContentPosY = -52f;
        //     var KTxt_Title = GetFromReference(UICommon_ItemTips.KTxt_Title);
        //     var KTxt_Des = GetFromReference(UICommon_ItemTips.KTxt_Des);
        //     var KContent = GetFromReference(UICommon_ItemTips.KContent);
        //     var KImg_ArrowDown = GetFromReference(UICommon_ItemTips.KImg_ArrowDown);
        //     var KImg_ArrowUp = GetFromReference(UICommon_ItemTips.KImg_ArrowUp);
        //
        //     KTxt_Title.SetActive(!titleNull);
        //
        //     KTxt_Des.GetTextMeshPro().SetTMPText(args.KTxt_Des);
        //     KTxt_Des.GetTextMeshPro().Get().alignment = TextAlignmentOptions.Left;
        //
        //     float titleHeight = 0;
        //
        //     if (!titleNull)
        //     {
        //         KTxt_Des.GetRectTransform().SetAnchoredPosition(new Vector2(0, DefalutDesPosY));
        //         KTxt_Title.GetTextMeshPro().SetTMPText(args.KTxt_Title);
        //         titleHeight = KTxt_Title.GetRectTransform().Height();
        //     }
        //     else
        //     {
        //         KTxt_Des.GetRectTransform().SetAnchoredPosition(Vector2.zero);
        //     }
        //
        //     var KContentRec = KContent.GetRectTransform();
        //
        //     var desHeight = KTxt_Des.GetTextMeshPro().Get().preferredHeight;
        //     //var titleHeight = KTxt_Title.GetTextMeshPro().Get().preferredHeight;
        //     KTxt_Des.GetRectTransform().SetHeight(desHeight);
        //     //KTxt_Title.GetRectTransform().SetHeight(titleHeight);
        //
        //     KContentRec.SetHeight(desHeight + 10f + titleHeight + DefaultContentPosY);
        //     var offset = KContentRec.Height() - DefalutContentHeitgt;
        //
        //     KImg_ArrowDown.GetRectTransform().SetAnchoredPosition(new Vector2(0, -offset));
        //     KContentRec.SetAnchoredPositionY(DefaultContentPosY);
        //     KContentRec.SetAnchoredPositionX(0);
        //     KImg_ArrowUp.GetRectTransform().SetAnchoredPositionX(0);
        //
        //     UnicornUIHelper.SetTipPosNew(args.itemUI, this, UICommon_ItemTips.KContent,
        //         UICommon_ItemTips.KImg_ArrowDown, UICommon_ItemTips.KImg_ArrowUp, args.ArrowType);
        //     KContentRec.SetAnchoredPositionY(DefaultContentPosY);
        // }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}