using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] private BehaviorGraphAgent MyBehaviourTreeAgent;
    [SerializeField] public NavMeshAgent MyNavAgent;
    [NonSerialized] private BlackboardReference MyBlackBoardRef;
    public AISenseHandler MySenseHandler;
    // [NonSerialized] private int CurrentCornerIndex = 0;

    public Vector3 GetEnemyForward()
    {
        return -this.MyNavAgent.velocity.normalized;
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
        Vector3 FinalDirection = GetEnemyForward();
        if (FinalDirection.magnitude >= 0.01)
        {
            FinalDirection.Normalize();
            Vector3 LocalUp = -GetGravityDirection();
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
        this.MyBlackBoardRef = this.MyBehaviourTreeAgent.BlackboardReference;
        // this.MyBlackBoardRef.SetVariableValue<EnemyBaseCharacter>("SelfEnemyRef", this.EnemyBaseCharacterRef);
    }

    public Vector3 GetCurrentNavAgentVelocity() { return MyNavAgent.velocity; }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    protected override void Awake()
    {
        base.Awake();
        this.MyNavAgent.updateRotation = false;  // This is done by the custom movements that we have already.
        this.MyNavAgent.updateUpAxis = false;  // Done by custom gravity
        this.MyNavAgent.updatePosition = false;
    }

    // For the blackboard.
    public void UpdatePlayerCharacterRef(PlayerCharacter playerCharacterRef = null, Vector3 LastPlayerLocation = new Vector3())
    {
        this.MyBlackBoardRef.SetVariableValue<PlayerCharacter>("PlayerCharacterRef", playerCharacterRef);
        this.MyBlackBoardRef.SetVariableValue<Transform>("PlayerCharacterRefTRANSFORM", playerCharacterRef == null ? null : playerCharacterRef.CapsuleCollision.transform);
        if (playerCharacterRef == null)
        {
            NavMeshPath p = new();
            // Checking if player was on my surface/reachable.
            this.MyBlackBoardRef.SetVariableValue<Vector3>("LastKnownPlayerLocation", LastPlayerLocation);
            this.MyBlackBoardRef.SetVariableValue<bool>("WasLastPlayerLocationInMySurface", this.MyNavAgent.CalculatePath(LastPlayerLocation, p));
        }
    }
}
