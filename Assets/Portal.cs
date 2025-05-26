using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{

    public enum PortalState
    {
        None,
        Locked,
        Unlocked
    }
    public PortalState State;
    public string SceneName;
    public float WaitTimer;

    public GameObject Fade;
    private bool Active;
    public PlayerController controller;

    public bool SaveGame;
    public int Level;
    public GameObject LockedObject;
    public GameObject UnlockObject;

    public int UnlockLvl;
    private void Start()
    {
        if (State == PortalState.None) return;
        LockedObject.SetActive(PlayerPrefs.GetInt("Lvl",0) < UnlockLvl);
        UnlockObject.SetActive(PlayerPrefs.GetInt("Lvl",0) >= UnlockLvl);
    }
    public void OnCollisionEnter(Collision other)
    {
        if (!other.transform.root.CompareTag("Player") || Active) return;
        if(State != PortalState.None)
        {
            if (PlayerPrefs.GetInt("Lvl", 0) < UnlockLvl) return;
        }
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
