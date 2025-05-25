using System;
using Unity.VisualScripting;
using UnityEngine;

public class PhysicsObjectBasic : MonoBehaviour
{
    // For objects in the game which are simulated by physics. Having our own gravity direction and time dilation.
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // BaseVelocity and BaseAcceleration are for normal velocity at CustomTimeDilation = 1.
    public Vector3 BaseGravity = Vector3.zero;
    [NonSerialized] public Vector3 GravityBeforeCustomGravity = Vector3.zero;  // For force fields
    public Rigidbody RigidbodyRef;
    public float CustomTimeDilation = 1f;  // Varies from 0f to 1f. 1 -> normal time. 0 -> stopped
    [NonSerialized] public Vector3 BaseVelocity = Vector3.zero;  // For bullets, set it to Vector3.zero but for grenades for example and throwing objects, set it manually here on this.
    [NonSerialized] public Vector3 VelocityBeforeTimeDilation = Vector3.zero;
    public float TimeDilationDifferenceIgnore = 0.01f;  // When reaching this threshold, make it equal to target.
    [NonSerialized] public float CustomTimeDilationTarget = 1f;  // We interpolate the Time Dilation to get the slow effect of transition
    [NonSerialized] public float TimeDilationInterpSpeed;  // How fast we interpolate it.
    [NonSerialized] public bool IsInGravityField = false;
    [NonSerialized] public bool CanBeIncluded = true;

    public void SetTimeDilation(float NewTimeDilation, float NewTimeDilationInterpSpeed = -1f)
    {
        if (NewTimeDilationInterpSpeed > 0f)  // By default we don't change the speed but we can have custom interpolation speed for time.
            TimeDilationInterpSpeed = NewTimeDilationInterpSpeed;
        else  // If the speed is less than equal 0, we set the time dilation instantly
            this.CustomTimeDilation = NewTimeDilation;
        this.CustomTimeDilationTarget = NewTimeDilation;
        // this.RigidbodyRef.linearVelocity = GetTimeScaledVelocity() + (GetGravityForceTimeScaled() * Time.deltaTime);
    }

    public virtual void InitializePhysicsObject(Vector3 InBaseGravity = new Vector3(), Vector3 Velocity=new Vector3(), float NewTimeDilation=1f)
    {
        SetGravityForceAndDirection(InBaseGravity);
        // SetTimeDilation(NewTimeDilation);
    }

    public virtual void UpdatePhysicsObjectBasedOnTimeDilation()
    {
        if (Mathf.Abs(this.CustomTimeDilation-1) <= 0.001)
            this.BaseVelocity = this.RigidbodyRef.linearVelocity;
        if (!this.RigidbodyRef.isKinematic)
        {
            this.RigidbodyRef.linearVelocity = GetTimeScaledVelocity() + (GetGravityForceTimeScaled() * Time.deltaTime);
            this.RigidbodyRef.angularVelocity *= CustomTimeDilation;
        }
        if (!IsInterpolatingTimeDilation()) return;
        this.CustomTimeDilation = Mathf.Lerp(this.CustomTimeDilation, this.CustomTimeDilationTarget, 1 - Mathf.Exp(-this.TimeDilationInterpSpeed * Time.deltaTime));
        if (Mathf.Abs(this.CustomTimeDilationTarget - this.CustomTimeDilation) < TimeDilationDifferenceIgnore)
            this.CustomTimeDilation = this.CustomTimeDilationTarget;
    }

    public Vector3 GetGravityForceTimeScaled() { return this.BaseGravity * this.CustomTimeDilation; }

    public bool IsInterpolatingTimeDilation()
    {
        return TimeDilationInterpSpeed > 0f && this.CustomTimeDilationTarget != this.CustomTimeDilation;
    }

    public Vector3 GetGravityDirection() { return BaseGravity.normalized; }
    public void SetGravityForceAndDirection(Vector3 Final, bool IsDoneByForceField=false)
    {
        if (RigidbodyRef)
            this.BaseGravity = new Vector3(Final.x, Final.y, Final.z) * RigidbodyRef.mass;
        if (!IsDoneByForceField && !IsInGravityField)
            this.GravityBeforeCustomGravity = new Vector3(Final.x, Final.y, Final.z);
    }


    public Vector3 GetTimeScaledVelocity() { return this.BaseVelocity * CustomTimeDilation; }

    public virtual void Start()
    {
        if (BaseGravity.magnitude <= 0)
            SetGravityForceAndDirection(Physics.gravity);
        GravityBeforeCustomGravity = BaseGravity;
        CheckTimeDilationOnSpawn();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }
    public virtual void FixedUpdate()
    {
        UpdatePhysicsObjectBasedOnTimeDilation();
    }

    public void CheckTimeDilationOnSpawn()
    {
        if (this.RigidbodyRef == null) return;
        Collider[] Colliders = Physics.OverlapSphere(this.RigidbodyRef.transform.position, 0.05f);
        foreach (Collider collider in Colliders)
        {
            TimeDilationField timeDilationField;
            bool IsTimeDilationField = collider.transform.root.TryGetComponent<TimeDilationField>(out timeDilationField);
            if (!IsTimeDilationField) continue;
            timeDilationField.PhysicsObjectEntered_ONSTART(this);
            // Debug.LogError("WORKS");
        }
    }
}
