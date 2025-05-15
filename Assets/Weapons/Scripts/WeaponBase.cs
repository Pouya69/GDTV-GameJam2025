using System;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponBase : InteractablePickable
{
    [Header("Components")]
    public GameObject BulletClass;  // The bullet prefab we shoot/spawn.
    [Header("Weapon")]
    public int CurrentBulletsInMagazine;
    public int MaxBulletsInMagazine;
    public int MaxBulletsAllowed = 80;
    public int BulletsLeft;  // Max bullets left (does not include CurrentBulletsInMagazine)
    public int BulletsShootingOneShot = 1;  // Things like double barrel shotgun or burst rifles and etc.
    public float BulletVelocityBase = 800f;  // When spawning a bullet, how fast it should go.
    [NonSerialized] private CharacterBase OwnerCharacterRef;
    [NonSerialized] public bool IsReloading;
    [NonSerialized] public int BulletsAddingAfterAnimation = 0;  // After the animation is done.
    [DoNotSerialize] public bool CanShoot = true;
    public float WeaponFireRate = 1f;
    [NonSerialized] private float TimePassedSinceLastShot = 0f;
    public Transform ShootLocation_TEST_ONLY;  // Hopefully we will have a skeleton with a reload animation...
    [NonSerialized] public bool IsShooting = false;

    public void InitializeWeapon(CharacterBase InOwnerCharacterRef)
    {
        this.OwnerCharacterRef = InOwnerCharacterRef;
    }
    // [Header("Components")]
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // CurrentBulletsInMagazine = MaxBulletsInMagazine;
    }

    // Update is called once per frame
    void Update()
    {
        if (OwnerCharacterRef == null || WeaponFireRate == 1f || !IsShooting) return;
        TimePassedSinceLastShot += Time.deltaTime;
        if (TimePassedSinceLastShot >= 1/WeaponFireRate)
        {
            if (IsShooting)
                Shoot();
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
        InitializeWeapon(CharacterRef);
        this.RigidbodyRef.freezeRotation = false;
        this.RigidbodyRef.useGravity = false;
        this.RigidbodyRef.detectCollisions = false;
        this.RigidbodyRef.isKinematic = true;
        this.RigidbodyRef.linearVelocity = Vector3.zero;
        this.RigidbodyRef.angularVelocity = Vector3.zero;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        CharacterRef.CurrentWeaponEquipped = this;
        // Stuff like collision and stuff
    }

    public void RemovedWeaponToCharacter()
    {
        this.RigidbodyRef.freezeRotation = true;
        this.OwnerCharacterRef.CurrentWeaponEquipped = null;
        this.OwnerCharacterRef = null;
        this.RigidbodyRef.isKinematic = false;
        this.RigidbodyRef.useGravity = true;
        this.RigidbodyRef.detectCollisions = true;
        this.RigidbodyRef.linearVelocity = Vector3.zero;
        this.RigidbodyRef.angularVelocity = Vector3.zero;
        // Stuff like collision and stuff
    }

    public bool IsCurrentMagazineEmpty() { return CurrentBulletsInMagazine == 0; }

    public bool Reload()
    {
        if (IsReloading) return true;
        if (BulletsLeft <= 0)
        {
            PlayEmptySound();
            return false;
        }
        if (CurrentBulletsInMagazine >= MaxBulletsInMagazine) return true;  // Already max ammo
        BulletsAddingAfterAnimation = MaxBulletsInMagazine - CurrentBulletsInMagazine;
        if (CurrentBulletsInMagazine <= 0)  // If 0 bullets left, we make it reload with 1 less bullet than max bullets in a mag.
            BulletsAddingAfterAnimation--;
        CurrentBulletsInMagazine = 0;
        IsReloading = true;
        PlayReloadAnimation();
        return true;

    }

    // For when the reload animation is done we finish it
    public void ReloadCompleted()
    {
        CurrentBulletsInMagazine += BulletsAddingAfterAnimation;
        BulletsLeft -= BulletsAddingAfterAnimation;
        BulletsAddingAfterAnimation = 0;
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
    }

    public void PlayEmptySound()
    {

    }

    public void StartShooting() { IsShooting = true; }

    public void StopShoot() { IsShooting = false; }

    // ONLY CALL FROM THE FIRERATE UNLESS SINGLE SHOT
    public void Shoot()
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
            if (CurrentBulletsInMagazine == 0)
            {
                StopShoot();
                break;
            }
            Vector3 BulletSpawnLocation = ShootLocation_TEST_ONLY.position;  // TODO
            Quaternion BulletSpawnRotation = ShootLocation_TEST_ONLY.rotation;  // TODO
            GameObject BulletSpawned = Instantiate(BulletClass, BulletSpawnLocation, BulletSpawnRotation);
            if (BulletSpawned == null)
            {
                Debug.LogError("Bullet did not spawn.");
                return;
            }
            BulletBase BulletComponentOnObject = BulletSpawned.GetComponent<BulletBase>();
            BulletComponentOnObject.InitializeBullet(OwnerCharacterRef, default, ShootLocation_TEST_ONLY.forward * BulletVelocityBase, 1f);
            CurrentBulletsInMagazine--;
        }
    }
}
