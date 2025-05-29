using UnityEngine;
using UnityEngine.SocialPlatforms;

public class BossController : EnemyBaseController
{
    [Header("Boss")]
    public float BaseSpeed = 20f;
    public BossCharacter MyBossCharacter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        // base.Start();
        this.MyBlackBoardRef = this.MyBehaviourTreeAgent.BlackboardReference;
        if (IsAirCharacter) IsOnGround = false;
        if (!IsAirCharacter && BaseGravity.magnitude == 0)
        {
            BaseGravity = new Vector3(Physics.gravity.x, Physics.gravity.y, Physics.gravity.z); // * this.RigidbodyRef.mass;
        }
        else
            SetGravityForceAndDirection(this.BaseGravity);
        RigidbodyRef.linearDamping = Damping;
    }

    // Update is called once per frame
    public override void Update()
    {
        // base.Update();
        // Setting speed.
        this.MyNavAgent.speed = BaseSpeed * this.CustomTimeDilation;
        this.MyBlackBoardRef.SetVariableValue<float>("MyBTMovementSpeed", this.MyNavAgent.speed);
    }

    protected override void Awake()
    {
        this.MyNavAgent.updateRotation = false;  // This is done by the custom movements that we have already.
        this.MyNavAgent.updateUpAxis = false;  // Done by custom gravity
        this.MyNavAgent.updatePosition = false;
    }

    public override void SetGravityForceAndDirection(Vector3 Final, bool IsDoneByForceField = false)
    {
        // base.SetGravityForceAndDirection(Final, IsDoneByForceField);
    }

    public override void CheckRaycastFromViewPoint()
    {
    }

    public override Vector3 GetForwardShootingVector()
    {
        return MyBossCharacter.transform.forward;
    }

    public override void FixedUpdate()
    {
        // base.FixedUpdate();
        InteroplateCharacterRotation();
        if (!IsMovementDisabled)
            UpdateCharacterMovement();
    }

    public override void UpdateCharacterMovement(float Multiplier = 1)
    {
        base.UpdateCharacterMovement(Multiplier);
    }

    public override void ManualMovementThroughNavAgent()
    {
        if (this.MyNavAgent.pathPending) return;
        if (this.MyNavAgent.path.corners == null || this.MyNavAgent.path.corners.Length == 0)
        {
            return;
        }
        Vector3 MyPosition = MyBossCharacter.CapsuleCollision.transform.position;
        Vector3 CurrentTargetInPath = this.MyNavAgent.nextPosition;
        if (Vector3.Distance(MyPosition, CurrentTargetInPath) < 0.3f) return;
        this.AddMovementInput((CurrentTargetInPath - MyPosition).normalized, MyBossCharacter.CurrentMovementSpeed);
    }

    public override void AddMovementInput(Vector3 Direction, float Scale)
    {
        base.AddMovementInput(Direction, Scale);
    }

    public override void InteroplateCharacterRotation()
    {
        Vector3 LocalUp = -GetGravityDirection();
        if (MyBossCharacter.IsChangingGravity)
        {
            TargetRotation = Quaternion.LookRotation(-MyBossCharacter.NewGravity.normalized, LocalUp);
        }
        else if (MyBossCharacter.IsLookingAtPlayer)
        {
            RotateTowardsPlayer();
            return;
        }
        else
        {
            Vector3 FinalDirection = GetEnemyForward();

            if (FinalDirection.magnitude >= 0.01)
            {
                FinalDirection.Normalize();

                if (Mathf.RoundToInt(Vector3.Angle(FinalDirection, LocalUp)) <= 94)
                    TargetRotation = Quaternion.LookRotation(FinalDirection, LocalUp);
            }
        }

        MyBossCharacter.CapsuleCollision.transform.rotation = Quaternion.RotateTowards(MyBossCharacter.CapsuleCollision.transform.rotation, TargetRotation, RotationSpeed * Time.deltaTime);
    }
}
