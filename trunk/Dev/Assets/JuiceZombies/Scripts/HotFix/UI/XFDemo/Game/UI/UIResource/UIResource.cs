//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using Unity.Mathematics;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UIResource)]
    internal sealed class UIResourceEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIResource;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.High;

        public override UI OnCreate()
        {
            return UI.Create<UIResource>();
        }
    }

    public struct ResourcePanelData
    {
        public int id;
        public int count;
    }

    public partial class UIResource : UI, IAwake<List<Vector3>>
    {
        public void Initialize(List<Vector3> rewards)
        {
            //ResourceAni(rewards);
        }

        // async void ResourceAni(List<Vector3> rewards)
        // {
        //     int reward2 = rewards.Where(a => (int)a.x == 2).Count();
        //     int reward3 = rewards.Where(a => (int)a.x == 3).Count();
        //     int reward4 = rewards.Where(a => (int)a.x == 1).Count();
        //
        //     var rewardx = (int)reward.x;
        //     var rewardy = (int)reward.y;
        //     var rewardz = (int)reward.z;
        //     if (rewardz < 0)
        //     {
        //         return;
        //     }
        //
        //     const float TurnBiggerTime = 0.3f;
        //     const float StayTime = 0.2f;
        //     const float FlyTime = 0.4f;
        //     const float BiggerScaleMin = 2f;
        //     const float BiggerScaleMax = 3f;
        //
        //
        //     Vector3 defalutflyPos = default;
        //
        //     var uiManager = Common.Instance.Get<UIManager>();
        //     var global = Common.Instance.Get<Global>();
        //     if (!uiManager.TryGet(UIType.UIPanel_Main, out var ui0))
        //     {
        //         Log.Debug($"UIPanel_JiyuGame", Color.green);
        //         return;
        //     }
        //
        //
        //     var uiJiyuGame = ui0 as UIPanel_Main;
        //     //UI bagUI = null;
        //
        //     //var KImg_Bag = GetFromReference(UIResource.KImg_Bag);
        //     //KImg_Bag.SetActive(false);
        //     Vector3 bagPos = default;
        //     if (rewardx == 5 || rewardx == 11)
        //     {
        //         bagPos = ResourcesSingleton.Instance.UIPosInfo.KBagPos;
        //         if (JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var ui))
        //         {
        //             var uis = ui as UIPanel_JiyuGame;
        //             var KOptions = uis.GetFromReference(UIPanel_JiyuGame.KOptions);
        //             foreach (var child in KOptions.GetList().Children)
        //             {
        //                 var childs = child as UISubPanel_ToggleItem;
        //                 if (childs.sort == 2)
        //                 {
        //                     bagPos = JiYuUIHelper.GetUIPos(childs);
        //
        //                     break;
        //                 }
        //             }
        //         }
        //     }
        //
        //
        //     int count = rewardz;
        //     const int MaxCount = 5;
        //     switch (rewardx)
        //     {
        //         #region huangjinguo add
        //
        //         case -1:
        //
        //             count = Mathf.Min(rewardz, MaxCount);
        //
        //             if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Task_DailyAndWeekly, out UI TaskUI))
        //             {
        //                 var KImgScore = TaskUI.GetFromReference(UIPanel_Task_DailyAndWeekly.KImg_Score);
        //                 defalutflyPos = JiYuUIHelper.GetUIPos(KImgScore);
        //             }
        //             else
        //             {
        //                 count = 0;
        //                 break;
        //             }
        //
        //             break;
        //
        //         #endregion
        //
        //         case 1:
        //             count = Mathf.Min(rewardz, MaxCount);
        //
        //             //var KEnergyIcon = uiJiyuGame.GetFromReference(UIPanel_Main.KImg_EnergyIcon);
        //
        //             //defalutflyPos = JiYuUIHelper.GetUIPos(KEnergyIcon);
        //             break;
        //         case 2:
        //             count = Mathf.Min(rewardz, MaxCount);
        //
        //             defalutflyPos = ResourcesSingleton.Instance.UIPosInfo.KBtn_DiamondPos;
        //             break;
        //         case 3:
        //             count = Mathf.Min(rewardz, MaxCount);
        //
        //             defalutflyPos = ResourcesSingleton.Instance.UIPosInfo.KBtn_MoneyPos;
        //             break;
        //         case 4:
        //             count = Mathf.Min(rewardz, MaxCount);
        //
        //             var KExperienceValueImage = uiJiyuGame.GetFromReference(UIPanel_Main.KImg_FilledImgExp);
        //
        //             defalutflyPos = JiYuUIHelper.GetUIPos(KExperienceValueImage);
        //             break;
        //         case 5:
        //             count = Mathf.Min(rewardz, MaxCount);
        //
        //             defalutflyPos = bagPos;
        //
        //
        //             break;
        //         case 6:
        //             break;
        //         case 7:
        //             break;
        //         case 8:
        //             break;
        //         case 9:
        //             break;
        //         case 10:
        //             break;
        //         case 11:
        //             count = Mathf.Min(rewardz, MaxCount);
        //             defalutflyPos = bagPos;
        //
        //             break;
        //         case 12:
        //             break;
        //         case 13:
        //             break;
        //         case 14:
        //             break;
        //         case 15:
        //             break;
        //     }
        //
        //     var KRoot = GetFromReference(UIResource.KRoot);
        //     var list = KRoot.GetList();
        //     list.Clear();
        //
        //     foreach (var reward in rewards)
        //     {
        //         //int index = i;
        //         var initPos = GenerateRandomPoint(new Rectangle(), new Rectangle
        //         {
        //             center = new float2(0, 400),
        //             width = 300,
        //             height = 100
        //         });
        //
        //         var StablePos = GenerateRandomPoint(new Rectangle(), new Rectangle
        //         {
        //             center = new float2(0, 400),
        //             width = 600,
        //             height = 400
        //         });
        //         var randScale = UnityEngine.Random.Range(BiggerScaleMin, BiggerScaleMax);
        //
        //         var ui = await list.CreateWithUITypeAsync(UIType.UIResourceItem, reward, false);
        //         //var ui = UIHelper.Create(UIType.UIResourceItem, reward, root.GameObject.transform);
        //         ui.GetRectTransform().SetAnchoredPosition(initPos);
        //
        //
        //         ui.GetRectTransform().DoAnchoredPosition(new Vector3(StablePos.x, StablePos.y, 0), TurnBiggerTime)
        //             .AddOnCompleted(async () =>
        //             {
        //                 await UniTask.Delay((int)(StayTime * 1000));
        //                 ui.GetRectTransform()
        //                     .DoAnchoredPosition(defalutflyPos, FlyTime).AddOnCompleted(async () =>
        //                     {
        //                         await UniTask.Delay(500);
        //                         // if (index == count - 1)
        //                         // {
        //                         //     if (rewardx == 5 || rewardx == 11)
        //                         //     {
        //                         //         KImg_Bag?.SetActive(false);
        //                         //     }
        //                         // }
        //
        //                         Close();
        //                     });
        //             });
        //
        //         ui.GetRectTransform()
        //             .DoScale(new Vector3(randScale, randScale, randScale), TurnBiggerTime)
        //             .AddOnCompleted(async () =>
        //             {
        //                 await UniTask.Delay((int)(StayTime * 1000));
        //                 ui.GetRectTransform()
        //                     .DoScale(new Vector3(1, 1, 1), FlyTime);
        //             });
        //     }
        // }

        async void ResourceAni(Vector3 reward)
        {
            var rewardx = (int)reward.x;
            var rewardy = (int)reward.y;
            var rewardz = (int)reward.z;
            if (rewardz < 0)
            {
                return;
            }

            const float TurnBiggerTime = 0.3f;
            const float StayTime = 0.2f;
            const float FlyTime = 0.4f;
            const float BiggerScaleMin = 2f;
            const float BiggerScaleMax = 3f;


            Vector3 defalutflyPos = default;

            var uiManager = Common.Instance.Get<UIManager>();
            var global = Common.Instance.Get<Global>();
            if (!uiManager.TryGet(UIType.UIPanel_Main, out var ui0))
            {
                Log.Debug($"UIPanel_JiyuGame", Color.green);
                return;
            }


            var uiJiyuGame = ui0 as UIPanel_Main;
            //UI bagUI = null;

            //var KImg_Bag = GetFromReference(UIResource.KImg_Bag);
            //KImg_Bag.SetActive(false);
            Vector3 bagPos = default;
            if (rewardx == 5 || rewardx == 11)
            {
                bagPos = ResourcesSingleton.Instance.UIPosInfo.KBagPos;
                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var ui))
                {
                    var uis = ui as UIPanel_JiyuGame;
                    var KOptions = uis.GetFromReference(UIPanel_JiyuGame.KOptions);
                    foreach (var child in KOptions.GetList().Children)
                    {
                        var childs = child as UISubPanel_ToggleItem;
                        if (childs.sort == 2)
                        {
                            bagPos = JiYuUIHelper.GetUIPos(childs);

                            break;
                        }
                    }
                }
            }


            int count = rewardz;
            const int MaxCount = 5;
            switch (rewardx)
            {
                #region huangjinguo add

                case -1:

                    count = Mathf.Min(rewardz, MaxCount);

                    if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Task_DailyAndWeekly, out UI TaskUI))
                    {
                        var KImgScore = TaskUI.GetFromReference(UIPanel_Task_DailyAndWeekly.KImg_Score);
                        defalutflyPos = JiYuUIHelper.GetUIPos(KImgScore);
                    }
                    else
                    {
                        count = 0;
                        break;
                    }

                    break;

                #endregion

                case 1:
                    count = Mathf.Min(rewardz, MaxCount);

                    //var KEnergyIcon = uiJiyuGame.GetFromReference(UIPanel_Main.KImg_EnergyIcon);

                    //defalutflyPos = JiYuUIHelper.GetUIPos(KEnergyIcon);
                    break;
                case 2:
                    count = Mathf.Min(rewardz, MaxCount);

                    defalutflyPos = ResourcesSingleton.Instance.UIPosInfo.KBtn_DiamondPos;
                    break;
                case 3:
                    count = Mathf.Min(rewardz, MaxCount);

                    defalutflyPos = ResourcesSingleton.Instance.UIPosInfo.KBtn_MoneyPos;
                    break;
                case 4:
                    count = Mathf.Min(rewardz, MaxCount);

                    var KExperienceValueImage = uiJiyuGame.GetFromReference(UIPanel_Main.KImg_FilledImgExp);

                    defalutflyPos = JiYuUIHelper.GetUIPos(KExperienceValueImage);
                    break;
                case 5:
                    count = Mathf.Min(rewardz, MaxCount);

                    defalutflyPos = bagPos;


                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:
                    count = Mathf.Min(rewardz, MaxCount);
                    defalutflyPos = bagPos;

                    break;
                case 12:
                    break;
                case 13:
                    break;
                case 14:
                    break;
                case 15:
                    break;
            }

            var KRoot = GetFromReference(UIResource.KRoot);
            var list = KRoot.GetList();
            list.Clear();
            for (int i = 0; i < count; i++)
            {
                int index = i;
                var initPos = GenerateRandomPoint(new Rectangle(), new Rectangle
                {
                    center = new float2(0, 400),
                    width = 300,
                    height = 100
                });

                var StablePos = GenerateRandomPoint(new Rectangle(), new Rectangle
                {
                    center = new float2(0, 400),
                    width = 600,
                    height = 400
                });
                var randScale = UnityEngine.Random.Range(BiggerScaleMin, BiggerScaleMax);

                var ui = await list.CreateWithUITypeAsync(UIType.UIResourceItem, reward, false);
                //var ui = UIHelper.Create(UIType.UIResourceItem, reward, root.GameObject.transform);
                ui.GetRectTransform().SetAnchoredPosition(initPos);


                ui.GetRectTransform().DoAnchoredPosition(new Vector3(StablePos.x, StablePos.y, 0), TurnBiggerTime)
                    .AddOnCompleted(async () =>
                    {
                        await UniTask.Delay((int)(StayTime * 1000));
                        ui.GetRectTransform()
                            .DoAnchoredPosition(defalutflyPos, FlyTime).AddOnCompleted(async () =>
                            {
                                await UniTask.Delay(500);
                                // if (index == count - 1)
                                // {
                                //     if (rewardx == 5 || rewardx == 11)
                                //     {
                                //         KImg_Bag?.SetActive(false);
                                //     }
                                // }

                                Close();
                            });
                    });

                ui.GetRectTransform()
                    .DoScale(new Vector3(randScale, randScale, randScale), TurnBiggerTime)
                    .AddOnCompleted(async () =>
                    {
                        await UniTask.Delay((int)(StayTime * 1000));
                        ui.GetRectTransform()
                            .DoScale(new Vector3(1, 1, 1), FlyTime);
                    });
            }


            // if (bagUI != null)
            // {
            //     var bagUIscript = bagUI as UIResourceItem;
            //     bagUIscript.OnDestroyUI();
            // }
        }

        private static float2 GenerateRandomPoint(Rectangle smallRect, Rectangle bigRect)
        {
            float2 point = new float2(smallRect.center.x, smallRect.center.y + 120f);

            for (int i = 0; i < 999; i++)
            {
                point.x = UnityEngine.Random.Range(bigRect.center.x - bigRect.width / 2,
                    bigRect.center.x + bigRect.width / 2);
                point.y = UnityEngine.Random.Range(bigRect.center.y - bigRect.height / 2,
                    bigRect.center.y + bigRect.height / 2);


                if (!smallRect.Contains(point))
                {
                    break;
                }
            }

            return point;
        }

        public struct Rectangle
        {
            public float2 center; // 
            public float width; // 
            public float height; //

            public bool Contains(float2 point)
            {
                float minX = center.x - width / 2;
                float maxX = center.x + width / 2;
                float minY = center.y - height / 2;
                float maxY = center.y + height / 2;

                return point.x >= minX && point.x <= maxX && point.y >= minY && point.y <= maxY;
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}