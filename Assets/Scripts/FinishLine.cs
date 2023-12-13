using System;
using UnityEngine;

namespace FinalProject
{
    public class FinishLine : MonoBehaviour
    {
        public static float FinishY;

        private void Start()
        {
            FinishY = transform.position.y;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position - transform.right * 1000f, transform.position + transform.right * 1000f);
        }
    }
}