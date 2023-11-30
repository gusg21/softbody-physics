using UnityEngine;

namespace FinalProject
{
    public enum PhysicsShapeType
    {
        PLANE, CIRCLE
    }
    
    public class PhysicsShape : MonoBehaviour
    {
        private PhysicsShapeType _type;

        public PhysicsShape(PhysicsShapeType type)
        {
            _type = type;
        }

        public PhysicsShapeType GetShapeType() => _type;
    }
}