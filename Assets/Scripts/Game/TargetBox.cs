using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBox : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public EdgeCollider2D edgeCollider2D;
    public EdgeCollider2D startLine;
    public EdgeCollider2D finishLine;
    private BoxCollider2D upperBackground;
    private BoxCollider2D lowerBackground;
    private BoxCollider2D threshold;
    private BoxCollider2D ground1;
    private BoxCollider2D ground2;
    private Vector3 velocity;
    private Vector3 start;
    private float boxDepthZ = 4f;
    public GameObject edgeCollider;
    public bool targetHit = false;
    public bool scoreCounted = false;
    public float spawnStart = 0f;
    private bool exitLowerThreshold = false;
    private bool lerp = false;
    float lerpTime = 0.02f;
    float currentLerpTime;
    float moveDistance = 10f;
    float foxValue = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        velocity = Vector3.zero;

        upperBackground = GameObject.Find("back").GetComponent<BoxCollider2D>();
        lowerBackground = GameObject.Find("back 2").GetComponent<BoxCollider2D>();
        threshold = GameObject.Find("threshold").GetComponent<BoxCollider2D>();
        ground1 = GameObject.Find("foreground").GetComponent<BoxCollider2D>(); 
        ground2 = GameObject.Find("foreground 2").GetComponent<BoxCollider2D>();
        
        Physics2D.IgnoreCollision(startLine, upperBackground);
        Physics2D.IgnoreCollision(startLine, lowerBackground);
        Physics2D.IgnoreCollision(startLine, threshold);
        Physics2D.IgnoreCollision(startLine, ground1);
        Physics2D.IgnoreCollision(startLine, ground2);

        Physics2D.IgnoreCollision(finishLine, upperBackground);
        Physics2D.IgnoreCollision(finishLine, lowerBackground);
        Physics2D.IgnoreCollision(finishLine, threshold);
        Physics2D.IgnoreCollision(finishLine, ground1);
        Physics2D.IgnoreCollision(finishLine, ground2);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
            foxValue = Fox.instance.sensorValue;
            velocity = new Vector3(0, foxValue / 7840f, 4);
            GameControl.instance.PlaySignalAudio(GameControl.instance.TargetHitSound);
            start = rb2d.position;
            lerp = true;
            Fox.instance.hasCollided = true;
            targetHit = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player" && foxValue > 2452f && !scoreCounted)
        {
            scoreCounted = true;
            GameControl.instance.PlayScoreAudio(GameControl.instance.OvershotSound);
            GameControl.instance.overshotCount++;
            GameControl.instance.totalTargets++;
            DataEntry entry = new DataEntry(
                        GameControl.instance.stopwatch.ElapsedMilliseconds,
                        GameControl.instance.currTarget,
                        GameControl.instance.sensorValue,
                        LogType.OVERSHOT
                    );
            GameControl.instance.logger.addEntry(entry);
        }
        else if (collision.gameObject.tag != "Player" && foxValue >= 1962f && foxValue <= 2452f && !scoreCounted)
        {
            GameControl.instance.PlayScoreAudio(GameControl.instance.ScoreSound);
            scoreCounted = true;
            GameControl.instance.score++;
            GameControl.instance.totalTargets++;
            DataEntry entry = new DataEntry(
            GameControl.instance.stopwatch.ElapsedMilliseconds,
            GameControl.instance.currTarget,
            GameControl.instance.sensorValue,
            LogType.SCORE
        );
            GameControl.instance.logger.addEntry(entry);
        }
          else if (collision.gameObject.tag != "Player" && foxValue < 1962f && !scoreCounted)
        {
            GameControl.instance.PlayScoreAudio(GameControl.instance.UndershotSound);
            scoreCounted = true;
            GameControl.instance.undershotCount++;
            GameControl.instance.totalTargets++;
            DataEntry entry = new DataEntry(
            GameControl.instance.stopwatch.ElapsedMilliseconds,
            GameControl.instance.currTarget,
            GameControl.instance.sensorValue,
            LogType.UNDERSHOT
        );
            GameControl.instance.logger.addEntry(entry);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(lerp)
        {
            currentLerpTime += Time.fixedDeltaTime;
            if (currentLerpTime > lerpTime)
            {
                currentLerpTime = lerpTime;
                lerp = false;
            }

            //lerp!
            float t = currentLerpTime / lerpTime;
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            for (int i = 0; i < 100; i++)
            {
                transform.position = Vector3.Lerp(start, start + velocity, t * i);
            }
        }
        
    }
}
