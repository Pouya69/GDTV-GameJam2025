using Unity.VisualScripting;
using UnityEngine;
using FMODUnity;
public class BulletBase : PhysicsObjectBasic
{
    [Header("Bullet")]
    public float BaseDamage = 20f;  // Wihtout time dilation 
    [DoNotSerialize] private CharacterBase OwnerCharacterRef;
    public float DestroyBulletAfterSeconds = 30;

    // When we call Shoot() on weapon, we create a new bullet and we initialize it here.
    public void InitializeBullet(CharacterBase InOwnerCharacterRef, Vector3 Gravity=new Vector3(), Vector3 Velocity=new Vector3(), float InCustomTimeDilation=1f)
    {
        base.InitializePhysicsObject(Gravity, Velocity, InCustomTimeDilation);
        this.OwnerCharacterRef = InOwnerCharacterRef;
        this.RigidbodyRef.linearVelocity = Velocity;
        this.BaseVelocity = Velocity;
        this.RigidbodyRef.linearDamping = 0;
        Destroy(gameObject, DestroyBulletAfterSeconds);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        CheckTimeDilationOnSpawn();
    }

    // For when time is stopped, we do not give damage
    public bool CanDamageOrAffect() { return this.CustomTimeDilation > 0f; }

    public override void UpdatePhysicsObjectBasedOnTimeDilation()
    {
        this.RigidbodyRef.linearVelocity = GetTimeScaledVelocity();// + (GetGravityForceTimeScaled() * Time.deltaTime);
        RigidbodyRef.AddForce(GetGravityForceTimeScaled());
        if (!IsInterpolatingTimeDilation()) return;
        this.CustomTimeDilation = Mathf.Lerp(this.CustomTimeDilation, this.CustomTimeDilationTarget, 1 - Mathf.Exp(-this.TimeDilationInterpSpeed * Time.deltaTime));
        if (Mathf.Abs(this.CustomTimeDilationTarget - this.CustomTimeDilation) < TimeDilationDifferenceIgnore)
            this.CustomTimeDilation = this.CustomTimeDilationTarget;
        this.transform.forward = this.RigidbodyRef.linearVelocity.normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!this.CanDamageOrAffect() || collision.gameObject.transform.root.TryGetComponent<FieldBase>(out _)) return;
        CharacterBase CharacterHit;
        bool IsCharacter = collision.gameObject.transform.root.TryGetComponent<CharacterBase>(out CharacterHit);
        if (!IsCharacter)
        {
            PhysicsObjectBasic PhysicsObjectHit;
            bool IsCustomPhysicsObject = collision.gameObject.transform.root.TryGetComponent<PhysicsObjectBasic>(out PhysicsObjectHit);
            BulletBase AnotherBulletHit;
            bool IsAnotherBullet = collision.gameObject.transform.root.TryGetComponent<BulletBase>(out AnotherBulletHit);  // For when we hit another bullet coincidentally.
            if (IsAnotherBullet) return;
            if (!IsCustomPhysicsObject)
            {
                Debug.Log("CUSTOMPHYSICS Bullet Collided with: " + collision.gameObject.name);
                Destroy(gameObject);
                return;
            }
            HandlePhysicsObjectHit(PhysicsObjectHit);
        }
        else
        {
            HandleCharacterHit(CharacterHit);
            return;
        }
        Debug.Log("DEFAULT Bullet Collided with: " + collision.gameObject.gameObject.name);
        RuntimeManager.PlayOneShot("event:/SFX_Gunshot Impact",transform.position);
        Destroy(gameObject);
    }

    public float GetDamage() { return this.BaseDamage * CustomTimeDilation; }

    public void HandlePhysicsObjectHit(PhysicsObjectBasic PhysicsObjectHit)
    {
        // Handling a physics object hit
    }

    public void HandleCharacterHit(CharacterBase CharacterHit)
    {
        if (CharacterHit.Equals(OwnerCharacterRef)) return;  // If the owner of the bullet (the person who shot the bullet) was hit nothing happens.
        CharacterHit.ReduceHealth(CharacterBase.EDamageType.BULLET, GetDamage());
        Debug.Log("CHARACTER Bullet Collided with: " + CharacterHit.gameObject.name);
        Destroy(gameObject);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        // TODO: Rotate bullet towards movement direction.
    }

    public CharacterBase GetOwnerCharacter() { return this.OwnerCharacterRef; }
}
