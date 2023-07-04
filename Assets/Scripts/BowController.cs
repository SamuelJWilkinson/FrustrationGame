using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class BowController : MonoBehaviour
{
    [SerializeField]
    private BowString bowStringRenderer;
    private XRGrabInteractable interactable;
    [SerializeField] private Transform midPointGrabObject, midPointVisualObject, midPointParent;
    [SerializeField] private float bowStringStretchLimit = .3f;
    private Transform interactor;
    private float strength;
    public UnityEvent OnBowPulled;
    public UnityEvent<float> OnBowReleased;

    private void Awake() {
        interactable = midPointGrabObject.GetComponent<XRGrabInteractable>();
    }
    
    private void Start() {
        interactable.selectEntered.AddListener(PrepareBowString);
        interactable.selectExited.AddListener(ResetBowString);
    }

    private void PrepareBowString(SelectEnterEventArgs arg0) {
        interactor = arg0.interactableObject.transform;

        OnBowPulled?.Invoke();
    }

    private void ResetBowString(SelectExitEventArgs arg0) {
        OnBowReleased?.Invoke(strength);
        strength = 0;

        interactor = null;
        midPointGrabObject.localPosition = Vector3.zero;
        midPointVisualObject.localPosition = Vector3.zero;
        bowStringRenderer.CreateString(null);
    }

    private void Update() {
        if (interactor != null) {
            Vector3 midPointLocalSpace = midPointParent.InverseTransformPoint(midPointGrabObject.position);
            float midPointLocalXAbs = Mathf.Abs(midPointLocalSpace.x);

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
            strength = Remap(midPointLocalXAbs, 0, bowStringStretchLimit, 0, 1);
            midPointVisualObject.localPosition = new Vector3(midPointLocalSpace.x, 0, 0);
        }
    }

    private float Remap(float value, int fromMin, float fromMax, int toMin, int toMax)
    {
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }

    private void HandleStringPulledBackToLimit(float midPointLocalZAbs, Vector3 midPointLocalSpace)
    {
        if (midPointLocalSpace.x < 0 && midPointLocalZAbs >= bowStringStretchLimit) {
            strength = 1;
            midPointVisualObject.localPosition = new Vector3(-bowStringStretchLimit, 0, 0);
        }
    }

    private void HandleStringPushedBackToStart(Vector3 midPointLocalSpace)
    {
        if (midPointLocalSpace.x >= 0) {
            strength = 0;
            midPointVisualObject.localPosition = Vector3.zero;
        }
    }
}
