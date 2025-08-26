//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: 2023-07-18 10:12:22
//---------------------------------------------------------------------


using Unity.Entities;
using Unity.Mathematics;

namespace Main
{
    public struct InputData : IComponentData
    {
        //public ControllerType LastDevice;
        public float2 Move;
        public float2 MouseRotaVector;
        public float2 MouseDelta;
        public float2 StickRotaVector;
        public FixedStepButton DashButton;
        public FixedStepButton AttackButton;
        public FixedStepButton Skill0Button;
        public FixedStepButton Skill1Button;
        public FixedStepButton ReloadButton;

        public bool isAttack;

        public bool isSpace;
    }

    public struct FixedStepButton
    {
        public bool WasPressed;
        public bool WasReleased;
        public bool IsHeld;

        private uint _lastTick;
        private float _pressThreshold;

        public void SetPressedThreshold(float value)
        {
            _pressThreshold = value;
        }

        public void UpdateWithValue(float value, uint tick)
        {
            // Clear when there is a tick change
            if (tick != _lastTick)
            {
                WasPressed = false;
                WasReleased = false;
            }

            bool wasHeld = IsHeld;
            IsHeld = value > math.max(math.EPSILON, _pressThreshold);

            if (!wasHeld && IsHeld)
            {
                WasPressed = true;
            }
            else if (wasHeld && !IsHeld)
            {
                WasReleased = true;
            }

            _lastTick = tick;
        }
    }
}