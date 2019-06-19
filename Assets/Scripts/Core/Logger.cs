using System.Collections;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using UnityEngine;

public class Logger
{
    private List<DataEntry> records;
    private StreamWriter writer;
    private CsvWriter csv;


    public Logger()
    {
        records = new List<DataEntry>();
        var logPath = Application.persistentDataPath + "/logs/";

        if (!Directory.Exists(logPath))
        {
            Directory.CreateDirectory(logPath);
        }
        writer = new StreamWriter(logPath + "participant.csv");
        csv = new CsvWriter(writer);
        csv.Configuration.RegisterClassMap<DataEntryMap>();
    }

    public void addEntry(DataEntry entry)
    {
        records.Add(entry);
    }

    public void writeToCSV()
    {
        csv.WriteRecords(records);
    }
}
