using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataEntry
{
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
}
