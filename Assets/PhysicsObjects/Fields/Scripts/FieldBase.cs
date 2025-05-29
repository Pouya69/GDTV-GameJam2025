using System;
using System.Collections.Generic;
using UnityEngine;

public class FieldBase : MonoBehaviour
{
    public float FieldAmount = 1.0f;  // This is derrived for all the child classes. TimeDilation, Physics Gravity Multiplier. 1 means normal. 0 means frozen. more than 1 is faster.
    public float FieldLifeTime = 4f;
    public SphereCollider SphereOverlapArea;
    public Transform SphereMesh;
    public float FieldRadius = 0.5f;
    [NonSerialized] public List<PhysicsObjectBasic> PhysicsObjectsInsideField = new List<PhysicsObjectBasic>();
    [NonSerialized] public List<EnemyBaseCharacter> CharactersInsideField = new List<EnemyBaseCharacter>();

    public virtual void Awake()
    {
        SetFieldRadius(this.FieldRadius);
    }

    public virtual void SetFieldAmount(float NewAmount)
    {
        this.FieldAmount = NewAmount;
    }

    public void SetFieldRadius(float InRadius) {
        this.SphereMesh.localScale *= InRadius / this.SphereOverlapArea.radius;  // MAKE SURE IT
        this.SphereOverlapArea.radius = InRadius;
        this.FieldRadius = InRadius;
    }

    public virtual void UpdateOverlappingObjects()
    {
        // This method is defined in the subclasses.
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        EnemyBaseCharacter CharacterOverlapping;
        GameObject RootPrefab = other.transform.root.gameObject;
        // if (RootPrefab.Equals(this.gameObject)) return;
        if (RootPrefab.TryGetComponent<FieldBase>(out _)) return;
        bool IsCharacter = RootPrefab.TryGetComponent<EnemyBaseCharacter>(out CharacterOverlapping);
        if (IsCharacter)
        {
            // Debug.LogWarning(other.gameObject.name);
            CharacterEntered(CharacterOverlapping);
        }
        // * MAKE SURE all the physics objects in scene has PhysicsObjectBasic or a child class *
        // if (!RootPrefab.TryGetComponent<PhysicsObjectBasic>(out PhysicsObjectBasic PhysicsObject)) return;
        PhysicsObjectEntered(RootPrefab.GetComponent<PhysicsObjectBasic>());
    }

    public virtual void CharacterEntered(EnemyBaseCharacter Character)
    {
        CharactersInsideField.Add(Character);
    }

    public virtual void PhysicsObjectEntered(PhysicsObjectBasic PhysicsObject) {
        PhysicsObjectsInsideField.Add(PhysicsObject);
    }

    protected virtual void ResetCharacter(EnemyBaseCharacter Character) { }
    protected virtual void ResetPhysicsObject(PhysicsObjectBasic PhysicsObject) { }

    protected virtual void OnTriggerExit(Collider other)
    {
        EnemyBaseCharacter CharacterOverlapping;
        GameObject RootPrefab = other.transform.root.gameObject;
        if (RootPrefab.TryGetComponent<FieldBase>(out _)) return;
        
        // if (RootPrefab.Equals(this.gameObject)) return;
        bool IsCharacter = RootPrefab.TryGetComponent<EnemyBaseCharacter>(out CharacterOverlapping);
        if (IsCharacter)
        {
            ResetCharacter(CharacterOverlapping);
            CharactersInsideField.Remove(CharacterOverlapping);
            return;
        }
        // * MAKE SURE all the physics objects in scene has PhysicsObjectBasic or a child class *
        // if (!RootPrefab.TryGetComponent<PhysicsObjectBasic>(out PhysicsObjectBasic PhysicsObject)) return;
        PhysicsObjectBasic PhysicsObject = RootPrefab.GetComponent<PhysicsObjectBasic>();
        ResetPhysicsObject(PhysicsObject);
        PhysicsObjectsInsideField.Remove(PhysicsObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        Destroy(gameObject, FieldLifeTime);
    }

    public virtual void OnDestroy()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void FixedUpdate()
    {
        UpdateOverlappingObjects();
    }
}
