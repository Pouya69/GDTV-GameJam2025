using UnityEngine;

public class BossGravityField : GravityField
{
    [Header("Boss")]
    public float CatchDistance = 4f;
    public BossCharacter SelfBossRef;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void UpdateOverlappingObjects()
    {
        // base.UpdateOverlappingObjects();
        foreach (PhysicsObjectBasic ObjectOverlapping in PhysicsObjectsInsideField)
        {
            if (ObjectOverlapping == null) continue;
            // ObjectOverlapping.BaseGravity = this.IsSuckingField ? GetObjectForceTowardsMe(ObjectOverlapping) : this.FieldGravity;
            ObjectOverlapping.SetGravityForceAndDirection(this.IsSuckingField ? GetObjectForceTowardsMe(ObjectOverlapping) : this.FieldGravity, !IsGravityPermanent);  // Instant
            if (Vector3.Distance(this.transform.position, ObjectOverlapping.transform.position) <= CatchDistance)
                SelfBossRef.CaughtPhysicsObjectSuck(ObjectOverlapping);
        }
        foreach (EnemyBaseCharacter ChaeracterOverlapping in CharactersInsideField)
        {
            if (ChaeracterOverlapping == null) continue;
            // ChaeracterOverlapping.MyEnemyController.BaseGravity = this.IsSuckingField ? GetObjectForceTowardsMe(ChaeracterOverlapping) : this.FieldGravity;
            ChaeracterOverlapping.MyEnemyController.SetGravityForceAndDirection(this.IsSuckingField ? GetCharacterForceTowardsMe(ChaeracterOverlapping) : this.FieldGravity, !IsGravityPermanent);
        }
    }

    protected override void ResetPhysicsObject(PhysicsObjectBasic PhysicsObject)
    {
        base.ResetPhysicsObject(PhysicsObject);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss") || other.attachedRigidbody.isKinematic) return;
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boss") || other.attachedRigidbody.isKinematic) return;
        base.OnTriggerExit(other);
    }

    public void TurnOnField()
    {
        this.enabled = true;
    }

    public void TurnOffField()
    {
        PhysicsObjectsInsideField.Clear();
        this.enabled = false;
    }

}
