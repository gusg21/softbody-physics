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

        private static CollisionInfo GetNormalAndPenetration_CircleCircle(CircleShape c1, CircleShape c2) =>
            GetNormalAndPenetration_CircleCircle(c1.GetCenter(), c1.GetRadius(), c2.GetCenter(), c2.GetRadius());

        private static CollisionInfo GetNormalAndPenetration_CircleCircle(Vector3 circle1Pos, float circle1Radius,
            Vector3 circle2Pos, float circle2Radius)
        {
            Profiler.BeginSample("C-C Collision");

            CollisionInfo info = new();

            Vector2 difference = circle1Pos - circle2Pos;

            info.Normal = difference.normalized;
            info.Penetration = circle1Radius + circle2Radius - difference.magnitude;

            Profiler.EndSample();

            return info;
        }

        private static CollisionInfo GetNormalAndPenetration_CirclePlane(CircleShape c, PlaneShape p) =>
            GetNormalAndPenetration_CirclePlane(c.GetCenter(), c.GetRadius(), p.GetNormal(), p.GetOffset());

        private static CollisionInfo GetNormalAndPenetration_CirclePlane(Vector3 circlePos, float circleRadius,
            Vector3 planeNormal, float planeOffset)
        {
            Profiler.BeginSample("C-P Collision");

            CollisionInfo info = new();

            float distance = Vector2.Dot(circlePos, planeNormal);
            var penetration = circleRadius + planeOffset - distance;

            info.Normal = planeNormal;
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

        public static bool CircleOverlapsShape(Vector3 position, float radius, PhysicsShape shape)
        {
            switch (shape.GetShapeType())
            {
                case PhysicsShapeType.PLANE:
                {
                    var planeShape = (PlaneShape)shape;
                    var info = GetNormalAndPenetration_CirclePlane(position, radius, planeShape.GetNormal(),
                        planeShape.GetOffset());
                    return info.Penetration > 0;
                }
                case PhysicsShapeType.CIRCLE:
                {
                    var circleShape = (CircleShape)shape;
                    var info = GetNormalAndPenetration_CircleCircle(position, radius, circleShape.GetCenter(),
                        circleShape.GetRadius());
                    return info.Penetration > 0;
                }
                default:
                    return false;
            }

            return false;
        }
    }
}