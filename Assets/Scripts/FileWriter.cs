using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileWriter : MonoBehaviour
{
    [SerializeField] private float updateRate = 0.1f;
    private float timer = 0;
    private string filename = "";
    private TextWriter tw;
    public bool recording;

    void Start() {
        filename = Application.dataPath + "/Log.csv";
        WriteHeadOfCSV();
        recording = true;
        tw = new StreamWriter(filename, true);
        InvokeRepeating("PrintData", 0, updateRate);
        //CreateFile();
    }

    void Update() {

    }

    private void CreateFile() {
        // File path
        string path = Application.dataPath + "/Log.csv";

        //Create file if it doesn't exist
        if (!File.Exists(path)) {
            File.WriteAllText(path, "Test");
        }
    }

    private void WriteHeadOfCSV() {
        tw = new StreamWriter(filename, false);

        // Here are the headings of the csv file:
        tw.WriteLine("timestamp, gripstrength");
        tw.Close();
    }
    private void PrintData() {
        if (recording) {
            // Why can't I write to fucking milliseconds
            tw.WriteLine((System.DateTime.Now) + "," + 123);
        }
    }
}
