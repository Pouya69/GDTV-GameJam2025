using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public GameObject StompExpansionPrefab;
    public List<Transform> LootSpawnPoints;
    public int AmountOfObjectsThrownBeforeExplosion = 0;
    [NonSerialized] public int AmountOfObjectsThrown = 5;
    [NonSerialized] public Vector3 NewGravity;
    [NonSerialized] public bool IsChangingGravity = false;
    [NonSerialized] public bool IsLookingAtPlayer = false;

    public void SpawnLootForPlayer()
    {
        for (int i = 0; i < LootSpawnPoints.Count; i++)
        {
            ManualItemDrop = GetRandomItemDrop();
            GameObject PrefabToInst = i % 2 == 0 ? HealthConsumablePrefab : AmmoConsumablePrefab;
            GameObject SpawnedLoot = Instantiate(PrefabToInst, LootSpawnPoints[i].position, Quaternion.identity);
            SpawnedLoot.TryGetComponent<PhysicsObjectBasic>(out PhysicsObjectBasic PhysObj);
            PhysObj.BaseGravity = Vector3.down * 9.81f;
        }
    }

    public void ChangeGravityAroundMe()
    {
        Collider[] Colliders = Physics.OverlapSphere(this.CapsuleCollision.transform.position, BossSuckerField.FieldRadius, MyBossController.PhysicsObjectsLayerMaskGroundCheck);

        if (Colliders.Length != 0)
        {
            foreach (Collider ObjInField in Colliders)
            {
                if (!ObjInField.transform.root.TryGetComponent<PhysicsObjectBasic>(out PhysicsObjectBasic PhysObj)) return;
                PhysObj.GravityBeforeCustomGravity = NewGravity;
                PhysObj.BaseGravity = NewGravity;
            }
        }
        
        PlayerRef.MyPlayerController.SetGravityForceAndDirection(NewGravity, false);
        IsChangingGravity = false;
        // By default we'll do player. No need for check.
    }

    public void StartChangeGravity()
    {
        bool IsLeftRight = UnityEngine.Random.value >= 0.5f;
        if (IsLeftRight)
            NewGravity = new Vector3(UnityEngine.Random.value >= 0.5f ? -9.81f : 9.81f, 0, 0);
        else
            NewGravity = new Vector3(0, 0, UnityEngine.Random.value >= 0.5f ? -9.81f : 9.81f);
        IsChangingGravity = true;
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
            if (AmountOfObjectsThrown >= AmountOfObjectsThrownBeforeExplosion)
            {
                for (int i = 0; i < PhysObjectsSuckedIn.Count; i++)
                {
                    Vector3 Vel = UnityEngine.Random.insideUnitSphere;
                    if (Vel.y < 0)
                        Vel.y *= -1;
                    PhysicsObjectBasic PhysObj = PhysObjectsSuckedIn[i];
                    Vel *= BossThrowObjectPower;
                    PhysObj.transform.SetParent(null, true);
                    //PhysObj.InitializePhysicsObject(this.MyBossController.BaseGravity, Vel);
                    PhysObj.RigidbodyRef.isKinematic = false;
                    PhysObj.RigidbodyRef.detectCollisions = true;
                    PhysObj.BaseVelocity = Vel;
                    PhysObj.RigidbodyRef.linearVelocity = Vel;
                    //PhysObj.GravityBeforeCustomGravity = this.MyBossController.BaseGravity;
                    PhysObj.CheckTimeDilationOnSpawn();
                    PhysObj.UpdatePhysicsObjectBasedOnTimeDilation();
                    PhysObj = null;
                    PhysObjectsSuckedIn.RemoveAt(0);
                }
                AmountOfObjectsThrown = 0;
                return false;
            }
            if (PhysObjectsSuckedIn.Count == 0) return false;
            PhysObjectInHand = PhysObjectsSuckedIn[0];
        }
        IsLookingAtPlayer = true;
        PhysObjectInHand.gameObject.SetActive(true);
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
        Instantiate(StompExpansionPrefab, CapsuleCollision.transform.position, Quaternion.identity);
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
            GameObject EnemySpawned = Instantiate(EnemySpawnPrefab, EnemySpawnPoints[i].position, Quaternion.identity);
           // EnemySpawned.transform.rotation = ;
            // PlaySummonEffect(EnemySpawnPoints[i].position);
            EnemyBaseCharacter EnemyRef = EnemySpawned.GetComponent<EnemyBaseCharacter>();
            EnemyRef.InitializeEnemySpawned(this.PlayerRef, Vector3.up);
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
        this.PhysObjectsSuckedIn.Add(PhysObj);
        PhysObj.RigidbodyRef.isKinematic = true;
        PhysObj.RigidbodyRef.detectCollisions = false;
        PhysObj.transform.SetParent(this.AttachTransform, false);
        PhysObj.transform.position = this.AttachTransform.position;
        PhysObj.gameObject.SetActive(false);
    }

    public void ThrowObject()
    {
        Vector3 Vel = (PlayerRef.CapsuleCollision.transform.position - HandTransform.position).normalized * BossThrowObjectPower;

        PhysObjectInHand.transform.SetParent(null, true);
        //PhysObjectInHand.InitializePhysicsObject(this.MyBossController.BaseGravity, Vel);
        PhysObjectInHand.RigidbodyRef.isKinematic = false;
        PhysObjectInHand.RigidbodyRef.detectCollisions = true;
        PhysObjectInHand.BaseVelocity = Vel;
        PhysObjectInHand.RigidbodyRef.linearVelocity = Vel;
        //PhysObjectInHand.GravityBeforeCustomGravity = this.MyBossController.BaseGravity;
        PhysObjectInHand.CheckTimeDilationOnSpawn();
        PhysObjectInHand.UpdatePhysicsObjectBasedOnTimeDilation();
        PhysObjectInHand = null;
        PhysObjectsSuckedIn.RemoveAt(0);
        AmountOfObjectsThrown++;
        Debug.Log("Thrown");
        IsLookingAtPlayer = false;
    }

    public override void Update()
    {
        // base.Update();
    }

    public override void Die()
    {
        //base.Die();

        SceneManager.LoadScene("Credits", LoadSceneMode.Single);

        // TODO: PLAYER WIN.
    }

    public override void Awake()
    {
        //base.Awake();
    }

    public void Jump()
    {
        //this.MyBossController.AddMovementInput(-this.MyBossController.GetGravityDirection(), 15000f);
    }

    public override void ReduceHealth(EDamageType DamageType, float Amount)
    {
        Health -= Amount;
        if (Health <= 0) Die();
        /*
        if (DamageType == EDamageType.SPECIAL_BOSS)
        {
            
        }
        */
    }
}
