using UnityEngine;

public class TimeDilationField : FieldBase
{

    public override void Start()
    {
        base.Start();
    }


    public override void UpdateOverlappingObjects()
    {
        base.UpdateOverlappingObjects();
        foreach (PhysicsObjectBasic ObjectOverlapping in PhysicsObjectsInsideField)
        {
            ObjectOverlapping.SetTimeDilation(FieldAmount);  // Instant
        }
        foreach (EnemyBaseCharacter ChaeracterOverlapping in CharactersInsideField)
        {
            ChaeracterOverlapping.MyEnemyController.SetTimeDilation(FieldAmount);  // Instant
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        foreach (PhysicsObjectBasic ObjectOverlapping in PhysicsObjectsInsideField)
        {
            ObjectOverlapping.SetTimeDilation(1f);  // Instant
        }
        foreach (EnemyBaseCharacter ChaeracterOverlapping in CharactersInsideField)
        {
            ChaeracterOverlapping.MyEnemyController.SetTimeDilation(1f);  // Instant
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        FieldBaseGrenade GrenadeEntered;
        bool IsFieldGrenade = other.TryGetComponent<FieldBaseGrenade>(out GrenadeEntered);
        if (IsFieldGrenade)
        {
            if (GrenadeEntered.IsTimeDilationFieldGrenade())
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
            if (GrenadeExited.IsTimeDilationFieldGrenade())
            {
                Destroy(other.gameObject);
                return;
            }
        }
        base.OnTriggerExit(other);
    }
}
