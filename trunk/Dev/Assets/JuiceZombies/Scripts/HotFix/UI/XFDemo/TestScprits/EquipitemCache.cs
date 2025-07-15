using System.Collections.Generic;
using System.Linq;
using Common;
using HotFix_UI;
using UnityEngine;

public static class EquipitemCache
{
    //角色还没装备的装备数量
    public static List<GameEquip> GameEquips = new List<GameEquip>();

    //角色已经装备的装备UID数量
    public static List<GameEquip> isWearUID = new List<GameEquip>();

    //角色在合成面板所有相同部位的装备,不包括通用合成材料
    public static List<GameEquip> allCompoundEquip = new List<GameEquip>();

    //角色在合成面板所有相同部位的装备,通用合成材料
    public static List<GameEquip> allCompoundMaterial = new List<GameEquip>();


    public static long EquipUID; //这个感觉可以不要

    //装备类型,为了动画
    public static int isFinshedEquipid;

    //长度为2的合成材料uid数组
    public static long[] CompoundUID = new long[2]; //这个后面是不要的

    //普通装备的数量
    public static int NorEquipsNumber;

    //合成图纸的数量
    public static int DrawingTextNumber;


    //通用合成材料集合,装备面板
    public static Dictionary<GameEquip, int> MaterialsTypeDic = new Dictionary<GameEquip, int>();

    //图纸集合,装备面板
    public static Dictionary<int, long> Darwitems = new Dictionary<int, long>();

    //通用合成材料集合,合成面板
    public static List<GameEquip> MaterialsTypeList = new List<GameEquip>();


    //合成相关的缓存数据,包括需要显示哪一个框的凭据数据
    public static int CompoundTotalNum = 0;

    //合成相关数据,区分主料还是辅料
    public static int CompoundIndexNum = 0;

    //这是第几个位置
    //public static int 

    //合成材料的相关数据,就是一个list的gameequip
    public static List<GameEquip> CompoundListEquip = new List<GameEquip>();

    //对池子A和池子B进行处理,删除处理
    public static void DeleteItemInCache()
    {
        for (int j = 0; j < isWearUID.Count; j++)
        {
            for (int i = 0; i < GameEquips.Count; i++)
            {
                if (isWearUID[j].Equals(GameEquips[i]))
                {
                    //将池子A对应的装备移除
                    GameEquips.Remove(GameEquips[i]);
                    break;
                }
            }
        }
    }

    //对池子A和池子B进行交换处理,为了装备所用
    public static void AddItemInCache(GameEquip equip)
    {
        for (int i = 0; i < GameEquips.Count; i++)
        {
            //找到对应装备,然后删除已装备池,添加到未装备池
            if (equip.PartId.Equals(GameEquips[i].PartId))
            {
                //判断面板原来有没有装备装备
                for (int j = 0; j < isWearUID.Count; j++)
                {
                    //如果他两部位相等,相当于交换
                    if (equip.PosId.Equals(isWearUID[j].PosId))
                    {
                        GameEquips.Add(isWearUID[j]);
                        isWearUID.Remove(isWearUID[j]);
                        break;
                    }
                }


                //添加数据
                isWearUID.Add(GameEquips[i]);

                //删除数据
                GameEquips.Remove(GameEquips[i]);


                //重新排序
                GeneralSort();

                return;
            }
        }
    }

    //对池子A和池子B进行添加处理,为了卸载下所用
    public static void RemoveItemInCache(GameEquip equip)
    {
        for (int i = 0; i < isWearUID.Count; i++)
        {
            //找到对应装备,然后删除已装备池,添加到未装备池
            if (equip.PartId.Equals(isWearUID[i].PartId))
            {
                //添加数据
                GameEquips.Add(isWearUID[i]);

                //删除数据
                isWearUID.Remove(isWearUID[i]);

                //重新排序
                GeneralSort();

                return;
            }
        }
    }


    //排序方法,对缓存排序,按照类型应该有好几种
    public static void GeneralSort()
    {
        GameEquips.Sort(new EquipComparer());
        if (MaterialsTypeList.Count != 0)
        {
            MaterialsTypeList.Sort(new EquipComparer());
        }
    }


    public static void LevelSort()
    {
    }


    public static void PortSort()
    {
    }

    // 排序比较器
    public class EquipComparer : IComparer<GameEquip>
    {
        public int Compare(GameEquip obj1, GameEquip obj2)
        {
            // 品质由高到低
            if (obj1.Quality > obj2.Quality)
                return -1;
            else if (obj1.Quality < obj2.Quality)
                return 1;

            // S在前，普通在后
            // if (obj1.IsS.Equals("S") && !obj2.IsS.Equals("S"))
            //     return -1;
            // else if (!obj1.IsS.Equals("S") && obj2.IsS.Equals("S"))
            //     return 1;

            // 部位id由小到大
            if (obj1.PosId < obj2.PosId)
                return -1;
            else if (obj1.PosId > obj2.PosId)
                return 1;

            // 等级由高到低
            if (obj1.Level > obj2.Level)
                return -1;
            else if (obj1.Level < obj2.Level)
                return 1;

            // 装备id由大到小
            if (obj1.EquipId > obj2.EquipId)
                return -1;
            else if (obj1.EquipId < obj2.EquipId)
                return 1;

            // uid从小到大
            if (obj1.PartId < obj2.PartId)
                return -1;
            else if (obj1.PartId > obj2.PartId)
                return 1;

            return 0;
        }
    }

    //字典排序比较器
    public static void MaterialsSort()
    {
        MaterialsTypeDic = MaterialsTypeDic.OrderByDescending(pair => pair.Key.Quality).ThenBy(pair => pair.Key.PosId)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        ;
    }


    //从未装备的装备池子或者从已经装备的装备池子拿东西
    public static GameEquip FindEquip(long UID, out bool isWearEquip)
    {
        GameEquip equip = new GameEquip();
        //从未装备的池子拿
        for (int i = 0; i < GameEquips.Count; i++)
        {
            if (UID == GameEquips[i].PartId)
            {
                equip = GameEquips[i];
                isWearEquip = false;
                return equip;
            }
        }

        //从装备装备的池子拿
        for (int i = 0; i < isWearUID.Count; i++)
        {
            if (UID == isWearUID[i].PartId)
            {
                equip = isWearUID[i];
                isWearEquip = true;
                return equip;
            }
        }

        //从通用合成装备池子里面拿
        for (int i = 0; i < allCompoundMaterial.Count; i++)
        {
            if (UID == allCompoundMaterial[i].PartId)
            {
                equip = allCompoundMaterial[i];
                isWearEquip = false;
                return equip;
            }
        }

        isWearEquip = false;
        return null;
    }


    public static GameEquip FindEquip(long UID, out bool isWearEquip, out int index)
    {
        GameEquip equip = new GameEquip();
        //从未装备的池子拿
        for (int i = 0; i < GameEquips.Count; i++)
        {
            if (UID == GameEquips[i].PartId)
            {
                equip = GameEquips[i];
                isWearEquip = false;
                index = i;
                return equip;
            }
        }

        //从装备装备的池子拿
        for (int i = 0; i < isWearUID.Count; i++)
        {
            if (UID == isWearUID[i].PartId)
            {
                equip = isWearUID[i];
                isWearEquip = true;
                index = i;
                return equip;
            }
        }

        isWearEquip = false;
        index = -1;
        return null;
    }

    //降级替换缓存中的装备
    public static void DecreaseLevelEquip(GameEquip equip)
    {
        //在装备的装备中查找
        for (int i = 0; i < isWearUID.Count; i++)
        {
            if (equip.Equals(isWearUID[i]))
            {
                isWearUID[i] = equip;
                return;
            }
        }

        //先在未装备的装备中查找
        for (int i = 0; i < GameEquips.Count; i++)
        {
            if (equip.PartId.Equals(GameEquips[i].PartId))
            {
                GameEquips[i] = equip;
                return;
            }
        }
    }


    public static void DecreaseQuailtyEquip(GameEquip equip, bool isWear, int equipIndex)
    {
        //在未装备的装备中替换
        if (!isWear)
        {
            GameEquips[equipIndex] = equip;
        }
        else //在装备的装备中替换
        {
            isWearUID[equipIndex] = equip;
        }

        return;
    }


    public static void FindCompoundEquipNum(GameObject equipObj)
    {
        if (CompoundTotalNum != 0) return;
        var tbequip_quality = ConfigManager.Instance.Tables.Tbequip_quality;

        long equipUid = equipObj.GetComponent<EquipItemBtnTest>().UID;
        bool isWear = false;
        //拿到对应的装备
        GameEquip newEquip = FindEquip(equipUid, out isWear);
        //计算拿到总量
        CompoundTotalNum = tbequip_quality.Get(newEquip.Quality).mergeRule[1];
        //标记为主料
        CompoundIndexNum = 0;

        //清楚合成装备的列表
        CompoundListEquip.Clear();
    }


    public static void AddToCompoundListEquip(GameObject equipObj, bool isEquip = false)
    {
        long equipUid = 0;
        if (!isEquip) equipUid = equipObj.GetComponent<EquipItemBtnTest>().UID;
        else equipUid = equipObj.GetComponent<EquipMaterialItemTest>().UID;
        bool isWear = false;
        //拿到对应的装备
        GameEquip newEquip = FindEquip(equipUid, out isWear);
        //添加装备
        //CompoundListEquip.Add(newEquip);
        Debug.Log(newEquip.PartId);

        //CompoundListEquip
        CompoundListEquip.Insert(CompoundIndexNum, newEquip);
    }

    public static void removeCompoundListEquip(GameObject equipObj, bool isEquip = false)
    {
        long equipUid = 0;
        if (!isEquip) equipUid = equipObj.GetComponent<EquipItemBtnTest>().UID;
        else equipUid = equipObj.GetComponent<EquipMaterialItemTest>().UID;
        bool isWear = false;
        //拿到对应的装备
        GameEquip newEquip = FindEquip(equipUid, out isWear);

        //移除装备
        //添加装备
        CompoundListEquip.Remove(newEquip);
    }


    //去操纵所有相同部位的装备缓存,不包括通用合成材料
    public static void addToAllCompoundEquips(GameEquip equip)
    {
        //清空缓存
        allCompoundEquip.Clear();

        //对三个缓存进行查找,查找相同的不为
        for (int i = 0; i < isWearUID.Count; i++)
        {
            if (equip.PosId.Equals(isWearUID[i].PosId)) allCompoundEquip.Add(isWearUID[i]);
        }

        for (int i = 0; i < GameEquips.Count; i++)
        {
            if (equip.PosId.Equals(GameEquips[i].PosId)) allCompoundEquip.Add(GameEquips[i]);
        }
    }

    public static void addToAllCompoundMaterial(GameEquip equip)
    {
        //清空缓存
        allCompoundMaterial.Clear();

        for (int i = 0; i < MaterialsTypeList.Count; i++)
        {
            if (equip.PosId.Equals(MaterialsTypeList[i].PosId))
            {
                allCompoundMaterial.Add(MaterialsTypeList[i]);
            }
        }
    }

    public static void RemoveequipItem(GameEquip equip)
    {
        for (int i = 0; i < GameEquips.Count; i++)
        {
            for (int j = 0; j < CompoundListEquip.Count; j++)
            {
                if (CompoundListEquip[j].Equals(GameEquips[i])) GameEquips.Remove(GameEquips[i]);
            }
        }

        for (int i = 0; i < isWearUID.Count; i++)
        {
            for (int j = 0; j < CompoundListEquip.Count; j++)
            {
                if (CompoundListEquip[j].Equals(isWearUID[i])) isWearUID.Remove(isWearUID[i]);
            }
        }

        GameEquips.Add(equip);

        GeneralSort();
    }


    //返回对应的下标,如果没有返回-1
    public static int indexOfCompoundList(GameObject obj)
    {
        var BtnTestComponent = obj.GetComponent<EquipItemBtnTest>();
        for (int i = 0; i < CompoundListEquip.Count; i++)
        {
            if (CompoundListEquip[i].PartId.Equals(BtnTestComponent.UID))
            {
                return CompoundListEquip.IndexOf(CompoundListEquip[i]);
            }
        }

        return -1;
    }


    public static int indexOfCompoundMaterilList(GameObject obj)
    {
        var BtnTestComponent = obj.GetComponent<EquipMaterialItemTest>();
        for (int i = 0; i < CompoundListEquip.Count; i++)
        {
            if (CompoundListEquip[i].PartId.Equals(BtnTestComponent.UID))
            {
                return CompoundListEquip.IndexOf(CompoundListEquip[i]);
            }
        }

        return -1;
    }


    //查找能否一键合成
    public static bool isRapidCoumpoundEnough()
    {
        int RapidCoumpoundNum = 0;

        List<GameEquip> allgameEquip = new List<GameEquip>();
        for (int i = 0; i < isWearUID.Count; i++)
        {
            allgameEquip.Add(isWearUID[i]);
        }

        for (int i = 0; i < GameEquips.Count; i++)
        {
            allgameEquip.Add(GameEquips[i]);
        }

        for (int i = 0; i < allgameEquip.Count; i++)
        {
            // if (allgameEquip[i].Type <= 3)
            // {
            //     for (int j = i; j < allgameEquip.Count; j++)
            //     {
            //         if (allgameEquip[j].Type.Equals(allgameEquip[i].Type)&& allgameEquip[j].PosId.Equals(allgameEquip[i].PosId)) RapidCoumpoundNum++;
            //     }
            //
            //     if (RapidCoumpoundNum >= 3) return true;
            // }

            RapidCoumpoundNum = 0;
        }


        return false;
    }
}