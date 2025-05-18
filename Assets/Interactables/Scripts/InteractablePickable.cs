using UnityEngine;

public class InteractablePickable : InteractableBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int Amount = 1;  // For things like weapon ammo and etc.
    public int MaxAmountAllowed = -1;  // If less than equal 0, it will be infinite.
    public bool IsInstantPickup = false;  // If true, player has to walk on it.
    [Header("Components")]
    public PhysicsObjectBasic PhysicsObjectComponent = null;
    public GameObject ObjectToGivePlayer = null;  // Things like grenades.
    public GameObject ObjectMesh;
    public Collider ObjectCollider;

    public bool CanStockUpInfiniteAmount() { return MaxAmountAllowed <= 0; }

    public override bool Interact(PlayerCharacter PlayerCharacterRef)
    {
        base.Interact(PlayerCharacterRef);
        PlayerCharacterRef.PickupInteractable(this);
        return true;
    }

    public virtual bool AddPickable(int AmountToAdd) {
        if (!CanStockUpInfiniteAmount() && Amount == MaxAmountAllowed)
            return false;
        Amount += AmountToAdd;
        if (!CanStockUpInfiniteAmount() && Amount > MaxAmountAllowed)
            Amount = MaxAmountAllowed;
        return true;
    }

    public virtual bool ReduceAmount(int AmountToReduce)
    {
        this.Amount -= AmountToReduce;
        return this.Amount <= 0;

    }

    public virtual void DestroyAndRemoveFromInventory()
    {
        Destroy(gameObject);
    }

    public virtual void JustAddedToInventory(CharacterBase CharacterBaseRef)
    {
        this.PhysicsObjectComponent.RigidbodyRef.detectCollisions = false;
        gameObject.layer = 0;
        Destroy(this.ObjectCollider);
        Destroy(this.PhysicsObjectComponent.RigidbodyRef);
        Destroy(this.PhysicsObjectComponent);
        Destroy(this.ObjectMesh);
        gameObject.transform.SetParent(CharacterBaseRef.CapsuleCollision.transform, false);
    }
}
