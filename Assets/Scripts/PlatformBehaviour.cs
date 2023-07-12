using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class PlatformBehaviour : MonoBehaviour
{
    // When the player is below the platform it should always be intangible
    [SerializeField] private Material intangibleMaterial;
    [SerializeField] private float yOffset = 0.1f;
    private Material tangibleMaterial;
    private Renderer rend;
    private MeshCollider mc;
    private GameObject player;
    private bool intangible = false;
    private bool movingCircularly = false;
    private bool movingDiagonally = true;
    private Vector3 startPos;
    private Vector3 oppositePos;
    public float movementSpeed = 5f;
    public float rotationDirection = 1f;

    void Start() {
        mc = GetComponent<MeshCollider>();
        player = GameObject.FindGameObjectWithTag("Player");
        rend = GetComponent<Renderer>();
        tangibleMaterial = rend.material;
        startPos = transform.position;
    }

    void Update() {
        if (player.transform.position.y + yOffset < transform.position.y) {
            // If the player is below the platform then we need to make it intangible
            MakePlatformIntangible();
        }
    }

    void FixedUpdate() {
        if (movingCircularly) {
            // Do something
            MovePlatformCircular();
        }
        if (movingDiagonally) {

        }
    }

    public void MakePlatformTangible() {
        mc.isTrigger = false;
        rend.material = tangibleMaterial;
    }

    public void MakePlatformIntangible() {
        mc.isTrigger = true;
        rend.material = intangibleMaterial;
    }

    private void MovePlatformCircular() {
        transform.RotateAround(Vector3.zero, Vector3.up, rotationDirection * movementSpeed * Time.deltaTime);
    }

    private void MovePlatformDiagonally() {
        // Diagonal code goes here when I can be bothered to implement it

    }

    public void SetCircularMovement(bool movingStatus) {
        movingCircularly = movingStatus;
    }
}
