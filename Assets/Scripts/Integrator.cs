using UnityEngine;

namespace FinalProject
{
    public static class Integrator
    {
        public static void Integrate(PhysicsBody body, float dt) // ok, efficient to do it this way
                                                                 // in real cases, you usually have a sub-structure just for these params
                                                                 // and not the whole body, but this is good
        {
            body.transform.position += (Vector3)(body.GetVelocity() * dt);

            body.SetAcceleration(body.GetAccumulatedForces() * body.GetInverseMass() + body.GetGravity());

            var velocity = body.GetVelocity();
            velocity += body.GetAcceleration() * dt;
            velocity *= Mathf.Pow(body.GetDamping(), dt);
            body.SetVelocity(velocity);
        }
    }
}