using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASD
{
    public class UnitInput : MonoBehaviour
    {
        public float Horizontal { get; private set; }
        public float Vertical { get; private set; }
        public float XRotation { get; private set; }
        public float YRotation { get; private set; }

        void Update()
        {
            MouseInput();
            KeyboardInput();
        }
        private void MouseInput()
        {
            XRotation = Input.GetAxis(GameStatics.MouseX);
            YRotation = Input.GetAxis(GameStatics.MouseY);
        }
        private void KeyboardInput()
        {
            Horizontal = Input.GetAxis(GameStatics.Horizontal);
            Vertical = Input.GetAxis(GameStatics.Vertical);
        }
    }
}
