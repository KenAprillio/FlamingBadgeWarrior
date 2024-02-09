using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    private static GameUI _instance;
    public static GameUI Instance { get { return _instance; } }

    public Server server;
    public Client client;

    [SerializeField] private TMP_InputField addressInput;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void OnOnlineHostButton()
    {
        server.Init(8007);
        client.Init("127.0.0.1", 8007);
    }

    public void OnOnlineConnectButton()
    {
        client.Init(addressInput.text, 8007);
    }

    public void OnHostBackButton()
    {
        server.Shutdown();
        client.Shutdown();
    }
}
