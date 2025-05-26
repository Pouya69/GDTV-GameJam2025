using UnityEngine;

public class BossAnimationScript : MonoBehaviour
{
    [Header("Components")]
    public BossCharacter SelfBossRef;
    public Animator BossAnimator;
    [Header("Parameters")]
    public float CharacterSpeedDamping = 0.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimator();
    }


    public void UpdateAnimator()
    {
        float deltaTime = Time.deltaTime;
        BossAnimator.SetFloat("TimeDilation", SelfBossRef.MyEnemyController.CustomTimeDilation);
        Vector3 Vel = SelfBossRef.MyBossController.GetEnemyForward();  // THIS IS MOVEMENT DIRECTION OF NAVAGENT
        BossAnimator.SetFloat("CharacterSpeed", SelfBossRef.MyEnemyController.RigidbodyRef.linearVelocity.magnitude, CharacterSpeedDamping, deltaTime);
    }



    public void StartSucking()
    {
        BossAnimator.SetTrigger("SuckTrigger");
    }

    public void StartSuckingEVENT()
    {
        SelfBossRef.SuckStarted();
    }

    public void EndSuckingEVENT()
    {
        SelfBossRef.SuckEnded();
    }

    public void StartThrowObject()
    {
        BossAnimator.SetTrigger("ThrowTrigger");
    }

    public void ThrowCompletedEVENT()
    {
        SelfBossRef.ThrowObject();
    }

    public void StartJumpStop()
    {

    }

    public void CompletedJumpStop()
    {

    }

    public void StartedMeleeEVENT()
    {

    }

    public void CompletedMeleeEVENT()
    {

    }

    public void StartSummonEnemies()
    {
        BossAnimator.SetTrigger("SummonEnemiesTrigger");
    }

    public void SummonEnemiesEVENT()
    {

    }
}
