using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using Random = UnityEngine.Random;

public class MainMenu : MonoBehaviour
{
    public GameObject nameInput;

    public TextAsset animalNamesFile;
    public TextAsset adjectivesFile;

    // Start is called before the first frame update
    void Start()
    {
        nameInput.GetComponent<TMP_InputField>().text = GetUserName();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public string GetUserName()
    {
        string userName = PlayerPrefs.GetString("UserName", GenerateRandomUserName());
        SetUserName(userName);
        return userName;
    }

    public void SetUserName(string userName)
    {
        PlayerPrefs.SetString("UserName", userName);
    }

    private string GenerateRandomUserName()
    {
        string name = "";
        string[] lines;

        // existing name


        // generate random name
        lines = adjectivesFile.text.Split('\n');
        name += toCamelCase(lines[Random.Range(0, lines.Length - 1)]);
        name += " ";
        lines = animalNamesFile.text.Split('\n');
        name += toCamelCase(lines[Random.Range(0, lines.Length - 1)]);

        return name;
    }

    private string toCamelCase(string s)
    {
        string ret = "";
        bool capNext = true;

        for(int i = 0; i < s.Length; i++)
        {
            if(capNext)
                ret += Char.ToUpper(s[i]);
            else
                ret += Char.ToLower(s[i]);

            capNext = false;
            if(ret[i] == ' ' || ret[i] == '-')
            {
                capNext = true;
            }
        }
        return ret;
    }
}
