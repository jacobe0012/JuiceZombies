using Common;
using Google.Protobuf;
using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;
using XFramework;

//这里后面移到框架里面改写
public class TestInitItem : MonoBehaviour
{
    public static int mTotalDataCount;
    public ScrollRect scrollRect;

    public float Retime = 5;

    private void Awake()
    {
        //UIEquipScrollView.InitKeyPressed += WebMessage;
    }


    private void WebMessage()
    {
        WebMessageHandlerOld.Instance.AddHandler(9, 5, OnInitEquipmentResponse);
        WebMessageHandlerOld.Instance.AddHandler(9, 11, OnInitISEquipmentResponse);
        NetWorkManager.Instance.SendMessage(9, 5);
        Log.Debug("webMessage", Color.red);
    }

    public void OnEnable()
    {
        //UIEquipLevelUp.OnSpaceKeyPressed += InitAndRefreshItem;
        //UIDecrease.RefreshEquip += WebMessage;
        //UICompound.InitEquipPanel += WebMessage;
        UIRapidCompoundResult.InitEquipPanel += WebMessage;
    }

    public void OnDisable()
    {
        //UIEquipLevelUp.OnSpaceKeyPressed -= InitAndRefreshItem;
        //UIDecrease.RefreshEquip -= WebMessage;
        // UICompound.InitEquipPanel -= WebMessage;
        // UIRapidCompoundResult.InitEquipPanel -= WebMessage;
    }


    public void InitAndRefreshItem()
    {
        //预留图纸位置,对 mTotalDataCount再进行更改


        //这里的 manager.itemPrefabs[x]需要根据实际需要重新修改或者填写,已经修改

        scrollRect.content.sizeDelta = Vector2.zero;

        ScrollViewManager manager = scrollRect.GetComponent<ScrollViewManager>();
        //清楚上一次的装备
        manager.RefreshScrollView();
        //实例化角色面板
        manager.AddItem(1, manager.itemPrefabs[0], 0);
        //实例化普通装备数量(未装备的数量)
        if (EquipitemCache.GameEquips.Count != 0)
            manager.AddItem(EquipitemCache.GameEquips.Count, manager.itemPrefabs[1], 1, true);
        //实例化已经装备的装备数量
        if (EquipitemCache.isWearUID.Count != 0)
            manager.AddItem(EquipitemCache.isWearUID.Count, manager.itemPrefabs[1], 4);

        EquipitemCache.Darwitems.Clear();
        //这个是图纸
        if (ResourcesSingletonOld.Instance.items.Count != 0)
        {
            var tbitem = ConfigManager.Instance.Tables.TbitemOld;
            //ResourcesSingletonOld.Instance.SortBagItem(ResourcesSingletonOld.Instance.items);
            foreach (var item in ResourcesSingletonOld.Instance.items)
            {
                if (item.Key >= tbitem.DataList[2].id && item.Key <= tbitem.DataList[8].id)
                {
                    EquipitemCache.Darwitems.Add(item.Key, item.Value);
                }
            }

            if (EquipitemCache.Darwitems.Count != 0)
            {
                manager.AddItem(1, manager.itemPrefabs[3], 3, false, "图纸");
                manager.AddItem(EquipitemCache.Darwitems.Count, manager.itemPrefabs[2], 6, true);
            }
        }


        //EquipitemCache.MaterialsTypeDic.Clear();
        //这个是通用合成材料
        if (EquipitemCache.MaterialsTypeDic.Count != 0)
        {
            Debug.Log(EquipitemCache.MaterialsTypeDic.Count + "EquipitemCache.MaterialsTypeDic.Count");
            manager.AddItem(1, manager.itemPrefabs[3], 5, false, "通用合成材料");
            manager.AddItem(EquipitemCache.MaterialsTypeDic.Count, manager.itemPrefabs[2], 2, true);
        }

        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x,
            scrollRect.content.sizeDelta.y + manager.itemPrefabs[2].GetComponent<RectTransform>().rect.height +
            manager.Clasp);
    }


    public void OnInitEquipmentResponse(object sender, WebMessageHandlerOld.Execute e)
    {
        WebMessageHandlerOld.Instance.RemoveHandler(9, 5, OnInitEquipmentResponse);
        EquipitemCache.MaterialsTypeDic.Clear();
        EquipitemCache.GameEquips.Clear();
        ByteValueList gameEquips = new ByteValueList();
        gameEquips.MergeFrom(e.data.ToByteArray());

        for (int i = 0; i < gameEquips.Values.Count; i++)
        {
            GameEquip equip1 = new GameEquip();
            equip1.MergeFrom(gameEquips.Values[i]);
            //这里有两个集合,一个是普通装备list,一个是通用合成材料dic
            if (equip1.EquipId % 100 == 0)
            {
                equip1.PartId = 0;
                //如果有相同种类的通用合成材料,则数量加1
                if (EquipitemCache.MaterialsTypeDic.ContainsKey(equip1)) EquipitemCache.MaterialsTypeDic[equip1]++;
                //如果没有,则视做新装备
                else
                {
                    EquipitemCache.MaterialsTypeDic.Add(equip1, 1);
                    mTotalDataCount += 1;
                }
            }
            else
            {
                EquipitemCache.GameEquips.Add(equip1);
                mTotalDataCount += 1;
            }
        }


        NetWorkManager.Instance.SendMessage(9, 11);
    }


    public void OnInitISEquipmentResponse(object sender, WebMessageHandlerOld.Execute e)
    {
        WebMessageHandlerOld.Instance.RemoveHandler(9, 11, OnInitISEquipmentResponse);
        EquipitemCache.isWearUID.Clear();
        ByteValueList gameEquips = new ByteValueList();
        gameEquips.MergeFrom(e.data.ToByteArray());

        for (int i = 0; i < gameEquips.Values.Count; i++)
        {
            GameEquip equip1 = new GameEquip();
            equip1.MergeFrom(gameEquips.Values[i]);
            EquipitemCache.isWearUID.Add(equip1);
        }

        //移除操作
        EquipitemCache.DeleteItemInCache();

        //排序操作
        EquipitemCache.GeneralSort();
        EquipitemCache.MaterialsSort();

        //实例化对应装备
        InitAndRefreshItem();
    }
}