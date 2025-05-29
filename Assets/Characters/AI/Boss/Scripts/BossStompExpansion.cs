using UnityEngine;

public class BossStompExpansion : MonoBehaviour
{
    public float FieldLifeTime = 4f;
    public SphereCollider SphereOverlapArea;
    public Transform SphereMesh;
    public float FieldRadius = 0.5f;
    public float MaxFieldRadius = 90f;
    public float DamageApplied = 30f;
    public float ExpansionSpeed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(SphereMesh.gameObject, FieldLifeTime);
        Destroy(this.gameObject, FieldLifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        this.SetFieldRadius(Mathf.MoveTowards(this.FieldRadius, this.MaxFieldRadius, Time.deltaTime * ExpansionSpeed));
    }

    public void SetFieldRadius(float InRadius)
    {
        this.SphereMesh.localScale *= InRadius / this.SphereOverlapArea.radius;  // MAKE SURE IT
        this.SphereOverlapArea.radius = InRadius;
        this.FieldRadius = InRadius;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject.TryGetComponent<PlayerCharacter>(out PlayerCharacter PlayerCollided) && PlayerCollided.MyPlayerController.IsOnGround)
            PlayerCollided.ReduceHealth(CharacterBase.EDamageType.FALL_DAMAGE, DamageApplied);
    }
}
