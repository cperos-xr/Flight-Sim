using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public delegate void OnPitchChanged(float pitch);
    public event OnPitchChanged PitchChanged;

    public delegate void OnRollChanged(float roll);
    public event OnRollChanged RollChanged;

    public delegate void OnYawChanged(float yaw);
    public event OnYawChanged YawChanged;

    public delegate void OnThrottleIncrease(float throttleIncrease);
    public event OnThrottleIncrease ThrottleIncreased;

    public delegate void OnThrottleDecrease(float throttleDecrease);
    public event OnThrottleDecrease ThrottleDecreased;

    public delegate void OnEngageAction();
    public event OnEngageAction EngagePressed;


    public InputAction pitchAction;
    public InputAction rollAction;
    public InputAction yawAction;
    public InputAction throttleAction;

    public InputAction engageAction;

    public bool inversePitchControls;
    public bool inverseRollControls;

    private float prevRoll;
    private float prevPitch;
    private float prevYaw;


    private void Awake()
    {
        throttleAction = new InputAction("Throttle", InputActionType.Value, "<Keyboard>/downArrow");
        throttleAction.AddBinding("<Keyboard>/upArrow").WithProcessor("Invert");


        // Initialize input actions
        pitchAction = new InputAction("Pitch", InputActionType.Value, "<Keyboard>/s");
        pitchAction.AddBinding("<Keyboard>/w").WithProcessor("Invert");

        // Roll now uses e/q instead of q/e
        rollAction = new InputAction("Roll", InputActionType.Value, "<Keyboard>/e");
        rollAction.AddBinding("<Keyboard>/q").WithProcessor("Invert");


        // Yaw now uses a/d, but a is inverted
        yawAction = new InputAction("Yaw", InputActionType.Value, "<Keyboard>/d");
        yawAction.AddBinding("<Keyboard>/a").WithProcessor("Invert");

        engageAction = new InputAction("Engage", InputActionType.Button, "<Keyboard>/space");
    }

    private void OnEnable()
    {
        throttleAction.Enable();

        // Enable the actions
        pitchAction.Enable();
        rollAction.Enable();
        yawAction.Enable();

        engageAction.Enable();

    }

    private void OnDisable()
    {
        throttleAction.Disable();
        // Disable the actions
        pitchAction.Disable();
        rollAction.Disable();
        yawAction.Disable();
        engageAction.Disable();

    }

    private void Update()
    {
        // Read the input values here
        float roll = rollAction.ReadValue<float>();
        float pitch = pitchAction.ReadValue<float>();
        float yaw = yawAction.ReadValue<float>();
 
        if (inversePitchControls)
        {
            pitch = -pitch;
        }

        if (inverseRollControls)
        {
            roll = -roll;
        }

        if (pitch != prevPitch)
        {
            prevPitch = pitch;
            PitchChanged?.Invoke(pitch);
        }

        if (roll != prevRoll)
        {
            prevRoll = roll;
            RollChanged?.Invoke(roll);
        }

        if (yaw != prevYaw)
        {
            prevYaw = yaw;
            YawChanged?.Invoke(yaw);
        }

        if (engageAction.triggered)
        {
            EngagePressed?.Invoke();
        }

        float throttle = throttleAction.ReadValue<float>();

        if (throttle > 0)
        {
            ThrottleIncreased?.Invoke(-throttle);
        }
        else if (throttle < 0)
        {
            ThrottleDecreased?.Invoke(throttle);
        }


    }
}
