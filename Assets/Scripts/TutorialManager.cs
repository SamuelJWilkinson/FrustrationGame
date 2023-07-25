using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private InputActionReference jumpActionReference;
    private Vector3 startingPos;
    [SerializeField] private GameObject teleportTutorial;
    private bool teleportTutorialCompleted = false;

    [SerializeField] private GameObject jumpTutorial;
    private bool jumpingTutorialCompleted = false;

    [SerializeField] private GameObject shootTutorial;
    private bool shootingTutorialCompleted = false;

    public GameObject menu;
    public Transform head;
    public float spawnDistance = 4;
    [SerializeField] GameController gc;

    void Start() {
        startingPos = player.transform.position;
        jumpActionReference.action.performed += OnJump;
    }
    void Update() {
        if (!teleportTutorialCompleted) {
            // Show teleport tutorial:
            CheckTeleportation();
        }
        if (menu.activeSelf == true) {
            menu.transform.position = head.position + new Vector3(head.forward.x, 0, head.forward.z).normalized * spawnDistance;
            menu.transform.LookAt(new Vector3(head.position.x, menu.transform.position.y, head.position.z));
            menu.transform.forward *= -1;
        }
    }

    private void CheckTeleportation() {
        if ( (player.transform.position - startingPos).magnitude > 1.5f ) {
            teleportTutorialCompleted = true;
            teleportTutorial.SetActive(false);
            jumpTutorial.SetActive(true);
        }
    }

    private void OnJump(InputAction.CallbackContext obj) {
        if (teleportTutorialCompleted && !jumpingTutorialCompleted) {
            jumpingTutorialCompleted = true;
            jumpTutorial.SetActive(false);
            shootTutorial.SetActive(true);
        }
    }

    public void OnShoot() {
        if (teleportTutorialCompleted && jumpingTutorialCompleted && !shootingTutorialCompleted) {
            shootingTutorialCompleted = true;
            gc.StartGame();
            Destroy(this.gameObject);
        }
    }
}
