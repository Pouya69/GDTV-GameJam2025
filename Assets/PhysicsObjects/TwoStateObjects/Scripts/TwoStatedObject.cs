using UnityEngine;

public class TwoStatedObject : MonoBehaviour
{
    [Header("Animations")]
    public AnimationClip AnimationSequence;
    public float CurrentFrame = 0;
    private int AnimationLengthInFrames;
    private float FramesPerSecond;

    private bool ObjectState = false; // false = before, true = after
    private bool IsButtonHeld = false;

    void Start()
    {
        FramesPerSecond = AnimationSequence.frameRate;
        AnimationLengthInFrames = Mathf.RoundToInt(AnimationSequence.frameRate * AnimationSequence.length);
        CurrentFrame = ObjectState ? AnimationLengthInFrames : 0;
    }

    void Update()
    {
        int targetFrame = GetTargetFrame();
        if (CurrentFrame == targetFrame) return;

        // Move toward the target frame
        float delta = Time.deltaTime * FramesPerSecond;
        CurrentFrame += Mathf.Sign(targetFrame - CurrentFrame) * delta;

        // Clamp and sample
        CurrentFrame = Mathf.Clamp(CurrentFrame, 0, AnimationLengthInFrames);
        float normalizedTime = CurrentFrame / AnimationLengthInFrames;
        AnimationSequence.SampleAnimation(gameObject, normalizedTime * AnimationSequence.length);

        if (CurrentFrame == targetFrame)
        {
            OnStateReachEnd();
        }
    }

    private int GetTargetFrame()
    {
        if (IsButtonHeld)
            return ObjectState ? 0 : AnimationLengthInFrames; // move to opposite state
        else
            return ObjectState ? AnimationLengthInFrames : 0; // return to resting state
    }

    public void SetContinuePlaying(bool isHeld)
    {
        IsButtonHeld = isHeld;
    }

    public virtual void OnStateReachEnd()
    {
        ObjectState = !ObjectState;
        IsButtonHeld = false;
        if (ObjectState)
            SetStateToBefore();
        else
            SetStateToAfter();
    }

    public virtual void SetStateToBefore()
    {

    }

    public virtual void SetStateToAfter()
    {

    }

    public bool IsInAfterState => !ObjectState;
}
