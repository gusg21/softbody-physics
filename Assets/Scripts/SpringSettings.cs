using System;
using UnityEngine;

namespace FinalProject
{
    [Serializable]
    public class SpringSettings : ICloneable
    {
        public float DampingRatio = 0.8f;
        public float Stiffness = 10f;

        public object Clone() => MemberwiseClone();
    }
}