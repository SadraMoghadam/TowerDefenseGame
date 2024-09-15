using System;
using UnityEngine;

namespace ASD
{
    public class FootControllerIK : MonoBehaviour
    {
        public enum CastType { Ray, Sphere, RayAndSphere }
        public enum RotationType { RawTarget, AddTarget, Direction, Animator }

        private const float MAX_DISTANCE_POWER = 2;
        private const float MAX_SMOOTHING_ANGLE = 3;
        private const float SMOOTHING_POWER = 1f;
        private const float MAX_STEP_HEIGHT = 2;
        private const float HEIGHT_OFFSET = 0.5f;

        private Animator m_Anim;
        private Limb m_LeftLeg;
        private Limb m_RightLeg;
        private bool m_IsInitialized = false;
        private float m_MeshHeightOffset;

        [Header("Main setting")]
        
        public LayerMask ignoreLayers;
        /// <summary>
        /// Setting "true" will disable component. Invoke FIKA.FootIK() in outside LateUpdate to run component.
        /// </summary>
        public bool outsideUpdate = false;
        /// <summary>
        /// Setting "true" will enable addition raycast from toes' bones for better foot positioning. Toes' bones are required for proper work. 
        /// </summary>
        [Space(5)] public bool increasedAccuracy = false;
        /// <summary>
        /// Use to avoid unnatural knees bending. 
        /// </summary>
        public bool fixKnee = false;
        /// <summary>
        /// Use to avoid wide angles of foot rotation on surface.
        /// </summary>
        public bool footConstraint = false;
        private float m_Incline = 0.85f;
        private float m_InclineRadian;
        
        [Space(5)] [Range(0, MAX_STEP_HEIGHT)] [SerializeField] private float m_MaxStepHeight = 0.3f;
        /// <summary>
        /// Max height on which character can step up.
        /// </summary>
        public float MaxStepHeight { get => m_MaxStepHeight; set { m_MaxStepHeight = Mathf.Clamp(value, 0, MAX_STEP_HEIGHT); } }
        /// <summary>
        /// Offset of foot position on surface.
        /// </summary>
        [Range(-HEIGHT_OFFSET, HEIGHT_OFFSET)] [SerializeField] private float m_FootHeightOffset = 0;
        public float FootHeightOffset { get => m_FootHeightOffset; set { m_FootHeightOffset = Mathf.Clamp(value, -HEIGHT_OFFSET, HEIGHT_OFFSET); } }

        [Header("Legs switching-on")]
        public bool leftEnabled = true;
        public bool rightEnabled = true;

        [Header("Movements Smooth")] 
        [Range(0, MAX_DISTANCE_POWER)] [SerializeField] private float m_DistancePower = 1f;
        /// <summary>
        /// Distance of current movement used for foot position smoothing. Zero value will disable smoothing.
        /// </summary>
        public float DistancePower { get => m_DistancePower; set { m_DistancePower = Mathf.Clamp(value, 0, MAX_DISTANCE_POWER); } }
        /// <summary>
        /// Angle used for foot rotation smoothing. Zero value disables smoothing.
        /// </summary>
        [Range(0, MAX_SMOOTHING_ANGLE)] [SerializeField] private float m_SmoothingAngle = 2f;
        public float SmoothingAngle { get => m_SmoothingAngle; set { m_SmoothingAngle = Mathf.Clamp(value, 0, MAX_SMOOTHING_ANGLE); } }

        [Range(0, SMOOTHING_POWER)] [SerializeField] private float m_GlobalSmoothingPower = 0f;
        /// <summary>
        /// Global smoothing power. Zero value disables smoothing.
        /// </summary>
        public float GlobalSmoothingPower { get => m_GlobalSmoothingPower; set { m_GlobalSmoothingPower = Mathf.Clamp(value, 0, SMOOTHING_POWER); } }
        private float m_MinimalSmoothDistance = 0.005f;

        [Header("Casts")]
        /// <summary>
        /// Current type of the cast which is used to check surface under the character.
        /// </summary>
        public CastType type = CastType.Ray;
        /// <summary>
        /// Radius for Spherecast.
        /// </summary>
        [Range(0.01f, 0.3f)] public float sphereRadius = 0.03f;

        [Header("Additional Targets")]
        /// <summary>
        /// Current type of foot rotation. Target is required for proper work.
        /// </summary>
        [Space(5)] public RotationType rotationType = RotationType.RawTarget;

        /// <summary>
        /// Target for left foot inverse kinematics.
        /// </summary>
        [Space(5)] public Transform leftTarget;
        /// <summary>
        /// Target for left knee inverse kinematics. Ñreates new plane for left leg rotation.
        /// </summary>
        public Transform leftKneeTarget;
        /// <summary>
        /// Target for right foot inverse kinematics.
        /// </summary>
        [Space(5)] public Transform rightTarget;
        /// <summary>
        /// Target for right knee inverse kinematics. Ñreates new plane for right leg rotation.
        /// </summary>
        public Transform rightKneeTarget;       
        
        /// <summary>
        /// If there is an accessible surface under the character.
        /// </summary>
        public bool CanReachTargets
        {
            get {
                if (m_LeftLeg != null && m_RightLeg != null)
                    return m_LeftLeg.canReachTarget && m_RightLeg.canReachTarget;
                else
                    return false;
            }
        }
        /// <summary>
        /// Returns the lowest foot position.y. Sets character tranform.position.y for better positioning. 
        /// </summary>
        public float LowestFootHeight
        {
            get
            {
                if (m_LeftLeg.LowestHitPoint.y < m_RightLeg.LowestHitPoint.y) return m_LeftLeg.LowestHitPoint.y;
                else return m_RightLeg.LowestHitPoint.y;
            }
        }
        /// <summary>
        /// Returns the lowest foot position. Sets character tranform.position.y for better positioning.
        /// </summary>
        public Vector3 LowestFootPosition
        {
            get
            {
                if (m_LeftLeg.LowestHitPoint.y < m_RightLeg.LowestHitPoint.y) return m_LeftLeg.LowestHitPoint;
                else return m_RightLeg.LowestHitPoint;
            }
        }
        /// <summary>
        /// Returns the lowest foot position, takes into account the character movement. Set character tranform.position.y for better positioning.
        /// </summary>
        /// <param name="moveDirection">Global vector of character movement direction.</param>
        /// <returns></returns>
        public Vector3 DirectionalFootHeight(Vector3 moveDirection)
        {
            float left = Vector3.Dot(m_LeftLeg.LowBonePosition - transform.position, moveDirection);
            float right = Vector3.Dot(m_RightLeg.LowBonePosition - transform.position, moveDirection);

            if(left > right)
                return m_LeftLeg.LowestHitPoint;
            else
                return m_RightLeg.LowestHitPoint;
        }

        private void OnEnable()
        {
            Initiation();
        }
        private void LateUpdate()
        {
            if (outsideUpdate || !m_Anim.enabled) return;

            if (m_Anim.updateMode != AnimatorUpdateMode.AnimatePhysics)
                FootIK();
            else
            {
                RotationSynchronization(m_LeftLeg);
                RotationSynchronization(m_RightLeg);
            }
        }

        /// <summary>
        /// Works only with AnimatorUpdateMode.AnimatePhysics and enabled the IK Pass checkbox in the Layer settings in Animator window.
        /// </summary>
        /// <param name="layerIndex"></param>
        private void OnAnimatorIK(int layerIndex)
        {
           if (m_Anim.updateMode == AnimatorUpdateMode.AnimatePhysics)
                FootIK();
        }
        /// <summary>
        /// Initiates the component.
        /// </summary>
        private void Initiation()
        {
            if (m_IsInitialized) return;

            m_Anim = GetComponent<Animator>();
            if (!m_Anim.avatar) throw new Exception("No Avatar to use");
            if (!m_Anim.isHuman) throw new Exception("Not Humanoid Avatar");
            if (!m_Anim.runtimeAnimatorController) throw new Exception("No Animator Controller");

            if (AnimatorBone(HumanBodyBones.LeftToes) && AnimatorBone(HumanBodyBones.RightToes))
            {
                m_LeftLeg = new Limb(AnimatorBone(HumanBodyBones.LeftUpperLeg), AnimatorBone(HumanBodyBones.LeftLowerLeg), AnimatorBone(HumanBodyBones.LeftFoot), AnimatorBone(HumanBodyBones.LeftToes));
                m_RightLeg = new Limb(AnimatorBone(HumanBodyBones.RightUpperLeg), AnimatorBone(HumanBodyBones.RightLowerLeg), AnimatorBone(HumanBodyBones.RightFoot), AnimatorBone(HumanBodyBones.RightToes));

                float leftHeelToeDistance = Vector3.Distance(AnimatorBone(HumanBodyBones.LeftFoot).position, AnimatorBone(HumanBodyBones.LeftLowerLeg).position);
                float rightHeelToeDistance = Vector3.Distance(AnimatorBone(HumanBodyBones.RightFoot).position, AnimatorBone(HumanBodyBones.RightLowerLeg).position);

                if (leftHeelToeDistance <= sphereRadius || rightHeelToeDistance <= sphereRadius)
                {
                    if (leftHeelToeDistance > rightHeelToeDistance)
                        sphereRadius = rightHeelToeDistance / 3f;
                    else
                        sphereRadius = leftHeelToeDistance / 3f;

                    if (sphereRadius <= 0.01)
                        sphereRadius = 0.01f;
                }
            }
            else
            {
                m_LeftLeg = new Limb(AnimatorBone(HumanBodyBones.LeftUpperLeg), AnimatorBone(HumanBodyBones.LeftLowerLeg), AnimatorBone(HumanBodyBones.LeftFoot));
                m_RightLeg = new Limb(AnimatorBone(HumanBodyBones.RightUpperLeg), AnimatorBone(HumanBodyBones.RightLowerLeg), AnimatorBone(HumanBodyBones.RightFoot));
            }

            SkinnedMeshRenderer[] meshsRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            Vector3 lowestPoint = transform.InverseTransformPoint( (AnimatorBone(HumanBodyBones.LeftFoot).position + AnimatorBone(HumanBodyBones.RightFoot).position) / 2);

            for (int i = 0; i < meshsRenderers.Length; i++)
            {
                Vector3[] vertices = meshsRenderers[i].sharedMesh.vertices;
                for (int j = 0; j < vertices.Length; j++)
                {
                    if (vertices[j].y < lowestPoint.y)
                        lowestPoint.y = vertices[j].y;
                }
            }

            lowestPoint = transform.TransformPoint(lowestPoint);

            m_MeshHeightOffset = Vector3.Dot(lowestPoint - transform.position, transform.up);
            m_LeftLeg.distanceFromMesh = Vector3.Dot(m_LeftLeg.LowBonePosition - transform.position, transform.up) - m_MeshHeightOffset;
            m_RightLeg.distanceFromMesh = Vector3.Dot(m_RightLeg.LowBonePosition - transform.position, transform.up) - m_MeshHeightOffset;

            m_InclineRadian = Mathf.Acos(m_Incline);// Max incline in radian from vector up;
            m_IsInitialized = true;
        }
        /// <summary>
        /// Calculates legs' position. Can be used from another componet with "true" outsideUpdate parameter.
        /// </summary>
        public void FootIK()
        {
            if (leftEnabled)
            {
                if (leftTarget)
                {
                    SetPositionRotationFromTarget(m_LeftLeg, leftTarget);
                    Smoothing(m_LeftLeg);
                    FootsPlacement(m_LeftLeg, leftTarget, leftKneeTarget);
                }
                else
                {
                    SetPositionRotationFromRayCast(m_LeftLeg);
                    Smoothing(m_LeftLeg);
                    FootsPlacement(m_LeftLeg, leftKneeTarget);
                }
            }

            if (rightEnabled)
            {
                if (rightTarget)
                {
                    SetPositionRotationFromTarget(m_RightLeg, rightTarget);
                    Smoothing(m_RightLeg);
                    FootsPlacement(m_RightLeg, rightTarget, rightKneeTarget);
                }
                else
                {
                    SetPositionRotationFromRayCast(m_RightLeg);
                    Smoothing(m_RightLeg);
                    FootsPlacement(m_RightLeg, rightKneeTarget);
                }
            }

            GlobalSmoothing(m_LeftLeg);
            GlobalSmoothing(m_RightLeg);

            SavingPositionRotation(m_LeftLeg);
            SavingPositionRotation(m_RightLeg);
        }
        #region IK voids
        private void SetPositionRotationFromTarget(Limb limb, Transform target)
        {
            float footHeight = limb.distanceFromMesh - m_FootHeightOffset;
            Vector3 footDistance = (limb.UpBone.position - target.position).normalized * footHeight;

            limb.targetPosition = target.position + footDistance;

            if (rotationType == RotationType.RawTarget)
            {
                limb.targetRotation = target.rotation;
            }
            else if (rotationType == RotationType.AddTarget)
            {
                limb.targetRotation = target.rotation * Quaternion.Inverse(limb.LowBoneRotation);
            }
            else if (rotationType == RotationType.Direction)
            {
                limb.targetRotation = Quaternion.FromToRotation(transform.up, limb.UpBone.position - limb.targetPosition) * limb.LowBoneRotation;
            }
            else
                limb.targetRotation = limb.LowBone.rotation;

            limb.canReachTarget = ((Vector3.Distance(limb.UpBone.position, limb.targetPosition) <= (limb.UpperLength + limb.LowerLength + footHeight)));
            limb.LowestHitPoint = target.position;
        }
        /// <summary>
        /// Set the target position and rotation
        /// </summary>
        /// <param name="limb">Character limb.</param>
        private void SetPositionRotationFromRayCast(Limb limb)
        {
            bool low;
            RaycastHit hit;
            float currentHeight = Vector3.Dot(transform.up, limb.LowBonePosition - transform.position);
            limb.AnimatorWeight = (Vector3.Dot(transform.up, limb.LowBonePosition - transform.position) - limb.distanceFromMesh - m_MeshHeightOffset);

            if (type == CastType.Ray)
                low = Physics.Raycast(limb.LowBonePosition + transform.up * (m_MaxStepHeight - currentHeight), -transform.up, out hit, m_MaxStepHeight * 2, ~ignoreLayers);
            else if (type == CastType.Sphere)
                low = Physics.SphereCast(limb.LowBonePosition + transform.up * (m_MaxStepHeight - currentHeight), sphereRadius, -transform.up, out hit, m_MaxStepHeight * 2, ~ignoreLayers);
            else
            {
                low = Physics.Raycast(limb.LowBonePosition + transform.up * (m_MaxStepHeight - currentHeight), -transform.up, out hit, m_MaxStepHeight * 2, ~ignoreLayers);
                if (!low)
                    low = Physics.SphereCast(limb.LowBonePosition + transform.up * (m_MaxStepHeight - currentHeight), sphereRadius, -transform.up, out hit, m_MaxStepHeight * 2, ~ignoreLayers);
            }

            if (increasedAccuracy && limb.ExtraBone)
            {
                bool extra;
                RaycastHit extraHit;

                currentHeight = Vector3.Dot(transform.up, limb.ExtraBone.position - transform.position);

                if (type == CastType.Ray)
                    extra = Physics.Raycast(limb.ExtraBone.position + transform.up * (m_MaxStepHeight - currentHeight), -transform.up, out extraHit, m_MaxStepHeight * 2, ~ignoreLayers);
                else if (type == CastType.Sphere)
                    extra = Physics.SphereCast(limb.ExtraBone.position + transform.up * (m_MaxStepHeight - currentHeight), sphereRadius, -transform.up, out extraHit, m_MaxStepHeight * 2, ~ignoreLayers);
                else
                {
                    extra = Physics.Raycast(limb.ExtraBone.position + transform.up * (m_MaxStepHeight - currentHeight), -transform.up, out extraHit, m_MaxStepHeight * 2, ~ignoreLayers);
                    if (!extra)
                        extra = Physics.SphereCast(limb.ExtraBone.position + transform.up * (m_MaxStepHeight - currentHeight), sphereRadius, -transform.up, out extraHit, m_MaxStepHeight * 2, ~ignoreLayers);
                }

                if (low && extra)
                {
                    Vector3 localLow = transform.InverseTransformPoint(hit.point);
                    Vector3 localExtra = transform.InverseTransformPoint(extraHit.point);

                    if (localLow.y > localExtra.y)
                    {
                        SetFootFromLow(limb, hit, extraHit);
                        limb.LowestHitPoint = extraHit.point;
                    }
                    else
                    {
                        SetFootFromExtra(limb, extraHit, hit);
                        limb.LowestHitPoint = hit.point;
                    }

                }
                else if (low && !extra)
                {
                    SetFootFromLow(limb, hit);
                    limb.LowestHitPoint = hit.point;
                }
                else if (!low && extra)
                {
                    SetFootFromExtra(limb, extraHit);
                    limb.LowestHitPoint = extraHit.point;
                }
                else
                    limb.canReachTarget = false;
            }
            else
            {
                if (low)
                {
                    SetFootFromLow(limb, hit);
                    limb.LowestHitPoint = hit.point;
                }
                else
                    limb.canReachTarget = false;
            }
        }
        /// <summary>
        /// Use low (foot) bone to find the target position
        /// </summary>
        /// <param name="limb">Character limb.</param>
        /// <param name="lowHit">Hit point.</param>
        private void SetFootFromLow(Limb limb, RaycastHit lowHit)
        {
            if (footConstraint)
                lowHit.normal = ConstraintedNormal(lowHit.normal);

            Vector3 animatorHeight = transform.up * (Vector3.Dot(transform.up, limb.LowBonePosition - transform.position) - limb.distanceFromMesh - m_MeshHeightOffset);
            Vector3 footHeight = lowHit.normal * (limb.distanceFromMesh - m_FootHeightOffset);

            limb.targetPosition = lowHit.point + footHeight + animatorHeight; //Add height as in animation clip and height offsets to hit point
            limb.targetRotation = Quaternion.FromToRotation(transform.up, lowHit.normal) * limb.LowBoneRotation;

            limb.canReachTarget = ((Vector3.Distance(limb.UpBone.position, limb.targetPosition) <= (limb.UpperLength + limb.LowerLength + m_MaxStepHeight)));
        }
        /// <summary>
        /// Use low (foot) bone to find the target position
        /// </summary>
        /// <param name="limb">Character limb.</param>
        /// <param name="lowHit">Hit point.</param>
        private void SetFootFromLow(Limb limb, RaycastHit lowHit, RaycastHit extraHit)
        {
            lowHit.normal = (lowHit.normal + extraHit.normal) / 2;

            if (footConstraint)
                lowHit.normal = ConstraintedNormal(lowHit.normal);

            Vector3 animatorHeight = transform.up * (Vector3.Dot(transform.up, limb.LowBonePosition - transform.position) - limb.distanceFromMesh - m_MeshHeightOffset);
            Vector3 footHeight = lowHit.normal * (limb.distanceFromMesh - m_FootHeightOffset);

            limb.targetPosition = lowHit.point + footHeight + animatorHeight; //Add height as in animation clip and height offsets to hit point
            limb.targetRotation = Quaternion.FromToRotation(transform.up, lowHit.normal) * limb.LowBoneRotation;

            limb.canReachTarget = ((Vector3.Distance(limb.UpBone.position, limb.targetPosition) <= (limb.UpperLength + limb.LowerLength + m_MaxStepHeight)));
        }
        /// <summary>
        /// Uses extra (toe) bone to find the target position.
        /// Requires transform.up equals Vector3.up
        /// </summary>
        /// <param name="limb">Character limb.</param>
        /// <param name="extraHit">Hit point.</param>
        private void SetFootFromExtra(Limb limb, RaycastHit extraHit)
        {
            if (footConstraint)
                extraHit.normal = ConstraintedNormal(extraHit.normal);

            Quaternion fromUpToNormal = Quaternion.FromToRotation(transform.up, extraHit.normal);

            Vector3 footDirection = (limb.ExtraBone.position - limb.LowBone.position);

            float forward = Vector3.Dot(footDirection, transform.forward);
            float right = Vector3.Dot(footDirection, transform.right);
            footDirection = transform.rotation * new Vector3(right, 0, forward);

            Vector3 directioOnGround = fromUpToNormal * footDirection;

            Vector3 animatorHeight = transform.up * (Vector3.Dot(transform.up, limb.LowBonePosition - transform.position) - limb.distanceFromMesh - m_MeshHeightOffset);
            Vector3 footHeight = extraHit.normal * (limb.distanceFromMesh - m_FootHeightOffset);

            limb.targetPosition = extraHit.point - directioOnGround + footHeight + animatorHeight; //Add height as in animation clip and height offsets to hit point
            limb.targetRotation = Quaternion.FromToRotation(transform.up, extraHit.normal) * limb.LowBoneRotation;

            limb.canReachTarget = ((Vector3.Distance(limb.UpBone.position, limb.targetPosition) <= (limb.UpperLength + limb.LowerLength + m_MaxStepHeight)));
        }
        /// <summary>
        /// Uses extra (toe) bone to find the target position
        /// </summary>
        /// <param name="limb">Character limb.</param>
        /// <param name="extraHit">Hit point.</param>
        private void SetFootFromExtra(Limb limb, RaycastHit extraHit, RaycastHit lowHit)
        {
            extraHit.normal = (extraHit.normal + lowHit.normal) / 2;

            if (footConstraint)
                extraHit.normal = ConstraintedNormal(extraHit.normal);

            Quaternion fromUpToNormal = Quaternion.FromToRotation(transform.up, extraHit.normal);

            Vector3 footDirection = (limb.ExtraBone.position - limb.LowBone.position);

            float forward = Vector3.Dot(footDirection, transform.forward);
            float right = Vector3.Dot(footDirection, transform.right);
            footDirection = transform.rotation * new Vector3(right, 0, forward);

            Vector3 directioOnGround = fromUpToNormal * footDirection;

            Vector3 animatorHeight = transform.up * (Vector3.Dot(transform.up, limb.LowBonePosition - transform.position) - limb.distanceFromMesh - m_MeshHeightOffset);
            Vector3 footHeight = extraHit.normal * (limb.distanceFromMesh - m_FootHeightOffset);

            limb.targetPosition = extraHit.point - directioOnGround + footHeight + animatorHeight; //Add height as in animation clip and height offsets to hit point
            limb.targetRotation = Quaternion.FromToRotation(transform.up, extraHit.normal) * limb.LowBoneRotation;

            limb.canReachTarget = ((Vector3.Distance(limb.UpBone.position, limb.targetPosition) <= (limb.UpperLength + limb.LowerLength + m_MaxStepHeight)));
        }
        /// <summary>
        /// Places foots using Cosine Rule
        /// </summary>
        /// <param name="limb">Character limb.</param>
        /// <param name="pole">Target to calculate new plane.</param>
        private void FootsPlacement(Limb limb, Transform pole)
        {
            if (limb.canReachTarget)
            {
                Vector3 planeThirdPoint;
                float targetDistance = Mathf.Min((limb.targetPosition - limb.UpBone.position).magnitude, limb.LowerLength + limb.UpperLength - .001f);

                if (pole)
                {
                    planeThirdPoint = pole.position;

                    Vector3 targetDirection = limb.targetPosition - limb.UpBone.position;
                    Vector3 poleDirection = planeThirdPoint - limb.UpBone.position;

                    Vector3 UpperLeg = limb.MiddleBone.position - limb.UpBone.position;
                    Vector3 cross = Vector3.Cross(targetDirection, poleDirection);
                    cross.Normalize();
                    float targetAngel = limb.UpperBoneAngle(targetDistance);
                    Vector3 newUpperRotation = Quaternion.AngleAxis(targetAngel, cross) * targetDirection;
                    limb.UpBone.rotation = Quaternion.FromToRotation(UpperLeg, newUpperRotation) * limb.UpBone.rotation;

                    Vector3 LowerLeg = limb.LowBone.position - limb.MiddleBone.position;
                    limb.MiddleBone.rotation = Quaternion.FromToRotation(LowerLeg, limb.targetPosition - limb.MiddleBone.position) * limb.MiddleBone.rotation;
                }
                else
                {
                    if (!fixKnee)
                    {
                        planeThirdPoint = (limb.MiddleBone.position + limb.LowBone.position) / 2 + transform.forward * 0.05f;// or limb.MiddleBone.position
                    }
                    else
                    {
                        Vector3 targetVector = limb.MiddleBone.position - limb.UpBone.position;
                        targetVector.Normalize();

                        float ang = Vector3.Angle(transform.right, targetVector);
                        Vector3 cross = Quaternion.AngleAxis(-ang, transform.up) * transform.forward;
                        cross.Normalize();

                        targetVector = Quaternion.AngleAxis(90, cross) * targetVector;
                        planeThirdPoint = limb.MiddleBone.position + targetVector * limb.Length;
                    }

                    float targetAngle = limb.MiddleBoneAngle(targetDistance);
                    float angle = Vector3.Angle(limb.LowBonePosition - limb.MiddleBone.position, limb.UpBone.position - limb.MiddleBone.position); // Angle between bones in animator
                    Vector3 axis = Vector3.Cross(limb.LowBonePosition - planeThirdPoint, limb.UpBone.position - planeThirdPoint);// Rotation of angle around this axis
                    axis.Normalize();

                    //limb.MiddleBone.RotateAround(limb.MiddleBone.position, axis, angle - targetAngle);// Add rotation to middle bone, (same below)

                    limb.MiddleBone.rotation = Quaternion.AngleAxis(angle - targetAngle, axis) * limb.MiddleBone.rotation;// Add rotation to middle bone
                    limb.UpBone.rotation = Quaternion.FromToRotation(limb.LowBonePosition - limb.UpBone.position, limb.targetPosition - limb.UpBone.position) * limb.UpBone.rotation;// Add rotation to upper bone
                }

                limb.LowBoneRotation = limb.targetRotation;
            }
        }
        /// <summary>
        /// Places foots using Cosine Rule
        /// </summary>
        /// <param name="limb">Character limb.</param>
        /// <param name="target">Target to calculate limb rotation.</param>
        /// <param name="pole">Target to calculate new plane.</param>
        private void FootsPlacement(Limb limb, Transform target, Transform pole)
        {
            Vector3 planeThirdPoint;

            if (pole)
                planeThirdPoint = pole.position;
            else
            {
                if (!fixKnee)
                    planeThirdPoint = limb.MiddleBone.position;
                else
                {
                    Vector3 targetVector = limb.targetPosition - limb.UpBone.position;

                    targetVector.Normalize();

                    planeThirdPoint = limb.UpBone.position + targetVector * limb.UpperLength;

                    float ang = Vector3.Angle(transform.right, targetVector);
                    Vector3 fromGlobalToLocaltarget = transform.InverseTransformPoint(target.position);
                    Vector3 fromGlobalToLacalUp = transform.InverseTransformPoint(limb.UpBone.position);

                    if (limb == m_RightLeg && fromGlobalToLocaltarget.x <= fromGlobalToLacalUp.x && fromGlobalToLocaltarget.z <= fromGlobalToLacalUp.z)
                        ang = 180 - ang;

                    if (limb == m_LeftLeg && fromGlobalToLocaltarget.x >= fromGlobalToLacalUp.x && fromGlobalToLocaltarget.z <= fromGlobalToLacalUp.z)
                        ang = 180 - ang;

                    Vector3 cross = Quaternion.AngleAxis(-ang, transform.up) * transform.forward;
                    cross.Normalize();

                    targetVector = Quaternion.AngleAxis(90, cross) * targetVector;//transform.right
                    planeThirdPoint += targetVector * limb.Length;
                }
            }

            Vector3 targetDirection = limb.targetPosition - limb.UpBone.position;
            float targetDistance = targetDirection.magnitude;
            Vector3 poleDirection = planeThirdPoint - limb.UpBone.position;

            if (targetDistance >= limb.UpperLength + limb.LowerLength)
            {
                targetDirection.Normalize();
                Quaternion targetRotation;

                Vector3 UpperLeg = limb.MiddleBone.position - limb.UpBone.position;
                targetRotation = Quaternion.FromToRotation(UpperLeg, targetDirection);
                limb.UpBone.rotation = targetRotation * limb.UpBone.rotation;


                Vector3 LowerLeg = limb.LowBone.position - limb.MiddleBone.position;
                targetRotation = Quaternion.FromToRotation(LowerLeg, targetDirection);
                limb.MiddleBone.rotation = targetRotation * limb.MiddleBone.rotation;
            }
            else
            {
                targetDirection.Normalize();
                poleDirection.Normalize();
                Quaternion targetRotation;

                Vector3 UpperLeg = limb.MiddleBone.position - limb.UpBone.position;

                Vector3 cross = Vector3.Cross(targetDirection, poleDirection);
                cross.Normalize();

                float targetAngel = limb.UpperBoneAngle(targetDistance);
                Vector3 newUpperRotation = Quaternion.AngleAxis(targetAngel, cross) * targetDirection;
                targetRotation = Quaternion.FromToRotation(UpperLeg, newUpperRotation);
                limb.UpBone.rotation = targetRotation * limb.UpBone.rotation;

                Vector3 LowerLeg = limb.LowBone.position - limb.MiddleBone.position;
                targetRotation = Quaternion.FromToRotation(LowerLeg, target.position - limb.MiddleBone.position);
                limb.MiddleBone.rotation = targetRotation * limb.MiddleBone.rotation;
            }

            limb.LowBoneRotation = limb.targetRotation;
        }
        #endregion
        /// <summary>
        /// Smoothing legs movements.
        /// </summary>
        /// <param name="limb">Character limb.</param>
        private void Smoothing(Limb limb)
        {
            if (limb.canReachTarget && m_DistancePower > 0)
            {
                float animatededDistance = Vector3.Distance(limb.lastLowBoneAnimationPosition, limb.LowBonePosition);
                float movementDistance = Vector3.Distance(limb.lastLowBonePosition, limb.targetPosition);

                if (animatededDistance < m_MinimalSmoothDistance)
                    animatededDistance = m_MinimalSmoothDistance;

                limb.targetPosition = Vector3.Lerp(limb.lastLowBonePosition, limb.targetPosition, (animatededDistance * (MAX_DISTANCE_POWER - m_DistancePower)) / movementDistance);
            }

            if (limb.canReachTarget && m_SmoothingAngle > 0)
            {
                float animatededAngle = Quaternion.Angle(limb.lastLowBoneAnimationRotation, limb.LowBoneRotation);
                float targetAngle = Quaternion.Angle(limb.lastLowBoneRotation, limb.targetRotation);
                
                limb.targetRotation = Quaternion.Lerp(limb.lastLowBoneRotation, limb.targetRotation, Mathf.Clamp01((animatededAngle + (MAX_SMOOTHING_ANGLE - m_SmoothingAngle))/ targetAngle));
            }

            limb.lastLowBoneAnimationPosition = limb.LowBone.position;
            limb.lastLowBoneAnimationRotation = limb.LowBoneRotation;
        }
        /// <summary>
        /// Smoothing legs movements.
        /// </summary>
        /// <param name="limb">Character limb.</param>
        private void GlobalSmoothing(Limb limb)
        {
            if (m_GlobalSmoothingPower <= 0) return;

            limb.UpBone.rotation = Quaternion.Lerp(limb.lastUpBoneRotation, limb.UpBone.rotation, 1 - m_GlobalSmoothingPower);
            limb.MiddleBone.rotation = Quaternion.Lerp(limb.lastMiddleBoneRotation, limb.MiddleBone.rotation, 1 - m_GlobalSmoothingPower);
            limb.LowBoneRotation = Quaternion.Lerp(limb.lastLowBoneRotation, limb.LowBoneRotation, 1 - m_GlobalSmoothingPower);
        }
        /// <summary>
        /// Saves limb bones rotation
        /// </summary>
        /// <param name="limb">Character limb.</param>
        private void SavingPositionRotation(Limb limb)
        {
            limb.lastUpBoneRotation = limb.UpBone.rotation;
            limb.lastMiddleBoneRotation = limb.MiddleBone.rotation;
            limb.lastLowBoneRotation = limb.LowBone.rotation;

            limb.lastLowBonePosition = limb.LowBone.position;
        }
        /// <summary>
        /// Synchronizes bones rotation in case of use "AnimatorUpdateMode.AnimatePhysics"
        /// </summary>
        /// <param name="limb">Character limb.</param>
        private void RotationSynchronization(Limb limb)
        {
            limb.UpBone.rotation = limb.lastUpBoneRotation;
            limb.MiddleBone.rotation = limb.lastMiddleBoneRotation;
            limb.LowBone.rotation = limb.lastLowBoneRotation;
        }
        /// <summary>
        /// Checks Normal to constraint foots rotation;
        /// </summary>
        /// <param name="normal">Hit point normal.</param>
        /// <returns></returns>
        private Vector3 ConstraintedNormal(Vector3 normal)
        {
            if (normal.y < m_Incline)
                return Vector3.RotateTowards(transform.up, normal, m_InclineRadian, 0f);// Max incline rotation from up to Normal vector 
            else
                return normal;
        }
      
        /// <summary>
        /// Returns bone transform
        /// </summary>
        /// <param name="bone"></param>
        /// <returns></returns>
        private Transform AnimatorBone(HumanBodyBones bone)
        {
            return m_Anim.GetBoneTransform(bone);
        }
        private class Limb
        {
            public Transform UpBone { get; private set; }
            public Transform MiddleBone { get; private set; }
            public Transform LowBone { get; private set; }
            public Transform ExtraBone { get; private set; }

            public float UpperLength { get; private set; }
            public float LowerLength { get; private set; }
            public float Length { get; private set; }
            public float UpperLengthSquared { get; private set; }
            public float LowerLengthSquared { get; private set; }
            public float ExtraLength { get; private set; }

            public float distanceFromMesh;
            public float CosineRuleNumeratorPart { get; private set; }
            public float CosineRuleDenominator { get; private set; }

            public Vector3 targetPosition;
            public Quaternion targetRotation;

            public Vector3 lastLowBonePosition;

            public Quaternion lastLowBoneRotation;
            public Quaternion lastMiddleBoneRotation;
            public Quaternion lastUpBoneRotation;

            public Vector3 lastLowBoneAnimationPosition;
            public Quaternion lastLowBoneAnimationRotation;

            public bool canReachTarget;
            public Vector3 LowestHitPoint;

            private float m_AnimatorWeight;
            public float AnimatorWeight
            {
                get { return m_AnimatorWeight; }
                set { m_AnimatorWeight = Mathf.Clamp01(value); }
            }

            public Vector3 LowBonePosition
            {
                get { return LowBone.position; }
                set { LowBone.position = value; }
            }
            public Quaternion LowBoneRotation
            {
                get { return LowBone.rotation; }
                set { LowBone.rotation = value; }
            }

            public Limb(Transform upBone, Transform middleBone, Transform lowBone)
            {
                UpBone = upBone;
                MiddleBone = middleBone;
                LowBone = lowBone;

                UpperLength = (upBone.position - middleBone.position).magnitude;
                LowerLength = (middleBone.position - lowBone.position).magnitude;
                Length = UpperLength + LowerLength;

                UpperLengthSquared = (upBone.position - middleBone.position).sqrMagnitude;
                LowerLengthSquared = (middleBone.position - lowBone.position).sqrMagnitude;

                CosineRuleNumeratorPart = UpperLengthSquared + LowerLengthSquared;
                CosineRuleDenominator = 2 * UpperLength * LowerLength;

                lastLowBonePosition = lowBone.position;

                lastLowBoneAnimationPosition = lowBone.position;
                lastLowBoneAnimationRotation = lowBone.rotation;

                canReachTarget = false;
            }
            public Limb(Transform upBone, Transform middleBone, Transform lowBone, Transform extraBone)
            {
                UpBone = upBone;
                MiddleBone = middleBone;
                LowBone = lowBone;
                ExtraBone = extraBone;

                UpperLength = (upBone.position - middleBone.position).magnitude;
                LowerLength = (middleBone.position - lowBone.position).magnitude;
                ExtraLength = (extraBone.position - lowBone.position).magnitude;
                Length = UpperLength + LowerLength + ExtraLength;

                UpperLengthSquared = (upBone.position - middleBone.position).sqrMagnitude;
                LowerLengthSquared = (middleBone.position - lowBone.position).sqrMagnitude;

                CosineRuleNumeratorPart = UpperLengthSquared + LowerLengthSquared;
                CosineRuleDenominator = 2 * UpperLength * LowerLength;

                lastLowBonePosition = lowBone.position;

                lastLowBoneAnimationPosition = lowBone.position;
                lastLowBoneAnimationRotation = lowBone.rotation;

                canReachTarget = false;
            }
            /// <summary>
            /// Cosine Rule to find middle bone angle
            /// </summary>
            /// <param name="targetDistance"></param>
            /// <returns></returns>
            public float MiddleBoneAngle(float targetDistance)
            {
                return Mathf.Acos(Mathf.Clamp((CosineRuleNumeratorPart - (targetDistance * targetDistance)) / CosineRuleDenominator, -1, 1)) * Mathf.Rad2Deg;
            }
            /// <summary>
            /// Cosine Rule to find lower bone angle
            /// </summary>
            /// <param name="targetDistance"></param>
            /// <returns></returns>
            public float LowerBoneAngle(float targetDistance)
            {
                return Mathf.Acos(Mathf.Clamp(((targetDistance * targetDistance) + (LowerLengthSquared) - (UpperLengthSquared)) / (2 * targetDistance * LowerLength), -1, 1)) * Mathf.Rad2Deg; ;
            }
            /// <summary>
            /// Cosine Rule to find upper bone angle
            /// </summary>
            /// <param name="targetDistance"></param>
            /// <returns></returns>
            public float UpperBoneAngle(float targetDistance)
            {
                return Mathf.Acos(Mathf.Clamp(((targetDistance * targetDistance) + (UpperLengthSquared) - (LowerLengthSquared)) / (2 * targetDistance * UpperLength), -1, 1)) * Mathf.Rad2Deg; ;
            }

        }
    }
}