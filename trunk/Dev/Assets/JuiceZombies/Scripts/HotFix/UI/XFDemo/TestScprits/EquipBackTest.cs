// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.UI;
// using XFramework;
// using Spine.Unity;
// using Spine;
// using System;
// using HotFix_UI;
// using JetBrains.Annotations;
//
// public class EquipBackTest : UIBase
// {
//     public List<GameObject> posList = new List<GameObject>();
//
//     //攻击力
//     public XTextMeshProUGUI AtkText;
//
//     //血量
//     public XTextMeshProUGUI HpText;
//
//     //攻击力界面按钮
//     public Button AtkButton;
//
//     //血量界面按钮
//     public Button HpButton;
//
//     //合成面板那妞
//     public Button ComplexButton;
//
//     //排序面板按钮
//     public Button SortButton;
//
//     public SkeletonGraphic palyerAnimaition;
//
//     public float testDesTime = 0.5f;
//
//     // Start is called before the first frame update
//     void Start()
//     {
//         Init();
//
//
//         for (int i = 0; i < EquipitemCache.isWearUID.Count; i++)
//         {
//             if (EquipitemCache.isWearUID[i].PosId.Equals(1))
//             {
//                 palyerAnimaition.Skeleton.SetAttachment("Weapon", EquipitemCache.isWearUID[i].EquipId.ToString());
//                 palyerAnimaition.AnimationState.SetAnimation(0,
//                         "equip_" + EquipitemCache.isWearUID[i].EquipId.ToString() + "_spine", false).Complete +=
//                     OnAnimationComplete;
//                 break;
//             }
//         }
//
//         palyerAnimaition.Skeleton.SetAttachment("Weapon", null);
//         palyerAnimaition.AnimationState.SetAnimation(0, "Player_Stand", true);
//     }
//
//     private void InitPlayerInfo()
//     {
//         NetWorkManager.Instance.SendMessage(2, 7);
//         AtkText.text = ResourcesSingletonOld.Instance.playerProperty.playerData.defaultProperty.atk.ToString();
//         HpText.text = ResourcesSingletonOld.Instance.playerProperty.playerData.defaultProperty.maxHp.ToString();
//     }
//
//     public void OnEnable()
//     {
//         InitPlayerInfo();
//         // UIEquipLevelUp.propertyRefresh += InitPlayerInfo;
//         // UIEquipLevelUp.EquipEvent += equipEventAnimation;
//     }
//
//     public void OnDisable()
//     {
//         // UIEquipLevelUp.propertyRefresh -= InitPlayerInfo;
//         // UIEquipLevelUp.EquipEvent -= equipEventAnimation;
//     }
//
//     public void Update()
//     {
//         if (testDesTime == 0)
//         {
//             return;
//         }
//
//         if (testDesTime >= 0)
//         {
//             testDesTime -= Time.deltaTime;
//         }
//         else if (testDesTime != 0)
//         {
//             for (int i = 0; i < EquipitemCache.isWearUID.Count; i++)
//             {
//                 if (EquipitemCache.isWearUID[i].PosId.Equals(1))
//                 {
//                     palyerAnimaition.Skeleton.SetAttachment("Weapon", EquipitemCache.isWearUID[i].EquipId.ToString());
//                     palyerAnimaition.AnimationState.SetAnimation(0,
//                             "equip_" + EquipitemCache.isWearUID[i].EquipId.ToString() + "_spine", false).Complete +=
//                         OnAnimationComplete;
//                     break;
//                 }
//             }
//
//             testDesTime = 0;
//         }
//     }
//
//
//     private void Init()
//     {
//         //合成装备按钮
//         ComplexButton?.onClick.AddListener(async () =>
//         {
//             if (Time.time - lastClickTime >= cooldownTime)
//             {
//                 EquipItemBtnTest.EquipmentPanel = false;
//                 lastClickTime = Time.time;
//                 //动画效果
//                 UnicornTweenHelper.GradualChange(ComplexButton.gameObject, 1.2f, 0.12f, 0.12f);
//
//                 Invoke("OpenCompoundPanelAsync", 0.1f);
//             }
//         });
//
//         //排序按钮
//         SortButton?.onClick.AddListener(async () =>
//         {
//             if (Time.time - lastClickTime >= cooldownTime)
//             {
//                 lastClickTime = Time.time;
//                 //动画效果
//                 UnicornTweenHelper.GradualChange(SortButton.gameObject, 1.2f, 0.12f, 0.12f);
//                 //测试通用面板
//                 //await UIHelper.CreateAsync(UIType.UIGeneralMaterial, UILayer.Mid);
//                 //UIHelper.Create(UIType.UIRapidCompound, UILayer.Mid);
//                 //List<Vector3> reward =new List<Vector3>();
//                 //reward.Add(new Vector3(3, 0, 1));
//                 //reward.Add(new Vector3(4, 0, 1));
//                 //reward.Add(new Vector3(5, 102001, 2));
//                 //reward.Add(new Vector3(5, 102002, 2));
//                 //reward.Add(new Vector3(5, 102003, 2));
//                 //reward.Add(new Vector3(5, 102004, 2));
//                 //reward.Add(new Vector3(5, 102005, 2));
//                 //await UIHelper.CreateAsync(UIType.UICommon_Reward, reward, UILayer.High);
//
//
//                 // UI ui = await UIHelper.CreateAsync(UIType.UISubPanel_Equipment, UILayer.High);
//                 // ui.GetComponent<RectTransform>().SetAnchoredPosition(Vector2.zero);
//             }
//         });
//
//
//         AtkButton?.onClick.AddListener(() =>
//         {
//             if (Time.time - lastClickTime >= cooldownTime)
//             {
//                 lastClickTime = Time.time;
//                 //动画效果
//                 UnicornTweenHelper.GradualChange(AtkButton.gameObject, 1.2f, 0.12f, 0.12f);
//
//                 Invoke("OpenPropertyPanel", 0.1f);
//             }
//         });
//
//
//         HpButton?.onClick.AddListener(() =>
//         {
//             if (Time.time - lastClickTime >= cooldownTime)
//             {
//                 lastClickTime = Time.time;
//                 //动画效果
//                 UnicornTweenHelper.GradualChange(HpButton.gameObject, 1.2f, 0.12f, 0.12f);
//
//                 Invoke("OpenPropertyPanel", 0.1f);
//             }
//         });
//     }
//
//     private async Task OpenCompoundPanelAsync()
//     {
//         await UIHelper.CreateAsync(UIType.UICompound, UILayer.Mid);
//     }
//
//     /// <summary>
//     /// 打开属性面板
//     /// </summary>
//     /// <returns></returns>
//     private async Task OpenPropertyPanel()
//     {
//         await UIHelper.CreateAsync(UIType.UIPlayerProperty, UILayer.Mid);
//     }
//
//     public void equipEventAnimation(int equipid)
//     {
//         Debug.Log(equipid / 100 + "equipid/100");
//         if (!equipid.Equals(0) && (equipid / 100).Equals(1))
//         {
//             palyerAnimaition.Skeleton.SetAttachment("Weapon", equipid.ToString());
//             palyerAnimaition.AnimationState.SetAnimation(0, "equip_" + equipid.ToString() + "_spine", false).Complete +=
//                 OnAnimationComplete;
//             //保存一下装备类型,以便衣服也用这个动画
//             EquipitemCache.isFinshedEquipid = equipid;
//         }
//         else if (!equipid.Equals(0) && (!EquipitemCache.isFinshedEquipid.Equals(0))) //衣服动画
//         {
//             palyerAnimaition.Skeleton.SetAttachment("Weapon", EquipitemCache.isFinshedEquipid.ToString());
//             palyerAnimaition.AnimationState
//                     .SetAnimation(0, "equip_" + EquipitemCache.isFinshedEquipid.ToString() + "_spine", false)
//                     .Complete +=
//                 OnAnimationComplete;
//         }
//         else
//         {
//             palyerAnimaition.Skeleton.SetAttachment("Weapon", null);
//             palyerAnimaition.AnimationState.SetAnimation(0, "Player_Stand", true);
//         }
//     }
//
//
//     private void OnAnimationComplete(TrackEntry trackEntry)
//     {
//         palyerAnimaition.AnimationState.SetAnimation(0, "Player_Stand", true);
//     }
// }

