using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference jumpActionReference;
    [SerializeField] private float jumpForce = 500.0f;
    [SerializeField] private LayerMask playerLayer;

    private Rigidbody rb;
    private XROrigin xrOrigin;
    private CapsuleCollider col;

    [SerializeField] private float smoothTime = 0.5f;
    private Vector3 velocity = Vector3.zero;
    

    // This is a shortcut to run a function everytime you want to evaluate the bool:
    public bool IsGrounded => Physics.Raycast(
        new Vector2(transform.position.x, transform.position.y + 2.0f),
        Vector3.down, 2.0f, ~playerLayer);

    void Start() {
        rb = GetComponent<Rigidbody>();
        xrOrigin = GetComponent<XROrigin>();
        col = GetComponent<CapsuleCollider>();
        jumpActionReference.action.performed += OnJump;
    }

    void Update() {
        var center = xrOrigin.CameraInOriginSpacePos;
        col.center = new Vector3(center.x, col.center.y, center.z);
        col.height = xrOrigin.CameraInOriginSpaceHeight;
    }

    private void OnJump(InputAction.CallbackContext obj) {
        if (!IsGrounded) return;
        rb.AddForce(Vector3.up * jumpForce);
    }

    public void TeleportPlayer(Vector3 targetPos) {
        //Debug.Log("Player teleported");
        //this.transform.position = targetPos;
        StartCoroutine(TeleportNumerator(targetPos));
    }

    IEnumerator TeleportNumerator(Vector3 targetPos) {
        float timeElapsed = 0;
        while (timeElapsed < smoothTime) {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
    }
}
