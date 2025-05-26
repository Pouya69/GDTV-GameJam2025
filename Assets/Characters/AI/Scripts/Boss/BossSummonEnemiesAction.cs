using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss Summon Enemies", story: "Boss Summon Enemies", category: "Boss Character", id: "d2a3cbc3a24c09bb730d73a94e2d5c05")]
public partial class BossSummonEnemiesAction : Action
{
    [SerializeReference] public BlackboardVariable<BossCharacter> SelfBossCharacter;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        SelfBossCharacter.Value.SummonEnemies();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

