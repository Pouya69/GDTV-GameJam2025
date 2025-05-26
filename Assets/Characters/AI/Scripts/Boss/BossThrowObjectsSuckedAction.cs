using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss Throw Objects Sucked", story: "Boss Throw Objects Sucked", category: "Boss Character", id: "28ae7a17d0ab52edaf1ab3445b0d45ee")]
public partial class BossThrowObjectsSuckedAction : Action
{
    [SerializeReference] public BlackboardVariable<BossCharacter> SelfBossCharacter;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        bool DidTStartThrow = SelfBossCharacter.Value.AttackThrowObjectsAtPlayer();
        return DidTStartThrow ? Status.Success : Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}
