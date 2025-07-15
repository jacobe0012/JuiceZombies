//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using cfg.config;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Equipment)]
    internal sealed class UISubPanel_EquipmentEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UISubPanel_Equipment;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Equipment>();
        }
    }

    public struct ModuleInfoStruct
    {
        //模块Id
        public int moduleId;

        //排序值
        public int sortValues;

        //二级页签名
        public string name;

        public string icon;
    }

    public partial class UISubPanel_Equipment : UI, IAwake
    {
        private Tbtag_func tbtag_func;
        private Tblanguage tblanguage;
        private int tag_id = 2;

        private int TypeTopTab = -1;

        //顶部最大标签数
        private int MaxTopTabNum = 4;

        //顶部UI引用
        private List<UICommon_SubBtn> TopTabUIS = new List<UICommon_SubBtn>();

        //背包页签下的小模块信息
        public List<ModuleInfoStruct> ModuleS = new List<ModuleInfoStruct>();


        //一级页签下的内容UI
        public UI Contentsui = null;

        //上一次点击按钮的索引
        private int lastPosId;
        private int posIdCount;
        public int tagId = 2;

        public void Initialize()
        {
            InitJson();
            Init().Forget();
        }

        protected override void OnClose()
        {
            Contentsui?.Dispose();
            Contentsui = null;
            ModuleS.Clear();

            //OnDestroyUI();
            base.OnClose();
        }


        //初始化json文件
        private void InitJson()
        {
            tbtag_func = ConfigManager.Instance.Tables.Tbtag_func;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
        }

        // 自定义比较器
        private class MyTagFuncComparer : IEqualityComparer<tag_func>
        {
            public bool Equals(tag_func x, tag_func y)
            {
                return x.tagId == y.tagId && x.posType == y.posType;
            }

            public int GetHashCode(tag_func obj)
            {
                return obj.tagId.GetHashCode() ^ obj.posType.GetHashCode();
            }
        }


        //初始化顶部页签
        private async UniTask Init()
        {
            var KTops = GetFromReference(UISubPanel_Equipment.KTops);
            var list = KTops.GetList();
            list.Clear();
            ModuleInfo();
            var uniqueList = tbtag_func.DataList.Where(a => a.tagId == 2).GroupBy(obj => obj.posType) // 按照 id 分组
                .Select(group => group.First()) // 每个 id 只选择第一个元素
                .ToList();
            posIdCount = uniqueList.Count;
            foreach (var tag in uniqueList)
            {
                //然后对此初始化
                //Log.Error($"tag.posType {tag.posType}");
                var ui =
                    await list.CreateWithUITypeAsync(UIType.UICommon_SubBtn, tag.posType, false) as UICommon_SubBtn;
                var KBtn = ui.GetFromReference(UICommon_SubBtn.KBtn);
                //读表读取名字
                string tagName = tblanguage.Get(tag.sub1Name).current;

                //初始化状态
                ui.InitWideget(tagName, true, tag.sub1Icon);

                ui.ChageState(false);

                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn, () => OnBtnClickEvent(tag.posType));

                //如果页签为1
                if (tag.posType == 1)
                {
                    OnBtnClickEvent(tag.posType);
                }
            }

            list.Sort((a, b) =>
            {
                var uia = a as UICommon_SubBtn;
                var uib = b as UICommon_SubBtn;
                return uia.posId.CompareTo(uib.posId);
            });
            //更新页签类型
            //TypeTopTab = tbtag_func.DataList[i].posType;
        }


        public async void OnBtnClickEvent(int posId, bool isRefresh = false)
        {
            //Log.Debug(ModuleS.Count.ToString(), Color.red);

            if (lastPosId == posId && !isRefresh) return;
            var KTops = GetFromReference(UISubPanel_Equipment.KTops);
            var list = KTops.GetList();
            foreach (var child in list.Children)
            {
                var childs = child as UICommon_SubBtn;
                childs.ChageState(false);
            }

            foreach (var child in list.Children)
            {
                var childs = child as UICommon_SubBtn;
                if (childs.posId == posId)
                {
                    childs.ChageState(true);
                    break;
                }
            }

            lastPosId = posId;
            // if (isRefresh)
            // {
            //     switch (lastPosId)
            //     {
            //         case 1:
            //             if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui2))
            //             {
            //                 var uis = ui2 as UIPanel_Equipment;
            //                 uis.scrollRect.SetEnabled(true);
            //                 uis.DestorySelected();
            //                 uis.RefreshItemAndEquip();
            //             }
            //
            //             break;
            //         case 2:
            //             if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Talent, out UI ui))
            //             {
            //                 //Log.Debug("TryGetUI(UIType.UIPanel_Talent,out UI ui)",Color.cyan);
            //                 var talent = ui as UIPanel_Talent;
            //                 talent.UpdateResource();
            //                 talent.SetTalentSkill();
            //             }
            //
            //             break;
            //         case 3:
            //             break;
            //         case 4:
            //             break;
            //     }
            //
            //     return;
            // }

            Contentsui?.Dispose();
            Contentsui = null;

            var KContents = GetFromReference(UISubPanel_Equipment.KContents);
            switch (posId)
            {
                case 1:
                {
                    // Contentsui = await UIHelper.CreateAsyncNew(KContents, UIType.UIPanel_Equipment,
                    //     ModuleS,
                    //     KContents.GetComponent<RectTransform>());

                    Contentsui = await UIHelper.CreateAsync(this, UIType.UIPanel_Equipment,
                        ModuleS, KContents.GetComponent<RectTransform>());

                    UIHelper.TryGetUIManager().TryAddAllUIs(UIType.UIPanel_Equipment, Contentsui);
                    if (ResourcesSingleton.Instance.isUIInit)
                    {
                        AudioManager.Instance.PlayFModAudio(1220);
                    }

                 

                    //Contentsui.GetComponent<RectTransform>().SetScale3(1);
                }
                    break;
                case 2:
                {
                    // Contentsui = await UIHelper.CreateAsyncNew(KContents, UIType.UIPanel_Talent,
                    //     KContents.GetComponent<RectTransform>());
                    Contentsui = await UIHelper.CreateAsync(this, UIType.UIPanel_Talent,
                        KContents.GetComponent<RectTransform>());
                    UIHelper.TryGetUIManager().TryAddAllUIs(UIType.UIPanel_Talent, Contentsui);
                  
                    if (ResourcesSingleton.Instance.isUIInit)
                    {
                        AudioManager.Instance.PlayFModAudio(1222);
                    }
                }
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
        }

        public void Refresh()
        {
            OnBtnClickEvent(lastPosId, true);
        }

        public async void OnLoopClick()
        {
            int id = lastPosId + 1;
            if (id > posIdCount)
            {
                id = 1;
            }

            //Log.Error($"id{id} lastPosId{lastPosId} posIdCount{posIdCount}");
            OnBtnClickEvent(id, true);
        }

        private void ModuleInfo()
        {
            for (int i = 0; i < tbtag_func.DataList.Count; i++)
            {
                if (tag_id == tbtag_func.DataList[i].tagId)
                {
                    switch (tbtag_func.DataList[i].posType)
                    {
                        case 1:
                        {
                            ModuleInfoStruct temp = new ModuleInfoStruct
                            {
                                moduleId = tbtag_func.DataList[i].id,
                                sortValues = tbtag_func.DataList[i].sort,
                                name = tbtag_func.DataList[i].name,
                                icon = tbtag_func.DataList[i].icon,
                            };
                            ModuleS.Add(temp);
                            break;
                        }
                        case 2:
                        {
                            break;
                        }
                        case 3:
                        {
                            break;
                        }
                        case 4:
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}