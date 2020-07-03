using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Escape : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Terminate()
    {
        try
        {

            if (FindObjectOfType<GameHost>() != null)
            {
                FindObjectOfType<GameHost>().Disconnect();
                FindObjectOfType<GameHost>().server.Shutdown("ServerShutDown");

                FindObjectOfType<GameClient>().Disconnect();
                Destroy(FindObjectOfType<GameClient>().gameObject);
            }
            else
            {
                FindObjectOfType<GameClient>().Disconnect();
                Destroy(FindObjectOfType<GameClient>().gameObject);
            }
        }
        catch
        {

        }
        SceneManager.LoadScene("OnlineLobby");
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (Input.GetKeyDown(KeyCode.Escape) && FindObjectOfType<Chat>().focused == false)
            {
                try
                {
                    if (GameObject.Find("BuyMenu").activeInHierarchy == false)
                    {
                        try
                        {

                            if (FindObjectOfType<GameHost>() != null)
                            {
                                FindObjectOfType<GameHost>().Disconnect();
                                FindObjectOfType<GameHost>().server.Shutdown("Server ShutDown");

                                FindObjectOfType<GameClient>().Disconnect();
                                Destroy(FindObjectOfType<GameClient>().gameObject);
                            }
                            else
                            {
                                FindObjectOfType<GameClient>().Disconnect();
                                Destroy(FindObjectOfType<GameClient>().gameObject);
                            }
                        }
                        catch
                        {

                        }
                        SceneManager.LoadScene("OnlineLobby");
                    }
                }
                catch
                {
                    try
                    {
                        if (FindObjectOfType<GameHost>() != null)
                        {
                            FindObjectOfType<GameHost>().Disconnect();
                            FindObjectOfType<GameHost>().server.Shutdown("Server ShutDown");

                            FindObjectOfType<GameClient>().Disconnect();
                            Destroy(FindObjectOfType<GameClient>().gameObject);
                        }
                        else
                        {
                            FindObjectOfType<GameClient>().Disconnect();
                            Destroy(FindObjectOfType<GameClient>().gameObject);
                        }
                    }
                    catch
                    {

                    }
                    SceneManager.LoadScene("OnlineLobby");
                }
            }
        }
        catch
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                try
                {
                    if (GameObject.Find("BuyMenu").activeInHierarchy == false)
                    {
                        try
                        {

                            if (FindObjectOfType<GameHost>() != null)
                            {
                                FindObjectOfType<GameHost>().Disconnect();
                                FindObjectOfType<GameHost>().server.Shutdown("Server ShutDown");

                                FindObjectOfType<GameClient>().Disconnect();
                                Destroy(FindObjectOfType<GameClient>().gameObject);
                            }
                            else
                            {
                                FindObjectOfType<GameClient>().Disconnect();
                                Destroy(FindObjectOfType<GameClient>().gameObject);
                            }
                        }
                        catch
                        {

                        }
                        SceneManager.LoadScene("OnlineLobby");
                    }
                }
                catch
                {
                    try
                    {
                        if (FindObjectOfType<GameHost>() != null)
                        {
                            FindObjectOfType<GameHost>().Disconnect();
                            FindObjectOfType<GameHost>().server.Shutdown("Server ShutDown");

                            FindObjectOfType<GameClient>().Disconnect();
                            Destroy(FindObjectOfType<GameClient>().gameObject);
                        }
                        else
                        {
                            FindObjectOfType<GameClient>().Disconnect();
                            Destroy(FindObjectOfType<GameClient>().gameObject);
                        }
                    }
                    catch
                    {

                    }
                    SceneManager.LoadScene("OnlineLobby");
                }
            }
        }

    }
}
