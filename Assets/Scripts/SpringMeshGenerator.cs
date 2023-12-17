using System;
using System.Collections.Generic;
using UnityEngine;

namespace FinalProject
{
    public class SpringMeshGenerator : MonoBehaviour
    {
        public PhysicsBody BodyPrefab;
        public SpringManager SpringManager;
        public int MeshWidth = 10;
        public int MeshHeight = 10;
        public float MeshSpacing = 2f;
        
        private List<PhysicsBody> _bodies = new();

        // I did not go into details, but it is always useful to have this kind of tool !

        private void Start()
        {
            GenerateMesh(MeshWidth, MeshHeight, MeshSpacing);
        }

        
        public void GenerateMesh(int width, int height, float spacing)
        {
            _bodies.Clear();
            
            float diagonalSpacing = Mathf.Sqrt(Mathf.Pow(spacing, 2f) + Mathf.Pow(spacing, 2f));
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var body = Instantiate(BodyPrefab);
                    body.transform.position = transform.position + new Vector3(
                        x * spacing, y * spacing, 0f
                    );
                    _bodies.Add(body);
                }
            }

            for (var index = 0; index < _bodies.Count; index++)
            {
                var bodyA = _bodies[index];
                int x = index % width;
                int y = index / width;

                if (x < width - 1)
                {
                    // Right
                    var rightAtt = new Spring();
                    rightAtt.A = bodyA;
                    rightAtt.B = _bodies[index + 1];
                    rightAtt.RestLength = spacing;
                    SpringManager.AddSpring(rightAtt);
                }

                if (x < width - 1 && y < height - 1)
                {
                    // Diagonal
                    var diagAtt = new Spring();
                    diagAtt.A = bodyA;
                    diagAtt.B = _bodies[index + width + 1];
                    diagAtt.RestLength = diagonalSpacing;
                    SpringManager.AddSpring(diagAtt);
                }

                if (x > 0 && y < height - 1)
                {
                    // Diagonal Reverse
                    var diagAtt = new Spring();
                    diagAtt.A = bodyA;
                    diagAtt.B = _bodies[index + width - 1];
                    diagAtt.RestLength = diagonalSpacing;
                    SpringManager.AddSpring(diagAtt);
                }

                if (y < height - 1)
                {
                    // Down
                    var downAtt = new Spring();
                    downAtt.A = bodyA;
                    downAtt.B = _bodies[index + width];
                    downAtt.RestLength = spacing;
                    SpringManager.AddSpring(downAtt);
                }
            }
        }
    }
}