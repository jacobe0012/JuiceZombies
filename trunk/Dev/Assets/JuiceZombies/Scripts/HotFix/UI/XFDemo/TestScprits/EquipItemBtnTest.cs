using cfg.config;
using Common;
using HotFix_UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XFramework;

public enum PartID
{
    Atk = 1,
    Cloth = 2,
    Necklace = 3,
    Belt = 4,
    Glove = 5,
    Shoes = 6,
}

public class EquipItemBtnTest : UIBase
{
    //装备按钮
    public Button EquipItemBtn;

    //锁和打勾的图片
    public Image imageMask;

    //表别名
    private Tbequip_data equipData;
    private Tbequip_quality equip_Quality;
    private Tblanguage tblanguage;
    private Tbattr_variable tbattr_varibles;
    private Tbequip_level tbequip_level;
    private Tbquality tbquality;

    //静态变量,区别是装备面板还是合成面板
    public static bool EquipmentPanel = true;

    //读取后端的装备信息
    //部位底色
    public Image PartBackColorImage;

    //图示底色
    //public Image PartColorImage;   //用不上
    //部位贴图
    public Image partImage; //暂时用不上

    //图示(是否为S)
    public Image isSImage;


    //这俩存在互斥,由EquipmentPanel决定
    //图示(是否装备)
    public Image PutOnImage;

    //图示(是否是合成)
    public Image CompositionImage;


    //图示底色
    public Image BackColorImage;

    //装备框
    public Image EquipFrameColorImage;

    //装备数字的底图
    public Image FrameImage;

    //装备数字
    public TMP_Text QualityLevelText;

    //装备等级数字
    public TMP_Text EquipLevelText; //后端传过来的

    //装备图
    public Image EquipImage;

    //升级
    public Image LevelUp; //预留设置

    //是否装备
    public Image isEquip;

    //读取表临时保存的信息
    //品质ID
    private int quality;

    //部位ID
    private int pos_id;

    //是否为s
    private bool isS;

    //等级数字
    private int sign_num;

    //装备名字
    private string EquipNameId;

    //主词条Id
    private int Main_entry_id;

    //每一级成长
    private int main_entry_grow;

    //词条初始值
    private int main_entry_init;

    //装备描述
    private string Desc;

    //是否满级
    public static int isFullLevel = 0;


    //是要装备到装备台还是从装备台卸载
    public bool isEquipCompound = false;

    //是否是主材料
    public bool isMainEquip = false;

    //后端唯一数据
    public long UID;
    public int equipid;
    public int qualityid;
    public int EquipLevel;


    //是否是预览升级后的材料信息
    public bool isFinishEquip = false;

    // Start is called before the first frame update
    void Start()
    {
        //isEquipCompound = true;
        //UIEquipLevelUp.OnLevelUp += OnLevelUpHandler;
        //UIDecrease.RefreshEquip += OnRefeshEquipBtn;
        tblanguage = ConfigManager.Instance.Tables.Tblanguage;


        //根据当前面板设置装备或者合成图片,这个地方不读表
        PutOnImage.gameObject.SetActive(EquipmentPanel);
        CompositionImage.gameObject.SetActive(!EquipmentPanel);


        //打开装备信息面板或者添加到合成台上
        EquipItemBtn?.onClick.AddListener(async () =>
        {
            //UIEquipLevelUp.UID = UID;
            if (Time.time - lastClickTime >= cooldownTime)
            {
                lastClickTime = Time.time;
                //动画效果
                //UnicornTweenHelper.GradualChange(EquipItemBtn.gameObject, 1.2f, 0.12f, 0.12f);

                if (EquipmentPanel)
                {
                    //打开装备信息面板

                    //UI ui = await UIHelper.CreateAsync(UIType.UIEquipLevelUp, UILayer.Mid);
                }
                else
                {
                    //如果是点开预览合成成功的装备
                    if (isFinishEquip)
                    {
                        // UI ui = await UIHelper.CreateAsync(UIType.UIEquipLevelUp, UILayer.High);
                        return;
                    }


                    //装上
                    if (!isEquipCompound && EquipitemCache.CompoundIndexNum <= EquipitemCache.CompoundTotalNum)
                    {
                        //实例化装备,然后将这个对象作为事件的参数,传递给合成面板
                        var newItem = Instantiate(this.gameObject);
                        var RectCompoment = newItem.GetComponent<RectTransform>();
                        newItem.GetComponent<EquipItemBtnTest>().isEquipCompound = true;
                        this.isEquipCompound = true;
                        RectCompoment.SetAnchorMin(new Vector2(0.5f, 0.5f));
                        RectCompoment.SetAnchorMax(new Vector2(0.5f, 0.5f));
                        RectCompoment.SetPivot(new Vector2(0.5f, 0.5f));
                        RectCompoment.SetSize(new Vector2(170, 170));
                        var UID = this.UID;

                        //索引为0的话,需要把这个标记为主料,并且计算总量
                        if (EquipitemCache.CompoundIndexNum.Equals(0))
                        {
                            EquipitemCache.FindCompoundEquipNum(newItem);
                        }

                        addCompoundEquip?.Invoke(newItem, EquipitemCache.CompoundTotalNum,
                            EquipitemCache.CompoundIndexNum);


                        EquipitemCache.AddToCompoundListEquip(newItem);
                        if (EquipitemCache.CompoundIndexNum.Equals(0)) //主材料会带来额外的刷新事件
                        {
                            newItem.GetComponent<EquipItemBtnTest>().isMainEquip = true;
                            //刷新面板,对应的框框,数据也要刷新,做动画,部分按钮的状态改变什么的
                            refalshScrollview?.Invoke(newItem);
                        }
                        else
                        {
                            //把对应的勾子激活和换图
                            var maskObj = this.GetComponent<EquipItemBtnTest>().imageMask.gameObject;

                            maskObj.SetActive(true);
                            var ItemCompoundStateobj = maskObj.GetComponent<RectTransform>().GetChild(0).gameObject;
                            ItemCompoundStateobj.GetComponent<XImage>().sprite =
                                ResourcesManager.LoadAsset<Sprite>("GameTask_Get");


                            //this.GetComponent<XButton>().enabled = false;
                        }


                        EquipitemCache.CompoundIndexNum = EquipitemCache.CompoundListEquip.Count;
                        //EquipitemCache.CompoundIndexNum++;
                    }
                    else if (isEquipCompound) //卸下
                    {
                        //改变索引
                        EquipitemCache.CompoundIndexNum = EquipitemCache.indexOfCompoundList(this.gameObject);


                        //需要有一个事件,让源对象可以被重新点击                                       
                        if (EquipitemCache.CompoundIndexNum.Equals(0) || isMainEquip.Equals(true)) //主材料会带来额外的刷新事件
                        {
                            refalshCompoundPanel?.Invoke();
                            EquipitemCache.CompoundTotalNum =
                                EquipitemCache.CompoundIndexNum = EquipitemCache.CompoundIndexNum = 0;
                            return;
                        }
                        else
                        {
                            RelashItem?.Invoke(UID, true);
                            EquipitemCache.removeCompoundListEquip(this.gameObject);
                        }


                        //面对不同的点击事件
                        if (this.gameObject.GetComponent<RectTransform>().anchoredPosition.Equals(Vector2.zero))
                        {
                            EquipItemBtnTest[] allItems = FindObjectsOfType<EquipItemBtnTest>();
                            foreach (var item in allItems)
                            {
                                if (item.UID.Equals(this.UID))
                                {
                                    item.isEquipCompound = false;
                                }
                            }

                            GameObject.Destroy(this.gameObject);
                        }
                        else
                        {
                            EquipItemBtnTest[] allItems = FindObjectsOfType<EquipItemBtnTest>();
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

                        this.isEquipCompound = false;
                    }
                }
            }

            return;
        });
    }


    /// <summary>
    /// 不走框架新增实例化Btn
    /// </summary>
    public void InitEquipBtn(UI ui)
    {
        UI uichild = ui.GetFromPath("ContentBG/EquipImagePos");

        //不走框架的创建UI
        GameObject newEquipItem = Instantiate(this.gameObject);
        EquipitemCache.EquipUID = UID;
        //UIEquipLevelUp.UID = UID;
        var newEquipItemTestComponent = newEquipItem.GetComponent<EquipItemBtnTest>();
        newEquipItemTestComponent.UID = UID;
        newEquipItemTestComponent.BackColorImage.sprite = this.BackColorImage.sprite;
        newEquipItemTestComponent.EquipImage.sprite = this.EquipImage.sprite;
        newEquipItemTestComponent.PartBackColorImage.sprite = this.PartBackColorImage.sprite;
        newEquipItemTestComponent.partImage.sprite = this.partImage.sprite;
        newEquipItemTestComponent.isSImage.gameObject.SetActive(isS);
        newEquipItemTestComponent.QualityLevelText.text = this.QualityLevelText.text;

        newEquipItemTestComponent.EquipLevelText.text = this.EquipLevel.ToString();
        newEquipItemTestComponent.EquipLevelText.gameObject.SetActive(false);
        newEquipItemTestComponent.CompositionImage.gameObject.SetActive(false);
        newEquipItem.GetComponent<Image>().raycastTarget = false;
        //删除上一次的Item
        if (uichild.GetComponent<RectTransform>().childCount > 1)
        {
            Destroy(uichild.GetComponent<RectTransform>().GetChild(1).gameObject);
        }

        newEquipItem.GetComponent<RectTransform>().SetParent(uichild.GetComponent<RectTransform>(), false);
        newEquipItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }


    /// <summary>
    /// 动画存在问题,后续调整 NeedNum是指总共需要1个还是2个装备,idnex是指当前是第几个
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="number"></param>
    public void InitEquipBtn(GameObject obj, int NeedNum, int indexNum)
    {
        Transform NoFinishPos = null;

        //选择是第几个显示,怎么添加等等
        switch (indexNum)
        {
            case 0:
            {
                NoFinishPos = obj.GetComponent<RectTransform>().GetChild(2).GetChild(0).GetChild(0);
                EquipitemCache.EquipUID = UID;
                break;
            }
            case 1:
            {
                if (NeedNum == 1)
                {
                    NoFinishPos = obj.GetComponent<RectTransform>().GetChild(2).GetChild(2).GetChild(0);
                }
                else if (NeedNum == 2)
                {
                    NoFinishPos = obj.GetComponent<RectTransform>().GetChild(2).GetChild(3).GetChild(0);
                }

                EquipitemCache.CompoundUID[0] = UID;

                break;
            }

            case 2:
            {
                NoFinishPos = obj.GetComponent<RectTransform>().GetChild(2).GetChild(3).GetChild(1);
                EquipitemCache.CompoundUID[1] = UID;
                break;
            }
        }


        //不走框架的创建UI
        GameObject newEquipItem = Instantiate(this.gameObject, NoFinishPos);

        //newEquipItem.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(-400, -0, 0);
        //newEquipItem.GetComponent<RectTransform>().localScale = this.gameObject.GetComponent<RectTransform>().localScale;
        //var newEquipItemTestComponent = newEquipItem.GetComponent<EquipItemBtnTest>();
        //newEquipItemTestComponent.UID = this.UID;
        //newEquipItemTestComponent.BackColorImage.sprite = this.BackColorImage.sprite;
        //newEquipItemTestComponent.EquipImage.sprite = this.EquipImage.sprite;
        //newEquipItemTestComponent.PartBackColorImage.sprite = this.PartBackColorImage.sprite;
        //newEquipItemTestComponent.partImage.sprite = this.partImage.sprite;
        //newEquipItemTestComponent.isSImage.gameObject.SetActive(isS);
        //newEquipItemTestComponent.QualityLevelText.text = this.QualityLevelText.text;
        //newEquipItemTestComponent.EquipLevelText.text = this.EquipLevel.ToString();
        //newEquipItemTestComponent.PutOnImage.gameObject.SetActive(false);
        //newEquipItemTestComponent.CompositionImage.gameObject.SetActive(true);
        //newEquipItem.GetComponent<Image>().raycastTarget = true;


        //设置哪一个框激活
        GameObject materialFrameGO = obj.GetComponent<RectTransform>().GetChild(2).GetChild(1 + NeedNum).gameObject;
        materialFrameGO.SetActive(true);
        //设置Item的初始位置
        newEquipItem.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);


        //需要材料的图片
        GameObject NeedImageGo = obj.GetComponent<RectTransform>().GetChild(2).GetChild(5).gameObject;

        //动画曲线(Item和材料图片)
        // UnicornTweenHelper.MoveEffect(newEquipItem, NeedImageGo, new Vector3(0, 0, 0), new Vector3(0, 35, 0), 0.4f, 0.1f,
        //     true);
        //
        // //动画曲线(框移动)
        // UnicornTweenHelper.MoveEffect(materialFrameGO, null,
        //     new Vector3(304, materialFrameGO.GetComponent<RectTransform>().anchoredPosition.y, 0), new Vector3(0, 0, 0),
        //     0.25f, 0.1f, false);
        
        obj.GetComponent<RectTransform>().GetChild(2).GetChild(1).gameObject.SetActive(true);
        if (NeedNum == indexNum)
        {
            //激活合成成功框
            GameObject objgroup = obj.GetComponent<RectTransform>().GetChild(2).GetChild(0).GetChild(2).gameObject;
            objgroup.SetActive(true);
            //播放动效
           


            //合成按钮激活
            obj.GetComponent<RectTransform>().GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject
                .SetActive(true);
            //显示合成后的属性变化
            GameObject CompoundPanelGo = GameObject.Find("CompoundPanel(Clone)").gameObject;
            var TextComponents = CompoundPanelGo.GetComponentsInChildren<XTextMeshProUGUI>();
            //获得装备信息
            GameObject noFinishPos = CompoundPanelGo.GetComponent<RectTransform>().GetChild(2).GetChild(0).GetChild(0)
                .GetChild(0).gameObject;


            foreach (var textComponent in TextComponents)
            {
                //失活默认组件
                if (textComponent.gameObject.name.Equals("TipsInfoText") && textComponent.gameObject.activeSelf)
                {
                    textComponent.gameObject.SetActive(false);


                    CompoundPanelGo.GetComponent<RectTransform>().GetChild(2).GetChild(4).GetChild(1).gameObject
                        .SetActive(true);
                    //激活TipFInishPos
                    continue;
                }

                if (!CompoundPanelGo.GetComponent<RectTransform>().GetChild(2).GetChild(4).GetChild(1).gameObject
                        .activeSelf) continue;
                var item = noFinishPos.GetComponent<EquipItemBtnTest>();
                if (item == null) continue;
                //这里后面要改成读表的装备名,且要读语言表
                var posType = item.pos_id;
                var type = posType % 2;
                //激活整个游戏对象


                //这里要不换成测试假缓存？
                //激活指定组件和初始化合成后的装备信息
                if (textComponent.gameObject.name.Equals("EquipNameText") && textComponent.text != "")
                {
                    textComponent.text = "品质为" + item.quality + "色武器";
                }
                else if (textComponent.gameObject.name.Equals("LevelOldValueText") && textComponent.text != "")
                {
                    textComponent.text = tblanguage.Get("attr_info_level_upperlimit").current +
                                         item.EquipLevel.ToString();
                }
                else if (textComponent.gameObject.name.Equals("PropertyOldValueText") && textComponent.text != "")
                {
                    if (type.Equals(1))
                    {
                        //这里后面要换成真正的武器主属性
                        textComponent.text = tblanguage.Get("attr_atk").current + item.main_entry_init;
                    }
                    else
                    {
                        //这里后面要换成真正的武器主属性
                        textComponent.text = tblanguage.Get("attr_hp").current + item.main_entry_init;
                    }
                }
                else if (textComponent.gameObject.name.Equals("LevelNewValueText") && textComponent.text != "")
                {
                    var equip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
                    var subtype = equip_quality.Get(item.quality).subtype;
                    if (subtype < equip_quality.Get(item.quality + 1).subtype)
                    {
                        textComponent.text = equip_quality.Get(item.quality + 1).levelMax.ToString();
                    }
                }
                else if (textComponent.gameObject.name.Equals("PropertyNewValueText") && textComponent.text != "")
                {
                    var equip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
                    var subtype = equip_quality.Get(item.quality).subtype;
                    if (subtype < equip_quality.Get(item.quality + 1).subtype)
                    {
                        if (type.Equals(1))
                        {
                            //这里后面要换成合成后的真正的武器主属性
                            textComponent.text = (item.main_entry_init + UnityEngine.Random.Range(5, 20).ToString());
                        }
                        else
                        {
                            //这里后面要换成合成后的真正的武器主属性
                            textComponent.text = (item.main_entry_init + UnityEngine.Random.Range(5, 20).ToString());
                        }
                    }
                }
            }
        }
    }


    //public void InitEquipBtn(GameObject obj) 

    private void ItemSetParent(GameObject obj, Transform t)
    {
        obj.GetComponent<RectTransform>().SetParent(t, true);
    }


    private void initEquiplevelUp(UI ui)
    {
        isFullLevel = 0;
        //ui.GetFromPath("ContentBG/EquipLevelBG/LevelInfoText").GetTextMeshPro()
        //   .SetTMPText(tblanguage.Get("attr_info_level").zhCn + ":");
        //设置等级数字
        //ui.GetFromPath("ContentBG/EquipLevelBG/LevelContentText").GetTextMeshPro()
        //    .SetTMPText(EquipLevelText.text.Substring(2) + "/" + equip_Quality.Get(quality).levelMax.ToString());
        //判断当前等级是否超过上限,超过就显示上限

        if (CompareNumber(int.Parse(EquipLevelText.text.Substring(2)), equip_Quality.Get(quality).levelMax) ||
            isFullLevel != 0)
        {
            ui.GetFromPath("ContentBG/ContentBottomPos/NormalState").GetComponent<RectTransform>().gameObject
                .SetActive(false);
            ui.GetFromPath("ContentBG/ContentBottomPos/FulllevelState").GetComponent<RectTransform>().GetChild(0)
                .GetComponent<TextMeshProUGUI>().text = (tblanguage.Get("equip_levelup_max").zhCn);
            ui.GetFromPath("ContentBG/ContentBottomPos/FulllevelState").GetComponent<RectTransform>().gameObject
                .SetActive(true);
            isFullLevel = 1;
        }


        //设置品质图片(先不做)

        Debug.Log(quality + "quality");
        //设置品质和品质数字
        //ui.GetFromPath("TopQualityImage/TopQualityText").GetTextMeshPro()
        // .SetTMPText(tblanguage.Get(equip_Quality.Get(quality).qualityName).zhCn + "+" + sign_num);
        //设置武器名字
        //ui.GetFromPath("EquipName").GetTextMeshPro().SetTMPText(tblanguage.Get(EquipNameId).zhCn);
        ui.GetFromPath("EquipName").GetText().SetText(EquipNameId);
        //设置部位
        ui.GetFromPath("ContentBG/EquipMainPropertyBG/EquipPartImage").GetImage()
            .SetSprite(ResourcesManager.LoadAsset<Sprite>(tbattr_varibles.Get(Main_entry_id).icon), false);
        //设置主属性
        ui.GetFromPath("ContentBG/EquipMainPropertyBG/EquipPartText").GetText().SetText(
            (main_entry_init + ((System.Convert.ToInt32(EquipLevelText.text.Substring(2)) - 1) * main_entry_grow))
            .ToString());
        //设置装备描述(多表)
        ui.GetFromPath("ContentBG/EquipDescriptionBG/EquipDescriptionText").GetTextMeshPro().SetTMPText(Desc);
        //设置技能描述(多表)
        //读取品质大类
        var typeInt = equip_Quality.Get(quality).type;
        //Debug.Log(typeInt);
        UI listPropertyui = ui.GetFromPath("ContentBG/EquipSkillBG/ScrollView/Viewport/Content");

        //解锁图片名字
        string Unlockname = tbquality.Get(typeInt).unlock;
        //未解锁图片名
        string lockname = tbquality.Get(typeInt).lock0;


        string part1 = Unlockname.Substring(0, 18);

        //这个是数字
        string part2 = Unlockname.Substring(18, 1);

        string part3 = Unlockname.Substring(19);

        string part4 = lockname.Substring(19);

        for (int i = 0; i < listPropertyui.GetComponent<RectTransform>().childCount; i++)
        {
            var child = listPropertyui.GetComponent<RectTransform>().GetChild(i);

            if (i + 1 < typeInt)
                child.GetChild(0).gameObject.GetComponent<Image>().sprite =
                    ResourcesManager.LoadAsset<Sprite>(part1 + (i + 1).ToString() + part3);


            else
                child.GetChild(0).gameObject.GetComponent<Image>().sprite =
                    ResourcesManager.LoadAsset<Sprite>(part1 + (i + 1).ToString() + part4);
        }

        //设置初始金币和初始图纸？后端计算？？
        //这里的999K后面要对接后端玩家剩余金币量
        //3表示美钞
        //5表示物品
        //消耗金币
        Debug.Log(EquipLevel + "EquipLevel");
        //Debug.Log(tbequip_level.Get(EquipLevel).cost1[0] +"VEC3");
        ui.GetFromPath("ContentBG/ContentBottomPos/NormalState/GoldConsumeNumber").GetTextMeshPro()
            .SetTMPText(ReturnValueFromTableType(tbequip_level.Get(EquipLevel).cost1[0]) + "/" + "999K");
        //消耗物品
        ui.GetFromPath("ContentBG/ContentBottomPos/NormalState/DrawingConsumeNumber").GetTextMeshPro()
            .SetTMPText((ReturnValueFromTableType(tbequip_level.Get(EquipLevel).cost1[1]) + "/" + "999K"));
        //设置消耗物品所对应的icon Image
    }


    /// <summary>
    /// 重载方法
    /// </summary>
    /// <param name="lineCount"></param>
    public void InitTableAsync(int id, int qualityId, long uid, GameEquip equipdata)
    {
        //缓存装备信息
        UID = uid;
        equipid = id;
        qualityid = qualityId;
        EquipLevel = equipdata.Level;


        tblanguage = ConfigManager.Instance.Tables.Tblanguage;
        equipData = ConfigManager.Instance.Tables.Tbequip_data;
        tbquality = ConfigManager.Instance.Tables.Tbquality;
        equip_Quality = ConfigManager.Instance.Tables.Tbequip_quality;

        tbattr_varibles = ConfigManager.Instance.Tables.Tbattr_variable;

        tbequip_level = ConfigManager.Instance.Tables.Tbequip_level;
        //从某行获得武器名
        EquipNameId = equipData.Get(id).name;

        //从某行获得品质
        quality = equipData.Get(id).quality;


        //从某行获得部位Id
        pos_id = equipData.Get(id).posId;

        //是否为s
        isS = equipData.Get(id).sYn == 1 ? true : false;
        Main_entry_id = equipData.Get(id).mainEntryId;
        //词条初始
        main_entry_init = equipData.Get(id).mainEntryInit;
        //每级成长
        main_entry_grow = equipData.Get(id).mainEntryGrow;
        //装备描述
        Desc = equipData.Get(id).desc;
        //直接初始化装备图
        EquipImage.sprite = ResourcesManager.LoadAsset<Sprite>(equipData.Get(id).icon);

        //初始化等级数字
        EquipLevelText.text = tblanguage.Get("attr_info_level").current + equipdata.Level;
    }

    /// <summary>
    /// 初始化装备信息
    /// </summary>
    public void InitEquipMent()
    {
        var typeid = equip_Quality.Get(quality).type;

        BackColorImage.sprite = ResourcesManager.LoadAsset<Sprite>(tbquality.Get(typeid).frame);
        PartBackColorImage.sprite = ResourcesManager.LoadAsset<Sprite>(tbquality.Get(typeid).posFrame);


        switch (pos_id)
        {
            case (int)PartID.Atk:
            {
                partImage.sprite = ResourcesManager.LoadAsset<Sprite>("icon_equip_pos_1");
            }
                break;

            case (int)PartID.Cloth:
            {
                partImage.sprite = ResourcesManager.LoadAsset<Sprite>("icon_equip_pos_2");
            }
                break;

            case (int)PartID.Necklace:
            {
                partImage.sprite = ResourcesManager.LoadAsset<Sprite>("icon_equip_pos_3");
            }
                break;
            case (int)PartID.Belt:
            {
                partImage.sprite = ResourcesManager.LoadAsset<Sprite>("icon_equip_pos_4");
            }
                break;
            case (int)PartID.Glove:
            {
                partImage.sprite = ResourcesManager.LoadAsset<Sprite>("icon_equip_pos_5");
            }
                break;
            case (int)PartID.Shoes:
            {
                partImage.sprite = ResourcesManager.LoadAsset<Sprite>("icon_equip_pos_5");
            }
                break;
        }

        sign_num = equip_Quality.Get(quality).signNum;
        if (sign_num == 0)
        {
            FrameImage.gameObject.SetActive(false);
        }
        else
        {
            FrameImage.gameObject.SetActive(true);
            //数字填充
            QualityLevelText.text = sign_num.ToString();
        }

        isSImage.gameObject.SetActive(isS);
    }

    #region 有可能删除

    /// <summary>
    /// 根据品质初始化颜色(背景)
    /// </summary>
    public Color InitBackBGColor(int quality)
    {
        Color colour = Color.white;
        if (quality == 1) colour = new Color(255, 255, 255, 255); //白色
        if (quality == 2) colour = new Color(60, 255, 60, 255); //绿色
        if (quality == 3) colour = new Color(60, 165, 255, 255); //蓝色
        if (quality >= 4 && quality <= 6) colour = new Color(163, 55, 217, 255); //alpha值为255的紫色
        if (quality >= 7 && quality <= 10) colour = new Color(255, 90, 0, 255); //alpha值为255的橙色
        if (quality >= 11 && quality <= 15) colour = new Color(230, 45, 60, 255); //红色
        return colour;
    }


    /// <summary>
    /// 根据品质初始化颜色
    /// </summary>
    public Color InitBackColor(int quality)
    {
        Color colour = Color.white;
        if (quality == 1) colour = new Color(200, 200, 200, 255); //白色
        if (quality == 2) colour = new Color(0, 150, 0, 255); //绿色
        if (quality == 3) colour = new Color(0, 80, 255, 255); //蓝色
        if (quality >= 4 && quality <= 6) colour = new Color(106, 0, 126, 255); //alpha值为255的紫色
        if (quality >= 7 && quality <= 10) colour = new Color(255, 40, 0, 255); //alpha值为255的橙色
        if (quality >= 11 && quality <= 15) colour = new Color(202, 0, 0, 255); //红色
        return colour;
    }

    #endregion

    /// <summary>
    /// 根据表类型返回对应的值
    /// </summary>
    /// <returns></returns>
    public string ReturnValueFromTableType(Vector3 TableType)
    {
        string ReturnValueText = ((int)TableType.z).ToString();
        if ((int)TableType.z >= 100000) ReturnValueText += "K";
        return ReturnValueText;
    }


    public bool CompareNumber(int num1, int num2)
    {
        if (num1 >= num2) return true;
        else return false;
    }


    public void OnEnable()
    {
        //UIEquipLevelUp.OnLevelUp -= OnLevelUpHandler;
    }

    //升级带来的升级事件
    void OnLevelUpHandler(int newLevel, long uid)
    {
        if (uid == UID)
            EquipLevelText.text = tblanguage.Get("attr_info_level").zhCn + newLevel.ToString();
    }

    //降级降品带来的刷新事件
    private void OnRefeshEquipBtn(GameEquip equipObject)
    {
        Debug.Log(equipObject.PartId + "equipObject.PartId" + EquipitemCache.EquipUID);
        if (equipObject.PartId == EquipitemCache.EquipUID)
        {
            var tbequip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
            EquipLevelText.text = tblanguage.Get("attr_info_level").zhCn + equipObject.Level;
            main_entry_init = equipObject.MainEntryInit;

            //数字角标
            if (tbequip_quality.Get(equipObject.Quality).signNum.Equals(0))
            {
                FrameImage.gameObject.SetActive(false);
            }

            else QualityLevelText.text = tbequip_quality.Get(equipObject.Quality).signNum.ToString();
        }
    }


    //合成面板中需要传递的对象,作为面板传递过去
    public delegate void CompoundEventHandler(GameObject obj, int TotolNum, int indexNum);

    public static event CompoundEventHandler addCompoundEquip;


    //合成面板中滚动视图的刷新事件
    public delegate void CompoundScorllViewEventHandler(GameObject obj);

    public static event CompoundScorllViewEventHandler refalshScrollview;

    //卸下主材料带来的刷新事件
    public delegate void CompoundPanelEventHandler();

    public static event CompoundPanelEventHandler refalshCompoundPanel;

    //卸下其他材料带来的刷新事件
    public delegate void RelashItemEventHandler(long uid, bool isEquip);

    public static event RelashItemEventHandler RelashItem;
}