using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryComponent : MonoBehaviour
{
    [DoNotSerialize] List<WeaponBase> InventoryWeapons;
    private void Awake()
    {
        this.enabled = false;
    }

    public void AddWeapon(WeaponBase WeaponToAdd, int BulletsToAddByDefault=5) {
        WeaponBase WeaponHaveAlready = this.FindWeapon(WeaponToAdd);
        if (WeaponHaveAlready == null)
        {
            InventoryWeapons.Add(WeaponToAdd);
            WeaponToAdd.AddedWeaponToCharacter();
        }
        else  // If we have the weapon already we add bullets
            WeaponHaveAlready.AddBullets(BulletsToAddByDefault);
    }

    public void RemoveWeapon(WeaponBase WeaponToRemoveRef)
    {
        WeaponBase WeaponToRemove = FindWeapon(WeaponToRemoveRef);
        if (WeaponToRemove == null)
            return;
        WeaponToRemove.RemovedWeaponToCharacter();
        InventoryWeapons.Remove(WeaponToRemove);
    }

    public void RemoveWeapon(int WeaponToRemoveIndex)
    {
        if (WeaponToRemoveIndex > InventoryWeapons.Count - 1)
            return;
        WeaponBase WeaponToRemove = InventoryWeapons[WeaponToRemoveIndex];
        WeaponToRemove.RemovedWeaponToCharacter();
        InventoryWeapons.Remove(WeaponToRemove);
    }

    public int FindWeaponIndex(WeaponBase WeaponToCheck)
    {
        for (int i = 0; i < InventoryWeapons.Count; i++)
        {
            if (WeaponToCheck.WeaponName.Equals(InventoryWeapons[i].WeaponName))
                return i;
        }
        return -1;
    }

    public WeaponBase FindWeapon(WeaponBase WeaponToCheck)
    {
        for (int i = 0; i < InventoryWeapons.Count; i++)
        {
            WeaponBase Weapon = InventoryWeapons[i];
            if (WeaponToCheck.WeaponName.Equals(Weapon.WeaponName))
                return Weapon;
        }
        return null;
    }
}
