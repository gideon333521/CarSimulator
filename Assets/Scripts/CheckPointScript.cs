using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class CheckPointScript : MonoBehaviour
{
    public enum CheckType
    {
        Result,
        Next
    }
    public CheckType check;
    [SerializeField] private GameObject ResultPanel;
    [SerializeField] private GameObject MainPanel;
    [SerializeField] private GameObject LogPanel;
    [SerializeField] private Text Text;
    [SerializeField] private Light light;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            
            switch (check)
            {
                case CheckType.Result:
                    CheckPointManager.instance.SetCheckPoint(transform.position);
                    Time.timeScale = 0;
                    ResultPanel.SetActive(true);
                    MainPanel.SetActive(false);
                    LogPanel.SetActive(false);
                    break;
                case CheckType.Next:
                    CheckPointManager.instance.SetCheckPoint(transform.position);
                    light.enabled = false;
                    break;
            }
        }
    }
}
