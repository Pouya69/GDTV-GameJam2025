using UnityEngine;

public class PlayCutScene : MonoBehaviour
{
    public GameObject PrePicture;
    public GameObject RigPicture;

    public void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("GameController")) return;
        PrePicture.SetActive(false);
        RigPicture.SetActive(true);
    }
}
