using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string SceneName;
    public float WaitTimer;

    private bool Active;
    public void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.CompareTag("Player") || Active) return;
        Active = true;
        StartCoroutine(StartChangingScene());
    }
    public IEnumerator StartChangingScene()
    {
        yield return new WaitForSeconds(WaitTimer) ;
        SceneManager.LoadSceneAsync(SceneName);
    }
}
