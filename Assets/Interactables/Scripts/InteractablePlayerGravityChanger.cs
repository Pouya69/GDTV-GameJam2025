using UnityEngine;

public class InteractablePlayerGravityChanger : InteractableBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Gets the gravity direction based on how the gameobject of this component is rotated.
    public Vector3 GetPlayerNewGravity() { return (gameObject.transform.rotation * Vector3.down).normalized; }

    public override bool Interact(PlayerCharacter PlayerCharacterRef)
    {
        PlayerCharacterRef.MyPlayerController.SetGravityForceAndDirection(GetPlayerNewGravity());
        return base.Interact(PlayerCharacterRef);
    }
}
