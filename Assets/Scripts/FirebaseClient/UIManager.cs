using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class UIManager : MonoBehaviour
{
    [Header("Панели")]
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject profilePanel;

    [Header("Логин")]
    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;
    public Button loginButton;
    public Button goToRegisterButton;
    public TextMeshProUGUI loginErrorText;

    [Header("Регистрация")]
    public TMP_InputField registerEmail;
    public TMP_InputField registerPassword;
    public TMP_InputField registerSurname;
    public TMP_InputField registerName;
    public TMP_InputField registerPatronymic;
    public Button registerButton;
    public Button goToLoginButton;
    public TextMeshProUGUI registerErrorText;

    [Header("Главная панель")]
    public TextMeshProUGUI welcomeText;
    public TextMeshProUGUI userInfoText;
    public Button logoutButton;
    void Start()
    {
        // Подписываемся на события AuthManager
        AuthManager.Instance.OnLoginSuccess += OnLoginSuccess;
        AuthManager.Instance.OnLoginFailed += OnLoginFailed;
        AuthManager.Instance.OnRegisterSuccess += OnRegisterSuccess;
        AuthManager.Instance.OnRegisterFailed += OnRegisterFailed;
        AuthManager.Instance.OnLogout += OnLogout;

        loginButton.onClick.AddListener(OnLoginClicked);
        registerButton.onClick.AddListener(OnRegisterClicked);
        goToRegisterButton.onClick.AddListener(ShowRegisterPanel);
        goToLoginButton.onClick.AddListener(ShowLoginPanel);
        logoutButton.onClick.AddListener(OnLogoutClicked);

        loginErrorText.gameObject.SetActive(false);
        registerErrorText.gameObject.SetActive(false);

        // Показываем соответствующую панель
        if (AuthManager.Instance.IsLoggedIn())
        {
            ShowProfilePanel();
        }
        else
        {
            ShowLoginPanel();
        }

    }

    private void OnLoginClicked()
    {
        string email = registerEmail.text.Trim();
        string password = loginPassword.text.Trim();
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowLoginError("Заполните все поля");
            return;
        }
        AuthManager.Instance.LoginWithEmail(email, password);
    }

    private void OnRegisterClicked()
    {
        string email = registerEmail.text.Trim();
        string password = loginPassword.text.Trim();
        string surname = registerSurname.text.Trim();
        string name = registerName.text.Trim();
        string patronymic = registerPatronymic.text.Trim();

        if (string.IsNullOrEmpty(email) || 
            string.IsNullOrEmpty(password) || 
            string.IsNullOrEmpty(surname) ||
            string.IsNullOrEmpty(name) || 
            string.IsNullOrEmpty(patronymic))
        {
            ShowRegisterError("Заполните обязательные поля (помечены *)");
            return;
        }

        if (password.Length < 6)
        {
            ShowRegisterError("Пароль должен содержать минимум 6 символов");
            return;
        }

        AuthManager.Instance.Register(email, password, surname, name, patronymic);
    }

    private void OnLogoutClicked()
    {
        AuthManager.Instance.Logout();
    }

    private void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        profilePanel.SetActive(false);

        // Очистка полей
        loginPassword.text = "";
        loginErrorText.gameObject.SetActive(false);
    }

    private void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        profilePanel.SetActive(false);

        // Очистка полей
        registerPassword.text = "";
        loginErrorText.gameObject.SetActive(false);
    }

    private void ShowProfilePanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        profilePanel.SetActive(true);
        UpdateUserInfo();
    }

    private void OnLoginSuccess(UsersData userData)
    {
        ShowProfilePanel();
        Debug.Log($"Вход успешно выполнен {userData.Email}");
    }

    private void OnLoginFailed(string error)
    {
        ShowLoginError(error);
    }

    private void OnRegisterSuccess(UsersData userData)
    {
        ShowProfilePanel();
        Debug.Log($"Вход успешен: {userData.Email}");
    }

    private void OnRegisterFailed(string error)
    {
        ShowRegisterError(error);
    }

    private void OnLogout()
    {
        ShowLoginPanel();
    }

    private void ShowLoginError(string message, bool isError = true)
    {
        loginErrorText.text = message;
        loginErrorText.color = isError ? Color.red : Color.green;
        loginErrorText.gameObject.SetActive(true);
    }

    private void ShowRegisterError(string message, bool isError = true)
    {
        registerErrorText.text = message;
        registerErrorText.color = isError ? Color.red : Color.green;
        registerErrorText.gameObject.SetActive(true);
    }



    private void UpdateUserInfo()
    {
        if (AuthManager.Instance.IsLoggedIn())
        {
            welcomeText.text = $"Добро пожаловать, {AuthManager.Instance.GetUserDisplayName()}!";

            var user = AuthManager.Instance.CurrentUser;
            userInfoText.text = $"Email: {user.Email}\n" +
                              $"Зарегистрирован: {user.registrationDate.ToShortDateString()}\n" +
                              $"Последний вход: {user.lastLoginDate.ToShortDateString()}";
        }
    }
}
