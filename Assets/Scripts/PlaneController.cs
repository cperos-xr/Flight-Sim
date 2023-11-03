using UnityEngine;
using UnityEngine.UI;

public class PlaneController : MonoBehaviour
{


    public SO_ShipConfiguration shipConfiguration;

    public delegate void StatusUpdateHandler(PlaneController sender);
    public event StatusUpdateHandler OnStatusUpdate;

    private float currentThrottle;
    private float currentRollTorqueFactor;
    private float currentPitchTorqueFactor;
    private float currentYawTorqueFactor;

    public AudioSource engineSound;
    public AudioSource pitchSound;
    public AudioSource yawRollSound;
    public float Throttle => currentThrottle;
    public float Velocity => rb.velocity.magnitude;
    public float Altitude => transform.position.y;

    public float PitchTorqueFactor => currentPitchTorqueFactor;
    public float RollTorqueFactor => currentRollTorqueFactor;
    public float YawTorqueFactor => currentYawTorqueFactor;

    public Rigidbody rb;

    public Transform prop;

    public Image w;
    public Image a;
    public Image s;
    public Image d;

    private float originalEngineVolume;

    private void Awake()
    {
        InitializeRigidbody();
        //Debug.Log("Initialization complete.");
    }


    public void AdjustThrottle(float throttle)
    {
        //Debug.Log("incoming throttle is " + throttle);

        this.currentThrottle += throttle;
        this.currentThrottle = Mathf.Clamp(this.currentThrottle, 0f, 80f);

        if (engineSound != null)
        {
            engineSound.volume = currentThrottle / 100f; // Normalize throttle value to a 0.0 - 1.0 range for volume
        }

        //Debug.Log("this.throttle is " + this.currentThrottle);
    }


    public void SetYawTorque(float yawFactor)
    {
        // Color logic (existing)
        if (yawFactor < 0) { a.color = Color.green; d.color = Color.white; }
        else if (yawFactor > 0) { d.color = Color.green; a.color = Color.white; }
        else
        {
            a.color = Color.white; d.color = Color.white;
        }

        this.currentYawTorqueFactor = yawFactor;

        // Sound and volume logic (new)
        HandleYawRollSoundAndVolume();
    }

    public void SetRollTorque(float rollFactor)
    {
        this.currentRollTorqueFactor = rollFactor;

        // Sound and volume logic (new)
        HandleYawRollSoundAndVolume();
    }

    // This method encapsulates the logic for handling sound and volume for both yaw and roll
    private void HandleYawRollSoundAndVolume()
    {
        bool isYawOrRollBeingApplied = Mathf.Abs(currentYawTorqueFactor) > 0f || Mathf.Abs(currentRollTorqueFactor) > 0f;

        if (yawRollSound != null && engineSound != null)
        {
            if (isYawOrRollBeingApplied)
            {
                if (!yawRollSound.isPlaying)
                {
                    yawRollSound.loop = true;
                    yawRollSound.Play();
                }
                engineSound.volume = Mathf.Lerp(engineSound.volume, originalEngineVolume * 0.5f, Time.deltaTime * 2.5f); // Slowed down interpolation
            }
            else
            {
                if (yawRollSound.isPlaying)
                {
                    yawRollSound.Stop();
                }
                engineSound.volume = Mathf.Lerp(engineSound.volume, originalEngineVolume, Time.deltaTime * 2.5f); // Slowed down interpolation
            }
        }
    }


    //public void SetRollTorque(float rollFactor)
    //{
    //    //Debug.Log("incoming Roll is " + rollFactor);
    //    this.currentRollTorqueFactor = rollFactor;
    //}

    public void SetPitchTorque(float pitchFactor)
    {
        // ... (Existing color change and debug code)

        this.currentPitchTorqueFactor = pitchFactor;

        // Pitch sound play/stop logic
        if (pitchSound != null && engineSound != null)
        {
            if (Mathf.Abs(currentPitchTorqueFactor) > 0f)
            {
                // If not already playing, start playing the pitch sound
                if (!pitchSound.isPlaying)
                {
                    pitchSound.loop = true;
                    pitchSound.Play();
                }

                // Reduce the volume of the engine sound while pitch is being applied
                engineSound.volume = Mathf.Lerp(engineSound.volume, originalEngineVolume * 0.5f, Time.deltaTime * 5f); // Reduce volume to 50% as an example
            }
            else
            {
                // Stop the pitch sound when there's no pitch input
                if (pitchSound.isPlaying)
                    pitchSound.Stop();

                // Return the volume of the engine sound back to original
                engineSound.volume = Mathf.Lerp(engineSound.volume, originalEngineVolume, Time.deltaTime * 5f);
            }
        }
    }

    //public void SetYawTorque(float yawFactor)
    //{
    //    if (yawFactor < 0) { a.color = Color.green; d.color = Color.white; }
    //    else if (yawFactor > 0) { d.color = Color.green; a.color = Color.white; }
    //    else
    //    {
    //        a.color = Color.white; d.color = Color.white;
    //    }
    //    //Debug.Log("incoming yaw is " + yawFactor);
    //    this.currentYawTorqueFactor = yawFactor;
    //}

    private void InitializeRigidbody()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = shipConfiguration.mass;
        rb.angularDrag = shipConfiguration.angularDrag;
        rb.drag = shipConfiguration.drag;
        //Debug.Log($"Rigidbody initialized with Mass: {rb.mass}, AngularDrag: {rb.angularDrag}, Drag: {rb.drag}");
    }

    private float responseModifier
    {
        get
        {
            float value = ((rb.mass / 10) * shipConfiguration.responsiveness);
            //Debug.Log($"Response Modifier calculated: {value}");
            return value;
        }
    }

    private void Update()
    {
        currentThrottle = Mathf.Clamp(currentThrottle, 0f, 100f);
        //Debug.Log($"Throttle clamped value: {throttle}");
        prop.Rotate(Vector3.back * currentThrottle);

        OnStatusUpdate?.Invoke(this); // Raise the event
    }


    private void FixedUpdate()
    {
        rb.AddForce(transform.forward * shipConfiguration.maxThrust * currentThrottle);
        //Debug.Log($"Force applied in forward direction: {transform.forward * shipConfiguration.maxThrust * throttle}");

        rb.AddTorque(transform.up * currentYawTorqueFactor * responseModifier);
        //Debug.Log($"Torque applied in up direction (yaw): {transform.up * yaw * responseModifier}");

        rb.AddTorque(transform.right * currentPitchTorqueFactor * responseModifier);
        //Debug.Log($"Torque applied in right direction (pitch): {transform.right * pitch * responseModifier}");

        rb.AddTorque(transform.forward * currentRollTorqueFactor * responseModifier);
        //Debug.Log($"Torque applied in forward direction (roll): {transform.forward * roll * responseModifier}");

        rb.AddForce(Vector3.up * rb.velocity.magnitude * shipConfiguration.lift);
        //Debug.Log($"Force applied in up direction (lift): {Vector3.up * rb.velocity.magnitude * shipConfiguration.lift}");

    }

}
