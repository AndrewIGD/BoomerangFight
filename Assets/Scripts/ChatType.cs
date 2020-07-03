using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatType : MonoBehaviour
{
    public bool all = true;

    public void ChangeType()
    {
        all = !all;
        if (all)
            GetComponentInChildren<Text>().text = "All";
        else GetComponentInChildren<Text>().text = "Team";

        FindObjectOfType<Chat>().CancelCanvas();
    }
}
