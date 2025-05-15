using UnityEngine;

public class InteractablePickable : InteractableBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int Amount = 1;  // For things like weapon ammo and etc.
    public int MaxAmountAllowed = -1;  // If less than equal 0, it will be infinite.
    public bool IsInstantPickup = false;  // If true, player has to walk on it.
    [Header("Components")]
    public Rigidbody RigidbodyRef;

    public bool CanStockUpInfiniteAmount() { return MaxAmountAllowed <= 0; }

    public override bool Interact(PlayerCharacter PlayerCharacterRef)
    {
        base.Interact(PlayerCharacterRef);
        PlayerCharacterRef.PickupInteractable(this);
        return true;
    }

    public bool AddPickable(int AmountToAdd) {
        if (!CanStockUpInfiniteAmount() && Amount == MaxAmountAllowed)
            return false;
        Amount += AmountToAdd;
        if (!CanStockUpInfiniteAmount() && Amount > MaxAmountAllowed)
            Amount = MaxAmountAllowed;
        return true;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
