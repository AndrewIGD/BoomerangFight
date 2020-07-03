using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Lidgren.Network;
using System;
using System.Linq;
using TMPro;

public class GameClient : MonoBehaviour
{
    public GameObject ctObj;
    public GameObject tObj;

    public InputField playerName;
    public InputField playerTeam;
    public InputField playerClass;

    int charClass = 0;

    public string ip;
    NetPeerConfiguration config;
    public NetClient client;

    public int playerId;

    public GameObject waitingForServer;
    public Text playerCount;
    public GameObject startGame;

    public GameObject[] players;

    string currentName;

    void SendPlayerName()
    {
        if (playerName.text != currentName)
        {
            Debug.Log("sent");

            currentName = playerName.text;

            mesaj += "NewPlayerName " + playerId + ' ' + playerName.text + "\n";
        }
    }

    bool hasPressedButton = false;
    bool buttonPress2 = false;

    void SendPlayerTeam()
    {
        if (playerTeam.text != team.ToString())
        {
            try
            {
                team = int.Parse(playerTeam.text);
                mesaj += "NewPlayerTeam " + playerId + ' ' + playerTeam.text + "\n";
            }
            catch
            {

            }
        }
    }

    void SendPlayerClass()
    {
        if (playerClass.text != charClass.ToString())
        {
            try
            {
                charClass = int.Parse(playerClass.text);
                if(charClass >= 1 && charClass <= 5)
                mesaj += "NewPlayerClass " + playerId + ' ' + playerClass.text + "\n";
            }
            catch
            {

            }
        }
    }

    string spawnPlayers = "";


    public bool upnp = false;
    // Start is called before the first frame update
    public void Disconnect()
    {
        var message3 = client.CreateMessage();
        message3.Write("Disconnect");

        client.SendMessage(message3,
        NetDeliveryMethod.ReliableOrdered);
    }
    public void OnDestroy()
    {
        var message3 = client.CreateMessage();
        message3.Write("Disconnect");

        client.SendMessage(message3,
        NetDeliveryMethod.ReliableOrdered);
    }
    public void OnApplicationQuit()
    {
        var message3 = client.CreateMessage();
        message3.Write("Disconnect");

        client.SendMessage(message3,
        NetDeliveryMethod.ReliableOrdered);
    }
    public void Begin()
    {
        if (FindObjectsOfType<GameClient>().Length >= 2 && FindObjectOfType<GameHost>() == null)
            Destroy(gameObject);
        else
        {

            try
            {
                players = new GameObject[999];
                config = new NetPeerConfiguration("Boomerangs");

                config.EnableUPnP = true;
                config.AutoFlushSendQueue = true;
                config.AutoExpandMTU = true;

                config = new NetPeerConfiguration("Boomerangs")
                {
                    EnableUPnP = true,
                    AcceptIncomingConnections = true,
                    PingInterval = 1f,
                    ResendHandshakeInterval = 1f,
                    MaximumHandshakeAttempts = 15,
                    ConnectionTimeout = 1000f,
                    ReceiveBufferSize = PlayerPrefs.GetInt("ClientReceiveBuffer", 3072),
                    SendBufferSize = PlayerPrefs.GetInt("ClientSendBuffer", 1024)
                };

                config.EnableMessageType(NetIncomingMessageType.StatusChanged);

                client = new NetClient(config);
                client.Start();

                client.Connect(host: ip, port: 25567);
                Invoke("TestConnection", 3.5f);
                Invoke("SendMessage", 0.25f);
                Invoke("ActivateMessage", 0.015625f);
            }
            catch(Exception err)
            {
                
            }
        }
    }

    public bool canSendMsg = false;
    void ActivateMessage()
    {
        canSendMsg = true;
        Invoke("ActivateMessage", 0.015625f);
    }
    void TestConnection()
    {
        if (playerId == 0)
        {
            var message = client.CreateMessage();
            message.Write("Disconnect");
            client.SendMessage(message,
            NetDeliveryMethod.ReliableOrdered);
            Destroy(gameObject);
        }
    }

    string mesaj = "";

    void SendFullMessage()
    {
        try
        {
            mesaj += currentPos + "\n";
            mesaj += shoot + "\n";

            var message = client.CreateMessage();
            message.Write(mesaj);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            mesaj = "";
            currentPos = "";
            shoot = "";
            canSendMsg = false;
        }
        catch
        {
            mesaj = "";
        }
    }

    void SendMessage()
    {

        mesaj += "NewPlayer" + "\n";
    }
    bool detected = false;
    public bool canDo = false;
    public float timeBetweenResponses = 0;
    string currentPos = "";
    string shoot = "";
    public void Send(string message)
    {
        if (message.Contains("PlayerTarget"))
        {
            currentPos = message;
        }
        if (message.Contains("Shoot"))
        {
            shoot = message;
        }
        else if (message.Contains("Msg"))
        {
            mesaj += message + "\n";
        }
        else if (canSendMsg)
        {
            mesaj += message + "\n";
        }
    }
    public int team;
    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name.Contains("OnlineWaitMenu"))
            spawnPlayers = "";
        if(spawnPlayers != "")
        {
            if(FindObjectOfType<SpawnedPlayers>().spawned == false)
            {

                string[] lines = spawnPlayers.Split('\n');

                foreach (string line in lines)
                {
                    try
                    {
                        string data = line;

                        bool ok = true;

                        foreach(Player p in FindObjectsOfType<Player>())
                        {
                            if (p.Controllable && p.PlayerNumber == int.Parse(data.Split(' ')[1]))
                            {
                                ok = false;
                            }
                        }

                        if (ok)
                        {
                            string[] parameters = data.Split(' ');

                            GameObject player = null;

                            foreach(Player pl in FindObjectsOfType<Player>())
                            {
                                if(pl.Controllable == false && pl.type == int.Parse(parameters[4]))
                                {
                                    player = Instantiate(pl.gameObject);
                                }
                            }

                            if (int.Parse(data.Split(' ')[1]) == playerId)
                            {
                                player.GetComponent<Player>().controlling = true;
                            }

                                players[int.Parse(data.Split(' ')[1])] = player;

                            player.GetComponent<Player>().PlayerNumber = int.Parse(data.Split(' ')[1]);

                            player.GetComponent<Player>().Controllable = true;

                            player.GetComponent<Player>().Team = int.Parse(parameters[3]);

                            player.GetComponent<Player>().Name.GetComponent<TextMeshPro>().text = parameters[2];

                            Vector2 spawnPos = new Vector2(float.Parse(parameters[5]), float.Parse(parameters[6]));

                            player.transform.position = spawnPos;

                            
                        }
                    }
                    catch
                    {

                    }
                }
                if (FindObjectOfType<GameHost>() != null)
                     FindObjectOfType<GameHost>().testing = true;
                spawnPlayers = "";
                FindObjectOfType<SpawnedPlayers>().spawned = true;
            }
        }

        buttonPress2 = false;
        try
        {
            GameObject.Find("Ping").GetComponent<Text>().text = (int)(client.ServerConnection.AverageRoundtripTime * 1000) + "ms";

            playerName = GameObject.Find("PlayerName").GetComponent<InputField>();
            playerTeam = GameObject.Find("PlayerTeam").GetComponent<InputField>();
            playerClass = GameObject.Find("PlayerClass").GetComponent<InputField>();

            playerName.onValueChanged.AddListener(delegate { SendPlayerName(); });
            playerTeam.onValueChanged.AddListener(delegate { SendPlayerTeam(); });
            playerClass.onValueChanged.AddListener(delegate { SendPlayerClass(); });

            if (buttonPress2 == false)
                hasPressedButton = false;
           

        }
        catch
        {

        }

        if (client != null)
        {


            try
            {
                if (FindObjectOfType<GameHost>() != null)
                {
                    GameObject.Find("Start").SetActive(true);
                }
                else
                {
                    GameObject.Find("Start").SetActive(false);
                }
            }
            catch
            {

            }

            timeBetweenResponses += Time.deltaTime;
            if (timeBetweenResponses >= 10f)
            {
                SceneManager.LoadScene("OnlineLobby");
                mesaj += "Disconnect" + "\n";
                if (FindObjectOfType<GameHost>() != null)
                    Destroy(FindObjectOfType<GameHost>().gameObject);
                Destroy(gameObject);
            }

            if (canSendMsg)
                mesaj += "ClientAlive " + playerId + "\n";

            int size = 0;

            string buyMenuText = "";

            bool setTime = false;


            List<NetIncomingMessage> messages = new List<NetIncomingMessage>();
            client.ReadMessages(messages);
            foreach (NetIncomingMessage message in messages)
            {

                string text = message.ReadString();

                size+=System.Text.ASCIIEncoding.UTF32.GetByteCount(text);

                string[] lines = text.Split('\n');

                foreach (string line in lines)
                {

                    try
                    {
                        string data = line;
                        string type = data.Split(' ')[0];
                        if (type == "ReceiveId")
                        {
                            playerId = int.Parse(data.Split(' ')[1]);
                        }
                        else if (type.Contains("Conn"))
                        {
                            var message3 = client.CreateMessage();
                            message3.Write("Connected");

                            client.SendMessage(message3,
                            NetDeliveryMethod.ReliableOrdered);
                        }
                        else if (type == "ServerAlive")
                        {
                            timeBetweenResponses = 0;
                        }
                        else if (type == "PlayerCount")
                        {
                            try
                            {
                                playerCount = GameObject.Find("PlayerCount").GetComponent<Text>();
                                playerCount.text = "Player Count: " + data.Split(' ')[1];
                            }
                            catch
                            {

                            }
                        }
                        else if (type == "LoadScene")
                        {
                                 SceneManager.LoadScene(data.Split(' ')[1]);
                        }
                        else if (type == "Disconnect" || (type.Contains("Server")&&type.Contains("ServerAlive")==false))
                        {
                            SceneManager.LoadScene("OnlineLobby");

                            if (FindObjectOfType<GameHost>() != null)
                                Destroy(FindObjectOfType<GameHost>().gameObject);

                            Destroy(gameObject);
                        }
                        else if (type == "StartGame")
                        {
                            string[] parameters = data.Split(' ');

                            try
                            {
                                if (parameters[1].Contains("OnlineLobby"))
                                    SceneManager.LoadScene("OnlineWaitMenu");
                                else SceneManager.LoadScene("OnlineFight" + parameters[1]);
                            }
                            catch
                            {
                                SceneManager.LoadScene("OnlineWaitMenu");
                            }

                        }
                        else if (type == "SpawnPlayer")
                        {
                            try
                            {
                                spawnPlayers += line + "\n";

                                
                            }
                            catch (Exception err)
                            {
                                Debug.Log(err.ToString());
                            }
                        }
                        else if (type == "PlayerInfo" )
                        {
                            if (FindObjectOfType<GameHost>() == null)
                            {
                                string[] parameters = data.Split(' ');

                                int id = int.Parse(parameters[1]);

                                players[id].transform.localEulerAngles = new Vector3(0, 0, float.Parse(parameters[4]));
                                players[id].GetComponent<Player>().health = float.Parse(parameters[5]);

                                players[id].GetComponent<Player>().BarResizer.transform.localScale = new Vector2(players[id].GetComponent<Player>().health / players[id].GetComponent<Player>().maxHealth, 1);

                                Vector2 playerPos = new Vector2(float.Parse(parameters[2]), float.Parse(parameters[3]));

                                if (Vector2.Distance(players[id].transform.position, playerPos) > 0.1f)
                                {
                                    if (players[id].GetComponent<Player>().timeAlive > 1)
                                    {
                                        players[id].GetComponent<Player>().interpolationCurrentPos = players[id].transform.position;
                                        players[id].GetComponent<Player>().interpolationTime = 0;
                                        players[id].GetComponent<Player>().interpolationTarget = playerPos;
                                    }
                                    else players[id].GetComponent<Player>().transform.position = playerPos;
                                }

                                players[id].GetComponent<Player>().movementDirection = new Vector2(float.Parse(parameters[6]), float.Parse(parameters[7]));

                                players[id].GetComponent<Animator>().Play(parameters[8]);

                                Debug.Log(parameters[9]);

                                players[id].GetComponent<Player>().armsAnimator.Play(parameters[9]);

                                players[id].GetComponent<Player>().thrownBoomerang.gameObject.SetActive(bool.Parse(parameters[10]));
                                players[id].GetComponent<Player>().boomerang.SetActive(!bool.Parse(parameters[10]));

                                Vector2 playerPos2 = new Vector2(float.Parse(parameters[11]), float.Parse(parameters[12]));

                                if (Vector2.Distance(players[id].GetComponent<Player>().thrownBoomerang.transform.position, playerPos2) > 0.1f)
                                {

                                        players[id].GetComponent<Player>().thrownBoomerang.interpolationCurrentPos = players[id].GetComponent<Player>().thrownBoomerang.transform.position;
                                        players[id].GetComponent<Player>().thrownBoomerang.interpolationTime = 0;
                                        players[id].GetComponent<Player>().thrownBoomerang.interpolationTarget = playerPos2;

                                }

                                players[id].GetComponent<Player>().thrownBoomerang.GetComponent<Rigidbody2D>().velocity = new Vector2(float.Parse(parameters[13]), float.Parse(parameters[14]));


                                players[id].GetComponent<Player>().thrownBoomerang.onlineOldPos =  players[id].GetComponent<Player>().boomerang.transform.position;

                                players[id].GetComponent<Player>().thrownBoomerang.onlineNewPos = players[id].GetComponent<Player>().thrownBoomerang.transform.position = playerPos2;

                            }
                        }
                        else if(type == "Play")
                        {
                            string[] parameters = data.Split(' ');

                            if(parameters.Length == 2)
                            {
                                GameObject obj = new GameObject();
                                obj.transform.position = Vector3.zero;
                                obj.AddComponent<AudioSource>();
                                obj.GetComponent<AudioSource>().clip = FindObjectOfType<SoundeArchive>().sounds[int.Parse(parameters[1])];
                                obj.GetComponent<AudioSource>().Play();
                                Destroy(obj, obj.GetComponent<AudioSource>().clip.length + 1);
                            }
                            else if(parameters.Length == 4)
                            {
                                GameObject obj = new GameObject();
                                obj.transform.position = new Vector3(float.Parse(parameters[2]),float.Parse(parameters[3]), 0);

                                obj.AddComponent<AudioSource>();
                                obj.GetComponent<AudioSource>().clip = FindObjectOfType<SoundeArchive>().sounds[int.Parse(parameters[1])];

                                obj.GetComponent<AudioSource>().spatialBlend = 1f;
                                obj.GetComponent<AudioSource>().spread = 360f;
                                obj.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
                                obj.GetComponent<AudioSource>().dopplerLevel = 0f;
                                obj.GetComponent<AudioSource>().maxDistance = 30f;
                                obj.GetComponent<AudioSource>().minDistance = 5f;

                                if (parameters[1] == "14" || parameters[1] == "17")
                                {
                                    obj.GetComponent<AudioSource>().maxDistance = 60f;
                                    obj.GetComponent<AudioSource>().minDistance = 10f;
                                }
                                if (parameters[1] == "25" || parameters[1] == "26")
                                {
                                    obj.GetComponent<AudioSource>().maxDistance = 45f;
                                    obj.GetComponent<AudioSource>().minDistance = 7.5f;
                                }


                                List<AudioSource> asList = new List<AudioSource>();

                                foreach (AudioSource a in FindObjectsOfType<AudioSource>())
                                {
                                    if (Vector2.Distance(a.transform.position, Camera.main.transform.position) < 10f && a.clip == obj.GetComponent<AudioSource>().clip && a != obj.GetComponent<AudioSource>())
                                    {
                                        asList.Add(a);
                                    }
                                }

                                for (int i = asList.Count - 20; i >= 0; i--)
                                {
                                    Destroy(asList[i].gameObject);
                                }

                                obj.GetComponent<AudioSource>().Play();
                                Destroy(obj, obj.GetComponent<AudioSource>().clip.length + 1);
                            }
                            else if(parameters.Length == 5)
                            {
                                GameObject obj = new GameObject();

                                int id = int.Parse(parameters[4]);
                                obj.transform.parent = players[id].transform;

                                obj.transform.position = new Vector3(float.Parse(parameters[2]), float.Parse(parameters[3]), 0);
                                obj.AddComponent<AudioSource>();
                                if (parameters[1] == "14")
                                    obj.GetComponent<AudioSource>().volume = 0.75f;

                                obj.GetComponent<AudioSource>().clip = FindObjectOfType<SoundeArchive>().sounds[int.Parse(parameters[1])];


                                obj.GetComponent<AudioSource>().spatialBlend = 1f;
                                obj.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
                                obj.GetComponent<AudioSource>().spread = 360f;
                                obj.GetComponent<AudioSource>().dopplerLevel = 0f;
                                obj.GetComponent<AudioSource>().maxDistance = 30f;
                                obj.GetComponent<AudioSource>().minDistance = 5f;

                                if(parameters[1] == "13" || parameters[1] == "12")
                                {
                                    obj.GetComponent<AudioSource>().maxDistance = 60f;
                                    obj.GetComponent<AudioSource>().minDistance = 20f;
                                }

                                List<AudioSource> asList = new List<AudioSource>();

                                foreach (AudioSource a in FindObjectsOfType<AudioSource>())
                                {
                                    if (Vector2.Distance(a.transform.position, Camera.main.transform.position) < 10f && a.clip == obj.GetComponent<AudioSource>().clip && a != obj.GetComponent<AudioSource>())
                                    {
                                        asList.Add(a);
                                    }
                                }

                                for (int i = asList.Count - 20; i >= 0; i--)
                                {
                                    Destroy(asList[i].gameObject);
                                }


                                obj.GetComponent<AudioSource>().Play();
                                Destroy(obj, obj.GetComponent<AudioSource>().clip.length + 1);
                            }
                        }
                        else if(type == "Msg")
                        {
                            string[] parameters = data.Split(' ');

                            if ((parameters[1] == "0" && int.Parse(parameters[2]) == team)||parameters[1] == "1")
                            {
                                string msg = string.Join(" ", parameters.Skip(3));



                                FindObjectOfType<Chat>().AddMessage(msg);
                            }
                        }
                        else if (type == "Destroy")
                        {
                            string[] parameters = data.Split(' ');

                            int id = int.Parse(parameters[1]);

                            Destroy(players[id].GetComponent<Player>().thrownBoomerang.gameObject);

                            Destroy(players[id]);
                        }
                        else if (type == "Damage")
                        {
                            string[] parameters = data.Split(' ');

                            int id = int.Parse(parameters[1]);

                            players[id].GetComponent<Player>().Damage();
                        }
                    }
                    catch(Exception err)
                    {
                        Debug.Log(err.ToString());
                    }

                }


            }
            

        }
        if (canSendMsg)
            SendFullMessage();

    }
    int gameTime;
}
