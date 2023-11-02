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

    private void Awake()
    {
        InitializeRigidbody();
        //Debug.Log("Initialization complete.");
    }


    public void AdjustThrottle(float throttle)
    {
        //Debug.Log("incoming throttle is " + throttle);

        this.currentThrottle += throttle;
        this.currentThrottle = Mathf.Clamp(this.currentThrottle, 0f, 100f);

        //Debug.Log("this.throttle is " + this.currentThrottle);
    }


    public void SetRollTorque(float rollFactor)
    {
        //Debug.Log("incoming Roll is " + rollFactor);
        this.currentRollTorqueFactor = rollFactor;
    }

    public void SetPitchTorque(float pitchFactor)
    {
        if (pitchFactor < 0) { s.color = Color.green; w.color = Color.white; }
        else if (pitchFactor > 0) { w.color = Color.green; s.color = Color.white; }
        else
        {
            w.color = Color.white; s.color = Color.white;
        }
        //Debug.Log("incoming Pitch is " + pitchFactor);
        this.currentPitchTorqueFactor = pitchFactor;
    }

    public void SetYawTorque(float yawFactor)
    {
        if (yawFactor < 0) { a.color = Color.green; d.color = Color.white; }
        else if (yawFactor > 0) { d.color = Color.green; a.color = Color.white; }
        else
        {
            a.color = Color.white; d.color = Color.white;
        }
        //Debug.Log("incoming yaw is " + yawFactor);
        this.currentYawTorqueFactor = yawFactor;
    }

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
