using System;
using UnityEngine;

namespace FinalProject
{
    public class PlaneShape : PhysicsShape // OK
    {
        public PlaneShape() : base(PhysicsShapeType.PLANE) {}
        
        public Vector2 GetNormal() => transform.up.normalized;
        public float GetOffset() => Vector2.Dot(GetNormal(), transform.position);

        private void OnDrawGizmos()
        {
            float d = 100;
            Gizmos.DrawWireCube(transform.position, new Vector3(d, 0, d));
        }
    }
}