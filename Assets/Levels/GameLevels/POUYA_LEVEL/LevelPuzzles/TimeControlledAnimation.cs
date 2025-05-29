using UnityEngine;

public class TimeControlledAnimation : PhysicsObjectBasic
{
    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        CanBeIncluded = false;
    }

    public override void UpdatePhysicsObjectBasedOnTimeDilation()
    {
        if (!IsInterpolatingTimeDilation()) return;
        this.CustomTimeDilation = Mathf.Lerp(this.CustomTimeDilation, this.CustomTimeDilationTarget, 1 - Mathf.Exp(-this.TimeDilationInterpSpeed * Time.deltaTime));
        if (Mathf.Abs(this.CustomTimeDilationTarget - this.CustomTimeDilation) < TimeDilationDifferenceIgnore)
            this.CustomTimeDilation = this.CustomTimeDilationTarget;
    }

    public override void FixedUpdate()
    {
        //base.FixedUpdate();
        //this.transform.Rotate(RotationAxis, this.CustomTimeDilation * RotatingSpeed * Time.deltaTime);
    }

    public override void Update()
    {
        animator.speed = CustomTimeDilation;
        // base.Update();
    }
}
