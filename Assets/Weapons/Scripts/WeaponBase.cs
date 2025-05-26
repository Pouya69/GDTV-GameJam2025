using System;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponBase : InteractablePickable
{
    [Header("Components")]
    public GameObject BulletClass;  // The bullet prefab we shoot/spawn.
    [Header("Weapon")]
    public int WeaponId = 0;  // This is for the animations.
    public int CurrentBulletsInMagazine;
    public int MaxBulletsInMagazine;
    public int MaxBulletsAllowed = 80;
    public int BulletsLeft;  // Max bullets left (does not include CurrentBulletsInMagazine)
    public int BulletsShootingOneShot = 1;  // Things like double barrel shotgun or burst rifles and etc.
    public float BulletVelocityBase = 800f;  // When spawning a bullet, how fast it should go.
    public Transform WeaponGrabTransform;
    [NonSerialized] private CharacterBase OwnerCharacterRef;
    [NonSerialized] private bool IsInfiniteAmmo = false;  // For enemies it is infinite
    [NonSerialized] public bool IsReloading;
    [NonSerialized] public int BulletsAddingAfterAnimation = 0;  // After the animation is done.
    [DoNotSerialize] public bool CanShoot = true;
    public float WeaponFireRate = 1f;
    [NonSerialized] private float TimePassedSinceLastShot = 0f;
    public Transform ShootLocation_TEST_ONLY;  // Hopefully we will have a skeleton with a reload animation...
    [NonSerialized] public bool IsShooting = false;
    public Vector3 AttachmentOffset = Vector3.zero;
    public Vector3 AttachmentOffsetRotation = Vector3.zero;
    [Header("Reload")]
    public GameObject MagazineAttachmentTo;
    public GameObject WeaponMagazine;
    public Vector3 MagazineAttachmentOffsetRotation;
    public Vector3 MagazineAttachmentOffsetPosition;

    public void InitializeWeapon(CharacterBase InOwnerCharacterRef)
    {
        this.OwnerCharacterRef = InOwnerCharacterRef;
    }
    // [Header("Components")]
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MagazineAttachmentOffsetRotation = WeaponMagazine.transform.localRotation.eulerAngles;
        MagazineAttachmentOffsetPosition = WeaponMagazine.transform.localPosition;
        // CurrentBulletsInMagazine = MaxBulletsInMagazine;
    }

    // Update is called once per frame
    void Update()
    {
        if (OwnerCharacterRef == null || WeaponFireRate == 1f || !IsShooting) return;
        TimePassedSinceLastShot += Time.deltaTime;
        if (TimePassedSinceLastShot >= 1/WeaponFireRate)
        {
            Shoot(OwnerCharacterRef.MyController.GetForwardShootingVector());
            TimePassedSinceLastShot = 0f;
        }
    }

    public bool IsWeaponSingleShot() { return WeaponFireRate == 1f; }

    public void AddBullets(int BulletsToAdd)
    {
        BulletsLeft += BulletsToAdd;
        if (IsCurrentMagazineEmpty())
            Reload();
    }

    public void AddedWeaponToCharacter(CharacterBase CharacterRef)
    {
        this.WeaponGrabTransform.transform.SetParent(CharacterRef.WeaponAttachHandTransform, false);
        IsInfiniteAmmo = CharacterRef.TryGetComponent<EnemyBaseCharacter>(out _);  // Enemies can shoot infinitely.
        PhysicsObjectComponent.RigidbodyRef.freezeRotation = false;
        PhysicsObjectComponent.RigidbodyRef.useGravity = false;
        PhysicsObjectComponent.RigidbodyRef.detectCollisions = false;
        PhysicsObjectComponent.RigidbodyRef.linearVelocity = Vector3.zero;
        PhysicsObjectComponent.RigidbodyRef.angularVelocity = Vector3.zero;
        PhysicsObjectComponent.RigidbodyRef.isKinematic = true;
        this.WeaponGrabTransform.transform.SetLocalPositionAndRotation(AttachmentOffset, Quaternion.Euler(AttachmentOffsetRotation));
        CharacterRef.CurrentWeaponEquipped = this;
        InitializeWeapon(CharacterRef);
        // Stuff like collision and stuff
    }

    public void RemovedWeaponToCharacter()
    {
        PhysicsObjectComponent.RigidbodyRef.freezeRotation = true;
        this.OwnerCharacterRef.CurrentWeaponEquipped = null;
        this.OwnerCharacterRef = null;
        PhysicsObjectComponent.RigidbodyRef.isKinematic = false;
        PhysicsObjectComponent.RigidbodyRef.useGravity = true;
        PhysicsObjectComponent.RigidbodyRef.detectCollisions = true;
        PhysicsObjectComponent.RigidbodyRef.linearVelocity = Vector3.zero;
        PhysicsObjectComponent.RigidbodyRef.angularVelocity = Vector3.zero;
        // Stuff like collision and stuff
    }

    public bool IsCurrentMagazineEmpty() { return !IsInfiniteAmmo && CurrentBulletsInMagazine <= 0; }

    public bool Reload()
    {
        if (IsReloading) return true;
        if (BulletsLeft <= 0)
        {
            PlayEmptySound();
            return false;
        }
        if (BulletsLeft <= 0) return false;
        
        IsReloading = true;
        PlayReloadAnimation();
        RuntimeManager.PlayOneShot("event:/SFX_Reload", transform.position);
        return true;
    }

    // For when the reload animation is done we finish it
    public void ReloadCompleted()
    {
        int Add = Mathf.Abs(MaxBulletsInMagazine - CurrentBulletsInMagazine);
        if (Add > BulletsLeft)
            Add = BulletsLeft;
        CurrentBulletsInMagazine += Add;
        BulletsLeft -= Add;
        IsReloading = false;
    }

    public void CancelReload()
    {
        // When the player cancels reloading.
        IsReloading = false;
    }

    public void PlayReloadAnimation()
    {
        // TODO: Implement Reload Animation for owner.
        this.OwnerCharacterRef.PlayReloadAnimation();
    }

    public void PlayEmptySound()
    {

    }

    public void StartShooting() {
        if (this.IsWeaponSingleShot())
            Shoot(OwnerCharacterRef.MyController.GetForwardShootingVector());
        else
            IsShooting = true;
    }

    public void StopShoot() { IsShooting = false; }

    // ONLY CALL FROM THE FIRERATE UNLESS SINGLE SHOT
    public void Shoot(Vector3 ForwardVector)
    {
        if (IsCurrentMagazineEmpty())
        {
            StopShoot();
            if (!IsReloading)
                Reload();
            return;
        }
        for (int i = 0; i < BulletsShootingOneShot; i++)
        {
            if (!IsInfiniteAmmo && CurrentBulletsInMagazine == 0)
            {
                StopShoot();
                break;
            }
            Vector3 BulletSpawnLocation = ShootLocation_TEST_ONLY.position;  // TODO
            GameObject BulletSpawned = Instantiate(BulletClass, BulletSpawnLocation, ShootLocation_TEST_ONLY.rotation);
            if (BulletSpawned == null)
            {
                Debug.LogError("Bullet did not spawn.");
                return;
            }
            // ShootLocation_TEST_ONLY.right
            BulletBase BulletComponentOnObject = BulletSpawned.GetComponent<BulletBase>();
            BulletComponentOnObject.InitializeBullet(OwnerCharacterRef, default, ForwardVector * BulletVelocityBase, 1f);
            RuntimeManager.PlayOneShot("event:/SFX_Gun", transform.position);
            Debug.DrawLine(ShootLocation_TEST_ONLY.position, ShootLocation_TEST_ONLY.position + (ForwardVector * BulletVelocityBase), Color.red);
            if (!IsInfiniteAmmo)
                CurrentBulletsInMagazine--;
        }
    }
}
