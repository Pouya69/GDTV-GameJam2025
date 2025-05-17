using Unity.VisualScripting;
using UnityEngine;

public class GravityField : FieldBase
{
    public bool IsSuckingField = false;  // Naughty... For sucking things instead of just normal gravity
    public bool IsGravityPermanent = false;  // If true, the effect of gravity will stay on the objects after Destroy() of this field.
    public Vector3 FieldGravity = Vector3.down;

    public override void Awake()
    {
        base.Awake();
        if (!IsSuckingField)
            FieldGravity = FieldGravity.normalized * FieldAmount;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void UpdateOverlappingObjects()
    {
        base.UpdateOverlappingObjects();
        foreach (PhysicsObjectBasic ObjectOverlapping in PhysicsObjectsInsideField)
        {
            if (ObjectOverlapping == null) continue;
            // ObjectOverlapping.BaseGravity = this.IsSuckingField ? GetObjectForceTowardsMe(ObjectOverlapping) : this.FieldGravity;
            ObjectOverlapping.SetGravityForceAndDirection(this.IsSuckingField ? GetObjectForceTowardsMe(ObjectOverlapping) : this.FieldGravity, !IsGravityPermanent);  // Instant
        }
        foreach (EnemyBaseCharacter ChaeracterOverlapping in CharactersInsideField)
        {
            if (ChaeracterOverlapping == null) continue;
            // ChaeracterOverlapping.MyEnemyController.BaseGravity = this.IsSuckingField ? GetObjectForceTowardsMe(ChaeracterOverlapping) : this.FieldGravity;
            ChaeracterOverlapping.MyEnemyController.SetGravityForceAndDirection(this.IsSuckingField ? GetObjectForceTowardsMe(ChaeracterOverlapping) : this.FieldGravity, !IsGravityPermanent);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        foreach (PhysicsObjectBasic ObjectOverlapping in PhysicsObjectsInsideField)
        {
            ResetPhysicsObject(ObjectOverlapping);
        }
        foreach (EnemyBaseCharacter ChaeracterOverlapping in CharactersInsideField)
        {
            ResetCharacter(ChaeracterOverlapping);
        }
    }

    public Vector3 GetObjectForceTowardsMe(MonoBehaviour Obj)
    {
        return (this.transform.position - Obj.transform.position).normalized * this.FieldAmount;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {

        base.OnTriggerExit(other);
    }

    protected override void ResetCharacter(EnemyBaseCharacter Character)
    {
        base.ResetCharacter(Character);
        Character.MyEnemyController.SetGravityForceAndDirection(Character.MyEnemyController.GravityBeforeCustomGravity, true);
        Character.MyEnemyController.RigidbodyRef.linearVelocity = Vector3.zero;
        Character.MyEnemyController.RigidbodyRef.angularVelocity = Vector3.zero;
    }

    protected override void ResetPhysicsObject(PhysicsObjectBasic PhysicsObject)
    {
        base.ResetPhysicsObject(PhysicsObject);
        PhysicsObject.SetGravityForceAndDirection(PhysicsObject.GravityBeforeCustomGravity, true);  // Instant
        PhysicsObject.RigidbodyRef.linearVelocity = Vector3.zero;
        PhysicsObject.RigidbodyRef.angularVelocity = Vector3.zero;
    }

    public override void CharacterEntered(EnemyBaseCharacter Character)
    {
        Character.MyEnemyController.GravityBeforeCustomGravity = Character.MyEnemyController.BaseGravity;
        base.CharacterEntered(Character);
    }

    public override void PhysicsObjectEntered(PhysicsObjectBasic PhysicsObject)
    {
        PhysicsObject.GravityBeforeCustomGravity = PhysicsObject.BaseGravity;
        base.PhysicsObjectEntered(PhysicsObject);
    }

    // For things like bullets that on spawn need to be checked.
    public void PhysicsObjectEntered_ONSTART(PhysicsObjectBasic PhysicsObject)
    {
        PhysicsObject.GravityBeforeCustomGravity = PhysicsObject.BaseGravity;
    }
}
