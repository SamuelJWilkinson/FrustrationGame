using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ValueLogging : MonoBehaviour
{
    public InputActionProperty rightPinchAction;
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

    // On screen text variables:
    public TMP_Text triggerText;
    public TMP_Text isPinchingText;
    public TMP_Text pinchDurationText;
    public TMP_Text pinchDownDurationText;
    public TMP_Text pinchReleaseDurationText;
    public TMP_Text gripText;

    void Update() {
        ReadValues();
        if (!pinching && triggerValue >= pinchThreshold) {
            pinching = true;
            pinchDown = true;
            isPinchingText.SetText("Pinch: " + pinching);
            pinchStart = Time.time;
        }
        if (pinchDown && triggerValue >= (1-pinchThreshold)) {
            pinchDown = false;
            pinchDownDuration = Time.time - pinchStart;
            pinchDownDurationText.SetText("Pinch down duration: " + pinchDownDuration);
        }
        if (pinching && triggerValue <= pinchThreshold) {
            pinching = false;
            isPinchingText.SetText("Pinch: " + pinching);
            pinchDuration = Time.time - pinchStart;
            pinchDurationText.SetText("Pinch duration: " + pinchDuration);
        }
    }

    private void ReadValues() {

        // I should probably clamp this value at some point:
        triggerValue = rightPinchAction.action.ReadValue<float>();
        triggerText.SetText("Pinch: " + triggerValue);

        gripValue = rightGripAction.action.ReadValue<float>();
        gripText.SetText("Grip: " + gripValue);
    }
}
