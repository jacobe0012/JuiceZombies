//---------------------------------------------------------------------
// UnicornStudio
// Author: huangjinguo
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UICommon_TopTab)]
    internal sealed class UICommon_TopTabEvent : AUIEvent
    {
        public override string Key => UIPathSet.UICommon_TopTab;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        //public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UICommon_TopTab>();
        }
    }

    public struct TopTabStruct
    {
        //icon图标名称
        public string icon;

        //页签名称
        public string name;

        //非选中状态下背景
        public string bg;

        //选中状态下的背景
        public string selectBg;

        //每个页签的宽度
        public int TabWidth;
    }

    public partial class UICommon_TopTab : UI, IAwake<List<TopTabStruct>>
    {
        //一级页签数量
        private int FirstLevelTabNum = 0;

        //页签List
        public List<UISubPanel_CommonBtn> TabList = new List<UISubPanel_CommonBtn>();

        //记录选中的UI
        private UISubPanel_CommonBtn lastTabUI;

        //每个页签的横向宽度
        private int TabWidth = 264;

        //放大缩小时间
        private float TabDoScaleTime = 0.1f;

        //屏幕宽度
        private float screenW = 0;

        //
        List<TopTabStruct> topTabList = new List<TopTabStruct>();

        public void Initialize(List<TopTabStruct> TopTabList)
        {
            topTabList = TopTabList;
            //初始化页签按钮
            BtnInit(TopTabList);
            //初始化页签按钮图片
            BtnImageInit(TopTabList);
        }

        /// <summary>
        /// 创建页签按钮
        /// </summary>
        private void BtnInit(List<TopTabStruct> TopTabList)
        {
            //screenW = this.GetRectTransform().Width();
            screenW = UnityEngine.Screen.width;
            FirstLevelTabNum = TopTabList.Count;
            TabWidth = TopTabList[0].TabWidth + 30;
            //设置排布的规则
            this.GetFromReference(KScrollView).GetScrollRect().Content.GetComponent<GridLayoutGroup>().cellSize =
                new Vector2(TopTabList[0].TabWidth, 92);
            //根据数量设置是否可以滑动
            if (FirstLevelTabNum * TabWidth <= screenW)
            {
                this.GetFromReference(KScrollView).GetRectTransform()
                    .SetOffsetWithLeft((screenW - FirstLevelTabNum * TabWidth) / 2);
                this.GetFromReference(KScrollView).GetComponent<ScrollRect>().movementType =
                    ScrollRect.MovementType.Clamped;
            }
            else
            {
                this.GetFromReference(KScrollView).GetRectTransform().SetOffsetWithLeft(0);
            }

            this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform()
                .SetWidth(FirstLevelTabNum * TabWidth);
            var content = this.GetFromReference(KScrollView).GetScrollRect().Content;
            var list = content.GetList();
            //生成页签按钮
            for (int i = 0; i < FirstLevelTabNum; i++)
            {
                int iHelp = i;
                var ui = list.CreateWithUIType(UIType.UISubPanel_CommonBtn, false) as UISubPanel_CommonBtn;
                //var ui = UIHelper.Create(UIType.UISubPanel_CommonBtn, uiTransform, true) as UISubPanel_CommonBtn;
                ui.index = iHelp;
                ui.icon = topTabList[iHelp].icon;
                ui.text = topTabList[iHelp].name;
                ui.GetFromReference(UISubPanel_CommonBtn.KText_Mid).SetActive(true);
                Log.Debug($"icon:{TopTabList[iHelp].icon},name:{TopTabList[iHelp].name}",Color.cyan);
                ui.GetFromReference(UISubPanel_CommonBtn.KText_Mid).GetTextMeshPro()
                    .SetTMPText(UnicornUIHelper.GetRewardTextIconName(TopTabList[iHelp].icon) + TopTabList[iHelp].name);
             
                //添加滑动居中的点击事件
                UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(ui.GetFromReference(UISubPanel_CommonBtn.KBtn_Common),
                    () => BtnOnClick(iHelp));
                //ui.GetFromReference(UISubPanel_CommonBtn.KBtn_Common).GetButton().OnClick.Add(() =>
                //{
                //    BtnOnClick(iHelp);
                //    //int posHelp = changeTabPos(iHelp, (int)screenW, FirstLevelTabNum);
                //    //this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().DoAnchoredPositionX(posHelp, TabDoScaleTime);
                //});
                //设置初始页签状态
                if (i == 0)
                {
                    ui.GetRectTransform().DoScale(new Vector3(1.1f, 1.1f, 1.0f), TabDoScaleTime);
                    lastTabUI = ui;
                }
                else
                {
                }

                //把按钮添加入List
                TabList.Add(ui);
            }

            list.Sort((a, b) =>
            {
                var uia = a as UISubPanel_CommonBtn;
                var uib = b as UISubPanel_CommonBtn;
                return uia.index.CompareTo(uib.index);
            });
            UnicornUIHelper.ForceRefreshLayout(content);
            //把初始位置设置到0
            this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform().SetAnchoredPositionX(0);
        }

        /// <summary>
        /// 初始化页签按钮图片
        /// </summary>
        private void BtnImageInit(List<TopTabStruct> TopTabList)
        {
            for (int i = 0; i < TabList.Count; i++)
            {
                int iHelp = i;
                //TabList[iHelp].GetFromReference(UICommon_TopTabBtn.KImg_2).GetImage().SetSprite(TopTabList[iHelp].selectBg, false);
                //TabList[iHelp].GetFromReference(UICommon_TopTabBtn.KImg_1).GetImage().SetSprite(TopTabList[iHelp].bg, false);
                //if (i == 0)
                //{

                //}

                if (i == 0)
                {
                    TabList[iHelp].GetFromReference(UISubPanel_CommonBtn.KImg_Btn).GetImage()
                        .SetSprite(TopTabList[iHelp].selectBg, false);
                }
                else
                {
                    TabList[iHelp].GetFromReference(UISubPanel_CommonBtn.KImg_Btn).GetImage()
                        .SetSprite(TopTabList[iHelp].bg, false);
                }
                //TabList[iHelp].GetFromReference(UISubPanel_CommonBtn.KBtn_Common).GetButton().OnClick.Add(() =>
                //{ 
                //    TabList[iHelp].GetFromReference(UISubPanel_CommonBtn.KBtn_Common).GetImage().SetSprite(TopTabList[iHelp].selectBg, false);
                //    if (TabList[iHelp] != lastTabUI)
                //    {
                //        lastTabUI.GetFromReference(UISubPanel_CommonBtn.KBtn_Common).GetImage().SetSprite(TopTabList[iHelp].bg, false);
                //        lastTabUI.GetRectTransform().DoScale(new Vector3(1.0f, 1.0f, 1.0f), TabDoScaleTime);
                //    }
                //    TabList[iHelp].GetRectTransform().DoScale(new Vector3(1.1f, 1.2f, 1.0f), TabDoScaleTime);
                //    lastTabUI = TabList[iHelp];
                //});
            }
        }

        public void BtnOnClick(int i)
        {
            int posHelp = changeTabPos(i, (int)screenW, FirstLevelTabNum);
            this.GetFromReference(KScrollView).GetScrollRect().Content.GetRectTransform()
                .DoAnchoredPositionX(posHelp, TabDoScaleTime);
            TabList[i].GetFromReference(UISubPanel_CommonBtn.KImg_Btn).GetXImage()
                .SetSprite(topTabList[i].selectBg, false);

            var selectIcon = topTabList[i].icon + "_select";
            TabList[i].GetFromReference(UISubPanel_CommonBtn.KText_Mid).GetTextMeshPro().SetTMPText(UnicornUIHelper.GetRewardTextIconName(selectIcon) + topTabList[i].name);
            //TabList[i].GetFromReference(UICommon_TopTabBtn.KImg_2).SetActive(true);
            if (TabList[i] != lastTabUI)
            {
                lastTabUI.GetFromReference(UISubPanel_CommonBtn.KImg_Btn).GetXImage()
                    .SetSprite(topTabList[i].bg, false);

                // Log.Debug($"iconName：{lastTabUI.GetFromReference(UISubPanel_CommonBtn.KImg_Left).GetXImage().}", Color.cyan);
                lastTabUI.GetFromReference(UISubPanel_CommonBtn.KText_Mid).GetTextMeshPro().SetTMPText(UnicornUIHelper.GetRewardTextIconName(lastTabUI.icon) + lastTabUI.text);
                //lastTabUI.GetFromReference(UICommon_TopTabBtn.KImg_2).SetActive(false);
                lastTabUI.GetRectTransform().DoScale(new Vector3(1.0f, 1.0f, 1.0f), TabDoScaleTime);
            }

            //TabList[i].GetRectTransform().DoScale(new Vector3(1.1f, 1.1f, 1.0f), TabDoScaleTime);
            lastTabUI = TabList[i];
        }

        /// <summary>
        /// 根据选中的页签位置确定调整页签视图x方向的位置
        /// </summary>
        private int changeTabPos(int i, int allWidth, int num)
        {
            int res = TabWidth * i - (allWidth - TabWidth) / 2;
            if (res <= 0)
            {
                res = 0;
            }

            if (res > num * TabWidth - allWidth)
            {
                res = num * TabWidth - allWidth;
            }

            return -res;
        }

        /// <summary>
        /// 删除所有的一级页签
        /// </summary>
        private void DestroyTab()
        {
            foreach (var VARIABLE in TabList)
            {
                VARIABLE.Dispose();
            }
        }

        protected override void OnClose()
        {
            DestroyTab();
            base.OnClose();
        }
    }
}