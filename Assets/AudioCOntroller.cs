using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioController : MonoBehaviour
{
    [Range(0f, 2f)] public float lowHealth = 0f;

    private StudioEventEmitter emitter;

    void Start()
    {
        emitter = GetComponent<StudioEventEmitter>();

        if (!emitter.IsPlaying())
        {
            emitter.Play(); 
        }
    }

    void Update()
    {
        float parameterValue = Mathf.Clamp(lowHealth, 0f, 2f);
        emitter.SetParameter("lowHealth", parameterValue);
    }
}
