using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BowController : MonoBehaviour
{
    [SerializeField]
    private BowString bowStringRenderer;
    private XRGrabInteractable interactable;
    [SerializeField] private Transform midPointGrabObject, midPointVisualObject, midPointParent;
    [SerializeField] private float bowStringStretchLimit = .3f;
    private Transform interactor;

    private void Awake() {
        interactable = midPointGrabObject.GetComponent<XRGrabInteractable>();
    }
    
    private void Start() {
        interactable.selectEntered.AddListener(PrepareBowString);
        interactable.selectExited.AddListener(ResetBowString);
    }

    private void PrepareBowString(SelectEnterEventArgs arg0) {
        interactor = arg0.interactableObject.transform;
    }

    private void ResetBowString(SelectExitEventArgs arg0) {
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
            midPointVisualObject.localPosition = new Vector3(midPointLocalSpace.x, 0, 0);
        }
    }

    private void HandleStringPulledBackToLimit(float midPointLocalZAbs, Vector3 midPointLocalSpace)
    {
        if (midPointLocalSpace.x < 0 && midPointLocalZAbs >= bowStringStretchLimit) {
            midPointVisualObject.localPosition = new Vector3(-bowStringStretchLimit, 0, 0);
        }
    }

    private void HandleStringPushedBackToStart(Vector3 midPointLocalSpace)
    {
        if (midPointLocalSpace.x >= 0) {
            midPointVisualObject.localPosition = Vector3.zero;
        }
    }
}
