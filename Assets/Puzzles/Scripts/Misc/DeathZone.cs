using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public PhysicsObjectBasic TimeDilationObj;
    public float Damage = 100f;
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.transform.root.gameObject.TryGetComponent<PlayerCharacter>(out PlayerCharacter PlayerRef)) return;
        if (TimeDilationObj == null)
            PlayerRef.Die();
        else
            PlayerRef.ReduceHealth(CharacterBase.EDamageType.DEFAULT, Damage * TimeDilationObj.CustomTimeDilation);
    }
}
