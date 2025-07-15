//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HotFix_UI;
using UnityEngine;
using static HotFix_UI.JiYuUIHelper;
using static XFramework.UIManager;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_First_Charge)]
    internal sealed class UIPanel_First_ChargeEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_First_Charge;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_First_Charge>();
        }
    }

    public partial class UIPanel_First_Charge : UI, IAwake
    {
        #region property

        private Tblanguage tblanguage;
        private Tbfr tbfr;

        private int ID = 1;
        private long timerId = 0;
        private CancellationTokenSource cts = new CancellationTokenSource();

        #endregion

        public async void Initialize()
        {
            await JiYuUIHelper.InitBlur(this);
            this.GetFromReference(KBtn_Close).GetXButton().OnClick?.Add(() =>
            {
                JiYuUIHelper.DestoryAllTips();
                Close();
            });
            DataInit();
            TextInit();
            SetIamge();
            CreatTime().Forget();
            BtnInit();
            SetAllCloseTip();
            JiYuTweenHelper.PlayUIImageTranstionFX(this.GetFromReference(UIPanel_First_Charge.KImg_Gift),
                cancellationToken: cts.Token);
            JiYuTweenHelper.SetEaseAlphaAndPosLtoR(this.GetFromReference(UIPanel_First_Charge.KTitle), 0, 100,
                cts.Token, 0.15f, false);
            JiYuTweenHelper.PlayUIImageTranstionFX(this.GetFromReference(UIPanel_First_Charge.KTitle), cts.Token,
                "FFF5C2", JiYuTweenHelper.UIDir.Right, 0.5f, 1f);

            var height1 = this.GetFromReference(UIPanel_First_Charge.KBg_Item).GetRectTransform().AnchoredPosition().y;
            JiYuTweenHelper.SetEaseAlphaAndPosB2U(this.GetFromReference(UIPanel_First_Charge.KBg_Item), height1, 100,
                cts.Token, 0.3f, true, true);
            await UniTask.Delay(200, cancellationToken: cts.Token);
            var KCommon_Btn = this.GetFromReference(UIPanel_First_Charge.KCommon_Btn);
            JiYuTweenHelper.PlayUIImageSweepFX(KCommon_Btn,
                cancellationToken: cts.Token);
        }

        private void DataInit()
        {
            tblanguage = ConfigManager.Instance.Tables.Tblanguage;
            tbfr = ConfigManager.Instance.Tables.Tbfr;
        }

        private void TextInit()
        {
            this.GetFromReference(KText_Top).GetTextMeshPro().SetTMPText(tblanguage.Get(tbfr.Get(ID).name).current);
            this.GetFromReference(KText_Notice).GetTextMeshPro().SetTMPText(tblanguage.Get(tbfr.Get(ID).desc).current);
            var KCommon_Btn = this.GetFromReference(UIPanel_First_Charge.KCommon_Btn);
            var KText_Btn = KCommon_Btn.GetFromReference(UICommon_Btn.KText_Btn);
            // if (ResourcesSingleton.Instance.firstChargeInt == 0)
            // {
            //     KText_Btn.GetTextMeshPro().SetTMPText("");
            // }
            if (ResourcesSingleton.Instance.firstChargeInt == 2 || ResourcesSingleton.Instance.firstChargeInt == 3)
            {
                KText_Btn.GetTextMeshPro()
                    .SetTMPText(tblanguage.Get("common_state_gain").current);
            }
            else
            {
                KText_Btn.GetTextMeshPro().SetTMPText(tblanguage.Get("fr_state_goto").current);
            }
        }

        private async UniTaskVoid CreatTime()
        {
            var itemList = this.GetFromReference(KPos_Item).GetList();
            itemList.Clear();

            foreach (Vector3 v3 in tbfr.Get(ID).reward)
            {
                var itemUI = await itemList.CreateWithUITypeAsync<Vector3>(UIType.UICommon_RewardItem, v3, false);
                if (v3.x != 11)
                {
                    JiYuUIHelper.SetRewardOnClick(v3, itemUI);
                }
                else
                {
                    var btn = itemUI.GetFromReference(UICommon_RewardItem.KBtn_Item);
                    btn.GetXButton().OnClick?.Add(() =>
                    {
                        UIHelper.Remove(UIType.UICommon_ItemTips);

                        var equip = new MyGameEquip()
                        {
                            reward = v3
                        };

                        var tipUI = UIHelper.Create(UIType.UICommon_EquipTips, equip);
                        tipUI.GetFromReference(UICommon_EquipTips.KImg_TopTitle).SetActive(false);
                        tipUI.GetFromReference(UICommon_EquipTips.KBottom).SetActive(false);
                        tipUI.GetFromReference(UICommon_EquipTips.KBtn_Decrease).SetActive(false);
                        tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).SetActive(false);
                        tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw).SetActive(true);

                        var itemPos = JiYuUIHelper.GetUIPos(btn);
                        tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw).GetRectTransform()
                            .SetAnchoredPositionX(itemPos.x);

                        tipUI.GetRectTransform().SetAnchoredPositionY(60);
                    });
                }

                itemUI.GetRectTransform().SetScale(new Vector2(0.865f, 0.865f));
            }
        }


        private void SetIamge()
        {
            this.GetFromReference(KImg_Gift).GetImage().SetSpriteAsync(tbfr.Get(ID).pic, false);
        }

        private void BtnInit()
        {
            var KCommon_Btn = this.GetFromReference(UIPanel_First_Charge.KCommon_Btn);


            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KCommon_Btn, () =>
            {
                JiYuUIHelper.DestoryAllTips();
                if (ResourcesSingleton.Instance.firstChargeInt == 2 || ResourcesSingleton.Instance.firstChargeInt == 3)
                {
                    WebMessageHandler.Instance.AddHandler(CMD.GETCHARGE, OnGetFirstChargeResponse);
                    NetWorkManager.Instance.SendMessage(CMD.GETCHARGE);
                }
                else
                {
                    GoToChongZhi();
                    Close();
                }
            });
        }

        private void GoToChongZhi()
        {
            var str = "type=1;para=[141]";
            JiYuUIHelper.GoToSomePanel(str);
            Close();
            JiYuUIHelper.DestroyAllSubPanel();
        }

        private void OnGetFirstChargeResponse(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(CMD.GETCHARGE, OnGetFirstChargeResponse);
            Google.Protobuf.WellKnownTypes.BoolValue boolValue = new Google.Protobuf.WellKnownTypes.BoolValue();
            boolValue.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            if (boolValue.Value == true)
            {
                ResourcesSingleton.Instance.firstChargeInt = 4;
                List<Vector3> vector3s = new List<Vector3>();
                vector3s = tbfr.Get(ID).reward;
                var ui = UIHelper.Create(UIType.UICommon_Reward, vector3s);
                var KCommon_Btn = this.GetFromReference(UIPanel_First_Charge.KCommon_Btn);
                KCommon_Btn?.SetActive(false);
                //ui.GetFromReference(UICommon_Reward.KBg_Img).GetXButton().OnClick?.Add(() =>
                //{
                //    var parent = this.GetParent<UICommonFunButton>();
                //    parent.SetActive(false);
                //    JiYuUIHelper.DestoryAllTips();
                //    Close();
                //});
                //ui.GetFromReference(UICommon_Reward.KBtn_Close).GetXButton().OnClick?.Add(() =>
                //{
                //    var parent = this.GetParent<UICommonFunButton>();
                //    parent.SetActive(false);
                //    JiYuUIHelper.DestoryAllTips();
                //    Close();
                //});
            }
        }

        private void SetAllCloseTip()
        {
            this.GetFromReference(KBg_Item).GetXButton().OnClick?.Add(JiYuUIHelper.DestoryAllTips);
            this.GetFromReference(KBtn_Img_Gift).GetXButton().OnClick?.Add(JiYuUIHelper.DestoryAllTips);
        }

        protected override void OnClose()
        {
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}