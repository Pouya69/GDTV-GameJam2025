using UnityEngine;

public class BossGravityField : GravityField
{
    [Header("Boss")]
    public float CatchDistance = 4f;
    public BossCharacter SelfBossRef;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        IsSuckingField = true;
        // base.Start();
    }

    public override void FixedUpdate()
    {
        if (!this.enabled) return;
        base.FixedUpdate();
    }

    public override void UpdateOverlappingObjects()
    {
        if (!this.enabled) return;
        // base.UpdateOverlappingObjects();
        foreach (PhysicsObjectBasic ObjectOverlapping in PhysicsObjectsInsideField)
        {
            if (ObjectOverlapping == null) continue;
            // ObjectOverlapping.BaseGravity = this.IsSuckingField ? GetObjectForceTowardsMe(ObjectOverlapping) : this.FieldGravity;
            ObjectOverlapping.SetGravityForceAndDirection(this.IsSuckingField ? GetObjectForceTowardsMe(ObjectOverlapping) : this.FieldGravity, !IsGravityPermanent);  // Instant
            if (!ObjectOverlapping.RigidbodyRef.isKinematic && Vector3.Distance(this.transform.position, ObjectOverlapping.transform.position) <= CatchDistance)
                SelfBossRef.CaughtPhysicsObjectSuck(ObjectOverlapping);
        }
    }

    protected override void ResetPhysicsObject(PhysicsObjectBasic PhysicsObject)
    {
        base.ResetPhysicsObject(PhysicsObject);
    }

    public override void CharacterEntered(EnemyBaseCharacter Character)
    {
        // base.CharacterEntered(Character);
    }

    protected override void ResetCharacter(EnemyBaseCharacter Character)
    {
        // base.ResetCharacter(Character);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!this.enabled || other.CompareTag("Boss") || other.attachedRigidbody.isKinematic) return;
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (!this.enabled || other.CompareTag("Boss") || other.attachedRigidbody.isKinematic) return;
        base.OnTriggerExit(other);
    }

    public void TurnOnField() {
        SelfBossRef.CapsuleCollision.enabled = false;
        SphereOverlapArea.enabled = true;
        this.enabled = true;
    }

    public void TurnOffField()
    {
        SelfBossRef.CapsuleCollision.enabled = true;
        PhysicsObjectsInsideField.Clear();
        SphereOverlapArea.enabled = false;
        this.enabled = false;
    }

}
