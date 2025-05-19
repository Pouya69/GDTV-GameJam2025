using UnityEngine;

public class EnemyBaseCharacter : CharacterBase
{
    public EnemyBaseController MyEnemyController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
    }

    public override void Awake()
    {
        base.Awake();
        if (this.CurrentWeaponEquipped != null)
        {
            this.CurrentWeaponEquipped.AddedWeaponToCharacter(this);
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void Attack()
    {
        base.Attack();
        if (CurrentWeaponEquipped == null) return;
        // Enemy Shoot at player logic.
        CurrentWeaponEquipped.StartShooting();
    }

    public override void StopShootingWeapon()
    {
        base.StopShootingWeapon();
    }

    public Vector3 GetRandomShotSpreadDirection(Vector3 TargetShotDirection)
    {
        // TODO
        return TargetShotDirection;
    }

}
