//using Unity.Collections;
//using Unity.Entities;

//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;
//using UnityEngine;


//namespace Main
//{
//    /// <summary>
//    ///场景技能-生成火焰
//    /// </summary>
//    public partial struct Skill_800001 : ISkillOld
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


//        public void OnAwake(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }

//        public void OnChargeFinish(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }

//        public void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            isOneShotSkill = true;
//            var currentSkillEffectID = 0;
//            ref var skillArray = ref inData.globalConfig.value.Value.configTbskills.configTbskills;

//            //拿到当前技能效果id
//            for (int i = 0; i < skillArray.Length; ++i)
//            {
//                if (skillArray[i].id == id)
//                {
//                    currentSkillEffectID = skillArray[i].skillEffectArray[0];
//                }
//            }


//            if (!BuffHelper.TryGetParmeter(inData.globalConfig, currentSkillEffectID, 30000014)) return;
//            if (!BuffHelper.TryGetParmeter(inData.globalConfig, currentSkillEffectID, 50000013)) return;

//            var buff = BuffHelper.UpdateParmeter(inData.globalConfig, currentSkillEffectID, 50000013);
//            var num = buff[1].x;
//            var tempPosList = new NativeList<Rect>(num, Allocator.Temp);
//            //ref var mapBossArray = ref inData.globalConfig.value.Value.configTbmap_template_bosss.configTbmap_template_bosss;
//            //boss战地图固定生成位置在9999，9999 战斗区域宽高240，240
//            for (int i = 0; i < num; ++i)
//            {
//                Debug.LogError($"time{inData.eT}");
//                GenerateFireArea(ref refData, inData, currentSkillEffectID, tempPosList, (int)inData.eT+i);
//            }


//            //refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//            //{
//            //    buffID = 40000003,
//            //    buffArgs = new float3x4 { c0 = new float3(push, 0, 0) }
//            //});
//            //refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//            //{
//            //    buffID = 30000003,
//            //    buffArgs = new float3x4 { c0 = new float3(attack, 0, 0) }
//            //});
//        }


//        public void OnDestroy(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            //throw new NotImplementedException();
//        }

//        public void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }


//        private float3 GetRandOffsetPos(int offset, NativeList<Rect> positions, int seed)
//        {

//            var targetPos = new float3(0, 0, 0);

//            var random =  new Unity.Mathematics.Random((uint)seed);
//            int mapWidth = 240;
//            int mapHeight = 240;
//            Rect tempRect;
//            do
//            {

//                float randomX = random.NextFloat(targetPos.x - mapWidth / 2f, targetPos.x + mapWidth / 2f);

//                float randomY = random.NextFloat(targetPos.y - mapHeight / 2f, targetPos.y + mapHeight / 2f);
//                tempRect = new Rect(randomX, randomY, offset, offset);
//            } while (IsCanNotSelect(tempRect, positions));
//            positions.Add(tempRect);


//            return new float3(tempRect.x,tempRect.y,0);
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


//        private void GenerateFireArea(ref TimeLineData_ReadWrite refData, TimeLineData_ReadOnly inData, int currentSkillEffectID, NativeList<Rect> tempPosList, int seed)
//        {

//            var buff = BuffHelper.UpdateParmeter(inData.globalConfig, currentSkillEffectID, 30000014);
//            var delay = buff[4].x / 1000f;
//            //生成预警圈
//            var prefab = inData.prefabMapData.prefabHashMap["CircleATKRangePrefab"];
//            var ins = refData.ecb.Instantiate(inData.sortkey, prefab);


//            buff = BuffHelper.UpdateParmeter(inData.globalConfig, currentSkillEffectID, 50000013);
//            var gapDis = buff[2].x;
//            var targetPos = GetRandOffsetPos(gapDis, tempPosList, seed+1);
//            //Debug.Log($"GenerateFireArea:第{seed}次,targetPos:{targetPos}");
//            var newpos = new LocalTransform
//            {
//                Position = targetPos,
//                //TODO:大小*10
//                Scale = inData.cdfeLocalTransform[prefab].Scale*10f,
//                Rotation = inData.cdfeLocalTransform[prefab].Rotation,
//            };

//            // Debug.Log($"position{inData.cdfeLocalTransform[inData.entity].Position}");
//            refData.ecb.SetComponent(inData.sortkey, ins, newpos);


//            refData.ecb.AddComponent(inData.sortkey, ins,
//                new SkillAttackData
//                {
//                    data = new SkillAttack_800001
//                    {
//                        id = currentSkillEffectID,
//                        duration = delay*10f,
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

