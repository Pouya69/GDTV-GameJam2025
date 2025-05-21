using System;
using UnityEngine;

public class CustomCharacterController : MonoBehaviour
{
    [Header("Components")]
    // public ConstantForce ConstantGravityForce;
    public Vector3 BaseGravity = new Vector3(0, -981f, 0);
    public CharacterBase CharacterBaseRef;
    public Rigidbody RigidbodyRef;
    [Header("Movements")]
    public bool CanMoveInAir = false;
    public float DownGroundCheckAfterCapsule = 0.4f;
    [NonSerialized] public Vector3 CurrentAcceleration = Vector3.zero;  // Movement Only. Gravity is done using ConstantGravityForce
    [NonSerialized] public Vector3 InputVelocity = Vector3.zero;  // Clears out after doing the actions
    [NonSerialized] public Vector3 VelocityBeforeTimeDilation = Vector3.zero;
    public float RotationSpeed = 500f;
    public float RotationSpeed_GRAVITYONLY = 5000f;  // For when preventing the player from falling
    [NonSerialized] public Quaternion TargetRotation;
    public float MaximumSpeed = 100f;
    public float Damping = 20f;
    public bool IsMovementDisabled = false;  // For disabling Character Movement
    [NonSerialized] public bool IsOnGround = true;
    public bool IsAirCharacter = false;  // For characters that roam in the air
    [NonSerialized] public Vector3 LastMovementDirection = Vector3.zero;  // For interpolating the character
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        // For when gravity is not set beforehand, we set it to default. Gives more control for initialization.
        if (IsAirCharacter) IsOnGround = false;
        if (!IsAirCharacter && BaseGravity.magnitude == 0)
        {
            BaseGravity = new Vector3(Physics.gravity.x, Physics.gravity.y, Physics.gravity.z); // * this.RigidbodyRef.mass;
        }
        else
            SetGravityForceAndDirection(this.BaseGravity);
        RigidbodyRef.linearDamping = Damping;
    }

    protected virtual void Awake()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public virtual void FixedUpdate()
    {
        InteroplateCharacterRotation();
        if (!IsMovementDisabled)
            UpdateCharacterMovement();
    }

    public virtual void AddMovementInput(Vector3 Direction, float Scale)
    {
        // if (!CanMoveInAir && !IsOnGround) return;
        InputVelocity += Direction.normalized * Scale * (IsOnGround ? 1 : 0.2f);
        
        LastMovementDirection = InputVelocity.normalized;
    }

    public virtual void InteroplateCharacterRotation()
    {
        // TODO: When character is boosing himself, does not work still needs work. :(
        Vector3 FinalDirection = -(Quaternion.identity * LastMovementDirection);
        Vector3 LocalUp = -GetGravityDirection();
        if (Mathf.RoundToInt(Vector3.Angle(FinalDirection, LocalUp)) <= 94)
            TargetRotation = Quaternion.LookRotation(FinalDirection, LocalUp);
    }

    public virtual void UpdateCharacterMovement(float Multiplier = 1f)
    {
        if (!this.IsAirCharacter)
            CheckIsOnGround();  // Air characters will never check for onGround
        this.RigidbodyRef.linearDamping = IsOnGround ? Damping : 0.5f;
        if (IsOnGround)
            RigidbodyRef.AddForce(Multiplier * InputVelocity);
        else
            RigidbodyRef.AddForce(Multiplier * (InputVelocity + (this.BaseGravity * this.RigidbodyRef.mass)));
        InputVelocity = Vector3.zero;
    }


    public void CheckIsOnGround()
    {
        Vector3 Start = CharacterBaseRef.CapsuleCollision.transform.position;
        Vector3 GravityDirection = GetGravityDirection();
        RaycastHit HitResult;
        Debug.DrawLine(Start, Start + (GravityDirection * (DownGroundCheckAfterCapsule + (CharacterBaseRef.GetCapsuleCollisionHeight()))), Color.cyan);
        bool didHitGround = Physics.Raycast(Start, GravityDirection, out HitResult, DownGroundCheckAfterCapsule + (CharacterBaseRef.GetCapsuleCollisionHeight()), 1);
        if (!didHitGround)
        {
            IsOnGround = false;
            return;
        }
        IsOnGround = !HitResult.collider.transform.tag.Equals("GameController");

    }

    public Vector3 GetGravityDirection() { return BaseGravity.normalized; }

    public Vector3 GetRightBasedOnGravity()
    {
        Vector3 LocalUp = -GetGravityDirection();
        return (Quaternion.FromToRotation(Vector3.up, LocalUp) * Vector3.right).normalized;
    }

    public Vector3 GetForwardBasedOnGravity()
    {
        Vector3 LocalUp = -GetGravityDirection();
        return (Quaternion.FromToRotation(Vector3.up, LocalUp) * Vector3.forward).normalized;
    }

    public virtual void SetGravityForceAndDirection(Vector3 Final, bool IsDoneByForceField=false) {
        this.BaseGravity = new Vector3(Final.x, Final.y, Final.z);// * this.RigidbodyRef.mass;
    }

    // Changes EVERY physics objects' gravity that does not have custom gravity and is not simulated by ConstantForce.
    public void ChangeWorldGravity(Vector3 Final)
    {
        Physics.gravity = Final;
    }

    public Vector3 GetRandomGravityDirection(float GForce = -981f)
    {
        int Dir = UnityEngine.Random.Range(0, 1) == 1 ? 1 : -1;
        switch (UnityEngine.Random.Range(0, 2))
        {
            case 0:
                return new Vector3(Dir * GForce, 0, 0);
            case 1:
                return new Vector3(0, Dir * GForce, 0);
            case 2:
                return new Vector3(0, 0, Dir * GForce);
            default:
                break;
        }
        return new Vector3();
    }

    public Quaternion CharacterPlanarRotation => Quaternion.AngleAxis(CharacterBaseRef.CapsuleCollision.transform.rotation.y, -GetGravityDirection());  // TODO
}
