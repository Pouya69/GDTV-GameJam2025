using UnityEngine;

public class InteractablePlayerGravityChanger : InteractableBase
{
    public float GravityAmount = 9.81f;
    public Vector3 CustomGravityDir = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CustomGravityDir.Normalize();
    }

    // Gets the gravity direction based on how the gameobject of this component is rotated.
    public Vector3 GetPlayerNewGravity() { return CustomGravityDir.Equals(Vector3.zero) ? gameObject.transform.forward * GravityAmount : CustomGravityDir * GravityAmount; }

    public override bool Interact(PlayerCharacter PlayerCharacterRef)
    {
        Debug.Log("Changed Gravity For Player");
        PlayerCharacterRef.MyPlayerController.SetGravityForceAndDirection(GetPlayerNewGravity());
        return base.Interact(PlayerCharacterRef);
    }
}
