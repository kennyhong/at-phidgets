using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using UnityEngine;

public class Logger
{
    private List<DataEntry> records;

    public Logger()
    {
        records = new List<DataEntry>();
        var logPath = Application.persistentDataPath + "/logs/";
        if (!Directory.Exists(logPath))
        {
            Directory.CreateDirectory(logPath);
        }
        using (var writer = new StreamWriter(Application.persistentDataPath + "/logs/" + "participant" + GameControl.instance.participantId + ".csv", append: true))
        {
            writer.WriteLine("participantId,trialNumber,timeElapse,currTarget,currScore,missedJumps,missedTargets,overshots,undershots,sensorValue,logType");
        }
    }

    public void addEntry(DataEntry entry)
    {
        records.Add(entry);
    }

    public void writeToCSV()
    {
        using (var writer = new StreamWriter(Application.persistentDataPath + "/logs/" + "participant" + GameControl.instance.participantId + ".csv", append: true))
        {
            records.ForEach(async record =>
            {
                await writer.WriteLineAsync(record.participantId + "," + record.trialNumber + "," + record.timeElapse + "," + 
                    record.currTarget + "," + record.currScore + "," + record.missedJumps + "," + record.missedTargets + "," + record.overshots + "," + 
                    record.undershots + "," + record.sensorValue + "," + record.logtype);
               
            });
        }
        records.Clear();
    }
}
