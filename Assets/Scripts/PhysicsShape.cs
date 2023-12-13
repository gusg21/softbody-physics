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
        private PhysicsBody _body;

        public PhysicsShape(PhysicsShapeType type)
        {
            _type = type;
        }

        public PhysicsShapeType GetShapeType() => _type;
        public PhysicsBody GetBody() => _body;
        public void SetBody(PhysicsBody body) => _body = body;
    }
}