//using cfg.blobstruct;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;

//namespace Main
//{
//    //生成陷阱
//    public partial struct SkillGenerateTrapAttack : ISkillAttack
//    {
//        /// <summary>
//        /// 0
//        /// </summary>
//        public int id;

//        /// <summary>
//        /// 1 一次性释放的技能实体 值=0
//        /// </summary>
//        public float duration;

//        /// <summary>
//        /// 2
//        /// </summary>
//        public int tick;

//        /// <summary>
//        /// 3 技能实体释放者
//        /// </summary>
//        public Entity caster;

//        /// <summary>
//        /// 4 是否是弹幕
//        /// </summary>
//        public bool isBullet;

//        /// <summary>
//        /// 5 弹幕hp
//        /// </summary>
//        public int hp;

//        /// <summary>
//        /// 6 是否是持续性攻击
//        /// </summary>
//        public bool stayAttack;

//        /// <summary>
//        /// 7 持续性攻击间隔
//        /// </summary>
//        public float stayAttackInterval;

//        /// <summary>
//        /// 8 当前持续性攻击间隔
//        /// </summary>
//        public float curStayAttackInterval;

//        /// <summary>
//        /// 技能实体执行延时 单位s  9
//        /// </summary>
//        public float skillDelay;

//        /// <summary>
//        /// 实体的击中目标 10
//        /// </summary>
//        public Entity target;

//        public LocalTransform localTrans;

//        public int judgeDis;

//        public int trapRadius;
//        public float trapTime;
//        public float breakSpeed;
//        public int pushForce;
//        public bool isBreak;

//        public int trapID;
//        public bool isActive;

//        //public int speed;
//        //public int skilleffectid;
//        //private int radius;
//        //private int pushRate;
//        //private int attackRate;
//        //private int attackAddRate;
//        //private int damageValue;

//        /// <summary>
//        /// 每帧做位置变动
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        /// <returns>变动后的LT</returns>
//        /// 
//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }
//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            return refData.cdfeLocalTransform[inData.entity];
//        }

//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            // throw new System.NotImplementedException();
//        }

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        private void HandleBuff(int currentBuffID, int buffCount, ref BlobArray<int3x4> buffArray,
//            in SkillAttackData_ReadOnly inData, int startIndex, int buffIndex)
//        {
//            var buffPar = GetBuffParmeters(ref buffArray, buffIndex, startIndex, buffCount);

//            if (currentBuffID == 40000023)
//            {
//                trapTime = buffPar[0].x / 1000f;
//                trapRadius = buffPar[1].x;
//                breakSpeed = buffPar[2].x;
//                pushForce = buffPar[3].x / 10000; //固定推力
//                isBreak = buffPar[4].x == 1 ? true : false;
//            }
//        }

//        private void GetLastParmeter(SkillAttackData_ReadOnly inData,
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
//                        //Debug.Log("没拿到数据");
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

//        private void UpdateParmeter(in SkillAttackData_ReadOnly inData, int updateID)
//        {
//            ref var skillEffectsArray = ref inData.config.value.Value.configTbskill_effects.configTbskill_effects;
//            GetLastParmeter(inData, ref skillEffectsArray, updateID);
//        }

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

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            if (!isActive)
//            {
//                ref var skilleffectConfig = ref inData.config.value.Value.configTbskill_effects.configTbskill_effects;
//                UpdateParmeter(inData, trapID);

//                for (int i = 0; i < inData.allEntities.Length; ++i)
//                {
//                    var currentEnemy = inData.allEntities[i];
//                    float dis = math.distance(localTrans.Position, refData.cdfeLocalTransform[currentEnemy].Position);

//                    if (math.abs(dis - judgeDis) < 0.1f)
//                    {
//                        isActive = true;
//                    }
//                }
//            }

//            if (isActive)
//            {
//                for (int i = 0; i < inData.allEntities.Length; ++i)
//                {
//                    //if (inData.cdfeTrapTag.HasComponent(inData.allEntities[i]))
//                    //{
//                    //    if (!isBreak && inData.cdfeChaStats[inData.allEntities[i]].chaResource.curMoveSpeed >= breakSpeed || (isBreak && curCooldown == 0))
//                    //    {
//                    //        for (int j = 0; j < refData.skillAtackDatas.Length; j++)
//                    //        {
//                    //            curCooldown = 0;
//                    //            if (refData.skillAtackDatas[j].data.CurrentTypeId == SkillAttack.TypeId.SkillTrapAttack)
//                    //            {
//                    //                //添加推力和伤害 销毁线和陷阱
//                    //                //Debug.Log($"isBreak");
//                    //                //refData.ecb.AppendToBuffer<SkillHitEffectBuffer>(inData.sortkey,,
//                    //                //    new SkillHitEffectBuffer
//                    //                //    {
//                    //                //        buffID = 201103,
//                    //                //        buffArgs = new float3x4
//                    //                //        {
//                    //                //            c0 = new float3(pushForce, 0, 0),
//                    //                //            c2 = new float3(2, 0, 0)
//                    //                //        }
//                    //                //    });
//                    //            }
//                    //        }
//                    //    }
//                    //}

//                    var currentEnemy = inData.allEntities[i];
//                    float dis = math.length(localTrans.Position - refData.cdfeLocalTransform[currentEnemy].Position);

//                    if (math.abs(dis - trapRadius) < 0.1f && !inData.cdfeTrapTag.HasComponent(currentEnemy) &&
//                        currentEnemy.Index != inData.player.Index)
//                    {
//                        refData.ecb.AddComponent(inData.sortkey, currentEnemy, new TrapTag { });
//                        //实例化Trap线
//                        var skillPrefab = inData.prefabMapData.prefabHashMap["SkillTrapLine"];
//                        var skillEntity = refData.ecb.Instantiate(inData.sortkey, skillPrefab);

//                        refData.ecb.AppendToBuffer(inData.sortkey, inData.entity,
//                            new LinkedEntityGroup { Value = skillEntity });


//                        refData.ecb.AddComponent(inData.sortkey, skillEntity, new SkillAttackData
//                        {
//                            data = new SkillTrapAttack
//                            {
//                                duration = trapTime,
//                                tick = 0,
//                                caster = inData.entity,
//                                isBullet = false,
//                                hp = 0,
//                                height =trapRadius ,
//                                width = 1,
//                                currentEnemy = currentEnemy,
//                                skillLoc = localTrans
//                            }.ToSkillAttack()
//                        });
//                    }
//                }
//            }


//            //refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//            //{
//            //    buffID = 40000010,
//            //    buffArgs = new float3x4
//            //    {
//            //        c0 = new float3(pushRate / 10000, 0, 0),
//            //        c1 = default,
//            //        c2 = default,
//            //        c3 = default
//            //    }
//            //});
//            //refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//            //{
//            //    buffID = 30000003,
//            //    buffArgs = new float3x4
//            //    {
//            //        c0 = new float3(attackRate, 301, attackAddRate),
//            //        c1 = default,
//            //        c2 = default,
//            //        c3 = default
//            //    }
//            //});
//            //refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//            //{
//            //    buffID = 30000004,
//            //    buffArgs = new float3x4
//            //    {
//            //        c0 = new float3(damageValue, 0, 0),
//            //        c1 = default,
//            //        c2 = default,
//            //        c3 = default
//            //    }
//            //});
//        }
//    }
//}

