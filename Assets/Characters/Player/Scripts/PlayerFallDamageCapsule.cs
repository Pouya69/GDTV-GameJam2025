using UnityEngine;

public class PlayerFallDamageCapsule : MonoBehaviour
{
    public PlayerCharacter PlayerRef;
    public string IgnoreDamageTag = "No Fall Damage";  // Trampoline and etc.
    public float ImpactThreshold = 15f;  // Before 4 ignore.
    public float MaxImpactDeath = 40f;  // greater or equal means death.
    public float MinDamageApplied = 10f;
    public float MaxDamageApplied = 150f;
    private void OnCollisionEnter(Collision collision)
    {
        // We already handle damage.
        if (collision.collider.CompareTag(IgnoreDamageTag) || collision.collider.CompareTag("Bullet")) return;
        float ImpactSpeed = Vector3.Project(collision.relativeVelocity, -PlayerRef.MyController.GetGravityDirection()).magnitude;
        //float ImpactSpeed = collision.relativeVelocity.magnitude;
        Debug.Log("Fall: " + ImpactSpeed);
        if (ImpactSpeed < ImpactThreshold) return;
        float DamageApplied = MinDamageApplied + ((MaxDamageApplied - MinDamageApplied) / (MaxImpactDeath - ImpactThreshold)) * (ImpactSpeed - ImpactThreshold);
        PlayerRef.ReduceHealth(CharacterBase.EDamageType.FALL_DAMAGE, DamageApplied);
    }


}
