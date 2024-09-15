using ASD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASD
{
    public class UnitStateAnimator : MonoBehaviour
    {
        public enum CharStates { idle, moving, action, onAir }

        [Header("State")]
        public CharStates CurrentState = CharStates.idle;
        public bool OnAction { get; private set; }

        private Vector2 m_MovementVectorToAnimator;
        private Animator m_Animator;

        private float m_AnimationDampTime;
        private UnitMovements m_UMovements;

        [Header("Foot")]
        public Transform frontFoot;
        public Transform upFoot;
        private Transform m_LeftFoot;
        private Transform m_RightFoot;

        private bool m_OnGround;
        private bool m_Moving;

        public Vector2 VectorToAnimator
        {
            get
            {
                return m_MovementVectorToAnimator;
            }

            set
            {
                m_MovementVectorToAnimator = value;
            }
        }
        public Animator Animator
        {
            get
            {
                return m_Animator;
            }

            set
            {
                m_Animator = value;
            }
        }

        private void OnEnable()
        {
            Initiation();
        }

        private void Update()
        {
            UnitMovementsLink();
            UnitStateUpdate();
            UnitAnimate();
            FootCheck();
        }
        private void Initiation()
        {
            if (!m_Animator)
            {
                m_Animator = GetComponent<Animator>();


                m_LeftFoot = m_Animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                m_RightFoot = m_Animator.GetBoneTransform(HumanBodyBones.RightFoot);
            }

            if (!m_UMovements)
                m_UMovements = GetComponent<UnitMovements>();
        }

        private void FootCheck()
        {
            Vector3 left = transform.InverseTransformVector(m_LeftFoot.position);
            Vector3 right = transform.InverseTransformVector(m_RightFoot.position);

            float leftZ = left.z;
            float rightZ = right.z;

            float leftY = left.y;
            float rightY = right.y;

            if (leftY > rightY)
                frontFoot = m_LeftFoot;
            else
                frontFoot = m_RightFoot;

            if (leftZ > rightZ)
                upFoot = m_LeftFoot;
            else
                upFoot = m_RightFoot;
        }
        private void UnitMovementsLink()
        {
            m_Moving = m_UMovements.isMoving;
            m_OnGround = m_UMovements.OnGround;
        }
        private void UnitStateUpdate()
        {
            if (m_OnGround)
            {
                if (OnAction)
                {
                    CurrentState = CharStates.action;
                    return;
                }

                if (m_Moving)
                {
                    CurrentState = CharStates.moving;
                }
                else
                {
                    CurrentState = CharStates.idle;
                }
            }
            else
            {
                CurrentState = CharStates.onAir;
            }
        }
        private void UnitAnimate()
        {
            switch (CurrentState)
            {
                case (CharStates.idle):
                    m_AnimationDampTime = 0.5f;
                    break;
                case (CharStates.moving):
                    m_AnimationDampTime = 0.2f;
                    break;
                case (CharStates.action):
                    m_AnimationDampTime = 1f;
                    break;

                case (CharStates.onAir):
                    m_AnimationDampTime = 0.05f;
                    break;

                default:
                    m_AnimationDampTime = 0.05f;
                    break;
            }

            m_Animator.SetFloat(GameStatics.Sideways, m_MovementVectorToAnimator.x, m_AnimationDampTime, Time.deltaTime);
            m_Animator.SetFloat(GameStatics.Forward, m_MovementVectorToAnimator.y, m_AnimationDampTime, Time.deltaTime);

        }
        public void UnitAction(string clipName)
        {
            m_Animator.CrossFade(clipName, 0.06f);
        }
    } 
}
