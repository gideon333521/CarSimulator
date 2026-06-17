using UnityEngine;
using UnityEngine.SceneManagement;

public class NextMode : MonoBehaviour
{
    public void NextExercise()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        GameSettings.SetNextExercise();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // перезагружаем текущую сцену
    }
}
