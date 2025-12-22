using System;
using UnityEngine;

[Serializable]
public class UsersData
{
    public string id;
    public string Email;
    public string Surname;
    public string Name;
    public string Patronymic;
    public DateTime registrationDate;
    public DateTime lastLoginDate;


    public UsersData(string id, string email, string surname, string name, string patronymic)
    {
        this.id = id;
        this.Email = email;
        this.Surname = surname;
        this.Name = name;
        this.Patronymic = patronymic;
    }

    // Для сериализации в JSON
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    // Для десериализации из JSON
    public static UsersData FromJson(string json)
    {
        return JsonUtility.FromJson<UsersData>(json);
    }
}
