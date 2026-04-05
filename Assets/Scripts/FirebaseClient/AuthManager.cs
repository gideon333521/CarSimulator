using UnityEngine;
using Firebase;
using Firebase.Auth;
using System;
using System.Threading.Tasks;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.ComponentModel;
public class AuthManager : MonoBehaviour
{
    public DependencyStatus dependencyStatus;
    public FirebaseUser user;
    private FirebaseAuth auth;

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

    void InitializeFirebase()
    {
        //Set the default instance object
        auth = FirebaseAuth.DefaultInstance;

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

     void AuthStateChanged(object sender, System.EventArgs eventArgs)
     {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
                UIManager.Instance.SignOutPanel();
            }

            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                IDText.text = user.UserId;
                NameText.text = user.DisplayName;
                UIManager.Instance.OpenProfilePanel();
            }
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

            Debug.Log(failedMessage);
            ErrorSignText.text = failedMessage;
            ErrorSignText.color = Color.red;
        }
        else
        {
            user = loginTask.Result.User;
            UserProfile nameProfile = new UserProfile { DisplayName = References.Name };
            UserProfile surnameProfile = new UserProfile { DisplayName = References.Surname };
            UserProfile patronymicProfile = new UserProfile { DisplayName = References.Patronymic };

            var updateProfileTask = user.UpdateUserProfileAsync(nameProfile);
            updateProfileTask = user.UpdateUserProfileAsync(surnameProfile);
            updateProfileTask = user.UpdateUserProfileAsync(patronymicProfile);
            ErrorSignText.color = Color.green;
            ErrorSignText.text = "Вы успешно авторизовались, здравствуйте" + user.DisplayName;
            Debug.LogFormat("{0} Вы успешно авторизовались", user.DisplayName);
            UIManager.Instance.OpenProfilePanel();
            IDText.text = user.UserId;
            NameText.text = user.DisplayName;
            SurnameText.text = surnameProfile.DisplayName;
            PatronymicText.text = patronymicProfile.DisplayName;
        }
    }

    public void SignOut()
    {
        if (auth != null && user != null)
        {
            auth.SignOut();
            UIManager.Instance.SignOutPanel();
        }
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
                UserProfile surnameProfile = new UserProfile { DisplayName = surname };
                UserProfile patronymicProfile = new UserProfile { DisplayName = patronymic };

                var updateProfileTask = user.UpdateUserProfileAsync(nameProfile);
                updateProfileTask = user.UpdateUserProfileAsync(surnameProfile);
                updateProfileTask = user.UpdateUserProfileAsync(patronymicProfile);



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
                    Debug.Log("Регистрация прошла успешна, Добро пожаловать " +  user.DisplayName);
                    ErrorRegText.text = "Регистрация прошла успешна, Добро пожаловать " +  user.DisplayName;
                    ErrorRegText.color = Color.green;
                    UIManager.Instance.OpenProfilePanel();
                    IDText.text = user.UserId;
                    NameText.text = nameProfile.DisplayName;
                    SurnameText.text = surnameProfile.DisplayName;
                    PatronymicText.text = patronymicProfile.DisplayName;
                }
            }
        }
    }
}
