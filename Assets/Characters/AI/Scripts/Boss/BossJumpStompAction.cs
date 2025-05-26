using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss Jump Stomp", story: "Boss Jump Stomp", category: "Boss Character", id: "9fc59b5b37d5f2daaaca9105ec41d9a1")]
public partial class BossJumpStompAction : Action
{
    [SerializeReference] public BlackboardVariable<BossCharacter> SelfBossCharacter;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        SelfBossCharacter.Value.JumpStomp();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

