using System;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    // This is for handling damage types when receiving damage.
    public enum EDamageType
    {
        DEFAULT,  // For when just want to do it.
        BULLET,
    }

    [Header("Components")]
    public CapsuleCollider CapsuleCollision;  // Will export the components so we can set them in engine.
    
    public GameObject SkeletalMesh;  // Handles the mesh for animations. Consists of Animator and Skinned Mesh Renderer.
    public CustomCharacterController MyController;

    [Header("Movement Character")]
    [NonSerialized] public float CurrentMovementSpeed;
    public float MovementSpeed = 10f;
    public float MovementSpeedSprint = 50f;
    [NonSerialized] public bool IsSprinting = false;
    public float GravityForce = 981f;  // For when we are changing gravity. It's the strength that can be changed through code/inspector.

    [Header("Time Dilation")]
    public float CustomTimeDilation = 1f;  // Varies from 0f to 1f. 1 -> normal time. 0 -> stopped
    public float TimeDilationDifferenceIgnore = 0.01f;  // When reaching this threshold, make it equal to target.

    [Header("Weapon")]
    [NonSerialized] public WeaponBase CurrentWeaponEquipped;

    [Header("Health")]
    float Health;
    public float MaxHealth = 100f;

    public virtual void Start()
    {
        Health = MaxHealth;
        CurrentMovementSpeed = MovementSpeed;
    }

    public virtual void Awake()
    {
        // CharacterMesh = SkeletalMesh.GetComponent<SkinnedMeshRenderer>();
        // CharacterAnimator = SkeletalMesh.GetComponent<Animator>();
    }

    public virtual void Update()
    {
        // Debug.Log(CurrentMovementSpeed.ToString());
    }
    public float GetCapsuleCollisionRadius() { return this.CapsuleCollision.radius; }
    public float GetCapsuleCollisionHeight() { return this.CapsuleCollision.height; }

    public float GetCurrentHealth() { return this.Health; }

    public void Die()
    {
        this.Health = 0;
    }

    public void ReduceHealth(EDamageType DamageType, float Amount)
    {
        // TODO: Handling DamageTypes
        switch (DamageType)
        {
            case EDamageType.BULLET:
                break;
            case EDamageType.DEFAULT:
                break;
            default:
                break;
        }
        this.Health -= Amount;
        if (this.Health < 0) Die();
    }
    
    public virtual void Move(Vector2 Direction)
    {
        MyController.AddMovementInput(Direction, CurrentMovementSpeed);
    }

    public void MoveUp(float Axis)
    {
        MyController.AddMovementInput(Axis * -MyController.GetGravityDirection(), CurrentMovementSpeed);
    }

    public bool HasWeaponEquipped() { return CurrentWeaponEquipped != null; }

    public virtual void Attack()
    {
        // Gets Derrived by child classes.
    }
}
