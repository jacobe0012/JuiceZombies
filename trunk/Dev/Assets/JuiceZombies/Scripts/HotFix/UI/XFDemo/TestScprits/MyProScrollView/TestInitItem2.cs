using Common;
using Google.Protobuf;
using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;
using XFramework;

public class TestInitItem2 : MonoBehaviour
{
    public static int mTotalDataCount;
    public ScrollRect scrollRect;

    //public float Retime = 5;

    public float contentX;

    public void OnEnable()
    {
        webMessage();
        //把两个装备数据清空

        //EquipitemCache.CompoundNum = 0;
        // EquipItemBtnTest.refalshCompoundPanel += InitAndRefreshItem2;
        // UICompound.refalshScrollview2 += ReflashItem2;
        // UIRapidCompoundResult.InitEquipPanel += webMessage;
        // UICompound.InitEquipPanel += webMessage;
    }

    public void webMessage()
    {
        WebMessageHandlerOld.Instance.AddHandler(9, 5, OnInitEquipmentResponse);
        WebMessageHandlerOld.Instance.AddHandler(9, 11, OnInitISEquipmentResponse);
        NetWorkManager.Instance.SendMessage(9, 5);
    }


    public void OnDisable()
    {
        // EquipItemBtnTest.refalshCompoundPanel -= InitAndRefreshItem2;
        // UICompound.refalshScrollview2 -= ReflashItem2;
        // UIRapidCompoundResult.InitEquipPanel -= webMessage;
        // UICompound.InitEquipPanel -= webMessage;
    }

    private void ReflashItem2(GameEquip equip)
    {
        scrollRect.content.sizeDelta = Vector2.zero; //Vector2.zero;

        ScrollViewManager manager = scrollRect.GetComponent<ScrollViewManager>();

        //清除上一次的装备
        manager.RefreshScrollView();
        //重刷装备
        //实例化所有装备,包括已装备的装备
        if (EquipitemCache.allCompoundEquip.Count != 0)
            manager.AddItem2(EquipitemCache.allCompoundEquip.Count, manager.itemPrefabs[1], 1, equip);
        //实例化所有装备,通用合成材料
        if (EquipitemCache.allCompoundMaterial.Count != 0)
            manager.AddItem2(EquipitemCache.allCompoundMaterial.Count, manager.itemPrefabs[2], 2, equip);

        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x,
            scrollRect.content.sizeDelta.y + manager.itemPrefabs[2].GetComponent<RectTransform>().rect.height +
            manager.Clasp);
    }


    public void InitAndRefreshItem2()
    {
        //预留图纸位置,对 mTotalDataCount再进行更改
        EquipitemCache.CompoundTotalNum = 0;
        EquipitemCache.CompoundIndexNum = 0;

        //这里的 manager.itemPrefabs[x]需要根据实际需要重新修改或者填写,已经修改

        scrollRect.content.sizeDelta = Vector2.zero;

        ScrollViewManager manager = scrollRect.GetComponent<ScrollViewManager>();
        //清除上一次的装备
        manager.RefreshScrollView();
        //清楚通用合成材料的索引
        scrollRect.GetComponent<ScrollViewManager>().CompoundEquiipIndex = 0;
        //实例化已经装备的装备数量
        if (EquipitemCache.isWearUID.Count != 0)
            manager.AddItem(EquipitemCache.isWearUID.Count, manager.itemPrefabs[1], 4, true);

        //实例化普通装备数量(未装备的数量)
        if (EquipitemCache.GameEquips.Count != 0)
            manager.AddItem(EquipitemCache.GameEquips.Count, manager.itemPrefabs[1], 1, true);

        //manager.SortGameEquipObj();

        //这个是通用合成材料
        if (EquipitemCache.MaterialsTypeList.Count != 0)
        {
            manager.AddItem(EquipitemCache.MaterialsTypeList.Count, manager.itemPrefabs[2], 2, true);
        }

        //微调整滚动视图
        //scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, scrollRect.content.sizeDelta.y + manager.itemPrefabs[2].GetComponent<RectTransform>().rect.height + manager.Clasp);
        //if ((EquipitemCache.isWearUID.Count+ EquipitemCache.GameEquips.Count+ EquipitemCache.MaterialsTypeList.Count)%5!=0)
        //    scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, scrollRect.content.sizeDelta.y + manager.itemPrefabs[2].GetComponent<RectTransform>().rect.height + manager.Clasp);

        //if(EquipitemCache.isWearUID.Count==0)
        //    scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, scrollRect.content.sizeDelta.y + manager.itemPrefabs[2].GetComponent<RectTransform>().rect.height + manager.Clasp);
    }


    public void OnInitEquipmentResponse(object sender, WebMessageHandlerOld.Execute e)
    {
        WebMessageHandlerOld.Instance.RemoveHandler(9, 5, OnInitEquipmentResponse);
        //清楚合成材料的数量
        //EquipitemCache.MaterialsTypeDic.Clear();
        EquipitemCache.MaterialsTypeList.Clear();
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
                //equip1.PartId = 0;
                ////如果有相同种类的通用合成材料,则数量加1
                //if (EquipitemCache.MaterialsTypeDic.ContainsKey(equip1)) {  EquipitemCache.MaterialsTypeDic[equip1]++; }
                ////如果没有,则视做新装备
                //else
                //{
                //    EquipitemCache.MaterialsTypeDic.Add(equip1, 1);
                //    mTotalDataCount += 1;
                //}
                EquipitemCache.MaterialsTypeList.Add(equip1);

                Debug.Log(equip1.PartId + "equip1.PartId");
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


        //实例化对应装备
        InitAndRefreshItem2();
    }
}