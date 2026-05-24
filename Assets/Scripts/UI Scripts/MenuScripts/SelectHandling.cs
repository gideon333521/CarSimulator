using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectHandling : MonoBehaviour
{
    [SerializeField] private Dropdown SelectorHandling;
    private CarController cr;
    void Start()
    {
        cr = GetComponent<CarController>();
        Select<CarController.HandlingState>(SelectorHandling);

    }

    void Select<target>(Dropdown dropdown) where target : Enum
    {
        dropdown.ClearOptions();
        var enumValues = Enum.GetValues(typeof(target))
            .Cast<target>()
            .Select(e => e.ToString())
            .ToList();
        dropdown.AddOptions(enumValues);   
    }
}
