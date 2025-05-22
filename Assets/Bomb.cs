using UnityEngine;

public class Bomb : MonoBehaviour
{
    public AnimationCurve BombCurve;
    public float DesiredScale = 1f;
    public float Duration = 1f;

    private float timer = 0f;

    void Update()
    {
        if (timer < Duration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / Duration;
            float scaleValue = BombCurve.Evaluate(normalizedTime) * DesiredScale;
            transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        }
        else Destroy(gameObject);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.CompareTag("Player")) return;
        other.gameObject.GetComponent<AfterEffects>().BombTimeFreeze();
    }
    public void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        other.gameObject.GetComponent<AfterEffects>().ExitBombTimeFreeze();
    }
}
