//using System.Diagnostics;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;
//using UnityEngine.UIElements;


//namespace Main
//{
//    /// <summary>
//    /// 生成诱饵
//    /// </summary>
//    public partial struct SkillAttack_406013 : ISkillAttack
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
//        /// 4 是否是弹幕 失效
//        /// </summary>
//        public bool isBullet;

//        /// <summary>
//        /// 5 弹幕hp  失效
//        /// </summary>
//        public int hp;

//        /// <summary>
//        /// 6 是否是持续性攻击  持续性攻击技能系统 失效
//        /// </summary>
//        public bool stayAttack;

//        /// <summary>
//        /// 7 持续性攻击间隔  持续性攻击技能系统
//        /// </summary>
//        public float stayAttackInterval;

//        /// <summary>
//        /// 8 当前持续性攻击间隔  持续性攻击技能系统
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

//        public Entity skillEntity;
//        public float width;
//        public float height;
//        public Entity currentEnemy;
//        public LocalTransform needLoc;
//        private int breakDuraion;
//        private bool isBoom;
//        private bool isActive;
//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            breakDuraion = 0;
//            isBoom = false;
//            if (BuffHelper.JudgeEquipExist(inData.entity,inData.bfePlayerEquipSkillBuffer, 406014, inData.config, out id))
//            {
//                breakDuraion = 1;
//            }
//            if (BuffHelper.JudgeEquipExist(inData.entity,inData.bfePlayerEquipSkillBuffer, 406016, inData.config, out id))
//            {
//                isBoom = true;
//                breakDuraion = 1;
//            }
//        }

//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//            return refData.cdfeLocalTransform[inData.entity];
//            //// UnityEngine.Debug.Log("dfsafsadfasdfsdfaaaaaaaaa");
//            //return newLoc;

//            // UnityEngine.Debug.Log($"postion:{newq.Position},scale:{newq.Scale},rotation:{newq.Rotation}");
//        }

//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            if (isBoom)
//            {
//                var prefab = inData.prefabMapData.prefabHashMap["CircleSkillAttack"];
//                var ins = refData.ecb.Instantiate(inData.sortkey, prefab);
//                var buff= BuffHelper.UpdateParmeter(inData.config, 4060161, 40000010);
//                var scale=buff[2].x;
//                var pushRate = buff[3].x;
//                var buff1 = BuffHelper.UpdateParmeter(inData.config, 4060161, 30000003);
//                var attackRate = buff1[1].x;


//                var newpos = new LocalTransform
//                {
//                    Position = refData.cdfeLocalTransform[inData.entity].Position,
//                    //TODO:大小*10
//                    Scale = scale * refData.cdfeLocalTransform[prefab].Scale,
//                    Rotation = refData.cdfeLocalTransform[inData.entity].Rotation,
//                };

//                refData.ecb.SetComponent(inData.sortkey, ins, newpos);
//                refData.ecb.AddComponent(inData.sortkey, ins,
//                    new SkillAttackData
//                    {
//                        data = new SkillAttack_0
//                        {
//                            id = id,
//                            duration = 0.02f,
//                            tick = 0,
//                            caster = inData.player,
//                            isBullet = false,
//                            hp = 0,
//                            stayAttack = false,
//                            stayAttackInterval = 0,
//                            curStayAttackInterval = 0,
//                            skillDelay = 0
//                        }.ToSkillAttack()
//                    });

//                refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//                {
//                    buffID = 40000003,
//                    buffArgs = new float3x4 { c0 = new float3(0, 0, 0) }
//                });
//                refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//                {
//                    buffID = 30000003,
//                    buffArgs = new float3x4 { c0 = new float3(attackRate, 0, 0) }
//                });

//            }
//        }

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            if (!isActive)
//            {
//                duration = breakDuraion;
//                isActive = true;
//            }
//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//           //for(int i = 0; i < inData.allEntities.Length; i++)
//           // {
//           //     var target = inData.allEntities[i];
//           //     if (target.Index == inData.player.Index) continue;
//           //     var dis = math.distance(refData.cdfeLocalTransform[inData.allEntities[i]].Position, refData.cdfeLocalTransform[inData.entity].Position);
//           //     if (dis < 10f)
//           //     {
//           //         UnityEngine.Debug.LogError("OnUpdate1111111111111111111111111");
//           //         refData.cdfeAgentBody[inData.allEntities[i]].SetDestination(refData.cdfeLocalTransform[inData.entity].Position);
//           //     }
//           // }
//        }
//    }


//}

