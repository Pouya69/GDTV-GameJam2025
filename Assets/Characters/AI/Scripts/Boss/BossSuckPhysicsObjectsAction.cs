using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss Suck Physics Objects", story: "Boss Suck Physics Objects", category: "Boss Character", id: "8306c1833c1425ff235225ffad5e0414")]
public partial class BossSuckPhysicsObjectsAction : Action
{
    [SerializeReference] public BlackboardVariable<BossCharacter> SelfBossCharacter;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        SelfBossCharacter.Value.SuckObjectsAround();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}
