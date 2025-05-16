using Unity.VisualScripting;
using UnityEngine;

public class GravityField : FieldBase
{
    public bool IsSuckingField = false;  // Naughty... For sucking things instead of just normal gravity
    public bool IsGravityTemporary = false;  // If false, the effect of gravity will stay on the objects after Destroy() of this field.
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
            ObjectOverlapping.SetGravityForceAndDirection(this.IsSuckingField ? GetObjectForceTowardsMe(ObjectOverlapping) : this.FieldGravity, !IsGravityTemporary);  // Instant
        }
        foreach (EnemyBaseCharacter ChaeracterOverlapping in CharactersInsideField)
        {
            ChaeracterOverlapping.MyEnemyController.SetGravityForceAndDirection(this.IsSuckingField ? GetObjectForceTowardsMe(ChaeracterOverlapping) : this.FieldGravity, !IsGravityTemporary);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        foreach (PhysicsObjectBasic ObjectOverlapping in PhysicsObjectsInsideField)
        {
            ObjectOverlapping.SetGravityForceAndDirection(ObjectOverlapping.GravityBeforeCustomGravity, !IsGravityTemporary);  // Instant
        }
        foreach (EnemyBaseCharacter ChaeracterOverlapping in CharactersInsideField)
        {
            ChaeracterOverlapping.MyEnemyController.SetGravityForceAndDirection(ChaeracterOverlapping.MyEnemyController.GravityBeforeCustomGravity, !IsGravityTemporary);  // Instant
        }
    }

    public Vector3 GetObjectForceTowardsMe(MonoBehaviour Obj)
    {
        return (this.transform.position - Obj.transform.position).normalized * this.FieldAmount;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        FieldBaseGrenade GrenadeEntered;
        bool IsFieldGrenade = other.TryGetComponent<FieldBaseGrenade>(out GrenadeEntered);
        if (IsFieldGrenade)
        {
            if (GrenadeEntered.IsGravityFieldGrenade())
            {
                Destroy(other.gameObject);
                return;
            }
        }
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        FieldBaseGrenade GrenadeExited;
        bool IsFieldGrenade = other.TryGetComponent<FieldBaseGrenade>(out GrenadeExited);
        if (IsFieldGrenade)
        {
            if (GrenadeExited.IsGravityFieldGrenade())
            {
                Destroy(other.gameObject);
                return;
            }
        }
        base.OnTriggerExit(other);
    }
}
