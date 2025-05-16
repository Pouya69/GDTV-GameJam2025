using UnityEngine;

public class TimeDilationField : FieldBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        bool IsTimeDilation = other.TryGetComponent<TimeDilationField>(out TimeDilationField _);
        if (IsTimeDilation)
        {
            Destroy(other.gameObject);
            return;
        }
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        bool IsTimeDilation = other.TryGetComponent<TimeDilationField>(out TimeDilationField _);
        if (IsTimeDilation)
        {
            Destroy(other.gameObject);
            return;
        }
        base.OnTriggerExit(other);
    }
}
