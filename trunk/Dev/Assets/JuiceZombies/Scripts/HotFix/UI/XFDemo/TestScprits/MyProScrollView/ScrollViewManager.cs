using System.Collections.Generic;
using System.Linq;
using Common;
using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;
using XFramework;

[DefaultExecutionOrder(5000)]
public class ScrollViewManager : MonoBehaviour
{
    public List<GameObject> itemPrefabs; // 预制体
    public RectTransform contentTransform; // Content对象的Transform组件

    public ScrollRect scrollRect; // ScrollRect组件

    //间隙
    public float Clasp = 20;

    private List<GameObject> items = new List<GameObject>();

    public int CompoundEquiipIndex = 0;

    public int InitPosX = -460;

    // 初始化滚动视图
    private void OnEnable()
    {
        scrollRect.content.sizeDelta = Vector2.zero;
        EquipItemBtnTest.RelashItem += ChageItemState;
        EquipMaterialItemTest.RelashMaterialItem += ChageItemState;
    }

    private void OnDisable()
    {
        EquipItemBtnTest.RelashItem -= ChageItemState;
        EquipMaterialItemTest.RelashMaterialItem -= ChageItemState;
    }

    public void ChageItemState(long uid, bool isEquip)
    {
        ScrollViewManager[] allScrollViews = FindObjectsOfType<ScrollViewManager>();
        GameObject obj = new GameObject();
        foreach (var item in allScrollViews)
        {
            if (!item.gameObject.name.Contains("(Clone)"))
            {
                obj = item.gameObject;
                break;
            }
        }

        for (int i = 0; i < obj.GetComponent<RectTransform>().GetChild(0).GetChild(0).childCount; i++)
        {
            var content = obj.GetComponent<RectTransform>().GetChild(0).GetChild(0);
            if (isEquip)
            {
                if (content.GetChild(i).GetComponent<EquipItemBtnTest>().UID.Equals(uid))
                {
                    content.GetChild(i).GetComponent<EquipItemBtnTest>().imageMask.gameObject.SetActive(false);
                    //可以重新被点击
                    content.GetChild(i).GetComponent<XButton>().enabled = true;
                    break;
                }
            }
            else
            {
                if (content.GetChild(i).GetComponent<EquipMaterialItemTest>() != null)
                {
                    if (content.GetChild(i).GetComponent<EquipMaterialItemTest>().UID.Equals(uid))
                    {
                        Debug.Log(111);
                        content.GetChild(i).GetComponent<EquipMaterialItemTest>().ImageMask.gameObject.SetActive(false);

                        //可以重新被点击
                        content.GetChild(i).GetComponent<EquipMaterialItemTest>().ItemBtn.gameObject
                            .GetComponent<XButton>().enabled = true;
                        break;
                    }
                    else continue;
                }
            }
        }
    }

    // 添加一个元素到滚动视图
    public void AddItem(int number, GameObject itemPrefab, int PrefabIndex, bool isNormalEquip = false,
        string name = "")
    {
        for (int i = 0; i < number; i++)
        {
            GameObject newItem = Instantiate(itemPrefab, contentTransform);
            //这里可以写初始化信息的
            switch (PrefabIndex)
            {
                case 0:
                {
                    //角色面板,可能数值需要初始化
                }
                    break;

                case 1:
                {
                    //未装备的装备信息赋值
                    EquipItemBtnTest equipItemBtnTest = newItem.GetComponent<EquipItemBtnTest>();
                    equipItemBtnTest.InitTableAsync(EquipitemCache.GameEquips[i].EquipId,
                        EquipitemCache.GameEquips[i].Quality, EquipitemCache.GameEquips[i].PartId,
                        EquipitemCache.GameEquips[i]);
                    equipItemBtnTest.InitEquipMent();
                }
                    break;

                case 2:
                {
                    EquipMaterialItemTest equipMaterialItemTest = newItem.GetComponent<EquipMaterialItemTest>();


                    //装备面板和合成面板初始化
                    if (EquipItemBtnTest.EquipmentPanel)
                        equipMaterialItemTest.InitMaterialItem(EquipitemCache.MaterialsTypeDic.ElementAt(i).Key,
                            EquipitemCache.MaterialsTypeDic.ElementAt(i).Value);
                    else
                    {
                        equipMaterialItemTest.InitMaterialItem2(EquipitemCache.MaterialsTypeList[i],
                            EquipitemCache.MaterialsTypeList[i].PartId);
                        equipMaterialItemTest.ImageMask.gameObject.SetActive(true);
                        newItem.GetComponent<RectTransform>().GetChild(0).GetComponent<XButton>().enabled = false;
                        //childButton.GetComponent<XButton>().enabled=false;
                    }
                }
                    break;
                case 3:
                {
                    //图纸面板,需要初始化,但脚本还没写
                    var equipDrawingComponent = newItem.GetComponent<EquipDrawingTest>();
                    equipDrawingComponent.initPanel(name);
                }
                    break;
                case 4:
                {
                    //已经装备的装备信息赋值
                    EquipItemBtnTest equipItemBtnTest = newItem.GetComponent<EquipItemBtnTest>();
                    equipItemBtnTest.InitTableAsync(EquipitemCache.isWearUID[i].EquipId,
                        EquipitemCache.isWearUID[i].Quality, EquipitemCache.isWearUID[i].PartId,
                        EquipitemCache.isWearUID[i]);
                    equipItemBtnTest.InitEquipMent();
                    equipItemBtnTest.isEquip.gameObject.SetActive(false);
                    if (!EquipItemBtnTest.EquipmentPanel) equipItemBtnTest.isEquip.gameObject.SetActive(true);
                }
                    break;

                case 5:
                {
                    //通用合成材料tips
                    var equipDrawingComponent = newItem.GetComponent<EquipDrawingTest>();
                    equipDrawingComponent.initMaterials();
                }
                    break;
                case 6:
                {
                    EquipMaterialItemTest equipMaterialItemTest = newItem.GetComponent<EquipMaterialItemTest>();

                    //图纸
                    equipMaterialItemTest.InitDrawItem(EquipitemCache.Darwitems.ElementAt(i).Key,
                        EquipitemCache.Darwitems.ElementAt(i).Value);
                }
                    break;
            }


            //Debug.Log(PrefabIndex + "PrefabIndex");
            var itemRectComponent = newItem.GetComponent<RectTransform>();

            //设置位置
            if (isNormalEquip)
            {
                //普通情况,包括所有没有装备或者不能装备的装备
                itemRectComponent.anchoredPosition = new(InitPosX + (i % 5) * (-InitPosX / 2),
                    -contentTransform.GetComponent<RectTransform>().rect.height);
            }
            else if (number <= 1 && PrefabIndex != 4)
            {
                //过度item
                itemRectComponent.anchoredPosition =
                    new Vector2(0, -contentTransform.GetComponent<RectTransform>().rect.height);
            }
            else if (PrefabIndex == 4)
            {
                //已经装备的装备信息赋值
                var FatherTransform = contentTransform.GetComponent<RectTransform>().GetChild(0).GetChild(0)
                    .GetChild(EquipitemCache.isWearUID[i].PosId - 1);


                //设置父亲对象为它
                itemRectComponent.SetParent(FatherTransform);

                //设置位置为0
                itemRectComponent.anchoredPosition = Vector2.zero;

                itemRectComponent.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            }


            items.Add(newItem);


            float contentHeight = itemRectComponent.GetComponent<RectTransform>().rect.height;

            float scale = itemRectComponent.GetComponent<RectTransform>().localScale.x;


            if (number >= 1)
            {
                if (PrefabIndex != 4 && EquipItemBtnTest.EquipmentPanel)
                {
                    //5个一行
                    if (i % 5 == 4)
                    {
                        // 更新滚动视图的大小

                        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x,
                            scrollRect.content.sizeDelta.y + Clasp + contentHeight * scale);
                    }
                    else if (number - i <= 1)
                    {
                        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x,
                            scrollRect.content.sizeDelta.y + Clasp + contentHeight * scale);
                    }
                }
                else if (!EquipItemBtnTest.EquipmentPanel)
                {
                    //5个一行
                    if (i % 5 == 4)
                    {
                        // 更新滚动视图的大小

                        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x,
                            scrollRect.content.sizeDelta.y + Clasp + contentHeight * scale);
                    }
                    else if (number - i <= 1)
                    {
                        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x,
                            scrollRect.content.sizeDelta.y + Clasp + contentHeight * scale);
                    }
                }
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform.GetComponent<RectTransform>());
    }

    //添加一个元素到滚动视图
    public void AddItem2(int number, GameObject itemPrefab, int PrefabIndex, GameEquip equip)
    {
        for (int i = 0; i < number; i++)
        {
            GameObject newItem = Instantiate(itemPrefab, contentTransform);

            switch (PrefabIndex)
            {
                case 0:
                {
                    //角色面板,可能数值需要初始化
                }
                    break;

                case 1:
                {
                    //所有装备信息赋值
                    EquipItemBtnTest equipItemBtnTest = newItem.GetComponent<EquipItemBtnTest>();
                    equipItemBtnTest.InitTableAsync(EquipitemCache.allCompoundEquip[i].EquipId,
                        EquipitemCache.allCompoundEquip[i].Quality, EquipitemCache.allCompoundEquip[i].PartId,
                        EquipitemCache.allCompoundEquip[i]);
                    equipItemBtnTest.InitEquipMent();

                    // if (!equip.Type.Equals(EquipitemCache.allCompoundEquip[i].Type)|| !equip.SubType.Equals(EquipitemCache.allCompoundEquip[i].SubType)) { 
                    //     equipItemBtnTest.imageMask.gameObject.SetActive(true); //然后失活按钮点击
                    //     newItem.GetComponent<XButton>().enabled = false;
                    // }
                    // if(equip.Equals(EquipitemCache.allCompoundEquip[i]))
                    // {
                    //     //先激活再打勾
                    //     equipItemBtnTest.imageMask.gameObject.SetActive(true);
                    //     //替换图片为打勾
                    //     var childCompoment=equipItemBtnTest.imageMask.gameObject.GetComponent<RectTransform>().GetChild(0).GetComponent<XImage>();
                    //     childCompoment.sprite=ResourcesManager.LoadAsset<Sprite>("GameTask_Get");
                    //     //然后失活按钮点击
                    //     equipItemBtnTest.isEquipCompound = true;
                    //
                    // }
                }
                    break;

                case 2:
                {
                    EquipMaterialItemTest equipMaterialItemTest = newItem.GetComponent<EquipMaterialItemTest>();
                    equipMaterialItemTest.InitMaterialItem2(EquipitemCache.allCompoundMaterial[i],
                        EquipitemCache.allCompoundMaterial[i].PartId);
                    // if (!equip.Type.Equals(EquipitemCache.allCompoundMaterial[i].Type)) { 
                    //     equipMaterialItemTest.ImageMask.gameObject.SetActive(true);
                    //     newItem.GetComponent<RectTransform>().GetChild(0).GetComponent<XButton>().enabled = false;
                    // }
                }
                    break;
            }

            items.Add(newItem);

            var itemRectComponent = newItem.GetComponent<RectTransform>();
            float contentHeight = itemRectComponent.GetComponent<RectTransform>().rect.height;

            float scale = itemRectComponent.GetComponent<RectTransform>().localScale.x;

            if (number > 1)
            {
                //5个一行
                if (i % 5 == 4)
                {
                    // 更新滚动视图的大小

                    scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x,
                        scrollRect.content.sizeDelta.y + Clasp + contentHeight * scale);
                }
                else if (number - i <= 1)
                {
                    scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x,
                        scrollRect.content.sizeDelta.y + Clasp + contentHeight * scale);
                }
            }
            else
            {
                scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x,
                    scrollRect.content.sizeDelta.y + Clasp + contentHeight * scale);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform.GetComponent<RectTransform>());
        //需要计算滚动视图大小吗？
    }

    // 刷新滚动视图中的元素排列
    public void RefreshScrollView()
    {
        for (int i = 0; i < items.Count; i++)
        {
            DestroyImmediate(items[i]);
        }

        items.Clear();
    }


    public void SortGameEquipObj()
    {
        this.items.Sort(new EquipGameObjComparer());
    }
}

public class EquipGameObjComparer : IComparer<GameObject>
{
    public int Compare(GameObject x, GameObject y)
    {
        var tbequip_data = ConfigManager.Instance.Tables.Tbequip_data;
        var xc = x.GetComponent<EquipItemBtnTest>();
        var yc = y.GetComponent<EquipItemBtnTest>();

        // 品质由高到低
        if (xc.qualityid > yc.qualityid)
            return -1;
        else if (xc.qualityid < yc.qualityid)
            return 1;

        // S在前，普通在后
        if (xc.isSImage.gameObject.Equals(true) && !yc.isSImage.gameObject.Equals(true))
            return -1;
        else if (!xc.isSImage.gameObject.Equals(true) && yc.isSImage.gameObject.Equals(true))
            return 1;

        var xPOS = tbequip_data.Get(xc.equipid).posId;
        var yPOS = tbequip_data.Get(yc.equipid).posId;
        // 部位id由小到大
        if (xPOS < yPOS)
            return -1;
        else if (xPOS > yPOS)
            return 1;


        // 等级由高到低
        if (xc.EquipLevel > yc.EquipLevel)
            return -1;
        else if (xc.EquipLevel < yc.EquipLevel)
            return 1;

        // 装备id由大到小
        if (xc.equipid > yc.equipid)
            return -1;
        else if (xc.equipid < yc.equipid)
            return 1;

        // uid从小到大
        if (xc.UID < yc.UID)
            return -1;
        else if (xc.UID > yc.UID)
            return 1;

        return 0;
    }
}