using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string SceneName;
    public float WaitTimer;

    public GameObject Fade;
    private bool Active;
    public PlayerController controller;

    public bool SaveGame;
    public int Level;
    public void OnCollisionEnter(Collision other)
    {
        if (!other.transform.root.CompareTag("Player") || Active) return;
        controller.IsMovementDisabled = true;
        Active = true;
        Fade.SetActive(true);
        if(SaveGame)
        {
            SavingSystem.Instance.SaveData(controller.PlayerCharacterRef.GetCurrentHealth(),controller.PlayerCharacterRef.CurrentWeaponEquipped.CurrentBulletsInMagazine,Level,controller.PlayerCharacterRef.CurrentWeaponEquipped.BulletsLeft);
        }
        else
        {
            SavingSystem.Instance.SaveData(controller.PlayerCharacterRef.GetCurrentHealth(), controller.PlayerCharacterRef.CurrentWeaponEquipped.CurrentBulletsInMagazine, PlayerPrefs.GetInt("Lvl"), controller.PlayerCharacterRef.CurrentWeaponEquipped.BulletsLeft);
        }
        StartCoroutine(StartChangingScene());
    }
    public IEnumerator StartChangingScene()
    {
        yield return new WaitForSeconds(WaitTimer);
        SceneManager.LoadSceneAsync(SceneName);
    }
}
