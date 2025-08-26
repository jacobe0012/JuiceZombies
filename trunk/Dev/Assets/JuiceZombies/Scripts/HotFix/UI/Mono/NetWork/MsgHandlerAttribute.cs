using System;

namespace HotFix_UI
{
    // 用于标记处理方法的特性
    [AttributeUsage(AttributeTargets.Method)]
    public class MsgHandlerAttribute : Attribute
    {
        public Type MsgType { get; }

        public MsgHandlerAttribute(Type msgType)
        {
            MsgType = msgType;
        }
    }
}