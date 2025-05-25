using UnityEngine;

public class RotatingArm : PhysicsObjectBasic
{
    public float RotatingSpeed = 5;
    public Vector3 RotationAxis = Vector3.forward;

    private void Awake()
    {
        CanBeIncluded = false;
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

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.root.gameObject.TryGetComponent<FieldBaseGrenade>(out _)) return;
        other.transform.root.gameObject.TryGetComponent<PlayerCharacter>(out PlayerCharacter PlayerRef);
        other.transform.root.gameObject.TryGetComponent<PhysicsObjectBasic>(out PhysicsObjectBasic PhysRef);
        if (PlayerRef != null)
            PlayerRef.transform.SetParent(this.transform, true);
        else if (PhysRef != null && PhysRef.CanBeIncluded)
            PhysRef.transform.SetParent(this.transform, true);
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.transform.root.gameObject.TryGetComponent<FieldBaseGrenade>(out _)) return;
        other.transform.gameObject.TryGetComponent<PhysicsObjectBasic>(out PhysicsObjectBasic PhysRef);
        other.transform.gameObject.TryGetComponent<PlayerCharacter>(out PlayerCharacter PlayerRef);
        if (PlayerRef != null)
            PlayerRef.transform.SetParent(null, true);
        else if (PhysRef != null && PhysRef.CanBeIncluded)
            PhysRef.transform.SetParent(null, true);
    }

}
