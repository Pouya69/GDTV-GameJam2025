using System;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [Header("Components")]
    public GameObject BulletClass;  // The bullet prefab we shoot/spawn.
    [Header("Weapon")]
    public int CurrentBulletsInMagazine;
    public int MaxBulletsInMagazine;
    public int BulletsLeft;  // Max bullets left (does not include CurrentBulletsInMagazine)
    public String WeaponName = "";
    [DoNotSerialize] private CharacterBase OwnerCharacterRef;
    [DoNotSerialize] public bool IsReloading;
    [DoNotSerialize] public int BulletsAddingAfterAnimation = 0;  // After the animation is done.

    public void InitializeWeapon(CharacterBase InOwnerCharacterRef)
    {
        this.OwnerCharacterRef = InOwnerCharacterRef;
    }
    // [Header("Components")]
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddBullets(int Magazines)
    {
        BulletsLeft += Magazines;
        if (IsCurrentMagazineEmpty())
            Reload();
    }

    public void AddedWeaponToCharacter()
    {
        // Stuff like collision and stuff
    }

    public void RemovedWeaponToCharacter()
    {
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

    public void Shoot()
    {
        if (IsCurrentMagazineEmpty())
        {
            Reload();
            return;
        }
        Vector3 BulletSpawnLocation = Vector3.zero;  // TODO
        Quaternion BulletSpawnRotation = Quaternion.identity;  // TODO
        GameObject BulletSpawned = Instantiate(BulletClass, BulletSpawnLocation, BulletSpawnRotation);
        if (BulletSpawned == null)
        {
            Debug.LogError("Bullet did not spawn.");
            return;
        }
        BulletBase BulletComponentOnObject = BulletSpawned.GetComponent<BulletBase>();
        BulletComponentOnObject.InitializeBullet(this.OwnerCharacterRef);
    }
}
