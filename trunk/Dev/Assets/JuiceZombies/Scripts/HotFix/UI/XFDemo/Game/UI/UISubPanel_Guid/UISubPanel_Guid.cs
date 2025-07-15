//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using Main;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Guid)]
    internal sealed class UISubPanel_GuidEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UISubPanel_Guid;

        public override bool IsFromPool => false;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.High;


        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Guid>();
        }
    }

    public partial class UISubPanel_Guid : UI, IAwake<int>, IAwake<int, int>
    {
        public int guideId;
        public int subId;
        public Tbguide tbguide;
        public Tblanguage tblanguage;

        private EntityManager entityManager;
        private EntityQuery wbeQuery;
        private EntityQuery playerQuery;
        public bool stop = false;

        public void Initialize(int args, int subId)
        {
            guideId = args;
            this.subId = subId;
            Log.Error($"guideId{guideId} subid{subId}");
            InitJson();
            InitNode();
        }

        public void Initialize(int args)
        {
            guideId = args;
            InitJson();
            InitNode();
        }

        void InitJson()
        {
            tbguide = ConfigManager.Instance.Tables.Tbguide;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
        }

        async UniTaskVoid InitNode()
        {
            const float BoarderLength = 20f;
            var KImg_Tips_1 = GetFromReference(UISubPanel_Guid.KImg_Tips_1);
            var KImg_Tips_2 = GetFromReference(UISubPanel_Guid.KImg_Tips_2);
            var KImg_Tips_3 = GetFromReference(UISubPanel_Guid.KImg_Tips_3);
            var KImg_Tips_4 = GetFromReference(UISubPanel_Guid.KImg_Tips_4);
            var KImg_Bg = GetFromReference(UISubPanel_Guid.KImg_Bg);
            var KImg_noForceBg = GetFromReference(UISubPanel_Guid.KImg_noForceBg);
            var KBtn_Bg = GetFromReference(UISubPanel_Guid.KBtn_Bg);
            var KImg_Tips_5 = GetFromReference(UISubPanel_Guid.KImg_Tips_5);
            var KImg_Tips_6 = GetFromReference(UISubPanel_Guid.KImg_Tips_6);
            var KImg_Tips_7 = GetFromReference(UISubPanel_Guid.KImg_Tips_7);
            var KText_Tips_2 = GetFromReference(UISubPanel_Guid.KText_Tips_2);
            var KText_Tips_4 = GetFromReference(UISubPanel_Guid.KText_Tips_4);
            var KText_Tips_5 = GetFromReference(UISubPanel_Guid.KText_Tips_5);
            var KText_Tips_6 = GetFromReference(UISubPanel_Guid.KText_Tips_6);
            var KText_Tips_13 = GetFromReference(UISubPanel_Guid.KText_Tips_13);
            var KImg_GuideAnim_4 = GetFromReference(UISubPanel_Guid.KImg_GuideAnim_4);
            var KImg_GuideAnim_2 = GetFromReference(UISubPanel_Guid.KImg_GuideAnim_2);
            var KImg_GuideAnim_3 = GetFromReference(UISubPanel_Guid.KImg_GuideAnim_3);
            var KImg_GuideAnim_1 = GetFromReference(UISubPanel_Guid.KImg_GuideAnim_1);


            this.GetRectTransform().SetAnchoredPosition(Vector2.zero);
            KImg_Tips_1.SetActive(false);
            KImg_Tips_2.SetActive(false);
            KImg_Tips_3.SetActive(false);
            KImg_Tips_4.SetActive(false);
            KImg_Bg.SetActive(false);
            KImg_noForceBg.SetActive(false);
            KBtn_Bg.SetActive(false);
            KImg_Tips_5.SetActive(false);
            KImg_Tips_6.SetActive(false);
            KImg_Tips_7.SetActive(false);
            Log.Debug($"KImg_Bg{guideId} {KImg_Bg.GetXImage().Get().name}");
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            playerQuery = entityManager.CreateEntityQuery(typeof(PlayerData));


            var guide = tbguide.Get(guideId);

            switch (guide.guideType)
            {
                case 311:

                    KImg_Tips_4.SetActive(true);
                    KImg_noForceBg.SetActive(true);

                    JiYuUIHelper.StartStopTime(false);
                    //JiYuUIHelper.EnableTriggerSystem(false);
                    //UnityHelper.StopTime();
                    KText_Tips_4.GetTextMeshPro().SetTMPText(tblanguage.Get(guide.guidePara[0]).current);

                    var uiBtn = KImg_noForceBg.GetXButton();
                    if (uiBtn == null)
                    {
                        Log.Error($"{this.Name}.GetXButton() is null");
                        return;
                    }

                    uiBtn.SetPointerActive(true);

                    uiBtn.SetLongPressInterval(0.5f);

                    uiBtn.SetMaxLongPressCount(JiYuTweenHelper.MaxLongPressCount);

                    uiBtn.OnLongPressEnd.Add((a) =>
                    {
                        JiYuUIHelper.StartStopTime(true);
                        Close();
                    });

                    break;
                case 312:

                    KImg_Tips_7.SetActive(true);
                    KImg_Bg.SetActive(true);

                    var para3121 = int.Parse(guide.guidePara[0]);
                    var para3122 = guide.guidePara[1];
                    //KImg_Avator.GetImage().SetSpriteAsync($"pic_introguide_frame_{para1}",false);
                    var str312 = tblanguage.Get($"{para3122}").current;
                    KText_Tips_13.GetTextMeshPro().SetTMPText(str312);


                    break;
                case 314:

                    KImg_Tips_7.SetActive(true);
                    KImg_Bg.SetActive(true);

                    var para3141 = int.Parse(guide.guidePara[0]);
                    var para3142 = guide.guidePara[1];
                    //KImg_Avator.GetImage().SetSpriteAsync($"pic_introguide_frame_{para1}",false);
                    var str314 = tblanguage.Get($"{para3142}").current;
                    KText_Tips_13.GetTextMeshPro().SetTMPText(str314);


                    break;

                case 316:
                    KImg_Bg.SetActive(true);
                    KImg_Tips_3.SetActive(true);
                    int tagId = 2;
                    UI itemUI = null;
                    if (subId == 1)
                    {
                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var ui))
                        {
                            var KOptions = ui.GetFromReference(UIPanel_JiyuGame.KOptions);
                            var KOptionslist = KOptions.GetList();
                            foreach (var child in KOptionslist.Children)
                            {
                                var childs = child as UISubPanel_ToggleItem;
                                if (childs.sort == tagId)
                                {
                                    itemUI = childs;
                                    break;
                                }
                            }
                        }
                    }
                    else if (subId == 2)
                    {
                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui))
                        {
                            var KBottom = ui.GetFromReference(UIPanel_Equipment.KBottom);

                            var KOptionslist = KBottom.GetList();
                            var lable = KOptionslist.Children[0] as UICommon_Label;
                            var KContainer_Content = lable.GetFromReference(UICommon_Label.KContainer_Content);
                            itemUI = KContainer_Content.GetList().Children[0];
                        }
                    }
                    else if (subId == 3)
                    {
                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_EquipTips, out var ui))
                        {
                            itemUI = ui.GetFromReference(UIPanel_EquipTips.KText_Equip);
                        }
                    }

                    JiYuUIHelper.SetForceGuideRectUI(itemUI, KImg_Bg);

                    var pos = JiYuUIHelper.GetUIPos(itemUI);
                    //pos.y += childs.GetRectTransform().Height();
                    KImg_Tips_3.GetRectTransform().SetAnchoredPosition(pos);
                    KImg_GuideAnim_3.GetRectTransform()
                        .SetWidth(itemUI.GetRectTransform().Width() + BoarderLength);
                    KImg_GuideAnim_3.GetRectTransform()
                        .SetHeight(itemUI.GetRectTransform().Height() + BoarderLength);
                    KImg_GuideAnim_1.GetRectTransform()
                        .SetWidth(itemUI.GetRectTransform().Width() + BoarderLength);
                    KImg_GuideAnim_1.GetRectTransform()
                        .SetHeight(itemUI.GetRectTransform().Height() + BoarderLength);
                    break;

                case 317:
                    KImg_Bg.SetActive(true);
                    KImg_Tips_3.SetActive(true);
                    int tagId317 = 1;
                    UI itemUI317 = null;
                    if (subId == 1)
                    {
                        Log.Error($"111");
                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var ui))
                        {
                            Log.Error($"222");
                            var KOptions = ui.GetFromReference(UIPanel_JiyuGame.KOptions);
                            var KOptionslist = KOptions.GetList();
                            foreach (var child in KOptionslist.Children)
                            {
                                var childs = child as UISubPanel_ToggleItem;
                                if (childs.sort == tagId317)
                                {
                                    Log.Error($"333");
                                    itemUI317 = childs;
                                    break;
                                }
                            }
                        }
                    }
                    else if (subId == 2)
                    {
                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Shop, out var ui))
                        {
                            var scrollRect = ui.GetFromReference(UIPanel_Shop.KScrollView).GetXScrollRect();
                            scrollRect.SetVerticalNormalizedPosition(0);
                            var KContent = scrollRect.Content;
                            var list = KContent.GetList();
                            Log.Debug($"list.Count{list.Children.Count}");
                            var uichild = list.Children[list.Children.Count - 1];

                            var KBg = uichild.GetFromReference(UISubPanel_Shop_Box_Bg.KBg);
                            var list1 = KBg.GetList();
                            itemUI317 = list1.Children[0].GetFromReference(UISubPanel_Shop_1103_Box.KBuyBtn);
                            
                            
                        }
                    }


                    JiYuUIHelper.SetForceGuideRectUI(itemUI317, KImg_Bg);

                    var pos317 = JiYuUIHelper.GetUIPos(itemUI317);
                    //pos.y += childs.GetRectTransform().Height();
                    KImg_Tips_3.GetRectTransform().SetAnchoredPosition(pos317);
                    KImg_GuideAnim_3.GetRectTransform()
                        .SetWidth(itemUI317.GetRectTransform().Width() + BoarderLength);
                    KImg_GuideAnim_3.GetRectTransform()
                        .SetHeight(itemUI317.GetRectTransform().Height() + BoarderLength);
                    KImg_GuideAnim_1.GetRectTransform()
                        .SetWidth(itemUI317.GetRectTransform().Width() + BoarderLength);
                    KImg_GuideAnim_1.GetRectTransform()
                        .SetHeight(itemUI317.GetRectTransform().Height() + BoarderLength);
                    break;
            }


            //
            //
            //
            // if (ResourcesSingleton.Instance.settingData.GuideList.Contains(guide.group))
            // {
            //     ResourcesSingleton.Instance.settingData.GuideList.Remove(guide.group);
            // }
            //
            //
            // Log.Debug($"开始新手引导ID:{guide.id}");
            // // switch (guide.templateId)
            // // {
            // //     case 1:
            // //         KImg_Tips_1.SetActive(true);
            // //         var child = KImg_Tips_1.GameObject.transform.GetChild(0);
            // //         var ui = KImg_Tips_1.AddChild(UIType.UICommon_ItemTips, child.gameObject, true);
            // //         var KImg_ArrowUp = ui.GetFromReference(UICommon_ItemTips.KImg_ArrowUp);
            // //         //KImg_ArrowUp.SetActive(false);
            // //         //ui.GetRectTransform().SetAnchoredPositionY(250);
            // //
            // //         var KTxt_Des = ui.GetFromReference(UICommon_ItemTips.KTxt_Des);
            // //
            // //         string desc = default;
            // //         if (guide.desc.Count == 1)
            // //         {
            // //             desc = tblanguage.Get(guide.desc[0]).current;
            // //         }
            // //         else if (guide.desc.Count > 1)
            // //         {
            // //             List<string> copiedList = new List<string>();
            // //             copiedList.AddRange(guide.desc);
            // //             copiedList.RemoveAt(0);
            // //             desc = string.Format(tblanguage.Get(guide.desc[0]).current, copiedList);
            // //         }
            // //
            // //         KTxt_Des.GetTextMeshPro().SetTMPText(desc);
            // //         JiYuEventManager.Instance.RegisterEvent("OnSwitchL10NResponse", (a) =>
            // //         {
            // //             if (guide.desc.Count == 1)
            // //             {
            // //                 desc = tblanguage.Get(guide.desc[0]).current;
            // //             }
            // //             else if (guide.desc.Count > 1)
            // //             {
            // //                 List<string> copiedList = new List<string>();
            // //                 copiedList.AddRange(guide.desc);
            // //                 copiedList.RemoveAt(0);
            // //                 desc = string.Format(tblanguage.Get(guide.desc[0]).current, copiedList);
            // //             }
            // //
            // //             KTxt_Des.GetTextMeshPro().SetTMPText(desc);
            // //         });
            // //         Log.Debug($"desc {desc}");
            // //
            // //         if (guide.buttonId == 31101)
            // //         {
            // //             if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out var uimain))
            // //             {
            // //                 //var uimains = uimain as UIPanel_Main;
            // //                 var itemUI = uimain.GetFromReference(UIPanel_Main.KBtn_Start);
            // //                 //Log.Debug($"SetTipPosAndResize");
            // //                 //JiYuUIHelper.SetTipPosAndResize(itemUI, ui);
            // //                 //ui.GetRectTransform().SetAnchoredPositionY(-708f);
            // //                 var itemWidth2 = itemUI.GetRectTransform().Width();
            // //                 var itemHeight2 = itemUI.GetRectTransform().Height();
            // //                 var rectx = guide.pos[0] / 10000f - 0.5f;
            // //                 var recty = guide.pos[1] / 10000f - 0.5f;
            // //
            // //                 var offsetX = rectx * itemWidth2 + guide.pos[2];
            // //                 var offsetY = recty * itemHeight2 + guide.pos[3];
            // //                 var pos = JiYuUIHelper.GetUIPos(itemUI);
            // //                 pos.x += offsetX;
            // //                 pos.y += offsetY;
            // //                 Log.Debug($"SetTipPosAndResize pos{pos} {itemHeight2}");
            // //                 ui.GetRectTransform().SetAnchoredPosition(pos);
            // //             }
            // //         }
            // //         else if (guide.buttonId == 99900)
            // //         {
            // //             var itemWidth = 200f;
            // //             var itemHeight = 400f;
            // //             var rectx = guide.pos[0] / 10000f - 0.5f;
            // //             var recty = guide.pos[1] / 10000f - 0.5f;
            // //
            // //             var offsetX = rectx * itemWidth + guide.pos[2];
            // //             var offsetY = recty * itemHeight + guide.pos[3];
            // //             var pos = Vector2.zero;
            // //             pos.x += offsetX;
            // //             pos.y += offsetY;
            // //             ui.GetRectTransform().SetAnchoredPosition(pos);
            // //
            // //             if (guide.closeType == 5)
            // //             {
            // //                 await UniTask.Delay(guide.closeTypePara[0]);
            // //             }
            // //             else
            // //             {
            // //                 await UniTask.Delay(3000);
            // //             }
            // //
            // //             //JiYuUIHelper.FinishGuide(guide.id);
            // //             Close();
            // //         }
            // //         else
            // //         {
            // //             Close();
            // //         }
            // //
            // //         // if (guide.id == 3)
            // //         // {
            // //         //     //KTxt_Des.GetTextMeshPro().SetTMPText("碰撞会受伤nokey");
            // //         //
            // //         //     await UniTask.Delay(3000);
            // //         //     JiYuUIHelper.FinishGuide(guide.id);
            // //         //     Close();
            // //         // }
            // //         // else if (guide.id == 4)
            // //         // {
            // //         //     //KTxt_Des.GetTextMeshPro().SetTMPText("将怪物击退到某些障碍物上可产生碰撞，产生多次伤害！nokey");
            // //         //     await UniTask.Delay(3000);
            // //         //     JiYuUIHelper.FinishGuide(guide.id);
            // //         //     Close();
            // //         // }
            // //         // // else if (guide.id == 5)
            // //         // // {
            // //         // //     KTxt_Des.GetTextMeshPro().SetTMPText("将怪物击退到某些障碍物上可产生碰撞，产生多次伤害！nokey");
            // //         // //     await UniTask.Delay(3000);
            // //         // //     JiYuUIHelper.FinishGuide(guide.id);
            // //         // //     Close();
            // //         // // }    
            // //         // else if (guide.id == 6)
            // //         // {
            // //         //     //KTxt_Des.GetTextMeshPro().SetTMPText("这是增加玩家经验的道具nokey");
            // //         //     await UniTask.Delay(3000);
            // //         //     JiYuUIHelper.FinishGuide(guide.id);
            // //         //     Close();
            // //         // }
            // //         // else if (guide.id == 7)
            // //         // {
            // //         //     //KTxt_Des.GetTextMeshPro().SetTMPText("气泡-道具b nokey");
            // //         //     await UniTask.Delay(3000);
            // //         //     JiYuUIHelper.FinishGuide(guide.id);
            // //         //     Close();
            // //         // }
            // //         // else
            // //         // {
            // //         //   
            // //         // }
            // //
            // //
            // //         break;
            // //
            // //     case 2:
            // //         KImg_Tips_2.SetActive(true);
            // //         KImg_Bg.SetActive(true);
            // //
            // //         string desc2 = default;
            // //         if (guide.desc.Count == 1)
            // //         {
            // //             desc2 = tblanguage.Get(guide.desc[0]).current;
            // //         }
            // //         else if (guide.desc.Count > 1)
            // //         {
            // //             List<string> copiedList = new List<string>();
            // //             copiedList.AddRange(guide.desc);
            // //             copiedList.RemoveAt(0);
            // //             desc2 = string.Format(tblanguage.Get(guide.desc[0]).current, copiedList);
            // //         }
            // //
            // //         KText_Tips_2.GetTextMeshPro().SetTMPText(desc2);
            // //
            // //         if (guide.buttonId != 0)
            // //         {
            // //             if (guide.buttonId > 99900)
            // //             {
            // //                 var tagId = guide.buttonId - 99900;
            // //                 if (JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var uijiyu))
            // //                 {
            // //                     var KOptions = uijiyu.GetFromReference(UIPanel_JiyuGame.KOptions);
            // //                     var KOptionslist = KOptions.GetList();
            // //                     foreach (var child0 in KOptionslist.Children)
            // //                     {
            // //                         var childs = child0 as UISubPanel_ToggleItem;
            // //                         if (childs.tagId == tagId)
            // //                         {
            // //                             if (!ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(tagId))
            // //                             {
            // //                                 Log.Error($"未解锁tagId:{tagId}");
            // //                                 guideId = 0;
            // //                                 Close();
            // //                                 return;
            // //                             }
            // //
            // //                             JiYuUIHelper.SetForceGuideRectUI(childs, KImg_Bg);
            // //                             var pos = JiYuUIHelper.GetUIPos(childs);
            // //                             //pos.y += childs.GetRectTransform().Height();
            // //                             KImg_Tips_2.GetRectTransform().SetAnchoredPosition(pos);
            // //                             KImg_GuideAnim_2.GetRectTransform()
            // //                                 .SetWidth(childs.GetRectTransform().Width() + BoarderLength);
            // //                             KImg_GuideAnim_2.GetRectTransform()
            // //                                 .SetHeight(childs.GetRectTransform().Height() + BoarderLength);
            // //                             KImg_GuideAnim_4.GetRectTransform()
            // //                                 .SetWidth(childs.GetRectTransform().Width() + BoarderLength);
            // //                             KImg_GuideAnim_4.GetRectTransform()
            // //                                 .SetHeight(childs.GetRectTransform().Height() + BoarderLength);
            // //                             childs.GetXButton().OnClick.Add(() =>
            // //                             {
            // //                                 if (this != null)
            // //                                 {
            // //                                     this.Close();
            // //                                 }
            // //                             });
            // //                             childs.GetXButton().OnLongPressEnd.Add((a) =>
            // //                             {
            // //                                 if (this != null)
            // //                                 {
            // //                                     this.Close();
            // //                                 }
            // //                             });
            // //
            // //                             break;
            // //                         }
            // //                     }
            // //                 }
            // //             }
            // //         }
            // //         else
            // //         {
            // //             Log.Error($"guide.id {guide.id} 控件字段没有配置或未实现");
            // //             guideId = 0;
            // //             Close();
            // //         }
            // //
            // //         break;
            // //
            // //     case 3:
            // //         KImg_Tips_3.SetActive(true);
            // //         KImg_Bg.SetActive(true);
            // //         if (guide.buttonId != 0)
            // //         {
            // //             if (guide.buttonId > 99900)
            // //             {
            // //                 var tagId = guide.buttonId - 99900;
            // //                 if (JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var uijiyu))
            // //                 {
            // //                     var KOptions = uijiyu.GetFromReference(UIPanel_JiyuGame.KOptions);
            // //                     var KOptionslist = KOptions.GetList();
            // //                     foreach (var child0 in KOptionslist.Children)
            // //                     {
            // //                         var childs = child0 as UISubPanel_ToggleItem;
            // //                         if (childs.tagId == tagId)
            // //                         {
            // //                             if (!ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(tagId))
            // //                             {
            // //                                 Log.Error($"未解锁tagId:{tagId}");
            // //                                 guideId = 0;
            // //                                 Close();
            // //                                 return;
            // //                             }
            // //
            // //                             JiYuUIHelper.SetForceGuideRectUI(childs, KImg_Bg);
            // //                             var pos = JiYuUIHelper.GetUIPos(childs);
            // //                             //pos.y += childs.GetRectTransform().Height();
            // //                             KImg_Tips_3.GetRectTransform().SetAnchoredPosition(pos);
            // //                             KImg_GuideAnim_3.GetRectTransform()
            // //                                 .SetWidth(childs.GetRectTransform().Width() + BoarderLength);
            // //                             KImg_GuideAnim_3.GetRectTransform()
            // //                                 .SetHeight(childs.GetRectTransform().Height() + BoarderLength);
            // //                             KImg_GuideAnim_1.GetRectTransform()
            // //                                 .SetWidth(childs.GetRectTransform().Width() + BoarderLength);
            // //                             KImg_GuideAnim_1.GetRectTransform()
            // //                                 .SetHeight(childs.GetRectTransform().Height() + BoarderLength);
            // //                             childs.GetXButton().OnClick.Add(() =>
            // //                             {
            // //                                 if (this != null)
            // //                                 {
            // //                                     this.Close();
            // //                                 }
            // //                             });
            // //                             childs.GetXButton().OnLongPressEnd.Add((a) =>
            // //                             {
            // //                                 if (this != null)
            // //                                 {
            // //                                     this.Close();
            // //                                 }
            // //                             });
            // //
            // //                             break;
            // //                         }
            // //                     }
            // //                 }
            // //             }
            // //         }
            // //         else
            // //         {
            // //             Log.Error($"guide.id {guide.id} 控件字段没有配置或未实现");
            // //             guideId = 0;
            // //             Close();
            // //         }
            // //
            // //         break;
            // //
            // //     case 4:
            // //         KImg_Tips_4.SetActive(true);
            // //         KImg_noForceBg.SetActive(true);
            // //
            // //         if (guide.id == 2)
            // //         {
            // //             JiYuUIHelper.StartStopTime(false);
            // //             //JiYuUIHelper.EnableTriggerSystem(false);
            // //             //UnityHelper.StopTime();
            // //             KText_Tips_4.GetTextMeshPro().SetTMPText(tblanguage.Get("battle_move_text").current);
            // //
            // //             var uiBtn = KImg_noForceBg.GetXButton();
            // //             if (uiBtn == null)
            // //             {
            // //                 Log.Error($"{this.Name}.GetXButton() is null");
            // //                 return;
            // //             }
            // //
            // //             uiBtn.SetPointerActive(true);
            // //
            // //             uiBtn.SetLongPressInterval(0.5f);
            // //
            // //             uiBtn.SetMaxLongPressCount(JiYuTweenHelper.MaxLongPressCount);
            // //
            // //             uiBtn.OnLongPressEnd.Add((a) =>
            // //             {
            // //                 JiYuUIHelper.StartStopTime(true);
            // //                 //JiYuUIHelper.EnableTriggerSystem(true);
            // //                 //UnityHelper.BeginTime();
            // //                 Close();
            // //             });
            // //
            // //             if (guide.buttonId == 99900)
            // //             {
            // //                 var itemWidth = 200f;
            // //                 var itemHeight = 400f;
            // //                 var rectx = guide.pos[0] / 10000f - 0.5f;
            // //                 var recty = guide.pos[1] / 10000f - 0.5f;
            // //                 var offsetX = rectx * itemWidth + guide.pos[2];
            // //                 var offsetY = recty * itemHeight + guide.pos[3];
            // //                 var pos = Vector2.zero;
            // //                 pos.x += offsetX;
            // //                 pos.y += offsetY;
            // //                 KImg_Tips_4.GetRectTransform().SetAnchoredPosition(pos);
            // //             }
            // //
            // //
            // //             // await UniTask.Delay(1000);
            // //             //
            // //             // while (playerQuery.IsEmpty)
            // //             // {
            // //             //     await UniTask.Delay(200);
            // //             // }
            // //             //
            // //             // var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            // //             // var chaStats = entityManager.GetComponentData<ChaStats>(player);
            // //             //
            // //             // while (chaStats.chaResource.curMoveSpeed < 1)
            // //             // {
            // //             //     await UniTask.Delay(1000);
            // //             //     chaStats = entityManager.GetComponentData<ChaStats>(player);
            // //             // }
            // //             //
            // //             // //JiYuUIHelper.FinishGuide(guide.id);
            // //             //
            // //             // Close();
            // //         }
            // //         else
            // //         {
            // //             Log.Error($"guide.id {guide.id} 控件字段没有配置或未实现");
            // //             guideId = 0;
            // //             Close();
            // //         }
            // //
            // //         break;
            // //     case 5:
            // //
            // //         break;
            // //
            // //     case 6:
            // //         KImg_Tips_6.SetActive(true);
            // //         KBtn_Bg.SetActive(true);
            // //
            // //
            // //         KBtn_Bg.GetButton().OnClick.Add(() =>
            // //         {
            // //             //JiYuUIHelper.FinishGuide(guide.id);
            // //
            // //             Close();
            // //         });
            // //
            // //
            // //         break;
            // // }
            // //
            // // if (guide.templateId != 2 && guide.templateId != 3)
            // // {
            // //     UI uiparent = default;
            // //     int curTagId = guide.targetId / 100;
            // //     switch (curTagId)
            // //     {
            // //         case 1:
            // //             if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Shop, out var ui1))
            // //             {
            // //                 uiparent = ui1;
            // //             }
            // //
            // //             break;
            // //         case 2:
            // //             if (JiYuUIHelper.TryGetUI(UIType.UISubPanel_Equipment, out var ui2))
            // //             {
            // //                 uiparent = ui2;
            // //             }
            // //
            // //             break;
            // //         case 3:
            // //             if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out var ui3))
            // //             {
            // //                 uiparent = ui3;
            // //             }
            // //
            // //             break;
            // //         case 4:
            // //             if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Challege, out var ui4))
            // //             {
            // //                 uiparent = ui4;
            // //             }
            // //
            // //             break;
            // //         case 5:
            // //             if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Person, out var ui5))
            // //             {
            // //                 uiparent = ui5;
            // //             }
            // //
            // //             break;
            // //     }
            // //
            // //     if (uiparent != null)
            // //     {
            // //         if (uiparent.GameObject.activeSelf)
            // //         {
            // //             this.GetRectTransform()?.SetParent(uiparent?.GameObject?.transform, true);
            // //         }
            // //     }
            // // }
            //
            Refresh().Forget();
        }

        private async UniTask ScaleRefresh(UI ui, List<UIAnimationTools.AnimationScale> list)
        {
            if (!ui.GameObject.activeSelf)
            {
                return;
            }

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                item.duration = Mathf.Max(item.duration, 0.02f);
                await UniTask.Delay((int)(item.startTime * 1000f), true);
                ui?.GetRectTransform()?.DoScale(new Vector3(item.offset0, item.offset0, 0),
                    new Vector3(item.offset1, item.offset1, 0),
                    item.duration);
                //
                if (i == list.Count - 1)
                {
                    //ui.GetRectTransform().DoAnchoredPosition(item.offset0, item.offset1, item.duration);
                    await UniTask.Delay((int)(item.duration * 1000f), true);
                }
            }
        }

        private async UniTask AlphaRefresh(UI ui, List<UIAnimationTools.AnimationAlpha> list)
        {
            if (!ui.GameObject.activeSelf)
            {
                return;
            }

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                item.duration = Mathf.Max(item.duration, 0.02f);
                await UniTask.Delay((int)(item.startTime * 1000f), true);
                ui?.GetImage()?.DoFade(item.offset0, item.offset1, item.duration);
                //
                if (i == list.Count - 1)
                {
                    //ui.GetRectTransform().DoAnchoredPosition(item.offset0, item.offset1, item.duration);
                    await UniTask.Delay((int)(item.duration * 1000f), true);
                }
            }
        }

        private async UniTask TranRefresh(UI ui, List<UIAnimationTools.AnimationTran> list)
        {
            if (!ui.GameObject.activeSelf)
            {
                return;
            }

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                item.duration = Mathf.Max(item.duration, 0.02f);
                await UniTask.Delay((int)(item.startTime * 1000f), true);
                ui?.GetRectTransform()?.DoAnchoredPosition(item.offset0, item.offset1, item.duration);
                //
                if (i == list.Count - 1)
                {
                    //ui.GetRectTransform().DoAnchoredPosition(item.offset0, item.offset1, item.duration);
                    await UniTask.Delay((int)(item.duration * 1000f), true);
                }
            }
        }

        public void ReSet()
        {
            for (int i = 1; i < 13; i++)
            {
                var index = i;
                var str = $"Img_GuideAnim_{index}";
                var img = this.GetFromReference(str);
                var tools = img?.GetComponent<UIAnimationTools>();
                if (tools == null)
                {
                    continue;
                }

                for (int j = 0; j < tools.animationScales.Count; j++)
                {
                    var item = tools.animationScales[j];
                    if (j == 0)
                    {
                        img.GetRectTransform().SetScale(new Vector2(item.offset0, item.offset0));
                        break;
                    }
                }

                for (int j = 0; j < tools.animationAlphas.Count; j++)
                {
                    var item = tools.animationAlphas[j];
                    if (j == 0)
                    {
                        img.GetImage().SetAlpha(item.offset0);
                        break;
                    }
                }

                for (int j = 0; j < tools.animationTrans.Count; j++)
                {
                    var item = tools.animationTrans[j];
                    if (j == 0)
                    {
                        img.GetRectTransform().SetAnchoredPosition(item.offset0);
                        break;
                    }
                }
            }

            //stop = true;
        }

        struct AnimToolsStuct
        {
            public UI ui;

            public UIAnimationTools tools;
            //public int type;
        }

        bool IsAllAnimDisable()
        {
            bool allDisable = true;
            for (int i = 1; i < 13; i++)
            {
                var index = i;
                var str = $"Img_GuideAnim_{index}";
                var img = this.GetFromReference(str);
                var tools = img.GetComponent<UIAnimationTools>();
                if (tools == null || !img.GameObject.activeSelf)
                {
                    continue;
                }

                allDisable = false;
            }

            return allDisable;
        }

        public async UniTaskVoid Refresh()
        {
            if (stop)
            {
                return;
            }

            //stop = false;
            var tasks = new List<UniTask>();
            var animToolsStuct = new List<AnimToolsStuct>();

            for (int i = 1; i < 14; i++)
            {
                var index = i;
                var str = $"Img_GuideAnim_{index}";
                var img = this.GetFromReference(str);
                var tools = img?.GetComponent<UIAnimationTools>();
                if (tools == null || !img.GameObject.activeSelf)
                {
                    continue;
                }


                animToolsStuct.Add(new AnimToolsStuct
                {
                    ui = img,
                    tools = tools,
                });
                // var task1 = ScaleRefresh(img, tools.animationScales);
                // var task2 = AlphaRefresh(img, tools.animationAlphas);
                // var task3 = TranRefresh(img, tools.animationTrans);

                // tasks.Add(task1);
                // tasks.Add(task2);
                // tasks.Add(task3);
            }

            // foreach (var VARIABLE in animToolsStuct)
            // {
            //     
            // }
            while (!stop && !IsAllAnimDisable())
            {
                foreach (var VARIABLE in animToolsStuct)
                {
                    var task1 = ScaleRefresh(VARIABLE.ui, VARIABLE.tools.animationScales);
                    var task2 = AlphaRefresh(VARIABLE.ui, VARIABLE.tools.animationAlphas);
                    var task3 = TranRefresh(VARIABLE.ui, VARIABLE.tools.animationTrans);

                    tasks.Add(task1);
                    tasks.Add(task2);
                    tasks.Add(task3);
                }

                await UniTask.WhenAll(tasks);
                tasks.Clear();
                ReSet();
            }
        }

        protected override void OnClose()
        {
            stop = true;
            // JiYuUIHelper.FinishGuide(guideId);
            if (subId == 0)
            {
                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out UI ui))
                {
                    var uis = ui as UIPanel_RunTimeHUD;
                    uis.OnGuideOrderFinished(guideId);
                }
            }

            // else if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out UI ui1))
            // {
            //     var uis = ui1 as UIPanel_Main;
            //     uis.OnGuideIdFinished(guideId, subId);
            // }

            base.OnClose();
        }
    }
}