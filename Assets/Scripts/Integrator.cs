using UnityEngine;

namespace FinalProject
{
    public static class Integrator
    {
        public static void Integrate(PhysicsBody body, float dt)
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