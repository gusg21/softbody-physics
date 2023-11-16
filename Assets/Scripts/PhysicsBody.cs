using System;
using UnityEngine;

namespace FinalProject
{
    public class PhysicsBody : MonoBehaviour
    {
        
        [SerializeField] private float _invMass = 1f;
        [SerializeField] private Vector2 _gravity = new(0, -9.8f);
        [SerializeField] private PhysicsShape _shape;

        private Vector2 _velocity = Vector2.zero;
        private Vector2 _acceleration = Vector2.zero;
        private float _damping = 0.99f;
        private Vector2 _accumulatedForces = Vector2.zero;

        private void FixedUpdate()
        {
            Integrator.Integrate(this, Time.deltaTime);
            ClearForces();
        }

        public PhysicsShape GetShape() => _shape;
        public float GetInverseMass() => _invMass;
        public float GetMass() => 1 / _invMass;
        public Vector2 GetVelocity() => _velocity;
        public void SetVelocity(Vector2 vel) => _velocity = vel;
        public Vector2 GetAcceleration() => _acceleration;
        public void SetAcceleration(Vector2 acceleration) => _acceleration = acceleration;
        public Vector2 GetAccumulatedForces() => _accumulatedForces;
        public void ClearForces() => _accumulatedForces = Vector2.zero;
        public void AddForce(Vector2 force) => _accumulatedForces += force;
        public Vector2 GetGravity() => _gravity;
        public float GetDamping() => _damping;
    }
}