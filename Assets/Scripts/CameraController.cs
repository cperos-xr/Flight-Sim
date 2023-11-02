using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private List<Transform> povs;
    [SerializeField] private float speed;

    private int index = 0;
    private Vector3 target;

    // Declare InputAction for POV Switch
    public InputAction switchPOVUpAction;
    public InputAction switchPOVDownAction;

    private void Awake()
    {
        // Initialize input actions for switching POVs
        switchPOVUpAction = new InputAction("SwitchPOVUp", InputActionType.Button, "<Keyboard>/pageUp");
        switchPOVDownAction = new InputAction("SwitchPOVDown", InputActionType.Button, "<Keyboard>/pageDown");
    }

    private void OnEnable()
    {
        // Enable the actions
        switchPOVUpAction.Enable();
        switchPOVDownAction.Enable();
    }

    private void OnDisable()
    {
        // Disable the actions
        switchPOVUpAction.Disable();
        switchPOVDownAction.Disable();
    }

    private void Update()
    {
        // Handle POV switching
        if (switchPOVUpAction.triggered)
        {
            index = (index + 1) % povs.Count; // Loop index if it exceeds list size
            Debug.Log($"POV Up triggered. Index set to: {index}");
        }
        else if (switchPOVDownAction.triggered)
        {
            index = (index - 1 + povs.Count) % povs.Count; // Loop index if it becomes negative
            Debug.Log($"POV Down triggered. Index set to: {index}");
        }

        target = povs[index].position;
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
        transform.forward = povs[index].forward;
    }
}
