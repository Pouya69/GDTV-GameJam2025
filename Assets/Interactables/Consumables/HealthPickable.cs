using UnityEngine;

public class HealthPickable : InteractablePickable
{

    public override bool AddPickable(int AmountToAdd)
    {
        return true;
        // return base.AddPickable(AmountToAdd);
    }

    public override bool Interact(PlayerCharacter PlayerCharacterRef)
    {
        // Debug.Log(this.InteractableName);
        PlayerCharacterRef.AddHealth(Amount);
        Destroy(gameObject);
        return true;
        // return base.Interact(PlayerCharacterRef);
    }
}
