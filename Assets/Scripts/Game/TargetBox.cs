using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBox : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private EdgeCollider2D edgeCollider2D;
    private Vector3 velocity;
    private Vector3 start;
    private float boxDepthZ = 4f;
    public GameObject edgeCollider;
    public bool targetHit = false;
    public bool scoreCounted = false;
    private bool exitLowerThreshold = false;
    private bool lerp = false;
    float lerpTime = 0.02f;
    float currentLerpTime;

    float moveDistance = 10f;

    // Start is called before the first frame update
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        velocity = Vector3.zero;
        edgeCollider2D = edgeCollider.GetComponent<EdgeCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Threshold" && collision.gameObject.tag != "UpperBackground" && collision.gameObject.tag != "LowerBackground")
        {
            velocity = new Vector3(0, Fox.instance.sensorValue/7840f, 4);
            GameControl.instance.PlaySignalAudio(GameControl.instance.TargetHitSound);
            start = rb2d.position;
            lerp = true;
            Fox.instance.hasCollided = true;
            targetHit = true;
        }
        else if (collision.gameObject.tag == "UpperBackground" && !scoreCounted)
        {
            scoreCounted = true;
            GameControl.instance.PlayScoreAudio(GameControl.instance.OvershotSound);
            GameControl.instance.overshotCount++;
            GameControl.instance.totalTargets++;
        }
        else if (collision.gameObject.tag == "Threshold" && !scoreCounted)
        {
            GameControl.instance.PlayScoreAudio(GameControl.instance.ScoreSound);
            scoreCounted = true;
            GameControl.instance.score++;
            GameControl.instance.totalTargets++;
        }
          else if (collision.gameObject.tag == "LowerBackground" && !scoreCounted)
        {
            GameControl.instance.PlayScoreAudio(GameControl.instance.UndershotSound);
            scoreCounted = true;
            GameControl.instance.undershotCount++;
            GameControl.instance.totalTargets++;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(lerp)
        {
            Debug.Log(Time.deltaTime);
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
