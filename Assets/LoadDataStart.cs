using UnityEngine;

public class LoadDataStart : MonoBehaviour
{
    public PlayerController controller;
    public void Start()
    {
        //controller.PlayerCharacterRef PlayerPrefs.GetFloat("HP", 100);
        if (controller.PlayerCharacterRef.HasWeaponEquipped())
        {
            controller.PlayerCharacterRef.CurrentWeaponEquipped.CurrentBulletsInMagazine = PlayerPrefs.GetInt("Bullets", controller.PlayerCharacterRef.CurrentWeaponEquipped.MaxBulletsInMagazine);
            controller.PlayerCharacterRef.CurrentWeaponEquipped.BulletsLeft = PlayerPrefs.GetInt("LeftBullet", 60);
        }
        controller.PlayerCharacterRef.SetHealthDirectly(PlayerPrefs.GetFloat("HP", 100));
    }
}
