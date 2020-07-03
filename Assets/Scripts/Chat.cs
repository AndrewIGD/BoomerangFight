using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.EventSystems;

public class Chat : MonoBehaviour
{
    public bool hovered = false;

    public GameObject canvas;
    public GameObject typeBox;
    public GameObject chat;

    bool selected = false;

    // Start is called before the first frame update
    void Start()
    {
        if (FindObjectsOfType<Chat>().Length >= 2)
            Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);

    }

    public void AddMessage(string message)
    {
        chat.GetComponent<Text>().text += message+"\n";

        canvas.SetActive(true);
    }

    public void MouseEnter()
    {
        hovered = true;
    }

    public void MouseExit()
    {
        hovered = false;
    }

    public bool focused = false;

    void CanvasOff()
    {
        CancelInvoke("CanvasOff");
        canvas.SetActive(false);
    }

    public void CancelCanvas()
    {
        CancelInvoke("CanvasOff");
    }

    // Update is called once per frame
    void Update()
    {
        if (hovered == false && Input.GetMouseButtonDown(0))
            focused = false;
        else if (hovered && Input.GetMouseButtonDown(0))
            focused = true;
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (canvas.activeInHierarchy == false)
            {
                canvas.SetActive(true);
            }
            if(focused == false)
            {
                typeBox.GetComponent<InputField>().Select();
            }
            else if(typeBox.GetComponent<InputField>().text != "")
            {
                int all = 0;
                if (FindObjectOfType<ChatType>().all)
                    all = 1;
                if (all == 1)
                {
                    if (FindObjectOfType<GameClient>().players[FindObjectOfType<GameClient>().playerId].GetComponent<Player>().Team == 0)
                        FindObjectOfType<GameClient>().Send("Msg " + 1 + " " + 0 + " <color=aqua>" + FindObjectOfType<GameClient>().players[FindObjectOfType<GameClient>().playerId].GetComponent<Player>().Name.GetComponent<TextMeshPro>().text + "</color>: " + typeBox.GetComponent<InputField>().text);
                    else FindObjectOfType<GameClient>().Send("Msg " + 1 + " " + 1 + " <color=red>" + FindObjectOfType<GameClient>().players[FindObjectOfType<GameClient>().playerId].GetComponent<Player>().Name.GetComponent<TextMeshPro>().text + "</color>: " + typeBox.GetComponent<InputField>().text);
                }
                else
                {
                    if (FindObjectOfType<GameClient>().players[FindObjectOfType<GameClient>().playerId].GetComponent<Player>().Team == 0)
                        FindObjectOfType<GameClient>().Send("Msg " + 0 + " " + 0 + " <color=aqua>(Team)" + FindObjectOfType<GameClient>().players[FindObjectOfType<GameClient>().playerId].GetComponent<Player>().Name.GetComponent<TextMeshPro>().text + "</color>: " + typeBox.GetComponent<InputField>().text);
                    else FindObjectOfType<GameClient>().Send("Msg " + 0 + " " + 1 + " <color=red>(Team)" + FindObjectOfType<GameClient>().players[FindObjectOfType<GameClient>().playerId].GetComponent<Player>().Name.GetComponent<TextMeshPro>().text + "</color>: " + typeBox.GetComponent<InputField>().text);
                }
                typeBox.GetComponent<InputField>().text = "";
            }
            focused = true;
        }
        if(focused == false && canvas.activeInHierarchy)
        {
            Invoke("CanvasOff", 5f);
        }
        else
        {
            CancelInvoke("CanvasOff");
        }
    }
}
