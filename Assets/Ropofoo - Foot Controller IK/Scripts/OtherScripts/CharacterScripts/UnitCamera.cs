using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ASD
{
    [RequireComponent(typeof(UnitInput))]
    public class UnitCamera : MonoBehaviour
    {
        public enum UpdateType { Update, FixedUpdate }

        private float m_XAngle;
        private float m_YAngle;

        private float m_CurrentXRotation;
        private float m_CurrentYRotation;

        private float m_XRotationVelocity;
        private float m_YRotationVelocity;

        private float m_XRotationVelocityLocal;
        private float m_YRotationVelocityLocal;

        [Header("Camera")]
        private Camera m_PlayerMainCamera;
        public Camera MainCamera => m_PlayerMainCamera;
        public UpdateType updateType = UpdateType.FixedUpdate;


        [Header("Camera Params")]
        [SerializeField] private float m_TiltMax = 45f;
        [SerializeField] private float m_TiltMin = 65f;

        [Header("Normal Position")]
        [SerializeField] private float m_CamHolderPosY = 2;
        [SerializeField] private float m_CamHolderPosZ = 1;

        [Header("Camera Follow")]
        [SerializeField] private float m_PivotSpeedFollow = 50;
        [SerializeField] private float m_FollowTime = .05f;

        [SerializeField] private float m_LookTurnSpeed = 5f;
        [SerializeField] private float m_SmoothDampTimeLook = .1f;

        private float xInputRotation;
        private float yInputRotation;

        private Vector3 m_RefCurrentVelocity;
        private GameObject m_Pivot;
        private GameObject m_ShakerNormal;
        private GameObject m_CameraHolder;

        [HideInInspector] public Transform pivotTransform;
        private Transform m_CameraShakerNormalTransform;
        private Transform m_CameraHolderTransform;
        private Transform m_PlayerCameraTransform;

        public Transform PlayerCameraTransform
        {
            get
            {
                return m_PlayerCameraTransform;
            }

            set
            {
                m_PlayerCameraTransform = value;
            }
        }
       
        private UnitInput m_UInput;

        private void OnEnable()
        {
            Initiation();
        }

        private void Initiation()
        {
            m_UInput = GetComponent<UnitInput>();
            m_PlayerMainCamera = FindObjectOfType<Camera>();
            m_PlayerMainCamera.gameObject.SetActive(true);

            if (!m_PlayerMainCamera.gameObject.GetComponent<GraphicRaycaster>())
                m_PlayerMainCamera.gameObject.AddComponent<GraphicRaycaster>();

            SetCamerastructure();
            SetCameraPosition();
        }

        private void Update()
        {
            xInputRotation = m_UInput.XRotation;
            yInputRotation = m_UInput.YRotation;

            if (updateType == UpdateType.Update)
            {
                Follow();
                HandleRotation();
            }
        }

        private void FixedUpdate()
        {
            if (updateType == UpdateType.FixedUpdate)
            {
                Follow();
                HandleRotation();
            }
        }
        private void SetCamerastructure()
        {
            m_Pivot = new GameObject(GameStatics.Pivot);
            m_ShakerNormal = new GameObject(GameStatics.ShakerNormal);
            m_CameraHolder = new GameObject(GameStatics.Holder);

            pivotTransform = m_Pivot.transform;
            m_CameraShakerNormalTransform = m_ShakerNormal.transform;
            m_CameraHolderTransform = m_CameraHolder.transform;
            PlayerCameraTransform = m_PlayerMainCamera.transform;

            m_CameraShakerNormalTransform.SetParent(pivotTransform);
            PlayerCameraTransform.SetParent(m_CameraHolderTransform);
            PlayerCameraTransform.localScale = Vector3.one;
        }
        private void SetCameraPosition()
        {
            pivotTransform.position = transform.position;
            m_CameraShakerNormalTransform.position = pivotTransform.position + pivotTransform.up * m_CamHolderPosY - pivotTransform.forward * m_CamHolderPosZ;
            m_CameraHolderTransform.position = m_CameraShakerNormalTransform.position;
            PlayerCameraTransform.position = m_CameraHolderTransform.position;

            pivotTransform.rotation = transform.rotation;
            m_CameraShakerNormalTransform.rotation = transform.rotation;
            m_CameraHolderTransform.rotation = transform.rotation;
            PlayerCameraTransform.rotation = transform.rotation;

            m_XAngle = pivotTransform.rotation.eulerAngles.y;
            m_YAngle = pivotTransform.rotation.eulerAngles.x;
        }
        private void Follow()
        {
            pivotTransform.position = Vector3.Lerp(pivotTransform.position, transform.position, m_PivotSpeedFollow * Time.deltaTime);
            m_CameraHolderTransform.position = Vector3.SmoothDamp(m_CameraHolderTransform.position, m_CameraShakerNormalTransform.position, ref m_RefCurrentVelocity, m_FollowTime);
        }

        private void HandleRotation()
        {
            if (m_SmoothDampTimeLook > 0)
            {
                m_CurrentXRotation = Mathf.SmoothDamp(m_CurrentXRotation, xInputRotation, ref m_XRotationVelocity, m_SmoothDampTimeLook);//сглаживание поворота
                m_CurrentYRotation = Mathf.SmoothDamp(m_CurrentYRotation, yInputRotation, ref m_YRotationVelocity, m_SmoothDampTimeLook);
            }
            else
            {
                m_CurrentXRotation = xInputRotation;
                m_CurrentYRotation = yInputRotation;
            }

            m_XAngle += m_CurrentXRotation * m_LookTurnSpeed;
            pivotTransform.rotation = Quaternion.Euler(0f, m_XAngle, 0);

            m_YAngle -= m_CurrentYRotation * m_LookTurnSpeed;
            m_YAngle = Mathf.Clamp(m_YAngle, -m_TiltMin, m_TiltMax);

            m_CameraHolderTransform.rotation = Quaternion.Euler(m_YAngle, m_XAngle, 0);
        }
    }
}