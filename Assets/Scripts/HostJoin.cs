using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lidgren.Network;
using System.Net;
using System;
using UnityEngine.SceneManagement;

public class HostJoin : MonoBehaviour
{
    public InputField ipAdress;

    public void HostGame()
    {
        try
        {
            if (FindObjectOfType<GameClient>() != null)
            {
                Destroy(FindObjectOfType<GameClient>().gameObject);
            }
            GameObject host = new GameObject();
            host.AddComponent<GameHost>();
            host.name = "GameHost";
            DontDestroyOnLoad(host);
            try
            {
                host.GetComponent<GameHost>().Begin();
            }
            catch (Exception err)
            {
                Destroy(host);
                SceneManager.LoadScene("OnlineLobby");
                return;
            }
            GameObject client = new GameObject();
            client.AddComponent<GameClient>();
            var localHost = Dns.GetHostEntry(Dns.GetHostName());
            string ownIp = "";
            foreach (var ip in localHost.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ownIp = ip.ToString();
                }
            }
            Debug.Log(ownIp);
            client.GetComponent<GameClient>().ip = ownIp;
            client.name = "GameClient";
            DontDestroyOnLoad(client);
            client.GetComponent<GameClient>().Begin();
        }
        catch (Exception ex)
        {
            FindObjectOfType<InputField>().text = ex.ToString();
        }
    }
    public void JoinGame()
    {
        try
        {
            GameObject client = new GameObject();
            client.AddComponent<GameClient>();
            client.name = "GameClient";
            DontDestroyOnLoad(client);
            client.GetComponent<GameClient>().ip = ipAdress.text;
            client.GetComponent<GameClient>().Begin();
        }
        catch (Exception err)
        {
            TextEditor te = new TextEditor();
            te.text = err.ToString();
            te.SelectAll();
            te.Copy();
        }
    }

    public void Settings()
    {
        SceneManager.LoadScene("Settings");
    }

    public bool changeMouseScroll = false;
    int mouseScroll = 0;

    public bool changeShowInv = false;
    int showInv = 0;
    private void Start()
    {
        if (changeMouseScroll)
        {
            mouseScroll = PlayerPrefs.GetInt("MouseScroll", 0);
            if (mouseScroll == 0)
                GetComponentInChildren<Text>().text = "Switch with Scroll: OFF";
            else if (mouseScroll == 1)
                GetComponentInChildren<Text>().text = "Switch with Scroll: ON";
        }

        if (changeShowInv)
        {
            showInv = PlayerPrefs.GetInt("ShowInv", 0);
            if (showInv == 0)
                GetComponentInChildren<Text>().text = "Permanently Show Inventory: OFF";
            else if (showInv == 1)
                GetComponentInChildren<Text>().text = "Permanently Show Inventory: ON";
        }
    }
    public void ChangeMouseScroll()
    {
        if (mouseScroll == 0)
        {
            mouseScroll = 1;

        }
        else mouseScroll = 0;

        PlayerPrefs.SetInt("MouseScroll", mouseScroll);
        if (mouseScroll == 0)
            GetComponentInChildren<Text>().text = "Switch with Scroll: OFF";
        else if (mouseScroll == 1)
            GetComponentInChildren<Text>().text = "Switch with Scroll: ON";
    }

    public void ChangeShowInv()
    {
        if (showInv == 0)
        {
            showInv = 1;

        }
        else showInv = 0;

        PlayerPrefs.SetInt("ShowInv", showInv);
        if (showInv == 0)
            GetComponentInChildren<Text>().text = "Permanently Show Inventory: OFF";
        else if (showInv == 1)
            GetComponentInChildren<Text>().text = "Permanently Show Inventory: ON";
    }

    public void GoLobby()
    {
        SceneManager.LoadScene("OnlineLobby");
    }

    public void Graf()
    {
        SceneManager.LoadScene("Graffiti");
    }

    public void Buffer()
    {
        SceneManager.LoadScene("Buffer");
    }
}
