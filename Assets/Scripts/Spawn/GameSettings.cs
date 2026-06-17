using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static string SelectedMode;
    public static readonly string[] ExerciseOrder = { "Tutorial", "Estakada", "Razvorot", "Garage", "Parking" };
    public static int CurrentExerciseIndex = 0;

    // Метод для перехода к следующему упражнению
    public static void SetNextExercise()
    {
        CurrentExerciseIndex = (CurrentExerciseIndex + 1) % ExerciseOrder.Length;
        SelectedMode = ExerciseOrder[CurrentExerciseIndex];
    }

    // Метод для установки конкретного упражнения (используется кнопками меню)
    public static void SetExercise(string mode)
    {
        SelectedMode = mode;
        for (int i = 0; i < ExerciseOrder.Length; i++)
        {
            if (ExerciseOrder[i] == mode)
            {
                CurrentExerciseIndex = i;
                break;
            }
        }
    }
}
