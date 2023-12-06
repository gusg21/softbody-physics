using System;
using System.Collections.Generic;
using SpatialPartition;
using UnityEngine;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace FinalProject
{
    public class CollisionManager : MonoBehaviour
    {
        private const float GRID_SIZE = 6f;
        
        public SPGridGeneric<PhysicsBody> Grid = new(GRID_SIZE, GRID_SIZE);

        private int _collisionCount = 0;

        public static void ApplyCollisionResolution(PhysicsBody body1, PhysicsBody body2)
        {
            const float EPSILON = 0.000001f;

            Profiler.BeginSample("Collision Detection");
            
            CollisionDetection.CollisionInfo info;
            {
                info = CollisionDetection.GetNormalAndPenetration(body1.GetShape(), body2.GetShape());
            }

            Profiler.EndSample();
            
            Vector2 normal = info.Normal;
            float penetration = info.Penetration;
            Vector2 contact = info.Contact;

            if (penetration < 0) // Objects not actually overlapping
                return;

            // DebugHelper.AddLingeringVector(contact, normal * info.Penetration * 10f);
            
            Profiler.BeginSample("Collision Resolution");

            float totalMass = 1f / (body1.GetInverseMass() + body2.GetInverseMass() + EPSILON);
            float s1MassRatio = totalMass * body1.GetInverseMass();
            float s2MassRatio = totalMass * body2.GetInverseMass();

            body1.transform.position += (Vector3)(normal * penetration * s1MassRatio);
            body2.transform.position -= (Vector3)(normal * penetration * s2MassRatio);

            Vector3 velocityDiff = body2.GetVelocity() - body1.GetVelocity();
            float closingVelocity = Vector3.Dot(velocityDiff, normal);

            if (closingVelocity < 0) // Objects not actually colliding
            {
                Profiler.EndSample();
                return;
            }

            body1.SetVelocity(body1.GetVelocity() -
                              normal * (closingVelocity * -2) * s1MassRatio * body1.GetBounciness());
            body2.SetVelocity(body2.GetVelocity() +
                              normal * (closingVelocity * -2) * s2MassRatio * body2.GetBounciness());
            
            Profiler.EndSample();
        }

        private void FixedUpdate()
        {
            Profiler.BeginSample("Update Boxes");
            {
                Grid.UpdateBoxes(
                    PhysicsBody.AllBodies,
                    positionFunc: body => body.transform.position,
                    universalCheck: body => body.GetShape() is PlaneShape
                );
            }
            Profiler.EndSample();

            Profiler.BeginSample("Collision");
            {
                _collisionCount = 0;
                int randomOffset = Random.Range(0, int.MaxValue);
                var bodies = PhysicsBody.AllBodies;
                for (var bodyIndex = 0; bodyIndex < bodies.Count; bodyIndex++)
                {
                    var wrappedIndex = (bodyIndex + randomOffset) % bodies.Count;
                    var body1 = bodies[wrappedIndex];
                    // Check against other spheres
                    Profiler.BeginSample("Get Neighbors");
                    var neighbors = Grid.GetNeighbors(body1.transform.position);
                    Profiler.EndSample();
                    foreach (var body2 in neighbors)
                    {
                        if (body1 != body2)
                        {
                            ApplyCollisionResolution(body1, body2);
                            _collisionCount += 1;
                        }
                    }
                }
            }
            Profiler.EndSample();
        }

        public bool QueryPoint(Vector2 position)
        {
            // Check against other spheres
            Profiler.BeginSample("Get Neighbors");
            var neighbors = Grid.GetNeighbors(position);
            Profiler.EndSample();
            foreach (var body2 in neighbors)
            {
                if (CollisionDetection.PointOverlapsShape(position, body2.GetShape()))
                    return true;
            }

            return false;
        }
        
        public bool QueryCircle(Vector2 position, float radius)
        {
            // Check against other spheres
            Profiler.BeginSample("Get Neighbors");
            var neighbors = Grid.GetNeighbors(position);
            Profiler.EndSample();
            foreach (var body2 in neighbors)
            {
                if (CollisionDetection.CircleOverlapsShape(position, radius, body2.GetShape()))
                    return true;
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            Grid.DrawGizmos();
        }

        private void OnGUI()
        {
            GUILayout.Label("col/body: " + (_collisionCount / PhysicsBody.AllBodies.Count));
        }
    }
}