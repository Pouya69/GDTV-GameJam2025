using System;
using UnityEngine;

public class FieldBaseGrenade : PhysicsObjectBasic
{
    public GameObject FieldToSpawn;  // Field that spawns on impact
    [NonSerialized] private bool ShouldSpawnField = false;  // This will make sure the field is spawned only on impact. Not when it is destroyed by other fields/objects.
    [NonSerialized] public bool CanWork = false;  // This is for when it is held by player or on the ground it does not work. We have to throw it.
    [NonSerialized] public float GrenadeCharge = 0f;
    public float MaxGrenadeCharge = 300f;  // Maximum charge
    public float GrenadeChargingSpeed = 20f;
    public bool IsInstantCharge = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public override void Start()
    {
        // We do the checking when the grenade is just thrown on GrenadeThrown().
    }

    public override void UpdatePhysicsObjectBasedOnTimeDilation()
    {
        if (Mathf.Abs(this.CustomTimeDilation - 1) <= 0.001)
            this.BaseVelocity = this.RigidbodyRef.linearVelocity;
        if (!this.RigidbodyRef.isKinematic)
        {
            // Debug.LogError(GetTimeScaledVelocity());
            this.RigidbodyRef.linearVelocity = GetTimeScaledVelocity() + (GetGravityForceTimeScaled() * Time.deltaTime);
            this.RigidbodyRef.angularVelocity *= CustomTimeDilation;
        }
        if (!IsInterpolatingTimeDilation()) return;
        this.CustomTimeDilation = Mathf.Lerp(this.CustomTimeDilation, this.CustomTimeDilationTarget, 1 - Mathf.Exp(-this.TimeDilationInterpSpeed * Time.deltaTime));
        if (Mathf.Abs(this.CustomTimeDilationTarget - this.CustomTimeDilation) < TimeDilationDifferenceIgnore)
            this.CustomTimeDilation = this.CustomTimeDilationTarget;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Transform ObjectRoot = collision.gameObject.transform.root;
        if (!CanWork || ObjectRoot.TryGetComponent<FieldBase>(out _) || ObjectRoot.TryGetComponent<PlayerCharacter>(out _)) return;
        ShouldSpawnField = true;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (!ShouldSpawnField) return;
        // Spawn the field.
        GameObject FieldSpawned = Instantiate(FieldToSpawn, transform.position, Quaternion.identity);
        FieldSpawned.GetComponent<FieldBase>().FieldAmount = this.GrenadeCharge / 100f;
    }

    // Returns true if fully charged
    public bool ChargeGrenade() {
        if (this.GrenadeCharge == this.MaxGrenadeCharge) return true;
        this.GrenadeCharge = Mathf.Lerp(this.GrenadeCharge, this.MaxGrenadeCharge, 1 - Mathf.Exp(-this.GrenadeChargingSpeed * Time.deltaTime));
        if (Mathf.Abs(this.MaxGrenadeCharge - this.GrenadeCharge) < 0.01)
        {
            this.GrenadeCharge = this.MaxGrenadeCharge;
            return true;
        }
        return false;
    }

    public bool IsTimeDilationFieldGrenade() { return FieldToSpawn.TryGetComponent<TimeDilationField>(out TimeDilationField _); }

    public bool IsGravityFieldGrenade() { return FieldToSpawn.TryGetComponent<GravityField>(out GravityField _); }

    public void GrenadeThrown(Vector3 Vel, Vector3 Gravity)
    {
        this.transform.SetParent(null, true);
        this.InitializePhysicsObject(Gravity, Vel);
        this.RigidbodyRef.isKinematic = false;
        this.RigidbodyRef.detectCollisions = true;
        this.BaseVelocity = Vel;
        this.RigidbodyRef.linearVelocity = Vel;
        this.GravityBeforeCustomGravity = Gravity;
        base.CheckTimeDilationOnSpawn();
        this.UpdatePhysicsObjectBasedOnTimeDilation();
        CanWork = true;
    }

    private void Awake()
    {
        this.RigidbodyRef.isKinematic = true;
        this.RigidbodyRef.detectCollisions = false;
        if (IsInstantCharge)
            GrenadeCharge = MaxGrenadeCharge;
    }
}
