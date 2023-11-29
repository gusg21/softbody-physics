using UnityEngine;

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
            switch (shapeA)
            {
                case CircleShape circleShapeA:
                    switch (shapeB)
                    {
                        case CircleShape circleShapeB:
                            return GetNormalAndPenetration_CircleCircle(circleShapeA, circleShapeB);
                        case PlaneShape planeShapeB:
                            return GetNormalAndPenetration_CirclePlane(circleShapeA, planeShapeB);
                    }

                    break;
                case PlaneShape planeShapeA:
                    switch (shapeB)
                    {
                        case CircleShape circleShapeB:
                            return GetNormalAndPenetration_CirclePlane(circleShapeB, planeShapeA);
                        case PlaneShape:
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
            CollisionInfo info = new();

            Vector2 difference = c1.GetCenter() - c2.GetCenter();

            info.Normal = difference.normalized;
            info.Penetration = c1.GetRadius() + c2.GetRadius() - difference.magnitude;

            return info;
        }

        private static CollisionInfo GetNormalAndPenetration_CirclePlane(CircleShape c, PlaneShape p)
        {
            CollisionInfo info = new();
            
            float distance = Vector2.Dot(c.GetCenter(), p.GetNormal());
            var penetration = c.GetRadius() + p.GetOffset() - distance;

            info.Normal = p.GetNormal();
            info.Penetration = penetration;

            return info;
        }
    }
}