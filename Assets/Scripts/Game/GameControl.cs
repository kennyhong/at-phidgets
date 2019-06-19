using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public static GameControl instance;
    //public Logger logger;
    //public GameObject trialCompletedText;
    public int participantId;
    public int trialNumber = 1;
    public int totalTargets = 0;
    public bool trialOver = true;
    public float scrollSpeed = -1.5f;
    public int overshotCount = 0;
    public int undershotCount = 0;
    public GameMode gamemode = GameMode.Visual;
    public int score = 0;
    public int missedTargets = 0;
    public int missedJumps = 0;
    public bool menuOn = true;
    public System.Diagnostics.Stopwatch stopwatch;
    public float sensorValue = 0f;
    private float trialCooldownTime = 300f;
    public bool trialCooldown = true;

    // Score Board
    public TextMeshPro OvershotText;
    public TextMeshPro UndershotText;
    public TextMeshPro ScoreText;
    public TextMeshPro MissedJumpsText;
    public TextMeshPro MissedTargetsText;

    //Game Panel
    public Button startTrial;
    public Button debugMode;
    public GameObject GamePanel;
    public TextMeshPro TrialMode;
    public TextMeshPro CurrentTrial;
    public TextMeshPro ServerIP;
    public TextMeshPro StartCooldownTimer;


    //Server
    public string ip;
    public int port;
    private Client client;

    // Start is called before the first frame update
    void Awake()
    {
        trialOver = true;
        Debug.Log($"UDP server address: {ip}");
        Debug.Log($"UDP server port: {port}");

        //// Create a new TCP chat client
        client = new Client(ip, port);


        //// Connect the client
        Debug.Log("Client connecting...");
        client.Connect();
        Debug.Log("Done!");
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }

        //logger = new Logger();
    }

    void Start()                                
    {
        trialOver = true;

        TrialMode.text = "Mode: " + gamemode.ToString();
        CurrentTrial.text = "Current Trial: " + trialNumber + " / 5";
    }

    void StartTrial()
    {
        switch(gamemode)
        {
            case GameMode.Visual:
                startPhidgetTrial();
                menuOn = false;
                break;
            case GameMode.Audio:
                menuOn = false;
                break;
            case GameMode.VisualAudio:
                menuOn = false;
                break;
            case GameMode.VATransitive:
                menuOn = false;
                break;
        }
    }

    void StartDebug()
    {
        gamemode = GameMode.Debug;
        menuOn = false;
        GamePanel.SetActive(false);
        trialOver = false;
    }

    void startPhidgetTrial()
    {
        GamePanel.SetActive(false);
        trialOver = false;
        stopwatch = new System.Diagnostics.Stopwatch();
    }

    // Update is called once per frame
    void Update()
    {
        UndershotText.text = "Undershot: " + undershotCount;
        ScoreText.text = "Score: " + score;
        OvershotText.text = "Overshot: " + overshotCount;
        MissedJumpsText.text = "Missed Jumps: " + missedJumps;
        MissedTargetsText.text = "Missed Targets: " + missedTargets;
        client.Send("ping");
        if(trialCooldown && trialCooldownTime > 0f)
        {
            trialCooldownTime--;
            StartCooldownTimer.text = "Cooldown Timer: " + trialCooldownTime;
        } else if (trialCooldownTime == 0f && trialCooldown)
        {
            trialCooldown = false;
            trialCooldownTime = 300f;
            StartCooldownTimer.text = "Cooldown Timer: Ready";
        }
        
        if (trialOver == true && menuOn == false)
        {
            SetNextTrial();
        }
        if(gamemode == GameMode.Debug && Input.GetKeyDown(KeyCode.Escape))
        {
            ReloadScene();
        }
        if (sensorValue * 1000f > 900f && trialOver && !trialCooldown)
        {
            StartTrial();
        }
    }

    private void ReloadScene()
    {
        trialOver = true;
        menuOn = true;
        undershotCount = 0;
        score = 0;
        overshotCount = 0;
        missedJumps = 0;
        missedTargets = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void SetNextTrial()
    {
        menuOn = true;
        undershotCount = 0;
        score = 0;
        overshotCount = 0;
        missedJumps = 0;
        missedTargets = 0;

        trialNumber++;

        // TODO: set game mode to next;

        TrialMode.text = "Mode: " + gamemode.ToString();
        CurrentTrial.text = "Current Trial: " + trialNumber + " / 5";
        GamePanel.SetActive(true);
        
    }

    private void DisconnectClient()
    {
        Debug.Log("Client disconnecting...");
        client.Disconnect();
        Debug.Log("Done!");
    }

    private void StopClient()
    {
        Debug.Log("Client disconnecting...");
        client.DisconnectAndStop();
        Debug.Log("Done!");
    }


    private void OnApplicationQuit()
    {
        StopClient();
    }

    public void trialComplete()
    {
        //trialCompletedText.SetActive(true);
        trialOver = true;
    }
}
