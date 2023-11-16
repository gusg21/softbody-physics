using UnityEngine;

namespace FinalProject
{
    public class CollisionDetection
    {
        public class CollisionInfo
        {
            public Vector2 Normal = Vector2.up;
            public float Penetration = 0f;
            public Vector2 Contact = Vector2.zero;
        }
        
        public static void GetNormalAndPenetration(PhysicsShape shapeA, PhysicsShape shapeB, out CollisionInfo info)
        {
            switch (shapeA)
            {
                case CircleShape circleShapeA:
                    switch (shapeB)
                    {
                        case CircleShape circleShapeB:
                            GetNormalAndPenetration_CircleCircle(circleShapeA, circleShapeB, out info);
                            return;
                        case PlaneShape planeShapeB:
                            GetNormalAndPenetration_CirclePlane(circleShapeA, planeShapeB, out info);
                            return;
                    }
                    break;
                case PlaneShape planeShapeA:
                    switch (shapeB)
                    {
                        case CircleShape circleShapeB:
                            GetNormalAndPenetration_CirclePlane(circleShapeB, planeShapeA, out info);
                            return;
                        case PlaneShape:
                            Debug.LogError($"Plane-plane not supported!");
                            break;
                    }
                    break;
            }
            
            Debug.LogError($"Some unsupported physics shape: {shapeA.name} or {shapeB.name}!");
            info = new();
        }
        
        private static void GetNormalAndPenetration_CircleCircle(CircleShape c1, CircleShape c2, out CollisionInfo info)
        {
            info = new();
            
            Vector2 difference = c1.GetCenter() - c2.GetCenter();
            
            info.Normal = difference.normalized;
            info.Penetration = c1.GetRadius() + c2.GetRadius() - difference.magnitude;
        }
        
        private static void GetNormalAndPenetration_CirclePlane(CircleShape c, PlaneShape p, out CollisionInfo info)
        {
            info = new();
            float distance = Vector2.Dot(c.GetCenter(), p.GetNormal());
            var penetration = c.GetRadius() + p.GetOffset() - distance;

            info.Normal = p.GetNormal();
            info.Penetration = penetration;
        }
    }
}