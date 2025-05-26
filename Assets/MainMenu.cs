using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Animator MainMenuUI;
    public GameObject ContiButton;
    public GameObject ContLine;
    public void DisplayMainMenu()
    {
        ContiButton.SetActive(PlayerPrefs.GetInt("Lvl",0) > 0);
        ContLine.SetActive(PlayerPrefs.GetInt("Lvl", 0) > 0);
        MainMenuUI.SetBool("Start", true);
    }
    public void NewGame()
    {
        SavingSystem.Instance.SaveData(100, 30, 0,80);
        SceneManager.LoadSceneAsync("FirstLevel_POUYA");
    }
    public void Continuo()
    {
        int lvl = PlayerPrefs.GetInt("Lvl",0);
        if(lvl == 0) SceneManager.LoadSceneAsync("FirstLevel_POUYA");
        else SceneManager.LoadSceneAsync("ClockTowerHub");
    }
    public void Credits()
    {
        SceneManager.LoadSceneAsync("Credits");
    }
    public void LeaveGame()
    {
        Application.Quit();
    }
}
