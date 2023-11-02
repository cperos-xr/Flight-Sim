using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HUDManager : MonoBehaviour
{
    public List<PlaneController> planeControllers;
    public TextMeshProUGUI hud;
    private int activePlaneIndex;

    // Declare InputAction for POV Switch
    public InputAction tabAction;


    private void OnEnable()
    {
        // Enable the actions
        tabAction.Enable();
    }

    private void OnDisable()
    {
        // Disable the actions
        tabAction.Disable();
    }

    private void Awake()
    {
        tabAction = new InputAction("Tab", InputActionType.Button, "<Keyboard>/tab");
        foreach (var planeController in planeControllers)
        {
            planeController.OnStatusUpdate += UpdateHUD;
        }
    }

    private void OnDestroy()
    {
        foreach (var planeController in planeControllers)
        {
            planeController.OnStatusUpdate -= UpdateHUD;
        }
    }

    private void Update()
    {
        if (tabAction.triggered)
        {
            Debug.Log("Switching HUD");
            SetActivePlane((activePlaneIndex + 1) % planeControllers.Count);
        }
    }

    public void SetActivePlane(int index)
    {
        if (index >= 0 && index < planeControllers.Count)
        {
            activePlaneIndex = index;
        }
    }

    public void UpdateHUD(PlaneController planeController)
    {
        // Update the HUD only if the event comes from the active plane
        if (planeController == planeControllers[activePlaneIndex])
        {
            hud.text = $"Throttle {planeController.Throttle.ToString("F0")} %\n" +
                       $"Velocity {(planeController.Velocity * 3.6).ToString("F0")} Km/Hr\n" +
                       $"Altitude {planeController.Altitude.ToString("F0")} M";
        }
    }
}
