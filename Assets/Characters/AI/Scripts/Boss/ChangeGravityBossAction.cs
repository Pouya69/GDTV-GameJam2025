using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Change Gravity Boss", story: "Change Gravity Boss", category: "Action", id: "a4c0133c7ac02b5e19565019a5076684")]
public partial class ChangeGravityBossAction : Action
{
    [SerializeReference] public BlackboardVariable<BossCharacter> SelfBossCharacter;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        SelfBossCharacter.Value.StartChangeGravity();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}
