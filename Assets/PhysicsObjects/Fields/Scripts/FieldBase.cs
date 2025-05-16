using System;
using System.Collections.Generic;
using UnityEngine;

public class FieldBase : MonoBehaviour
{
    public float FieldAmount = 1.0f;  // This is derrived for all the child classes. TimeDilation, Physics Gravity Multiplier. 1 means normal. 0 means frozen. more than 1 is faster.
    public float FieldLifeTime = 4f;
    public SphereCollider SphereOverlapArea;
    public MeshRenderer SphereMesh;
    public float FieldRadius = 0.5f;
    [NonSerialized] public List<PhysicsObjectBasic> PhysicsObjectsInsideField = new List<PhysicsObjectBasic>();
    [NonSerialized] public List<EnemyBaseCharacter> CharactersInsideField = new List<EnemyBaseCharacter>();

    public virtual void Awake()
    {
        SetFieldRadius(this.FieldRadius);
    }

    public void SetFieldRadius(float InRadius) {
        this.SphereMesh.transform.localScale *= InRadius / this.FieldRadius;
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
        bool IsCharacter = other.TryGetComponent<EnemyBaseCharacter>(out CharacterOverlapping);
        if (IsCharacter)
        {
            CharactersInsideField.Add(CharacterOverlapping);
            return;
        }
        // * MAKE SURE all the physics objects in scene has PhysicsObjectBasic or a child class *
        PhysicsObjectsInsideField.Add(other.GetComponent<PhysicsObjectBasic>());
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        EnemyBaseCharacter CharacterOverlapping;
        bool IsCharacter = other.TryGetComponent<EnemyBaseCharacter>(out CharacterOverlapping);
        if (IsCharacter)
        {
            CharactersInsideField.Remove(CharacterOverlapping);
            return;
        }
        // * MAKE SURE all the physics objects in scene has PhysicsObjectBasic or a child class *
        PhysicsObjectsInsideField.Remove(other.GetComponent<PhysicsObjectBasic>());
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
