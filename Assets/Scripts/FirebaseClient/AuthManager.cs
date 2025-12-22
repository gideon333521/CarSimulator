using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
public class AuthManager : MonoBehaviour
{
    //События для UI
    public event Action<UsersData> OnLoginSuccess;
    public event Action<string> OnLoginFailed;
    public event Action<UsersData> OnRegisterSuccess;
    public event Action<string> OnRegisterFailed;
    public event Action OnLogout;

    // Статический доступ
    public static AuthManager Instance { get; private set; }

    // Текущий пользователь
    public UsersData CurrentUser { get; private set; }
    public FirebaseUser FirebaseUser { get; private set; }

    // Ссылки Firebase
    private FirebaseAuth auth;
    private DatabaseReference database;

    // Флаги инициализации
    private bool isInitialized = false;
    private bool isInitializing = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeFirebase();
    }

    // Инициализация Firebase
    private void InitializeFirebase()
    {
        if (isInitializing || isInitialized) return;

        isInitializing = true;
        Debug.Log("Начинаем инициализацию Firebase...");
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            try
            {
                var dependencyStatus = task.Result;

                if (dependencyStatus == DependencyStatus.Available)
                {
                    auth = FirebaseAuth.DefaultInstance;
                    database = FirebaseDatabase.DefaultInstance.RootReference;
                    isInitialized = true;

                    Debug.Log("Firebase Auth initialized");

                    // Проверяем, есть ли сохраненная сессия
                    CheckAutoLogin();
                }
                else
                {
                    Debug.LogError($"Could not resolve Firebase dependencies: {dependencyStatus}");
                    OnLoginFailed?.Invoke($"Ошибка инициализации Firebase: {dependencyStatus}");
                }

            }
            catch (Exception ex)
            {
                Debug.LogError($"Исключение при инициализации Firebase: {ex.Message}");
                OnLoginFailed?.Invoke($"Ошибка инициализации: {ex.Message}");
            }
            finally
            {
                isInitializing = false;
            }
        });
    }

    // Проверка автоматического входа
    private void CheckAutoLogin()
    {
        if (auth.CurrentUser != null)
        {
            FirebaseUser = auth.CurrentUser;
            LoadUserData(FirebaseUser.UserId);
        }
    }

    public void Register(string email, string password, string surname, string name, string patronymic)
    {
        if (!isInitialized)
        {
            OnRegisterFailed?.Invoke("Firebase не инициализирован");
        }

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(surname) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(patronymic))
        {
            OnRegisterFailed?.Invoke("Заполните все поля");
            return;
        }

        if (password.Length < 6)
        {
            OnRegisterFailed?.Invoke("Пароль должен содержать минимум 6 символов");
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, password)
         .ContinueWithOnMainThread(task =>
         {
             if (task.IsCanceled)
             {
                 OnRegisterFailed?.Invoke("Регистрация отменена");
                 return;
             }

             if (task.IsFaulted)
             {
                 HandleRegistrationError(task.Exception);
                 return;
             }

             // Регистрация успешна
             AuthResult result = task.Result;
             FirebaseUser = result.User;

             // Создаем данные пользователя
             CreateUserData(FirebaseUser.UserId, email, surname, name, patronymic);
         });
    }

    // Создание данных пользователя в базе данных
    private void CreateUserData(string id, string email, string surname, string name, string patronymic)
    {
        UsersData userData = new UsersData(id, email, surname, name, patronymic);

        // Сохраняем в Realtime Database
        database.Child("users").Child(id).SetRawJsonValueAsync(userData.ToJson())
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"Ошибка сохранения данных: {task.Exception}");
                    OnRegisterFailed?.Invoke("Ошибка сохранения данных пользователя");

                    // Удаляем созданного пользователя, если не удалось сохранить данные
                    DeleteUser();
                    return;
                }

                CurrentUser = userData;
                OnRegisterSuccess?.Invoke(userData);
                Debug.Log($"Пользователь зарегистрирован: {email}");
            });
    }

    // Удаление пользователя при ошибке сохранения данных
    private async void DeleteUser()
    {
        if (FirebaseUser != null)
        {
            try
            {
                await FirebaseUser.DeleteAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка удаления пользователя: {ex.Message}");
            }
        }
    }

    // ========== АВТОРИЗАЦИЯ ==========

    // Вход с email и паролем
    public void LoginWithEmail(string email, string password)
    {
        if (!isInitialized)
        {
            OnLoginFailed?.Invoke("Firebase не инициализирован");
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    OnLoginFailed?.Invoke("Вход отменен");
                    return;
                }

                if (task.IsFaulted)
                {
                    HandleLoginError(task.Exception);
                    return;
                }

                // Вход успешен
                AuthResult result = task.Result;
                FirebaseUser = result.User;

                // Загружаем данные пользователя
                LoadUserData(FirebaseUser.UserId);
            });
    }

    // Загрузка данных пользователя из базы
    private void LoadUserData(string userId)
    {
        database.Child("users").Child(userId).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"Ошибка загрузки данных: {task.Exception}");
                    OnLoginFailed?.Invoke("Ошибка загрузки данных пользователя");
                    return;
                }

                DataSnapshot snapshot = task.Result;

                if (!snapshot.Exists)
                {
                    // Данные пользователя не найдены
                    OnLoginFailed?.Invoke("Данные пользователя не найдены");
                    return;
                }

                // Десериализуем данные
                string json = snapshot.GetRawJsonValue();
                CurrentUser = UsersData.FromJson(json);

                // Обновляем дату последнего входа
                UpdateLastLogin();

                OnLoginSuccess?.Invoke(CurrentUser);
                Debug.Log($"Пользователь вошел: {CurrentUser.Email}");
            });
    }

    // Обновление даты последнего входа
    private void UpdateLastLogin()
    {
        if (CurrentUser == null) return;

        CurrentUser.lastLoginDate = DateTime.Now;
        database.Child("users").Child(CurrentUser.id)
            .Child("lastLoginDate").SetValueAsync(CurrentUser.lastLoginDate.ToString());
    }
    public void Logout()
    {
        if (auth != null)
        {
            auth.SignOut();
            CurrentUser = null;
            FirebaseUser = null;
            OnLogout?.Invoke();
            Debug.Log("Пользователь вышел");
        }
    }

    // ========== ОБРАБОТКА ОШИБОК ==========

    private void HandleRegistrationError(Exception exception)
    {
        string errorMessage = "Ошибка регистрации";

        if (exception is AggregateException aggregateException)
        {
            foreach (var innerException in aggregateException.Flatten().InnerExceptions)
            {
                if (innerException is FirebaseException firebaseException)
                {
                    AuthError errorCode = (AuthError)firebaseException.ErrorCode;
                    errorMessage = GetErrorMessage(errorCode);
                    break;
                }
            }
        }

        OnRegisterFailed?.Invoke(errorMessage);
    }

    private void HandleLoginError(Exception exception)
    {
        string errorMessage = "Ошибка входа";

        if (exception is AggregateException aggregateException)
        {
            foreach (var innerException in aggregateException.Flatten().InnerExceptions)
            {
                if (innerException is FirebaseException firebaseException)
                {
                    AuthError errorCode = (AuthError)firebaseException.ErrorCode;
                    errorMessage = GetErrorMessage(errorCode);
                    break;
                }
            }
        }

        OnLoginFailed?.Invoke(errorMessage);
    }

    private string GetErrorMessage(AuthError errorCode)
    {
        switch (errorCode)
        {
            case AuthError.InvalidEmail:
                return "Неверный формат email";
            case AuthError.EmailAlreadyInUse:
                return "Email уже используется";
            case AuthError.WeakPassword:
                return "Слабый пароль. Используйте минимум 6 символов";
            case AuthError.WrongPassword:
                return "Неверный пароль";
            case AuthError.UserNotFound:
                return "Пользователь не найден";
            case AuthError.TooManyRequests:
                return "Слишком много попыток. Попробуйте позже";
            case AuthError.NetworkRequestFailed:
                return "Ошибка сети. Проверьте подключение";
            case AuthError.MissingEmail:
                return "Введите email";
            case AuthError.MissingPassword:
                return "Введите пароль";
            default:
                return $"Ошибка: {errorCode}";
        }
    }

    // ========== ПРОВЕРКИ ==========

    public bool IsLoggedIn()
    {
        return CurrentUser != null && FirebaseUser != null;
    }

    public string GetUserDisplayName()
    {
        if (CurrentUser == null) return "Гость";

        if (!string.IsNullOrEmpty(CurrentUser.Patronymic))
        {
            return $"{CurrentUser.Surname} {CurrentUser.Name} {CurrentUser.Patronymic}";
        }

        return $"{CurrentUser.Surname} {CurrentUser.Name}";
    }

    // ========== ОБНОВЛЕНИЕ ДАННЫХ ==========

    public void UpdateUserProfile(string surname, string name, string patronymic)
    {
        if (CurrentUser == null) return;

        CurrentUser.Surname = surname;
        CurrentUser.Name = name;
        CurrentUser.Patronymic = patronymic;

        // Обновляем в базе данных
        database.Child("users").Child(CurrentUser.id)
            .Child("surname").SetValueAsync(surname);
        database.Child("users").Child(CurrentUser.id)
            .Child("name").SetValueAsync(name);
        database.Child("users").Child(CurrentUser.id)
            .Child("patronymic").SetValueAsync(patronymic);
    }
}
