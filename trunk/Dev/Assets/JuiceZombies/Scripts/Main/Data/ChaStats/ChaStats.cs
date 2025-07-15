//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-07-19 11:51:46
//---------------------------------------------------------------------

using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Main
{
    [Serializable]
    public struct ChaStats : IComponentData
    {
        // public ChaProperty TotalProperty
        // {
        //     get
        //     {
        //         //TODD:改
        //         var totalProp = new ChaProperty
        //         {
        //             hp = chaResource.hp + buffsProperty.hp + itemsProperty.hp,
        //             hpRecoveryPerSecond = chaResource.hpRecoveryPerSecond +
        //                                   buffsProperty.hpRecoveryPerSecond +
        //                                   itemsProperty.hpRecoveryPerSecond,
        //             hpRecoveryPlusFixed = chaResource.hpRecoveryPlusFixed +
        //                                   buffsProperty.hpRecoveryPlusFixed +
        //                                   itemsProperty.hpRecoveryPlusFixed,
        //             hpRecoveryPlusRatios = chaResource.hpRecoveryPlusRatios +
        //                                    buffsProperty.hpRecoveryPlusRatios +
        //                                    itemsProperty.hpRecoveryPlusRatios,
        //             rebirthCount = chaProperty.rebirthCount +
        //                      buffsProperty.rebirthCount +
        //                      itemsProperty.rebirthCount,
        //
        //             atk = chaProperty.atk + buffsProperty.atk + itemsProperty.atk,
        //             atkPlus = chaProperty.atkPlus +
        //                       buffsProperty.atkPlus +
        //                       itemsProperty.atkPlus,
        //
        //             critical = chaProperty.critical + buffsProperty.critical + itemsProperty.critical,
        //             criticalDamage = chaProperty.criticalDamage +
        //                              buffsProperty.criticalDamage +
        //                              itemsProperty.criticalDamage,
        //             damagePlus = chaProperty.damagePlus +
        //                          buffsProperty.damagePlus +
        //                          itemsProperty.damagePlus,
        //             collDamagePlus = chaProperty.collDamagePlus +
        //                              buffsProperty.collDamagePlus +
        //                              itemsProperty.collDamagePlus,
        //             continuousCollDamagePlus = chaProperty.continuousCollDamagePlus +
        //                                        buffsProperty.continuousCollDamagePlus +
        //                                        itemsProperty.continuousCollDamagePlus,
        //             dodge = chaProperty.dodge + buffsProperty.dodge + itemsProperty.dodge,
        //             recoveryTime = chaProperty.recoveryTime +
        //                            buffsProperty.recoveryTime +
        //                            itemsProperty.recoveryTime,
        //
        //             coolDown = chaProperty.coolDown + buffsProperty.coolDown + itemsProperty.coolDown,
        //             skillRange = chaProperty.skillRange +
        //                          buffsProperty.skillRange +
        //                          itemsProperty.skillRange,
        //             curPushForce = chaProperty.curPushForce +
        //                            buffsProperty.curPushForce +
        //                            itemsProperty.curPushForce,
        //             curCoolDown = chaProperty.curCoolDown + buffsProperty.curCoolDown + itemsProperty.curCoolDown,
        //
        //             pushForce = chaProperty.pushForce + buffsProperty.pushForce + itemsProperty.pushForce,
        //
        //             curMoveSpeed = chaResource.curMoveSpeed + buffsProperty.curMoveSpeed + itemsProperty.curMoveSpeed,
        //             maxMoveSpeed = chaProperty.maxMoveSpeed + buffsProperty.maxMoveSpeed + itemsProperty.maxMoveSpeed,
        //             acceleration = chaProperty.acceleration + buffsProperty.acceleration + itemsProperty.acceleration,
        //             maxMoveSpeedPlus = chaProperty.maxMoveSpeedPlus +
        //                                buffsProperty.maxMoveSpeedPlus +
        //                                itemsProperty.maxMoveSpeedPlus,
        //             basicMovementSpeed = chaProperty.basicMovementSpeed +
        //                                  buffsProperty.basicMovementSpeed +
        //                                  itemsProperty.basicMovementSpeed,
        //             basicAcceleration = chaProperty.basicAcceleration +
        //                                 buffsProperty.basicAcceleration +
        //                                 itemsProperty.basicAcceleration,
        //             stallAcceleration = chaProperty.stallAcceleration +
        //                                 buffsProperty.stallAcceleration +
        //                                 itemsProperty.stallAcceleration,
        //             mass = chaProperty.mass + buffsProperty.mass + itemsProperty.mass,
        //             pushForcePlus = chaProperty.pushForcePlus +
        //                             buffsProperty.pushForcePlus +
        //                             itemsProperty.pushForcePlus,
        //             basicRecoveryTime = chaProperty.basicRecoveryTime +
        //                                 buffsProperty.basicRecoveryTime +
        //                                 itemsProperty.basicRecoveryTime,
        //             speedRecoveryTime = chaProperty.speedRecoveryTime +
        //                                 buffsProperty.speedRecoveryTime +
        //                                 itemsProperty.speedRecoveryTime,
        //             reduceHurtRatios = chaProperty.reduceHurtRatios +
        //                                buffsProperty.reduceHurtRatios +
        //                                itemsProperty.reduceHurtRatios,
        //             reduceHurt = chaProperty.reduceHurt +
        //                          buffsProperty.reduceHurt +
        //                          itemsProperty.reduceHurt,
        //
        //             reduceHitBackRatios = chaProperty.reduceHitBackRatios +
        //                                   buffsProperty.reduceHitBackRatios +
        //                                   itemsProperty.reduceHitBackRatios,
        //             reduceHitBack = chaProperty.reduceHitBack +
        //                             buffsProperty.reduceHitBack +
        //                             itemsProperty.reduceHitBack,
        //
        //             maxReduceHitBack = chaProperty.maxReduceHitBack +
        //                                buffsProperty.maxReduceHitBack +
        //                                itemsProperty.maxReduceHitBack,
        //             reduceHitBackRecoveryTime = chaProperty.reduceHitBackRecoveryTime +
        //                                         buffsProperty.reduceHitBackRecoveryTime +
        //                                         itemsProperty.reduceHitBackRecoveryTime,
        //             reduceBulletHurtRatios = chaProperty.reduceBulletHurtRatios +
        //                                      buffsProperty.reduceBulletHurtRatios +
        //                                      itemsProperty.reduceBulletHurtRatios,
        //             reduceHitBackDamageRatios = chaProperty.reduceHitBackDamageRatios +
        //                                         buffsProperty.reduceHitBackDamageRatios +
        //                                         itemsProperty.reduceHitBackDamageRatios,
        //
        //             direction = default,
        //             actionSpeed = chaResource.actionSpeed + buffsProperty.actionSpeed + itemsProperty.actionSpeed,
        //         };
        //         return totalProp;
        //     }
        // }

        /// <summary>
        /// 单位当前的属性值
        /// </summary>
        public ChaProperty chaProperty;

        /// <summary>
        /// 单位环境影响的属性值
        /// </summary>
        public ChaProperty enviromentProperty;

        /// <summary>
        /// 单位的初始属性值
        /// </summary>
        //public ChaProperty defaultProperty;

        // public ChaProperty buffsProperty;
        // public ChaProperty itemsProperty;
        public ChaResource chaResource;

        //public ChaControlState chaControlState;
        public ChaControlState chaControlState;
        public ChaImmuneState chaImmuneState;

        public bool IsStrongControl(bool isPlayer = false)
        {
            return !chaImmuneState.immuneControl && chaControlState.IsStrongControl(isPlayer);
        }
    }

    [Serializable]
    public struct ChaProperty
    {
        /// <summary>
        /// 202000_生命值_类型:1_是否玩家独有:0
        /// </summary>
        public int maxHp;


        /// <summary>
        /// 202010_局外生命值_类型:1_是否玩家独有:0
        /// </summary>
        public int defaultMaxHp;


        /// <summary>
        /// 202020_生命值加成_类型:2_是否玩家独有:0
        /// </summary>
        public int hpRatios;


        /// <summary>
        /// 202030_生命值固定加成_类型:1_是否玩家独有:0
        /// </summary>
        public int hpAdd;


        /// <summary>
        /// 202040_当前生命万分比_类型:2_是否玩家独有:0
        /// </summary>
        public int curHpRatios;


        /// <summary>
        /// 202100_生命恢复_类型:1_是否玩家独有:0
        /// </summary>
        public int hpRecovery;


        /// <summary>
        /// 202110_局外生命恢复_类型:1_是否玩家独有:0
        /// </summary>
        public int defaultHpRecovery;


        /// <summary>
        /// 202120_生命恢复加成_类型:2_是否玩家独有:0
        /// </summary>
        public int hpRecoveryRatios;


        /// <summary>
        /// 202130_生命恢复固定加成_类型:1_是否玩家独有:0
        /// </summary>
        public int hpRecoveryAdd;


        /// <summary>
        /// 203000_攻击力_类型:1_是否玩家独有:0
        /// </summary>
        public int atk;


        /// <summary>
        /// 203010_局外攻击力_类型:1_是否玩家独有:0
        /// </summary>
        public int defaultAtk;


        /// <summary>
        /// 203020_攻击力加成_类型:2_是否玩家独有:0
        /// </summary>
        public int atkRatios;


        /// <summary>
        /// 203030_攻击力固定加成_类型:1_是否玩家独有:0
        /// </summary>
        public int atkAdd;


        /// <summary>
        /// 204000_复活次数_类型:1_是否玩家独有:0
        /// </summary>
        public int rebirthCount;

        /// <summary>
        /// 204010_复活次数小恶魔_类型:1_是否玩家独有:0
        /// </summary>
        public int rebirthCount1;

        /// <summary>
        /// 205000_暴击率_类型:2_是否玩家独有:0
        /// </summary>
        public int critical;


        /// <summary>
        /// 205010_临时暴击率_类型:2_是否玩家独有:0
        /// </summary>
        public int tmpCritical;


        /// <summary>
        /// 205100_暴击伤害率_类型:2_是否玩家独有:0
        /// </summary>
        public int criticalDamageRatios;


        /// <summary>
        /// 206120_伤害加成_类型:2_是否玩家独有:0
        /// </summary>
        public int damageRatios;


        /// <summary>
        /// 206130_伤害固定加成_类型:1_是否玩家独有:0
        /// </summary>
        public int damageAdd;


        /// <summary>
        /// 206220_伤害减免_类型:2_是否玩家独有:0
        /// </summary>
        public int reduceHurtRatios;


        /// <summary>
        /// 206230_伤害固定减免_类型:1_是否玩家独有:0
        /// </summary>
        public int reduceHurtAdd;


        /// <summary>
        /// 206240_弹幕伤害减免_类型:2_是否玩家独有:0
        /// </summary>
        public int reduceBulletRatios;


        /// <summary>
        /// 206250_受到玩家伤害变更_类型:2_是否玩家独有:0
        /// </summary>
        public int changedFromPlayerDamage;


        /// <summary>
        /// 207000_移动速度_类型:1_是否玩家独有:0
        /// </summary>
        public int maxMoveSpeed;


        /// <summary>
        /// 207010_局外移动速度_类型:1_是否玩家独有:0
        /// </summary>
        public int defaultMaxMoveSpeed;


        /// <summary>
        /// 207020_移动速度加成_类型:2_是否玩家独有:0
        /// </summary>
        public int maxMoveSpeedRatios;


        /// <summary>
        /// 207030_移动速度固定加成_类型:_是否玩家独有:0
        /// </summary>
        public int maxMoveSpeedAdd;


        /// <summary>
        /// 207100_速度恢复时间(0到最大)_类型:1_是否玩家独有:0
        /// </summary>
        public int speedRecoveryTime;

        /// <summary>
        /// 208000_角色质量_类型:1_是否玩家独有:0
        /// </summary>
        public int mass;

        /// <summary>
        /// 208010_局外角色质量_类型:1_是否玩家独有:0
        /// </summary>
        public int defaultMass;

        /// <summary>
        /// 208020_质量加成_类型:2_是否玩家独有:0
        /// </summary>
        public int massRatios;

        /// <summary>
        /// 209000_角色推力_类型:1_是否玩家独有:0
        /// </summary>
        public int pushForce;

        /// <summary>
        /// 209010_局外角色推力_类型:1_是否玩家独有:0
        /// </summary>
        public int defaultPushForce;

        /// <summary>
        /// 209020_推力加成_类型:2_是否玩家独有:0
        /// </summary>
        public int pushForceRatios;

        /// <summary>
        /// 209030_推力固定加成_类型:_是否玩家独有:0
        /// </summary>
        public int pushForceAdd;

        /// <summary>
        /// 210000_击退减免_类型:2_是否玩家独有:0
        /// </summary>
        public int reduceHitBackRatios;

        /// <summary>
        /// 211000_闪避率_类型:2_是否玩家独有:0
        /// </summary>
        public int dodge;

        /// <summary>
        /// 212000_抵挡层数_类型:1_是否玩家独有:0
        /// </summary>
        public int shieldCount;

        /// <summary>
        /// 213000_局外技能冷却减免_类型:2_是否玩家独有:0
        /// </summary>
        public int defaultcoolDown;

        /// <summary>
        /// 215000_局外弹幕范围加成_类型:2_是否玩家独有:0
        /// </summary>
        public int defaultBulletRangeRatios;

        /// <summary>
        /// 218100_撞击伤害加成_类型:2_是否玩家独有:0
        /// </summary>
        public int collideDamageRatios;

        /// <summary>
        /// 218200_连续撞击伤害加成_类型:2_是否玩家独有:0
        /// </summary>
        public int continuousCollideDamageRatios;

        /// <summary>
        /// 222100_体型大小_类型:2_是否玩家独有:0
        /// </summary>
        public int scaleRatios;
    }

    public struct ChaResource
    {
        /// <summary>
        ///自加 是否死亡
        /// </summary>
        public bool isDead;

        /// <summary>
        /// 202040  当前生命
        /// </summary>
        public int hp;

        /// <summary>
        /// 自加 当前推力  废弃
        /// </summary>
        public int curPushForce;

        /// <summary>
        /// 自加 当前移动速度
        /// </summary>
        public float curMoveSpeed;

        /// <summary>
        /// 自加 最后移动方向
        /// </summary>
        public float3 direction;

        /// <summary>
        /// 自加 连续碰撞次数
        /// </summary>
        public int continuousCollCount;

        /// <summary>
        /// 自加 行动速度，和移动速度不同，他是增加角色行动速度，也就是变化timeline和动画播放的scale的
        /// </summary>
        public float actionSpeed;

        /// <summary>
        /// 
        /// </summary>
        public Enviroment env;

        /// <summary>
        /// 自加 累计伤害
        /// </summary>
        public long totalDamage;
    }


    //负面类buff
    [Serializable]
    public struct ChaControlState
    {
        /// <summary>
        /// 攻击者
        /// </summary>
        public Entity attacker;

        /// <summary>
        /// 无法移动
        /// </summary>
        public bool cantMove;

        /// <summary>
        /// 无法攻击 
        /// </summary>
        //public bool cantAttack;

        /// <summary>
        /// 无法武器攻击 
        /// </summary>
        public bool cantWeaponAttack;

        /// <summary>
        /// 反向移动
        /// </summary>
        public bool counterMove;

        /// <summary>
        /// 强制移动 1远离来源 2.靠近来源
        /// </summary>
        public bool2 forcedmove;

        /// <summary>
        /// 致盲--降低玩家可视距离
        /// </summary>
        public bool restrictView;

        /// <summary>
        /// 被力场吸引 软控
        /// </summary>
        public bool softForce;

        /// <summary>
        /// 技能期间boss可以移动并且镜像翻转
        /// </summary>
        public bool bossCanMove;

        /// <summary>
        /// 技能期间boss可以移动并且镜像翻转
        /// </summary>
        public bool bossCanFlip;
        public bool IsStrongControl(bool isPlayer = false)
        {
            return cantMove || (counterMove && !isPlayer) || forcedmove.x || forcedmove.y;
        }
    }

    /// <summary>
    /// 正面的buff
    /// </summary>
    [Serializable]
    public struct ChaImmuneState
    {
        /// <summary>
        /// 免疫击退
        /// </summary>
        public bool immunePush;

        /// <summary>
        /// 免疫伤害
        /// </summary>
        public bool immuneDamage;

        /// <summary>
        /// 免疫控制
        /// </summary>
        public bool immuneControl;

        /// <summary>
        /// 免疫正面狀態 废弃
        /// </summary>
        //public bool immuneBuff;

        /// <summary>
        /// 免疫負面狀態  废弃
        /// </summary>
        //public bool immuneDeBuff;
    }
}