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
    [SerializeField] private float minJumpForce = 0.5f;
    [SerializeField] private float maxJumpForce = 1.5f;
    [SerializeField] private float maxHoldTime = 2f;
    [SerializeField] private InputActionReference respawnBowReference;
    [SerializeField] private GameObject bow;
    [SerializeField] private GameObject headCamera;
    [SerializeField] private float bowSpawnDistance = 0.5f;

    private Rigidbody rb;
    private XROrigin xrOrigin;
    private CapsuleCollider col;
    private float startTime;

    [SerializeField] private float smoothTime = 0.5f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private GameMenuManager gmm;
    [SerializeField] private float menuDropThreshold = 30f;
    [SerializeField] private float menuTimePeriod = 120f;
    private float menuTimer = 0;
    private float lastGroundedHeight;
    private bool grounded = true;

    private Vector3 playerStartingPos;
    

    // This is a shortcut to run a function everytime you want to evaluate the bool:
    // This needs to run a raycast collision with objects on a ground layer
    public bool IsGrounded => Physics.Raycast(
        new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z),
        Vector3.down, 2.0f, ~playerLayer);

    void Start() {
        rb = GetComponent<Rigidbody>();
        xrOrigin = GetComponent<XROrigin>();
        col = GetComponent<CapsuleCollider>();
        jumpActionReference.action.performed += OnJump;
        jumpActionReference.action.canceled += ReleaseJump;
        respawnBowReference.action.performed += SpawnBow;
        menuTimer = 0f;
        playerStartingPos = transform.position;

    }

    void Update() {
        var center = xrOrigin.CameraInOriginSpacePos;
        col.center = new Vector3(center.x, col.center.y, center.z);
        col.height = xrOrigin.CameraInOriginSpaceHeight;

        // When you hit the ground for the first time
        if (IsGrounded && grounded == false) {
            //Debug.Log("You hit the ground"); 
            if (this.transform.position.y < (lastGroundedHeight - menuDropThreshold)) {
                //Debug.Log("You hit the ground from very high");
                gmm.showMenu();
                menuTimer = 0;
            }
            grounded = true;
        }

        // Ground checking
        if (IsGrounded) {
            lastGroundedHeight = this.transform.position.y;
        } else {
            grounded = false;
        }
        
        // Menu timer nonsense:
        if (gmm.menuShowing == false) {
            menuTimer += Time.deltaTime;
        }
        if (menuTimer > menuTimePeriod && grounded) {
            gmm.showMenu();
            menuTimer = 0;
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            TeleportPlayer(playerStartingPos);
        }
    }

    private void SpawnBow(InputAction.CallbackContext obj) {
        GameObject[] currentBows = GameObject.FindGameObjectsWithTag("bow");
        foreach (GameObject bow in currentBows) {
            Destroy(bow);
        }
        // I want to spawn the bow right in front of the players head probably
        Vector3 bowPos = headCamera.transform.position + (headCamera.transform.forward * bowSpawnDistance);

        Instantiate(bow, bowPos, Quaternion.identity);
    }

    private void OnJump(InputAction.CallbackContext obj) {
        startTime = Time.time;
    }

    private void ReleaseJump(InputAction.CallbackContext obj) {
        if (!IsGrounded) return;
        float duration = Time.time - startTime;
        float multiplier = Mathf.Lerp(minJumpForce, maxJumpForce, Mathf.Clamp((duration / maxHoldTime), 0, 1));
        rb.AddForce(Vector3.up * jumpForce * multiplier);
    }

    public void TeleportPlayer(Vector3 targetPos) {
        //Debug.Log("Player teleported");
        Vector3 heightAdjustedPosition = new Vector3(targetPos.x, targetPos.y + col.height, targetPos.z);
        this.transform.position = heightAdjustedPosition;
        
        // I need to fix this lerping and add an effect if I want it in here
        //StartCoroutine(TeleportNumerator(targetPos));
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

    void OnCollisionStay(Collision other) {
        if(other.gameObject.tag == "platform") {
            transform.parent = other.transform;
        }
    }

    void OnCollisionExit(Collision other) {
        if(other.gameObject.tag == "platform") {
            transform.parent = null;
        }
    }
}
