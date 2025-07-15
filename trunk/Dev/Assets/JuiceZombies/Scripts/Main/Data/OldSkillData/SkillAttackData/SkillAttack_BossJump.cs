//using cfg.config;
//using Rewired;
//using System;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;

//namespace Main
//{
//    //boss跳跃
//    public partial struct SkillAttack_BossJump : ISkillAttack
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

//        /// <summary>
//        /// 技能跟随者
//        /// </summary>
//        public Entity flowed;

//        public float3 finalPoint;

//        public float maxDuration;

//        /// <summary>
//        /// 每帧做位置变动
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        /// <returns>变动后的LT</returns>
//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            if (refData.storageInfoFromEntity.Exists(flowed))
//            {
//                return refData.cdfeLocalTransform[flowed];
//            }

//            BossJump(in inData, ref refData);


//            return refData.cdfeLocalTransform[inData.entity];
//        }

//        private void BossJump(in SkillAttackData_ReadOnly inData, ref SkillAttackData_ReadWrite refData)
//        {
//            var startPoint = refData.cdfeLocalTransform[flowed].Position;
//            var offsetX = math.abs(startPoint.x - finalPoint.x);
//            var offsetY = math.abs(startPoint.y - finalPoint.y);
//            float3 point1 = new float3(startPoint.x + offsetX / 2, startPoint.y + offsetY, 0);
//            float3 point2 = new float3(startPoint.x + offsetX, startPoint.y, 0);
//            refData.ecb.SetComponent<LocalTransform>(inData.sortkey, flowed, new LocalTransform
//            {
//                Position = PhysicsHelper.CubicBezier(
//                    (maxDuration - duration) / maxDuration,
//                    startPoint, point1, point2, finalPoint),
//                Scale = refData.cdfeLocalTransform[flowed].Scale,
//                Rotation = refData.cdfeLocalTransform[flowed].Rotation
//            });
//        }

//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            ////矩形拳击
//            //if (!BuffHelper.TryGetParmeter(inData.config, id, 40000004)) return;
//            //var buff = BuffHelper.UpdateParmeter(inData.config, id, 40000004);
//            //int offset = buff[1].x;
//            //float jumpDuration = buff[0].x / 1000f;

//            var dir = math.normalizesafe(finalPoint - refData.cdfeLocalTransform[flowed].Position);

//            if (!BuffHelper.TryGetParmeter(inData.config, id, 40000004)) return;

//            var buff = BuffHelper.UpdateParmeter(inData.config, id, 40000004);
//            var width = buff[1].x;
//            var height = buff[2].x;
//            //TODO:
//            //BuffHelper.GenerateWarningWithNotMove(ref refData.ecb, inData.config, inData.sortkey, inData.prefabMapData,
//            //    refData.cdfeLocalTransform, dir, 2, width, height, 1, id, 0.6f, flowed);
//        }

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            if (id == 1003063)
//            {
//                var buff1 = BuffHelper.UpdateParmeter(inData.config, id, 20003001);
//                var rate = buff1[1].x;
//                var duration = buff1[0].x / 1000f;
//                var probability = buff1[2].x;
//                for (int i = 0; i < inData.otherEntities.Length; i++)
//                {
//                    var rand = inData.rand.NextInt(probability, 10000);
//                    if (rand <= probability)
//                    {
//                        refData.ecb.AppendToBuffer(inData.sortkey, inData.otherEntities[i],
//                            new Buff_20003001
//                            {
//                                id = 20003001,
//                                priority = 0,
//                                maxStack = 0,
//                                tags = 0,
//                                tickTime = 0,
//                                timeElapsed = 0,
//                                ticked = 0,
//                                duration = duration,
//                                permanent = false,
//                                caster = default,
//                                buffArgs = new BuffArgs
//                                {
//                                    args0 = rate
//                                },
//                                carrier = inData.otherEntities[i],
//                                canBeStacked = false,
//                                buffStack = default,
//                            }.ToBuffOld());
//                    }

//                    var buff2 = BuffHelper.UpdateParmeterNew(inData.config, id, 20003009);
//                    rate = buff2[0].x;
//                    duration = buff2[1].x / 1000f;
//                    refData.ecb.AppendToBuffer(inData.sortkey, inData.otherEntities[i],
//                        new Buff_20003009
//                        {
//                            id = 20003009,
//                            priority = 0,
//                            maxStack = 0,
//                            tags = 0,
//                            tickTime = 1f,
//                            timeElapsed = 0,
//                            ticked = 0,
//                            duration = duration,
//                            permanent = false,
//                            caster = default,
//                            buffArgs = new BuffArgs
//                            {
//                                args0 = rate
//                            },
//                            carrier = inData.otherEntities[i],
//                            canBeStacked = false,
//                            buffStack = default,
//                        }.ToBuffOld());
//                }
//            }
//            else if (id == 1003073)
//            {
//                for (int i = 0; i < inData.otherEntities.Length; i++)
//                {
//                    refData.ecb.AppendToBuffer(inData.sortkey, inData.otherEntities[i],
//                        new Buff_20003008
//                        {
//                            id = 20003008,
//                            priority = 0,
//                            maxStack = 0,
//                            tags = 0,
//                            tickTime = 1f,
//                            timeElapsed = 0,
//                            ticked = 0,
//                            duration = duration,
//                            permanent = false,
//                            caster = default,
//                            buffArgs = new BuffArgs
//                            {
//                                args0 = id
//                            },
//                            carrier = inData.otherEntities[i],
//                            canBeStacked = false,
//                            buffStack = default,
//                        }.ToBuffOld());
//                }
//            }
//        }

//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }
//    }
//}

