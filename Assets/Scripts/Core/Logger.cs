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
            writer.WriteLine("participantId,condition,trialNumber,timeElapse,currTarget,currTargetTimeBegin,currTargetTimeEnd,currScore,missedJumps,missedTargets,overshots,undershots,sensorValue,logType");
            writer.Close();
        }
    }

    public void addEntry(DataEntry entry)
    {
        records.Add(entry);
    }

    public async void writeToCSV()
    {
        using (var writer = new StreamWriter(Application.persistentDataPath + "/logs/" + "participant" + GameControl.instance.participantId + ".csv", append: true))
        {
            for(int i = 0; i < records.Count; i++)
            {
                await writer.WriteLineAsync(records[i].participantId + "," + records[i].mode + "," + records[i].trialNumber + "," + records[i].timeElapse + "," +
                    records[i].currTarget + "," + records[i].currTargetBegin + "," + records[i].currTargetEnd + "," + records[i].currScore + "," +
                    records[i].missedJumps + "," + records[i].missedTargets + "," +
                    records[i].overshots + "," + records[i].undershots + "," + records[i].sensorValue + "," + records[i].logtype);
            }
            writer.Close();
        }
        records.Clear();
    }
}
