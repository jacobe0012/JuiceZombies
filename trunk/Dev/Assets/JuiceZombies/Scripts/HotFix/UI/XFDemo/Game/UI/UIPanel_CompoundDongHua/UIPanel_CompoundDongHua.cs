//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Google.Protobuf;
using Google.Protobuf.Collections;
using HotFix_UI;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static XFramework.UIPanel_Equipment;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_CompoundDongHua)]
    internal sealed class UIPanel_CompoundDongHuaEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_CompoundDongHua;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Mid;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_CompoundDongHua>();
        }
    }

    public partial class UIPanel_CompoundDongHua : UI, IAwake<List<MyGameEquip>>
    {
        private Tbequip_data tbEquip_data;
        private Tblanguage tbLanguage;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private RepeatedField<EquipDto> results;
        private bool isAnimationEnd;

        public async void Initialize(List<MyGameEquip> equips)
        {
            InitJson();
            InitNode();
            SetItem(equips);
            await UniTask.Delay(1000, cancellationToken: cts.Token);
            await SetSuccess(equips.Count);
            await GenereteItem();
        }

        private async void GenerateItemNoEffect()
        {
            if (await IsGetData())
            {
                //GetFromReference(KSuccess).SetActive(true);
                GetFromReference(KNoSucess).SetActive(true);
                List<Vector3> resultsEquip = new List<Vector3>();
                for (int i = 0; i < results.Count; i++)
                {
                    int id = results[i].EquipId * 100 + results[i].Quality;
                    resultsEquip.Add(new Vector3(11, id, 1));
                }

                InitRewardItems(resultsEquip);
               
               
               

            }
        }


        private async UniTask SetSuccess(int count)
        {
            string key = "";
            if (count == 3)
            {
                key = "LightEffect3";
            }
            else if (count == 5)
            {
                key = "LightEffect5";
            }
            else
            {
                key = "LightEffect6";
            }
           
            var itme=GetFromReference(key);
            itme.SetActive(true);
            var tween = itme.GetRectTransform().DoScale(new Vector3(3, 3, 3), 2f);
            tween.AddOnCompleted(async () =>
            {
                itme.SetActive(false); 
            });
        }

        private async UniTask GenereteItem()
        {
            if (await IsGetData())
            {
                //GetFromReference(KSuccess).SetActive(true);
                GetFromReference(KNoSucess).SetActive(false);
                List<Vector3> resultsEquip = new List<Vector3>();
                for (int i = 0; i < results.Count; i++)
                {
                    int id = results[i].EquipId * 100 + results[i].Quality;
                    resultsEquip.Add(new Vector3(11, id, 1));
                }

                InitRewardItems(resultsEquip, cts.Token).Forget();
                NetWorkManager.Instance.SendMessage(CMD.QUERYEQUIP);
                await UniTask.Delay(1000, cancellationToken: cts.Token);
                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Compound, out UI ui))
                {
                    var uis = ui as UIPanel_Compound;
                    uis.RefreshCompoundPanel();
                }
            }
        }

        private void InitJson()
        {
            results = new RepeatedField<EquipDto>();
            GetFromReference(KSuccess).SetActive(false);
            GetFromReference(KNoSucess).SetActive(true);
            tbEquip_data = ConfigManager.Instance.Tables.Tbequip_data;
            tbLanguage = ConfigManager.Instance.Tables.Tblanguage;
            NetWorkManager.Instance.SendMessage(CMD.EQUIPALLCOMPOUND);
            //tbEquip_data = ConfigManager.Instance.Tables.tb;
        }

        private void SetItem(List<MyGameEquip> equips)
        {
            var KBg_Mask = GetFromReference(UIPanel_CompoundDongHua.KBg_Mask);
            GetFromReference(KSuccess).GetButton().OnClick.Add(Close);

            KBg_Mask.GetButton().OnClick.Add(() => ClosePanel());
            if (equips.Count <= 6)
            {
                GetFromReference(KEquipPosThree).SetActive(false);
                GetFromReference(KEquipPosFive).SetActive(false);
                GetFromReference(KEquipPosSix).SetActive(false);

                if (equips.Count == 3)
                {
                    GetFromReference(KEquipPosThree).SetActive(true);
                    var list3 = GetFromReference(KEquipPosThree).GetList();
                    for (var i = 0; i < 3; i++)
                    {
                        var equip = equips[i];
                        var child = GetFromReference(KEquipPosThree).GetRectTransform().GetChild(i);
                        UI item = list3.Create(child.gameObject, true);
                        JiYuUIHelper.SetEquipIcon(equip, item, EquipPanelType.Compose);
                        StartShake(item, item.GetRectTransform().AnchoredPosition3D(),
                            item.GetRectTransform().Rotation());
                    }
                }
                else if (equips.Count == 5)
                {
                    GetFromReference(KEquipPosFive).SetActive(true);
                    var list5 = GetFromReference(KEquipPosFive).GetList();
                    for (var i = 0; i < 5; i++)
                    {
                        var equip = equips[i];
                        var child = GetFromReference(KEquipPosFive).GetRectTransform().GetChild(i);
                        UI item = list5.Create(child.gameObject, true);
                        JiYuUIHelper.SetEquipIcon(equip, item, EquipPanelType.Compose);
                        StartShake(item, item.GetRectTransform().AnchoredPosition3D(),
                            item.GetRectTransform().Rotation());
                    }
                }
                else
                {
                    GetFromReference(KEquipPosSix).SetActive(true);
                    var list6 = GetFromReference(KEquipPosSix).GetList();
                    for (var i = 0; i < 6; i++)
                    {
                        var equip = equips[i];
                        var child = GetFromReference(KEquipPosSix).GetRectTransform().GetChild(i);
                        UI item = list6.Create(child.gameObject, true);
                        JiYuUIHelper.SetEquipIcon(equip, item, EquipPanelType.Compose);
                        StartShake(item, item.GetRectTransform().AnchoredPosition3D(),
                            item.GetRectTransform().Rotation());
                    }
                }
            }
            else if (equips.Count > 6)
            {
                GetFromReference(KEquipPosSix).SetActive(true);
                List<MyGameEquip> newlist = new List<MyGameEquip>(6);
                var temp = equips;
                for (var i = 1; i <= 6; i++)
                {
                    var number = UnityEngine.Random.Range(0, temp.Count);
                    newlist.Add(temp[number]);
                    temp.RemoveAt(number);
                }

                foreach (var equip in newlist)
                {
                    Log.Debug($"equipid:{equip.equip.EquipId}");
                }

                for (var i = 1; i <= 6; i++)
                {
                    var equip = newlist[i - 1];
                    var key = "item" + i.ToString();
                    JiYuUIHelper.SetEquipIcon(equip, GetFromReference(key), EquipPanelType.Compose);
                }

                for (int i = 1; i <= 6; i++)
                {
                    var key = "item" + i.ToString();
                    StartShake(GetFromReference(key), GetFromReference(key).GetRectTransform().AnchoredPosition3D(),
                        GetFromReference(key).GetRectTransform().Rotation());
                }
            }
        }

        private void ClosePanel()
        {
            cts.Cancel();
            cts.Dispose();
            GenerateItemNoEffect();
            //Close();

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

            ui.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>().alpha = 1;
            ui.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>().DOFade(0, 1.8f)
                .SetEase(Ease.InQuad);
            // 确保动画结束时恢复原始位置和旋转
            shakeSequence.OnComplete(() =>
            {
                uiElement.anchoredPosition3D = originalPosition;
                uiElement.rotation = originalRotation;
            });
        }

        private async UniTask<bool> IsGetData()
        {
            while (results.Count <= 0)
            {
                await UniTask.Delay(1000, cancellationToken: cts.Token);
            }

            return true;
        }

        void InitNode()
        {
            WebMessageHandlerOld.Instance.AddHandler(CMD.EQUIPALLCOMPOUND, OnEquipAllCompoundResponse);
        }


        public async UniTask InitRewardItems(List<Vector3> args, CancellationToken cct)
        {
            this.GetFromReference(KSuccess).SetActive(false);
            var KMid_Pos = this.GetFromReference(UIPanel_CompoundDongHua.KMid_Pos);
            this.GetTextMeshPro(KText_Tips).SetTMPText(tbLanguage.Get("equip_merge_success").current);
            var list = KMid_Pos.GetList();
            list.Clear();
            GetFromReference(UIPanel_CompoundDongHua.KViewport).GetComponent<Mask>().enabled = false;
            foreach (var item in args)
            {
                var ui = await list.CreateWithUITypeAsync(UIType.UICommon_RewardItem, item, false, cct);
                ui.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>().alpha = 0;
            }

            list.Sort(JiYuUIHelper.RewardUIComparer);
            //KMid_Pos.SetActive(true);
            var bg = GetFromReference(UIPanel_CompoundDongHua.KBg_MidImg);
            float addition = Mathf.Ceil(args.Count / 5f) - 1;
            var initHeight = 700;
            float maxHeight = 900f;
            bg.GetRectTransform().SetHeight(initHeight);
            Log.Debug($"InitReWardItem,addition:{addition}");
            if (addition >= 0 && addition <= 4)
            {
                bg.GetRectTransform().SetHeight(initHeight + addition * 166);
            }
            else if (addition > 4)
            {
                bg.GetRectTransform().SetHeight(maxHeight);
            }

            this.GetFromReference(KSuccess).SetActive(true);

            InitEffect();
           

           
        }
        public  void InitRewardItems(List<Vector3> args)
        {
            this.GetFromReference(KSuccess).SetActive(true);
            var KMid_Pos = this.GetFromReference(UIPanel_CompoundDongHua.KMid_Pos);
            this.GetTextMeshPro(KText_Tips).SetTMPText(tbLanguage.Get("equip_merge_success").current);
            var list = KMid_Pos.GetList();
            list.Clear();
            GetFromReference(UIPanel_CompoundDongHua.KViewport).GetComponent<Mask>().enabled = false;
            foreach (var item in args)
            {
                var ui =list.CreateWithUITypeAsync(UIType.UICommon_RewardItem, item, false);
                //ui.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>().alpha = 0;
            }

            list.Sort(JiYuUIHelper.RewardUIComparer);
            //KMid_Pos.SetActive(true);
            var bg = GetFromReference(UIPanel_CompoundDongHua.KBg_MidImg);
            float addition = Mathf.Ceil(args.Count / 5f) - 1;
            var initHeight = 700;
            float maxHeight = 900f;
            bg.GetRectTransform().SetHeight(initHeight);
            Log.Debug($"InitReWardItem,addition:{addition}");
            if (addition >= 0 && addition <= 4)
            {
                bg.GetRectTransform().SetHeight(initHeight + addition * 166);
            }
            else if (addition > 4)
            {
                bg.GetRectTransform().SetHeight(maxHeight);
            }
            NetWorkManager.Instance.SendMessage(CMD.QUERYEQUIP);
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Compound, out UI ui1))
            {
                var uis = ui1 as UIPanel_Compound;
                uis.RefreshCompoundPanel();
            }
        }

        private async void InitEffect()
        {
            var bg = GetFromReference(UIPanel_CompoundDongHua.KBg_MidImg);
            var KMid_Pos = this.GetFromReference(UIPanel_CompoundDongHua.KMid_Pos);
            var list = KMid_Pos.GetList();
            JiYuTweenHelper.SetEaseAlphaAndPosUtoB(bg, 0, 1200, cts.Token, 0.35f, false, false).Forget();

            foreach (var item in list.Children)
            {
                //item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>().alpha = 1;
                setScaleCanCancel(item.GetFromReference(UICommon_RewardItem.KBtn_Item), 0.5f, true, 2, 1);
                await UniTask.Delay(200, cancellationToken: cts.Token);
            }
        }

        private async UniTask<AsyncUnit> setScaleCanCancel(UI ui, float duration = 0.25f, bool isAlpha = true,
            float scaleStart = 0f, float scaleEnd = 1f, Ease type = Ease.InQuad)
        {
            if(ui == null)
            {
                return  AsyncUnit.Default;
            }
            try
            {
                if (isAlpha)
                {
                    ui.GetComponent<CanvasGroup>().alpha = 0f;
                    ui.GetComponent<CanvasGroup>().DOFade(1, duration).SetEase(type).SetUpdate(true);
                }
                await UniTask.Delay(200, cancellationToken: cts.Token);
                ui?.GetRectTransform()?.SetScale(new Vector3(scaleStart, scaleStart, scaleStart));
                var tween = ui.GetComponent<RectTransform>().DOScale(new Vector3(scaleEnd, scaleEnd, scaleEnd), duration)
                    .SetEase(type)
                    .SetUpdate(true);
                return AsyncUnit.Default;
            }
            catch (OperationCanceledException)
            {

               ui.GetComponent<RectTransform>().DOComplete();
               ui.GetComponent<CanvasGroup>().DOComplete();
            }

            return AsyncUnit.Default;
        }

        private async void OnEquipAllCompoundResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.EQUIPALLCOMPOUND, OnEquipAllCompoundResponse);
            EquipResDto resultCraft = new EquipResDto();
            resultCraft.MergeFrom(e.data);
            //RepeatedField<string> repeatedField = new RepeatedField<string>();
            if (e.data.IsEmpty)
            {
                Log.Debug("OnEquipAllCompoundResponse.IsEmpty", Color.red);

                return;
            }

            Log.Debug($"resultCraft {resultCraft}", Color.green);
            //JiYuUIHelper.TurnStrReward2List()
            results = resultCraft.EquipDtoList;
        }

        

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}