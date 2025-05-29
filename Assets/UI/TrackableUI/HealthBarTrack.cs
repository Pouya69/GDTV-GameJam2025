using UnityEngine;
using UnityEngine.UI;

public class HealthBarTrack : MonoBehaviour
{
    public PlayerCharacter character;
    public AnimationCurve FollowPath = AnimationCurve.Linear(0, 0, 1, 1);
    public Vector2 StartPoint;
    public Vector2 MidPoint;
    public Vector2 EndPoint;

    public Image FillBar;
    public RectTransform TxtFollow;
    public Text HealthText;


    public Text BulletsTxt;
    public Text LeftTxt;

    public Image GravityGranede;
    public Image TimeGranede;
    public Image WeaponUI;
    void Update()
    {
        float t = character.GetCurrentHealth() / 100;
        FillBar.fillAmount = t;
        t = FollowPath.Evaluate(t);
        Vector2 a = Vector2.Lerp(EndPoint, MidPoint, t);
        Vector2 b = Vector2.Lerp(MidPoint, StartPoint, t);
        Vector2 final = Vector2.Lerp(a, b, t);

        TxtFollow.anchoredPosition = final;
        HealthText.text = Mathf.RoundToInt(t * 100).ToString();

        WeaponUI.gameObject.SetActive(character.CurrentWeaponEquipped);
        if (character.CurrentWeaponEquipped != null)
        {
            BulletsTxt.text = character.CurrentWeaponEquipped.CurrentBulletsInMagazine.ToString() + "/" + character.CurrentWeaponEquipped.MaxBulletsInMagazine.ToString();
            LeftTxt.text = character.CurrentWeaponEquipped.BulletsLeft.ToString();
        }
        else { BulletsTxt.text = ""; LeftTxt.text = ""; }
    }

    public void GravityGranedeCheck(bool Have) { GravityGranede.gameObject.SetActive(Have); }
    public void TimeGranedeCheck(bool Have) { TimeGranede.gameObject.SetActive(Have); }

    private void Start()
    {
        GravityGranedeCheck(true);
        TimeGranedeCheck(false);
    }
}
