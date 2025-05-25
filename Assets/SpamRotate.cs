using UnityEngine;

public class SpamRotator : PhysicsObjectBasic
{
    public enum RotationState
    {
        ConstantRotate,
        LoopBetwen,
        NoRotation
    }

    public RotationState State;

    [Header("Constant Rotation")]
    public Vector3 RotationDir;
    public float RotationSpeed;

    [Header("Loop Between")]
    public Vector3 StartPos;
    public Vector3 End;
    public AnimationCurve LoopCurve;
    public float LoopDuration = 2f;
    public float RotationSpeed_Mult = 1f;
    private float loopTimer;

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        switch (State)
        {
            case RotationState.ConstantRotate:
                transform.Rotate(RotationDir * (RotationSpeed * Time.deltaTime));
                break;

            case RotationState.LoopBetwen:
                loopTimer += Time.deltaTime * this.CustomTimeDilation * RotationSpeed_Mult;

                float pingPongT = Mathf.PingPong(loopTimer / (LoopDuration * 0.5f), 1f);
                float curveT = LoopCurve.Evaluate(pingPongT);
                transform.rotation = Quaternion.Lerp(
                    Quaternion.Euler(StartPos),
                    Quaternion.Euler(End),
                    curveT
                );
                break;

            case RotationState.NoRotation:
                break;
        }
    }
}

