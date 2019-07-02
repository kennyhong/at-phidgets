using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : MonoBehaviour
{
    private const float COOLDOWN_TIME = 45f;
    private float cooldownTimer = 0f;
    public static Fox instance;
    private Rigidbody2D rb2d;
    public float upforce = 200f;
    private Animator anim;
    public bool onGround = true;
    public bool hasCollided = false;
    public float sensorValue = 0f;
    private BoxCollider2D collider2d;
    private int collisionTest = 0;
    private bool gameStart = true;
    public bool firstJump = true;
    public bool jumpCooldownStatus = true;
    private bool repeatedBackground = true;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        collider2d = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (gameStart)
        {
            gameStart = false;
        } else if (collision.gameObject.tag == "Ground")
        {
            onGround = true;
            if (!hasCollided && !gameStart && !repeatedBackground)
            {
                GameControl.instance.PlaySignalAudio(GameControl.instance.MissedJumpSound);
                GameControl.instance.missedJumps++;
                repeatedBackground = true;
                DataEntry entry = new DataEntry(
                        GameControl.instance.stopwatch.ElapsedMilliseconds,
                        GameControl.instance.currTarget,
                        GameControl.instance.sensorValue,
                        LogType.MISSED_JUMP

                    );
                GameControl.instance.logger.addEntry(entry);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(jumpCooldownStatus && !GameControl.instance.trialOver)
        {
            cooldownTimer++;
            if(cooldownTimer == COOLDOWN_TIME)
            {
                jumpCooldownStatus = false;
                cooldownTimer = 0f;
            }
        }
        if (GameControl.instance.trialOver == true)
        {
            rb2d.velocity = Vector2.zero;
            anim.SetTrigger("Idle");
            firstJump = true;
        } else
        {
            anim.SetTrigger("Run");
        }
        if (onGround == true && !GameControl.instance.trialOver)
        {
            if ((GameControl.instance.sensorValue * 1000f) > 500f && !jumpCooldownStatus && !firstJump)
            {
                sensorValue = ((float)GameControl.instance.sensorValue * 1000f);
                DataEntry entry = new DataEntry(
                       GameControl.instance.stopwatch.ElapsedMilliseconds,
                       GameControl.instance.currTarget,
                       GameControl.instance.sensorValue,
                       LogType.JUMP
                   );
                GameControl.instance.logger.addEntry(entry);
                if (sensorValue > 4200f)
                {
                    sensorValue = 4200f;
                }
                rb2d.velocity = Vector2.zero;
                rb2d.AddForce(new Vector2(0, upforce));
                anim.SetTrigger("Jump");
                onGround = false;
                repeatedBackground = false;
                hasCollided = false;
                jumpCooldownStatus = true;
                GameControl.instance.sensorValue = 0f;
            }
        } 
    }
}
