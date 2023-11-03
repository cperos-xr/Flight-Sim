
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Range(0.01f, 2.0f)]
    public float throttleSensitivity = 0.5f;

    public int score;
    
    public TextMeshProUGUI scoreText;

    public PlaneController planeController;
    public InputManager inputManager;

    void Awake()
    {
        // Subscribe to the InputManager events

        inputManager.YawChanged += planeController.SetYawTorque;
        inputManager.PitchChanged += planeController.SetPitchTorque;
        inputManager.RollChanged += planeController.SetRollTorque;

        inputManager.ThrottleIncreased += SetPlaneThrottle;
        inputManager.ThrottleDecreased += value => SetPlaneThrottle(-value);
    }

    void OnDestroy()
    {

        inputManager.YawChanged -= planeController.SetYawTorque;
        inputManager.PitchChanged -= planeController.SetPitchTorque;
        inputManager.RollChanged -= planeController.SetRollTorque;

        inputManager.ThrottleIncreased -= SetPlaneThrottle;
        inputManager.ThrottleDecreased -= value => SetPlaneThrottle(-value);


    }

    private void SetPlaneThrottle(float throttleValue)
    {
        planeController.AdjustThrottle(throttleValue * throttleSensitivity); // Now has throttle change with sensitivity
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name + " has entered the trigger zone of " + this.name, other.gameObject);
        ObstacleController oc = other.GetComponent<ObstacleController>();
        if (oc != null)
        {
            if (oc.obstacle is SO_Collectable collectable)
            {
                //Debug.Log(other.name + " has a collectableValue of " + collectable.value, other.gameObject);
                score += collectable.value;
                other.gameObject.SetActive(false);
                scoreText.text = "Score : " + score.ToString();
            }
        }

    }
}
