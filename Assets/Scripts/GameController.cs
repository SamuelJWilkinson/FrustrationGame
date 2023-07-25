using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

public class GameController : MonoBehaviour
{
    public class continuousDataPoint {
        public float timestamp;
        public float rightGrabValue;
        public float leftGrabValue;

        public continuousDataPoint(float timestamp, float rightGrabValue, float leftGrabValue) {
            this.timestamp = timestamp;
            this.rightGrabValue = rightGrabValue;
            this.leftGrabValue = leftGrabValue;
        }
    }

    public class actionDataPoint {
        public float startTime;
        public float endTime;
        public float gripDownLength;
        public float gripUpLength;
        public float controllerActionDisplacement;

        public actionDataPoint(float startTime, float endTime, float gripDownLength, float gripUpLength, float controllerActionDisplacement) {
            this.startTime = startTime;
            this.endTime = endTime;
            this.gripDownLength = gripDownLength;
            this.gripUpLength = gripUpLength;
            this.controllerActionDisplacement = controllerActionDisplacement;
        }
    }

    //Inputs:
    public InputActionProperty rightGripAction;
    private float rightGripValue;
    public InputActionProperty leftGripAction;
    private float leftGripValue;

    private List<continuousDataPoint> continuousDataPoints = new List<continuousDataPoint>();
    private string continuousData;
    private TextWriter tw;
    public bool isRecording;

    private List<actionDataPoint> actionDataPoints = new List<actionDataPoint>();
    private string actionData;
    private TextWriter tw2;

    private GameObject levelController;
    private GameObject menu;
    private GameMenuManager gmm;

    void Start() {

        // Set up the file:
        continuousData = Application.dataPath + "/ContinuousDataLog.csv";
        tw = new StreamWriter(continuousData, false);
        tw.WriteLine("Timestamp, Right Grab Value, Left Grip Value, Frustration Level, Emotion, Menu Count");
        tw.Close();

        actionData = Application.dataPath + "/ActionDataLog.csv";
        tw2 = new StreamWriter(actionData, false);
        tw2.WriteLine("Start Time, End Time, Grip Down Length, Grip Up Length, Controller Displacement, Frustration Level, Emotion, Menu Count");
        tw2.Close();

        levelController = GameObject.FindGameObjectWithTag("levelController");
        levelController.SetActive(false);
        menu = GameObject.FindGameObjectWithTag("menu");
        gmm = menu.GetComponent<GameMenuManager>();

    }

    void Update() {
        if (isRecording) {
            ReadGripValues();
            continuousDataPoint currentDataPoint = new continuousDataPoint(Time.time, rightGripValue, leftGripValue);
            continuousDataPoints.Add(currentDataPoint);
        }
    }

    public void WriteToCSV(float frustrationScore, string emotion, int menuCount) {
        isRecording = false;
        if (continuousDataPoints.Count > 0) {
            tw = new StreamWriter(continuousData, true);

            foreach (continuousDataPoint dp in continuousDataPoints) {
                tw.WriteLine(dp.timestamp + "," + dp.rightGrabValue + "," + dp.leftGrabValue + "," + frustrationScore + "," + emotion + "," + menuCount);
            }

            tw.Close();
        }
        continuousDataPoints.Clear();
        isRecording = true;
    }
    public void WriteFirstToCSV(float frustrationScore, string emotion, int menuCount) {
        isRecording = false;
        tw = new StreamWriter(continuousData, true);
        tw.WriteLine("-,-,-,"  + frustrationScore + "," + emotion + "," + menuCount);
        tw.Close();
        isRecording = true;
    }
    public void WriteActionsToCSV(float frustrationScore, string emotion, int menuCount) {
        isRecording = false;
        if (actionDataPoints.Count > 0) {
            tw2 = new StreamWriter(actionData, true);

            foreach (actionDataPoint adp in actionDataPoints) {
                tw2.WriteLine(adp.startTime + "," + adp.endTime + "," + adp.gripDownLength + "," + adp.gripUpLength + "," + adp.controllerActionDisplacement + "," + frustrationScore + "," + emotion + "," + menuCount);
            }

            tw2.Close();
        }
        actionDataPoints.Clear();
        isRecording = true;
    }

    public void WriteFirstActionToCSV(float frustrationScore, string emotion, int menuCount) {
        isRecording = false;
        tw2 = new StreamWriter(actionData, true);
        tw2.WriteLine("-,-,-,-,-," + frustrationScore + "," + emotion + "," + menuCount);
        tw2.Close();
        isRecording = true;
    }

    public void AddActionToList(float startTime, float endTime, float gripDownLength, float gripUpLength, float controllerActionDisplacement) {
        if (!isRecording) {
            return;
        }
        actionDataPoint currentActionPoint = new actionDataPoint(startTime, endTime, gripDownLength, gripUpLength, controllerActionDisplacement);
        actionDataPoints.Add(currentActionPoint);
    }

    private void ReadGripValues() {
        rightGripValue = rightGripAction.action.ReadValue<float>();
        leftGripValue = leftGripAction.action.ReadValue<float>();
    }

    public void StartGame() {
        isRecording = true;
        levelController.SetActive(true);
        gmm.showMenu();
    }

}
