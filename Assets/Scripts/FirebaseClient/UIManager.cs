using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using TMPro;
using System;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [Header("Панели")]
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject profilePanel;

    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OpenLoginPanel()
    {
        
       loginPanel.SetActive(true);
       registerPanel.SetActive(false);
    }

    public void OpenRegistrationPanel()
    {
        registerPanel.SetActive(true);
        loginPanel.SetActive(false);
    }

    public void OpenProfilePanel()
    {
        profilePanel.SetActive(true);
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
    }

    public void SignOutPanel()
    {
        loginPanel.SetActive(true);
        profilePanel.SetActive(false);
    }
}
