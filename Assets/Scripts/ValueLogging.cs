using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ValueLogging : MonoBehaviour
{
    public InputActionProperty rightPinchAction;
    [SerializeField] private GameObject rightHandController;
    private float triggerValue;
    private bool pinching = false;
    private bool pinchDown = false;
    private bool pinchRelease = false;
    [SerializeField] private float pinchThreshold = 0.05f;
    private float pinchStart;
    private float pinchDuration;
    private float pinchDownDuration;

    public InputActionProperty rightGripAction;
    private float gripValue;

    void Update() {
        ReadValues();
        if (!pinching && triggerValue >= pinchThreshold) {
            pinching = true;
            pinchDown = true;
            pinchStart = Time.time;
        }
        if (pinchDown && triggerValue >= (1-pinchThreshold)) {
            pinchDown = false;
            pinchDownDuration = Time.time - pinchStart;
        }
        if (pinching && triggerValue <= pinchThreshold) {
            pinching = false;
            pinchDuration = Time.time - pinchStart;
        }
    }

    private void ReadValues() {

        // I should probably clamp this value at some point:
        triggerValue = rightPinchAction.action.ReadValue<float>();

        gripValue = rightGripAction.action.ReadValue<float>();
    }
}
