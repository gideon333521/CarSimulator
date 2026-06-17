using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CheckPointManager : MonoBehaviour
{
    public static CheckPointManager instance;
    private Vector3 checkPoint;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void SetCheckPoint(Vector3 pos)
    {
      checkPoint = pos;
      Debug.Log("Чекпоинт сохранен:");
    }
}
