using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.ComponentModel;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class AuthManager : MonoBehaviour
{
    public DependencyStatus dependencyStatus;
    private FirebaseUser user;
    private FirebaseAuth auth;
    private DatabaseReference reference;

    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;

    public TMP_InputField registerEmail;
    public TMP_InputField registerPassword;
    public TMP_InputField registerSurname;
    public TMP_InputField registerName;
    public TMP_InputField registerPatronymic;

    public TextMeshProUGUI IDText;
    public TextMeshProUGUI SurnameText;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI PatronymicText;
    public TextMeshProUGUI ErrorRegText;
    public TextMeshProUGUI ErrorSignText;

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all firebase dependencies: " + dependencyStatus);
            }
        });
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
     {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }


            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
     }

    void InitializeFirebase()
    {
        //Записать default instance
        auth = FirebaseAuth.DefaultInstance;

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    private IEnumerator CheckForAutoLogin()
    {
        if (user != null)
        {
            var reloadUserTask = user.ReloadAsync();

            yield return new WaitUntil(() => reloadUserTask.IsCompleted);
            AutoLogin();
        }
        else 
        {
            UIManager.Instance.OpenLoginPanel();
        }
    }

     public void AutoLogin()
     {
        if (user != null)
        {
            UIManager.Instance.OpenProfilePanel();
        }
        else 
        {
            UIManager.Instance.OpenLoginPanel();
        }
     }

    private IEnumerator CheckAndFixDepedemciesAsync()
    {
        var dependencyyTask = FirebaseApp.CheckAndFixDependenciesAsync();

        yield return new WaitUntil(() => dependencyyTask.IsCompleted);

        dependencyStatus = dependencyyTask.Result;

        if (dependencyStatus == DependencyStatus.Available)
        {
            InitializeFirebase();
            yield return new WaitForEndOfFrame();
            StartCoroutine(CheckForAutoLogin());
        }
        else
        {
            Debug.LogError("Could not resolve all firebase dependencies: " + dependencyStatus);
        }
    }


    public void Login()
    {
        StartCoroutine(LoginAsync(loginEmail.text, loginPassword.text));
    }

    private IEnumerator LoginAsync(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogError(loginTask.Exception);

            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;


            string failedMessage = "Вход провален! ";

            switch (authError)
            {
                case AuthError.InvalidEmail:
                    failedMessage += "Неверный email";
                    break;
                case AuthError.WrongPassword:
                    failedMessage += "Неверный пароль";
                    break;
                default: 
                    failedMessage = "Вход провален";
                    break;
            }

            Debug.Log(failedMessage + authError);
            ErrorSignText.text = failedMessage + authError;
            ErrorSignText.color = Color.red;
        }
        else
        {
           user = loginTask.Result.User;
           ErrorSignText.color = Color.green;
           ErrorSignText.text = "Вы успешно авторизовались, здравствуйте" + user.DisplayName;
           Debug.LogFormat("{0} Вы успешно авторизовались", user.DisplayName);
           UIManager.Instance.OpenProfilePanel();
           IDText.text = user.UserId;
           References.Name = user.DisplayName;
        }
    }

    public void SignOut()
    {
       auth.SignOut();
       UIManager.Instance.SignOutPanel();
    }

    public void Register()
    {
        StartCoroutine(RegisterAsync(registerName.text, registerSurname.text, registerPatronymic.text, registerEmail.text, registerPassword.text));
    }

    private IEnumerator RegisterAsync(string name, string surname, string patronymic, string email, string password)
    {
        if (name == "")
        {
            Debug.LogError("Заполните имя");
        }

        if (surname == "")
        {
            Debug.LogError("Заполните фамилию");
        }

        if (patronymic == "")
        {
            Debug.LogError("Заполните отчество");
        }

        if (email == "")
        {
            Debug.LogError("Заполните почту");
        }

        if (password == "")
        {
            Debug.LogError("Заполните пароль");
        }
        else
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(() => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                Debug.LogError(registerTask.Exception);

                FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;

                string failedMessage = "Registration Failed! Becuase ";
                switch (authError)
                {
                    case AuthError.InvalidEmail:
                        failedMessage += "Email is invalid";
                        break;
                    case AuthError.WrongPassword:
                        failedMessage += "Wrong Password";
                        break;
                    case AuthError.MissingEmail:
                        failedMessage += "Email is missing";
                        break;
                    case AuthError.MissingPassword:
                        failedMessage += "Password is missing";
                        break;
                    default:
                        failedMessage = "Registration Failed";
                        break;
                }

                Debug.Log(failedMessage);
                ErrorRegText.text = failedMessage +  authError;
                ErrorRegText.color = Color.red;
            }  
            else
            {
                // Get The User After Registration Success
                user = registerTask.Result.User;
                UserProfile nameProfile = new UserProfile { DisplayName = name };
                var updateProfileTask = user.UpdateUserProfileAsync(nameProfile);
                yield return new WaitUntil(() => updateProfileTask.IsCompleted);

                if (updateProfileTask.Exception != null)
                {
                    // Delete the user if user update failed 
                    user.DeleteAsync();

                    Debug.LogError(updateProfileTask.Exception);

                    FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError authError = (AuthError)firebaseException.ErrorCode;

                    string failedMessage = "Profile update Failed! Becuase ";
                    switch (authError)
                    {
                        case AuthError.InvalidEmail:
                            failedMessage += "Email is invalid";
                            break;
                        case AuthError.WrongPassword:
                            failedMessage += "Wrong Password";
                            break;
                        case AuthError.MissingEmail:
                            failedMessage += "Email is missing";
                            break;
                        case AuthError.MissingPassword:
                            failedMessage += "Password is missing";
                            break;
                        default:
                            failedMessage = "Profile update Failed";
                            break;
                    }

                    Debug.Log(failedMessage);
                    ErrorRegText.text = failedMessage +  authError;
                }
                else
                {
                    if (user.DisplayName != null)
                    {
                        Debug.Log("Регистрация прошла успешна, Добро пожаловать " +  user.DisplayName);
                        ErrorRegText.text = "Регистрация прошла успешна, Добро пожаловать " +  user.DisplayName;
                        ErrorRegText.color = Color.green;
                        UIManager.Instance.OpenProfilePanel();
                        IDText.text = user.UserId;
                        References.Name = NameText.text;
                        References.Surname = SurnameText.text;
                        References.Patronymic = PatronymicText.text;
                        
                    }
                }
            }
        }
    }
}
