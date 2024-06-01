using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabInputField : MonoBehaviour
{
    [SerializeField] List<GameObject> inputList;
    EventSystem system;
    void Start()
    {
        system = EventSystem.current;
        //inputList = new List<GameObject>();
        //InputField[] array = transform.GetComponentsInChildren<InputField>();
        //for (int i = 0; i < array.Length; i++)
        //{
        //    inputList.Add(array[i].gameObject);
        //}
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //Debug.Log("0000");
            if (inputList.Contains(system.currentSelectedGameObject))
            {
                //正序
                GameObject next = NextInput(system.currentSelectedGameObject);
                system.SetSelectedGameObject(next);
                //倒序
                //GameObject last = LastInput(system.currentSelectedGameObject);
                //system.SetSelectedGameObject(last);
            }
        }
    }
    //获取上一个物体
    private GameObject LastInput(GameObject input)
    {
        int indexNow = IndexNow(input);
        if (indexNow - 1 >= 0)
        {
            return inputList[indexNow - 1].gameObject;
        }
        else
        {
            return inputList[inputList.Count - 1].gameObject;
        }
    }
    //获取当前选中物体的序列
    private int IndexNow(GameObject input)
    {
        int indexNow = 0;
        for (int i = 0; i < inputList.Count; i++)
        {
            if (input == inputList[i])
            {
                indexNow = i;
                break;
            }
        }
        return indexNow;
    }

    //获取下一个物体
    private GameObject NextInput(GameObject input)
    {
        int indexNow = IndexNow(input);
        if (indexNow + 1 < inputList.Count)
        {
            return inputList[indexNow + 1].gameObject;
        }
        else
        {
            return inputList[0].gameObject;
        }
    }
}

