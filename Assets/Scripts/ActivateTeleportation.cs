using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ActivateTeleportation : MonoBehaviour
{
    [SerializeField] private GameObject leftTeleportation;
    [SerializeField] private GameObject rightTeleportation;
    public InputActionProperty leftActivate;
    public InputActionProperty rightActivate;
    [SerializeField] private PlayerController pc;

    void Update() {
        bool grounded = pc.IsGrounded;

        leftTeleportation.SetActive(leftActivate.action.ReadValue<float>() > 0.1f && grounded);
        rightTeleportation.SetActive(rightActivate.action.ReadValue<float>() > 0.1f && grounded);
    }
}
