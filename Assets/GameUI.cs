using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CameraAngle
{
    menu = 0,
    playerOne = 1,
    playerTwo = 2,
}

public class GameUI : MonoBehaviour
{
    private static GameUI _instance;
    public static GameUI Instance { get { return _instance; } }

    public Server server;
    public Client client;

    [SerializeField] private TMP_InputField addressInput;
    [SerializeField] private GameObject[] cameraAngles;

    [Header("Canvas Menus")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject onlineSetupMenu;
    [SerializeField] private GameObject waitingConnectionMenu;

    [Header("Gameplay UI")]
    [SerializeField] private GameObject unitInfo;
    [SerializeField] private Image unitSprite;
    [SerializeField] private TMP_Text unitName;
    [SerializeField] private TMP_Text unitHealth;




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

        RegisterEvents();
    }

    // Camera Functions
    public void ChangeCamera(CameraAngle index)
    {
        for (int i = 0; i < cameraAngles.Length; i++)
            cameraAngles[i].SetActive(false);

        cameraAngles[(int)index].SetActive(true);
    }

    public void ShowUnitInfo(CharacterInfo character)
    {
        unitSprite.sprite = character.characterClass.unitSprite;
        unitName.text = character.characterClass.name;
        unitHealth.text = character.characterClass.healthPoints + " / " + Mathf.Abs(character.currentHealth);
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

    private void RegisterEvents()
    {
        NetUtility.C_START_GAME += OnStartGameClient;
    }
    private void UnregisterEvents()
    {
        NetUtility.C_START_GAME -= OnStartGameClient;
    }

    private void OnStartGameClient(NetMessage obj)
    {
        mainMenu.SetActive(false);
        onlineSetupMenu.SetActive(false);
        waitingConnectionMenu.SetActive(false);
    }
}
