//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-07-17 12:15:10
//---------------------------------------------------------------------

using cfg.blobstruct;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Main
{
    //挂炸弹 效果id是目标时 每一个都是单独的buff
    public partial struct Buff_14034403 : IBuff
    {
        ///<summary>0
        ///buff的id
        ///</summary>
        public int id;

        ///<summary>1
        ///buff添加时间点/刻 替换优先级
        ///</summary>
        public float startTime;

        ///<summary>2
        ///buff已经存在了多少时间了，单位：帧
        ///</summary>
        public int timeElapsed;

        ///<summary>3
        ///剩余多久，单位：秒
        ///</summary>
        public float duration;

        ///<summary>4
        ///是否是一个永久的buff，永久的duration不会减少，但是timeElapsed还会增加
        ///</summary>
        public bool permanent;

        ///<summary>5
        ///buff的施法者是谁，当然可以是空的
        ///</summary>
        public Entity caster;

        ///<summary>6
        ///buff的携带者，实际上是作为参数传递给脚本用，具体是谁，可定是所在控件的this.gameObject了
        ///</summary>
        public Entity carrier;

        /// <summary>7
        /// 替换类型 用于同类buff的替换 仅用于免疫和控制和属性变更 为属性变更时 x是类型 y是上限
        /// </summary>
        public int2 changeType;

        /// <summary>8
        /// 是否能够清除 用于不同类buff的替换 仅用于控制类
        /// new字段
        /// </summary>
        public int canClear;

        /// <summary>9
        /// 护盾层数 仅作用于免疫
        /// new
        /// </summary>
        public int immuneStack;

        /// <summary>10
        /// 从 effect传入的的方向
        /// new
        /// </summary>
        public float3 direction;

        /// <summary>11
        /// buff的状态类型 分无状态 正面状态和负面状态
        /// </summary>
        public int buffState;

        /// <summary>12
        /// float4-1:output 2:bonus_type 3:calc_type and pra
        /// </summary>
        public BuffArgsNew argsNew;

        ///<summary>13
        ///buff的优先级，优先级越低的buff越后面执行，这是一个非常重要的属性
        ///比如经典的“吸收50点伤害”和“受到的伤害100%反弹给攻击者”应该反弹多少，取决于这两个buff的priority谁更高
        ///</summary>
        public int priority;

        /// <summary>14
        /// 元素的概率 
        /// </summary>
        public int power;

        /// <summary>15
        /// 用来处理ontick的逻辑 =0时不执行ontick 
        /// </summary>
        public int tickTime;

        /// <summary>16
        /// 用来处理移动伤害的逻辑  第一个float3的x为每移动多少米 y为当前移动总距离 z为1时该移动伤害才生效！！！！
        /// 第二个记录上帧的位置 
        /// </summary>
        public float3x2 distancePar;

        /// <summary>17
        /// 是否是弹幕
        /// </summary>
        public bool isBullet;

        /// <summary>
        ///  18 buff2里上一份属性变更的值
        /// </summary>
        public int oldValue;

        /// <summary>
        /// 19 是否禁用特效
        /// </summary>
        public bool isDisableSpecial;

        /// <summary>
        /// 20 技能id
        /// </summary>
        public int skillId;

        public void OnOccur(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {



        }

        public void OnRemoved(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            ref var skillEffect =
                ref inData.globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;
            ref var elementsTable =
                ref inData.globalConfigData.value.Value.configTbskill_effectElements.configTbskill_effectElements;

            int calueValue = 0;
            for (int i = 0; i < skillEffect.Length; i++)
            {
                if (skillEffect[i].id == id)
                {
                    calueValue = skillEffect[i].calcTypePara[0];
                    break;
                }
            }

            for (int i = 0; i < skillEffect.Length; i++)
            {
                if (skillEffect[i].id == calueValue)
                {
                    if (skillEffect[i].target == 3)
                    {
                        ref var elementList = ref skillEffect[i].elementList;

                        //TODO:加自身buff
                        //Debug.LogError($"effectID{effectID}");
                        //AddBuffToSelf(ref elementList, caster, index);

                    }
                    else
                    {
                        UnityEngine.Debug.Log("GenerateEffectAttack");
                        GenerateEffectAttack(ref skillEffect[i], carrier, inData.sortkey, caster, inData, ref refData);
                    }


                }
            }
        }



        private void GenerateEffectAttack(ref ConfigTbskill_effectNew skillEffect, Entity locer, int sortKey, Entity caster, BuffData_ReadOnly inData, ref BuffData_ReadWrite refData)
        {



            LocalTransform castPos = refData.cdfeLocalTransform[locer];

           
            ref var elementConfig = ref inData.globalConfigData.value.Value.configTbskill_effectElements
                .configTbskill_effectElements;
            var rangeType = skillEffect.rangeType;
            FixedString128Bytes modelName = $"model_skilleffect_{rangeType}";
            var prefab = inData.prefabMapData.prefabHashMap[modelName];
            var searchType = skillEffect.searchType;
            ref var searchPar = ref skillEffect.searchTypePara;
            ref var rangPar = ref skillEffect.rangeTypePara;
            var deviateType = skillEffect.deviateType;
            ref var deviateTypePar = ref skillEffect.deviateTypePara;
            ref var targetParlist = ref skillEffect.targetPara;
            float targetPar = 0;
            for (int j = 0; j < targetParlist.Length; j++)
            {
                targetPar += math.pow(2, targetParlist[j]);
            }

            castPos.Position = refData.cdfeLocalTransform[locer].Position;
            float3 dir = math.normalize(refData.cdfeLocalTransform[carrier].Position - refData.cdfeLocalTransform[caster].Position);
            if (deviateType == 1)
            {
                //TODO:
                castPos.Position = BuffHelper.GetRandomPointInCircle(castPos.Position, deviateTypePar[0] / 1000f, 0,
                    (uint)(inData.timeTick * 10));
            }

            if (rangeType == 4)
            {
                castPos.Position += dir * rangPar[3] / 1000f;
            }

            var ins = refData.ecb.Instantiate(sortKey, prefab);
            var preTran = refData. cdfeLocalTransform[prefab];
            preTran.Position = castPos.Position;
            preTran.Rotation = castPos.Rotation;
            //矩形 宽高
            //扇形 半径半径度数
            //圆形 半径半径0
            //环装扇形 最小半径最大半径度数
            switch (rangeType)
            {
                case 0:

                    preTran.Scale = 5f;

                    break;
                case 1:

                    if (rangPar[0] == rangPar[1])
                    {
                        Debug.Log("==");
                        preTran.Scale *= rangPar[0] / 1000f;
                    }
                    else
                    {
                        Debug.Log("!=");
                        refData. ecb.AddComponent(sortKey, ins, new PostTransformMatrix
                        {
                            Value = float4x4.Scale(preTran.Scale * (rangPar[0] / 1000f),
                                preTran.Scale * (rangPar[1] / 1000f), 1)
                        });
                        var collider = PhysicsHelper.CreateBoxColliderAndSetSize(refData.cdfePhysicsCollider[prefab],
                            new float3(rangPar[0] / 1000f, rangPar[1] / 1000f, 1));
                        refData.ecb.SetComponent(sortKey, ins, collider);
                    }

                    break;
                case 2:
                    preTran.Scale *= rangPar[1] / 1000f;

                    break;
                case 3:
                    preTran.Scale *= rangPar[0] / 1000f;
                    break;
                case 4:
                    preTran.Scale *= (rangPar[1] + rangPar[4]) / 1000f;
                    break;
            }

            //preTran.Scale *= 10;
         
            Debug.Log(
                   $"pretran:Position{preTran.Position},scale{preTran.Scale}");
            refData. ecb.SetComponent(sortKey, ins, preTran);

            #region SpecialEffects

            UnityHelper.SpawnSpecialEffect(ref refData.ecb, sortKey, inData.eT, refData. cdfeLocalTransform,inData.cdfePostTransformMatrix, inData. gameTimeData,
                inData.globalConfigData,
                ref skillEffect.specialEffects,
                inData.prefabMapData, locer, false, skillEffect.skillId, preTran, preTran.Position, default,1);

            #endregion

            //Debug.Log($"bullet skill skillEffect {skillEffect.skillId}");
            var data = new SkillAttackData
            {
                data = new SkillAttack_0
                {
                    id = skillEffect.id,
                    duration = 0f,
                    caster = caster,
                    skillID = skillEffect.skillId,
                    targetPos = dir,
                    tirggerCount = 1,
                    holder = locer,
                    isBulletDamage = false,
                }.ToSkillAttack()
            };

            refData.ecb.AddComponent(sortKey, ins, data);
            refData.ecb.AddComponent(sortKey, ins, new TargetData
            {
                tick = 0,
                AttackWith = (uint)(targetPar)
            });
            refData.ecb.AddBuffer<Buff>(sortKey, ins);
            refData.ecb.AddComponent(sortKey, ins, new ElementData { id = skillEffect.elementId });
        }
    

    





        public void OnUpdate(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnHit(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnBeHurt(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnTick(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

       public void OnKill(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnBeforeBeKilled(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnBeKilled(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnPerUnitMove(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            //ref battleConstConfig=ref inData.globalConfigData.value.Value.configtbb
            float moveDis = 5000 / 1000f;
            float moveDamage = 200 / 10000f * refData.cdfeChaStats[carrier].chaProperty.maxHp;
        }
    }
}