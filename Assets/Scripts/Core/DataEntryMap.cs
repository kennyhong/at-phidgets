using CsvHelper.Configuration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataEntryMap : ClassMap<DataEntry>
{ 
    public DataEntryMap()
    {
        Map(m => m.participantId).Index(0).Name("participantId");
        Map(m => m.trialNumber).Index(1).Name("trialNumber");
        Map(m => m.timeElapse).Index(2).Name("timeElapse");
        Map(m => m.currTarget).Index(3).Name("currTarget");
        Map(m => m.currScore).Index(4).Name("currScore");
        Map(m => m.missedJumps).Index(5).Name("missedJumps");
        Map(m => m.missedTargets).Index(6).Name("missedTargets");
        Map(m => m.overshots).Index(7).Name("overshots");
        Map(m => m.undershots).Index(8).Name("undershots");
        Map(m => m.sensorValue).Index(9).Name("sensorValue");
    }
}
