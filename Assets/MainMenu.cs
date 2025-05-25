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
        //Reset Saved Data
        SceneManager.LoadSceneAsync("FirstLevel_POUYA");
    }
    public void Continuo()
    {
        //Load Data
        //SceneManager.LoadSceneAsync("ClockTowerHub");
    }
    public void Credits()
    {
        //Show Credits Menus

    }
    public void ReturnMainmenu()
    {
        //Hide Credits
        //Show MainMenu
    }
    public void LeaveGame()
    {
        Application.Quit();
    }
}
