using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class AfterEffects : MonoBehaviour
{
    [Header("Time Freeze Effect")]
    [SerializeField] private Volume FreezeTimeEffect;
    private float TimeTransition = 0;
    [SerializeField] private float FreezeTransitionSpeed;
    [SerializeField] private GameObject WorldScanner;
    [SerializeField] private float ScannerDuration;

    [Header("Blur Effect")]
    [SerializeField] private Volume BlurEffect;
    private float BlurTransition;
    [SerializeField] private float BlueSpeed;

    [Header("Getting Hit")]
    [SerializeField] private Volume HitEffect;
    private float HitTransition;
    [SerializeField] private float HitSpeed;
    public void FreezeTime(bool State)
    {
        if (State) StartCoroutine(FreezeTime());
        else StartCoroutine(UnFreezeTime());
    }

    public void BlueEffect()
    {
        StartCoroutine(BluringEffect());
    }
    #region Time Freeze Effect
    private IEnumerator FreezeTime()
    {
        StartCoroutine(BluringEffect());
        TimeTransition = 0;
        GameObject Scanner = Instantiate(WorldScanner, transform);
        Destroy(Scanner, ScannerDuration);
        while (TimeTransition < 1)
        {
            TimeTransition += FreezeTransitionSpeed * Time.deltaTime;
            FreezeTimeEffect.weight = Mathf.Lerp(FreezeTimeEffect.weight, 1, TimeTransition);
            yield return 0.05f;
        }
    }
    private IEnumerator UnFreezeTime()
    {
        TimeTransition = 0;
        while (TimeTransition < 1)
        {
            TimeTransition += FreezeTransitionSpeed * Time.deltaTime;
            FreezeTimeEffect.weight = Mathf.Lerp(FreezeTimeEffect.weight, 0, TimeTransition);
            yield return 0.05f;
        }
    }

    #endregion

    #region BlurEffect
    public IEnumerator BluringEffect()
    {
        BlurTransition = 0;
        while (BlurTransition < 1)
        {
            BlurTransition += BlueSpeed * Time.deltaTime;
            BlurEffect.weight = Mathf.Lerp(BlurEffect.weight, 1, BlurTransition);
            yield return 0.005f;
        }
        StartCoroutine(ExitBlurEffect());
    }
    public IEnumerator ExitBlurEffect()
    {
        BlurTransition = 0;
        while (BlurTransition < 1)
        {
            BlurTransition += BlueSpeed * Time.deltaTime;
            BlurEffect.weight = Mathf.Lerp(BlurEffect.weight, 0, BlurTransition);
            yield return 0.005f;
        }
    }
    #endregion
    #region Hit Effect

    public void GettingHit()
    {
        StartCoroutine(HitingEffect());
    }
    private IEnumerator HitingEffect()
    {
        HitTransition = 0;
        while (HitTransition < 1)
        {
            HitTransition += HitSpeed * Time.deltaTime;
            HitEffect.weight = Mathf.Lerp(HitEffect.weight, 1, HitTransition);
            yield return 0.005f;
        }
        StartCoroutine(ExitHitEffect());
    }
    private IEnumerator ExitHitEffect()
    {
        HitTransition = 0;
        while (HitTransition < 1)
        {
            HitTransition += HitSpeed * Time.deltaTime;
            HitEffect.weight = Mathf.Lerp(HitEffect.weight, 0, HitTransition);
            yield return 0.005f;
        }
    }
    #endregion
    #region Time Freeze Bomb

    public void BombTimeFreeze()
    {
        StartCoroutine(BombFreezeTime());
    }
    public void ExitBombTimeFreeze()
    {
        StartCoroutine(BombUnFreezeTime());
    }
    private IEnumerator BombFreezeTime()
    {
        StartCoroutine(BluringEffect());
        TimeTransition = 0;
        while (TimeTransition < 1)
        {
            TimeTransition += FreezeTransitionSpeed * Time.deltaTime;
            FreezeTimeEffect.weight = Mathf.Lerp(FreezeTimeEffect.weight, 1, TimeTransition);
            yield return 0.05f;
        }
    }
    private IEnumerator BombUnFreezeTime()
    {
        TimeTransition = 0;
        while (TimeTransition < 1)
        {
            TimeTransition += FreezeTransitionSpeed * Time.deltaTime;
            FreezeTimeEffect.weight = Mathf.Lerp(FreezeTimeEffect.weight, 0, TimeTransition);
            yield return 0.05f;
        }
    }

    #endregion
}
