using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Lidgren.Network;
using System.Linq;
using System;
using UnityEngine.VFX;
using System.Net;
using System.IO;
using TMPro;

public class GameHost : MonoBehaviour
{
    public List<string> animationList2;
    public List<string> animationList;

    public List<string> playerNames;
    public List<int> playerTeams;
    public List<int> playerClasses;

    public List<NetConnection> clientConnections;
    public int players = 1;
    NetPeerConfiguration config;
    public NetServer server;
    // Start is called before the first frame update

    public void Disconnect()
    {
        var toAll = server.CreateMessage();
        toAll.Write("Disconnect");
        server.SendMessage(toAll, recipients: clientConnections, NetDeliveryMethod.ReliableOrdered, 0);
    }
    public void OnDestroy()
    {
        var toAll = server.CreateMessage();
        toAll.Write("Disconnect");
        server.SendMessage(toAll, recipients: clientConnections, NetDeliveryMethod.ReliableOrdered, 0);
    }
    public bool upnp = false;
    public void Begin()
    {
        if (FindObjectsOfType<GameHost>().Length >= 2)
            Destroy(gameObject);
        else
        {


            playerNames = new List<string>();
            playerTeams = new List<int>();
            playerClasses = new List<int>();

            animationList = new List<string>();
            animationList2 = new List<string>();
            clientsConnected = new List<bool>();
            timeOut = new List<float>();

            animationList.Add("idle");
            animationList.Add("run");

            animationList2.Add("idle");
            animationList2.Add("throw");


            SceneManager.LoadScene("OnlineWaitMenu");
            clientConnections = new List<NetConnection>();
            config = new NetPeerConfiguration("Boomerangs")
            { Port = 25567, ConnectionTimeout = 10, EnableUPnP = true, AutoFlushSendQueue = true, AutoExpandMTU = true };

            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);


            config = new NetPeerConfiguration("Boomerangs")
            {
                EnableUPnP = true,
                Port = 25567,
                AcceptIncomingConnections = true,
                PingInterval = 1f,
                ResendHandshakeInterval = 1f,
                MaximumHandshakeAttempts = 15,
                ConnectionTimeout = 1000f,
                ReceiveBufferSize = PlayerPrefs.GetInt("ServerReceiveBuffer", 1024),
                SendBufferSize = PlayerPrefs.GetInt("ServerSendBuffer", 3072)
            };

            server = new NetServer(config);
            server.Start();
            Invoke("ActivateMessage", 0.015625f);
        }
    }

    bool ableToSend = false;

    void ActivateMessage()
    {
        ableToSend = true;
        Invoke("ActivateMessage", 0.015625f);
    }

    public string mesaj = "";
    bool canSendMsg = false;
    void SendMessage()
    {
        canSendMsg = true;
        try
        {
            var message = server.CreateMessage();
            message.Write(mesaj);
            server.SendMessage(message, clientConnections, NetDeliveryMethod.ReliableOrdered, 0);
            mesaj = "";
            ableToSend = false;
        }
        catch
        {
            mesaj = "";
        }
    }
    private void OnApplicationQuit()
    {
        var toAll = server.CreateMessage();
        toAll.Write("Disconnect");
        server.SendMessage(toAll, recipients: clientConnections, NetDeliveryMethod.ReliableOrdered, 0);
    }
    bool detected = false;
    public bool sentPlayers = false;
    // Update is called once per frame
    public void StartGame()
    {
        if (players >= 3)
        {
            bool ok = false;
            int team = playerTeams[0];
            foreach(int playerTeam in playerTeams)
            {
                if (playerTeam != team)
                    ok = true;
            }
            if (ok)
            {

                mesaj += "StartGame 1" + "\n";
                sentPlayers = false;
                SendMessage();
            }
        }
    }

    bool freeze = true;
    public bool bomb = false;


    bool deactivated = false;



    void Win()
    {
        StartGame();
    }

    public bool testing = false;

    string currentScene = "OnlineFight";
    void Update()
    {
        List<int> teams = new List<int>();
        foreach (Player player in FindObjectsOfType<Player>())
        {
            if (player.controllable)
            {
                if (teams.Contains(player.team) == false && player.gameObject != gameObject)
                    teams.Add(player.team);
            }

        }
        if (teams.Count == 1)
        {
            if (SceneManager.GetActiveScene().name == "OnlineFight1")
            {
                mesaj += "StartGame " + 2 + "\n";

            }
            else if (SceneManager.GetActiveScene().name == "OnlineFight2")
            {
                mesaj += "StartGame " + 3 + "\n";
            }
            else if (SceneManager.GetActiveScene().name == "OnlineFight3")
            {

                mesaj += "StartGame " + 4 + "\n";
            }
            else if (SceneManager.GetActiveScene().name == "OnlineFight4")
            {
                mesaj += "StartGame " + "OnlineLobby" + "\n";
            }
            sentPlayers = false;
        }


        if (clientConnections.Count != 0)
            {
                if (ableToSend)
                    mesaj += "ServerAlive" + "\n";
            }

            foreach (Player player in FindObjectsOfType<Player>())
            {
                if (player.Controllable)
                {
                    
                    if (ableToSend && sentPlayers)
                    {
                    string playerAnim = "";
                    string playerAnim2 = "";

                    foreach (string animation in animationList)
                        {
                            if (player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(animation))
                            {
                                playerAnim = animation;
                                break;
                            }
                        }

                    foreach (string animation in animationList2)
                    {
                        if (player.armsAnimator.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(animation))
                        {
                            playerAnim2 = animation;
                            break;
                        }
                    }

                    try
                        {
                            mesaj += "PlayerInfo" + " " + player.PlayerNumber + " " + player.transform.position.x + " " + player.transform.position.y + " " + player.transform.localEulerAngles.z + " " + player.health + " " + player.GetComponent<Rigidbody2D>().velocity.x + " " + player.GetComponent<Rigidbody2D>().velocity.y + " " + playerAnim + " " + playerAnim2 + " " + player.thrownBoomerang.gameObject.activeInHierarchy + " " + player.thrownBoomerang.transform.position.x + " " + player.thrownBoomerang.transform.position.y + " " + player.thrownBoomerang.GetComponent<Rigidbody2D>().velocity.x + " " + player.thrownBoomerang.GetComponent<Rigidbody2D>().velocity.y + "\n";
                        }
                        catch
                        {

                        }
                    }
                }
            }
        
        int playersConnected = 0;
        for(int i=1; i< playerTeams.Count + 1; i++)
        {
            if(clientsConnected[i-1])
            {
                playersConnected++;
                timeOut[i - 1] += Time.deltaTime;
                if (timeOut[i - 1] >= 10)
                    DisconnectClient(clientConnections[i - 1].RemoteEndPoint);
            }
        }
        bool hasRed = false;
        bool hasBlue = false;
        if (SceneManager.GetActiveScene().name.Contains("OnlineFight") && sentPlayers == false)
        {
            for (int i = 1; i < playerTeams.Count + 1; i++)
            {
                try
                {
                    if (clientsConnected[i-1])
                    {
                        //test playeri pentru echipe

                        mesaj += "SpawnPlayer " + i + " " + playerNames[i - 1] + " " + playerTeams[i - 1] + " " + playerClasses[i - 1] + " " + UnityEngine.Random.Range(-16f,16f) + " " +UnityEngine.Random.Range(-9f,9f) + "\n";
                    }
                }
                catch (Exception err)
                {
                    Debug.LogError(err.ToString());
                }
            }
            sentPlayers = true;
        }

        try
        {
            if (SceneManager.GetActiveScene().name == "OnlineWaitMenu")
            {
                if (ableToSend)
                    mesaj += "PlayerCount " + (playersConnected) + "\n";
            }
        }
        catch
        {

        }

        List<NetIncomingMessage> messages = new List<NetIncomingMessage>();
        server.ReadMessages(messages);

        if (messages.Count != 0)
        {
            foreach (NetIncomingMessage message in messages)
            {
                string text = message.ReadString();
                

                string[] lines = text.Split('\n');
                foreach (string line in lines)
                {

                    string data = line;
                    if (data.Contains("Connected"))
                    {
                        clientConnections.Add(message.SenderConnection);
                        var response = server.CreateMessage();
                        response.Write("ReceiveId " + players);

                        server.SendMessage(response, recipient: message.SenderConnection, NetDeliveryMethod.ReliableOrdered);

                        var response2 = server.CreateMessage();

                        response2.Write("LoadScene " + SceneManager.GetActiveScene().name);
                        server.SendMessage(response2, recipient: message.SenderConnection, NetDeliveryMethod.ReliableOrdered);

                        playerNames.Add("");
                        playerTeams.Add(0);
                        playerClasses.Add(0);
                        clientsConnected.Add(true);
                        timeOut.Add(0f);
                        players++;
                    }

                    if (data.Contains("Server"))
                        Destroy(gameObject);

                    if (message.MessageType == NetIncomingMessageType.Data)
                    {
                        if (data == "Disconnect")
                        {
                            DisconnectClient(message);
                        }
                        else
                        {
                            string type = data.Split(' ')[0];
                            if (type == "NewPlayerName")
                            {
                                int id = int.Parse(data.Split(' ')[1]) - 1;
                                try
                                {
                                    playerNames[id] = data.Split(' ')[2];
                                }
                                catch
                                {

                                }
                            }
                            else if (type == "ClientAlive")
                            {
                                int id = int.Parse(data.Split(' ')[1]) - 1;
                                if(id != -1)
                                timeOut[id] = 0;
                            }
                            else if (type == "NewPlayerTeam")
                            {
                                int id = int.Parse(data.Split(' ')[1]) - 1;
                                try
                                {
                                    playerTeams[id] = int.Parse(data.Split(' ')[2]);
                                }
                                catch
                                {

                                }
                            }
                            else if (type == "NewPlayerClass")
                            {
                                int id = int.Parse(data.Split(' ')[1]) - 1;
                                try
                                {
                                    playerClasses[id] = int.Parse(data.Split(' ')[2]);
                                }
                                catch
                                {

                                }
                            }
                            else if (type == "PlayerTarget")
                            {
                                string[] parameters = data.Split(' ');
                                Player player = FindObjectOfType<GameClient>().players[int.Parse(parameters[1])].GetComponent<Player>();
                                player.NewTarget(float.Parse(parameters[2]), float.Parse(parameters[3]));
                            }
                            else if (type == "Shoot")
                            {
                                string[] parameters = data.Split(' ');
                                Player player = FindObjectOfType<GameClient>().players[int.Parse(parameters[1])].GetComponent<Player>();
                                player.ShootBoom(float.Parse(parameters[2]), float.Parse(parameters[3]));
                            }
                            else if(type == "Msg")
                            {
                                mesaj += data + "\n";
                            }
                        }
                    }
                    if (message.MessageType == NetIncomingMessageType.ConnectionApproval)
                    {
                        message.SenderConnection.Approve();
                    }
                    if(message.MessageType == NetIncomingMessageType.StatusChanged)
                    {
                        
                    }
                    try
                    {
                        if (message.SenderConnection.Status == NetConnectionStatus.Disconnected)
                        {
                            DisconnectClient(message);
                        }
                    }
                    catch
                    {

                    }
                    
                }
            }
        }

        if (ableToSend)
            SendMessage();

    }

    public List<bool> clientsConnected;
    public List<float> timeOut;

    private void DisconnectClient(NetIncomingMessage message)
    {
        bool breaking = false;
        for (int i = 0; i < clientConnections.Count; i++)
        {
            if (clientConnections[i].RemoteEndPoint == message.SenderConnection.RemoteEndPoint)
            {
                clientsConnected[i] = false;
                break;
            }
        }
    }
    private void DisconnectClient(IPEndPoint ip)
    {
        bool breaking = false;
        for (int i = 0; i < clientConnections.Count; i++)
        {
            if (clientConnections[i].RemoteEndPoint == ip)
            {
                clientsConnected[i] = false;
                break;
            }
        }
    }

    public int drop;
}
