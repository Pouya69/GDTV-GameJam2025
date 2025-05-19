using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Increment Number", story: "Increment Number", category: "Action/Blackboard", id: "3a971e36bb1481895d1a2d8f41371882")]
public partial class IncrementNumberAction : Action
{
    [SerializeReference] public BlackboardVariable<int> Number;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Number.Value++;
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

