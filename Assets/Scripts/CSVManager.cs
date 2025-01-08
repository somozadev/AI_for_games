using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVManager
{
    private string filePath;

    public CSVManager(string fileName)
    {
        filePath = Path.Combine(Application.persistentDataPath, fileName);
        Debug.Log(filePath);
    }

    public void AppendData(string[] data)
    {
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine(string.Join(",", data));
        }
    }

    public List<string[]> ReadData()
    {
        List<string[]> data = new List<string[]>();
        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    data.Add(values);
                }
            }
        }

        return data;
    }
}