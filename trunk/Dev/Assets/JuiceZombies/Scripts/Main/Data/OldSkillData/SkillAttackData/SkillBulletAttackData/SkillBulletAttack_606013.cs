//using cfg.blobstruct;
//using System.Data;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;

//namespace Main
//{
//    //扔石灰的bullect
//    public partial struct SkillBulletAttack_606013 : ISkillAttack
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
//        /// 实体的击中目标 6
//        /// </summary>
//        public Entity target;

//        /// <summary>
//        /// 7
//        /// </summary>
//        public int4 args;

//        /// <summary>
//        /// 弹幕速度 8
//        /// </summary>
//        public float speed;

//        /// <summary>
//        /// 弹幕用 触发器id 9
//        /// </summary>
//        public int triggerID;


//        private int attackRate;


//        /// <summary>
//        /// 每帧做位置变动
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        /// <returns>变动后的LT</returns>
//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            //var dir = new float3(0, 1, 0);
//            var actionspeed = inData.cdfeChaStats[caster].chaResource.actionSpeed < math.EPSILON
//                ? 1
//                : inData.cdfeChaStats[caster].chaResource.actionSpeed;

//            //math.clamp(inData.cdfeChaStats[caster].chaResource.actionSpeed,1,)
//            var dir = math.mul(refData.cdfeLocalTransform[inData.entity].Rotation, new float3(0, 1, 0));
//            var temp = refData.cdfeLocalTransform[inData.entity];
//            temp.Position += dir * (speed / 100f) * inData.fDT * actionspeed;

//            return temp;
//        }


//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            // throw new System.NotImplementedException();
//        }


//        private void HandleBuff(int currentBuffID, int buffCount, ref BlobArray<int3x4> buffArray,
//            in SkillAttackData_ReadOnly inData, int startIndex, int buffIndex)
//        {
//            var buffPar = GetBuffParmeters(ref buffArray, buffIndex, startIndex, buffCount);

//            if (currentBuffID == 30000003)
//            {
//                attackRate = buffPar[1].x;
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
//            ref var bulletConfig = ref inData.config.value.Value.configTbbullets.configTbbullets;
//            ref var skillConfig = ref inData.config.value.Value.configTbskills.configTbskills;

//            ref var skilleffectConfig = ref inData.config.value.Value.configTbskill_effects.configTbskill_effects;
//            UpdateParmeter(inData, id);
//            // UnityEngine.Debug.Log($"skilleffectid:{skilleffectid}");
//            // NativeArray<int> tempArray = new NativeArray<int>(5, Allocator.Temp);
//            // for (int k = 0; k < 5; k++)
//            // {
//            //     tempArray[k] = 1;
//            // }
//            //
//            // var distincInt = tempArray.Distinct();
//            // int finallytarget = 0;
//            // foreach (var elem in distincInt)
//            // {
//            //     finallytarget += elem;
//            // }

//            var prefab = inData.prefabMapData.prefabHashMap["CircleSkillAttack"];
//            var ins = refData.ecb.Instantiate(inData.sortkey, prefab);

//            //Debug.LogError($"radius{radius}");
//            var newpos = new LocalTransform
//            {
//                Position = refData.cdfeLocalTransform[inData.entity].Position,
//                //TODO:大小*10
//                Scale = refData.cdfeLocalTransform[inData.entity].Scale*5f,
//                Rotation = refData.cdfeLocalTransform[prefab].Rotation,
//            };

//            refData.ecb.SetComponent(inData.sortkey, ins, newpos);

//            refData.ecb.AddComponent(inData.sortkey, ins,
//                new SkillAttackData
//                {
//                    data = new SkillAttack_0
//                    {
//                        id = 606013,
//                        duration = 0.5f,
//                        tick = 0,
//                        caster = caster,
//                        isBullet = false,
//                        hp = 0,
//                    }.ToSkillAttack()
//                });

//            // UnityEngine.Debug.Log($"pushRate:{pushRate}attackRate:{attackRate}");

//            refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//            {
//                buffID = 30000003,
//                buffArgs = new float3x4
//                {
//                    c0 = new float3(attackRate, 0, 0),
//                    c1 = default,
//                    c2 = default,
//                    c3 = default
//                }
//            });
//            var nextEffectID = 0;
//            for(int i=0;i< skillConfig.Length; i++)
//            {
//                if (skillConfig[i].id == id)
//                {
//                    nextEffectID = skillConfig[i].skillEffectArray[2];
//                    break;
//                }
//            }

//            BuffHelper.TryGetParmeter(inData.config,nextEffectID, out var parmeter, 20003018);
//            var limitViewDur = parmeter[0].x;
//            var limitViewDis = parmeter[1].x;
//            //TODO:添加视野限制预制体
//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }
//    }
//}

