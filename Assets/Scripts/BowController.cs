using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class BowController : MonoBehaviour
{
    [SerializeField]
    private BowString bowStringRenderer;
    private XRGrabInteractable interactable;
    [SerializeField] private Transform midPointGrabObject, midPointVisualObject, midPointParent;
    [SerializeField] private float bowStringStretchLimit = .3f;
    private Transform interactor;
    private float strength, previousStrength;
    public UnityEvent OnBowPulled;
    public UnityEvent<float> OnBowReleased;
    [SerializeField] private float stringSoundThreshold = 0.001f;
    [SerializeField] private AudioSource bowDrawAudioSource;

    // Action data point variables:
    private GameObject gm;
    private GameController gc;
    [SerializeField] private InputActionProperty rightGripAction;
    private float gripThreshold = 0.05f;
    private float gripValue;
    private float startTime;
    private float endTime;
    private bool pinching;
    private bool releasing;
    private bool holding;
    private float gripDownTime;
    private float gripUpTime;
    private float releaseStartTime;
    private GameObject controller;
    private Vector3 initialControllerPos;
    private Vector3 finalControllerPos;
    private float controllerActionDisplacement;

    private void Awake() {
        interactable = midPointGrabObject.GetComponent<XRGrabInteractable>();
    }
    
    private void Start() {
        interactable.selectEntered.AddListener(PrepareBowString);
        interactable.selectExited.AddListener(ResetBowString);
        gm = GameObject.FindGameObjectWithTag("gameManager");
        gc = gm.GetComponent<GameController>();
        controller = GameObject.FindGameObjectWithTag("controller");
    }

    private void PrepareBowString(SelectEnterEventArgs arg0) {
        interactor = arg0.interactableObject.transform;

        // Start of action:
        startTime = Time.time;
        pinching = true;
        initialControllerPos = controller.transform.position;

        OnBowPulled?.Invoke();
    }

    private void ResetBowString(SelectExitEventArgs arg0) {

        OnBowReleased?.Invoke(strength);
        strength = 0;
        previousStrength = 0;
        bowDrawAudioSource.pitch = 1;
        bowDrawAudioSource.Stop();

        interactor = null;
        midPointGrabObject.localPosition = Vector3.zero;
        midPointVisualObject.localPosition = Vector3.zero;
        bowStringRenderer.CreateString(null);
    }

    private void Update() {
        gripValue = rightGripAction.action.ReadValue<float>();
        if (pinching) {
            if (gripValue > (1 - gripThreshold)) {
                pinching = false;
                gripDownTime = Time.time - startTime;
                holding = true;
            }
        }
        if (holding) {
            if (gripValue < (1 - gripThreshold)) {
                holding = false;
                releaseStartTime = Time.time;
                releasing = true;
            }
        }
        if (releasing) {
            if (gripValue < gripThreshold) {
                releasing = false;
                gripUpTime = Time.time - releaseStartTime;

                // End of action:
                endTime = Time.time;
                finalControllerPos = controller.transform.position;
                controllerActionDisplacement = (finalControllerPos - initialControllerPos).magnitude;

                gc.AddActionToList(startTime, endTime, gripDownTime, gripUpTime, controllerActionDisplacement);

                // Debug.Log(startTime);
                // Debug.Log(endTime);
                // Debug.Log(gripDownTime);
                // Debug.Log(gripUpTime);
                // Debug.Log(controllerActionDisplacement);
            }
        }
        if (interactor != null) {
            Vector3 midPointLocalSpace = midPointParent.InverseTransformPoint(midPointGrabObject.position);
            float midPointLocalXAbs = Mathf.Abs(midPointLocalSpace.x);

            previousStrength = strength;

            HandleStringPushedBackToStart(midPointLocalSpace);
            HandleStringPulledBackToLimit(midPointLocalXAbs, midPointLocalSpace);
            HandlePullingString(midPointLocalXAbs, midPointLocalSpace);

            bowStringRenderer.CreateString(midPointVisualObject.position);

            //bowStringRenderer.CreateString(midPointGrabObject.position);
        }
    }

    private void HandlePullingString(float midPointLocalXAbs, Vector3 midPointLocalSpace)
    {
        if (midPointLocalSpace.x < 0 && midPointLocalXAbs < bowStringStretchLimit) {

            if (bowDrawAudioSource.isPlaying == false && strength <= 0.01f) {
                bowDrawAudioSource.Play();
            }

            strength = Remap(midPointLocalXAbs, 0, bowStringStretchLimit, 0, 1);
            midPointVisualObject.localPosition = new Vector3(midPointLocalSpace.x, 0, 0);

            PlayStringPullingSound();
        }
    }

    private void PlayStringPullingSound()
    {
        if (Mathf.Abs(strength - previousStrength) > stringSoundThreshold) {
            if (strength < previousStrength) {
                bowDrawAudioSource.pitch = -1;
            } else {
                bowDrawAudioSource.pitch = 1;
            }
            bowDrawAudioSource.UnPause();
        } else {
            bowDrawAudioSource.Pause();
        }
    }

    private float Remap(float value, int fromMin, float fromMax, int toMin, int toMax)
    {
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }

    private void HandleStringPulledBackToLimit(float midPointLocalZAbs, Vector3 midPointLocalSpace)
    {
        if (midPointLocalSpace.x < 0 && midPointLocalZAbs >= bowStringStretchLimit) {
            bowDrawAudioSource.Pause();
            strength = 1;
            midPointVisualObject.localPosition = new Vector3(-bowStringStretchLimit, 0, 0);
        }
    }

    private void HandleStringPushedBackToStart(Vector3 midPointLocalSpace)
    {
        if (midPointLocalSpace.x >= 0) {
            bowDrawAudioSource.pitch = 1;
            bowDrawAudioSource.Stop();
            strength = 0;
            midPointVisualObject.localPosition = Vector3.zero;
        }
    }
}
