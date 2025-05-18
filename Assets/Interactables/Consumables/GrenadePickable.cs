using UnityEngine;

public class GrenadePickable : InteractablePickable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public override bool AddPickable(int AmountToAdd)
    {
        return base.AddPickable(AmountToAdd);
    }

    public override bool Interact(PlayerCharacter PlayerCharacterRef)
    {
        // Debug.Log(this.InteractableName);
        return base.Interact(PlayerCharacterRef);
    }
}
