using UnityEngine;

public class LevelTriggers : MonoBehaviour
{
    public GameObject NextObj;
    public GameObject MyObj;
    public void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.gameObject.CompareTag("Player")) return;
        NextObj.SetActive(true);
        MyObj.SetActive(false);
        Destroy(gameObject);
    }
}
