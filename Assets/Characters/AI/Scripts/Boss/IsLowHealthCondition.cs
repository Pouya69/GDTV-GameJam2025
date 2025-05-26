using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is Low Health", story: "Is [SelfBossCharacter] Health Lower than [HEALTH]", category: "Conditions", id: "7ac572f2da57e8e7bcc3a61f9435343b")]
public partial class IsLowHealthCondition : Condition
{
    [SerializeReference] public BlackboardVariable<BossCharacter> SelfBossCharacter;
    [SerializeReference] public BlackboardVariable<float> HEALTH;

    public override bool IsTrue()
    {
        return SelfBossCharacter.Value.GetCurrentHealth() <= HEALTH;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
