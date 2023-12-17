using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FinalProject
{
    public class CircleShape : PhysicsShape // OK
    {
        [SerializeField] private float _radius;

        public CircleShape() : base(PhysicsShapeType.CIRCLE) {}
        
        public float GetRadius() => _radius;
        public Vector2 GetCenter() => transform.position;
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, GetRadius());
        }
    }
}
