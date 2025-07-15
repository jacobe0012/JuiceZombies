using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Main
{
    public partial struct SkillA : ISkillOld
    {
        ///<summary>0
        ///技能的id
        ///</summary>
        public int id;

        ///<summary>1
        ///技能等级
        ///</summary>
        public int level;

        ///<summary>2
        ///冷却时间，单位秒。尽管游戏设计里面是没有冷却时间的，但是我们依然需要这个数据
        ///因为作为一个ARPG子分类，和ARPG游戏有一样的问题：一次按键（时间够久）会发生连续多次使用技能，所以得有一个GCD来避免问题
        ///当然和wow的gcd不同，这个“GCD”就只会让当前使用的技能进入0.1秒的冷却
        ///</summary>
        public float cooldown;


        ///<summary>3
        ///持续时间，单位：秒
        ///</summary>
        public float duration;


        ///<summary>4
        ///倍速，1=100%，0.1=10%是最小值
        ///</summary>
        public float timeScale;

        ///<summary>5
        ///Timeline的焦点对象也就是创建timeline的负责人，比如技能产生的timeline，就是技能的施法者
        ///</summary>
        public Entity caster;

        ///<summary>6
        ///技能已经运行了多少帧了 无需赋值
        ///</summary>
        public int tick;

        /// <summary>7
        /// 技能当前冷却时间 无需赋值
        /// </summary>
        public float curCooldown;

        ///<summary>8
        ///剩余时间，单位：秒
        ///</summary>
        public float curDuration;

        ///<summary>9
        ///距离这个技能最近的目标
        ///</summary>
        public Entity target;

        ///<summary>10
        ///技能从创建到现在总的tick
        ///</summary>
        public int totalTick;

        ///<summary>11
        ///是否时一次性释放技能
        ///</summary>
        public bool isOneShotSkill;

        ///<summary>12
        ///是否是确定位置坐标
        ///</summary>
        public bool isUseCertainPos;

        ///<summary>13
        ///一次性释放技能的坐标
        ///</summary>
        public float3 pos;

        /// <summary>
        /// ...args.c0 x存id y存次数
        /// args.c1
        /// args.c2
        /// args.c3
        /// </summary>
        public int2x4 args;

        public void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
        {
        }

        public void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
        {
            var skillBaseBallBat = inData.prefabMapData.prefabHashMap["SkillBaseBallBat"];
            var entity = refData.ecb.Instantiate(inData.sortkey, skillBaseBallBat);


            //refData.ecb.AddComponent(inData.sortkey, entity, new SkillAttackData
            //{
            //    data = new SkillAttack_FollowPlayer()
            //    {
            //        id = 9999,
            //        duration = 1,
            //        tick = 0,
            //        caster = inData.entity
            //    }.ToSkillAttack()
            //});
            refData.ecb.SetComponent(inData.sortkey, entity, new LocalTransform
            {
                Position = inData.cdfeLocalTransform[inData.player].Position,
                Scale = 15,
                Rotation = inData.cdfeLocalTransform[skillBaseBallBat].Rotation
            });
            refData.ecb.AppendToBuffer(inData.sortkey, entity, new SkillHitEffectBuffer
            {
                buffID = 40000005,
                buffArgs = new float3x4
                {
                    c0 = new float3(8000, 209000, 10000),
                    c1 = default,
                    c2 = default,
                    c3 = default
                }
            });
            refData.ecb.AppendToBuffer(inData.sortkey, entity, new SkillHitEffectBuffer
            {
                buffID = 30000003,
                buffArgs = new float3x4
                {
                    c0 = new float3(12000, 203000, 10000),
                    c1 = default,
                    c2 = default,
                    c3 = default
                }
            });
            refData.ecb.AppendToBuffer(inData.sortkey, entity, new SkillHitEffectBuffer
            {
                buffID = 30000004,
                buffArgs = new float3x4
                {
                    c0 = new float3(5, 0, 0),
                    c1 = default,
                    c2 = default,
                    c3 = default
                }
            });

            //target = BuffHelper.NerestTarget(inData.allEntities, inData.cdfeLocalTransform,
            //     inData.cdfeChaStats, inData.cdfePlayerData, inData.cdfeEnemyData, inData.cdfeSpiritData,
            //     inData.entity, TargetAttackType.Enemy);

            // if (refData.storageInfoLookup.Exists(target))
            // {
            //     Debug.Log($"target");
            //
            //     var newdir =
            //         math.normalizesafe(inData.cdfeLocalTransform[target].Position -
            //                            inData.cdfeLocalTransform[inData.player].Position);
            //
            //
            //     var newpos = new LocalTransform
            //     {
            //         Position = inData.cdfeLocalTransform[inData.player].Position,
            //         Scale = inData.cdfeLocalTransform[skillBaseBallBat].Scale,
            //         Rotation = quaternion.LookRotation(new float3(newdir.x, newdir.y, 0),
            //             new float3(0, 0, 1))
            //     };
            //
            //     //Quaternion.LookRotation(Vector3.forward, directionToEnemy);
            //     //Quaternion.LookRotation(Vector3.forward, directionToEnemy);
            //     // var newrot =
            //     //     quaternion.LookRotation(new float3(newdir.x, newdir.y, 0),
            //     //         new float3(0, 1, 0));
            //
            //     refData.ecb.SetComponent(inData.sortkey, entity, newpos);
            // }
        }

        public void OnDestroy(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
        {
        }

        public void OnAwake(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
        {
        }

        public void OnChargeFinish(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
        {
        }
    }
}