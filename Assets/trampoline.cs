using UnityEngine;
using FMODUnity;
public class trampoline : MonoBehaviour
{
    public PlayerCharacter character;
    public void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.root.gameObject.CompareTag("Player")) return;
        if (character.MyPlayerController.IsOnGround) return;
        character.MyPlayerController.RigidbodyRef.AddForce(-character.MyPlayerController.GetGravityDirection() * character.BoostUpForce);
        RuntimeManager.PlayOneShot("event:/SFX_Jumps", transform.position);
    }
}
