using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace FinalProject
{
    public class Spring
    {
        public PhysicsBody A;
        public PhysicsBody B;
        public float RestLength;
    }

    public class SpringManager : MonoBehaviour
    {
        private const float INTERACT_RADIUS = 0.5f;

        private List<Spring> _springs = new(); // ok, this makes sense
        // the use of a map here as an acceleration structure is good
        private Dictionary<PhysicsBody, List<PhysicsBody>> _network = new();

        [SerializeField] private float _snapLength; // could be in the spring settings
        [SerializeField] private SpringSettings _settings; // makes sense to have a unique setting

        private void FixedUpdate()
        {
            List<Spring> springsToRemove = new();
            foreach (var spring in _springs)
            {
                
                // Snapping
                if (Vector3.Distance(spring.A.transform.position, spring.B.transform.position) > _snapLength) // ok
                {
                    springsToRemove.Add(spring);
                }
                else
                {
                    // Spring is NOT stretched to far, apply forces
                    // ok, this works well
                    var otherForce = ComputeForce(spring.A, spring.B, spring.RestLength, _settings);
                    spring.A.AddForce(-otherForce);
                    spring.B.AddForce(otherForce);
                }
                

                // Interactivity
                if (Input.GetMouseButton((int)MouseButton.RightMouse))
                {
                    // very small issue, but I would split the fixed update into more smaller functions
                    var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mouseWorldPos.z = 0;

                    var a = spring.A.transform.position;
                    var b = spring.B.transform.position;

                    // not sure on the details, but looks like good maths with plenty of Dot products
                    var distance = Vector3.Distance(a, b);
                    var direction = (b - a).normalized;
                    var aDot = Vector3.Dot(direction, a);
                    var bDot = Vector3.Dot(direction, b);
                    var lengthAlong = Vector3.Dot(direction, mouseWorldPos);
                    var closestPoint = Vector3.Lerp(a, b, (Mathf.Clamp(lengthAlong, aDot, bDot) - aDot) / distance);

                    // could have been uncommented but hidden behind a debug option
                    // Debug.Log(Vector3.Distance(mouseWorldPos, closestPoint));

                    if (Vector3.Distance(mouseWorldPos, closestPoint) < INTERACT_RADIUS)
                    {
                        springsToRemove.Add(spring);
                    }
                }
                
            }
            
            foreach (var deadSpring in springsToRemove)
                RemoveSpring(deadSpring); // good to have a list to remove and do them all at the end
        }

        public void AddSpring(Spring spring)
        {
            _springs.Add(spring);
            
            // I do not know for Dictionaries in Unity, but usually maps have functions like
            // "FindOrCreate", would save a small amount of code here
            if (!_network.ContainsKey(spring.A)) _network.Add(spring.A, new());
            if (!_network.ContainsKey(spring.B)) _network.Add(spring.B, new());
            
            _network[spring.A].Add(spring.B);
            _network[spring.B].Add(spring.A);
        }

        public void RemoveSpring(Spring spring)
        {
            _springs.Remove(spring);

            // yup, the drawback of the acceleration structure is to make sure to update it
            _network[spring.A].Remove(spring.B);
            _network[spring.B].Remove(spring.A);
        }
        
        public List<Spring> GetSprings() => _springs;

        public bool SpringExistsBetweenBodies(PhysicsBody a, PhysicsBody b)
        {
            if (!_network.ContainsKey(a)) return false;
            
            return _network[a].Contains(b);
        }

        public PhysicsBody GetMutualConnectionBetweenBodies(PhysicsBody a, PhysicsBody b)
        {
            if (!_network.ContainsKey(a) || !_network.ContainsKey(b)) return null;
            
            foreach (var aConnection in _network[a])
            {
                if (_network[b].Contains(aConnection))
                    return aConnection;
            }

            return null;
        }

        public SpringSettings GetSettings() => _settings;
        public void SetSettings(SpringSettings settings) => _settings = settings;

        private static Vector3 ComputeForce(PhysicsBody a, PhysicsBody b, float restLength, SpringSettings settings)
        {
            if (a == null || b == null)
            {
                Debug.LogWarning("No other body!"); // useful kind of debug
                return new();
            }

            Vector3 aPos = a.transform.position;
            Vector3 bPos = b.transform.position;
            float stiffness = settings.Stiffness;

            Vector3 offset = aPos - bPos;
            float length = offset.magnitude;

            float displacement = length - restLength;
            float springForceMagnitude = stiffness * displacement;
            Vector3 springForceDir = offset.normalized;
            Vector2 springVelocity = b.GetVelocity() - a.GetVelocity();
            Vector3 dampingForce = Vector3.Dot(springForceDir, springVelocity) 
                              * settings.DampingRatio * -springForceDir; // ok, a bit confusing to have a -
                                                                         // like this, the velocity could be a - b and you would avoind a - later

            return springForceDir * springForceMagnitude + dampingForce;
        }

        private void OnDrawGizmos()
        {
            
            foreach (var spring in _springs) // ok
            {
                var aPos = spring.A.transform.position;
                var bPos = spring.B.transform.position;

                var absDiff = Mathf.Abs(spring.RestLength - Vector3.Distance(aPos, bPos));
                Gizmos.color = Color.Lerp(Color.green, Color.red, absDiff / 5f);
                Gizmos.DrawLine(aPos, bPos);
            }

            // do not submit commented code
        }
    }
}