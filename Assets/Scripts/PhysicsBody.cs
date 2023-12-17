using System;
using System.Collections.Generic;
using UnityEngine;

namespace FinalProject
{
    public class PhysicsBody : MonoBehaviour
    {
        public static List<PhysicsBody> AllBodies = new(); // I see this is used, but I'm not sure this is necessary
        
        [SerializeField] private float _invMass = 1f;
        [SerializeField] private Vector2 _gravity = new(0, -9.8f); // gravity would be better as a global constant
        [SerializeField] private PhysicsShape _shape;
        [SerializeField] private float _bounciness = 0.5f;

        private Vector2 _velocity = Vector2.zero;
        private Vector2 _acceleration = Vector2.zero;
        private float _damping = 0.8f;
        private Vector2 _accumulatedForces = Vector2.zero;

        private void Start()
        {
            AllBodies.Add(this);
            
            if (_shape != null) _shape.SetBody(this);
        }

        private void OnDestroy()
        {
            AllBodies.Remove(this);
            
            if (_shape != null) _shape.SetBody(null);
        }

        private void FixedUpdate()
        {
            Integrator.Integrate(this, Time.deltaTime); // ok
            ClearForces(); // ok
        }

        // for this type of classes, I just put everything public and avoid this wall of setter/getter
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
        public float GetBounciness() => _bounciness;
    }
}