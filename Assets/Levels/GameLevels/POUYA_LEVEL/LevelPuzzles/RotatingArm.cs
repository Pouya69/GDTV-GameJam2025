using UnityEngine;

public class RotatingArm : PhysicsObjectBasic
{
    public float RotatingSpeed = 5;
    public Vector3 RotationAxis = Vector3.forward;

    private void Awake()
    {
        RotationAxis.Normalize();
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
        base.FixedUpdate();
        this.transform.Rotate(RotationAxis, this.CustomTimeDilation * RotatingSpeed * Time.deltaTime);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        
    }


}
