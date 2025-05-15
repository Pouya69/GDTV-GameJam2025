using UnityEngine;

public class InteractableBase : MonoBehaviour
{
    public float InteractionSpeed = 70f;
    public string InteractableName = "ITEM_TEST_NAME";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    public virtual bool Interact(PlayerCharacter PlayerCharacterRef)
    {
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
