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

    void Update()
    {
        float t = character.GetCurrentHealth();
        FillBar.fillAmount = t;
        t = FollowPath.Evaluate(t);
        Vector2 a = Vector2.Lerp(EndPoint, MidPoint, t);
        Vector2 b = Vector2.Lerp(MidPoint, StartPoint, t);
        Vector2 final = Vector2.Lerp(a, b, t);

        TxtFollow.anchoredPosition = final;
        HealthText.text = Mathf.RoundToInt(t * character.MaxHealth).ToString();
    }
}
