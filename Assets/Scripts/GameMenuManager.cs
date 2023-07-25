using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour
{
    public GameObject menu;
    public GameController gc;
    public InputActionProperty showButton;
    public Transform head;
    public Slider frustrationSlider;
    public float spawnDistance = 2;
    public bool menuShowing = false;
    public Toggle highlightedToggle;
    private string emotion;
    private int frustrationLevel;
    private int menuShowCount = 0;


    // Start is called before the first frame update
    void Start()
    {
        menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (showButton.action.WasPressedThisFrame()) {
            showMenu();
        }

        if (Input.GetKeyDown(KeyCode.M)) {
            showMenu();
        }

        if (Input.GetKeyDown(KeyCode.H)) {
            hideMenu();
        }

        if (menu.activeSelf == true) {
            menu.transform.position = head.position + new Vector3(head.forward.x, 0, head.forward.z).normalized * spawnDistance;
            menu.transform.LookAt(new Vector3(head.position.x, menu.transform.position.y, head.position.z));
            menu.transform.forward *= -1;
        }
    }

    public void showMenu() {
        menuShowing = true;
        menu.SetActive(true);

    }

    public void hideMenu() {
        menuShowing = false;
        menu.SetActive(false);
    }

    public void updateToggles(Toggle currentToggle) {
        highlightedToggle = currentToggle;
    }

    public void SubmitButton() {
        hideMenu();
        gc.WriteToCSV(frustrationSlider.value, highlightedToggle.name, menuShowCount);
        gc.WriteActionsToCSV(frustrationSlider.value, highlightedToggle.name, menuShowCount);
        menuShowCount++;
    }
}
