using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UserData : MonoBehaviour
{
    //public static string name;
    //public static string number;
    //public static string gender;
    //public static string age;
    public static string[] data;
    [SerializeField] TMP_InputField[] text_list;
    [SerializeField] GameObject[] reminder;
    string nullValue;

    
    // Start is called before the first frame update
    void Start()
    {
        data = new string[text_list.Length];
        nullValue = data[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clickRecord() {
        int d = 0;
        for(int i =0; i<text_list.Length; i++)
        {
            data[i] = text_list[i].text;
            //print(text_list[i].text);
            //print(data[i]);
            if (data[i].Length == 0)
            {
                reminder[i].SetActive(true);
            }
            else
            {
                reminder[i].SetActive(false);
                d += 1;
            }
        }
        if (d == text_list.Length)
        {
            SceneManager.LoadScene("Start");
        }
        //Debug.Log(name);
    }
}
