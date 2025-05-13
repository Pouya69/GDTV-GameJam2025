using Unity.VisualScripting;
using UnityEngine;

public class BulletBase : PhysicsObjectBasic
{
    [Header("Bullet")]
    public float BaseDamage = 20f;  // Wihtout time dilation 
    [DoNotSerialize] private CharacterBase OwnerCharacterRef;

    // When we call Shoot() on weapon, we create a new bullet and we initialize it here.
    public void InitializeBullet(CharacterBase InOwnerCharacterRef, Vector3 Gravity=new Vector3(), Vector3 Velocity=new Vector3(), float InCustomTimeDilation=1f)
    {
        base.InitializePhysicsObject(Gravity, Velocity, InCustomTimeDilation);
        this.OwnerCharacterRef = InOwnerCharacterRef;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        
    }

    // For when time is stopped, we do not give damage
    public bool CanDamageOrAffect() { return this.CustomTimeDilation > 0f; }

    public override void UpdatePhysicsObjectBasedOnTimeDilation()
    {
        base.UpdatePhysicsObjectBasedOnTimeDilation();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null || !this.CanDamageOrAffect() || collision.gameObject == null) return;
        CharacterBase CharacterHit = collision.gameObject.GetComponent<CharacterBase>();
        if (CharacterHit == null)
        {
            PhysicsObjectBasic PhysicsObjectHit = collision.gameObject.GetComponent<PhysicsObjectBasic>();
            BulletBase AnotherBulletHit = collision.gameObject.GetComponent<BulletBase>();  // For when we hit another bullet coincidentally.
            if (AnotherBulletHit != null) return;
            if (PhysicsObjectHit == null)
            {
                Destroy(this);
                return;
            }
            HandlePhysicsObjectHit(PhysicsObjectHit);
        }
        else
            HandleCharacterHit(CharacterHit);
        Destroy(this);
    }

    public float GetDamage() { return this.BaseDamage * CustomTimeDilation; }

    public void HandlePhysicsObjectHit(PhysicsObjectBasic PhysicsObjectHit)
    {
        // Handling a physics object hit
    }

    public void HandleCharacterHit(CharacterBase CharacterHit)
    {
        if (CharacterHit == OwnerCharacterRef) return;  // If the owner of the bullet (the person who shot the bullet) was hit nothing happens.
        CharacterHit.ReduceHealth(CharacterBase.EDamageType.BULLET, GetDamage());
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public CharacterBase GetOwnerCharacter() { return this.OwnerCharacterRef; }
}
