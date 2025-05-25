using UnityEngine;

public class SavingSystem : MonoBehaviour
{
    public static SavingSystem Instance;
    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public void SaveData(float HP , int Bullets ,int LVL,int LBullets)
    {
        PlayerPrefs.SetFloat("HP",HP);
        PlayerPrefs.SetInt("Bullets", Bullets);
        PlayerPrefs.SetInt("Lvl", LVL);
        PlayerPrefs.SetInt("LeftBullet", LBullets);
    }
    public void RestartNewGame()
    {
        PlayerPrefs.SetFloat("HP", 100);
        PlayerPrefs.SetInt("Bullets", 0);
        PlayerPrefs.SetInt("Lvl", 0);
    }
}
