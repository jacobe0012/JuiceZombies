using System;
using System.Collections;
using System.Collections.Generic;
using cfg.config;
using Main;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AnimationMono : MonoBehaviour
{
    [Header("参数个数:类型1:6个;类型2:8个;类型3:8个")] [Header("Boss释放技能武器动画类型:4,参数个数:8")] [Header("玩家死亡武器动画类型:5,参数个数:7")]
    public List<int> args;

    //[Header("Args")]
    private int CycleTimes = 1;
    private int CycleInterval = 0;
    [Header("复原时间")] public float RecovTime = 500;

    [Header("动画类型")] public int animType = 1;

    [Header("播放")] public bool play;

    private GameObject _rotateOffset;

    //private Transform _transform;
    private float3 rotateOffset;

    //public bool save;
    private float attackTime;

    private float curAttackTime;

    private LocalTransform ogiTran;

    private int repeatTimes;

    private float interval;

    private float currecovTime;


    void Start()
    {
        //save = false;

        rotateOffset = transform.GetChild(0).position;
        // if (_rotateOffset != null)
        // {
        //     rotateOffset = _rotateOffset.transform.position;
        // }

        //this.transform .RotateAround(rotateOffset, new float3(0, 0, 1), 90);
        //tran = RotateAround(tran, tran.Position + rotateOffset, radians);
        ogiTran.Position = transform.position;
        ogiTran.Scale = transform.localScale.x;
        ogiTran.Rotation = transform.rotation;

        // repeatTimes = args[5] == 0 ? 1 : args[5];
        // interval = args[6] / 1000f;

        repeatTimes = CycleTimes == 0 ? 1 : CycleTimes;
        CycleInterval = Mathf.Max(CycleInterval, 1);
        //CycleInterval= Mathf.Max(CycleInterval, 1);
        interval = CycleInterval / 1000f;
        currecovTime = Mathf.Max(RecovTime / 1000f, 0.01f);

        switch (animType)
        {
            case 1:
                attackTime = (args[0] + args[1] + args[5] + RecovTime) / 1000f;

                break;
            case 2:
                attackTime =
                    (args[0] + args[1] + args[2] + args[7] + RecovTime) / 1000f;

                break;
            case 3:
                repeatTimes = args[6] == 0 ? 1 : args[6];
                CycleInterval = Mathf.Max(args[7], 1);
                //CycleInterval= Mathf.Max(CycleInterval, 1);
                interval = CycleInterval / 1000f;

                attackTime =
                    (args[0] + args[1] + args[2] + args[5] + RecovTime) / 1000f;

                break;
            case 4:
                attackTime =
                    (args[4] + args[6] + args[7] + RecovTime) / 1000f;

                break;

            case 5:
                attackTime =
                    (args[1] + args[3] + args[5]) / 1000f;

                break;
        }

        curAttackTime = attackTime;
        // if (args.Count != 7)
        // {
        //     Debug.LogError($"参数个数不等于8个");
        // }
    }


    private void FixedUpdate()
    {
        if (play)
        {
            switch (animType)
            {
                case 1:
                    if (args.Count != 6)
                    {
                        Debug.LogError($"类型{animType} 需要6个Args长度！");
                    }

                    break;
                case 2:
                    if (args.Count != 8)
                    {
                        Debug.LogError($"类型{animType} 需要8个Args长度！");
                    }

                    break;
                case 3:
                    if (args.Count != 8)
                    {
                        Debug.LogError($"类型{animType} 需要8个Args长度！");
                    }

                    break;
            }

            if (repeatTimes > 0)
            {
                if (curAttackTime > 0)
                {
                    var tran = new LocalTransform();

                    switch (animType)
                    {
                        case 1:
                            tran = WeaponAnimTran1(args, curAttackTime, attackTime, ogiTran);
                            break;

                        case 2:
                            tran = WeaponAnimTran2(args, curAttackTime, attackTime, ogiTran);
                            //WeaponAnimTran2(args, curAttackTime, attackTime, transform);
                            break;
                        case 3:
                            tran = WeaponAnimTran3(args, curAttackTime, attackTime, ogiTran);
                            break;
                        case 4:
                            tran = WeaponAnimTran4(args, curAttackTime, attackTime, ogiTran);
                            break;
                        case 5:
                            tran = WeaponAnimTran5(args, curAttackTime, attackTime, ogiTran);
                            break;
                    }

                    transform.position = tran.Position;
                    transform.localScale = new Vector3(tran.Scale, tran.Scale, tran.Scale);
                    transform.rotation = tran.Rotation;

                    curAttackTime -= Time.fixedDeltaTime;
                }
                else
                {
                    if (animType != 5)
                    {
                        transform.position = ogiTran.Position;
                        transform.localScale = new Vector3(ogiTran.Scale, ogiTran.Scale, ogiTran.Scale);
                        transform.rotation = ogiTran.Rotation;
                    }

                    if (interval > 0)
                    {
                        interval -= Time.fixedDeltaTime;
                    }
                    else
                    {
                        repeatTimes--;
                        currecovTime = Mathf.Max(RecovTime / 1000f, 0.01f);
                        //interval = args[6] / 1000f;
                        interval = Mathf.Max(CycleInterval / 1000f, 0.01f);
                        switch (animType)
                        {
                            case 1:
                                attackTime = (args[0] + args[1] + args[5] + RecovTime) / 1000f;

                                break;

                            case 2:
                                attackTime =
                                    (args[0] + args[1] + args[2] + args[7] + RecovTime) / 1000f;

                                break;
                            case 3:
                                // repeatTimes = args[6] == 0 ? 1 : args[6];
                                // CycleInterval = Mathf.Max(args[7], 1);
                                // //CycleInterval= Mathf.Max(CycleInterval, 1);
                                // interval = CycleInterval / 1000f;

                                attackTime =
                                    (args[0] + args[1] + args[2] + args[5] + RecovTime) / 1000f;

                                break;
                            case 4:
                                attackTime =
                                    (args[4] + args[6] + args[7] + RecovTime) / 1000f;

                                break;
                            case 5:
                                attackTime =
                                    (args[1] + args[3] + args[5]) / 1000f;

                                break;
                        }

                        curAttackTime = attackTime;
                    }
                    // if (currecovTime > 0)
                    // {
                    //     var t = CycleInterval / 1000f - currecovTime;
                    //
                    //     var clamp1 = math.clamp(t, 0, CycleInterval / 1000f);
                    //
                    //     float t1 = clamp1 / (CycleInterval / 1000f);
                    //
                    //     //float t1 = math.clamp(0, p1 / 1000f, p1 / 1000f - weaponData.curAttackTime);
                    //     //float degreePerFrame = (p3 / (p1 / 1000f)) / 50;
                    //     //var degree = math.lerp(0, p3, t1);
                    //     var pos = math.lerp(transform.position, ogiTran.Position, t1);
                    //
                    //     var scale = math.lerp(transform.localScale, ogiTran.Scale, t1);
                    //     var quaternion = Quaternion.Lerp(transform.rotation, ogiTran.Rotation, t1);
                    //     // var newTran = new LocalTransform();
                    //     // newTran.Position.x = posX;
                    //
                    //     transform.position = pos;
                    //     transform.localScale = scale;
                    //     transform.rotation = quaternion;
                    //
                    //     Debug.LogError($"currecovTime{currecovTime}  rotation{transform.rotation} t{t}");
                    //     //float radians = math.radians(-degree);
                    //
                    //     //tran = tran.RotateZ(radians);
                    //     currecovTime -= Time.fixedDeltaTime;
                    // }
                    // else
                    // {
                    //     
                    // }
                }
            }
            else
            {
                // if (_rotateOffset != null)
                // {
                //     rotateOffset = _rotateOffset.transform.position;
                // }

                play = false;
                repeatTimes = CycleTimes == 0 ? 1 : CycleTimes;
                if (animType == 3)
                {
                    repeatTimes = args[6] == 0 ? 1 : args[6];
                    CycleInterval = Mathf.Max(args[7], 1);
                    //CycleInterval= Mathf.Max(CycleInterval, 1);
                    interval = CycleInterval / 1000f;
                }
            }
        }
    }


    private LocalTransform WeaponAnimTran1(List<int> animPara,
        float curAttackTime, float attackTime, LocalTransform tran)
    {
        //var origtran = tran;
        float p1 = animPara[0];
        float p2 = animPara[1];
        float p3 = animPara[2];
        float p4 = animPara[3];
        float p5 = animPara[4];
        float p6 = animPara[5];

        curAttackTime = attackTime - curAttackTime;

        var clamp1 = math.clamp(curAttackTime, 0,
            p1 / 1000f);

        float t1 = clamp1 / (p1 / 1000f);

        var degree = math.lerp(0, p3, t1);

        float radians = math.radians(-degree);
        radians = -radians;

        var curDistance = -math.lerp(0, p4 / 100f, t1);

        curDistance = -curDistance;

        tran = tran.Translate(new float3(curDistance, 0f, 0f));

        tran = MathHelper.RotateAround(tran, rotateOffset, radians);


        var clamp2 = math.clamp((curAttackTime - p1 / 1000f), 0,
            p2 / 1000f);

        float t2 = clamp2 / (p2 / 1000f);

        var curDistance2 = math.lerp(0, p5 / 100f, t2);

        curDistance2 = -curDistance2;

        tran = tran.Translate(new float3(curDistance2, 0f, 0f));


        // var clamp6 = math.clamp((curAttackTime - p1 / 1000f- p2 / 1000f), 0,
        //     p6 / 1000f);
        //
        // float t6 = clamp6 / (p6 / 1000f);
        //
        // var curDistance2 = math.lerp(0, p5 / 100f, t6);
        //
        // curDistance2 = -curDistance2;
        //
        // tran = tran.Translate(new float3(curDistance2, 0f, 0f));

        //Recov
        var clampEnd = math.clamp((curAttackTime - (p1 / 1000f) - (p2 / 1000f) - (p6 / 1000f)), 0,
            RecovTime / 1000f);

        float tR = clampEnd / (RecovTime / 1000f);

        var pos = math.lerp(tran.Position, ogiTran.Position, tR);

        var scale = math.lerp(tran.Scale, ogiTran.Scale, tR);
        var quaternion = math.nlerp(tran.Rotation, ogiTran.Rotation, tR);

        tran.Position = pos;
        tran.Scale = scale;
        tran.Rotation = quaternion;

        return tran;
    }

    private LocalTransform WeaponAnimTran2(List<int> animPara,
        float curAttackTime, float attackTime, LocalTransform tran)
    {
        float p1 = animPara[0];
        float p2 = animPara[1];
        float p3 = animPara[2];
        float p4 = animPara[3];
        float p5 = animPara[4];
        float p6 = animPara[5];
        float p7 = animPara[6];
        float p8 = animPara[7];

        curAttackTime = attackTime - curAttackTime;


        var clamp1 = math.clamp(curAttackTime, 0,
            p1 / 1000f);

        float t1 = clamp1 / (p1 / 1000f);


        var degree = math.lerp(0, p4, t1);


        float radians = math.radians(degree);

        radians = -radians;

        tran = MathHelper.RotateAround(tran, rotateOffset, radians);


        var s1 = math.lerp(0, p6 / 1000f, t1);
        var up1 = MathHelper.Forward(tran.Rotation);
        tran.Position += (s1 * up1);

        var clamp3 = math.clamp(curAttackTime - p1 / 1000f - p2 / 1000f, 0, p3 / 1000f);
        float t3 = clamp3 / (p3 / 1000f);


        var degree3 = math.lerp(0, p5, t3);

        float radians3 = math.radians(degree3);
        radians3 = -radians3;
        tran = MathHelper.RotateAround(tran, rotateOffset, radians3);


        var s2 = math.lerp(0, p7 / 1000f, t3);
        var up2 = MathHelper.Forward(tran.Rotation);
        var newup2 = MathHelper.RotateVector(up2, -45);

        tran.Position += (s2 * -newup2);


        //Recov
        var clampEnd = math.clamp((curAttackTime - (p1 / 1000f) - (p2 / 1000f) - (p3 / 1000f) - (p8 / 1000f)), 0,
            RecovTime / 1000f);

        float tR = clampEnd / (RecovTime / 1000f);

        var pos = math.lerp(tran.Position, ogiTran.Position, tR);

        var scale = math.lerp(tran.Scale, ogiTran.Scale, tR);
        var quaternion = math.nlerp(tran.Rotation, ogiTran.Rotation, tR);

        tran.Position = pos;
        tran.Scale = scale;
        tran.Rotation = quaternion;

        return tran;
    }

    private LocalTransform WeaponAnimTran3(List<int> animPara,
        float curAttackTime, float attackTime, LocalTransform tran)
    {
        float p1 = animPara[0];
        float p2 = animPara[1];
        float p3 = animPara[2];
        float p4 = animPara[3];
        float p5 = animPara[4];
        float p6 = animPara[5];
        curAttackTime = attackTime - curAttackTime;

        var clamp1 = math.clamp(curAttackTime, 0,
            p1 / 1000f);

        float t1 = clamp1 / (p1 / 1000f);


        var degree = math.lerp(0, p4, t1);

        float radians = math.radians(degree);


        radians = -radians;

        tran = MathHelper.RotateAround(tran, rotateOffset, radians);

        var clamp3 = math.clamp(curAttackTime - p1 / 1000f - p2 / 1000f, 0, p3 / 1000f);
        float t3 = clamp3 / (p3 / 1000f);


        var degree3 = math.lerp(0, p5, t3);

        float radians3 = math.radians(degree3);
        radians3 = -radians3;

        tran = MathHelper.RotateAround(tran, rotateOffset, radians3);

        //Recov
        var clampEnd = math.clamp((curAttackTime - (p1 / 1000f) - (p2 / 1000f) - (p3 / 1000f) - (p6 / 1000f)), 0,
            RecovTime / 1000f);

        float tR = clampEnd / (RecovTime / 1000f);

        var pos = math.lerp(tran.Position, ogiTran.Position, tR);

        var scale = math.lerp(tran.Scale, ogiTran.Scale, tR);
        var quaternion = math.nlerp(tran.Rotation, ogiTran.Rotation, tR);

        tran.Position = pos;
        tran.Scale = scale;
        tran.Rotation = quaternion;


        return tran;
    }


    private LocalTransform WeaponAnimTran4(List<int> animPara,
        float curAttackTime, float attackTime, LocalTransform tran)
    {
        float p1 = animPara[0];
        float p2 = animPara[1];
        float p3 = animPara[2];
        float p4 = animPara[3];
        float p5 = animPara[4];
        float p6 = animPara[5];
        float p7 = animPara[6];
        float p8 = animPara[7];


        curAttackTime = attackTime - curAttackTime;
        //Debug.LogError($"{rotateOffset}");
        rotateOffset.x = ogiTran.Position.x + p3 / 1000f;
        rotateOffset.y = ogiTran.Position.y + p4 / 1000f;
        rotateOffset.z = 0;

        var rotateOffset1 = new float3(ogiTran.Position.x + p1 / 1000f, ogiTran.Position.y + p2 / 1000f, 0);
        var dir = math.normalize(rotateOffset1 - rotateOffset);
        var caldegree = MathHelper.SignedAngle(dir, math.up());
        //float calangle = Vector3.Angle(dir, math.up());
        //var caldegree= math.degrees(calangle); 
        //Debug.LogError($"{caldegree} {dir}");
        var clamp1 = math.clamp(curAttackTime, 0,
            p5 / 1000f);

        float t1 = clamp1 / (p5 / 1000f);

        var degree = math.lerp(0, caldegree, t1);

        float radians = math.radians(degree);

        radians = -radians;

        tran = MathHelper.RotateAround(tran, rotateOffset, radians);


        var clamp2 = math.clamp(curAttackTime - p5 / 1000f, 0, p7 / 1000f);
        float t2 = clamp2 / (p7 / 1000f);

        var curDistance = -math.lerp(0, p6 / 100f, t2);

        curDistance = -curDistance;

        tran = tran.Translate(new float3(0f, curDistance, 0f));


        //放大缩小
        float scaleMulti = 1.5f;
        var clamp3 = math.clamp(curAttackTime - (p5 / 1000f) - (p7 / 1000f), 0, p8 / 1000f);

        float t3 = clamp3 / (p8 / 1000f);


        if (t3 < 0.5f)
        {
            var scale3 = math.lerp(1, scaleMulti, t3);

            tran.Scale *= scale3;
        }
        else
        {
            var scale3 = math.lerp(scaleMulti, 1, t3);

            tran.Scale *= scale3;
        }


        //Recov
        var clampEnd = math.clamp((curAttackTime - (p5 / 1000f) - (p7 / 1000f) - (p8 / 1000f)), 0,
            RecovTime / 1000f);

        float tR = clampEnd / (RecovTime / 1000f);

        var pos = math.lerp(tran.Position, ogiTran.Position, tR);

        var scale = math.lerp(tran.Scale, ogiTran.Scale, tR);
        var quaternion = math.nlerp(tran.Rotation, ogiTran.Rotation, tR);

        tran.Position = pos;
        tran.Scale = scale;
        tran.Rotation = quaternion;


        return tran;
    }


    private LocalTransform WeaponAnimTran5(List<int> animPara,
        float curAttackTime, float attackTime, LocalTransform tran)
    {
        float p1 = animPara[0];
        float p2 = animPara[1];
        float p3 = animPara[2];
        float p4 = animPara[3];
        float p5 = animPara[4];
        float p6 = animPara[5];
        float p7 = animPara[6];
        curAttackTime = attackTime - curAttackTime;
        rotateOffset = 0;
        var clamp1 = math.clamp(curAttackTime, 0,
            p2 / 1000f);

        float t1 = clamp1 / (p2 / 1000f);


        var degree = math.lerp(0, p1, t1);


        float radians = math.radians(degree);
        radians = -radians;
        tran = tran.RotateZ(radians);
        //tran = MathHelper.RotateAround(tran, rotateOffset, radians);


        var curDistance = -math.lerp(0, p3 / 100f, t1);

        curDistance = -curDistance;

        tran = tran.Translate(new float3(0f, curDistance, 0f));


        var clamp3 = math.clamp(curAttackTime - p2 / 1000f - p4 / 1000f, 0, p6 / 1000f);
        float t3 = clamp3 / (p6 / 1000f);


        var degree3 = math.lerp(0, p5, t3);

        float radians3 = math.radians(degree3);
        radians3 = -radians3;

        tran = tran.RotateZ(radians3);

        //tran = MathHelper.RotateAround(tran, tran.Position + rotateOffset, radians3);


        var curDistance2 = math.lerp(0, p7 / 100f, t3);

        curDistance2 = -curDistance2;

        tran = tran.Translate(new float3(0f, curDistance2, 0f));

        //Recov


        return tran;
    }
}