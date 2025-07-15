using cfg.config;
using Common;
using HotFix_UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XFramework;

public class EquipMaterialItemTest : MonoBehaviour
{
    public Button ItemBtn;
    public Image itemBG;
    public Image ItemIcon;
    public TextMeshProUGUI itemCountText;


    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI DesText;
    public Image ImageMask;

    public bool isAdd = false;


    public long UID;

    public long GoldNum = 0;
    public int DrawingPos = 0;
    public long DrawingNum = 0;

    //tips面板
    public GameObject TipPanel;
    private bool isTipTitleVisible = false;

    // Start is called before the first frame update
    void Start()
    {
        TipPanel.SetActive(false);

        //UICompound.deliveryPanel += AddXFrameUI;

        //Debug.Log(UID + "uid");
        ItemBtn?.onClick.AddListener(() =>
        {
            //装备面板
            if (EquipItemBtnTest.EquipmentPanel)
            {
                OnMouseDown();
                //状态取反
                //var TipPos = TipPanel.gameObject.activeSelf;
                //TipPanel.gameObject.SetActive(!TipPos);
            }
            else
            {
                if (!isAdd && EquipitemCache.CompoundIndexNum <= EquipitemCache.CompoundTotalNum)
                {
                    //合成面板
                    var newItem = Instantiate(this.gameObject);
                    newItem.GetComponent<RectTransform>().GetChild(0).GetComponent<XButton>().enabled = true;
                    var RectCompoment = newItem.GetComponent<RectTransform>();
                    RectCompoment.GetComponent<EquipMaterialItemTest>().isAdd = true;
                    this.isAdd = true;
                    RectCompoment.SetAnchorMin(new Vector2(0.5f, 0.5f));
                    RectCompoment.SetAnchorMax(new Vector2(0.5f, 0.5f));
                    RectCompoment.SetPivot(new Vector2(0.5f, 0.5f));
                    RectCompoment.SetSize(new Vector2(170, 170));
                    //把自己打勾
                    var maskObj = this.GetComponent<EquipMaterialItemTest>().ImageMask.gameObject;
                    maskObj.SetActive(true);
                    EquipitemCache.AddToCompoundListEquip(newItem, true);

                    var ItemCompoundStateobj = maskObj.GetComponent<RectTransform>().GetChild(0).gameObject;
                    ItemCompoundStateobj.GetComponent<XImage>().sprite =
                        ResourcesManager.LoadAsset<Sprite>("GameTask_Get");

                    GameObject fatherobj = GameObject.Find("CompoundPanel(Clone)");

                    //移动动画
                    DoMoveItem(fatherobj, newItem, EquipitemCache.CompoundTotalNum, EquipitemCache.CompoundIndexNum);

                    //this.ItemBtn.gameObject.GetComponent<XButton>().enabled=false;
                    EquipitemCache.CompoundIndexNum = EquipitemCache.CompoundListEquip.Count;
                    //EquipitemCache.CompoundIndexNum++;
                }
                else if (isAdd)
                {
                    //改变索引
                    EquipitemCache.CompoundIndexNum = EquipitemCache.indexOfCompoundMaterilList(this.gameObject);

                    RelashMaterialItem?.Invoke(UID, false);
                    EquipitemCache.removeCompoundListEquip(this.gameObject, true);
                    var maskObj = this.GetComponent<EquipMaterialItemTest>().ImageMask.gameObject;

                    //销毁当前这个对象
                    if (this.gameObject.GetComponent<RectTransform>().anchoredPosition.Equals(Vector2.zero))
                    {
                        Destroy(this.gameObject);
                        EquipMaterialItemTest[] allItems = FindObjectsOfType<EquipMaterialItemTest>();
                        foreach (var item in allItems)
                        {
                            if (item.UID.Equals(this.UID))
                            {
                                item.isAdd = false;
                            }
                        }
                    }
                    else
                    {
                        EquipMaterialItemTest[] allItems = FindObjectsOfType<EquipMaterialItemTest>();
                        foreach (var item in allItems)
                        {
                            if (item.UID.Equals(this.UID) && item.GetComponent<RectTransform>().anchoredPosition
                                    .Equals(Vector2.zero))
                            {
                                GameObject.Destroy(item.gameObject);
                                break;
                            }
                        }
                    }

                    this.isAdd = false;
                }
            }
        });
    }


    private void OnMouseDown()
    {
        // 切换tip title的显示与隐藏状态
        isTipTitleVisible = !isTipTitleVisible;

        // 隐藏其他item的tip title
        EquipMaterialItemTest[] allItems = FindObjectsOfType<EquipMaterialItemTest>();
        foreach (EquipMaterialItemTest item in allItems)
        {
            if (item != this)
            {
                item.HideTipTitle();
            }
        }

        // 显示或隐藏当前item的tip title
        if (isTipTitleVisible)
        {
            ShowTipTitle();
        }
        else
        {
            HideTipTitle();
        }
    }

    private void ShowTipTitle()
    {
        TipPanel.SetActive(true);
    }

    private void HideTipTitle()
    {
        TipPanel.SetActive(false);
    }


    private void DoMoveItem(GameObject fatherobj, GameObject obj, int TotolNum, int indexNum)
    {
        RectTransform fatherTransform;
        switch (indexNum)
        {
            case 0:
            {
                break;
            }

            case 1:
            {
                //如果总量是1
                if (TotolNum.Equals(1))
                {
                    fatherTransform = fatherobj.GetComponent<RectTransform>().GetChild(2).GetChild(2).GetChild(0)
                        .GetComponent<RectTransform>();
                    obj.GetComponent<RectTransform>().parent = fatherTransform;
                    obj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(1000, 0, 0);
                }
                else //如果总量是2
                {
                    fatherTransform = fatherobj.GetComponent<RectTransform>().GetChild(2).GetChild(3).GetChild(0)
                        .GetComponent<RectTransform>();
                    obj.GetComponent<RectTransform>().parent = fatherTransform;
                    obj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(1000, 0, 0);
                }

                break;
            }

            case 2:
            {
                fatherTransform = fatherobj.GetComponent<RectTransform>().GetChild(2).GetChild(3).GetChild(1)
                    .GetComponent<RectTransform>();
                obj.GetComponent<RectTransform>().parent = fatherTransform;
                obj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(1400, 0, 0);
                break;
            }
        }


        obj.GetComponent<RectTransform>().SetScale(new Vector3(1, 1, 1));
        // obj.GetComponent<RectTransform>().DOAnchorPos3DX(0, 0.5f).OnComplete(() =>
        // {
        //     if (indexNum >= TotolNum)
        //     {
        //         fatherobj.GetComponent<RectTransform>().GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject
        //             .SetActive(true);
        //     }
        // });
    }


    //初始化通用合成材料函数,这个是在装备面板
    public void InitMaterialItem(GameEquip Materialitem, int number)
    {
        Tbequip_data MaterialitemInfo = ConfigManager.Instance.Tables.Tbequip_data;
        Tbequip_quality MaterialitemQuailyInfo = ConfigManager.Instance.Tables.Tbequip_quality;
        Tbquality tbquality = ConfigManager.Instance.Tables.Tbquality;
        Tblanguage tblanguage = ConfigManager.Instance.Tables.Tblanguage;

        //Debug.Log(Materialitem.EquipId+"222" + Materialitem.Quality + "111");
        //设置通用合成材料图片
        ItemIcon.sprite =
            ResourcesManager.LoadAsset<Sprite>(MaterialitemInfo.Get(Materialitem.EquipId).icon);

        var typeId = MaterialitemQuailyInfo.Get(Materialitem.Quality).type;

        //设置通用合成材料的底部图片
        itemBG.sprite = ResourcesManager.LoadAsset<Sprite>(tbquality.Get(typeId).frame);
        //设置数量
        itemCountText.text = "x " + number.ToString();

        string MaterialName = tblanguage.Get(MaterialitemInfo.Get(Materialitem.EquipId).name)
            .current;
        string tempString0 = MaterialName.Substring(0, 7);
        //拿到颜色字
        string tempString = MaterialName.Substring(7, 1);


        if (MaterialitemInfo.Get(Materialitem.EquipId).quality >= 4 &&
            MaterialitemInfo.Get(Materialitem.EquipId).quality <= 6)
        {
            tempString = "<color=#800080>" + tempString + "</color>";
        }
        else if (MaterialitemInfo.Get(Materialitem.EquipId).quality >= 7 &&
                 MaterialitemInfo.Get(Materialitem.EquipId).quality <= 10)
        {
            tempString = "<color=#FFA500>" + tempString + "</color>";
        }
        else if (MaterialitemInfo.Get(Materialitem.EquipId).quality >= 11 &&
                 MaterialitemInfo.Get(Materialitem.EquipId).quality <= 15)
        {
            tempString = "<color=#FF0000>" + tempString + "</color>";
        }

        //设置tips
        TitleText.text = tempString0 + tempString;
        //设置描述
        DesText.text = tblanguage.Get(MaterialitemInfo.Get(Materialitem.EquipId).desc).current;
    }

    //初始化图纸item,装备面板
    public void InitDrawItem(int DrawItemId, long number)
    {
        var tbitem = ConfigManager.Instance.Tables.Tbitem;
        var tblanguage = ConfigManager.Instance.Tables.Tblanguage;
        itemBG.sprite = ResourcesManager.LoadAsset<Sprite>("pic_quality_frame_1");
        ItemIcon.sprite = ResourcesManager.LoadAsset<Sprite>(tbitem.Get(DrawItemId).icon);
        //设置数量
        itemCountText.text = "x " + number.ToString();

        //图纸信息
        TitleText.text = tblanguage.Get(tbitem.Get(DrawItemId).name).current;
        DesText.text = tblanguage.Get(tbitem.Get(DrawItemId).desc).current;
    }


    //初始化通用合成材料,合成面板
    public void InitMaterialItem2(GameEquip Materialitem, long uid)
    {
        this.UID = uid;
        Tbequip_data MaterialitemInfo = ConfigManager.Instance.Tables.Tbequip_data;
        Tbequip_quality MaterialitemQuailyInfo = ConfigManager.Instance.Tables.Tbequip_quality;
        Tbquality tbquality = ConfigManager.Instance.Tables.Tbquality;
        var typeId = MaterialitemQuailyInfo.Get(Materialitem.Quality).type;
        //设置通用合成材料图片
        ItemIcon.sprite =
            ResourcesManager.LoadAsset<Sprite>(MaterialitemInfo.Get(Materialitem.EquipId).icon);

        //设置通用合成材料的底部图片
        itemBG.sprite = ResourcesManager.LoadAsset<Sprite>(tbquality.Get(typeId).frame);

        //设置数量不显示
        itemCountText.gameObject.SetActive(false);
    }

    //初始化得到的金币数量,这个是在降级降品面板
    public void InitGoldDecrease(GameEquip equip)
    {
        var equip_level = ConfigManager.Instance.Tables.Tbequip_level;
        var pos = equip.PosId;
        float consume = 0;
        //拿到对应等级的索引
        int indexLevelCost = equip_level.DataList.IndexOf(equip_level.Get(equip.Level)) + 1;

        switch (pos)
        {
            //计算总消耗
            case 1:
            {
                for (int i = 1; i < indexLevelCost; i++)
                {
                    consume += equip_level.DataList[i].cost1[0].z;
                }
            }
                break;
            case 2:
            {
                for (int i = 1; i < indexLevelCost; i++)
                {
                    consume += equip_level.DataList[i].cost2[0].z;
                }
            }
                break;
            case 3:
            {
                for (int i = 1; i < indexLevelCost; i++)
                {
                    consume += equip_level.DataList[i].cost3[0].z;
                }
            }
                break;
            case 4:
            {
                for (int i = 1; i < indexLevelCost; i++)
                {
                    consume += equip_level.DataList[i].cost4[0].z;
                }
            }
                break;
            case 5:
            {
                for (int i = 1; i < indexLevelCost; i++)
                {
                    consume += equip_level.DataList[i].cost5[0].z;
                }
            }
                break;
            case 6:
            {
                for (int i = 1; i < indexLevelCost; i++)
                {
                    consume += equip_level.DataList[i].cost6[0].z;
                }
            }
                break;
        }


        GoldNum = (long)consume;

        //显示金币
        ItemIcon.sprite = ResourcesManager.LoadAsset<Sprite>("icon_money");

        //显示底色
        itemBG.sprite = ResourcesManager.LoadAsset<Sprite>("pic_quality_frame_3");

        //显示数字
        itemCountText.text = "x " + numsLimit(consume);
    }

    //初始化图纸得到的数量,这个也是在降级降品面板
    public void InitDrawingDecrease(GameEquip equip)
    {
        var equip_level = ConfigManager.Instance.Tables.Tbequip_level;
        var DrawingItem = ConfigManager.Instance.Tables.Tbitem;
        var pos = equip.PosId;
        //拿到对应的图纸
        ItemIcon.sprite = ResourcesManager.LoadAsset<Sprite>(DrawingItem.Get(1020000 + pos).icon);
        //图纸底框
        itemBG.sprite = ResourcesManager.LoadAsset<Sprite>("pic_quality_frame_1");
        //计算数量
        float consume = 0;
        //拿到对应等级的索引
        int indexLevelCost = equip_level.DataList.IndexOf(equip_level.Get(equip.Level)) + 1;

        switch (pos)
        {
            //计算总消耗
            case 1:
            {
                for (int i = 1; i < indexLevelCost; i++)
                {
                    consume += equip_level.DataList[i].cost1[1].z;
                }
            }
                break;
            case 2:
            {
                for (int i = 1; i < indexLevelCost; i++)
                {
                    consume += equip_level.DataList[i].cost2[1].z;
                }
            }
                break;
            case 3:
            {
                for (int i = 1; i < indexLevelCost; i++)
                {
                    consume += equip_level.DataList[i].cost3[1].z;
                }
            }
                break;
            case 4:
            {
                for (int i = 1; i < indexLevelCost; i++)
                {
                    consume += equip_level.DataList[i].cost4[1].z;
                }
            }
                break;
            case 5:
            {
                for (int i = 1; i < indexLevelCost; i++)
                {
                    consume += equip_level.DataList[i].cost5[1].z;
                }
            }
                break;
            case 6:
            {
                for (int i = 1; i < indexLevelCost; i++)
                {
                    consume += equip_level.DataList[i].cost6[1].z;
                }
            }
                break;
        }

        DrawingPos = 1020000 + pos;
        DrawingNum = (long)consume;

        //显示数字
        itemCountText.text = "x " + numsLimit(consume);
    }

    //初始化通用合成材料得到的数量,这个在降品页面
    public void InitMaterialDecrease(GameEquip equip)
    {
        var equip_quailty = ConfigManager.Instance.Tables.Tbequip_quality;
        var equip_Data = ConfigManager.Instance.Tables.Tbequip_data;
        Tbquality tbquality = ConfigManager.Instance.Tables.Tbquality;

        var typeid = equip_quailty.Get(equip_Data.Get(equip.PosId * 100).quality).type;
        //设置通用合成材料图片
        ItemIcon.sprite = ResourcesManager.LoadAsset<Sprite>(equip_Data.Get(equip.PosId * 100).icon);

        //设置通用合成材料的底部图片
        itemBG.sprite = ResourcesManager.LoadAsset<Sprite>(tbquality.Get(typeid).frame);
        //设置数量
        //拿到对应的品质

        int index = equip_quailty.DataList.IndexOf(equip_quailty.Get(equip.Quality));
        int itemTotleCount = 0;

        for (int i = 0; i < index; i++)
        {
            if (equip_quailty.Get(equip.Quality).type.Equals(equip_quailty.DataList[i].type))
                itemTotleCount += equip_quailty.DataList[i].mergeRule[1];
        }

        itemCountText.text = "x " + itemTotleCount.ToString();
    }

    //数字显示规范
    public string numsLimit(float num)
    {
        if (num < 9999)
        {
            return num.ToString();
        }
        else if (num < 9999999)
        {
            float result = Mathf.Floor(num / 1000f * 100f) / 100f;
            return result.ToString("F0") + "K";
        }
        else
        {
            float result = Mathf.Floor(num / 1000000f * 100f) / 100f;
            return result.ToString("F0") + "M";
        }
    }

    public delegate void RelashMaterialItemEventHandler(long uid, bool isEquip);

    public static event RelashMaterialItemEventHandler RelashMaterialItem;
}