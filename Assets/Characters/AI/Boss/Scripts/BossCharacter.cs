using System.Collections.Generic;
using UnityEngine;

public class BossCharacter : EnemyBaseCharacter
{
    List<PhysicsObjectBasic> ObjectsSuckedIn = new List<PhysicsObjectBasic>();
    public override void Attack()
    {
        // base.Attack();
        // PUNCH HERE.
    }

    public bool AttackThrowObjectsAtPlayer()
    {
        if (ObjectsSuckedIn.Count == 0) return false;
        return true;
    }

    public void SuckObjectsAround()
    {

    }

    public void JumpStomp()
    {

    }

    public void SummonEnemies()
    {

    }
}
