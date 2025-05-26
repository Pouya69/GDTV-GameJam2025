using UnityEngine;

public class BossController : EnemyBaseController
{
    [Header("Boss")]
    public float BaseSpeed = 20f;
    public BossCharacter MyBossCharacter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        // base.Update();
        // Setting speed.
        this.MyNavAgent.speed = BaseSpeed * this.CustomTimeDilation;
        this.MyBlackBoardRef.SetVariableValue<float>("MyBTMovementSpeed", this.MyNavAgent.speed);
    }
}
