using UnityEngine;

public class InteractablePlayerGravityChanger : InteractableBase
{
    public float GravityAmount = 9.81f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Gets the gravity direction based on how the gameobject of this component is rotated.
    public Vector3 GetPlayerNewGravity() { return (gameObject.transform.rotation * Vector3.down).normalized * GravityAmount; }

    public override bool Interact(PlayerCharacter PlayerCharacterRef)
    {
        Debug.Log("Changed Gravity For Player");
        PlayerCharacterRef.MyPlayerController.SetGravityForceAndDirection(GetPlayerNewGravity());
        return base.Interact(PlayerCharacterRef);
    }
}
