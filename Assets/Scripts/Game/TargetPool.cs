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
    private float timeSinceLastSpawned;
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
}

    // Update is called once per frame
    void Update()
    {
        timeSinceLastSpawned += Time.deltaTime;
        if (GameControl.instance.totalTargets == 20 && Fox.instance.onGround && GameControl.instance.gamemode != GameMode.Debug)
        {
            GameControl.instance.trialOver = true;
            GameControl.instance.trialCooldown = true;
            GameControl.instance.totalTargets = 0;
            ResetBoxes();
            firstBox = true;
            
        }

        if (GameControl.instance.trialOver == false && timeSinceLastSpawned >= spawnRate)
        {
            
            timeSinceLastSpawned = 0f;
            if (currentBox > 0 && firstBox==false)
            {
                if (!targetBoxes[currentBox - 1].GetComponent<TargetBox>().targetHit)
                {
                    GameControl.instance.missedTargets++;
                    GameControl.instance.totalTargets++;
                }  else
                {
                    targetBoxes[currentBox - 1].GetComponent<TargetBox>().targetHit = false;
                    targetBoxes[currentBox - 1].GetComponent<TargetBox>().scoreCounted = false;
                }
            } else if (currentBox == 0 && firstBox == false)
            {
                if (!targetBoxes[boxPoolSize - 1].GetComponent<TargetBox>().targetHit)
                {
                    GameControl.instance.missedTargets++;
                    GameControl.instance.totalTargets++;
                } else
                {
                    targetBoxes[boxPoolSize - 1].GetComponent<TargetBox>().targetHit = false;
                    targetBoxes[boxPoolSize - 1].GetComponent<TargetBox>().scoreCounted = false;
                }
            }
            
            targetBoxes[currentBox].transform.position = new Vector3(spawnXposition, spawnYposition, spawnZposition);
            currentBox++;
            if(currentBox >= boxPoolSize)
            {
                currentBox = 0;
            }
            

            firstBox = false;
        }
    }
}
