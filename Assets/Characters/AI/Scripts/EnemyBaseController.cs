using System;
using UnityEngine;

public class EnemyBaseController : CustomCharacterController
{
    public float CustomTimeDilation = 1f;  // Varies from 0f to 1f. 1 -> normal time. 0 -> stopped
    public float TimeDilationDifferenceIgnore = 0.01f;  // When reaching this threshold, make it equal to target.
    [NonSerialized] public float CustomTimeDilationTarget = 1f;  // We interpolate the Time Dilation to get the slow effect of transition
    [NonSerialized] public float TimeDilationInterpSpeed;  // How fast we interpolate it.
    [NonSerialized] public Vector3 GravityBeforeCustomGravity = Vector3.zero;  // For force fields


    public void SetTimeDilation(float NewTimeDilation, float NewTimeDilationInterpSpeed = -1f)
    {
        if (NewTimeDilationInterpSpeed > 0f)  // By default we don't change the speed but we can have custom interpolation speed for time.
            TimeDilationInterpSpeed = NewTimeDilationInterpSpeed;
        else  // If the speed is less than equal 0, we set the time dilation instantly
            this.CustomTimeDilation = NewTimeDilation;
        this.CustomTimeDilationTarget = NewTimeDilation;
        // this.RigidbodyRef.linearVelocity = GetTimeScaledVelocity() + (GetGravityForceTimeScaled() * Time.deltaTime);
    }

    public virtual void UpdatePhysicsObjectBasedOnTimeDilation()
    {
        if (!IsInterpolatingTimeDilation()) return;
        this.CustomTimeDilation = Mathf.Lerp(this.CustomTimeDilation, this.CustomTimeDilationTarget, 1 - Mathf.Exp(-this.TimeDilationInterpSpeed * Time.deltaTime));
        if (Mathf.Abs(this.CustomTimeDilationTarget - this.CustomTimeDilation) < TimeDilationDifferenceIgnore)
            this.CustomTimeDilation = this.CustomTimeDilationTarget;
    }

    public Vector3 GetTimeScaledVelocity() { return InputVelocity * CustomTimeDilation; }

    public Vector3 GetGravityForceTimeScaled() { return this.BaseGravity * this.CustomTimeDilation; }

    public override void SetGravityForceAndDirection(Vector3 Final, bool IsDoneByForceField = false)
    {
        base.SetGravityForceAndDirection(Final, IsDoneByForceField);
        if (!IsDoneByForceField)
            this.GravityBeforeCustomGravity = Final;
    }

    public bool IsInterpolatingTimeDilation()
    {
        return TimeDilationInterpSpeed > 0f && this.CustomTimeDilationTarget != this.CustomTimeDilation;
    }

    public override void UpdateCharacterMovement(float Multiplier = 1)
    {
        base.UpdateCharacterMovement(CustomTimeDilation);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
