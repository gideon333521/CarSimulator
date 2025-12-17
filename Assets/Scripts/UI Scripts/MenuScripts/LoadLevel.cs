using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadLevel : MonoBehaviour
{
    public int LevelNumber;
    public void PressButton()
    {
        SceneManager.LoadScene(LevelNumber);
    }
}
