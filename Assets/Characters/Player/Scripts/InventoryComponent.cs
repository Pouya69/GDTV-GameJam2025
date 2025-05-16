using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryComponent : MonoBehaviour
{
    [NonSerialized] List<WeaponBase> InventoryWeapons = new List<WeaponBase>();
    [NonSerialized] List<InteractablePickable> InventoryItems = new List<InteractablePickable>();
    public Transform WeaponAttachHandTransform;
    private void Awake()
    {
        this.enabled = false;
    }

    public bool AddItemToInventory(CharacterBase CharacterBaseRef, InteractablePickable interactablePickable, bool CheckIfIsWeapon = true)
    {
        if (CheckIfIsWeapon)
        {
            WeaponBase ItemWeapon;
            bool IsWeapon = interactablePickable.TryGetComponent<WeaponBase>(out ItemWeapon);
            if (IsWeapon && ItemWeapon != null)
            {
                bool AddedWeaponOrAmmo = AddWeapon(CharacterBaseRef, ItemWeapon, interactablePickable.Amount);
                if (!AddedWeaponOrAmmo)
                    interactablePickable.Amount = 0;
                Debug.LogWarning("Picked up wepaon: " + ItemWeapon.InteractableName);
                return true;
            }
        }
        InteractablePickable AlreadyExisitngItem = FindItem(interactablePickable);
        if (AlreadyExisitngItem == null)
            InventoryItems.Add(interactablePickable);
        else
        {
            bool IsAlreadyFull = !interactablePickable.AddPickable(interactablePickable.Amount);
            if (IsAlreadyFull)
                return false;
        }
        return true;
    }

    public bool AddWeapon(CharacterBase CharacterBaseRef, WeaponBase WeaponToAdd, int BulletsToAddByDefault=5) {
        WeaponBase WeaponHaveAlready = this.FindWeapon(WeaponToAdd);
        if (WeaponHaveAlready == null)
        {
            InventoryWeapons.Add(WeaponToAdd);
            WeaponToAdd.transform.SetParent(WeaponAttachHandTransform, false);
            WeaponToAdd.AddedWeaponToCharacter(CharacterBaseRef);
            return true;
        }
        else
        {
            // If we have the weapon already we add bullets
            WeaponHaveAlready.AddBullets(BulletsToAddByDefault);
            Destroy(WeaponToAdd.gameObject);
            return false;
        }
            
    }

    public void RemoveWeapon(CharacterBase CharacterBaseRef, WeaponBase WeaponToRemoveRef)
    {
        WeaponBase WeaponToRemove = FindWeapon(WeaponToRemoveRef);
        if (WeaponToRemove == null)
            return;
        WeaponToRemove.transform.SetParent(null, true);
        WeaponToRemove.RemovedWeaponToCharacter();
        InventoryWeapons.Remove(WeaponToRemove);
    }

    public void RemoveWeapon(CharacterBase CharacterBaseRef, int WeaponToRemoveIndex)
    {
        if (WeaponToRemoveIndex > InventoryWeapons.Count - 1)
            return;
        WeaponBase WeaponToRemove = InventoryWeapons[WeaponToRemoveIndex];
        WeaponToRemove.transform.SetParent(null, true);
        WeaponToRemove.RemovedWeaponToCharacter();
        InventoryWeapons.Remove(WeaponToRemove);
    }

    public int FindWeaponIndex(WeaponBase WeaponToCheck)
    {
        for (int i = 0; i < InventoryWeapons.Count; i++)
        {
            if (WeaponToCheck.InteractableName.Equals(InventoryWeapons[i].InteractableName))
                return i;
        }
        return -1;
    }

    public WeaponBase FindWeapon(WeaponBase WeaponToCheck)
    {
        for (int i = 0; i < InventoryWeapons.Count; i++)
        {
            WeaponBase Weapon = InventoryWeapons[i];
            if (WeaponToCheck.InteractableName.Equals(Weapon.InteractableName))
                return Weapon;
        }
        return null;
    }

    public int FindItemIndex(InteractablePickable ItemToCheck)
    {
        for (int i = 0; i < InventoryItems.Count; i++)
        {
            if (ItemToCheck.InteractableName.Equals(InventoryItems[i].InteractableName))
                return i;
        }
        return -1;
    }

    public InteractablePickable FindItem(InteractablePickable InteractableToCheck)
    {
        for (int i = 0; i < InventoryItems.Count; i++)
        {
            InteractablePickable Interactable = InventoryItems[i];
            if (InteractableToCheck.InteractableName.Equals(Interactable.InteractableName))
                return Interactable;
        }
        return null;
    }
}
