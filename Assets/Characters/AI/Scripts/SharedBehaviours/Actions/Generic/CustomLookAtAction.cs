using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Custom Look At", story: "CUSTOM [Self] Look At Player", category: "Action", id: "9a7ba317a9db0905bdc079c3b5981efe")]
public partial class CustomLookAtAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<EnemyBaseCharacter> SelfEnemyBaseCharacter;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        SelfEnemyBaseCharacter.Value.MyEnemyController.RotateTowardsPlayer();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

