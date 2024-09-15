using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASD
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(FootControllerIK))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(UnitCamera))]
    public class UnitMovements : MonoBehaviour
    {
        [Header("Ignore Layers")]
        [SerializeField] private LayerMask ignoreLayers;
        [SerializeField] private float m_Mass = 5;
        [SerializeField] private float m_Drag = 5;
        [SerializeField] private float m_AngularDrag = 10;

        [Header("Unit parameters")]
        [SerializeField] private float m_UnitHeight = 1.7f;
        [SerializeField] private float m_UnitRadius = 0.4f;
        [SerializeField] private float m_MaxStepHeight = 0.45f;
        [SerializeField] private float m_LevitationOffset = 0.1f;

        [Header("Levitation")]
        [SerializeField] private float m_LevitationForce = 5.0f;
        [SerializeField] private float m_LevitationDamp = 0.4f;
        [SerializeField] private float m_ClampVelocityY = 0.5f;

        [Header("Turns Params")]
        [SerializeField] private float m_TurnSpeed = 5f;

        private Rigidbody m_RB;
        private UnitInput m_UInput;
        private UnitStateAnimator m_AnimatorState;
        private FootControllerIK m_FIK;
        private CapsuleCollider m_Capsule;
        private UnitCamera m_Camera;
        private Vector3 m_InputDirection;//m_UCamera.pivotTransform.forward
        private Vector3 m_MoveDirection;

        public bool OnGround { get; private set; }
        public bool isMoving { get; private set; }

        private void OnEnable()
        {
            if (!m_RB)
                m_RB = GetComponent<Rigidbody>();

            if (!m_FIK)
                m_FIK = GetComponent<FootControllerIK>();

            if (!m_Capsule)
                m_Capsule = GetComponent<CapsuleCollider>();

            if (!m_AnimatorState)
                m_AnimatorState = GetComponent<UnitStateAnimator>();

            if (!m_UInput)
                m_UInput = GetComponent<UnitInput>();

            if(!m_Camera)
                m_Camera = GetComponent<UnitCamera>();

            m_Capsule.radius = m_UnitRadius;

            if (m_MaxStepHeight >= m_UnitHeight) throw new Exception("Max Step Height cannot be greater then Unit Height");

            m_Capsule.height = m_UnitHeight - m_MaxStepHeight;

            if (m_Capsule.height > m_UnitRadius * 2) m_Capsule.center = new Vector3(0, m_MaxStepHeight + m_Capsule.height / 2, 0);
            else m_Capsule.center = new Vector3(0, m_UnitHeight - m_UnitRadius, 0);

            m_FIK.MaxStepHeight = m_MaxStepHeight;

            m_RB.mass = m_Mass;
            m_RB.angularDrag = m_AngularDrag;
            m_RB.drag = m_Drag;
            m_RB.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;

            m_MoveDirection = Vector3.zero;
        }
        private void FixedUpdate()
        {
            Levitation();

            UpdateMoveDirection();

            UnitTurn();
            UnitAnimatorMove();
        }
        private void OnAnimatorMove()
        {
            //To activate root motion
        }
        private void UpdateMoveDirection()
        {
            if (m_UInput.Horizontal == 0 && m_UInput.Vertical == 0)
            {
                m_InputDirection = Vector3.zero;
                m_MoveDirection = Vector3.zero;
            }
            else
            {
                m_InputDirection.x = m_UInput.Horizontal;
                m_InputDirection.z = m_UInput.Vertical;

                if (m_InputDirection.magnitude > 1)
                    m_InputDirection = m_InputDirection.normalized;

                m_MoveDirection = m_Camera.pivotTransform.forward * m_InputDirection.z + m_Camera.pivotTransform.right * m_InputDirection.x;
            }
            
            if (m_MoveDirection != Vector3.zero) isMoving = true;
            else isMoving = false;
        }

        private void Levitation()
        {
            bool hovering;
            float hitDistance = 0;

            Vector3 origin = transform.position + transform.up * (m_MaxStepHeight + 0.05f);

            float rigidbodyVelocityY = Vector3.Dot(m_RB.velocity, Physics.gravity);

            if (rigidbodyVelocityY < m_ClampVelocityY)
                OnGround = true;
            else
                OnGround = false;

            if (Physics.Raycast(origin, -transform.up, out RaycastHit hit, m_MaxStepHeight + m_LevitationOffset + 0.05f, ~ignoreLayers))
            {
                hovering = true;
                hitDistance = hit.distance;
            }
            else if (Physics.SphereCast(origin, m_UnitRadius / 2, -Vector3.up, out hit, m_MaxStepHeight + m_LevitationOffset + 0.05f, ~ignoreLayers))
            {
                hovering = true;

                hit.point = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                hitDistance = Vector3.Distance(origin, hit.point);
            }
            else
            {
                hovering = false;
                OnGround = false;
            }

            if (m_FIK && m_FIK.CanReachTargets && hovering)
            {
                if(isMoving)
                    hit.point = new Vector3(hit.point.x, m_FIK.DirectionalFootHeight(m_MoveDirection).y, hit.point.z);
                else
                    hit.point = new Vector3(hit.point.x, m_FIK.LowestFootHeight, hit.point.z);

                hitDistance = Vector3.Distance(origin, hit.point);
            }

            if (hovering)
            {
                float hoverError = m_MaxStepHeight + m_LevitationOffset - hitDistance;

                if (hoverError > 0)
                {
                    float upwardSpeed = m_RB.velocity.y;
                    float lift = hoverError * m_LevitationForce - upwardSpeed * m_LevitationDamp;
                    m_RB.AddForce(lift * transform.up, ForceMode.VelocityChange);
                }
            }

            if (OnGround == false) m_RB.drag = 0;
            else m_RB.drag = m_Drag;
        }
        private void UnitAnimatorMove()
        {
            if (OnGround)
            {
                float z = Vector3.Dot(m_MoveDirection, transform.forward);
                float x = Vector3.Dot(m_MoveDirection, transform.right);

                m_AnimatorState.VectorToAnimator = new Vector2(x, z);

                Vector3 vel = m_AnimatorState.Animator.velocity;
                vel.y = m_RB.velocity.y;
                m_RB.velocity = vel;
            }
        }
        private void UnitTurn()
        {
            if (isMoving && OnGround)
                m_RB.rotation =  Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(m_MoveDirection), m_TurnSpeed * Time.deltaTime);
        }
        public float AngleSigned(Vector3 v1, Vector3 v2, Vector3 normal)
        {
            return Mathf.Atan2(Vector3.Dot(normal, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }
    }
}
