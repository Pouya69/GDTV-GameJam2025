using UnityEngine;

public class BossFightStarter : MonoBehaviour
{
    public BossCharacter BossCharacterRef;

    private void OnTriggerEnter(Collider other)
    {
        BossCharacterRef.enabled = true;
        BossCharacterRef.MyBossController.enabled = true;
        BossCharacterRef.MyBossController.MyNavAgent.enabled = true;
        BossCharacterRef.MyBossController.MyBehaviourTreeAgent.enabled = true;
        // Add stuff to do on boss fight start here.

        Destroy(this.gameObject);
    }
}
