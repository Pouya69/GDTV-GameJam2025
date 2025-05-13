using Unity.VisualScripting;
using UnityEngine;

public class PhysicsObjectBasic : MonoBehaviour
{
    // For objects in the game which are simulated by physics. Having our own gravity direction and time dilation.
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // BaseVelocity and BaseAcceleration are for normal velocity at CustomTimeDilation = 1.
    [DoNotSerialize] public Vector3 BaseVelocity = Vector3.zero;
    public Vector3 BaseGravity = Vector3.zero;
    public Rigidbody RigidbodyRef;
    public float CustomTimeDilation = 1f;  // Varies from 0f to 1f. 1 -> normal time. 0 -> stopped
    public float TimeDilationDifferenceIgnore = 0.01f;  // When reaching this threshold, make it equal to target.
    [DoNotSerialize] public float CustomTimeDilationTarget = 1f;  // We interpolate the Time Dilation to get the slow effect of transition
    [DoNotSerialize] public float TimeDilationInterpSpeed;  // How fast we interpolate it.

    public void SetTimeDilation(float NewTimeDilation, float NewTimeDilationInterpSpeed = -1f)
    {
        if (NewTimeDilationInterpSpeed > 0f)  // By default we don't change the speed but we can have custom interpolation speed for time.
            TimeDilationInterpSpeed = NewTimeDilationInterpSpeed;
        else  // If the speed is less than equal 0, we set the time dilation instantly
            this.CustomTimeDilation = NewTimeDilation;
        this.CustomTimeDilationTarget = NewTimeDilation;
    }

    public virtual void InitializePhysicsObject(Vector3 InBaseGravity = new Vector3(), Vector3 Velocity=new Vector3(), float NewTimeDilation=1f)
    {
        this.BaseVelocity = Velocity;
        SetGravityForceAndDirection(InBaseGravity);
        SetTimeDilation(NewTimeDilation);
    }

    public virtual void UpdatePhysicsObjectBasedOnTimeDilation()
    {
        this.RigidbodyRef.AddForce(GetGravityForceTimeScaled() + GetTimeScaledVelocity());  // The velocity and the gravity force are applied.
        // this.ConstantGravityForce.force = this.BaseVelocity * this.CustomTimeDilation;
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
    public void SetGravityForceAndDirection(Vector3 Final)
    {
        this.BaseGravity = new Vector3(Final.x, Final.y, Final.z) * RigidbodyRef.mass;
    }

    public Vector3 GetTimeScaledVelocity() { return BaseVelocity * CustomTimeDilation; }

    public virtual void Start()
    {
        if (BaseGravity.magnitude <= 0)
            SetGravityForceAndDirection(Physics.gravity);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }
    public virtual void FixedUpdate()
    {
        UpdatePhysicsObjectBasedOnTimeDilation();
    }
}
