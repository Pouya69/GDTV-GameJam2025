using GLTFast.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

public class EnemyBaseController : CustomCharacterController
{
    public float CustomTimeDilation = 1f;  // Varies from 0f to 1f. 1 -> normal time. 0 -> stopped
    public float TimeDilationDifferenceIgnore = 0.01f;  // When reaching this threshold, make it equal to target.
    [NonSerialized] public float CustomTimeDilationTarget = 1f;  // We interpolate the Time Dilation to get the slow effect of transition
    [NonSerialized] public float TimeDilationInterpSpeed;  // How fast we interpolate it.
    [NonSerialized] public Vector3 GravityBeforeCustomGravity = Vector3.zero;  // For force fields
    [Header("Base Enemy")]
    public EnemyBaseCharacter EnemyBaseCharacterRef;
    public float StoppingDistanceFromDestination = 0.2f;  // If less than equal this, that means we have arrived.
    [SerializeField] public BehaviorGraphAgent MyBehaviourTreeAgent;
    [SerializeField] public NavMeshAgent MyNavAgent;
    [NonSerialized] public BlackboardReference MyBlackBoardRef;
    public AISenseHandler MySenseHandler;
    [Header("Ragdoll")]
    [NonSerialized] public bool IsInGravityField = false;
    public Rigidbody PelvisRigidBody;
    public Collider PelvisCollider;
    public Transform PelvisTransform;
    public LayerMask RagdollRecoverLayerMask = new LayerMask();
    public float RagdollRecoverGetUpDistanceCheck = 0.3f;
    [NonSerialized] public Vector3 PelvisCapsuleOffset;
    [NonSerialized] public Vector3 PelvisCharacterOffset;
    [NonSerialized] public Vector3 SkeletalMeshCapsuleOffset;
    public List<Transform> BonesListAll = new List<Transform>();  // All the bones in the skeleton Excluding weapons and extra. For ragdoll recovery.
    Dictionary<Transform, Pose> ragdollPose = new Dictionary<Transform, Pose>();
    [NonSerialized] public bool IsTryingToRecoverFromRagdoll = false;
    public float CheckRagdollRecoveryEverySeconds = 1f;
    [Header("Aiming")]
    public LayerMask AimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform TransformAimPoint;
    [SerializeField] private float AimTransitionSpeed = 50f;
    public GameObject AimFocusPoint;
    [Range(0.1f, 1f)]
    public float AimAccuracy = 0.7f;
    public float AccuracyMultiplier = 1.5f;

    // TODO: BASE SPEED FOR ENEMY
    


    public void CacheRagdollPose()
    {
        ragdollPose.Clear();
        foreach (Transform bone in BonesListAll)
            ragdollPose[bone] = new Pose(bone.localPosition, bone.localRotation);
    }

    void CollectBones(Transform current)
    {
        BonesListAll.Add(current);
        foreach (Transform child in current)
        {
            CollectBones(child);
        }
    }


    public IEnumerator BlendToAnimatorPose(float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            foreach (Transform bone in BonesListAll)
            {
                Pose cached = ragdollPose[bone];
                bone.SetLocalPositionAndRotation(Vector3.Lerp(cached.position, bone.localPosition, t), Quaternion.Slerp(cached.rotation, bone.localRotation, t));
            }
            time += Time.deltaTime;
            yield return null;
        }
    }


    public Vector3 GetEnemyForward()
    {
        return -this.MyNavAgent.velocity.normalized;
    }

    public void LookAtPlayer(bool ShouldLookAtPlayer) {
        CharacterBaseRef.IsAimingWeapon = ShouldLookAtPlayer;
        IK_Aim.weight = ShouldLookAtPlayer ? 1f : 0f;
        IK_Aim_Rig.weight = ShouldLookAtPlayer ? 1f : 0f;
        IK_Aim_RigBuilder.layers[0].active = ShouldLookAtPlayer;
        IK_Aim_RigAnimation.enabled = ShouldLookAtPlayer;
        // IK_Aim.data.sourceObjects.SetTransform(0, ShouldLookAtPlayer ? MySenseHandler.PlayerCharacterRef_CHECK_ONLY.CapsuleCollision.transform : null);
        CharacterBaseRef.CurrentMovementSpeed = ShouldLookAtPlayer ? CharacterBaseRef.AimingMovementSpeed : CharacterBaseRef.MovementSpeed;
    }

    public void RotateTowardsPlayer()
    {
        RotateTowardsTarget(MySenseHandler.PlayerCharacterRef_CHECK_ONLY.CapsuleCollision.transform.position);
        //Vector3 gravityUp = -GetGravityDirection(); // character's up
        //Vector3 targetPosition = MySenseHandler.PlayerCharacterRef_CHECK_ONLY.CapsuleCollision.transform.position;
        
        //Vector3 toTarget = (targetPosition - EnemyBaseCharacterRef.CapsuleCollision.transform.position).normalized;
        // Quaternion targetRotation = Quaternion.LookRotation(toTarget, gravityUp);
        //EnemyBaseCharacterRef.CapsuleCollision.transform.LookAt(toTarget, gravityUp);
        // Quaternion LocalYaw = Quaternion.AngleAxis(local, gravityUp);
        //Quaternion TargetRotation = Quaternion.LookRotation(toTarget, gravityUp);
        // transform.rotation = TargetRotation;
        // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        
    }

    public void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 toTarget = targetPosition - transform.position;
        Vector3 localUp = -GetGravityDirection();
        // Project the direction onto the plane perpendicular to the character's up
        Vector3 projectedDirection = Vector3.ProjectOnPlane(toTarget, localUp).normalized;

        if (projectedDirection.sqrMagnitude < 0.0001f)
            return; // Avoid zero direction

        // Compute the current forward direction projected on the same plane
        Vector3 projectedForward = Vector3.ProjectOnPlane(transform.forward, localUp).normalized;

        // Compute the rotation from current forward to the target direction
        //Quaternion rotation = Quaternion.FromToRotation(projectedForward, projectedDirection);

        // Apply rotation around the local up axis
        transform.rotation = Quaternion.AngleAxis(
            Vector3.SignedAngle(projectedForward, projectedDirection, localUp),
            localUp
        ) * transform.rotation;
    }









    public void SetTimeDilation(float NewTimeDilation, float NewTimeDilationInterpSpeed = -1f)
    {
        if (NewTimeDilationInterpSpeed > 0f)  // By default we don't change the speed but we can have custom interpolation speed for time.
            TimeDilationInterpSpeed = NewTimeDilationInterpSpeed;
        else {
            // If the speed is less than equal 0, we set the time dilation instantly
            this.CustomTimeDilation = NewTimeDilation;
            this.MyBlackBoardRef.SetVariableValue<float>("CurrentTimeDilation", this.CustomTimeDilation);
        }  
        this.CustomTimeDilationTarget = NewTimeDilation;
        // this.RigidbodyRef.linearVelocity = GetTimeScaledVelocity() + (GetGravityForceTimeScaled() * Time.deltaTime);
    }

    public void UpdateTimeDilation()
    {
        if (!IsInterpolatingTimeDilation()) return;
        this.MyBlackBoardRef.SetVariableValue<float>("CurrentTimeDilation", this.CustomTimeDilation);
        this.CustomTimeDilation = Mathf.Lerp(this.CustomTimeDilation, this.CustomTimeDilationTarget, 1 - Mathf.Exp(-this.TimeDilationInterpSpeed * Time.deltaTime));
        if (Mathf.Abs(this.CustomTimeDilationTarget - this.CustomTimeDilation) < TimeDilationDifferenceIgnore)
            this.CustomTimeDilation = this.CustomTimeDilationTarget;
    }

    public Vector3 GetTimeScaledVelocity() { return InputVelocity * CustomTimeDilation; }

    public Vector3 GetGravityForceTimeScaled() { return this.BaseGravity * this.CustomTimeDilation; }

    public override void SetGravityForceAndDirection(Vector3 Final, bool IsDoneByForceField = false)
    {
        base.SetGravityForceAndDirection(Final, IsDoneByForceField);
        if (!IsDoneByForceField)
            this.GravityBeforeCustomGravity = Final;
    }

    public bool IsInterpolatingTimeDilation()
    {
        return TimeDilationInterpSpeed > 0f && this.CustomTimeDilationTarget != this.CustomTimeDilation;
    }

    public override void UpdateCharacterMovement(float Multiplier = 1)
    {
        this.UpdateTimeDilation();
        // The movement input is update from CustomMoveToAction node.
        if (!this.IsAirCharacter)
            CheckIsOnGround();  // Air characters will never check for onGround
        this.ManualMovementThroughNavAgent();
        this.RigidbodyRef.linearDamping = IsOnGround ? Damping : 0.0f;
        if (IsOnGround)
            RigidbodyRef.AddForce(this.CustomTimeDilation * InputVelocity);
        else
            RigidbodyRef.AddForce(this.CustomTimeDilation * (InputVelocity + (this.BaseGravity * this.RigidbodyRef.mass)));
        InputVelocity = Vector3.zero;
        
        
        // base.UpdateCharacterMovement(CustomTimeDilation);
    }

    public override void InteroplateCharacterRotation()
    {
        // Vector3 FinalDirection = -(Quaternion.identity * LastMovementDirection);
        
        //if (MySenseHandler.CanSeePlayer)
        //{
        //    TargetRotation = Quaternion.LookRotation((MySenseHandler.PlayerCharacterRef_CHECK_ONLY.transform.position - CharacterBaseRef.CapsuleCollision.transform.position).normalized, LocalUp);
        //}
        if (EnemyBaseCharacterRef.IsRagdolling())
        {
            return;
        }
        Vector3 LocalUp = -GetGravityDirection();
        if (EnemyBaseCharacterRef.IsAimingWeapon)
        {
            
            RotateTowardsPlayer();
            //if (LocalUp.Equals(Vector3.zero))
            //    LocalUp = Vector3.up;
            //TargetRotation = Quaternion.AngleAxis(CameraRotation.y - (LocalUp.y < 0 ? 0 : 180), LocalUp) * Quaternion.LookRotation(GetForwardBasedOnGravity(), LocalUp);
            //EnemyBaseCharacterRef.CapsuleCollision.transform.rotation = Quaternion.RotateTowards(EnemyBaseCharacterRef.CapsuleCollision.transform.rotation, TargetRotation, RotationSpeed * Time.deltaTime);
            return;
        }

        Vector3 FinalDirection = GetEnemyForward();
        
        if (FinalDirection.magnitude >= 0.01)
        {
            FinalDirection.Normalize();
            
            if (Mathf.RoundToInt(Vector3.Angle(FinalDirection, LocalUp)) <= 94)
                TargetRotation = Quaternion.LookRotation(FinalDirection, LocalUp);
        }


        EnemyBaseCharacterRef.CapsuleCollision.transform.rotation = Quaternion.RotateTowards(EnemyBaseCharacterRef.CapsuleCollision.transform.rotation, TargetRotation, RotationSpeed * Time.deltaTime);
        
    }

    public void ManualMovementThroughNavAgent()
    {
        if (this.MyNavAgent.pathPending) return;
        if (this.MyNavAgent.path.corners == null || this.MyNavAgent.path.corners.Length == 0)
        {
            // CurrentCornerIndex = 0;
            return;
        }
        //if (CurrentCornerIndex > this.MyNavAgent.path.corners.Length-1)
            //return;
        Vector3 MyPosition = EnemyBaseCharacterRef.CapsuleCollision.transform.position;
        Vector3 CurrentTargetInPath = this.MyNavAgent.nextPosition; //this.MyNavAgent.path.corners[CurrentCornerIndex];
        this.AddMovementInput((CurrentTargetInPath - MyPosition).normalized, EnemyBaseCharacterRef.CurrentMovementSpeed);
        //if (Vector3.Distance(MyPosition, CurrentTargetInPath) < StoppingDistanceFromDestination)
            //CurrentCornerIndex++;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        // IK_Aim.data.sourceObjects.Add(new UnityEngine.Animations.Rigging.WeightedTransform(MySenseHandler.PlayerCharacterRef_CHECK_ONLY.CapsuleCollision.transform, 1f));
        LookAtPlayer(false);
        this.MyBlackBoardRef = this.MyBehaviourTreeAgent.BlackboardReference;
        // this.MyBlackBoardRef.SetVariableValue<EnemyBaseCharacter>("SelfEnemyRef", this.EnemyBaseCharacterRef);
    }

    public Vector3 GetCurrentNavAgentVelocity() { return MyNavAgent.velocity; }

    // Update is called once per frame
    public override void Update()
    {
        if (!EnemyBaseCharacterRef.IsAlive())
        {
            EnemyBaseCharacterRef.StopShootingWeapon();
            return;
        }
        base.Update();
        if (EnemyBaseCharacterRef.CapsuleCollision.enabled)
            CheckRaycastFromViewPoint();
        if (!MySenseHandler.CanSeePlayer)
            EnemyBaseCharacterRef.StopShootingWeapon();
        /*if (EnemyBaseCharacterRef.IsRagdolling() || !EnemyBaseCharacterRef.CapsuleCollision.enabled)
        {
            MyNavAgent.nextPosition = transform.position;
            transform.position = PelvisTransform.position + PelvisCharacterOffset;
        }*/
    }

    public override void FixedUpdate()
    {
        // EnemyBaseCharacterRef.CapsuleCollision.transform.position = PelvisTransform.position + PelvisCapsuleOffset;

        if (EnemyBaseCharacterRef.IsRagdolling())
        {
            // EnemyBaseCharacterRef.CapsuleCollision.transform.position = PelvisTransform.position + PelvisCapsuleOffset;
            if (!EnemyBaseCharacterRef.EnemyAnimator.enabled && !IsTryingToRecoverFromRagdoll)
            {
                Vector3 GroundLoc = CheckIsOnGround_RAGDOLL();
                if (ShouldStopRagdolling())
                {
                    IsTryingToRecoverFromRagdoll = true;
                    StartCoroutine(RagdollRecoveryTimer(GroundLoc));
                    return;
                }
                if (!IsOnGround)
                {
                    PelvisRigidBody.AddForce(RigidbodyRef.mass * GetGravityForceTimeScaled());
                    // RigidbodyRef.AddForce(ForceApplied);
                }
                
                InputVelocity = Vector3.zero;
                // CheckIsOnGround_RAGDOLL();

                // InteroplateCharacterRotation();
            }
            //MyNavAgent.nextPosition = transform.position;
            //transform.position = PelvisTransform.position + PelvisCharacterOffset;
            return;
        }
        //else if (!EnemyBaseCharacterRef.CapsuleCollision.enabled)
        //{
        //    transform.position = PelvisTransform.position + PelvisCharacterOffset;
        //    MyNavAgent.nextPosition = transform.position;
        //}
        
        // Debug.Log("Normal movement");
        //if (EnemyBaseCharacterRef.IsRagdollRecoveryCompleted())
        base.FixedUpdate();
    }

    protected override void Awake()
    {
        base.Awake();
        PelvisCharacterOffset = EnemyBaseCharacterRef.transform.position - PelvisTransform.position;
        PelvisCapsuleOffset = EnemyBaseCharacterRef.CapsuleCollision.transform.position - PelvisTransform.position;
        SkeletalMeshCapsuleOffset = EnemyBaseCharacterRef.SkeletalMesh.transform.position - EnemyBaseCharacterRef.CapsuleCollision.transform.position;


        // BonesListAll.Clear();
        // CollectBones(PelvisTransform);
        this.MyNavAgent.updateRotation = false;  // This is done by the custom movements that we have already.
        this.MyNavAgent.updateUpAxis = false;  // Done by custom gravity
        this.MyNavAgent.updatePosition = false;
    }

    // For the blackboard.
    public void UpdatePlayerCharacterRef(PlayerCharacter playerCharacterRef = null, Vector3 LastPlayerLocation = new Vector3())
    {
        if (playerCharacterRef == null)
        {
            LookAtPlayer(false);
            NavMeshPath p = new();
            // Checking if player was on my surface/reachable.
            this.MyBlackBoardRef.SetVariableValue<Vector3>("LastKnownPlayerLocation", LastPlayerLocation);
            // this.MyBlackBoardRef.SetVariableValue<bool>("WasLastPlayerLocationInMySurface", this.MyNavAgent.CalculatePath(LastPlayerLocation, p));
        }
        else
            LookAtPlayer(true);
        this.MyBlackBoardRef.SetVariableValue<PlayerCharacter>("PlayerCharacterRef", playerCharacterRef);
        this.MyBlackBoardRef.SetVariableValue<Transform>("PlayerCharacterRefTRANSFORM", playerCharacterRef == null ? null : playerCharacterRef.CapsuleCollision.transform);
    }

    public bool RagdollShouldGetUpFromBack() {
        // The forward is actually the back side of pelvis.
        return Physics.Raycast(PelvisTransform.position, PelvisTransform.forward, RagdollRecoverGetUpDistanceCheck, RagdollRecoverLayerMask);
    }

    public Vector3 CheckIsOnGround_RAGDOLL()
    {
        Vector3 Start = PelvisTransform.position;
        Vector3 GravityDirection = GravityBeforeCustomGravity.normalized;
        Debug.DrawLine(Start, Start + (GravityDirection * 0.3f), Color.cyan);
        bool didHitGround = Physics.Raycast(Start, GravityDirection, out RaycastHit HitResult, 0.25f, GroundCheckLayerMask);
        if (!didHitGround)
        {
            IsOnGround = false;
            return Vector3.zero;
        }
        IsOnGround = !HitResult.collider.transform.CompareTag("GameController") && !HitResult.collider.transform.root.TryGetComponent<PhysicsObjectBasic>(out _);
        return HitResult.point;
    }

    IEnumerator RagdollRecoveryTimer(Vector3 GroundLoc)
    {
        yield return new WaitForSeconds(CheckRagdollRecoveryEverySeconds);
        // CacheRagdollPose();
        // StartCoroutine(EnemyBaseCharacterRef.RecoverFromRagdollCoroutine(!RagdollShouldGetUpFromBack()));
        // EnemyBaseCharacterRef.CapsuleCollision.transform.position = PelvisTransform.position + PelvisCapsuleOffset;
        PelvisRigidBody.linearVelocity = Vector3.zero;
        PelvisRigidBody.angularVelocity = Vector3.zero;
        // StartCoroutine(EnemyBaseCharacterRef.RecoverFromRagdollCoroutine(!RagdollShouldGetUpFromBack()));
        // Vector3 GroundLoc = CheckIsOnGround_RAGDOLL();
        if (GroundLoc.Equals(Vector3.zero))
            GroundLoc = PelvisTransform.position;
        EnemyBaseCharacterRef.StopRagdolling(!RagdollShouldGetUpFromBack(), GroundLoc);
        // Do something after the delay
    }

    public bool ShouldStopRagdolling()
    {
        if (!EnemyBaseCharacterRef.IsAlive()) return false;
        bool Result = !IsInGravityField && IsOnGround && PelvisRigidBody.linearVelocity.magnitude <= 0.1f;
        if (!Result)
        {
            Debug.Log("OnGround: " + IsOnGround + ", pelvis vel mag: " + PelvisRigidBody.linearVelocity.magnitude);
        }
        return Result;
    }

    public override Vector3 GetForwardShootingVector()
    {
        //float Accuracy = 1 - AimAccuracy;
        //if (UnityEngine.Random.value >= Accuracy)
            //return (TransformAimPoint.position - EnemyBaseCharacterRef.CurrentWeaponEquipped.ShootLocation_TEST_ONLY.transform.position).normalized;
        // return ((TransformAimPoint.position + (Accuracy * AccuracyMultiplier * UnityEngine.Random.insideUnitSphere)) - EnemyBaseCharacterRef.CurrentWeaponEquipped.ShootLocation_TEST_ONLY.transform.position).normalized;
        return (TransformAimPoint.position - EnemyBaseCharacterRef.CurrentWeaponEquipped.ShootLocation_TEST_ONLY.transform.position).normalized;
    }

    public override void CheckRaycastFromViewPoint()
    {
        if (!EnemyBaseCharacterRef.IsAimingWeapon) return;
        Vector3 Start = MySenseHandler.RaycastStartPoint.position;
        // Vector3 Direction = GetForwardShootingVector();
        Vector3 Direction = MySenseHandler.RaycastStartPoint.forward;
        Vector3 FinalAimLocation = MySenseHandler.PlayerCharacterRef_CHECK_ONLY.CapsuleCollision.transform.position;
        TransformAimPoint.position = Vector3.Lerp(TransformAimPoint.position, FinalAimLocation, Time.deltaTime * AimTransitionSpeed);
        /*if (Physics.Raycast(Start, Direction, out RaycastHit HitResult, 999f, AimColliderLayerMask))
        {
            TransformAimPoint.position = Vector3.Lerp(TransformAimPoint.position, HitResult.point, Time.deltaTime * AimTransitionSpeed);
        }
        else
        {
            TransformAimPoint.position = Vector3.Lerp(TransformAimPoint.position, Start + Direction * 5, Time.deltaTime * AimTransitionSpeed);
        }*/
    }

    
}
