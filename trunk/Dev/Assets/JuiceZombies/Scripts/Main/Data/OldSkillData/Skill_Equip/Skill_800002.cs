//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;
//using UnityEngine;


//namespace Main
//{
//    /// <summary>
//    ///场景技能-朝玩家发射火焰
//    /// </summary>
//    public partial struct Skill_800002 : ISkillOld
//    {
//        ///<summary>0
//        ///技能的id
//        ///</summary>
//        public int id;

//        ///<summary>1
//        ///技能等级
//        ///</summary>
//        public int level;

//        ///<summary>2
//        ///冷却时间，单位秒。尽管游戏设计里面是没有冷却时间的，但是我们依然需要这个数据
//        ///因为作为一个ARPG子分类，和ARPG游戏有一样的问题：一次按键（时间够久）会发生连续多次使用技能，所以得有一个GCD来避免问题
//        ///当然和wow的gcd不同，这个“GCD”就只会让当前使用的技能进入0.1秒的冷却
//        ///</summary>
//        public float cooldown;


//        ///<summary>3
//        ///持续时间，单位：秒
//        ///</summary>
//        public float duration;


//        ///<summary>4
//        ///倍速，1=100%，0.1=10%是最小值
//        ///</summary>
//        public float timeScale;

//        ///<summary>5
//        ///Timeline的焦点对象也就是创建timeline的负责人，比如技能产生的timeline，就是技能的施法者
//        ///</summary>
//        public Entity caster;

//        ///<summary>6
//        ///技能已经运行了多少帧了 无需赋值
//        ///</summary>
//        public int tick;

//        /// <summary>7
//        /// 技能当前冷却时间 无需赋值
//        /// </summary>
//        public float curCooldown;

//        ///<summary>8
//        ///剩余时间，单位：秒
//        ///</summary>
//        public float curDuration;

//        ///<summary>9
//        ///距离这个技能最近的目标
//        ///</summary>
//        public Entity target;

//        /// <summary>
//        /// 是否是充能技能 10
//        /// </summary>


//        /// <summary>
//        /// 充能时间 毫秒 11 （充能次数充能也转换成时间进行取模）
//        /// </summary>


//        /// <summary>
//        /// 技能实体的执行时间 用于充能技能 12
//        /// </summary>


//        ///<summary>13
//        ///技能从创建到现在总的tick
//        ///</summary>
//        public int totalTick;

//        ///<summary>14
//        ///是否时一次性释放技能
//        ///</summary>
//        public bool isOneShotSkill;

//        ///<summary>15
//        ///是否是确定位置坐标
//        ///</summary>
//        public bool isUseCertainPos;

//        ///<summary>16
//        ///一次性释放技能的坐标
//        ///</summary>
//        public float3 pos;

//        private int currentSkillEffectID;

//        private float gap;

//        private int times;

//        private bool isReady;


//        public void OnAwake(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }

//        public void OnChargeFinish(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }

//        public void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            cooldown = 20f;
//            Debug.LogError($"OnCast");
//            isOneShotSkill = true;

//            ref var skillArray = ref inData.globalConfig.value.Value.configTbskills.configTbskills;

//            //拿到当前技能效果id
//            for (int i = 0; i < skillArray.Length; ++i)
//            {
//                if (skillArray[i].id == id)
//                {
//                    currentSkillEffectID = skillArray[i].skillEffectArray[0];
//                    break;
//                }
//            }


//            if (!BuffHelper.TryGetParmeter(inData.globalConfig, currentSkillEffectID, 50000014)) return;

//            var buff = BuffHelper.UpdateParmeter(inData.globalConfig, currentSkillEffectID, 50000014);
//            times = buff[3].x;
//            gap = buff[4].x / 1000f;
//            isReady = false;
//        }


//        public void OnDestroy(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            //throw new NotImplementedException();
//        }

//        public void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            if (!BuffHelper.TryGetParmeter(inData.globalConfig, currentSkillEffectID, 30000014)) return;
//            if (!BuffHelper.TryGetParmeter(inData.globalConfig, currentSkillEffectID, 50000014)) return;

//            var buff = BuffHelper.UpdateParmeter(inData.globalConfig, currentSkillEffectID, 50000014);
//            var num = buff[1].x;

//            //Debug.LogError($"OnUpdate  tick:{tick},{gap/inData.fdT},times{times},");
//            int condition = (int)(gap / inData.fdT);
//            if (tick % condition == 0 && times > 0)
//            {
//                Debug.LogError($"OnUpdate:{times}");
//                times--;
//                var tempPosList = new NativeList<Rect>(num, Allocator.Temp);

//                for (int i = 0; i < num; ++i)
//                {
//                    GenerateFireArea(ref refData, inData, currentSkillEffectID, ref tempPosList,
//                        ((int)inData.eT + tick + num + i));
//                }
//            }
//        }


//        private float3 GetRandOffsetPos(int offset, LocalTransform targetLoc, ref NativeList<Rect> positions, int seed)
//        {
//            var targetPos = targetLoc.Position;

//            var random = new Unity.Mathematics.Random((uint)seed);
//            int mapWidth = 25;
//            int mapHeight = 25;
//            Rect tempRect;
//            do
//            {
//                float randomX = random.NextFloat(targetPos.x - mapWidth, targetPos.x + mapWidth);

//                float randomY = random.NextFloat(targetPos.y - mapHeight, targetPos.y + mapHeight);
//                tempRect = new Rect(randomX, randomY, offset + 20, offset + 20);
//            } while (IsCanNotSelect(tempRect, positions));

//            positions.Add(tempRect);


//            Debug.LogError($"GenerateFireArea:{tempRect.x}:{tempRect.y},{positions.Length}");

//            return new float3(tempRect.x, tempRect.y, 0);
//        }

//        private bool IsCanNotSelect(Rect tempRect, NativeList<Rect> positions)
//        {
//            foreach (var pos in positions)
//            {
//                if (tempRect.Overlaps(pos))
//                {
//                    return true;
//                }
//            }

//            return false;
//        }


//        private void GenerateFireArea(ref TimeLineData_ReadWrite refData, TimeLineData_ReadOnly inData,
//            int currentSkillEffectID, ref NativeList<Rect> tempPosList, int seed)
//        {
//            var buff = BuffHelper.UpdateParmeter(inData.globalConfig, currentSkillEffectID, 30000014);
//            var delay = buff[4].x / 1000f;
//            buff = BuffHelper.UpdateParmeter(inData.globalConfig, currentSkillEffectID, 50000014);
//            var gapDis = buff[2].x;
//            var targetPos = GetRandOffsetPos(gapDis, inData.cdfeLocalTransform[inData.player], ref tempPosList,
//                seed + 1);

//            //生成预警圈
//            GenerateWarning(refData, inData, currentSkillEffectID, delay, targetPos);
//        }

//        private void GenerateWarning(TimeLineData_ReadWrite refData, TimeLineData_ReadOnly inData,
//            int currentSkillEffectID, float delay, float3 targetPos)
//        {
//            var ins = BuffHelper.GenerateWarning(ref refData.ecb, inData.globalConfig, inData.sortkey,
//                inData.prefabMapData, inData.cdfeLocalTransform, targetPos, 1, 1, 1, 20, inData.entity);


//            refData.ecb.AddComponent(inData.sortkey, ins,
//                new SkillAttackData
//                {
//                    data = new SkillAttack_800001
//                    {
//                        id = currentSkillEffectID,
//                        duration = delay * 10f,
//                        tick = 0,
//                        caster = inData.entity,
//                        isBullet = false,
//                        hp = 0,
//                        stayAttack = false,
//                        stayAttackInterval = 0,
//                        curStayAttackInterval = 0,
//                        skillDelay = 0
//                    }.ToSkillAttack()
//                });
//        }
//    }
//}

