
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public static GameControl instance;
    public Logger logger;
    //public GameObject trialCompletedText;
    public const float SIGNAL_TIME = 3f;
    public int participantId;
    public int trialNumber;
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
    private float trialCooldownTime = 60f;
    public bool trialCooldown = true;
    private IEnumerator signalCoroutine;
    public float playDelayCount = 0f;
    private float playDelay = 3f;
    public bool delayStart = false;
    public float delayStartTimer = 0f;
    private float delayStartTotal = 0.35f;
    private bool studyComplete = false;
    public GameObject VisualBlock;

    public int currTarget = 0;
    public float currTargetBegin = -1f;
    public float currTargetEnd = -1f;

    // Score Board
    public TextMeshPro OvershotText;
    public TextMeshPro UndershotText;
    public TextMeshPro ScoreText;
    public TextMeshPro MissedJumpsText;
    public TextMeshPro MissedTargetsText;

    public TextMeshPro BlockOvershotText;
    public TextMeshPro BlockUndershotText;
    public TextMeshPro BlockScoreText;
    public TextMeshPro BlockMissedJumpsText;
    public TextMeshPro BlockMissedTargetsText;

    //Game Panel
    public GameObject GamePanel;
    public TextMeshPro TrialMode;
    public TextMeshPro CurrentTrial;
    public TextMeshPro ServerIP;
    public TextMeshPro StartCooldownTimer;
    public TextMeshPro Instructions;
    public TextMeshPro NumberTargets;
    public TextMeshPro StudyCompletedText;

    public TextMeshPro SummaryTitle;
    public TextMeshPro ScoreSummary;
    public TextMeshPro OvershotSummary;
    public TextMeshPro UndershotSummary;
    public TextMeshPro MissedTargetsSummary;
    public TextMeshPro MissedJumpsSummary;

    //Audio Clip
    public AudioSource AudioScoreSource;
    public AudioSource AudioSignalSource;
    public AudioSource AudioFoxSource;
    public AudioClip TrialReadySound;
    public AudioClip TrialCompletedSound;
    public AudioClip ScoreSound;
    public AudioClip TargetHitSound;
    public AudioClip JumpSound;
    public AudioClip JumpSignalSound;
    public AudioClip MissedTargetSound;
    public AudioClip OvershotSound;
    public AudioClip UndershotSound;
    public AudioClip MissedJumpSound;


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

        stopwatch = new System.Diagnostics.Stopwatch();
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
        signalCoroutine = SignalJump();
        logger = new Logger();
    }

    void Start()                                
    {
        trialOver = true;
        trialNumber = 1;
        TrialMode.text = "Mode: " + gamemode.ToString();
        CurrentTrial.text = "Current Trial: " + trialNumber + " / 20";
    }

    void StartTrial()
    {
        sensorValue = 0f;
        Fox.instance.sensorValue = 0f;
        switch (gamemode)
        {
            case GameMode.Visual:
                GamePanel.SetActive(false);
                AudioScoreSource.mute = true;
                AudioFoxSource.mute = true;
                AudioSignalSource.mute = true;
                trialOver = false;
                stopwatch.Start();
                StartCoroutine(signalCoroutine);
                menuOn = false;
                break;
            case GameMode.Audio:
                GamePanel.SetActive(false);
                VisualBlock.SetActive(true);
                trialOver = false;
                stopwatch.Start();
                StartCoroutine(signalCoroutine);
                menuOn = false;
                break;
            case GameMode.VisualAudio:
                GamePanel.SetActive(false);
                trialOver = false;
                stopwatch.Start();
                StartCoroutine(signalCoroutine);
                menuOn = false;
                break;
            case GameMode.VATransitive:
                GamePanel.SetActive(false);
                if(trialNumber > 10)
                {
                    VisualBlock.SetActive(true);
                }
                trialOver = false;
                stopwatch.Start();
                StartCoroutine(signalCoroutine);
                menuOn = false;
                break;
        }
        Fox.instance.firstJump = false;
        Fox.instance.jumpCooldownStatus = true;
    }

    // Update is called once per frame
    void Update()
    {
        UndershotText.text = "Undershot: " + undershotCount;
        ScoreText.text = "Score: " + score;
        OvershotText.text = "Overshot: " + overshotCount;
        MissedJumpsText.text = "Missed Jumps: " + missedJumps;
        MissedTargetsText.text = "Missed Targets: " + missedTargets;

        BlockUndershotText.text = "Undershot: " + undershotCount;
        BlockScoreText.text = "Score: " + score;
        BlockOvershotText.text = "Overshot: " + overshotCount;
        BlockMissedJumpsText.text = "Missed Jumps: " + missedJumps;
        BlockMissedTargetsText.text = "Missed Targets: " + missedTargets;

        client.Send("ping");
        if(trialCooldown && trialCooldownTime > 0f)
        {
            trialCooldownTime--;
            StartCooldownTimer.text = "Cooldown Timer: " + trialCooldownTime;
        } else if (trialCooldownTime == 0f && trialCooldown)
        {
            trialCooldown = false;
            trialCooldownTime = 60f;
            StartCooldownTimer.text = "Cooldown Timer: Ready";
            PlaySignalAudio(TrialReadySound);
        }
        
        if (trialOver == true && menuOn == false)
        {
            if(trialNumber < 20)
            {
                logger.writeToCSV();
                SetNextTrial();
            } else
            {
                StudyComplete();
            }
        }
        if (sensorValue * 1000f > 900f && trialOver && !trialCooldown && !studyComplete)
        {
            StartTrial();
        }
        if(!trialOver && delayStart)
        {
            if(playDelayCount >= playDelay)
            {
                PlaySignalAudio(JumpSignalSound);
                playDelayCount = 0f;
            }
            playDelayCount += Time.deltaTime;
        }
        if(!delayStart && delayStartTimer >= delayStartTotal && !trialOver)
        {
            delayStart = true;
        }
        if(!delayStart && !trialOver)
        {
            delayStartTimer += Time.deltaTime;
        }
    }

    public void PlayScoreAudio(AudioClip clip)
    {
        AudioScoreSource.clip = clip;
        AudioScoreSource.Play();
    }

    public void PlaySignalAudio(AudioClip clip)
    {
        AudioSignalSource.clip = clip;
        AudioSignalSource.Play();
    }

    public void PlayFoxAudio(AudioClip clip)
    {
        AudioFoxSource.clip = clip;
        AudioFoxSource.Play();
    }

    private void SetNextTrial()
    {
        menuOn = true;

        UndershotSummary.text = "Undershot: " + undershotCount;
        ScoreSummary.text = "Score: " + score;
        OvershotSummary.text = "Overshot: " + overshotCount;
        MissedJumpsSummary.text = "Missed Jumps: " + missedJumps;
        MissedTargetsSummary.text = "Missed Targets: " + missedTargets;

        undershotCount = 0;
        score = 0;
        overshotCount = 0;
        missedJumps = 0;
        missedTargets = 0;

        trialNumber++;

        TrialMode.text = "Mode: " + gamemode.ToString();
        CurrentTrial.text = "Current Trial: " + trialNumber + " / 20";
        Fox.instance.firstJump = true;
        GamePanel.SetActive(true);
        if(gamemode == GameMode.Audio || gamemode == GameMode.VATransitive)
        {
            VisualBlock.SetActive(false);
        }
        
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

    public void StudyComplete()
    {
        if(!studyComplete)
        {
            trialOver = true;
            StopClient();
            VisualBlock.SetActive(false);
            TrialMode.gameObject.SetActive(false);
            ServerIP.gameObject.SetActive(false);
            CurrentTrial.gameObject.SetActive(false);
            StartCooldownTimer.gameObject.SetActive(false);
            Instructions.gameObject.SetActive(false);
            NumberTargets.gameObject.SetActive(false);
            StudyCompletedText.gameObject.SetActive(true);
            GamePanel.SetActive(true);
            logger.writeToCSV();
            studyComplete = true;
        }
    }

    private IEnumerator SignalJump()
    {
        while (true)
        {
            yield return new WaitForSeconds(SIGNAL_TIME);
                
        }
    }
}
