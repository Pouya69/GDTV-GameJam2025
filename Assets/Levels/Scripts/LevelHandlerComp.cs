using UnityEngine;

public class LevelHandlerComp : MonoBehaviour
{
    [SerializeReference] public PlayerCharacter PlayerCharacterRef_InLevel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemyAtLocation(Vector3 SpawnLocation, Vector3 EnemyRotation, GameObject EnemyPrefabClass)
    {
        Instantiate(EnemyPrefabClass, SpawnLocation, Quaternion.Euler(EnemyRotation));
    }
}
