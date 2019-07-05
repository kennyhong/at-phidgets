using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPool : MonoBehaviour
{
    private GameObject[] targetBoxes;
    public int boxPoolSize = 5;
    public GameObject boxPrefab;
    public float spawnRate = 3f;

    private Vector3 objectPoolPosition = new Vector3(-15f, -1000f, 40f);
    private float timeSinceLastSpawned = 0;
    private float spawnXposition = 1f;
    private float spawnYposition = -0.1f;
    private float spawnZposition = 4f;
    private int currentBox = 0;
    private bool firstBox = true;
    // Start is called before the first frame update
    void Start()
    {
        targetBoxes = new GameObject[boxPoolSize];
        for (int i = 0; i < boxPoolSize; i++)
        {
            targetBoxes[i] = (GameObject)Instantiate(boxPrefab, objectPoolPosition, Quaternion.identity);
        }
    }

    public void ResetBoxes()
    {
        for (int i = 0; i < boxPoolSize; i++)
        {
            Destroy(targetBoxes[i]);
        }
        for (int i = 0; i < boxPoolSize; i++)
        {
            targetBoxes[i] = (GameObject)Instantiate(boxPrefab, objectPoolPosition, Quaternion.identity);
        }
        currentBox = 0;
        timeSinceLastSpawned = 0f;
        firstBox = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameControl.instance.trialOver == false)
        {
            timeSinceLastSpawned += Time.deltaTime;
        }

        if (GameControl.instance.totalTargets == 20 && Fox.instance.onGround)
        {
            GameControl.instance.trialOver = true;
            GameControl.instance.trialCooldown = true;
            GameControl.instance.totalTargets = 0;
            GameControl.instance.StopAllCoroutines();
            GameControl.instance.playDelayCount = 0;
            GameControl.instance.delayStartTimer = 0;
            GameControl.instance.delayStart = false;
            GameControl.instance.PlaySignalAudio(GameControl.instance.TrialCompletedSound);
            GameControl.instance.stopwatch.Stop();
            GameControl.instance.stopwatch.Reset();
            ResetBoxes();
        }

        if (GameControl.instance.trialOver == false && timeSinceLastSpawned >= spawnRate)
        {

            timeSinceLastSpawned = 0f;
            if (currentBox > 0 && firstBox == false)
            {
                if (!targetBoxes[currentBox - 1].GetComponent<TargetBox>().targetHit)
                {
                    GameControl.instance.PlayScoreAudio(GameControl.instance.MissedTargetSound);
                    GameControl.instance.currTargetEnd = GameControl.instance.stopwatch.ElapsedMilliseconds;
                    GameControl.instance.missedTargets++;
                    GameControl.instance.totalTargets++;
                    GameControl.instance.currTargetBegin = -1f;
                    GameControl.instance.currTargetEnd = -1f;
                }
                else
                {
                    GameControl.instance.currTargetEnd = GameControl.instance.stopwatch.ElapsedMilliseconds;
                    targetBoxes[currentBox - 1].GetComponent<TargetBox>().targetHit = false;
                    targetBoxes[currentBox - 1].GetComponent<TargetBox>().scoreCounted = false;
                    GameControl.instance.currTargetBegin = -1f;
                    GameControl.instance.currTargetEnd = -1f;
                }
            }
            else if (currentBox == 0 && firstBox == false)
            {
                if (!targetBoxes[boxPoolSize - 1].GetComponent<TargetBox>().targetHit)
                {
                    GameControl.instance.PlayScoreAudio(GameControl.instance.MissedTargetSound);
                    GameControl.instance.currTargetEnd = GameControl.instance.stopwatch.ElapsedMilliseconds;
                    GameControl.instance.missedTargets++;
                    GameControl.instance.totalTargets++;
                    GameControl.instance.currTargetBegin = -1f;
                    GameControl.instance.currTargetEnd = -1f;
                }
                else
                {
                    GameControl.instance.currTargetEnd = GameControl.instance.stopwatch.ElapsedMilliseconds;
                    targetBoxes[boxPoolSize - 1].GetComponent<TargetBox>().targetHit = false;
                    targetBoxes[boxPoolSize - 1].GetComponent<TargetBox>().scoreCounted = false;
                    GameControl.instance.currTargetBegin = -1f;
                    GameControl.instance.currTargetEnd = -1f;
                }
            }
            targetBoxes[currentBox].transform.position = new Vector3(spawnXposition, spawnYposition, spawnZposition);
            GameControl.instance.currTargetBegin = GameControl.instance.stopwatch.ElapsedMilliseconds;
            currentBox++;
            GameControl.instance.currTarget++;
            if (currentBox >= boxPoolSize)
            {
                currentBox = 0;
            }

            firstBox = false;
        }
    }
}
