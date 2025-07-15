using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotFix_UI
{
    public class UIAnimationTools : MonoBehaviour
    {
        [SerializeField] public List<AnimationTran> animationTrans = new List<AnimationTran>();
        [SerializeField] public List<AnimationScale> animationScales = new List<AnimationScale>();
        [SerializeField] public List<AnimationAlpha> animationAlphas = new List<AnimationAlpha>();

        private void Start()
        {
        }

        [Serializable]
        public struct AnimationTran : IAnimationTools
        {
            public Vector2 offset0;
            public Vector2 offset1;
            public float duration;
            public float startTime;
        }

        [Serializable]
        public struct AnimationScale : IAnimationTools
        {
            public float offset0;
            public float offset1;
            public float duration;
            public float startTime;
        }

        [Serializable]
        public struct AnimationAlpha : IAnimationTools
        {
            public float offset0;
            public float offset1;
            public float duration;
            public float startTime;
        }

        public interface IAnimationTools
        {
        }
    }
}