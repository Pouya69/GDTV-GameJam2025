using System;
using Unity.VisualScripting;
using UnityEngine;

public class CustomCharacterController : MonoBehaviour
{
    [Header("Components")]
    public ConstantForce ConstantGravityForce;
    public CharacterBase CharacterBaseRef;
    public Rigidbody RigidbodyRef;
    [Header("Movements")]
    public float DownGroundCheckAfterCapsule = 0.4f;
    [NonSerialized] public Vector3 CurrentAcceleration = Vector3.zero;  // Movement Only. Gravity is done using ConstantGravityForce
    [NonSerialized] public Vector3 InputVelocity = Vector3.zero;  // Clears out after doing the actions
    public float RotationSpeed = 500f;
    public float RotationSpeed_GRAVITYONLY = 5000f;  // For when preventing the player from falling
    [NonSerialized] public Quaternion TargetRotation;
    public float MaximumSpeed = 100f;
    public float Damping = 20f;
    public bool IsMovementDisabled = false;  // For disabling Character Movement
    [NonSerialized] public bool IsOnGround = true;
    public bool IsAirCharacter = false;  // For characters that roam in the air
    [NonSerialized] Vector3 LastMovementDirection = Vector3.zero;  // For interpolating the character
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        // For when gravity is not set beforehand, we set it to default. Gives more control for initialization.
        if (IsAirCharacter) IsOnGround = false;
        if (!IsAirCharacter && ConstantGravityForce.force.magnitude == 0)
        {
            ConstantGravityForce.force = new Vector3(Physics.gravity.x, Physics.gravity.y, Physics.gravity.z) * 100;
        }
        RigidbodyRef.linearDamping = Damping;
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
        InputVelocity += Direction.normalized * Scale;
        LastMovementDirection = InputVelocity.normalized;
    }

    public virtual void InteroplateCharacterRotation()
    {
        // Vector3 FinalDirection = -(transform.rotation * LastMovementDirection);
        Vector3 FinalDirection = -(transform.rotation * LastMovementDirection);
        Vector3 LocalUp = -GetGravityDirection();
        TargetRotation = Quaternion.LookRotation(FinalDirection, LocalUp);
        // Debug.LogWarning(TargetRotation.ToString());
    }

    public virtual void UpdateCharacterMovement()
    {
        if (!this.IsAirCharacter)
            CheckIsOnGround();  // Air characters will never check for onGround
        RigidbodyRef.AddForce(InputVelocity);
        Debug.Log(InputVelocity.magnitude.ToString());
        // Debug.LogWarning("Mag: " + RigidbodyRef.linearVelocity.magnitude.ToString());
        InputVelocity = Vector3.zero;
    }


    public void CheckIsOnGround()
    {
        Vector3 Start = CharacterBaseRef.CapsuleCollision.transform.position;
        Vector3 GravityDirection = GetGravityDirection();
        RaycastHit HitResult;
        bool didHitGround = Physics.Raycast(Start, GravityDirection, out HitResult, DownGroundCheckAfterCapsule + (CharacterBaseRef.GetCapsuleCollisionHeight()), 1);
        IsOnGround = didHitGround;
    }

    public Vector3 GetGravityDirection() { return ConstantGravityForce.force.normalized; }

    public Vector3 GetRightBasedOnGravity()
    {
        Vector3 LocalUp = -GetGravityDirection();
        return Quaternion.FromToRotation(Vector3.up, LocalUp) * Vector3.right;
    }

    public Vector3 GetForwardBasedOnGravity()
    {
        Vector3 LocalUp = -GetGravityDirection();
        return Quaternion.FromToRotation(Vector3.up, LocalUp) * Vector3.forward;
    }

    public virtual void SetGravityForceAndDirection(Vector3 Final) {
        this.ConstantGravityForce.force = new Vector3(Final.x, Final.y, Final.z);
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
