using UnityEngine;

namespace FinalProject // btw, good to use a namespace
{
    public enum PhysicsShapeType
    {
        PLANE, CIRCLE // you should put those on different lines in case you want to add more it's easier to merge
    }
    
    public class PhysicsShape : MonoBehaviour // I like this kind of code
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