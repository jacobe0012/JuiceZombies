using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace Main
{
    /// <summary>
    /// Player独有的字段
    /// </summary>
    [Serializable]
    public struct PlayerData : IComponentData
    {
        /// <summary>
        /// 当前玩家特有属性
        /// </summary>
        public PlayerSelfData playerData;

        /// <summary>
        /// 所有自加属性
        /// </summary>
        public PlayerOtherData playerOtherData;

        /// <summary>
        /// 模板属性，根据此属性值计算当前属性值
        /// </summary>
        //public ChaProperty chaProperty;

        /// <summary>
        /// 模板属性，根据此属性值计算当前属性值
        /// </summary>
        //public PlayerSelfData defaultPlayerData;
    }

    [Serializable]
    public struct PlayerOtherData
    {
        /// <summary>
        /// 隐身
        /// </summary>
        public bool cloak;

        /// <summary>
        /// 自加 当前武器技能冷却时间
        /// </summary>
        public float curWeaponSkillCoolDown;

        /// <summary>
        /// 自加 武器技能冷却时间
        /// </summary>
        public float weaponSkillCoolDown;

        /// <summary>
        /// 自加 击杀小怪
        /// </summary>
        public int killLittleMonster;

        /// <summary>
        /// 自加 击杀精英怪数量
        /// </summary>
        public int killElite;

        /// <summary>
        /// 自加 击杀boss数量
        /// </summary>
        public int killBoss;

        /// <summary>
        /// 自加 击杀bossId列表
        /// </summary>
        public FixedList32Bytes<int> killBossIdList;

        /// <summary>
        /// 自加 击杀monsterId列表(含boss)
        /// </summary>
        public FixedList512Bytes<int> killMonsterIdList;

        /// <summary>
        /// 自加 玩家当前武器技能id
        /// </summary>
        public int weaponSkillId;

        /// <summary>
        /// 自加 玩家当前武器id  
        /// </summary>
        public int weaponId;

        /// <summary>
        /// 自加 玩家当前武器品质 废弃 使用 weaponId %100
        /// </summary>
        //public int weaponQuality;
        public DamageSumInfo playerDamageSumInfo;

        /// <summary>
        /// 自加 boss战之前的位置
        /// </summary>
        public float3 bossBeforePos;

        /// <summary>
        /// 自加 ,武器技能升阶buff id
        /// </summary>
        public int4 weaponAdditionID;

        /// <summary>
        /// 自加 所有未完成的新手引导id
        /// </summary>
        public FixedList128Bytes<int> guideList;

        /// <summary>
        /// 自加 所有带到局外的item
        /// </summary>
        public FixedList128Bytes<FixedString64Bytes> outerItemList;

        /// <summary>
        /// 自加 是否在boss场景
        /// </summary>
        public bool isBossFight;

        /// <summary>
        /// 自加 Boss的实体
        /// </summary>
        public Entity BossEntity;

        /// <summary>
        /// 自加 x是boss战场景size yz是pos的xy
        /// </summary>
        public float3 bossScenePos;

        /// <summary>
        /// 技能临时购买次数
        /// </summary>
        public int skillTempFreeBuyCount;

        /// <summary>
        /// 玩家总复活次数
        /// </summary>
        public int rebirthCount;

        /// <summary>
        /// 玩家拾取范围自加
        /// </summary>
        public int pickupRadius;
        /// <summary>
        /// entity集合
        /// </summary>
        //public EntityGroup entityGroup;
    }

    /// <summary>
    /// 统计的造成伤害
    /// </summary>
    public struct DamageSumInfo
    {
        public float sumDamage;
        public float weaponDamage;
        public float collideDamage;
        public float areaDamage;

        public float enemy2enemy;
        public float enemy2obstan;
    }

    /// <summary>
    /// 存储玩家拾取的道具 key 道具id value为数量
    /// </summary>
    // [Serializable]
    // public struct PropsData : IBufferElementData
    // {
    //     [UnityEngine.SerializeField] public UnsafeHashMap<int, int> idToNum;
    // }
    public struct PlayerSelfData
    {
        /// <summary>
        /// 201100_等级_类型:1_是否玩家独有:1
        /// </summary>
        public int level;


        /// <summary>
        /// 201200_经验_类型:1_是否玩家独有:1
        /// </summary>
        public int exp;


        /// <summary>
        /// 201220_经验获取加成_类型:2_是否玩家独有:1
        /// </summary>
        public int expRatios;


        /// <summary>
        /// 201300_金币_类型:1_是否玩家独有:1
        /// </summary>
        public int gold;


        /// <summary>
        /// 201320_金币拾取加成_类型:2_是否玩家独有:1
        /// </summary>
        public int goldRatios;


        /// <summary>
        /// 201400_图纸_类型:1_是否玩家独有:1
        /// </summary>
        public int paper;


        /// <summary>
        /// 201420_图纸掉率加成_类型:2_是否玩家独有:1
        /// </summary>
        public int paperRatios;


        /// <summary>
        /// 201500_装备_类型:1_是否玩家独有:1
        /// </summary>
        public int equip;


        /// <summary>
        /// 201520_装备掉率加成_类型:2_是否玩家独有:1
        /// </summary>
        public int equipRatios;


        /// <summary>
        /// 201600_拾取范围加成_类型:2_是否玩家独有:1
        /// </summary>
        public int pickUpRadiusRatios;


        /// <summary>
        /// 201700_杀敌数_类型:1_是否玩家独有:1
        /// </summary>
        public int killEnemy;


        /// <summary>
        /// 202140_道具恢复加成_类型:2_是否玩家独有:1
        /// </summary>
        public int propsRecoveryRatios;


        /// <summary>
        /// 202150_道具恢复固定加成_类型:1_是否玩家独有:1
        /// </summary>
        public int propsRecoveryAdd;


        /// <summary>
        /// 214000_技能免费购买次数_类型:1_是否玩家独有:1
        /// </summary>
        public int skillFreeBuyCount;


        /// <summary>
        /// 214100_技能购买价格万分比_类型:2_是否玩家独有:1
        /// </summary>
        public int buySkillRatios;


        /// <summary>
        /// 214200_刷新商店价格万分比_类型:2_是否玩家独有:1
        /// </summary>
        public int refreshShopRatios;


        /// <summary>
        /// 214300_技能商店免费刷新次数_类型:1_是否玩家独有:1
        /// </summary>
        public int skillRefreshCount;

        /// <summary>
        /// 214700_临时免费刷新次数_类型:1_是否玩家独有:1
        /// </summary>
        public int skillTempRefreshCount;

        /// <summary>
        /// 214400_蓝色技能权重提升万分比_类型:2_是否玩家独有:1
        /// </summary>
        public int skillWeightIncrease1;


        /// <summary>
        /// 214500_紫色技能权重提升万分比_类型:2_是否玩家独有:1
        /// </summary>
        public int skillWeightIncrease2;


        /// <summary>
        /// 214600_金色技能权重提升万分比_类型:2_是否玩家独有:1
        /// </summary>
        public int skillWeightIncrease3;


        /// <summary>
        /// 219100_超级推力概率_类型:2_是否玩家独有:1
        /// </summary>
        public int superPushForceChance;


        /// <summary>
        /// 219200_极限推力概率_类型:2_是否玩家独有:1
        /// </summary>
        public int maxPushForceChance;


        /// <summary>
        /// 220100_对普通怪物伤害加成_类型:2_是否玩家独有:1
        /// </summary>
        public int normalMonsterDamageRatios;


        /// <summary>
        /// 220200_对稀有怪物伤害加成_类型:2_是否玩家独有:1
        /// </summary>
        public int specialMonsterDamageRatios;


        /// <summary>
        /// 220300_对boss伤害加成_类型:2_是否玩家独有:1
        /// </summary>
        public int bossMonsterDamageRatios;


        /// <summary>
        /// 221100_武器技能额外次数_类型:1_是否玩家独有:1
        /// </summary>
        public int weaponSkillExtraCount;
    }

    //[InternalBufferCapacity(1)]
    // public struct SkillInfo : IBufferElementData
    // {
    //     /// <summary>10
    //     /// 所属技能id
    //     /// </summary>
    //     public int skillId;
    // }

    public struct PlayerProps : IBufferElementData
    {
        //拾取道具id count
        public int propId;
        public int count;
    }
}