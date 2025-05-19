using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryComponent : MonoBehaviour
{
    [NonSerialized] List<WeaponBase> InventoryWeapons = new List<WeaponBase>();
    [NonSerialized] List<InteractablePickable> InventoryItems = new List<InteractablePickable>();
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
            if (IsWeapon)
            {
                bool AddedWeaponOrAmmo = AddWeapon(CharacterBaseRef, CharacterBaseRef.WeaponAttachHandTransform, ItemWeapon, interactablePickable.Amount);
                //if (AddedWeaponOrAmmo)
                //Debug.LogWarning("Picked up wepaon: " + ItemWeapon.InteractableName);
                return true;
            }
        }
        InteractablePickable AlreadyExisitngItem = FindItem(interactablePickable);
        if (AlreadyExisitngItem == null)
        {
            InventoryItems.Add(interactablePickable);
            interactablePickable.JustAddedToInventory(CharacterBaseRef);
            //Debug.LogError("WORKS ADDED.");
        }
        else
        {
            bool IsAlreadyFull = !AlreadyExisitngItem.AddPickable(interactablePickable.Amount);
            if (IsAlreadyFull)
                return false;
            //Debug.LogError("WORKS ADDED ALREADY THERE. AMOUNT now: " + AlreadyExisitngItem.Amount);
            Destroy(interactablePickable.gameObject);
        }
        return true;
    }

    public bool AddWeapon(CharacterBase CharacterBaseRef, Transform WeaponAttachHandTransform, WeaponBase WeaponToAdd, int BulletsToAddByDefault=5) {
        WeaponBase WeaponHaveAlready = this.FindWeapon(WeaponToAdd);
        if (WeaponHaveAlready == null)
        {
            InventoryWeapons.Add(WeaponToAdd);
            WeaponToAdd.AddedWeaponToCharacter(CharacterBaseRef);
            return true;
        }
        else
        {
            if (WeaponHaveAlready.gameObject.Equals(WeaponToAdd.gameObject)) return false;
            // If we have the weapon already we add bullets
            WeaponHaveAlready.AddBullets(BulletsToAddByDefault);
            Destroy(WeaponToAdd.gameObject);
            return false;
        }
            
    }

    public WeaponBase RemoveWeapon(CharacterBase CharacterBaseRef, WeaponBase WeaponToRemoveRef)
    {
        WeaponBase WeaponToRemove = FindWeapon(WeaponToRemoveRef);
        if (WeaponToRemove == null)
            return null;
        WeaponToRemove.transform.SetParent(null, true);
        WeaponToRemove.RemovedWeaponToCharacter();
        InventoryWeapons.Remove(WeaponToRemove);
        return WeaponToRemove;
    }

    public WeaponBase RemoveWeapon(CharacterBase CharacterBaseRef, int WeaponToRemoveIndex)
    {
        if (WeaponToRemoveIndex > InventoryWeapons.Count - 1 || WeaponToRemoveIndex == -1)
            return null;
        WeaponBase WeaponToRemove = InventoryWeapons[WeaponToRemoveIndex];
        WeaponToRemove.transform.SetParent(null, true);
        WeaponToRemove.RemovedWeaponToCharacter();
        InventoryWeapons.Remove(WeaponToRemove);
        return WeaponToRemove;
    }

    public GameObject RemoveItem(CharacterBase CharacterBaseRef, InteractablePickable ItemToRemoveRef, int AmountToRemove=1)
    {
        InteractablePickable ItemToRemove = FindItem(ItemToRemoveRef);
        if (ItemToRemove == null)
            return null;
        if (AmountToRemove > ItemToRemove.Amount)
            AmountToRemove = ItemToRemove.Amount;
        bool ShouldDeleteFromInventory = ItemToRemove.ReduceAmount(AmountToRemove);
        GameObject PrefabToSpawn = ItemToRemove.ObjectToGivePlayer;
        if (ShouldDeleteFromInventory)
        {
            ItemToRemove.DestroyAndRemoveFromInventory();
            InventoryItems.Remove(ItemToRemove);
        }
        else
        {
            // Todo: Drop the item
        }
        return PrefabToSpawn;
    }

    public GameObject RemoveItem(CharacterBase CharacterBaseRef, string ItemName, int AmountToRemove = 1)
    {
        InteractablePickable ItemToRemove = FindItem(ItemName);
        if (ItemToRemove == null)
            return null;
        if (AmountToRemove > ItemToRemove.Amount)
            AmountToRemove = ItemToRemove.Amount;
        bool ShouldDeleteFromInventory = ItemToRemove.ReduceAmount(AmountToRemove);
        // Debug.Log("Amount after remove: " + ItemToRemove.Amount);
        GameObject PrefabToSpawn = ItemToRemove.ObjectToGivePlayer;
        if (ShouldDeleteFromInventory)
        {
            ItemToRemove.DestroyAndRemoveFromInventory();
            InventoryItems.Remove(ItemToRemove);
        }
        else
        {
            // Todo: Drop the item
        }
        return PrefabToSpawn;
    }

    public GameObject RemoveItem(CharacterBase CharacterBaseRef, int ItemToRemoveIndex, int AmountToRemove = 1)
    {
        if (ItemToRemoveIndex > InventoryItems.Count - 1 || ItemToRemoveIndex == -1)
            return null;
        InteractablePickable ItemToRemove = InventoryItems[ItemToRemoveIndex];
        bool ShouldDeleteFromInventory = ItemToRemove.ReduceAmount(AmountToRemove);
        GameObject PrefabToSpawn = ItemToRemove.ObjectToGivePlayer;
        if (ShouldDeleteFromInventory)
        {
            ItemToRemove.DestroyAndRemoveFromInventory();
            InventoryItems.Remove(ItemToRemove);
        }
        else
        {
            // Todo: Drop the item
        }
        return PrefabToSpawn;
    }

    public int FindWeaponIndex(string WeaponName)
    {
        for (int i = 0; i < InventoryWeapons.Count; i++)
        {
            if (WeaponName.Equals(InventoryWeapons[i].InteractableName))
                return i;
        }
        return -1;
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

    public int FindItemIndex(string ItemName)
    {
        for (int i = 0; i < InventoryItems.Count; i++)
        {
            if (ItemName.Equals(InventoryItems[i].InteractableName))
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

    public InteractablePickable FindItem(string ItemName)
    {
        for (int i = 0; i < InventoryItems.Count; i++)
        {
            InteractablePickable Interactable = InventoryItems[i];
            if (ItemName.Equals(Interactable.InteractableName))
                return Interactable;
        }
        return null;
    }
}
