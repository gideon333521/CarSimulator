using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSelectMode : MonoBehaviour
{
    public void SelectGarage()
    {
        GameSettings.SelectedMode = "Garage";
        GameSettings.SetExercise("Garage");
        SceneManager.LoadScene("Autodrom");
    }

    public void SelectEstakada()
    {
        GameSettings.SelectedMode = "Estakada";
        GameSettings.SetExercise("Estakada");
        SceneManager.LoadScene("Autodrom");
    }

    public void SelectRazvorot()
    {
        GameSettings.SelectedMode = "Razvorot";
        GameSettings.SetExercise("Razvorot");
        SceneManager.LoadScene("Autodrom");
    }

    public void SelectParking()
    {
        GameSettings.SelectedMode = "ParallelParking";
        GameSettings.SetExercise("ParallelParking");
        SceneManager.LoadScene("Autodrom");
    }

    public void SelectTutorial()
    {
        GameSettings.SelectedMode = "Tutorial";
        GameSettings.SetExercise("Tutorial");
        SceneManager.LoadScene("Autodrom");
    }
}
