using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
public class FileExporter
{
    public delegate void FileSavedEvent(string path);
    public static FileSavedEvent OnFileSaved;
    public static void WriteLogFile(string content, string fileName)
    {

        string path = Application.persistentDataPath + "/" + fileName + "-" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".txt";
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine(content);
            writer.Close();

            OnFileSaved?.Invoke(path);
            Debug.Log("File saved at " + path);
        }
    }


}
