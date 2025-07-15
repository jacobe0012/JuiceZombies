//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Google.Protobuf;
using HotFix_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static XFramework.UIPanel_Equipment;

namespace XFramework
{
	[UIEvent(UIType.UIPanel_CompoundNormalDongHua)]
    internal sealed class UIPanel_CompoundNormalDongHuaEvent : AUIEvent, IUILayer
    {
	    public override string Key => UIPathSet.UIPanel_CompoundNormalDongHua;

        public override bool IsFromPool => true;
		
		public override bool AllowManagement => true;
		
		public UILayer Layer => UILayer.Mid;
		
        public override UI OnCreate()
        {
            return UI.Create<UIPanel_CompoundNormalDongHua>();
        }
    }

    public partial class UIPanel_CompoundNormalDongHua : UI, IAwake<List<MyGameEquip>>
	{

        private CancellationTokenSource cts;
		
		protected override void OnClose()
		{
			base.OnClose();
		}

        public async void Initialize(List<MyGameEquip> equips)
        {

            cts = new CancellationTokenSource();
            InitBtn();
            EffectInit(equips).Forget();
        }

        private async UniTask EffectInit(List<MyGameEquip> equips)
        {
            CreateEffect(equips);
            await SetEffect(equips);
            SetSuccess(this, equips).Forget();
        }

        private void CreateEffect(List<MyGameEquip> list)
        {
            if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_Compound, out UI ui1))
            {
                return;
            }
            var uiCompound = ui1 as UIPanel_Compound;
            var myGameEquip0 = uiCompound.equipMain;


            if (list.Count == 2)
            {
                var two = this.GetFromReference(UIPanel_CompoundNormalDongHua.KEquipPosTwo);
                two.GetList().Clear();
                for (int i = 0; i < list.Count; i++)
                {
                    var equipData = list[i];
                    if (i == 0)
                    {
                        equipData = myGameEquip0;
                    }
                    var go = two.GetRectTransform().GetChild(i).gameObject;
                    var item = two.GetList().Create(go, true);
                    JiYuUIHelper.SetEquipIcon(equipData, item, EquipPanelType.Compose);
                }

               
            }
            else
            {
                var three = this.GetFromReference(UIPanel_CompoundNormalDongHua.KEquipPosThree);
                three.GetList().Clear();
                for (int i = 0; i < list.Count; i++)
                {
                    var equipData = list[i];
                    if (i == 0)
                    {
                        equipData = myGameEquip0;
                    }
                    var go = three.GetRectTransform().GetChild(i).gameObject;
                    var item = three.GetList().Create(go, true);
                    JiYuUIHelper.SetEquipIcon(equipData, item, EquipPanelType.Compose);
                }
            }
        }

        private void InitBtn()
        {

            GetFromReference(KBg_Mask).GetButton().OnClick.Add(() =>CancelCts());
            
        }

        private void CancelCts()
        {
            Log.Debug($"CancelCts",Color.cyan);
            cts.Cancel();
            cts= new CancellationTokenSource();
        }

        private async UniTask<AsyncUnit> SetEffect(List<MyGameEquip> list)
        {
            var two = this.GetFromReference(UIPanel_CompoundNormalDongHua.KEquipPosTwo);
            var three = this.GetFromReference(UIPanel_CompoundNormalDongHua.KEquipPosThree);
            two.SetActive(false);
            three.SetActive(false);

            if (list.Count == 2)
            {
                two.SetActive(true);

                for (int i = 0; i < two.GetList().Children.Count; i++)
                {

                    var item=two.GetList().Children[i] as UICommon_RewardItem;
                    Effect1(i, item).Forget();
                }

                return AsyncUnit.Default;
            }
            else
            {
                three.SetActive(true);
                var childrens = three.GetList().Children;
                for (int i = 0; i < childrens.Count; i++)
                {

                    var item = childrens[i];
                    Effect2(i, item).Forget();
                }
            }

            //await UniTask.Delay(1000, cancellationToken: cts.Token);
            return AsyncUnit.Default;
        }

        private async UniTask<AsyncUnit> Effect2(int i, UI item)
        {
            try
            {
                await UniTask.Delay(1, cancellationToken: cts.Token);
                if (i == 0)
                {
                    //item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>().alpha = 1;
                    item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<Transform>()
                        .DOScale(1.5f, 1.8f).SetEase(Ease.InQuad);
                    item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<RectTransform>()
                        .DOAnchorPosY(372f, 1.8f).SetEase(Ease.InQuad);
                }
                else
                {
                    item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>().alpha =
                        1;
                    item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>()
                        .DOFade(0, 1.8f).SetEase(Ease.InQuad);
                    StartShake(item, item.GetRectTransform().AnchoredPosition3D(),
                        item.GetRectTransform().Rotation());
                }

                return AsyncUnit.Default;
            }
            catch (OperationCanceledException)
            {
                item.GetComponent<RectTransform>()?.DOComplete();
                item.GetComponent<CanvasGroup>()?.DOComplete();
                Log.Debug("Animation cancelled", Color.yellow);
                return AsyncUnit.Default;
            }
        }

        private async UniTask<AsyncUnit> Effect1(int i, UI item)
        {
            try
            {
                await UniTask.Delay(1, cancellationToken: cts.Token);
                if (i == 0)
                {
                    //item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>().alpha = 1;
                    item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<RectTransform>()
                        .DOScale(1.5f, 1.8f).SetEase(Ease.InQuad);
                    item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<RectTransform>()
                        .DOAnchorPosY(372f, 1.8f).SetEase(Ease.InQuad);
                }
                else
                {
                    item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>().alpha =
                        1;
                    item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>()
                        .DOFade(0, 1.8f).SetEase(Ease.InQuad);
                    StartShake(item, item.GetRectTransform().AnchoredPosition3D(),
                        item.GetRectTransform().Rotation());
                }

                return AsyncUnit.Default;
            }
            catch (OperationCanceledException)
            {
                item.GetComponent<RectTransform>()?.DOComplete();
                item.GetComponent<CanvasGroup>()?.DOComplete();
                Log.Debug("Animation cancelled", Color.yellow);
                return AsyncUnit.Default;
            }
        }
        public void StartShake(UI ui, Vector3 originalPosition, Quaternion originalRotation,
          float rotationStrength = 8f, float shakeDuration = 2f, float shakeStrength = 18f, int vibrato = 8,
          float randomness = 45f)
        {
            var uiElement = ui.GetComponent<RectTransform>();
            // 停止之前的动画（避免叠加）
            // 创建一个 Sequence 来同时执行位置和旋转抖动
            Sequence shakeSequence = DOTween.Sequence();

            // 添加位置抖动（左右）
            shakeSequence.Join(uiElement.DOShakePosition(
                duration: shakeDuration,
                strength: new Vector3(shakeStrength, 0, 0), // 只在 X 轴抖动
                vibrato: vibrato,
                randomness: randomness,
                snapping: false,
                fadeOut: true
            ));

            // 添加旋转抖动（正负 m 度，围绕 Z 轴）
            shakeSequence.Join(uiElement.DOShakeRotation(
                duration: shakeDuration,
                strength: new Vector3(0, 0, rotationStrength), // 只围绕 Z 轴旋转
                vibrato: vibrato,
                randomness: randomness,
                fadeOut: true
            ));


            // 确保动画结束时恢复原始位置和旋转
            shakeSequence.OnComplete(() =>
            {
                uiElement.anchoredPosition3D = originalPosition;
                uiElement.rotation = originalRotation;
            });
        }
        private async UniTaskVoid SetSuccess(UIPanel_CompoundNormalDongHua ui, List<MyGameEquip> list)
        {


            ByteValueList valueList = new ByteValueList();
            foreach (var item in list)
            {
                valueList.Values.Add(item.equip.ToByteString());
            }
            if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_Compound, out UI ui1))
            {
                return;
            }
            var uiCompound = ui1 as UIPanel_Compound;
            var myGameEquip0 = uiCompound.equipMain;

            NetWorkManager.Instance.SendMessage(CMD.EQUIPCOMPOSE, valueList);
            NetWorkManager.Instance.SendMessage(CMD.QUERYEQUIP);
            var tween = ui.GetFromReference(UIPanel_CompoundNormalDongHua.KLightEffect).GetRectTransform()
                .DoScale(new Vector3(3, 3, 3), 2f);
            tween.AddOnCompleted(async () =>
            {
                ui.Dispose();
                var data = new CompoundSucData
                {
                    isAllCompound = false,
                    Equips = list,
                    firstEquips = myGameEquip0
                };

                UIHelper.CreateAsync(UIType.UIPanel_CompoundSuc, data);

                await UniTask.Delay(1000,cancellationToken:cts.Token);
                uiCompound.RefreshCompoundPanel();
            });
        }
    }
}
