using System;
using UnityEngine;

public class FieldBaseGrenade : PhysicsObjectBasic
{
    public GameObject FieldToSpawn;  // Field that spawns on impact
    [NonSerialized] private bool ShouldSpawnField = false;  // This will make sure the field is spawned only on impact. Not when it is destroyed by other fields/objects.
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnCollisionEnter(Collision collision)
    {
        ShouldSpawnField = true;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (!ShouldSpawnField) return;
        // Spawn the field.
        GameObject FieldSpawned = Instantiate(FieldToSpawn, transform.position, Quaternion.identity);
    }

    public bool IsTimeDilationFieldGrenade() { return FieldToSpawn.TryGetComponent<TimeDilationField>(out TimeDilationField _); }

    public bool IsGravityFieldGrenade() { return FieldToSpawn.TryGetComponent<GravityField>(out GravityField _); }
}
