using UnityEngine;

public class TimeDilationField : FieldBase
{

    public override void Start()
    {
        base.Start();
        Debug.LogWarning("Force in timefield: " + this.FieldAmount);
    }


    public override void UpdateOverlappingObjects()
    {
        base.UpdateOverlappingObjects();
        foreach (PhysicsObjectBasic ObjectOverlapping in PhysicsObjectsInsideField)
        {
            if (ObjectOverlapping == null) continue;
            //  Debug.Log("Time Dilating: " + ObjectOverlapping.gameObject.name);
            ObjectOverlapping.SetTimeDilation(FieldAmount);  // Instant
        }
        foreach (EnemyBaseCharacter ChaeracterOverlapping in CharactersInsideField)
        {
            if (ChaeracterOverlapping == null) continue;
            ChaeracterOverlapping.MyEnemyController.SetTimeDilation(FieldAmount);  // Instant
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

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }

    public override void CharacterEntered(EnemyBaseCharacter Character)
    {
        Character.MyEnemyController.VelocityBeforeTimeDilation = Character.MyEnemyController.RigidbodyRef.linearVelocity;
        base.CharacterEntered(Character);
        Character.MyEnemyController.SetTimeDilation(FieldAmount);
        Character.MyController.UpdateCharacterMovement();
    }

    public override void PhysicsObjectEntered(PhysicsObjectBasic PhysicsObject)
    {
        PhysicsObject.VelocityBeforeTimeDilation = PhysicsObject.RigidbodyRef.linearVelocity;
        //PhysicsObject.RigidbodyRef.angularVelocity = Vector3.zero;
        base.PhysicsObjectEntered(PhysicsObject);
        PhysicsObject.SetTimeDilation(FieldAmount);
        PhysicsObject.UpdatePhysicsObjectBasedOnTimeDilation();
    }

    // For things like bullets that on spawn need to be checked.
    public void PhysicsObjectEntered_ONSTART(PhysicsObjectBasic PhysicsObject)
    {
        PhysicsObject.SetTimeDilation(FieldAmount);
        PhysicsObject.UpdatePhysicsObjectBasedOnTimeDilation();
    }

    protected override void ResetCharacter(EnemyBaseCharacter Character) {
        if (Character == null) return;
        base.ResetCharacter(Character);
        Character.MyEnemyController.SetTimeDilation(1f);
        // * Characters could be different. *
        // Character.MyEnemyController.RigidbodyRef.linearVelocity = Character.MyEnemyController.VelocityBeforeTimeDilation;
    }
    protected override void ResetPhysicsObject(PhysicsObjectBasic PhysicsObject) {
        if (PhysicsObject == null) return;
        base.ResetPhysicsObject(PhysicsObject);
        PhysicsObject.SetTimeDilation(1f);
        // We can add conditions to whether affect it or not after.
        PhysicsObject.RigidbodyRef.linearVelocity = PhysicsObject.VelocityBeforeTimeDilation;
    }

}
