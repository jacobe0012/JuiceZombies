//using cfg.blobstruct;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;
//using UnityEngine;


//namespace Main
//{
//    /// <summary>
//    /// 攻击后 附带一次幻影攻击
//    /// </summary>
//    public partial struct Skill_320001 : ISkillOld
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
//                ///<summary>14
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
//        public LocalTransform curLoc;

//        public int limitCullTime;
//        public int currentSkillEffectID;

//        private int angle;
//        private int scale;
//        private int pushRate;
//        private int diffuseSpeed;
//        private bool isActive;

//        public void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            isActive = true;

//            ref var skillArray = ref inData.globalConfig.value.Value.configTbskills.configTbskills;
//            //拿到当前技能效果id
//            currentSkillEffectID = 0;
//            for (int i = 0; i < skillArray.Length; ++i)
//            {
//                if (skillArray[i].id == id)
//                {
//                    // cooldown = skillArray[i].cd / 1000f;
//                    currentSkillEffectID = skillArray[i].skillEffectArray[0];
//                }
//            }

//            ref var skillEffectsArray = ref inData.globalConfig.value.Value.configTbskill_effects.configTbskill_effects;
//            GetLastParmeter(inData, ref skillEffectsArray, currentSkillEffectID);
//        }

//        private void GetLastParmeter(TimeLineData_ReadOnly inData,
//            ref BlobArray<ConfigTbskill_effect> skillEffectsArray, int currentSkillEffectID)
//        {
//            BlobArray<int3x4> buffDefault = new BlobArray<int3x4>();
//            ref BlobArray<int3x4> buffArray = ref buffDefault;


//            for (int i = 0; i < skillEffectsArray.Length; ++i)
//            {
//                if (skillEffectsArray[i].id == currentSkillEffectID)
//                {
//                    //拿到buff组

//                    buffArray = ref skillEffectsArray[i].skillEffectBuffNew;
//                    if (buffArray.Length <= 0)
//                    {
//                        Debug.Log("没拿到数据");
//                        return;
//                    }


//                    //处理buff
//                    for (int j = 1; j <= 8; j++)

//                    {
//                        int currentBuffID = 0;
//                        int currentBuffCount = 0;
//                        int startIndex = 0;
//                        switch (j)
//                        {
//                            case 1:
//                                currentBuffID = buffArray[0].c0.x;
//                                currentBuffCount = buffArray[0].c0.y;
//                                break;

//                            case 2:
//                                currentBuffID = buffArray[0].c1.x;
//                                currentBuffCount = buffArray[0].c1.y;
//                                break;

//                            case 3:
//                                currentBuffID = buffArray[0].c2.x;
//                                currentBuffCount = buffArray[0].c2.y;
//                                break;

//                            case 4:
//                                currentBuffID = buffArray[0].c3.x;
//                                currentBuffCount = buffArray[0].c3.y;
//                                break;
//                            case 5:
//                                currentBuffID = buffArray[1].c0.x;
//                                currentBuffCount = buffArray[1].c0.y;
//                                startIndex = 1;
//                                break;

//                            case 6:
//                                currentBuffID = buffArray[1].c1.x;
//                                currentBuffCount = buffArray[1].c1.y;
//                                startIndex = 1;
//                                break;
//                            case 7:
//                                currentBuffID = buffArray[1].c2.x;
//                                currentBuffCount = buffArray[1].c2.y;
//                                startIndex = 1;
//                                break;
//                            case 8:
//                                currentBuffID = buffArray[1].c3.x;
//                                currentBuffCount = buffArray[1].c3.y;
//                                startIndex = 1;
//                                break;
//                        }

//                        HandleBuff(currentBuffID, currentBuffCount, ref buffArray, in inData, startIndex, j);
//                    }

//                    break;
//                }
//            }
//        }

//        /// <summary>
//        /// 根据buffID和buffCount拿到buff的参数
//        /// </summary>
//        /// <param name="currentBuffID">当前的buffID</param>
//        /// <param name="buffCount">当前buffID对应的buff参数</param>
//        /// <param name="buffArray">buff表</param>
//        /// <param name="inData"></param>
//        /// <param name="startIndex">1-4的buff的startIndex为0 5-8的buff的startIndex为1</param>
//        /// <param name="buffIndex">该buff的第i个参数</param>
//        private void HandleBuff(int currentBuffID, int buffCount, ref BlobArray<int3x4> buffArray,
//            in TimeLineData_ReadOnly inData, int startIndex, int buffIndex)
//        {
//            var buffPar = GetBuffParmeters(ref buffArray, buffIndex, startIndex, buffCount);
//            if (currentBuffID == 40000003)
//            {
//                angle = buffPar[0].x;
//                scale = buffPar[1].x + buffPar[1].x *
//                    (int)(inData.cdfeChaStats[inData.player].chaProperty.skillRangeRatios * buffPar[1].z / 10000f);
//                pushRate = buffPar[2].x;
//                diffuseSpeed = 120;
//                timeSkillAttack = angle / 120;
//            }
//        }


//        /// <summary>
//        /// 根据数据取得一个buff参数的int3结构 x=参数 y=对该参数的加成属性 z=对该参数的加成多少(大多为万分比)
//        /// </summary>
//        /// <param name="buffArray"></param>
//        /// <param name="buffIndex">第几个buff的索引</param>
//        /// <param name="startIndex">1-4的buff的startIndex为0 5-8的buff的startIndex为1</param>
//        /// <param name="buffParameterCount">对应的buffid的参数的数量</param>
//        /// <returns></returns>
//        private NativeList<int3> GetBuffParmeters(ref BlobArray<int3x4> buffArray, int buffIndex, int startIndex,
//            int buffParameterCount)
//        {
//            NativeList<int3> temp = new NativeList<int3>(buffParameterCount, Allocator.Temp);
//            for (int j = 1; j <= buffParameterCount; ++j)
//            {
//                int3 tempInt3 = new int3(0, 0, 0);

//                switch (buffIndex)
//                {
//                    case 1:
//                    case 5:
//                        int currentParameterRate = buffArray[2 * j + startIndex].c0.x;

//                        tempInt3.x = currentParameterRate;
//                        int currentParameterInfectID = buffArray[2 * j + startIndex].c0.y;
//                        if (currentParameterInfectID != 0)
//                        {
//                            tempInt3.y = currentParameterInfectID;
//                            int currentParameterInfectRate = buffArray[2 * j + startIndex].c0.z;
//                            tempInt3.z = currentParameterInfectRate;
//                        }
//                        else
//                        {
//                            tempInt3.y = 0;
//                            tempInt3.z = 0;
//                        }

//                        break;
//                    case 2:
//                    case 6:
//                        currentParameterRate = buffArray[2 * j + startIndex].c1.x;

//                        tempInt3.x = currentParameterRate;
//                        currentParameterInfectID = buffArray[2 * j + startIndex].c1.y;
//                        if (currentParameterInfectID != 0)
//                        {
//                            tempInt3.y = currentParameterInfectID;
//                            int currentParameterInfectRate = buffArray[2 * j + startIndex].c1.z;
//                            tempInt3.z = currentParameterInfectRate;
//                        }
//                        else
//                        {
//                            tempInt3.y = 0;
//                            tempInt3.z = 0;
//                        }

//                        break;

//                    case 3:
//                    case 7:
//                        currentParameterRate = buffArray[2 * j].c2.x;

//                        tempInt3.x = currentParameterRate;
//                        currentParameterInfectID = buffArray[2 * j].c2.y;
//                        if (currentParameterInfectID != 0)
//                        {
//                            tempInt3.y = currentParameterInfectID;
//                            int currentParameterInfectRate = buffArray[2 * j].c2.z;
//                            tempInt3.z = currentParameterInfectRate;
//                        }
//                        else
//                        {
//                            tempInt3.y = 0;
//                            tempInt3.z = 0;
//                        }

//                        break;
//                    case 4:
//                    case 8:
//                        currentParameterRate = buffArray[2 * j].c3.x;

//                        tempInt3.x = currentParameterRate;
//                        currentParameterInfectID = buffArray[2 * j].c3.y;
//                        if (currentParameterInfectID != 0)
//                        {
//                            tempInt3.y = currentParameterInfectID;
//                            int currentParameterInfectRate = buffArray[2 * j].c3.z;
//                            tempInt3.z = currentParameterInfectRate;
//                        }
//                        else
//                        {
//                            tempInt3.y = 0;
//                            tempInt3.z = 0;
//                        }

//                        break;
//                }

//                temp.Add(tempInt3);
//            }

//            return temp;
//        }

//        private LocalTransform SetTrans(TimeLineData_ReadOnly inData, Entity skillBaseBallBat, int limitDistance)
//        {
//            float distance = 9999;
//            target = BuffHelper.NerestTarget(inData.allEntities, inData.cdfeLocalTransform,
//                inData.cdfeChaStats, inData.cdfePlayerData, inData.cdfeEnemyData, inData.cdfeSpiritData,
//                inData.cdfeObstacleTag,
//                inData.entity, TargetAttackType.Enemy, ref distance);
//            if (distance > limitDistance)
//            {
//            }

//            //拿到对敌角度
//            if (IsAttackToEnemy(limitDistance, inData))
//            {
//                //TODO:方向
//                float3 dir = inData.cdfeLocalTransform[target].Position -
//                             inData.cdfeLocalTransform[inData.player].Position;


//                float needAngel = MathHelper.SignedAngle(math.normalizesafe(dir),
//                    new Vector3(0, 1, 0));

//                var qua = quaternion.AxisAngle(new float3(0, 0, -1), math.radians(needAngel));

//                var tran = new LocalTransform
//                {
//                    Position = inData.cdfeLocalTransform[inData.entity].Position,
//                    Scale = inData.cdfeLocalTransform[inData.entity].Scale,
//                    Rotation = qua
//                };


//                return inData.cdfeLocalTransform[inData.entity].RotateZ(math.radians(needAngel));
//                //return tran;

//                //return inData.cdfeLocalTransform[inData.entity];
//            }

//            return inData.cdfeLocalTransform[inData.player];
//        }

//        public bool IsAttackToEnemy(int limitDistance, TimeLineData_ReadOnly inData)
//        {
//            //判断是否对怪
//            float distance = 999f;
//            target = BuffHelper.NerestTarget(inData.allEntities, inData.cdfeLocalTransform,
//                inData.cdfeChaStats, inData.cdfePlayerData, inData.cdfeEnemyData, inData.cdfeSpiritData,
//                inData.cdfeObstacleTag,
//                inData.entity, TargetAttackType.Enemy, ref distance);
//            if (distance > limitDistance)
//            {
//                target = default;
//            }

//            return target != default ? true : false;
//        }

//        public void OnDestroy(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            //throw new NotImplementedException();
//        }


//        public void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            //    Debug.Log($"continuousCollCount{inData.cdfeChaStats[inData.player].chaResource.continuousCollCount}");
//            if (inData.cdfeChaStats[inData.player].chaResource.continuousCollCount >= limitCullTime && isActive)
//            {
//                isActive = false;
//                var skillBaseBallBat = inData.prefabMapData.prefabHashMap["SkillBaseBallBat"];
//                var skillEntity = refData.ecb.Instantiate(inData.sortkey, skillBaseBallBat);
//                //Debug.Log($"技能id：{currentEffectID}角度：{angle},半径：{scale},旋转角速度:{diffuseSpeed},技能时间:{timeSkillAttack},scale:{scale}");
//                //设置对敌角度
//                var enemyLoc = SetTrans(inData, skillBaseBallBat, scale);
//                refData.ecb.SetComponent(inData.sortkey, skillEntity, enemyLoc);
//                refData.ecb.AddComponent(inData.sortkey, skillEntity, new SkillAttackData
//                {
//                    data = new SkillBaseBallBatAttack
//                    {
//                        id = currentSkillEffectID,
//                        angle = angle,
//                        diffuseSpeed = diffuseSpeed,
//                        duration = timeSkillAttack,
//                        scale = scale,
//                        tick = 0,
//                        caster = inData.player,
//                        hp = 0,
//                        isBullet = false,
//                        skillDelay = 0,
//                        stayAttack = false,
//                        stayAttackInterval = 0,
//                        curStayAttackInterval = 0,
//                    }.ToSkillAttack()
//                });
//            }
//        }

//        public void OnAwake(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }

//        public void OnChargeFinish(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }
//    }
//}

