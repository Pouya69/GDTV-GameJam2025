using System;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    // This is for handling damage types when receiving damage.
    public enum EDamageType
    {
        DEFAULT,  // For when just want to do it.
        FALL_DAMAGE,
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
    public float MovementSpeedChangeSpeed = 30f;
    public float AimingMovementSpeed = 1000f;
    [NonSerialized] public bool IsSprinting = false;

    public float GravityForce = 981f;  // For when we are changing gravity. It's the strength that can be changed through code/inspector.
    [Header("Weapon")]
    public Transform WeaponAttachHandTransform;
    public WeaponBase CurrentWeaponEquipped;
    [NonSerialized] public bool IsAimingWeapon = false;

    [Header("Health")]
    float Health;
    public float MaxHealth = 100f;
    [NonSerialized] public bool HasDied = false;

    public virtual void Start()
    {
        Health = MaxHealth;
        CurrentMovementSpeed = MovementSpeed;

    }

    public virtual void Awake()
    {
        // this.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public virtual void Update()
    {
        //CurrentMovementSpeed = Mathf.MoveTowards(CurrentMovementSpeed, IsSprinting ? MovementSpeedSprint : MovementSpeed, Time.deltaTime * MovementSpeedChangeSpeed);
        // Debug.Log(CurrentMovementSpeed.ToString());
    }

    public virtual void StartSprint()
    {
        IsSprinting = true;
        CurrentMovementSpeed = this.MovementSpeedSprint;
    }

    public virtual void StopSprint()
    {
        IsSprinting = false;
        CurrentMovementSpeed = this.MovementSpeed;
    }
   
    public virtual void StopShootingWeapon()
    {
        if (CurrentWeaponEquipped == null) return;
        CurrentWeaponEquipped.StopShoot();
    }
    public virtual void AimWeapon(bool IsAiming) {
        IsAimingWeapon = IsAiming;
    }


    public virtual void Reload()
    {

    }

    public virtual void ReloadComplete()
    {

    }

    public virtual void PlayReloadAnimation()
    {
        
    }

    public virtual int GetCurrentWeaponId() { return CurrentWeaponEquipped == null ? -1 : CurrentWeaponEquipped.WeaponId; }

    public float GetCapsuleCollisionRadius() { return this.CapsuleCollision.radius; }
    public float GetCapsuleCollisionHeight() { return this.CapsuleCollision.height; }

    public float GetCurrentHealth() { return this.Health; }

    public virtual void Die()
    {
        Debug.LogWarning("Dying...");
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
        Debug.LogWarning("New Health: " +  this.Health);
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

    public bool IsAlive() { return this.Health > 0; }

    public bool IsOnLowHealth() { return this.Health < 30f; }

    public bool HasWeaponEquipped() { return CurrentWeaponEquipped != null; }

    public virtual void Attack()
    {
        // Gets Derrived by child classes.
    }

    public void SetHealthDirectly(float InHealth)
    {
        this.Health = Mathf.Clamp(InHealth, 0, 100);
    }

    public void AddHealth(float InHealth)
    {
        this.Health = Mathf.Clamp(this.Health+InHealth, 0, 100);
        // Used for consumables.
    }
}
