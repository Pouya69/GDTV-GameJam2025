using System;
using System.Collections.Generic;
using UnityEngine;

public class BossCharacter : EnemyBaseCharacter
{
    [Header("Boss")]
    public BossController MyBossController;
    public BossAnimationScript BossAnimationScript;
    [NonSerialized] public PhysicsObjectBasic PhysObjectInHand;
    [NonSerialized] public List<PhysicsObjectBasic> PhysObjectsSuckedIn = new List<PhysicsObjectBasic>();
    public float BossThrowObjectPower = 10f;
    public BossGravityField BossSuckerField;
    public GameObject EnemySpawnPrefab;
    public List<Transform> EnemySpawnPoints;
    public PlayerCharacter PlayerRef;
    public Transform HandTransform;
    public Transform AttachTransform;

    public void ChangeGravityAroundMe()
    {
        Collider[] Colliders = Physics.OverlapSphere(this.CapsuleCollision.transform.position, BossSuckerField.FieldRadius);
        if (Colliders.Length == 0) return;
        bool IsLeftRight = UnityEngine.Random.value >= 0.5f;
        Vector3 NewGravity;
        if (IsLeftRight)
            NewGravity = new Vector3(UnityEngine.Random.value >= 0.5f ? -9.81f : 9.81f, 0, 0);
        else
            NewGravity = new Vector3(0, 0, UnityEngine.Random.value >= 0.5f ? -9.81f : 9.81f);
        foreach (Collider ObjInField in Colliders)
        {
            if (!ObjInField.transform.root.TryGetComponent<PhysicsObjectBasic>(out PhysicsObjectBasic PhysObj)) return;
            PhysObj.GravityBeforeCustomGravity = NewGravity;
            PhysObj.BaseGravity = NewGravity;
        }
        PlayerRef.MyPlayerController.SetGravityForceAndDirection(NewGravity, false);
        // By default we'll do player. No need for check.
    }

    public void StartChangeGravity()
    {
        BossAnimationScript.ChangeGravityOfEverything();
    }

    public override void Attack()
    {
        // base.Attack();
        // PUNCH HERE.
        BossAnimationScript.StartedMelee();
    }

    public bool StartAttackThrowObjectsAtPlayer()
    {
        if (PhysObjectInHand == null)
        {
            if (PhysObjectsSuckedIn.Count == 0) return false;
            PhysObjectInHand = PhysObjectsSuckedIn[0];
        }
        PhysObjectInHand.transform.SetParent(this.HandTransform, false);
        PhysObjectInHand.transform.position = this.HandTransform.position;
        BossAnimationScript.StartThrowObject();
        return true;
    }

    public void StartSuckObjectsAround()
    {
        BossAnimationScript.StartSucking();
    }

    public void StartJumpStomp()
    {
        BossAnimationScript.StartJumpStop();
    }

    public void StompHappened()
    {
        // TODO: Stomp.
    }

    public void StartSummonEnemies()
    {
        BossAnimationScript.StartSummonEnemies();
    }

    public void SummonEnemies()
    {
        for (int i = 0; i < EnemySpawnPoints.Count; i++)
        {
            GameObject EnemySpawned = Instantiate(EnemySpawnPrefab, EnemySpawnPoints[i].position, EnemySpawnPoints[i].rotation);
            PlaySummonEffect(EnemySpawnPoints[i].position);
            EnemyBaseCharacter EnemyRef = EnemySpawned.GetComponent<EnemyBaseCharacter>();
            EnemyRef.InitializeEnemySpawned(this.PlayerRef, -EnemySpawnPoints[i].up);
        }
    }

    public void PlaySummonEffect(Vector3 SummonLocation) {
        
    }

    public void SuckStarted()
    {
        BossSuckerField.TurnOnField();
    }

    public void SuckEnded()
    {
        BossSuckerField.TurnOffField();
    }

    public void CaughtPhysicsObjectSuck(PhysicsObjectBasic PhysObj) {
        Debug.Log("Caught");
        this.PhysObjectsSuckedIn.Add(PhysObj);
        PhysObj.RigidbodyRef.isKinematic = true;
        PhysObj.RigidbodyRef.detectCollisions = false;
        PhysObj.transform.SetParent(this.AttachTransform, false);
        PhysObj.transform.position = this.AttachTransform.position;
    }

    public void ThrowObject()
    {
        Vector3 Vel = (PlayerRef.CapsuleCollision.transform.position - HandTransform.position).normalized * BossThrowObjectPower;

        PhysObjectInHand.transform.SetParent(null, true);
        PhysObjectInHand.InitializePhysicsObject(this.MyBossController.BaseGravity, Vel);
        PhysObjectInHand.RigidbodyRef.isKinematic = false;
        PhysObjectInHand.RigidbodyRef.detectCollisions = true;
        PhysObjectInHand.BaseVelocity = Vel;
        PhysObjectInHand.RigidbodyRef.linearVelocity = Vel;
        PhysObjectInHand.GravityBeforeCustomGravity = this.MyBossController.BaseGravity;
        PhysObjectInHand.CheckTimeDilationOnSpawn();
        PhysObjectInHand.UpdatePhysicsObjectBasedOnTimeDilation();
        PhysObjectInHand = null;
        PhysObjectsSuckedIn.RemoveAt(0);
        Debug.Log("Thrown");
    }

    public override void Update()
    {
        // base.Update();
    }

    public override void Die()
    {
        base.Die();
        // TODO: PLAYER WIN.
    }

    public override void Awake()
    {
        //base.Awake();
    }

    public void Jump()
    {
        this.MyBossController.AddMovementInput(-this.MyBossController.GetGravityDirection(), 15000f);
    }
}
