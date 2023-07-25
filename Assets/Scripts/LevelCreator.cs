using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private int numberOfPlatforms;
    [SerializeField] private float minHeight = 10f;
    [SerializeField] private float maxHeight = 10f;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float minSpawnDistance = 1f;
    [SerializeField] private float yOffsetToSpawnPlatforms = 30f;
    private float currentHeight = 0f;
    private Vector2 previousPosInCircle = Vector2.zero;
    private Vector2 currentPosInCircle;
    private GameObject player;
    private float platformCount = 0;
    [SerializeField] private float maxPlatform = 100;

    //Difficulty values:
    [SerializeField] private float levelSpeedIncrease = 0.05f;
    [SerializeField] private float maxSpeedScore = 5f;
    private float levelSpeedScore = 1f;
    [Range(0,1)] [SerializeField] private float maxProportionOfMovingPlatforms = 0.5f;
    [SerializeField] private float maxPlatformScale = 1;
    [SerializeField] private float minPlatformScale = 0.5f;
    private Vector3 startingScale;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        startingScale = platformPrefab.transform.localScale;
        GenerateLevel();
    }
    
    void Update() {
        if(player.transform.position.y + yOffsetToSpawnPlatforms > currentHeight) {
            GenerateLevel();
        }
    }

    private void GenerateLevel() {
        // Get the new height
        currentHeight += Random.Range(minHeight, maxHeight);

        // Get the position in a certain radius
        GenerateFlatCoordsForPlatform();

        // Spawn platform
        Vector3 nextPosition = new Vector3(currentPosInCircle.x, currentHeight, currentPosInCircle.y);
        var platform = Instantiate(platformPrefab, nextPosition, Quaternion.identity);

        // Decide what kind of platform it is here
        if(shouldPlatformMove(ReMap(platformCount, 0, maxPlatform, 0, maxProportionOfMovingPlatforms))) {
            PlatformBehaviour pb = platform.GetComponent<PlatformBehaviour>();
            pb.SetCircularMovement(true);
            if (platformCount % 2 == 1) {
                pb.rotationDirection = -1f;
            }
            pb.movementSpeed = pb.movementSpeed * levelSpeedScore; 
        }
        
        // Need to change the scale here
        float scale = ReMap(platformCount, 0, maxPlatform, maxPlatformScale, minPlatformScale);
        platform.transform.localScale = new Vector3(scale * startingScale.x, scale * startingScale.y, scale * startingScale.z);

        // Set up for next spawn:
        if (platformCount < maxPlatform) {
            platformCount++;
        }

        if (levelSpeedScore < maxSpeedScore) {
            levelSpeedScore += levelSpeedIncrease;
        }

        previousPosInCircle = currentPosInCircle;
        platform.transform.parent = this.transform; 
    }

    private void GenerateFlatCoordsForPlatform() {
        currentPosInCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector2 difference = currentPosInCircle - previousPosInCircle;
        if (difference.magnitude < minSpawnDistance) {
            // get a new randomPosInCircle
            GenerateFlatCoordsForPlatform();
        }
    }

    private bool shouldPlatformMove(float input) {
        float val = Random.Range(0,maxPlatform);
        val = val / maxPlatform;
        if (input > val) {
            return true;
        } else {
            return false;
        }
    }
    private float ReMap(float value, float from1, float to1, float from2, float to2) {
        // Debug.Log((value - from1) / (to1 - from1) * (to2 - from2) + from2);
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
