using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataEntry
{
    public DataEntry(float time, int curr, float sensorVal, LogType type)
    {
        participantId = GameControl.instance.participantId;
        trialNumber = GameControl.instance.trialNumber;
        timeElapse = time;
        currTarget = curr;
        currScore = GameControl.instance.score;
        missedJumps = GameControl.instance.missedJumps;
        missedTargets = GameControl.instance.missedTargets;
        overshots = GameControl.instance.overshotCount;
        undershots = GameControl.instance.undershotCount;
        sensorValue = sensorVal * 1000f;
        logtype = type;
    }

    public int participantId { get; set; }
    public int trialNumber { get; set; }
    public float timeElapse { get; set; }
    public int currTarget { get; set; }
    public int currScore { get; set; }
    public int missedJumps { get; set; }
    public int missedTargets { get; set; }
    public int overshots { get; set; }
    public int undershots { get; set; }
    public float sensorValue { get; set; }
    public LogType logtype { get; set; }
}
