//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Google.Protobuf;
using HotFix_UI;
using Main;
using Spine;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static XFramework.UISubPanel_Shop_Draw;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_Draw)]
    internal sealed class UISubPanel_Shop_DrawEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UISubPanel_Shop_Draw;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_Draw>();
        }
    }

    public partial class UISubPanel_Shop_Draw : UI, IAwake<ShopDrawHelp>
    {
        public struct ShopDrawHelp
        {
            public List<Vector3> rewardList;

            //是否是广告
            public bool isAdvert;
        }

        private int BoxID = 0;
        private int DrawCount = 0;
        //private List<UI> itemList = new List<UI>();
        private List<Vector3> v3List = new List<Vector3>();
        private Tbquality tbquality;
        private Tblanguage tblanguage;
        private Tbequip_data tbequip_Data;
        private Tbdraw_box tbdraw_Box;
        private TbitemOld tbitem;
        private Tbuser_variable tbuser_Variable;
        private DrawParam drawParam = new DrawParam();
        private int thisTag_func = 0;
        private long keyNum = 0;
        private long diaNum = 0;
        private int keyConsume = 0;
        private int diaConsume = 0;
        private int diaConsumeTen = 0;
        private bool isKey = false;
        private bool isDia = false;
        private bool isAD = false;
        private Vector2 RePos = new Vector2();
        private int currentDrawCount;
        private CancellationTokenSource cts;
        private bool isCancelClick = false;
        public void Initialize(ShopDrawHelp shopDrawHelp)
        {

            cts = new CancellationTokenSource();
            AudioManager.Instance.PlayFModAudio(1211);
            RePos = this.GetFromReference(KPos_Reward).GetRectTransform().AnchoredPosition();
            //读取多语言表
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            //读取quality表
            tbquality = ConfigManager.Instance.Tables.Tbquality;
            //读取equip_Data表
            tbequip_Data = ConfigManager.Instance.Tables.Tbequip_data;
            //读取draw_Box表
            tbdraw_Box = ConfigManager.Instance.Tables.Tbdraw_box;
            //读取item表
            tbitem = ConfigManager.Instance.Tables.TbitemOld;
            //读取tbuser_Variable表
            tbuser_Variable = ConfigManager.Instance.Tables.Tbuser_variable;
            BoxID = (int)shopDrawHelp.rewardList[0][0];
            thisTag_func = tbdraw_Box[BoxID].tagFunc;
            v3List = shopDrawHelp.rewardList;
            isAD = shopDrawHelp.isAdvert;
            Vector3 v3Help = shopDrawHelp.rewardList[0];
            //List<Vector3> result =v3List.Remove(v3Help);
            List<Vector3> v3HelpList = new List<Vector3>();
            for (int i = 0; i < v3List.Count; i++)
            {
                v3HelpList.Add(v3List[i]);
            }

            v3HelpList.Remove(v3Help);
            UnicornUIHelper.AddReward(v3HelpList, false,false);
            AddMoneyReward(v3HelpList);


            this.GetFromReference(KText_Skip).GetTextMeshPro().SetTMPText(tblanguage.Get("text_window_pass").current);
            this.GetFromReference(KText_Continue).GetTextMeshPro()
                .SetTMPText(tblanguage.Get("text_window_continue").current);
            this.GetFromReference(KText_end).SetActive(false);

            //this.GetFromReference(KBtn_Skip).SetActive(true);
            if (shopDrawHelp.rewardList.Count == 2)
            {
                DrawCount = 1;
            }
            else if (shopDrawHelp.rewardList.Count == 11)
            {
                DrawCount = 10;
            }
            else
            {
                Debug.Log("UISubPanl_shop_draw's drawList is wrong");
            }


            drawParam.BoxId = BoxID;
            drawParam.DrawCount = DrawCount;
            drawParam.DrawType = 0;
            
            DisplayInit();
          
            var buyBtn = this.GetFromReference(KBtn_Common);
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(buyBtn, BuyBtnOnClick);

            this.GetFromReference(KBg).GetXButton().OnClick.Add(async () =>
            {
               
              
                Log.Debug($"drawCount:{DrawCount}", Color.cyan);
                if (DrawCount == 1)
                {
                    Close();
                }

                if (DrawCount == 10)
                {
                    //if (cts.IsCancellationRequested)
                    //{
                    //    Close();
                    //}
                    if (!isCancelClick)
                    {
                        isCancelClick = true;
                        cts.Cancel();
                    }
                    else
                    {
                        Close();
                    }
               
                   
                }
                cts = new CancellationTokenSource();
            });
        }
        private void SetEffect()
        {
         
            var oneReward = GetFromReference(KPos_Reward).GetList().Children[0] as UICommon_RewardItem;
            oneReward.SetActive(true);
            GetFromReference(KBg_txt).SetActive(true);
            UnicornTweenHelper.SetEaseAlphaAndScaleWithBounce(GetFromReference(KBg_txt), 0.3f, false, 2, 1).Forget();
            UnicornTweenHelper.SetEaseAlphaAndScaleWithBounce(oneReward.GetFromReference(UICommon_RewardItem.KBtn_Item), 0.3f, false, 2, 1).Forget();
            UniTask.Delay(200,cancellationToken:cts.Token);
            UnicornTweenHelper.SetEaseAlphaAndPosB2U(GetFromReference(KBtn_Container), -280f,cancellationToken:cts.Token,duration:0.2f);
        }
        private async void DisplayInit()
        {
            //this.GetFromReference(KPos_Reward).GetRectTransform().SetAnchoredPosition(RePos);
            //this.GetFromReference(KPos_Reward).GetRectTransform().SetHeight(0);
            this.GetFromReference(KText_end).SetActive(false);
            DestroyItem();

            #region 采集数据

            diaNum = ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin;
            if (ResourcesSingletonOld.Instance.items.TryGetValue(tbdraw_Box[BoxID].item, out long value))
            {
                keyNum = value;
            }
            else
            {
                keyNum = 0;
            }

            if (tbdraw_Box[BoxID].item > 0)
            {
                keyConsume = 1;
            }
            else
            {
                keyConsume = 0;
            }

            if (tbdraw_Box[BoxID].drawType == 2 || tbdraw_Box[BoxID].drawType == 4)
            {
                diaConsumeTen = tbdraw_Box[BoxID].ten;
            }
            else
            {
                diaConsumeTen = 0;
            }

            diaConsume = tbdraw_Box[BoxID].one;

            #endregion
            GetFromReference(KAnimation_One).SetActive(false);
            switch (tbdraw_Box[BoxID].drawType)
            {
                case 2:
                    OnlyTenDraw();
                    break;
                case 4:
                    OnlyTenDraw();
                    break;
                case 5:
                    OneToTen();
                    break;
                default:
                    break;
            }


            if (DrawCount == 1)
            {

                //this.GetFromReference(KPos_Reward).GetRectTransform().SetHeight(700);
                this.GetFromReference(KPos_Reward).GetRectTransform().SetAnchoredPositionY(-340);
                this.GetFromReference(KBtn_Container).GetRectTransform().SetAnchoredPositionY(-280);
                this.GetFromReference(KBg_txt).GetRectTransform().SetAnchoredPositionY(-992);
                GetFromReference(KAnimation_One).SetActive(false);
                this.GetFromReference(KBtn_Animation).SetActive(false);
                currentDrawCount = 1;
                var list = this.GetFromReference(KPos_Reward).GetList();
                list.Clear();
                var uiRe = await list.CreateWithUITypeAsync(UIType.UICommon_RewardItem, v3List[1],false);
                uiRe.GetRectTransform().SetScale2(2f);
                uiRe.SetActive(false);
                GetFromReference(KBg_txt).SetActive(false);
                this.GetFromReference(KBtn_Skip).SetActive(false);
                this.GetFromReference(KText_Reward).SetActive(true);
                this.GetFromReference(KText_Reward).GetTextMeshPro()
                    .SetColor("#" + tbquality[(int)v3List[1][2]].fontColor);
                this.GetFromReference(KText_Reward).GetTextMeshPro().SetTMPText(
                    tblanguage.Get(tbquality[(int)v3List[1][2]].name).current +
                    tblanguage.Get(tbequip_Data.Get((int)v3List[1][1]).name).current);
                switch (tbdraw_Box[BoxID].drawType)
                {
                    case 1:
                        OnlyOneDraw();
                        break;
                    case 4:
                        OnlyOneDraw();
                        break;
                    case 5:
                        OneToTen();
                        break;
                    default:
                        break;
                }
                JudgeCountIsEnuoghOrNot();

                SetEffect();
            }
            else if (DrawCount == 10)
            {
                //this.GetFromReference(KPos_Reward).GetRectTransform().SetAnchoredPositionY(255);
                this.GetFromReference(KPos_Reward).GetRectTransform().SetAnchoredPositionY(-109);
                this.GetFromReference(KBtn_Container).GetRectTransform().SetAnchoredPositionY(-470);
                this.GetFromReference(KBg_txt).GetRectTransform().SetAnchoredPositionY(-767);
                GetFromReference(KAnimation_One).SetActive(false);
                this.GetButton(KBg).SetEnabled(true);
                this.GetFromReference(KText_end).SetActive(true);
                this.GetFromReference(KText_end).GetTextMeshPro().SetTMPText(tblanguage.Get("drawbox_gain_money_text")
                    .current
                    .Replace("{0}", (tbdraw_Box[BoxID].money * 10).ToString()));
                this.GetFromReference(KText_Continue).SetActive(false);
                this.GetFromReference(KBtn_Skip).SetActive(false);
                this.GetFromReference(KText_Reward).SetActive(false);
                this.GetFromReference(KBtn_Animation).SetActive(false);
                this.GetFromReference(KBtn_Common).SetActive(false);
                //中央排布
                //-----------------------------------------------------------------------
                var list = this.GetFromReference(KPos_Reward).GetList();
                list.Clear();

                await GenerateTenItemAsync(list);
                this.GetFromReference(KBtn_Common).SetActive(true);



                //JudgeCountIsEnuoghOrNot();
            }


            if (isAD)
            {
                //Debug.Log("isADDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD");
                this.GetFromReference(KBtn_Common).SetActive(false);
                this.GetFromReference(KText_Continue).SetActive(true);
            }
        }

      

        private async UniTask GenerateTenItemAsync(UIListComponent list)
        {
           
                list.Clear();
                for (int i = 0; i < 10; i++)
                {
                    currentDrawCount++;
                    int iHelp = i;
                    var uiRe = await list.CreateWithUITypeAsync(UIType.UICommon_RewardItem, v3List[i + 1], false);
                    uiRe.GetFromReference(UICommon_RewardItem.KEffect).SetActive(false);
                    uiRe.GetRectTransform().SetScale(new Vector2(1f, 1f));
                    uiRe.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>().alpha = 0;

                }
            UnicornUIHelper.ForceRefreshLayout(GetFromReference(KPos_Reward));
           await SetEffectForItem(list);
            isCancelClick = true;
           
        }
        private async  UniTask<AsyncUnit> SetEffectForItem(UIListComponent list)
        {
            if (list == null)
            {
                return AsyncUnit.Default;
            }
            for (int i = 0; i < list.Children.Count; i++)
            {
                var uiRe= list.Children[i] as UICommon_RewardItem;
                //uiRe.SetActive(true);
                uiRe.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>().alpha = 1;
                if (tbequip_Data.Get((int)v3List[i + 1][1]).sYn == 1)
                {
                    uiRe.GetFromReference(UICommon_RewardItem.KEffect).SetActive(true);
                    this.GetFromReference(KBtn_Animation).SetActive(true);
                    var equip = v3List[i + 1];
                    UnicornUIHelper.SetIconOnly(equip, this.GetFromReference(KImgrotateCenter));
                    this.GetButton(KBtn_Animation)?.OnClick.Clear();
                    this.GetButton(KBtn_Animation).OnClick.Add(CloseDisplayS);
                    await WaitForButtonClickAsync(this.GetFromReference(KBtn_Animation));
                   
                    try
                    {
                        await UnicornTweenHelper.SetScaleWithBounce(uiRe.GetFromReference(UICommon_RewardItem.KBtn_Item), 0.15f, 4f, 4, 1, cancellationToken: cts.Token);
                        await UniTask.Delay(250, cancellationToken: cts.Token);
                       
                    }
                    catch (OperationCanceledException)
                    {
                        uiRe.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<RectTransform>()?.DOComplete();
                        uiRe.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>()?.DOComplete();
                        foreach (var item in list.Children)
                        {
                            var uiRe1 = item as UICommon_RewardItem;
                            uiRe1.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>().alpha = 1;
                        }
                        return AsyncUnit.Default;
                    }
                }
                else
                {

                    try
                    {
                        UnicornTweenHelper.SetEaseAlphaAndPosRtoL(uiRe.GetFromReference(UICommon_RewardItem.KBtn_Item), 0, 200, cts.Token, 0.2f - i * 0.02f, false, false);
                        UnicornTweenHelper.SetAngleRotateXZ(uiRe.GetFromReference(UICommon_RewardItem.KBtn_Item), 0, 0, cts.Token, 80, 25, 0.2f);
                        //UnicornUIHelper.SetAngleRotateX(uiRe.GetFromReference(UICommon_RewardItem.KBtn_Item), 0, 80, 0.2f);
                        //UnicornTweenHelper.SetAngleRotate(uiRe.GetFromReference(UICommon_RewardItem.KBtn_Item), 0, 25, 0.2f);
                        await UniTask.Delay(100, cancellationToken: cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        foreach (var item in list.Children)
                        {
                            var uiRe1 = item as UICommon_RewardItem;
                            uiRe1.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>().alpha = 1;
                        }
                        uiRe.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<RectTransform>()?.DOComplete();
                        uiRe.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>()?.DOComplete();

                        return AsyncUnit.Default;
                    }


                }
            }

            return AsyncUnit.Default;
        }
        private UniTask WaitForButtonClickAsync(UI button)
        {
            var tcs = new UniTaskCompletionSource(); // 创建 UniTaskCompletionSource

            // 添加按钮点击监听
            button.GetButton().OnClick.AddListener(() =>
            {
                tcs.TrySetResult(); // 按钮点击时完成 UniTask
            });

            return tcs.Task; // 返回等待的 UniTask
        }


        private void CloseDisplayS()
        {
            this.GetFromReference(KBtn_Animation).SetActive(false);
        }

        private void OnlyOneDraw()
        {
            drawParam.DrawCount = 1;
            if (keyConsume > 0 && keyNum > 0) //可以用key并且有key
            {
                this.GetFromReference(KText_Continue).SetActive(false);
                this.GetFromReference(KBtn_Common).SetActive(true);
                this.GetFromReference(KText_Mid).SetActive(false);
                this.GetFromReference(KText_Right).GetTextMeshPro().SetTMPText(keyNum.ToString() + "/1");
                this.GetFromReference(KImg_Left).GetImage().SetSprite(tbitem[tbdraw_Box[BoxID].item].icon, false);
                this.GetFromReference(KText_Right).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("drawbox_key_one_again_text").current);
                SetBtnW();
                isKey = true;
                isDia = false;
            }
            else if (diaConsume <= 0 && keyConsume > 0 && keyNum <= 0) //只能用key且没有key
            {
                this.GetFromReference(KText_Continue).SetActive(true);
                this.GetFromReference(KBtn_Common).SetActive(false);
            }
            else if (diaConsume > 0 && diaNum > diaConsume) //可以用钻石且有足够钻石
            {
                this.GetFromReference(KText_Continue).SetActive(false);
                this.GetFromReference(KBtn_Common).SetActive(true);
                this.GetFromReference(KText_Mid).SetActive(false);
                this.GetFromReference(KText_Right).GetTextMeshPro().SetTMPText(diaConsume.ToString());
                this.GetFromReference(KImg_Left).GetImage().SetSprite(tbuser_Variable[2].icon, false);
                this.GetFromReference(KText_Right).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("drawbox_key_one_again_text").current);
                SetBtnW();
                isKey = false;
                isDia = true;
            }
            else
            {
                this.GetFromReference(KText_Continue).SetActive(true);
                this.GetFromReference(KBtn_Common).SetActive(false);
            }

            if (isAD)
            {
                this.GetFromReference(KBtn_Common).SetActive(false);
                this.GetFromReference(KText_Continue).SetActive(true);
            }
        }

        private void OneToTen()
        {
            drawParam.DrawCount = 1;
            if (keyConsume > 0 && keyNum > 0) //可以用key并且有key
            {
                this.GetFromReference(KText_Continue).SetActive(false);
                this.GetFromReference(KBtn_Common).SetActive(true);
                this.GetFromReference(KText_Mid).SetActive(false);
                if (keyNum > 0 && keyNum < 10)
                {
                    this.GetFromReference(KText_Right).GetTextMeshPro().SetTMPText(keyNum.ToString() + "/1");
                    this.GetFromReference(KText_Right).GetTextMeshPro()
                        .SetTMPText(tblanguage.Get("drawbox_key_one_again_text").current);
                    drawParam.DrawCount = 1;
                }
                else if (keyNum >= 10)
                {
                    this.GetFromReference(KText_Right).GetTextMeshPro().SetTMPText(keyNum.ToString() + "/10");
                    this.GetFromReference(KText_Right).GetTextMeshPro()
                        .SetTMPText(tblanguage.Get("drawbox_key_ten_again_text").current);
                    drawParam.DrawCount = 10;
                }

                this.GetFromReference(KImg_Left).GetImage().SetSprite(tbitem[tbdraw_Box[BoxID].item].icon, false);
                SetBtnW();
                isKey = true;
                isDia = false;
            }
            else if (diaConsume <= 0 && keyConsume > 0 && keyNum <= 0) //只能用key且没有key
            {
                this.GetFromReference(KText_Continue).SetActive(true);
                this.GetFromReference(KBtn_Common).SetActive(false);
            }
            else if (diaConsume > 0 && diaNum > diaConsume) //可以用钻石且有足够钻石
            {
                this.GetFromReference(KText_Continue).SetActive(false);
                this.GetFromReference(KBtn_Common).SetActive(true);
                this.GetFromReference(KText_Mid).SetActive(false);
                this.GetFromReference(KText_Right).GetTextMeshPro().SetTMPText(diaConsume.ToString());
                this.GetFromReference(KImg_Left).GetImage().SetSprite(tbuser_Variable[2].icon, false);
                this.GetFromReference(KText_Right).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("drawbox_key_one_again_text").current);
                SetBtnW();
                isKey = false;
                isDia = true;
            }
            else
            {
                this.GetFromReference(KText_Continue).SetActive(true);
                this.GetFromReference(KBtn_Common).SetActive(false);
            }

            if (isAD)
            {
                this.GetFromReference(KBtn_Common).SetActive(false);
                this.GetFromReference(KText_Continue).SetActive(true);
            }
        }

        private void OnlyTenDraw()
        {
            drawParam.DrawCount = 10;
            if (keyConsume > 0 && keyNum > 10) //可以用key并且有key
            {
                this.GetFromReference(KText_Continue).SetActive(false);
                this.GetFromReference(KBtn_Common).SetActive(true);
                this.GetFromReference(KText_Mid).SetActive(false);
                this.GetFromReference(KText_Right).GetTextMeshPro().SetTMPText(keyNum.ToString() + "/10");
                this.GetFromReference(KImg_Left).GetImage().SetSprite(tbitem[tbdraw_Box[BoxID].item].icon, false);
                this.GetFromReference(KText_Right).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("drawbox_key_ten_again_text").current);
                SetBtnW();
                isKey = true;
                isDia = false;
            }
            else if (diaConsume <= 0 && keyConsume > 0 && keyNum < 10) //只能用key且没有key
            {
                this.GetFromReference(KText_Continue).SetActive(true);
                this.GetFromReference(KBtn_Common).SetActive(false);
            }
            else if (diaConsume > 0 && diaNum > diaConsumeTen) //可以用钻石且有足够钻石
            {
                this.GetFromReference(KText_Continue).SetActive(false);
                this.GetFromReference(KBtn_Common).SetActive(true);
                this.GetFromReference(KText_Mid).SetActive(false);
                this.GetFromReference(KText_Right).GetTextMeshPro().SetTMPText(diaConsumeTen.ToString());
                this.GetFromReference(KImg_Left).GetImage().SetSprite(tbuser_Variable[2].icon, false);
                this.GetFromReference(KText_Right).GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("drawbox_key_ten_again_text").current);
                SetBtnW();
                isKey = false;
                isDia = true;
            }
            else
            {
                this.GetFromReference(KText_Continue).SetActive(true);
                this.GetFromReference(KBtn_Common).SetActive(false);
            }

            if (isAD)
            {
                this.GetFromReference(KBtn_Common).SetActive(false);
                this.GetFromReference(KText_Continue).SetActive(true);
            }
        }

        private void BuyBtnOnClick()
        {
            isCancelClick = false;
            Log.Debug("buybtnclick", Color.cyan);
            if (thisTag_func == 1101)
            {
                WebMessageHandlerOld.Instance.AddHandler(11, 14, OnClick1101BoxBtnResponse);
                NetWorkManager.Instance.SendMessage(11, 14, drawParam);
            }
            else if (thisTag_func == 1102)
            {
                WebMessageHandlerOld.Instance.AddHandler(11, 2, OnSResponse);
                NetWorkManager.Instance.SendMessage(11, 2, drawParam);
            }
            else if (thisTag_func == 1103)
            {
                WebMessageHandlerOld.Instance.AddHandler(11, 3, OnCommonResponse);
                NetWorkManager.Instance.SendMessage(11, 3, drawParam);
            }
            else
            {
                Debug.Log("tag func is wrong");
            }
        }

        private void OnSResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            Log.Debug("OnSResponse", Color.cyan);
            WebMessageHandlerOld.Instance.RemoveHandler(11, 2, OnSResponse);
            DrawInfo drawInfo = new DrawInfo();
            drawInfo.MergeFrom(e.data);
            Debug.Log(e);
            Debug.Log(drawInfo);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                Close();
                return;
            }


            var reStrs = drawInfo.Reward;
            List<Vector3> RewardList = new List<Vector3>();
            RewardList.Add(new Vector3(BoxID, BoxID, BoxID));
            foreach (var re in reStrs)
            {
                RewardList.Add(UnityHelper.StrToVector3(re));
            }

            v3List = RewardList;
            Vector3 v3Help = v3List[0];
            List<Vector3> v3HelpList = new List<Vector3>();
            for (int i = 0; i < v3List.Count; i++)
            {
                v3HelpList.Add(v3List[i]);
            }

            v3HelpList.Remove(v3Help);
            UnicornUIHelper.AddReward(v3HelpList, false,false);
            AddMoneyReward(v3HelpList);
            if (RewardList.Count == 2)
            {
                DrawCount = 1;
            }
            else if (RewardList.Count == 11)
            {
                DrawCount = 10;
            }
            else
            {
                Debug.Log("UISubPanl_shop_draw's 11-2 drawList is wrong");
            }

            //前端缓存
            if (isKey == true && isDia == false)
            {
                ResourcesSingletonOld.Instance.items[tbdraw_Box[BoxID].item] -= DrawCount;
            }
            else if (isKey == false && isDia == true)
            {
                if (DrawCount == 1)
                {
                    ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin -= tbdraw_Box[BoxID].one;
                }
                else if (DrawCount == 10)
                {
                    ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin -= tbdraw_Box[BoxID].ten;
                }
            }

            ResourcesSingletonOld.Instance.UpdateResourceUI();

            //设置父界面文本
            var box1102 = this.GetParent<UISubPanel_Shop_1102_SBox>();
            var box1103 = this.GetParent<UISubPanel_Shop_1103_Box>();
            if (box1102 != null)
            {
                List<int> drawInt = new List<int>();
                drawInt.Add(drawInfo.Guarantee[0]);
                drawInt.Add(drawInfo.Guarantee[1]);
                box1102.DrawSetTxt(drawInt);
            }

            if (box1103 != null)
            {
                int boxHelp = drawInfo.Guarantee[0];
                List<int> drawInt = new List<int>();
                drawInt.Add(boxHelp);
                box1103.DrawSetTxt(drawInt);
            }

            DisplayInit();
        }

        private void OnClick1101BoxBtnResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            Log.Debug("OnClick1101BoxBtnResponse", Color.cyan);
            WebMessageHandlerOld.Instance.RemoveHandler(11, 14, OnClick1101BoxBtnResponse);

            DrawInfo drawInfo = new DrawInfo();
            drawInfo.MergeFrom(e.data);
            Debug.Log(drawInfo);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                Close();
                return;
            }

            var reStrs = drawInfo.Reward;
            List<Vector3> RewardList = new List<Vector3>();
            RewardList.Add(new Vector3(BoxID, BoxID, BoxID));
            foreach (var re in reStrs)
            {
                RewardList.Add(UnityHelper.StrToVector3(re));
            }

            v3List = RewardList;
            Vector3 v3Help = v3List[0];
            List<Vector3> v3HelpList = new List<Vector3>();
            for (int i = 0; i < v3List.Count; i++)
            {
                v3HelpList.Add(v3List[i]);
            }

            v3HelpList.Remove(v3Help);
            UnicornUIHelper.AddReward(v3HelpList, false,false);
            AddMoneyReward(v3HelpList);
            if (RewardList.Count == 2)
            {
                DrawCount = 1;
            }
            else if (RewardList.Count == 11)
            {
                DrawCount = 10;
            }
            else
            {
                Debug.Log("UISubPanl_shop_draw's 11-2 drawList is wrong");
            }

            //前端缓存
            if (tbdraw_Box.Get(BoxID).limitType == 3 || tbdraw_Box.Get(BoxID).limitType == 4)
            {
                for (int i = 0; i < ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1101].BoxInfoList.Count; i++)
                {
                    if (ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1101].BoxInfoList[i].Id == BoxID)
                    {
                        ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1101].BoxInfoList[i].DrawCount -= DrawCount;
                        break;
                    }
                }
            }

            if (isKey == true && isDia == false)
            {
                ResourcesSingletonOld.Instance.items[tbdraw_Box[BoxID].item] -= DrawCount;
            }
            else if (isKey == false && isDia == true)
            {
                if (DrawCount == 1)
                {
                    ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin -= tbdraw_Box[BoxID].one;
                }
                else if (DrawCount == 10)
                {
                    ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin -= tbdraw_Box[BoxID].ten;
                }
            }

            ResourcesSingletonOld.Instance.UpdateResourceUI();

            //设置父界面文本
            var box1102 = this.GetParent<UISubPanel_Shop_1102_SBox>();
            var box1103 = this.GetParent<UISubPanel_Shop_1103_Box>();
            if (box1102 != null)
            {
                List<int> drawInt = new List<int>();
                drawInt.Add(drawInfo.Guarantee[0]);
                drawInt.Add(drawInfo.Guarantee[1]);
                box1102.DrawSetTxt(drawInt);
            }

            if (box1103 != null)
            {
                int boxHelp = drawInfo.Guarantee[0];
                List<int> drawInt = new List<int>();
                drawInt.Add(boxHelp);
                box1103.DrawSetTxt(drawInt);
            }

            DisplayInit();
        }

        private void OnCommonResponse(object sender, WebMessageHandlerOld.Execute e)
        {

            Log.Debug("OnCommonResponse", Color.cyan);
            WebMessageHandlerOld.Instance.RemoveHandler(11, 3, OnCommonResponse);
            DrawInfo drawInfo = new DrawInfo();
            drawInfo.MergeFrom(e.data);
            Debug.Log(e);
            Debug.Log(drawInfo);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                Close();
                return;
            }


            var reStrs = drawInfo.Reward;
            List<Vector3> RewardList = new List<Vector3>();
            RewardList.Add(new Vector3(BoxID, BoxID, BoxID));
            foreach (var re in reStrs)
            {
                RewardList.Add(UnityHelper.StrToVector3(re));
            }

            v3List = RewardList;
            Vector3 v3Help = v3List[0];
            List<Vector3> v3HelpList = new List<Vector3>();
            for (int i = 0; i < v3List.Count; i++)
            {
                v3HelpList.Add(v3List[i]);
            }

            v3HelpList.Remove(v3Help);
            UnicornUIHelper.AddReward(v3HelpList, false,false);
            AddMoneyReward(v3HelpList);
            if (RewardList.Count == 2)
            {
                DrawCount = 1;
            }
            else if (RewardList.Count == 11)
            {
                DrawCount = 10;
            }
            else
            {
                Debug.Log("UISubPanl_shop_draw's 11-3 drawList is wrong");
            }

            //前端缓存
            if (isKey == true && isDia == false)
            {
                ResourcesSingletonOld.Instance.items[tbdraw_Box[BoxID].item] -= DrawCount;
            }
            else if (isKey == false && isDia == true)
            {
                if (DrawCount == 1)
                {
                    ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin -= tbdraw_Box[BoxID].one;
                }
                else if (DrawCount == 10)
                {
                    ResourcesSingletonOld.Instance.UserInfo.RoleAssets.Bitcoin -= tbdraw_Box[BoxID].ten;
                }
            }

            ResourcesSingletonOld.Instance.UpdateResourceUI();

            //设置父界面文本
            var box1102 = this.GetParent<UISubPanel_Shop_1102_SBox>();
            var box1103 = this.GetParent<UISubPanel_Shop_1103_Box>();
            if (box1102 != null)
            {
                List<int> drawInt = new List<int>();
                drawInt.Add(drawInfo.Guarantee[0]);
                drawInt.Add(drawInfo.Guarantee[1]);
                box1102.DrawSetTxt(drawInt);
            }

            if (box1103 != null)
            {
                int boxHelp = drawInfo.Guarantee[0];
                List<int> drawInt = new List<int>();
                drawInt.Add(boxHelp);
                box1103.DrawSetTxt(drawInt);
            }


            DisplayInit();
        }

        private void SetBtnW()
        {
            float btnW = 0;
            btnW += 40;
            
            btnW += this.GetFromReference(KText_Right).GetComponent<XTextMeshProUGUI>().preferredWidth;
            btnW += this.GetFromReference(KImg_Left).GetRectTransform().Width();

            btnW=math.max(btnW,this.GetFromReference(KBtn_Container).GetRectTransform().Width());
            this.GetFromReference(KBtn_Container).GetRectTransform().SetWidth(btnW);
            //this.GetFromReference(KPos_Btn).GetRectTransform().SetWidth(btnW);
            //LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetFromReference(KPos_Btn).GetComponent<RectTransform>());
            UnicornUIHelper.ForceRefreshLayout(GetFromReference(KPosLeftRight));
        }

        private void AddMoneyReward(List<Vector3> vector3s)
        {
            int money = tbdraw_Box.Get(BoxID).money;
            //Debug.Log(thisTag_func);
            //foreach (var tb in tbdraw_Box.DataList)
            //{
            //    if (tb.tagFunc == thisTag_func)
            //    {
            //        money = tb.money;
            //    }
            //}
            Debug.Log("money: " + money);
            Debug.Log("vector3s.Count: " + vector3s.Count);
            Vector3 reward = new Vector3(3, 0, money * vector3s.Count);
            UnicornUIHelper.AddReward(reward, false).Forget();
        }

        private void DestroyItem()
        {

        }

        private void JudgeCountIsEnuoghOrNot()
        {
            if (tbdraw_Box.Get(BoxID).tagFunc == 1101)
            {
                if (tbdraw_Box.Get(BoxID).limitType == 3 || tbdraw_Box.Get(BoxID).limitType == 4)
                {
                    var boxList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1101].BoxInfoList;
                    foreach (var b in boxList)
                    {
                        if (b.Id == BoxID)
                        {
                            int surplusCount = b.DrawCount;
                            if (DrawCount > surplusCount)
                            {
                                this.GetFromReference(KBtn_Common).SetActive(false);
                                this.GetFromReference(KText_Continue).SetActive(true);
                            }

                            break;
                        }
                    }
                }
            }
            //var boxList = ResourcesSingletonOld.Instance.shopMap.IndexModuleMap[1101].
        }

        protected override void OnClose()
        {
            cts.Cancel();
            NetWorkManager.Instance.SendMessage(CMDOld.QUERYEQUIP);
            DestroyItem();
            base.OnClose();
        }
    }
}