using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Main
{
    public struct StateMachine : IComponentData
    {
        public State currentState;
        public float speed;
        public float3 startTranslation;
        public bool isInitialized;
        public int transitionToStateIndex;

        public FixedString128Bytes animStr;
        public AnimationEnum curAnim;
        public AnimationEnum lastAnim;


        public float animSpeedScale;
    }

    //[InternalBufferCapacity(0)]
    public struct StateTypeMap : IBufferElementData
    {
        public State.TypeId typeId;
        public int index;
    }
}