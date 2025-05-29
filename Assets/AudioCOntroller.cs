using UnityEngine;
using FMODUnity;

public class AudioController : MonoBehaviour
{
    public PlayerCharacter character;
    public float State = 0;
    public StudioEventEmitter HeartBeat;
    public StudioEventEmitter Ambiance;

    private float inCombat;
    private float LastHealth;
    void Start()
    {
        LastHealth = character.GetCurrentHealth();
        if (!HeartBeat.IsPlaying())
        {
            HeartBeat.Play();
        }
        if (!Ambiance.IsPlaying())
        {
            Ambiance.Play();
        }
    }

    void Update()
    {
        if (LastHealth != character.GetCurrentHealth()) { State = 2; inCombat = 10f; LastHealth = character.GetCurrentHealth(); }
        if(inCombat > 0)
        {
            inCombat -= Time.deltaTime;
        }
        else
        {
            State = 0;
        }
        float parameterValue = Mathf.Clamp((character.GetCurrentHealth() * 2) / 100, 0f, 2f);
        HeartBeat.SetParameter("lowHealth", parameterValue);
        Ambiance.SetParameter("lowHealth", parameterValue);

        float GameStateParamter = Mathf.Clamp(State, 0f, 2f);
        Ambiance.SetParameter("gameState", GameStateParamter);
    }

    public void StopCurrentMusic()
    {
        HeartBeat.Stop();
        Ambiance.Stop();
    }
}
