using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Main
{
    /// <summary>
    /// 
    /// </summary>
    public partial struct Skill_9999103 : ISkillOld
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

        public int4x4 args;

        public int4 args1;


        public void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
        {
        }


        public void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
        {
            //Debug.LogError($"skill9999103");
            var radius = args1.x / 1000f;
            //var duration = args.y;


            //TODO：技能实体炸弹碰撞盒名字更改
            var prefab = inData.prefabMapData.prefabHashMap["CircleSkillAttack"];
            var ins = refData.ecb.Instantiate(inData.sortkey, prefab);

            var newpos = new LocalTransform
            {
                Position = pos,
                Scale = inData.cdfeLocalTransform[prefab].Scale * radius,
                Rotation = inData.cdfeLocalTransform[prefab].Rotation
            };


            refData.ecb.SetComponent(inData.sortkey, ins, newpos);

            refData.ecb.AddComponent(inData.sortkey, ins,
                new SkillAttackData
                {
                    data = new SkillAttack_0
                    {
                        id = id,
                        duration = 0,
                        tick = 0,
                        caster = inData.entity,
                        args = args1
                    }.ToSkillAttack()
                });
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