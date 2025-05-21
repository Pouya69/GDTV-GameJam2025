using UnityEngine;

public class SpamRotator : MonoBehaviour
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
    public Vector3 Start;
    public Vector3 End;
    public AnimationCurve LoopCurve;
    public float LoopDuration = 2f;

    private float loopTimer;

    void Update()
    {
        switch (State)
        {
            case RotationState.ConstantRotate:
                transform.Rotate(RotationDir * (RotationSpeed * Time.deltaTime));
                break;

            case RotationState.LoopBetwen:
                loopTimer += Time.deltaTime;

                float pingPongT = Mathf.PingPong(loopTimer / (LoopDuration * 0.5f), 1f);
                float curveT = LoopCurve.Evaluate(pingPongT);
                transform.rotation = Quaternion.Lerp(
                    Quaternion.Euler(Start),
                    Quaternion.Euler(End),
                    curveT
                );
                break;

            case RotationState.NoRotation:
                break;
        }
    }
}

