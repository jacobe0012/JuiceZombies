using UnityEngine;

/// <summary>
/// 自定义UI基类,至少有一些属性
/// </summary>
public class UIBase : MonoBehaviour
{
    //冷静时间
    protected float cooldownTime = 0.4f;
    protected float lastClickTime = 0f;
}