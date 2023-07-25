using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private GameObject midPointVisual, arrowPrefab, arrowSpawnPoint;
    [SerializeField] private float arrowMaxSpeed = 10f;
    [SerializeField] private AudioSource bowReleaseAudioSource;

    [SerializeField] private GameObject tutorial;

    public void PrepareArrow() {
        midPointVisual.SetActive(true);
    }

    public void ReleaseArrow(float strength) {
        bowReleaseAudioSource.Play();
        midPointVisual.SetActive(false);
        
        GameObject arrow = Instantiate(arrowPrefab);
        arrow.transform.position = arrowSpawnPoint.transform.position;
        arrow.transform.rotation = arrowSpawnPoint.transform.rotation;
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.AddForce(arrowSpawnPoint.transform.forward * strength * arrowMaxSpeed, ForceMode.Impulse);

        tutorial = GameObject.FindGameObjectWithTag("tutorial");
        if (tutorial != null) {
            TutorialManager tm = tutorial.GetComponent<TutorialManager>();
            tm.OnShoot();
        }
        

        Destroy(arrow, 5);
    } 
}
