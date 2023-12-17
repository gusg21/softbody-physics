using System;
using System.Collections.Generic;
using UnityEngine;

namespace FinalProject
{
    internal class LingeringVector
    {
        public Vector2 Origin;
        public Vector2 Delta;

        public float Lifetime;
    }
    
    // this class is very good and useful, way to go !
    public class DebugHelper : MonoBehaviour
    {
        private static List<LingeringVector> _lingeringVectors = new();
        
        public static void AddLingeringVector(Vector2 origin, Vector2 delta)
        {
            _lingeringVectors.Add(new()
            {
                Origin = origin,
                Delta = delta,
                Lifetime = 10f
            });
        }

        private void Update()
        {
            List<int> deadIndices = new();
            for (var index = 0; index < _lingeringVectors.Count; index++)
            {
                _lingeringVectors[index].Lifetime -= Time.deltaTime;

                if (_lingeringVectors[index].Lifetime < 0f) deadIndices.Add(index);
            }

            foreach (var index in deadIndices) _lingeringVectors.RemoveAt(index);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            foreach (var lingeringVector in _lingeringVectors)
            {
                Gizmos.DrawSphere(lingeringVector.Origin, 0.1f);
                Gizmos.DrawRay(lingeringVector.Origin, lingeringVector.Delta);
            }
        }
    }
}