//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using cfg.config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HotFix_UI;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Activity_EnergyShop)]
    internal sealed class UIPanel_Activity_EnergyShopEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Activity_EnergyShop;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Activity_EnergyShop>();
        }
    }

    public partial class UIPanel_Activity_EnergyShop : UI, IAwake<int>
    {
        public int id;

        // private UI img_Rabbit;
        // private UI txt_DesRight;
        // private UI txt_DesLeft;
        // private UI img_Gold;
        // private UI img_Gold_Big;
        // private UI txt_GoldNum;
        // private UI txt_Title;
        // private UI txt_Time;
        // private UI btnClose;
        private Tbenergy_shop tbEnergy_shop;
        private Tbenergy_shop_goods tbEnergy_shop_goods;
        private Tblanguage tblanguage;
        private Tbconstant tbConstant;
        private Tbitem tbItem;
        private Tbactivity tbActivity;
        public bool isCanBuy;
        private long timerId;
        private long endTime;
        private long startTime;
        private long tillTime;
        private bool isInit;
        private CancellationTokenSource cts = new CancellationTokenSource();

        public async void Initialize(int activityId)
        {
            await UnicornUIHelper.InitBlur(this);
            InitJsonConfig();
            StartTimer();
            id = tbActivity.GetOrDefault(activityId).link;
            isCanBuy = false;
            isInit = false;
           
            InitShopDes().Forget();

            InitTimeSet(activityId);

            SetWidth();

            InitEffect();
        }


        public void StartTimer()
        {
            //����һ��ÿִ֡�е������൱��Update
            var timerMgr = TimerManager.Instance;
            timerId = timerMgr.StartRepeatedTimer(2500, this.Update);
        }
        void Update()
        {
            var KCommon_CloseInfo = GetFromReference(UIPanel_Activity_EnergyShop.KCommon_CloseInfo);
            KCommon_CloseInfo.GetTextMeshPro().SetTMPText(tblanguage.Get("text_window_close").current);

            KCommon_CloseInfo.GetTextMeshPro().DoFade(1, 0.1f, 1f).AddOnCompleted(() =>
            {
                KCommon_CloseInfo.GetTextMeshPro().DoFade(0.1f, 1, 1f);
            });
        }
        private async void InitEffect()
        {
            var KContainerItem = this.GetFromReference(UIPanel_Activity_EnergyShop.KContainerItem);
            UnicornTweenHelper.SetEaseAlphaAndScaleWithFour(GetFromReference(KTip),cancellationToken:cts.Token);
            UnicornTweenHelper.PlayUIImageTranstionFX(GetFromReference(KImg_Rabbit),cancellationToken:cts.Token);

            GetFromReference(KTip).GetComponent<CanvasGroup>().alpha = 0f;
            GetFromReference(KTip).GetComponent<CanvasGroup>().DOFade(1f, 1.5f).SetEase(Ease.InQuad).SetUpdate(true);
            var height1 = KContainerItem.GetRectTransform().AnchoredPosition().y;
            UnicornTweenHelper.SetEaseAlphaAndPosB2U(KContainerItem, height1, 100, cts.Token, 0.3f, true,
                true);

            
            var parent = this.GetFromReference(UIPanel_Activity_EnergyShop.KItemPos);
            var list = parent.GetList();
            foreach (var item in list)
            {
                var items = item as UISubPanel_EnergyShopItem;
                UnicornTweenHelper.ChangeSoftness(items.GetFromReference(UISubPanel_EnergyShopItem.KMid), 300, 0.35f, cancellationToken: cts.Token);
            }

            var parent1 = this.GetFromReference(UIPanel_Activity_EnergyShop.KItemPosBott);
            var list1 = parent1.GetList();
            foreach (var item in list1)
            {
                var items = item as UISubPanel_EnergyShopItem;
                UnicornTweenHelper.ChangeSoftness(items.GetFromReference(UISubPanel_EnergyShopItem.KMid), 300, 0.35f, cancellationToken: cts.Token);
            }

        }

        private void SetWidth()
        {
            var width = this.GetTextMeshPro(KTxt_GoldNum).Get().preferredWidth + 91f;
            GetFromReference(KUp).GetRectTransform().SetWidth(width + 20f);
            GetFromReference(KUp).GetRectTransform().SetAnchoredPositionX(0);
        }

        private void InitTimeSet(int activityId)
        {
            Log.Debug($"�id{activityId}");
            endTime = ResourcesSingletonOld.Instance.activity.activityMap.ActivityMap_[activityId].EnergyRecord.EndTime -
                      (ResourcesSingletonOld.Instance.serverDeltaTime / 1000);

            if (!ResourcesSingletonOld.Instance.activity.activityMap.ActivityMap_.TryGetValue(activityId,
                    out var gameActivity))
            {
                Close();
                return;
            }

            startTime = gameActivity.EnergyRecord.StartTime;
            tillTime = gameActivity.EnergyRecord.StartTime + RetrunShowValidSecond() + 10;


            InitContainer();
        }

      

        private void InitContainer()
        {
            Log.Debug($"InitContainer");

            long clientT = UnicornUIHelper.GetServerTimeStamp(true);
            string timeStr = "";
            if (!UnicornUIHelper.TryGetRemainingTime(clientT, endTime, out timeStr))
            {
                CloseThisPanel();
                Log.Debug($"Close");
                return;
            }
            else
            {
                if (clientT >= startTime && clientT < tillTime)
                {
                    isCanBuy = false;
                }
                else
                {
                    isCanBuy = true;
                }
            }

            UpdateConatainerItemTopAsync(isCanBuy).Forget();
            SetTimeTxt(isCanBuy, timeStr);

        }

        private void UpdateTimer()
        {
            //Log.Debug($"UpdateTimer");

            long clientT = UnicornUIHelper.GetServerTimeStamp(true);
            string timeStr = "";
            if (!UnicornUIHelper.TryGetRemainingTime(clientT, tillTime, out timeStr))
            {
                CloseThisPanel();
            }
            else
            {
                if (clientT >= startTime && clientT < tillTime)
                {
                    isCanBuy = false;
                }
                else
                {
                    if (!isCanBuy)
                    {
                        isCanBuy = true;
                        UpdateConatainerItemTopAsync(isCanBuy).Forget();
                    }
                    else
                    {
                        isCanBuy = true;
                    }
                }
            }

            SetTimeTxt(isCanBuy, timeStr);
        }

        private long RetrunShowValidSecond()
        {
            return tbEnergy_shop.GetOrDefault(id).showValid * 24 * 60 * 60;
        }

        private void CloseThisPanel()
        {
            CloseUpdate();
            UnicornUIHelper.DestoryAllTips();
            Close();
        }

        // private void CloseAllTip()
        // {
        //     UIHelper.Remove(UIType.UICommon_Reward_Tip);
        //     UIHelper.Remove(UIType.UICommon_EquipTips);
        //     UIHelper.Remove(UIType.UICommon_ItemTips);
        // }

        private void CloseUpdate()
        {
            var timerMgr = TimerManager.Instance;
            timerMgr?.RemoveTimerId(ref this.timerId);
            this.timerId = 0;
        }

        private void InitJsonConfig()
        {
            tbEnergy_shop = ConfigManager.Instance.Tables.Tbenergy_shop;
            tbEnergy_shop_goods = ConfigManager.Instance.Tables.Tbenergy_shop_goods;
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbConstant = ConfigManager.Instance.Tables.Tbconstant;
            tbItem = ConfigManager.Instance.Tables.Tbitem;
            tbActivity = ConfigManager.Instance.Tables.Tbactivity;
        }

        private async UniTaskVoid InitShopDes()
        {
            var img_Rabbit = GetFromReference(UIPanel_Activity_EnergyShop.KImg_Rabbit);
            var txt_DesRight = GetFromReference(UIPanel_Activity_EnergyShop.KTxt_DesRight);
            var txt_DesLeft = GetFromReference(UIPanel_Activity_EnergyShop.KTxt_DesLeft);
            var img_Gold = GetFromReference(UIPanel_Activity_EnergyShop.KImg_Gold);
            var img_Gold_Big = GetFromReference(UIPanel_Activity_EnergyShop.KImg_GoldBig);
            var txt_GoldNum = GetFromReference(UIPanel_Activity_EnergyShop.KTxt_GoldNum);
            var txt_Title = GetFromReference(UIPanel_Activity_EnergyShop.KTxt_Title);
            var txt_Time = GetFromReference(UIPanel_Activity_EnergyShop.KTxt_Time);
            var btnClose = GetFromReference(UIPanel_Activity_EnergyShop.KBtnClose);
            var energyShop = tbEnergy_shop.Get(id);
            var name = energyShop.name;
            var nameLang = tblanguage.Get(name).current;
            txt_Title.GetComponent<TMP_Text>().SetTMPText(nameLang);
            var pic = energyShop.pic;
            img_Rabbit.GetImage().SetSpriteAsync(pic, false);
            var str = tblanguage.Get("energy_shop_desc").current;
            var strs = str.Split(",");
            if (strs.Length >= 2)
            {
                var strL = strs[0].Replace("{0}", tbConstant.Get("energy_shop_item_num").constantValue.ToString());
                txt_DesLeft.GetTextMeshPro().SetTMPText(strL);
                txt_DesRight.GetTextMeshPro().SetTMPText("," + strs[1]);
            }

            var goldId = tbConstant.Get("energy_shop_item").constantValue;
            var goldIcon = tbItem.Get(goldId).icon;
            img_Gold.GetImage().SetSpriteAsync(goldIcon, false);
            img_Gold_Big.GetImage().SetSpriteAsync(goldIcon, false);
            if (ResourcesSingletonOld.Instance.items.ContainsKey(goldId))
            {
                txt_GoldNum.GetTextMeshPro().SetTMPText(ResourcesSingletonOld.Instance.items[goldId].ToString());
            }
            else
            {
                txt_GoldNum.GetTextMeshPro().SetTMPText(0.ToString());
            }

            btnClose.GetXButton().OnClick.Add(() => CloseThisPanel());
            var btnClose1 = GetFromReference(UIPanel_Activity_EnergyShop.KBtnClose1);
            btnClose1.GetXButton().OnClick.Add(() => UnicornUIHelper.DestoryAllTips());
            //btnClose.GetComponent<Button>()
            //TODO:

            //SetTimeTxt(false,0);

            //UpdateConatainerItemTopAsync(false).Forget();
        }


        public void UpdateGoldNum()
        {
            var img_Rabbit = GetFromReference(UIPanel_Activity_EnergyShop.KImg_Rabbit);
            var txt_DesRight = GetFromReference(UIPanel_Activity_EnergyShop.KTxt_DesRight);
            var txt_DesLeft = GetFromReference(UIPanel_Activity_EnergyShop.KTxt_DesLeft);
            var img_Gold = GetFromReference(UIPanel_Activity_EnergyShop.KImg_Gold);
            var img_Gold_Big = GetFromReference(UIPanel_Activity_EnergyShop.KImg_GoldBig);
            var txt_GoldNum = GetFromReference(UIPanel_Activity_EnergyShop.KTxt_GoldNum);
            var txt_Title = GetFromReference(UIPanel_Activity_EnergyShop.KTxt_Title);
            var txt_Time = GetFromReference(UIPanel_Activity_EnergyShop.KTxt_Time);
            var btnClose = GetFromReference(UIPanel_Activity_EnergyShop.KBtnClose);
            var goldId = tbConstant.GetOrDefault("energy_shop_item").constantValue;
            if (ResourcesSingletonOld.Instance.items.ContainsKey(goldId))
            {
                txt_GoldNum.GetTextMeshPro().SetTMPText(ResourcesSingletonOld.Instance.items[goldId].ToString());
            }
            else
            {
                txt_GoldNum.GetTextMeshPro().SetTMPText("0");
            }

            SetWidth();
        }


        private async UniTaskVoid UpdateConatainerItemTopAsync(bool isStart)
        {
            // var top = GetFromReference(KItemTop);

            var goodsList = tbEnergy_shop_goods.DataList;
            int itemCount = 0;
            var goodsIDs = new List<int>();
            for (int i = 0; i < goodsList.Count; i++)
            {
                if (goodsList[i].energyShop == id)
                {
                    itemCount++;
                    goodsIDs.Add(goodsList[i].id);
                }
            }

            Log.Debug($"itemCount:{itemCount}");
            if (itemCount == 5)
            {
                #region ���� �п����ö�ʧ

                var parent = this.GetFromReference(UIPanel_Activity_EnergyShop.KItemPos);
                var list = parent.GetList();

                list.Clear();
                for (int i = 0; i < 2; i++)
                {
                    var ui = await list.CreateWithUITypeAsync(UIType.UISubPanel_EnergyShopItem, goodsIDs[i], false);
                    //ui.GetFromReference(UISubPanel_EnergyShopItem.KMid).GetComponent<CanvasGroup>().alpha = 0;
                    if (isStart)
                    {
                        var uis = ui as UISubPanel_EnergyShopItem;
                        uis.DisplayLock(false);
                    }
                    //UnicornUIHelper.ForceRefreshLayout(ui.GetFromReference(UISubPanel_EnergyShopItem.KPosLeftRight));
                }

                list.Sort((a, b) =>
                {
                    var uia = a as UISubPanel_EnergyShopItem;
                    var uib = b as UISubPanel_EnergyShopItem;
                    return uib.sort.CompareTo(uia.sort);
                });


                var parent1 = this.GetFromReference(UIPanel_Activity_EnergyShop.KItemPosBott);
                var list1 = parent1.GetList();
                list1.Clear();
                for (int i = 2; i < 5; i++)
                {
                    var ui = await list1.CreateWithUITypeAsync(UIType.UISubPanel_EnergyShopItem, goodsIDs[i], false);
                    //ui.GetFromReference(UISubPanel_EnergyShopItem.KMid). GetComponent<CanvasGroup>().alpha = 0;
                    if (isStart)
                    {
                        var uis = ui as UISubPanel_EnergyShopItem;
                        uis.DisplayLock(false);
                    }
                    // UnicornUIHelper.ForceRefreshLayout(ui.GetFromReference(UISubPanel_EnergyShopItem.KPosLeftRight));
                }

                list1.Sort((a, b) =>
                {
                    var uia = a as UISubPanel_EnergyShopItem;
                    var uib = b as UISubPanel_EnergyShopItem;
                    return uib.sort.CompareTo(uia.sort);
                });

                #endregion

                //for (int i = 1; i <= 5; i++)
                //{
                //    var key = "SubPanel_EnergyShopItem" + i.ToString();
                //    var ui = GetFromReference(key) as UISubPanel_EnergyShopItem;
                //    ui.Initialize(goodsIDs[i - 1]);
                //    if (isStart)
                //    {
                //        ui.DisplayLock(false);
                //    }

                //}
            }
            else
            {
                //�ж���������������ټ�
            }
        }

        private void SetTimeTxt(bool isStart, string timeStr)
        {
            var txt_Time = GetFromReference(UIPanel_Activity_EnergyShop.KTxt_Time);
            if (isStart)
            {
                txt_Time.GetTextMeshPro().SetTMPText(tblanguage.GetOrDefault("energy_end_text").current + timeStr);
            }
            else
            {
                txt_Time.GetTextMeshPro().SetTMPText(tblanguage.GetOrDefault("energy_start_text").current + timeStr);
            }
        }


        protected override void OnClose()
        {
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}