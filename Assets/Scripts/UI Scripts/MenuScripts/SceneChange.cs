using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    [SerializeField] private int MenuNumder;

    public void PressButton()
    {
        SceneManager.LoadScene(MenuNumder);
    }
}
