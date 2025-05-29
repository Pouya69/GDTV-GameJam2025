using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Spawn Loot For Player", story: "Spawn Loot for Player", category: "Boss Character", id: "dd5738bebad0c7d2fc4aa3e7282fd984")]
public partial class SpawnLootForPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<BossCharacter> SelfBossCharacter;
    protected override Status OnStart()
    {
        SelfBossCharacter.Value.SpawnLootForPlayer();
        return Status.Success;
    }
}

