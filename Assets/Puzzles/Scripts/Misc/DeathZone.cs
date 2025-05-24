using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.transform.root.gameObject.TryGetComponent<PlayerCharacter>(out PlayerCharacter PlayerRef)) return;
        PlayerRef.Die();
    }
}
