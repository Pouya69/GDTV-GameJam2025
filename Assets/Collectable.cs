using UnityEngine;
using FMODUnity;
public class Collectable : MonoBehaviour
{
    public enum Collectableitems
    {
        Health,
        Ammo
    }
    private PlayerCharacter character;
    public Collectableitems itemType;
    public float Health = 25f;
    public int Ammo = 20;

    public void Start()
    {
        character = GameObject.FindWithTag("Player").GetComponent<PlayerCharacter>();
    }
    public void Update()
    {
        transform.Rotate(0,10 * Time.deltaTime,0);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.CompareTag("Player")) return;
        if(itemType == Collectableitems.Health) character.SetHealthDirectly(character.GetCurrentHealth() + Health);
        else
        {
            if (character.CurrentWeaponEquipped == null) return;
            character.CurrentWeaponEquipped.BulletsLeft += Ammo;
        }
        RuntimeManager.PlayOneShot("event:/SFX_Pickup", transform.position);
        Destroy(gameObject);
    }
}
