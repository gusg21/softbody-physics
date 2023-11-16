using UnityEngine;

namespace FinalProject
{
    public class CollisionManager : MonoBehaviour
    {
        public static void ApplyCollisionResolution(PhysicsBody body1, PhysicsBody body2)
        {
            const float EPSILON = 0.000001f;
        
            CollisionDetection.GetNormalAndPenetration(body1.GetShape(), body2.GetShape(), out var info);

            Vector2 normal = info.Normal;
            float penetration = info.Penetration;
            Vector2 contact = info.Contact;
            
            if (penetration < 0) // Objects not actually overlapping
                return;
            
            DebugHelper.AddLingeringVector(contact, normal * info.Penetration * 10f);
        
            float totalMass = 1f / (body1.GetInverseMass() + body2.GetInverseMass() + EPSILON);
            float s1MassRatio = totalMass * body1.GetInverseMass();
            float s2MassRatio = totalMass * body2.GetInverseMass();

            body1.transform.position += (Vector3)(normal * penetration * s1MassRatio);
            body2.transform.position -= (Vector3)(normal * penetration * s2MassRatio);

            Vector3 velocityDiff = body2.GetVelocity() - body1.GetVelocity();
            float closingVelocity = Vector3.Dot(velocityDiff, normal);

            if (closingVelocity < 0) // Objects not actually colliding
                return;

            body1.SetVelocity(body1.GetVelocity() - normal * (closingVelocity * -2) * s1MassRatio);
            body2.SetVelocity(body2.GetVelocity() + normal * (closingVelocity * -2) * s2MassRatio);
        }
        
        private void FixedUpdate()
        {
            PhysicsBody[] bodies = FindObjectsOfType<PhysicsBody>();

            for (int i = 0; i < bodies.Length; i++)
            {
                var body1 = bodies[i];
            
                // Check against other spheres
                for (int j = i + 1; j < bodies.Length; j++)
                {
                    var body2 = bodies[j];
                    ApplyCollisionResolution(body1, body2);
                }
            }
        }
    }
}