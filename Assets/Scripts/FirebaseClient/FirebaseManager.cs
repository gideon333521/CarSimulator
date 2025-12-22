using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using UnityEngine;
public class FirebaseManager : MonoBehaviour
{
    public string databaseUrl = "https://carsimulatordb-default-rtdb.firebaseio.com/";

    void Start()
    {
        SetupFirebase();
    }

    void SetupFirebase()
    {
        Debug.Log("Настройка Firebase...");

        // Проверяем URL
        if (string.IsNullOrEmpty(databaseUrl))
        {
            Debug.LogError("Database URL не указан!");
            return;
        }

        // Убедимся, что URL правильный
        if (!databaseUrl.StartsWith("https://"))
        {
            databaseUrl = "https://" + databaseUrl;
        }

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                // Простейший способ: используем GetInstance с URL
                FirebaseDatabase database = FirebaseDatabase.GetInstance(databaseUrl);

                if (database != null)
                {
                    DatabaseReference dbRef = database.RootReference;
                    FirebaseAuth auth = FirebaseAuth.DefaultInstance;

                    Debug.Log("Firebase настроен успешно!");
                    Debug.Log($"Auth: {auth != null}");
                    Debug.Log($"Database: {database != null}");
                    Debug.Log($"DatabaseRef: {dbRef != null}");

                    // Тестируем
                    TestSetup(dbRef);
                }
                else
                {
                    Debug.LogError("Не удалось получить Database");
                }
            }
            else
            {
                Debug.LogError($"Dependencies error: {task.Result}");
            }
        });
    }

    void TestSetup(DatabaseReference dbRef)
    {
        dbRef.Child("test").SetValueAsync("Hello Firebase").ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Test failed: {task.Exception?.Message}");
            }
            else
            {
                Debug.Log("Test passed! Firebase работает.");
            }
        });
    }

    [ContextMenu("Тест подключения")]
    void TestConnection()
    {
        if (string.IsNullOrEmpty(databaseUrl))
        {
            Debug.LogError("Сначала укажите Database URL");
            return;
        }

        SetupFirebase();
    }
}
