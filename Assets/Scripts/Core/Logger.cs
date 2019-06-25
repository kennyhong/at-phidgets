using System.Collections;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using UnityEngine;

public class Logger
{
    private List<DataEntry> records;
    private StreamWriter writer;

    public Logger()
    {
        records = new List<DataEntry>();
        var logPath = Application.persistentDataPath + "/logs/";
        if (!Directory.Exists(logPath))
        {
            Directory.CreateDirectory(logPath);
        }
        writer = new StreamWriter(Application.persistentDataPath + "/logs/" + "participant" + GameControl.instance.participantId + ".csv", append: true);
    }

    public void addEntry(DataEntry entry)
    {
        records.Add(entry);
    }

    public void writeToCSV()
    {
        using (var csv = new CsvWriter(writer))
        {
            //csv.Configuration.RegisterClassMap<DataEntryMap>();
            csv.WriteRecords(records);
        }
    }
}
