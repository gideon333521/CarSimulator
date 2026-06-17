using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    [SerializeField] private Transform spawn;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject mode;
    [Tooltip("Какой режим активирует этот спавн. Должно совпадать с GameSettings.SelectedMode")]
    [SerializeField] private string modeName;

    void Start()
    {
        if (GameSettings.SelectedMode == modeName)
        {
            Spawn();
        }
    }
    

    public void Spawn()
    {
        playerPrefab.transform.position = spawn.position;
        playerPrefab.transform.rotation = spawn.rotation;
        playerPrefab.SetActive(true);
        mode.SetActive(true);
    }
}
