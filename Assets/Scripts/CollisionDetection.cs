using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace FinalProject
{
    public class CollisionDetection
    {
        public struct CollisionInfo
        {
            public Vector2 Normal;
            public float Penetration;
            public Vector2 Contact;
        }

        public static CollisionInfo GetNormalAndPenetration(PhysicsShape shapeA, PhysicsShape shapeB)
        {
            switch (shapeA.GetShapeType())
            {
                case PhysicsShapeType.CIRCLE:
                    switch (shapeB.GetShapeType())
                    {
                        case PhysicsShapeType.CIRCLE:
                            return GetNormalAndPenetration_CircleCircle((CircleShape)shapeA, (CircleShape)shapeB);
                        case PhysicsShapeType.PLANE:
                            return GetNormalAndPenetration_CirclePlane((CircleShape)shapeA, (PlaneShape)shapeB);
                    }

                    break;
                case PhysicsShapeType.PLANE:
                    switch (shapeB.GetShapeType())
                    {
                        case PhysicsShapeType.CIRCLE:
                            return GetNormalAndPenetration_CirclePlane((CircleShape)shapeB, (PlaneShape)shapeA);
                        case PhysicsShapeType.PLANE:
                            Debug.LogError($"Plane-plane not supported!");
                            break;
                    }

                    break;
            }

            Debug.LogError($"Some unsupported physics shape: {shapeA.name} or {shapeB.name}!");
            
            return new();
        }

        private static CollisionInfo GetNormalAndPenetration_CircleCircle(CircleShape c1, CircleShape c2)
        {
            Profiler.BeginSample("C-C Collision");
            
            CollisionInfo info = new();

            Vector2 difference = c1.GetCenter() - c2.GetCenter();

            info.Normal = difference.normalized;
            info.Penetration = c1.GetRadius() + c2.GetRadius() - difference.magnitude;

            Profiler.EndSample();
            
            return info;
        }

        private static CollisionInfo GetNormalAndPenetration_CirclePlane(CircleShape c, PlaneShape p)
        {
            Profiler.BeginSample("C-P Collision");
            
            CollisionInfo info = new();
            
            float distance = Vector2.Dot(c.GetCenter(), p.GetNormal());
            var penetration = c.GetRadius() + p.GetOffset() - distance;

            info.Normal = p.GetNormal();
            info.Penetration = penetration;

            Profiler.EndSample();
            
            return info;
        }
        
        public static bool PointOverlapsShape(Vector3 position, PhysicsShape shape)
        {
            switch (shape.GetShapeType())
            {
                case PhysicsShapeType.PLANE:
                    var planeShape = (PlaneShape)shape;
                    float distance = Vector2.Dot(position, planeShape.GetNormal());
                    return distance < planeShape.GetOffset();
                case PhysicsShapeType.CIRCLE:
                    var circleShape = (CircleShape)shape;
                    return Vector2.Distance(position, circleShape.GetCenter()) < circleShape.GetRadius();
                default:
                    return false;
            }

            return false;
        }
    }
}