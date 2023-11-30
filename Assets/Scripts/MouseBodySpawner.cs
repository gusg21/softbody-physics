using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace FinalProject
{
    public class MouseBodySpawner : MonoBehaviour
    {
        public float MaxRestLength = 3f;
        public float MinRestLength = 3f;
        
        public PhysicsBody BodyPrefab;
        public CollisionManager CollisionManager;
        public SpringManager SpringManager;

        private PhysicsBody _myBody;
        private LineRenderer _line;
        private List<PhysicsBody> _connections = new();

        private void Start()
        {
            _line = GetComponent<LineRenderer>();
        }

        public int CompareBodyDistances(PhysicsBody a, PhysicsBody b)
        {
            return Vector3.Distance(transform.position, a.transform.position)
                .CompareTo(Vector3.Distance(transform.position, b.transform.position));
        }

        public void Update()
        {
            if (Input.GetMouseButtonUp((int)MouseButton.LeftMouse))
            {
                if (_connections.Count >= 2)
                {
                    _myBody = Instantiate(BodyPrefab);
                    _myBody.transform.position = transform.position;
                    
                    foreach (var connection in _connections)
                    {
                        SpringManager.AddSpring(new Spring()
                        {
                            A = _myBody,
                            B = connection,
                            RestLength = Mathf.Clamp(
                                Vector3.Distance(_myBody.transform.position, connection.transform.position),
                                MinRestLength, MaxRestLength
                            )
                        });
                    }
                }
            }
        }

        public void FixedUpdate()
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            transform.position = mousePos;

            _connections.Clear();
            List<PhysicsBody> neighbors = CollisionManager.Grid.GetLargerNeighborhood(transform.position, 5);
            neighbors.Sort(CompareBodyDistances);
            for (int i = 0; i < 3; i++)
            {
                if (i < neighbors.Count)
                {
                    var body = neighbors[i];
                    if (Vector3.Distance(transform.position, body.transform.position) < MaxRestLength)
                        _connections.Add(body);
                }
            }
            
            
            // Helper strut
            if (_connections.Count == 2)
            {
                var a = _connections[0];
                var b = _connections[1];
                if (!SpringManager.SpringExistsBetweenBodies(a, b))
                {
                    var mutual = SpringManager.GetMutualConnectionBetweenBodies(a, b);
                    if (mutual != null)
                    {
                        _connections.Add(mutual);
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            foreach (var body in _connections)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, body.transform.position);
            }
        }
    }
}