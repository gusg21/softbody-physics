using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FinalProject
{
    // ok
    public class CameraBackgroundRandomizer : MonoBehaviour
    {
        private Camera _camera;

        private void Start()
        {
            _camera = GetComponent<Camera>();
            _camera.backgroundColor = Color.HSVToRGB(Random.Range(0f, 1f), 0.3f, 0.7f);
        }
    }
}