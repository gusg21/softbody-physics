using System;
using System.Collections.Generic;
using SpatialPartition;
using UnityEngine;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using ColInfo = FinalProject.CollisionDetection.CollisionInfo;

namespace FinalProject
{
    public class CollisionManager : MonoBehaviour
    {
        private const float GRID_SIZE = 6f;
        
        public SPGridGeneric<PhysicsBody> Grid = new(GRID_SIZE, GRID_SIZE);

        private List<ColInfo> _frameContacts = new();
        private int _collisionCheckCount = 0;

        // YES !! someone did simplify this code
        public static ColInfo ApplyCollisionResolution(PhysicsBody body1, PhysicsBody body2)
        {
            const float EPSILON = 0.000001f;

            Profiler.BeginSample("Collision Detection");
            
            ColInfo info;
            info = CollisionDetection.GetNormalAndPenetration(body1.GetShape(), body2.GetShape());

            Profiler.EndSample();
            
            Vector2 normal = info.Normal;
            float penetration = info.Penetration;
            // Vector2 contact = info.Contact; // commented

            if (penetration < 0) // Objects not actually overlapping
                return info;

            // commented code
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
                return info;
            }

            body1.SetVelocity(body1.GetVelocity() -
                              normal * (closingVelocity * -2) * s1MassRatio * body1.GetBounciness());
            body2.SetVelocity(body2.GetVelocity() +
                              normal * (closingVelocity * -2) * s2MassRatio * body2.GetBounciness());
            
            Profiler.EndSample();

            return info;
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
                _collisionCheckCount = 0;
                _frameContacts.Clear();
                
                int randomIndexOffset = Random.Range(0, int.MaxValue);
                var bodies = PhysicsBody.AllBodies;
                for (var bodyIndex = 0; bodyIndex < bodies.Count; bodyIndex++)
                {
                    // this wrapped index does not make sense, but if it helped you, all good !
                    var wrappedIndex = (bodyIndex + randomIndexOffset) % bodies.Count;
                    var body1 = bodies[wrappedIndex];
                    
                    // Check against other spheres
                    var neighbors = Grid.GetNeighbors(body1.transform.position);
                    foreach (var body2 in neighbors)
                    {
                        if (body1 != body2)
                        {
                            var info = ApplyCollisionResolution(body1, body2);
                            if (info.IsColliding())
                                _frameContacts.Add(info);
                            _collisionCheckCount += 1;
                        }
                    }
                }
            }
            Profiler.EndSample();
        }

        public bool QueryPoint(Vector2 position)
        {
            // Check against other spheres
            var neighbors = Grid.GetNeighbors(position);
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
            var neighbors = Grid.GetNeighbors(position);
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
            GUILayout.Label("checks/body: " + (_collisionCheckCount / PhysicsBody.AllBodies.Count));
            GUILayout.Label("contacts: " + _frameContacts.Count);
            GUILayout.Label("fps:" + 1f/Time.deltaTime);
        }
    }
}