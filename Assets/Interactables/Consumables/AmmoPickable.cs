using UnityEngine;

public class AmmoPickable : InteractablePickable
{

    public override bool AddPickable(int AmountToAdd)
    {
        return true;
        // return base.AddPickable(AmountToAdd);
    }

    public override bool Interact(PlayerCharacter PlayerCharacterRef)
    {
        // Debug.Log(this.InteractableName);
        if (!PlayerCharacterRef.HasWeaponEquipped()) return false;
        PlayerCharacterRef.CurrentWeaponEquipped.AddBullets(Amount);
        Destroy(gameObject);
        return true;
       // return base.Interact(PlayerCharacterRef);
    }
}
