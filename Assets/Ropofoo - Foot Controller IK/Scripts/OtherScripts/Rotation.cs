using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASD
{
    public class Rotation : MonoBehaviour
    {
        public float speed = 5f;
        public Vector3 axis = Vector3.zero;

        public void FixedUpdate()
        {
            transform.Rotate(axis, speed);
        }
    }
}