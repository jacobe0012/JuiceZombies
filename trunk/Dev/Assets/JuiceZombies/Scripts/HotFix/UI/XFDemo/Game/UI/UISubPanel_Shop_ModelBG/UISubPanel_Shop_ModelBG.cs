//---------------------------------------------------------------------
// JiYuStudio
// Author: huangjinguo
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using cfg.config;
using HotFix_UI;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_ModelBG)]
    internal sealed class UISubPanel_Shop_ModelBGEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Shop_ModelBG;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_ModelBG>();
        }
    }

    public partial class UISubPanel_Shop_ModelBG : UI, IAwake //<int>
    {
        //item的list
        public List<UI> itemList = new List<UI>();

        //item纵向的间隔
        private float itemDeltaH = 30f;

        //title高度
        private float titleH = 100;

        //recharge表
        private Tbrecharge tbrecharge;

        //language表
        private Tblanguage tblanguage;

        //tag_func表
        private Tbtag_func tbtag_func;

        //
        private Tbprice tbprice;

        //
        private Tbchapter tbchapter;

        //
        private Tbgift tbgift;

        //
        private Tbuser_variable tbuser_variable;

        //
        private List<UI> RewardList = new List<UI>();

        //
        public List<UI> RewardHelpList = new List<UI>();

        //reward的横向间隔
        private float rewardW = 10;

        //
        private int m1401IntHelp = 0;

        //
        private int m1401IdHelp = 0;

        //
        private int m1401Extra = 0;

        //
        private UI m1401UI;

        //
        private int m1402IntHelp = 0;

        //
        private int m1402PriceHelp = 0;

        //
        private int m1402Unit = 0;

        //
        private int m1402IdHelp = 0;

        //
        private UI m1402UI;

        //
        private recharge Recharge;

        //
        private long timerId = 0;

        //本次购买的礼包ID
        private int giftId = 0;

        //
        private Tbshop_daily tbshop_Daily;

        //
        private Tbconstant tbconstant;

        //1201模块第一个位置的CD状态
        private bool inCD12011 = false;

        //1201模块需要计算CD的UI
        private UI cd12011ui;

        //1201模块第一个位置的上一次时间
        private long upTime12011 = 0;

        //1201模块第一个位置的取值参数
        private int tagID12011 = 0;

        private int i12011 = 0;

        //1201模块的购买位置
        private int pos1201 = 0;
        private UI secUI;

        //public void Initialize(int InitID)
        public void Initialize()
        {
            this.GetFromReference(KAllItemBg).GetList().Clear();
            this.GetFromReference(KPos_Layout).GetList().Clear();
            this.GetFromReference(KBtn_CloseTip).GetXButton().OnClick.Add(() =>
            {
                JiYuUIHelper.DestoryAllTips();
            });

            //ParaInit();
            //switch (InitID)
            //{
            //    case 1201:
            //        Init1201(InitID);
            //        break;
            //    case 1301:
            //        Init1301(InitID);
            //        break;
            //    case 1401:
            //        Init1401(InitID);
            //        break;
            //    case 1402:
            //        Init1402(InitID);
            //        break;
            //    default:
            //        Debug.Log("Do not have model: " + InitID.ToString());
            //        break;
            //}
        }


        #region

        ///// <summary>
        ///// 参数初始化
        ///// </summary>
        //public void ParaInit()
        //{
        //    tbrecharge = ConfigManager.Instance.Tables.Tbrecharge;
        //    tblanguage = ConfigManager.Instance.Tables.Tblanguage;
        //    tbtag_func = ConfigManager.Instance.Tables.Tbtag_func;
        //    tbprice = ConfigManager.Instance.Tables.Tbprice;
        //    tbchapter = ConfigManager.Instance.Tables.Tbchapter;
        //    tbgift = ConfigManager.Instance.Tables.Tbgift;
        //    tbuser_variable = ConfigManager.Instance.Tables.Tbuser_variable;
        //    tbshop_Daily = ConfigManager.Instance.Tables.Tbshop_daily;
        //    tbconstant = ConfigManager.Instance.Tables.Tbconstant;
        //}

        ///// <summary>
        ///// 初始化钻石商店
        ///// </summary>
        //private void Init1401(int tagID)
        //{
        //    this.GetFromReference(KTitleText).GetTextMeshPro()
        //        .SetTMPText(tblanguage.Get(tbtag_func[tagID].name).current);
        //    this.GetFromReference(KDescText).SetActive(false);
        //    List<recharge> rechargeList = new List<recharge>();
        //    foreach (var recharge in tbrecharge.DataList)
        //    {
        //        if (recharge.tagFunc == tagID)
        //        {
        //            rechargeList.Add(recharge);
        //        }
        //    }

        //    //排序
        //    rechargeList.Sort(delegate(recharge rc1, recharge rc2) { return rc1.sort.CompareTo(rc2.sort); });
        //    //测试后端值是否首充
        //    foreach (var recharge in rechargeList)
        //    {
        //        var ui = UIHelper.Create(UIType.UISubPanel_Shop_item,
        //            this.GetFromReference(KAllItemBg).GameObject.transform, true);
        //        itemList.Add(ui);

        //        ui.GetFromReference(UISubPanel_Shop_item.KItemTitleBg).SetActive(true);
        //        ui.GetFromReference(UISubPanel_Shop_item.KItemImagePos).SetActive(true);
        //        ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(true);
        //        ui.GetFromReference(UISubPanel_Shop_item.KGoldText).SetActive(true);

        //        ui.GetFromReference(UISubPanel_Shop_item.KItemDescText).SetActive(false);
        //        ui.GetFromReference(UISubPanel_Shop_item.KCutBg).SetActive(false);
        //        ui.GetFromReference(UISubPanel_Shop_item.KRewardText).SetActive(false);
        //        ui.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(false);

        //        if (ResourcesSingleton.Instance.shopInit.shopHelpDic[1401][recharge.id][0] == 0)
        //        {
        //            //如果是第一次充值
        //            ui.GetFromReference(UISubPanel_Shop_item.KTitleText).GetTextMeshPro()
        //                .SetTMPText(tblanguage.Get("recharge_double_text").current);
        //        }
        //        else
        //        {
        //            ui.GetFromReference(UISubPanel_Shop_item.KTitleText).GetTextMeshPro().SetTMPText(
        //                tblanguage.Get("recharge_extra_text").current.Replace("{0}", recharge.valueExtra.ToString()));
        //        }

        //        ui.GetFromReference(UISubPanel_Shop_item.KGoldNumTxt).GetTextMeshPro()
        //            .SetTMPText(recharge.value.ToString());
        //        ui.GetFromReference(UISubPanel_Shop_item.KGoldDescTxt).GetTextMeshPro().SetTMPText(
        //            tblanguage.Get(recharge.name).current);
        //        ui.GetFromReference(UISubPanel_Shop_item.KItemImage).GetImage().SetSprite(
        //            recharge.pic, true);
        //        ui.GetFromReference(UISubPanel_Shop_item.KText_Right).GetTextMeshPro().SetTMPText(
        //            tbprice[recharge.price].rmb.ToString() + tblanguage.Get("common_coin_unit").current);
        //        var thisBuyBtn = ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common);
        //        IntValue intValue = new IntValue();
        //        intValue.Value = recharge.id;
        //        JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(thisBuyBtn, () =>
        //        {
        //            if (ResourcesSingleton.Instance.shopInit.shopHelpDic[1401][recharge.id][0] == 0)
        //            {
        //                m1401IntHelp = recharge.value * 2;
        //            }
        //            else
        //            {
        //                m1401IntHelp = recharge.value + recharge.valueExtra;
        //            }

        //            m1401UI = ui;
        //            m1401IdHelp = recharge.id;
        //            m1401Extra = recharge.valueExtra;

        //            WebMessageHandler.Instance.AddHandler(11, 4, On1401Response);
        //            NetWorkManager.Instance.SendMessage(11, 4, intValue);
        //        });
        //    }

        //    rechargeList.Clear();
        //    PosSort();
        //}

        //private void On1401Response(object sender, WebMessageHandler.Execute e)
        //{
        //    WebMessageHandler.Instance.RemoveHandler(11, 4, On1401Response);
        //    var intvalue = new IntValue();
        //    //ConfigurationLoad.ChapterIdFieldNumber=
        //    intvalue.MergeFrom(e.data);
        //    Debug.Log(e);
        //    Debug.Log(intvalue);
        //    if (e.data.IsEmpty)
        //    {
        //        Log.Debug("e.data.IsEmpty", Color.red);
        //        //return;
        //    }

        //    Vector3 vector3 = new Vector3();
        //    vector3.x = 2;
        //    vector3.y = 0;
        //    vector3.z = m1401IntHelp;
        //    JiYuUIHelper.AddReward(vector3, true).Forget();
        //    //ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin += m1401IntHelp;
        //    //ResourcesSingleton.Instance.UpdateResourceUI();
        //    ResourcesSingleton.Instance.shopInit.shopHelpDic[1401][m1401IdHelp][0] += 1;
        //    m1401UI.GetFromReference(UISubPanel_Shop_item.KTitleText).GetTextMeshPro().SetTMPText(
        //        tblanguage.Get("recharge_extra_text").current.Replace("{0}", m1401Extra.ToString()));
        //}

        ///// <summary>
        ///// 初始化金币商店
        ///// </summary>
        ///// <param name="tagID"></param>
        //private void Init1402(int tagID)
        //{
        //    this.GetFromReference(KTitleText).GetTextMeshPro()
        //        .SetTMPText(tblanguage.Get(tbtag_func[tagID].name).current);
        //    this.GetFromReference(KDescText).SetActive(false);
        //    List<recharge> rechargeList = new List<recharge>();
        //    foreach (var recharge in tbrecharge.DataList)
        //    {
        //        if (recharge.tagFunc == tagID)
        //        {
        //            rechargeList.Add(recharge);
        //        }
        //    }

        //    rechargeList.Sort(delegate(recharge rc1, recharge rc2) { return rc1.sort.CompareTo(rc2.sort); });
        //    foreach (var recharge in rechargeList)
        //    {
        //        var ui = UIHelper.Create(UIType.UISubPanel_Shop_item,
        //            this.GetFromReference(KAllItemBg).GameObject.transform, true);
        //        itemList.Add(ui);

        //        ui.GetFromReference(UISubPanel_Shop_item.KItemTitleBg).SetActive(false);
        //        ui.GetFromReference(UISubPanel_Shop_item.KItemImagePos).SetActive(true);
        //        ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(true);
        //        ui.GetFromReference(UISubPanel_Shop_item.KGoldText).SetActive(true);

        //        ui.GetFromReference(UISubPanel_Shop_item.KItemDescText).SetActive(false);
        //        ui.GetFromReference(UISubPanel_Shop_item.KCutBg).SetActive(false);
        //        ui.GetFromReference(UISubPanel_Shop_item.KRewardText).SetActive(false);
        //        ui.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(false);

        //        int moneyHelp = ResourcesSingleton.Instance.levelInfo.maxPassChapterID;
        //        //Debug.Log(moneyHelp);
        //        if (moneyHelp == 0)
        //        {
        //            moneyHelp = tbchapter[1].money;
        //        }
        //        else
        //        {
        //            foreach (var chapter in tbchapter.DataList)
        //            {
        //                if (moneyHelp == chapter.id)
        //                {
        //                    moneyHelp = chapter.money;
        //                    break;
        //                }
        //            }
        //        }
        //        moneyHelp = moneyHelp * 6 * recharge.value;
        //        moneyHelp = (moneyHelp * (ResourcesSingleton.Instance.UserInfo.PatrolGainName + 100)) / 100;
        //        ui.GetFromReference(UISubPanel_Shop_item.KGoldNumTxt).GetTextMeshPro().SetTMPText(moneyHelp.ToString());
        //        ui.GetFromReference(UISubPanel_Shop_item.KGoldDescTxt).GetTextMeshPro().SetTMPText(
        //            tblanguage.Get(recharge.name).current.Replace("{0}", recharge.value.ToString()));
        //        ui.GetFromReference(UISubPanel_Shop_item.KItemImage).GetImage().SetSprite(
        //            recharge.pic, true);
        //        if (recharge.unit == 2)
        //        {
        //            ui.GetFromReference(UISubPanel_Shop_item.KImg_Left).SetActive(true);
        //            ui.GetFromReference(UISubPanel_Shop_item.KImg_Left).GetImage()
        //                .SetSprite(tbuser_variable[2].icon, false);
        //            ui.GetFromReference(UISubPanel_Shop_item.KText_Right).GetTextMeshPro().SetTMPText(
        //                recharge.price.ToString());
        //        }

        //        RedPointMgr.instance.Init("ShopRoot", recharge.tagFunc.ToString() + recharge.id.ToString(),
        //            async (RedPointState state, int data) =>
        //            {
        //                if (JiYuUIHelper.TryGetUI(UIType.UISubPanel_Shop_Tag3, out UI uui))
        //                {
        //                    if (state == RedPointState.Show
        //                        || ResourcesSingleton.Instance.shopInit.shopHelpDic[1402][recharge.id][1] > 0
        //                        || ResourcesSingleton.Instance.shopInit.shopHelpDic[1402][recharge.id][2] > 0)
        //                    {
        //                        await ui.GetImage().SetSpriteAsync("bg_ShopItemGreen", false);
        //                        //RedPointMgr.instance.SetState("ShopRoot", recharge.tagFunc.ToString() + recharge.id.ToString(), RedPointState.Show, 1);
        //                    }
        //                    else
        //                    {
        //                        await ui.GetImage().SetSpriteAsync("bg_ShopItem", false);
        //                    }
        //                    //ui.GetFromReference(UISubPanel_Shop_item.KImg_RedDot).SetActive(state == RedPointState.Show);
        //                }
        //            }, ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common).GetXButton());

        //        //免费次数大于0
        //        if (ResourcesSingleton.Instance.shopInit.shopHelpDic[1402][recharge.id][1] > 0)
        //        {
        //            ui.GetFromReference(UISubPanel_Shop_item.KImg_Left).SetActive(false);
        //            ui.GetFromReference(UISubPanel_Shop_item.KText_Right).GetTextMeshPro().SetTMPText(
        //                tblanguage.Get("common_free_text").current + "("
        //                                                           + ResourcesSingleton.Instance.shopInit.shopHelpDic[
        //                                                               1402][recharge.id][1].ToString()
        //                                                           + ")");
        //            RedPointMgr.instance.SetState("ShopRoot", recharge.tagFunc.ToString() + recharge.id.ToString(),
        //                RedPointState.Show,
        //                (int)(ResourcesSingleton.Instance.shopInit.shopHelpDic[1402][recharge.id][1] +
        //                      ResourcesSingleton.Instance.shopInit.shopHelpDic[1402][recharge.id][2]));
        //        }
        //        else if (ResourcesSingleton.Instance.shopInit.shopHelpDic[1402][recharge.id][2] > 0)
        //        {
        //            ui.GetFromReference(UISubPanel_Shop_item.KImg_Left).SetActive(true);
        //            ui.GetFromReference(UISubPanel_Shop_item.KImg_Left).GetImage().SetSprite("Advert", false);
        //            ui.GetFromReference(UISubPanel_Shop_item.KText_Right).GetTextMeshPro().SetTMPText(
        //                tblanguage.Get("common_free_text").current + "("
        //                                                           + ResourcesSingleton.Instance.shopInit.shopHelpDic[
        //                                                               1402][recharge.id][2].ToString()
        //                                                           + ")");
        //            RedPointMgr.instance.SetState("ShopRoot", recharge.tagFunc.ToString() + recharge.id.ToString(),
        //                RedPointState.Show,
        //                (int)(ResourcesSingleton.Instance.shopInit.shopHelpDic[1402][recharge.id][1] +
        //                      ResourcesSingleton.Instance.shopInit.shopHelpDic[1402][recharge.id][2]));
        //        }
        //        else
        //        {
        //            RedPointMgr.instance.SetState("ShopRoot", recharge.tagFunc.ToString() + recharge.id.ToString(),
        //                RedPointState.Hide, 0);
        //        }

        //        var thisBuyBtn = ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common);
        //        IntValue intValue = new IntValue();
        //        intValue.Value = recharge.id;
        //        JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(thisBuyBtn, () =>
        //        {
        //            m1402IntHelp = moneyHelp;
        //            m1402Unit = recharge.unit;
        //            m1402PriceHelp = recharge.price;
        //            m1402IdHelp = recharge.id;
        //            m1402UI = ui;
        //            Recharge = recharge;

        //            WebMessageHandler.Instance.AddHandler(11, 4, On1402Response);
        //            NetWorkManager.Instance.SendMessage(11, 4, intValue);
        //        });
        //    }

        //    rechargeList.Clear();
        //    PosSort();
        //}


        //private void On1402Response(object sender, WebMessageHandler.Execute e)
        //{
        //    WebMessageHandler.Instance.RemoveHandler(11, 4, On1402Response);
        //    var intvalue = new IntValue();
        //    //ConfigurationLoad.ChapterIdFieldNumber=
        //    intvalue.MergeFrom(e.data);
        //    Debug.Log(e);
        //    Debug.Log(intvalue);
        //    if (e.data.IsEmpty)
        //    {
        //        Log.Debug("e.data.IsEmpty", Color.red);
        //        //return;
        //    }

        //    //ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill += m1402IntHelp;
        //    Vector3 vector3 = new Vector3();
        //    vector3.x = 3;
        //    vector3.y = 0;
        //    vector3.z = m1402IntHelp;
        //    JiYuUIHelper.AddReward(vector3, true).Forget();

        //    if (ResourcesSingleton.Instance.shopInit.shopHelpDic[1402][m1402IdHelp][1] > 0)
        //    {
        //        ResourcesSingleton.Instance.shopInit.shopHelpDic[1402][m1402IdHelp][1] -= 1;
        //    }
        //    else if (ResourcesSingleton.Instance.shopInit.shopHelpDic[1402][m1402IdHelp][2] > 0)
        //    {
        //        ResourcesSingleton.Instance.shopInit.shopHelpDic[1402][m1402IdHelp][2] -= 1;
        //    }
        //    else if (m1402Unit == 2)
        //    {
        //        ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin -= m1402PriceHelp;
        //    }

        //    if (ResourcesSingleton.Instance.shopInit.shopHelpDic[1402][m1402IdHelp][1] > 0)
        //    {
        //        m1402UI.GetFromReference(UISubPanel_Shop_item.KImg_Left).SetActive(false);
        //        m1402UI.GetFromReference(UISubPanel_Shop_item.KText_Right).GetTextMeshPro().SetTMPText(
        //            tblanguage.Get("common_free_text").current + "("
        //                                                       + ResourcesSingleton.Instance.shopInit.shopHelpDic[1402][
        //                                                           m1402IdHelp][1].ToString()
        //                                                       + ")");
        //        RedPointMgr.instance.SetState("ShopRoot", Recharge.tagFunc.ToString() + Recharge.id.ToString(),
        //            RedPointState.Show, 1);
        //    }
        //    else if (ResourcesSingleton.Instance.shopInit.shopHelpDic[1402][m1402IdHelp][2] > 0)
        //    {
        //        m1402UI.GetFromReference(UISubPanel_Shop_item.KImg_Left).SetActive(true);
        //        m1402UI.GetFromReference(UISubPanel_Shop_item.KImg_Left).GetImage().SetSprite("Advert", false);
        //        m1402UI.GetFromReference(UISubPanel_Shop_item.KText_Right).GetTextMeshPro().SetTMPText(
        //            tblanguage.Get("common_free_text").current + "("
        //                                                       + ResourcesSingleton.Instance.shopInit.shopHelpDic[1402][
        //                                                           m1402IdHelp][2].ToString()
        //                                                       + ")");
        //        RedPointMgr.instance.SetState("ShopRoot", Recharge.tagFunc.ToString() + Recharge.id.ToString(),
        //            RedPointState.Show, 1);
        //    }
        //    else if (m1402Unit == 2)
        //    {
        //        m1402UI.GetFromReference(UISubPanel_Shop_item.KImg_Left).SetActive(true);
        //        m1402UI.GetFromReference(UISubPanel_Shop_item.KImg_Left).GetImage()
        //            .SetSprite(tbuser_variable[2].icon, false);
        //        m1402UI.GetFromReference(UISubPanel_Shop_item.KText_Right).GetTextMeshPro().SetTMPText(
        //            m1402PriceHelp.ToString());
        //        RedPointMgr.instance.SetState("ShopRoot", Recharge.tagFunc.ToString() + Recharge.id.ToString(),
        //            RedPointState.Hide, 0);
        //    }

        //    ResourcesSingleton.Instance.UpdateResourceUI();
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="tagID"></param>
        //private void Init1301(int tagID)
        //{
        //    this.GetFromReference(KTitleText).GetTextMeshPro()
        //        .SetTMPText(tblanguage.Get(tbtag_func[tagID].name).current);
        //    this.GetFromReference(KDescText).SetActive(true);
        //    this.GetFromReference(KDescText).GetTextMeshPro().SetTMPText(tblanguage.Get("func_1301_desc").current);
        //    int giftsHelp = ResourcesSingleton.Instance.levelInfo.maxLockChapterID;
        //    if (giftsHelp == 0)
        //    {
        //        giftsHelp = tbchapter[1].id;
        //    }

        //    //WebMessageHandler.Instance.AddHandler(11, 1, On1301Response);
        //    //NetWorkManager.Instance.SendMessage(11, 1);
        //    int giftHelp = 0;
        //    List<gift> giftList = new List<gift>();
        //    foreach (var gift in tbgift.DataList)
        //    {
        //        //if (gift.tagFunc == 1301)
        //        //{
        //        //    if (ResourcesSingleton.Instance.shopInit.shopHelpDic.TryGetValue(1301,
        //        //            out Dictionary<int, List<long>> v))
        //        //    {
        //        //        if (ResourcesSingleton.Instance.shopInit.shopHelpDic[1301]
        //        //            .TryGetValue(gift.id, out List<long> value))
        //        //        {
        //        //            if (ResourcesSingleton.Instance.shopInit.shopHelpDic[1301][gift.id][2] > 0)
        //        //            {
        //        //                giftList.Add(gift);
        //        //                giftHelp++;
        //        //            }
        //        //        }
        //        //    }
        //        //}

        //        if (giftHelp > 10)
        //        {
        //            break;
        //        }
        //    }

        //    giftList.Sort(delegate(gift gf1, gift gf2) { return gf2.id.CompareTo(gf1.id); });


        //    foreach (var gift in giftList)
        //    {
        //        var ui = UIHelper.Create(UIType.UISubPanel_Shop_ChapterGift,
        //            this.GetFromReference(KAllItemBg).GameObject.transform, true);
        //        itemList.Add(ui);
        //        //giftHelpDic.Add(gift.id, ui);

        //        ui.GetFromReference(UISubPanel_Shop_ChapterGift.KGiftTitleText).GetTextMeshPro().SetTMPText(
        //            tblanguage.Get(gift.name).current.Replace("{0}", gift.namePara.ToString()));
        //        ui.GetFromReference(UISubPanel_Shop_ChapterGift.KText_Right).GetTextMeshPro().SetTMPText(
        //            tbprice[gift.price].rmb.ToString() + tblanguage.Get("common_coin_unit").current);
        //        ui.GetFromReference(UISubPanel_Shop_ChapterGift.KText_Mid).GetTextMeshPro().SetTMPText(
        //            (tbprice[gift.price].rmb * gift.ratio / 100).ToString() +
        //            tblanguage.Get("common_coin_unit").current);
        //        ui.GetFromReference(UISubPanel_Shop_ChapterGift.KAmazingValueText).GetTextMeshPro().SetTMPText(
        //            gift.ratio.ToString() + "%" + tblanguage.Get("gift_ratio_text").current);
        //        //ui.GetFromReference(UISubPanel_Shop_ChapterGift.KGiftImage).GetImage().SetSprite(gift.pic, false);
        //        switch (gift.opYn)
        //        {
        //            case 0:
        //                ui.GetFromReference(UISubPanel_Shop_ChapterGift.KText_Mid).SetActive(false);
        //                break;
        //            case 1:
        //                ui.GetFromReference(UISubPanel_Shop_ChapterGift.KText_Mid).SetActive(true);
        //                break;
        //            default:
        //                Debug.Log("OpYn have some bug");
        //                break;
        //        }

        //        float BgW = ui.GetFromReference(UISubPanel_Shop_ChapterGift.KGiftItemBg).GetRectTransform().Width();
        //        float reW = BgW / 6;
        //        List<Vector3> removeEmptList = JiYuUIHelper.RewardRemoveEmptiness(gift.reward);
        //        int rewardCount = removeEmptList.Count;
        //        //var parent2 = this.GetParent<UIPanel_Shop>();
        //        for (int i = 0; i < rewardCount; i++)
        //        {
        //            int ihelp = i;
        //            var uiRe = UIHelper.Create(UIType.UICommon_RewardItem, removeEmptList[ihelp],
        //                ui.GetFromReference(UISubPanel_Shop_ChapterGift.KGiftItemBg).GameObject.transform);

        //            uiRe.GetRectTransform().SetAnchoredPositionY(0);
        //            RewardList.Add(uiRe);
        //            float thisUiReW = uiRe.GetRectTransform().Width();
        //            //Debug.Log("thisUiReW" + thisUiReW.ToString());
        //            uiRe.GetRectTransform()
        //                .SetScale(new Vector2((reW - rewardW) / thisUiReW, (reW - rewardW) / thisUiReW));
        //            JiYuUIHelper.SetRewardOnClick(removeEmptList[ihelp], uiRe);
        //            //uiRe.GetFromReference(UICommon_RewardItem.KBtn_Item).GetXButton().OnClick.Add(() =>
        //            //{
        //            //    //parent2.DisableTipPanel();
        //            //    var tipUI = UIHelper.Create<Vector3>(UIType.UICommon_ItemTips, gift.reward[ihelp]) as
        //            //            UICommon_ItemTips;

        //            //    JiYuUIHelper.SetTipPos(uiRe, tipUI, UICommon_ItemTips.KContent,
        //            //        UICommon_ItemTips.KImg_Arrow);

        //            //    SetItemTipPanel(tipUI);
        //            //});
        //        }

        //        float reWHelp = reW / 2;
        //        int j = 0;
        //        //Debug.Log("BgW : " + BgW.ToString());
        //        foreach (var reward in RewardList)
        //        {
        //            //Debug.Log((-BgW / 2 + reW * j + reWHelp).ToString());
        //            reward.GetRectTransform().SetAnchoredPositionX(-BgW / 2 + reW * j + reWHelp);
        //            j++;
        //        }

        //        foreach (var reward in RewardList)
        //        {
        //            RewardHelpList.Add(reward);
        //        }

        //        RewardList.Clear();
        //        j = 0;

        //        var buyBtn = ui.GetFromReference(UISubPanel_Shop_ChapterGift.KBtn_Common);
        //        JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(buyBtn, () =>
        //        {
        //            giftId = gift.id;
        //            WebMessageHandler.Instance.AddHandler(11, 5, OnBuyGiftResponse);
        //            IntValue intValue = new IntValue();
        //            intValue.Value = gift.id;
        //            //Debug.Log(intValue.Value.ToString());
        //            NetWorkManager.Instance.SendMessage(11, 5, intValue);
        //            //UIHelper.Create(UIType.UICommon_Reward, gift.reward);
        //        });
        //    }

        //    giftList.Clear();
        //    GiftSort();
        //}

        //private void OnBuyGiftResponse(object sender, WebMessageHandler.Execute e)
        //{
        //    WebMessageHandler.Instance.RemoveHandler(11, 5, OnBuyGiftResponse);
        //    GiftResult giftResult = new GiftResult();
        //    giftResult.MergeFrom(e.data);
        //    Debug.Log(e);
        //    Debug.Log(giftResult);
        //    if (e.data.IsEmpty)
        //    {
        //        Log.Debug("e.data.IsEmpty", Color.red);
        //        return;
        //    }


        //    List<Vector3> reList = new List<Vector3>();
        //    foreach (var itemstr in giftResult.Reward)
        //    {
        //        reList.Add(UnityHelper.StrToVector3(itemstr));
        //    }

        //    UIHelper.Create(UIType.UICommon_Reward, reList);

        //    Debug.Log(giftResult.ResidueNum);

        //    var parent = this.GetParent<UIPanel_Shop>();
        //    if (giftResult.ResidueNum <= 0)
        //    {
        //        if (ResourcesSingleton.Instance.shopInit.shopHelpDic.ContainsKey(1301))
        //        {
        //            if (ResourcesSingleton.Instance.shopInit.shopHelpDic[1301].ContainsKey(giftId))
        //            {
        //                ResourcesSingleton.Instance.shopInit.shopHelpDic[1301].Remove(giftId);
        //            }
        //        }

        //        parent.DestroyTabModule();
        //        itemList.Clear();
        //        parent.InitModelBG(1301);
        //    }
        //}

        //private class ModuleLength
        //{
        //    public event ModuleChangeEventHandle ModuleChangeEvent;

        //    public void OnCall()
        //    {
        //        Debug.Log("ModuleChangeEvent");
        //        ModuleChangeEvent();
        //    }
        //}


        ///// <summary>
        ///// 生成1201模块相关
        ///// </summary>
        ///// <param name="tagID"></param>
        //private void Init1201(int tagID)
        //{
        //    var timerMgr = TimerManager.Instance;
        //    inCD12011 = false;
        //    timerId = timerMgr.StartRepeatedTimer(200, this.Update1201);
        //    this.GetFromReference(KTitleText).GetTextMeshPro()
        //        .SetTMPText(tblanguage.Get(tbtag_func[tagID].name).current);
        //    this.GetFromReference(KDescText).SetActive(true);
        //    this.GetFromReference(KDescText).GetTextMeshPro().SetTMPText(
        //        tblanguage.Get("shop_daily_countdown_text").current
        //        + JiYuUIHelper.GeneralTimeFormat(new int4(2, 3, 2, 1), TimeHelper.GetToTomorrowTime()));
        //    Init1201Help(tagID);
        //}

        //private async void Init1201Help(int tagID)
        //{
        //    int dailyShopNumber = 6;
        //    for (int i = 1; i < dailyShopNumber + 1; i++)
        //    {
        //        var ui = UIHelper.Create(UIType.UISubPanel_Shop_item,
        //            this.GetFromReference(KAllItemBg).GameObject.transform, true);
        //        itemList.Add(ui);
        //        ui.GetFromReference(UISubPanel_Shop_item.KItemTitleBg).SetActive(false);
        //        int ihelp = i;
        //        if (ResourcesSingleton.Instance.shopInit.shopHelpDic.ContainsKey(tagID))
        //        {
        //            //Debug.Log("tagID" + tagID);
        //            //Debug.Log("i" + i);
        //            int sign = (int)ResourcesSingleton.Instance.shopInit.shopHelpDic[tagID][i][0];
        //            //Debug.Log("sign" + sign);
        //            int times = (int)ResourcesSingleton.Instance.shopInit.shopHelpDic[tagID][i][1];
        //            long upTime = ResourcesSingleton.Instance.shopInit.shopHelpDic[tagID][i][2];
        //            if (tbshop_Daily.DataMap.ContainsKey(sign))
        //            {
        //                foreach (var r in tbshop_Daily[sign].reward)
        //                {
        //                    var ReUi = UIHelper.Create(UIType.UICommon_RewardItem, r,
        //                        ui.GetFromReference(UISubPanel_Shop_item.KRewardItemPos).GameObject.transform);
        //                    ui.GetFromReference(UISubPanel_Shop_item.KRewardText).GetTextMeshPro()
        //                        .SetTMPText(JiYuUIHelper.GetRewardName(r));
        //                }

        //                ui.GetFromReference(UISubPanel_Shop_item.KRewardText).SetActive(true);
        //                ui.GetFromReference(UISubPanel_Shop_item.KItemTitleBg).SetActive(false);
        //                //判断折扣显示
        //                if (tbshop_Daily[sign].discount != 0)
        //                {
        //                    ui.GetFromReference(UISubPanel_Shop_item.KCutText).GetTextMeshPro().SetTMPText(
        //                        tbshop_Daily[sign].discount.ToString() +
        //                        tblanguage.Get("shop_daily_discount_text").current);
        //                    if (tbshop_Daily[sign].discountUi != "")
        //                    {
        //                        await ui.GetFromReference(UISubPanel_Shop_item.KCutBg).GetImage()
        //                            .SetSpriteAsync(tbshop_Daily[sign].discountUi, false);
        //                    }

        //                    ui.GetFromReference(UISubPanel_Shop_item.KCutBg).SetActive(true);
        //                }
        //                else
        //                {
        //                    ui.GetFromReference(UISubPanel_Shop_item.KCutBg).SetActive(false);
        //                }

        //                if (times < tbshop_Daily[sign].times)
        //                {
        //                    if (tbshop_Daily[sign].pos == 1)
        //                    {
        //                        RedPointMgr.instance.Init("ShopRoot", "12011",
        //                            async (RedPointState state, int data) =>
        //                            {
        //                                if (JiYuUIHelper.TryGetUI(UIType.UISubPanel_Shop_Tag1, out UI uui))
        //                                {
        //                                    if (state == RedPointState.Show)
        //                                    {
        //                                        await ui.GetImage().SetSpriteAsync("bg_ShopItemGreen", false);
        //                                    }
        //                                    else
        //                                    {
        //                                        await ui.GetImage().SetSpriteAsync("bg_ShopItem", false);
        //                                    }
        //                                }
        //                            }, ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common).GetXButton());
        //                        //位置1
        //                        cd12011ui = ui;
        //                        upTime12011 = upTime;
        //                        tagID12011 = tagID;
        //                        i12011 = i;
        //                        if (TimeHelper.ClientNowSeconds() - upTime >
        //                            tbconstant.Get("shop_daily_ad_cd").constantValue)
        //                        {
        //                            //不在CD
        //                            RedPointMgr.instance.SetState("ShopRoot", "12011", RedPointState.Show);
        //                            inCD12011 = false;
        //                            ui.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(false);
        //                            //判断是广告免费还是免费
        //                            if (times < tbshop_Daily[sign].freeRule[0][0] - tbshop_Daily[sign].freeRule[0][1])
        //                            {
        //                                //免费
        //                                if (tbshop_Daily[sign].freeRule[0][0] - tbshop_Daily[sign].freeRule[0][1] -
        //                                    times > 1)
        //                                {
        //                                    ui.GetFromReference(UISubPanel_Shop_item.KText_Right).GetTextMeshPro()
        //                                        .SetTMPText(tblanguage.Get("common_free_text").current + "(" +
        //                                                    (tbshop_Daily[sign].freeRule[0][0] -
        //                                                     tbshop_Daily[sign].freeRule[0][1] - times).ToString()
        //                                                    + ")");
        //                                }
        //                                else
        //                                {
        //                                    ui.GetFromReference(UISubPanel_Shop_item.KText_Right).GetTextMeshPro()
        //                                        .SetTMPText(tblanguage.Get("common_free_text").current);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                //广告免费
        //                                ui.GetFromReference(UISubPanel_Shop_item.KImg_Left).GetImage()
        //                                    .SetSprite("Advert", false);
        //                                ui.GetFromReference(UISubPanel_Shop_item.KImg_Left).SetActive(true);
        //                                if (tbshop_Daily[sign].freeRule[0][1] -
        //                                    (times - (tbshop_Daily[sign].freeRule[0][0] -
        //                                              tbshop_Daily[sign].freeRule[0][1])) > 1)
        //                                {
        //                                    ui.GetFromReference(UISubPanel_Shop_item.KText_Right).GetTextMeshPro()
        //                                        .SetTMPText(tblanguage.Get("common_free_text").current + "(" +
        //                                                    (tbshop_Daily[sign].freeRule[0][1]
        //                                                     - (times - (tbshop_Daily[sign].freeRule[0][0] -
        //                                                                 tbshop_Daily[sign].freeRule[0][1]))).ToString()
        //                                                    + ")");
        //                                }
        //                                else
        //                                {
        //                                    ui.GetFromReference(UISubPanel_Shop_item.KText_Right).GetTextMeshPro()
        //                                        .SetTMPText(tblanguage.Get("common_free_text").current);
        //                                }
        //                            }

        //                            ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(true);
        //                        }
        //                        else
        //                        {
        //                            //在CD
        //                            RedPointMgr.instance.SetState("ShopRoot", "12011", RedPointState.Hide);
        //                            long cdTime = tbconstant.Get("shop_daily_ad_cd").constantValue -
        //                                TimeHelper.ClientNowSeconds() + upTime;
        //                            //cdTime = -cdTime;
        //                            ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(false);
        //                            ui.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(true);
        //                            ui.GetFromReference(UISubPanel_Shop_item.KStateText).GetTextMeshPro()
        //                                .SetTMPText(JiYuUIHelper.GeneralTimeFormat(new int4(1, 2, 1, 1), cdTime) + "\n"
        //                                    + tblanguage.Get("shop_daily_1_countdown_text").current);
        //                            inCD12011 = true;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //除了1之外的其他位置
        //                        ui.GetFromReference(UISubPanel_Shop_item.KText_Right).GetTextMeshPro()
        //                            .SetTMPText(tbshop_Daily[sign].cost[0][2].ToString());
        //                        if (tbshop_Daily[sign].cost[0][0] == 3)
        //                        {
        //                            //消耗游戏币
        //                            ui.GetFromReference(UISubPanel_Shop_item.KImg_Left).GetImage()
        //                                .SetSprite(tbuser_variable[3].icon, false);
        //                        }

        //                        if (tbshop_Daily[sign].cost[0][0] == 2)
        //                        {
        //                            //消耗充值货币
        //                            ui.GetFromReference(UISubPanel_Shop_item.KImg_Left).GetImage()
        //                                .SetSprite(tbuser_variable[2].icon, false);
        //                        }

        //                        ui.GetFromReference(UISubPanel_Shop_item.KImg_Left).SetActive(true);
        //                        ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(true);
        //                    }
        //                }
        //                else
        //                {
        //                    //没有购买次数
        //                    if (tbshop_Daily[sign].pos == 1)
        //                    {
        //                        //当前位置位1
        //                        RedPointMgr.instance.SetState("ShopRoot", "12011", RedPointState.Hide);
        //                        ui.GetFromReference(UISubPanel_Shop_item.KStateText).GetTextMeshPro().SetTMPText(
        //                            tblanguage.Get("shop_daily_gained_text").current);
        //                    }
        //                    else
        //                    {
        //                        //当前位置为除了1之外的位置
        //                        ui.GetFromReference(UISubPanel_Shop_item.KStateText).GetTextMeshPro().SetTMPText(
        //                            tblanguage.Get("shop_daily_purchased_text").current);
        //                    }

        //                    ui.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(true);
        //                    ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(false);
        //                }
        //            }

        //            var BuyBtn = ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common);
        //            //int ihelp = i;
        //            if (ihelp != 1)
        //            {
        //                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(BuyBtn, () =>
        //                {
        //                    var uis = UIHelper.Create(UIType.UICommon_Sec_Confirm);
        //                    uis.GetFromReference(UICommon_Sec_Confirm.KText_Return).GetTextMeshPro()
        //                        .SetTMPText(tblanguage.Get("common_state_cancel").current);
        //                    uis.GetFromReference(UICommon_Sec_Confirm.KText_Buy).GetTextMeshPro()
        //                        .SetTMPText(tblanguage.Get("common_state_buy").current);
        //                    string textHelp = tblanguage.Get("text_purchase_confirm").current;
        //                    string rewardHelp = "";
        //                    foreach (var r in tbshop_Daily[sign].reward)
        //                    {
        //                        rewardHelp += JiYuUIHelper.GetRewardName(r);
        //                    }

        //                    if (tbshop_Daily[sign].cost[0][0] == 3)
        //                    {
        //                        textHelp = textHelp.Replace("{0}",
        //                            "<sprite=1>" + UnityHelper.RichTextColor(tbshop_Daily[sign].cost[0][2].ToString(),
        //                                "475569"));
        //                    }

        //                    if (tbshop_Daily[sign].cost[0][0] == 2)
        //                    {
        //                        textHelp = textHelp.Replace("{0}",
        //                            "<sprite=0>" + UnityHelper.RichTextColor(tbshop_Daily[sign].cost[0][2].ToString(),
        //                                "475569"));
        //                    }

        //                    textHelp = textHelp.Replace("{1}", rewardHelp);
        //                    uis.GetFromReference(UICommon_Sec_Confirm.KText).GetTextMeshPro().SetTMPText(textHelp);
        //                    var secBuyBtn = uis.GetFromReference(UICommon_Sec_Confirm.KBtn_Buy);
        //                    JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(secBuyBtn, () =>
        //                    {
        //                        IntValue intValue = new IntValue();
        //                        intValue.Value = sign;
        //                        pos1201 = ihelp;
        //                        secUI = uis;
        //                        WebMessageHandler.Instance.AddHandler(CMD.SHOP1201, OnDailyBuyResponse);
        //                        NetWorkManager.Instance.SendMessage(CMD.SHOP1201, intValue);
        //                    });
        //                });
        //            }
        //            else
        //            {
        //                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(BuyBtn, () =>
        //                {
        //                    IntValue intValue = new IntValue();
        //                    intValue.Value = sign;
        //                    pos1201 = ihelp;
        //                    secUI = null;
        //                    WebMessageHandler.Instance.AddHandler(CMD.SHOP1201, OnDailyBuyResponse);
        //                    NetWorkManager.Instance.SendMessage(CMD.SHOP1201, intValue);
        //                });
        //            }
        //        }
        //    }

        //    PosSort();
        //    await UniTask.Delay(1000 * (int)TimeHelper.GetToTomorrowTime());
        //    if (JiYuUIHelper.TryGetUI(UIType.UISubPanel_Shop_Tag1, out UI ui1))
        //    {
        //        //发送查询
        //        WebMessageHandler.Instance.AddHandler(11, 6, OnDailyShopResponse);
        //        NetWorkManager.Instance.SendMessage(11, 6);
        //    }
        //}

        //private async void JiyuRedPointHelp1201()
        //{
        //    await UniTask.Delay(1000 * ((int)tbconstant.Get("shop_daily_ad_cd").constantValue + 1));
        //    ResourcesSingleton.Instance.UpdateResourceUI();
        //    Debug.Log("are you ready?");
        //}

        //private void OnDailyBuyResponse(object sender, WebMessageHandler.Execute e)
        //{
        //    WebMessageHandler.Instance.RemoveHandler(CMD.SHOP1201, OnDailyBuyResponse);
        //    StringValue stringValue = new StringValue();
        //    stringValue.MergeFrom(e.data);
        //    Debug.Log(e);
        //    Debug.Log(stringValue);
        //    if (e.data.IsEmpty)
        //    {
        //        Log.Debug("e.data.IsEmpty", Color.red);
        //        return;
        //    }

        //    if (stringValue.Value == "true")
        //    {
        //        if (secUI != null)
        //        {
        //            secUI.Dispose();
        //        }

        //        int sign = (int)ResourcesSingleton.Instance.shopInit.shopHelpDic[1201][pos1201][0];
        //        if (secUI != null)
        //        {
        //            List<Vector3> vector3s = new List<Vector3>();
        //            foreach (var r in tbshop_Daily[sign].reward)
        //            {
        //                //JiYuUIHelper.AddReward(r, true);
        //                vector3s.Add(r);
        //            }

        //            UIHelper.Create(UIType.UICommon_Reward, vector3s);
        //            vector3s.Clear();
        //        }
        //        else
        //        {
        //            foreach (var r in tbshop_Daily[sign].reward)
        //            {
        //                JiYuUIHelper.AddReward(r, true);
        //            }
        //        }

        //        foreach (var c in tbshop_Daily[sign].cost)
        //        {
        //            JiYuUIHelper.TryReduceReward(c);
        //        }

        //        ResourcesSingleton.Instance.shopInit.shopHelpDic[1201][pos1201][1] += 1;
        //        ResourcesSingleton.Instance.shopInit.shopHelpDic[1201][pos1201][2] = TimeHelper.ClientNow() / 1000;
        //        inCD12011 = false;
        //        DestroyItem();
        //        Init1201Help(1201);
        //        ResourcesSingleton.Instance.UpdateResourceUI();
        //        JiyuRedPointHelp1201();
        //    }
        //}

        //private void OnDailyShopResponse(object sender, WebMessageHandler.Execute e)
        //{
        //    WebMessageHandler.Instance.RemoveHandler(11, 6, OnDailyShopResponse);
        //    ByteValueList dailyBuys = new ByteValueList();
        //    dailyBuys.MergeFrom(e.data);
        //    Debug.Log(e);
        //    Debug.Log(dailyBuys);
        //    if (e.data.IsEmpty)
        //    {
        //        Log.Debug("e.data.IsEmpty", Color.red);
        //        return;
        //    }

        //    foreach (var d in dailyBuys.Values)
        //    {
        //        //ResourcesSingleton.Instance.shopInit.shopHelpDic[1201][];
        //        DailyBuy daily = new DailyBuy();
        //        daily.MergeFrom(d);
        //        List<long> dailyHelp = new List<long>();
        //        dailyHelp.Add(daily.Sign);
        //        dailyHelp.Add(daily.BuyCount);
        //        dailyHelp.Add(daily.UpTime);
        //        ResourcesSingleton.Instance.shopInit.shopHelpDic[1201][tbshop_Daily.Get(daily.Sign).pos] = dailyHelp;
        //    }

        //    if (JiYuUIHelper.TryGetUI(UIType.UISubPanel_Shop_Tag1, out UI uuii))
        //    {
        //        inCD12011 = false;
        //        DestroyItem();
        //        Init1201Help(1201);
        //    }

        //    ResourcesSingleton.Instance.UpdateResourceUI();
        //}

        //private void Update1201()
        //{
        //    long timeS = TimeHelper.GetToTomorrowTime();
        //    string timeStr = JiYuUIHelper.GeneralTimeFormat(new int4(2, 3, 2, 1), timeS);
        //    this.GetFromReference(KDescText).GetTextMeshPro()
        //        .SetTMPText(tblanguage.Get("shop_daily_countdown_text").current + timeStr);
        //    if (inCD12011)
        //    {
        //        long cdTime = tbconstant.Get("shop_daily_ad_cd").constantValue - TimeHelper.ClientNowSeconds() +
        //                      upTime12011;
        //        if (cdTime > 0)
        //        {
        //            cd12011ui.GetFromReference(UISubPanel_Shop_item.KStateText).GetTextMeshPro()
        //                .SetTMPText(JiYuUIHelper.GeneralTimeFormat(new int4(1, 2, 1, 1), cdTime)
        //                            + "\n" + tblanguage.Get("shop_daily_1_countdown_text").current);
        //        }
        //        else
        //        {
        //            inCD12011 = false;
        //            int sign = (int)ResourcesSingleton.Instance.shopInit.shopHelpDic[tagID12011][i12011][0];
        //            int times = (int)ResourcesSingleton.Instance.shopInit.shopHelpDic[tagID12011][i12011][1];
        //            //判断一下
        //            if (times < tbshop_Daily[sign].times)
        //            {
        //                RedPointMgr.instance.SetState("ShopRoot", "12011", RedPointState.Show);
        //                ResourcesSingleton.Instance.UpdateResourceUI();
        //                cd12011ui.GetFromReference(UISubPanel_Shop_item.KStateText).SetActive(false);
        //                if (times < tbshop_Daily[sign].freeRule[0][0] - tbshop_Daily[sign].freeRule[0][1])
        //                {
        //                    //免费
        //                    if (tbshop_Daily[sign].freeRule[0][0] - tbshop_Daily[sign].freeRule[0][1] - times > 1)
        //                    {
        //                        cd12011ui.GetFromReference(UISubPanel_Shop_item.KText_Right).GetTextMeshPro()
        //                            .SetTMPText(tblanguage.Get("common_free_text").current + "(" +
        //                                        (tbshop_Daily[sign].freeRule[0][0] - tbshop_Daily[sign].freeRule[0][1] -
        //                                         times).ToString()
        //                                        + ")");
        //                    }
        //                    else
        //                    {
        //                        cd12011ui.GetFromReference(UISubPanel_Shop_item.KText_Right).GetTextMeshPro()
        //                            .SetTMPText(tblanguage.Get("common_free_text").current);
        //                    }
        //                }
        //                else
        //                {
        //                    //广告免费
        //                    cd12011ui.GetFromReference(UISubPanel_Shop_item.KImg_Left).GetImage()
        //                        .SetSprite("Advert", false);
        //                    cd12011ui.GetFromReference(UISubPanel_Shop_item.KImg_Left).SetActive(true);
        //                    if (tbshop_Daily[sign].freeRule[0][1] -
        //                        (times - (tbshop_Daily[sign].freeRule[0][0] - tbshop_Daily[sign].freeRule[0][1])) > 1)
        //                    {
        //                        cd12011ui.GetFromReference(UISubPanel_Shop_item.KText_Right).GetTextMeshPro()
        //                            .SetTMPText(tblanguage.Get("common_free_text").current + "(" +
        //                                        (tbshop_Daily[sign].freeRule[0][1]
        //                                         - (times - (tbshop_Daily[sign].freeRule[0][0] -
        //                                                     tbshop_Daily[sign].freeRule[0][1]))).ToString()
        //                                        + ")");
        //                    }
        //                    else
        //                    {
        //                        cd12011ui.GetFromReference(UISubPanel_Shop_item.KText_Right).GetTextMeshPro()
        //                            .SetTMPText(tblanguage.Get("common_free_text").current);
        //                    }
        //                }

        //                cd12011ui.GetFromReference(UISubPanel_Shop_item.KBtn_Common).SetActive(true);
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// 根据位置进行位置
        ///// </summary>
        //private void PosSort()
        //{
        //    float itemH = 0;
        //    float itemW = 0;
        //    int itemListCount = itemList.Count;
        //    if (itemListCount > 0)
        //    {
        //        itemH = itemList[0].GetRectTransform().Height();
        //        itemW = itemList[0].GetRectTransform().Width();
        //    }

        //    float BgW = this.GetFromReference(KAllItemBg).GetRectTransform().Width();
        //    for (int i = 0; i < itemListCount; i++)
        //    {
        //        int itemY = i / 3;
        //        int itemX = i % 3;
        //        float thisItemH = itemY * (itemH + itemDeltaH);
        //        itemList[i].GetRectTransform().SetAnchoredPositionY(-thisItemH);
        //        switch (itemX)
        //        {
        //            case 0:
        //                itemList[i].GetRectTransform().SetAnchoredPositionX(-(BgW - itemW) / 2);
        //                break;
        //            case 1:
        //                itemList[i].GetRectTransform().SetAnchoredPositionX(0);
        //                break;
        //            case 2:
        //                itemList[i].GetRectTransform().SetAnchoredPositionX((BgW - itemW) / 2);
        //                break;
        //            default:
        //                Debug.Log("Something is wrong.");
        //                break;
        //        }
        //    }

        //    this.GetFromReference(KAllItemBg).GetRectTransform()
        //        .SetHeight(math.ceil((float)itemListCount / (float)3) * (itemH + itemDeltaH));
        //    this.GetFromReference(KAllItemBg).GetRectTransform().SetOffsetWithLeft(25);
        //    this.GetFromReference(KAllItemBg).GetRectTransform().SetOffsetWithRight(-25);
        //    this.GetRectTransform()
        //        .SetHeight(math.ceil((float)itemListCount / (float)3) * (itemH + itemDeltaH) + titleH);
        //    this.GetRectTransform().SetOffsetWithLeft(0);
        //    this.GetRectTransform().SetOffsetWithRight(0);
        //}

        ///// <summary>
        ///// 礼包根据位置进行排序
        ///// </summary>
        //private void GiftSort()
        //{
        //    float itemH = 0;
        //    int itemListCount = itemList.Count;
        //    if (itemListCount > 0)
        //    {
        //        itemH = itemList[0].GetRectTransform().Height();
        //    }

        //    for (int i = 0; i < itemListCount; i++)
        //    {
        //        int itemY = i;
        //        float thisItemH = itemY * (itemH + itemDeltaH);
        //        itemList[i].GetRectTransform().SetAnchoredPositionY(-thisItemH);
        //    }

        //    this.GetFromReference(KAllItemBg).GetRectTransform()
        //        .SetHeight(math.ceil((float)itemListCount / (float)3) * (itemH + itemDeltaH));
        //    this.GetFromReference(KAllItemBg).GetRectTransform().SetOffsetWithLeft(10);
        //    this.GetFromReference(KAllItemBg).GetRectTransform().SetOffsetWithRight(-10);
        //    this.GetRectTransform().SetHeight(itemListCount * (itemH + itemDeltaH) + titleH);
        //    this.GetRectTransform().SetOffsetWithLeft(0);
        //    this.GetRectTransform().SetOffsetWithRight(0);
        //}


        ///// <summary>
        ///// 销毁内部的item
        ///// </summary>
        //private void DestroyItem()
        //{
        //    int itemListCount = itemList.Count;
        //    for (int i = 0; i < itemListCount; i++)
        //    {
        //        UnityEngine.GameObject.Destroy(itemList[i].GameObject);
        //    }

        //    itemList.Clear();
        //}

        //public void RemoveTimer()
        //{
        //    if (this.timerId != 0)
        //    {
        //        //Debug.Log("RemoveTimer");
        //        var timerMgr = TimerManager.Instance;
        //        timerMgr?.RemoveTimerId(ref this.timerId);
        //        this.timerId = 0;
        //    }
        //}

        #endregion

        protected override void OnClose()
        {
            //DestroyItem();
            //this.GetFromReference(KAllItemBg).GetList().Clear();
            //this.GetFromReference(KPos_Layout).GetList().Clear();
            base.OnClose();
        }
    }
}