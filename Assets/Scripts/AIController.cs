using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class AIController : MonoBehaviour
{

    const float DIRECTION_UP = -1;
    const float DIRECTION_DOWN = 1;
    const float DIRECTION_LEFT = 1;
    const float DIRECTION_RIGHT = -1;
    const float DIRECTION_NONE = 0;

    const float ROLL_NONE = 0;
    const float ROLL_CCW = 1;
    const float ROLL_CW = -1;

    [Serializable]
    public enum EPlaneState
    {
        Grounded,
        TakeOff,
        SelectingTarget,
        FollowPlayer,
        notReinventingWheel,
        Leveling,
        TurningAround,
        AvoidingObstacle
    }

    public TextMeshProUGUI stateText;



    public PlaneController planeController;
    public InputManager inputManager;

    [SerializeField] private List<PlaneController> targetPlanes = new List<PlaneController>();
    [SerializeField] private PlaneController currentActiveTarget;

    [SerializeField] private float minTargetDistance = 30f;
    [SerializeField] private float maxTargetDistance = 60f;

    [SerializeField] private float throttleIncrement = 0.05f;

    [SerializeField] private float altitudeThreshold = 10f;

    public float tooClose = 100;


    PlaneData planeData = new PlaneData();

    private int stableFrameCount = 0;  // Initialize a counter
    private int requiredStableFrames = 100;  // Set the required stable frames

    public EPlaneState state = EPlaneState.Grounded;


    void Awake()
    {
        currentActiveTarget = targetPlanes[0];
        inputManager.EngagePressed += EngageAI;
    }

    void OnDestroy()
    {
        inputManager.EngagePressed -= EngageAI;
    }

    private void EngageAI()
    {
        Debug.Log("Engage AI");
        state = EPlaneState.TakeOff;
    }


    private void TakeOff()
    {
        //Debug.Log("Initiating take off");

        float takeoffThrottle = 65;

        if (planeController.Throttle < takeoffThrottle)
        {
            planeController.AdjustThrottle(throttleIncrement);
        }

        if (planeController.Altitude > 5)
        {
            Debug.Log("Plane is now flying!");
            state = EPlaneState.SelectingTarget;
        }
    }

    private void Update()
    {
        UpdatePlaneData();
        planeController.SetYawTorque(DIRECTION_NONE);
        planeController.SetPitchTorque(DIRECTION_NONE);
        planeController.SetRollTorque(ROLL_NONE);


        switch (state)
        {
            default: break;
            case EPlaneState.Grounded:
                break;
            case EPlaneState.TakeOff:
                TakeOff();
                break;
            case EPlaneState.SelectingTarget:
                SelectClosestTarget();
                break;
            case EPlaneState.FollowPlayer:
                FollowPlayer();
                break;
            case EPlaneState.Leveling:
                LevelPlane();
                break;
            case EPlaneState.TurningAround:
                TurnAround();
                break;
            case EPlaneState.notReinventingWheel:
                TheEasyWayThatWillAlwaysWorkBetter();
                break;
            case EPlaneState.AvoidingObstacle:
                ObstacleAvoidance();
                break;

        }

        stateText.text = state.ToString();

    }

    private void CheckForObstacles()
    {
        Vector3 position = planeController.transform.position;
        Vector3 forwardDirection = planeController.transform.forward;

        bool checkFront = CastRayAndCheckObstacle(position, forwardDirection, tooClose);
        bool checkRight = CastRayAndCheckObstacle(position, Quaternion.Euler(0, 45, 0) * forwardDirection, tooClose);
        bool checkLeft = CastRayAndCheckObstacle(position, Quaternion.Euler(0, -45, 0) * forwardDirection, tooClose);
        bool checkUp = CastRayAndCheckObstacle(position, Quaternion.Euler(-45, 0, 0) * forwardDirection, tooClose);
        bool checkDown = CastRayAndCheckObstacle(position, Quaternion.Euler(45, 0, 0) * forwardDirection, tooClose);

        bool checkRight1 = CastRayAndCheckObstacle(position, Quaternion.Euler(0, 35, 0) * forwardDirection, tooClose);
        bool checkLeft1 = CastRayAndCheckObstacle(position, Quaternion.Euler(0, -35, 0) * forwardDirection, tooClose);
        bool checkUp1 = CastRayAndCheckObstacle(position, Quaternion.Euler(-35, 0, 0) * forwardDirection, tooClose);
        bool checkDown1 = CastRayAndCheckObstacle(position, Quaternion.Euler(35, 0, 0) * forwardDirection, tooClose);

        bool checkRight2 = CastRayAndCheckObstacle(position, Quaternion.Euler(0, 25, 0) * forwardDirection, tooClose);
        bool checkLeft2 = CastRayAndCheckObstacle(position, Quaternion.Euler(0, -25, 0) * forwardDirection, tooClose);
        bool checkUp2 = CastRayAndCheckObstacle(position, Quaternion.Euler(-25, 0, 0) * forwardDirection, tooClose);
        bool checkDown2 = CastRayAndCheckObstacle(position, Quaternion.Euler(25, 0, 0) * forwardDirection, tooClose);


        // Create a rotation of -90 degrees around the X-axis
        bool obstacleDetected = false;
        if (checkRight || checkLeft || checkUp || checkDown || checkFront ||
            checkRight1 || checkLeft1 || checkUp1 || checkDown1 ||
            checkRight2 || checkLeft2 || checkUp2 || checkDown2)
        {
            obstacleDetected = true;
        }

        if (obstacleDetected)
        {
            state = EPlaneState.AvoidingObstacle;
        }
    }

    private void ObstacleAvoidance()
    {
        Vector3 position = planeController.transform.position;
        Vector3 forwardDirection = planeController.transform.forward;

        //Debug.Log($"Plane Position: {position}");
        //Debug.Log($"Forward Direction: {forwardDirection}");

        // Cast rays in different directions and get distances
        float distanceRight = CastRayAndGetDistance(position, Quaternion.Euler(0, 45, 0) * forwardDirection, tooClose);
        float distanceLeft = CastRayAndGetDistance(position, Quaternion.Euler(0, -45, 0) * forwardDirection, tooClose);
        float distanceUp = CastRayAndGetDistance(position, Quaternion.Euler(-45, 0, 0) * forwardDirection, tooClose);
        float distanceDown = CastRayAndGetDistance(position, Quaternion.Euler(45, 0, 0) * forwardDirection, tooClose);
        float distanceFront = CastRayAndGetDistance(position, forwardDirection, tooClose);

        // Cast additional rays for averaging
        float distanceRight1 = CastRayAndGetDistance(position, Quaternion.Euler(0, 35, 0) * forwardDirection, tooClose);
        float distanceLeft1 = CastRayAndGetDistance(position, Quaternion.Euler(0, -35, 0) * forwardDirection, tooClose);
        float distanceUp1 = CastRayAndGetDistance(position, Quaternion.Euler(-35, 0, 0) * forwardDirection, tooClose);
        float distanceDown1 = CastRayAndGetDistance(position, Quaternion.Euler(35, 0, 0) * forwardDirection, tooClose);
        float distanceRight2 = CastRayAndGetDistance(position, Quaternion.Euler(0, 25, 0) * forwardDirection, tooClose);
        float distanceLeft2 = CastRayAndGetDistance(position, Quaternion.Euler(0, -25, 0) * forwardDirection, tooClose);
        float distanceUp2 = CastRayAndGetDistance(position, Quaternion.Euler(-25, 0, 0) * forwardDirection, tooClose);
        float distanceDown2 = CastRayAndGetDistance(position, Quaternion.Euler(25, 0, 0) * forwardDirection, tooClose);

        // Calculate average distances
        float averageDistanceRight = (distanceRight + distanceRight1 + distanceRight2) / 3;
        float averageDistanceLeft = (distanceLeft + distanceLeft1 + distanceLeft2) / 3;
        float averageDistanceUp = (distanceUp + distanceUp1 + distanceUp2) / 3;
        float averageDistanceDown = (distanceDown + distanceDown1 + distanceDown2) / 3;

        /*
        // Log average distances
        Debug.Log($"Average Distance Right: {averageDistanceRight}");
        Debug.Log($"Average Distance Left: {averageDistanceLeft}");
        Debug.Log($"Average Distance Up: {averageDistanceUp}");
        Debug.Log($"Average Distance Down: {averageDistanceDown}");
        */

        // Check if there's an obstacle in front
        if (distanceFront < tooClose)
        {
            //Debug.Log("Obstacle detected in front");

            // Throttle adjustment if too close
            if (planeController.Throttle > 10)
            {
                //Debug.Log($"Throttle too high ({planeController.Throttle}), reducing by {throttleIncrement}");
                planeController.AdjustThrottle(-10);
            }

            // Determine the direction with the greatest average distance
            //float maxAverageDistance = Mathf.Max(new float[] { averageDistanceRight, averageDistanceLeft, averageDistanceUp, averageDistanceDown });
            ////Debug.Log($"Maximum average avoidance distance: {maxAverageDistance}");

            //// Set the direction based on the maximum average distance
            //if (maxAverageDistance == averageDistanceDown)
            //{
            //    //Debug.Log("Turning Down");
            //    planeController.SetPitchTorque(DIRECTION_DOWN);
            //}
            //else if (maxAverageDistance == averageDistanceRight)
            //{
            //    //Debug.Log("Turning Right");
            //    planeController.SetYawTorque(DIRECTION_RIGHT);
            //}
            //else if (maxAverageDistance == averageDistanceLeft)
            //{
            //    //Debug.Log("Turning Left");
            //    planeController.SetYawTorque(DIRECTION_LEFT);
            //}
            //else if (maxAverageDistance == averageDistanceUp)
            //{
            //    //Debug.Log("Turning Up");
            //    planeController.SetPitchTorque(DIRECTION_UP);
            //}

            // Calculate minimum average distance
            float minAverageDistance = Mathf.Min(new float[] { averageDistanceRight, averageDistanceLeft, averageDistanceUp, averageDistanceDown });

            // If the minimum average distance is below a certain threshold, consider turning away from that direction
            if (minAverageDistance < tooClose)
            {
                // Determine the direction with the shortest average distance and turn away from it
                if (minAverageDistance == averageDistanceDown)
                {
                    // Turn Up
                    planeController.SetPitchTorque(DIRECTION_UP);
                }
                else if (minAverageDistance == averageDistanceRight)
                {
                    // Turn Left
                    planeController.SetYawTorque(DIRECTION_LEFT);
                }
                else if (minAverageDistance == averageDistanceLeft)
                {
                    // Turn Right
                    planeController.SetYawTorque(DIRECTION_RIGHT);
                }
                else if (minAverageDistance == averageDistanceUp)
                {
                    // Turn Down
                    planeController.SetPitchTorque(DIRECTION_DOWN);
                }
            }
        }
        else
        {
            //Debug.Log("No obstacle in the immediate vicinity. Switching to FollowPlayer state.");
            state = EPlaneState.FollowPlayer;
        }
    }



    // Modify this function to return the distance to the obstacle, or a large value if no obstacle is detected
    private float CastRayAndGetDistance(Vector3 position, Vector3 direction, float tooClose)
    {
        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        float maxRayDistance = 1000f; // Set this to your maximum considered distance

        if (Physics.Raycast(ray, out hit, maxRayDistance))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red); // Draw a red line to show the ray
            ObstacleController obstacleController = hit.collider.gameObject.GetComponent<ObstacleController>();
            float obstacleDistance = Vector3.Distance(position, hit.point);

            if (obstacleController) // it is an obstacle
            {
                return obstacleDistance;
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * maxRayDistance, Color.green); // Draw a green line if nothing is hit
        }
        return maxRayDistance; // Return a large number to indicate 'no obstacle' or 'maximum distance'
    }


    private bool CastRayAndCheckObstacle(Vector3 position, Vector3 direction, float tooClose)
    {
        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red); // Draw a red line to show the ray
            ObstacleController obstacleController = hit.collider.gameObject.GetComponent<ObstacleController>();
            float obstacleDistance = Vector3.Distance(position, hit.point);

            if (obstacleController && obstacleDistance < tooClose) // it is an obstacle and it is too close
            {
                return true;
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.green); // Draw a green line if nothing is hit
        }
        return false;
    }


    private void UpdatePlaneData()
    {

        planeData.deltaAltitudeToTarget = currentActiveTarget.Altitude - planeController.Altitude;

        planeData.altitude = planeController.Altitude;
        planeData.pitch = planeController.PitchTorqueFactor;
        planeData.yaw = planeController.YawTorqueFactor;
        planeData.roll = planeController.RollTorqueFactor;
        planeData.velocity = planeController.Velocity;

        planeData.currentTime = Time.time;  //for smoothing
        planeData.currentFrame = Time.frameCount;
        planeData.pitchAngleToTarget = GetAscentAngle(Mathf.Abs(planeData.deltaAltitudeToTarget));
        planeData.currentRemappedPitchAngle = ConvertToSignedAngle(planeController.gameObject.transform.eulerAngles.x);

    }


    private void UpdateThrottle()
    {

        float currentDistance = Vector3.Distance(planeController.transform.position, currentActiveTarget.transform.position);

        //Debug.Log($"Current distance to target {currentDistance}, current ideal throttle {CalculateThrottle(currentDistance)}, planeDate.throttle is {planeController.Throttle}");

        if (planeController.Throttle > CalculateThrottle(currentDistance))
        {
            planeController.AdjustThrottle(-throttleIncrement);
        }
        else if (planeController.Throttle < CalculateThrottle(currentDistance))
        {
            planeController.AdjustThrottle(throttleIncrement);
        }
    }


    private float CalculateThrottle(float distanceToTarget)
    {
        float minThrottle = 20;
        float maxThrottle = 100;
        if (distanceToTarget <= minTargetDistance) return minThrottle;
        if (distanceToTarget >= maxTargetDistance) return maxThrottle;

        // Calculate the interpolation factor t
        float t = (distanceToTarget - minTargetDistance) / (maxTargetDistance - minTargetDistance);

        // Interpolate between min and max using the factor t
        float result = minThrottle + t * (maxThrottle - minThrottle);

        return result;
    }



    public struct PlaneData
    {
        public float throttle;
        public float roll;
        public float pitch;
        public float yaw;
        public float altitude;
        public float velocity;

        public float currentTime;
        public int currentFrame;
        public float deltaAltitudeToTarget;
        public float pitchAngleToTarget;

        public float currentRemappedPitchAngle;

        public PlaneController currentActiveTarget;  // *****do we need to do something here
    }


    private float ConvertToSignedAngle(float angle)
    {
        // Assuming 'angle' is an Euler angle that ranges from 0 to 360
        if (angle > 180)
        {
            return angle - 360;
        }
        return angle;
    }

    private void TheEasyWayThatWillAlwaysWorkBetter()
    {
        planeController.transform.LookAt(currentActiveTarget.transform.position);
    }

    private bool IsTargetInFrontOfPlane( Vector3 directionToTarget )
    {
        Vector3 normalizedPlaneForwardDirection = planeController.transform.forward;
        normalizedPlaneForwardDirection.Normalize();
        return Vector3.Dot(normalizedPlaneForwardDirection, directionToTarget) > 0;
    }

    private bool IsTooSteepIncline()
    {
        return Vector3.Dot(planeController.transform.forward, Vector3.up) > 0.95f;
    }
    private bool IsNosedive()
    {
        return Vector3.Dot(planeController.transform.forward, Vector3.up) < -0.95f;
    }
    private bool IsCapsized()
    {
        return Vector3.Dot(planeController.transform.up, Vector3.up) < 0.0f;
    }

    private void FollowPlayer()
    {
        CheckForObstacles();
        UpdateThrottle();
        Vector3 directionToTarget = currentActiveTarget.transform.position - planeController.transform.position;
        directionToTarget.Normalize();

        Vector3 planeForwardDirection = planeController.transform.forward;
        Vector3 planeRightDirection = planeController.transform.right;
        Vector3 planeUpDirection = planeController.transform.up;

        Vector3 forwardRightProjection = directionToTarget - Vector3.Dot(directionToTarget, planeUpDirection) * planeUpDirection;
        forwardRightProjection.Normalize();
        float horizontalAlignment = Vector3.Dot(planeForwardDirection, forwardRightProjection);

        Vector3 forwardUpProjection = directionToTarget - Vector3.Dot(directionToTarget, planeRightDirection) * planeRightDirection;
        forwardUpProjection.Normalize();
        float verticalAlignment = Vector3.Dot(planeForwardDirection, forwardUpProjection);

        float horizontalDeviation = Mathf.Acos(horizontalAlignment) * Mathf.Rad2Deg;
        float verticalDeviation = Mathf.Acos(verticalAlignment) * Mathf.Rad2Deg;
        float coneAngle = 20;
        float coneCosine = MathF.Cos(Mathf.Deg2Rad * coneAngle);

        if (IsCapsized())
        {
            state = EPlaneState.Leveling;
            return;
        }

        if (IsTooSteepIncline())
        {
            planeController.SetPitchTorque(DIRECTION_DOWN);
        }
        else if (IsNosedive())
        {
            planeController.SetPitchTorque(DIRECTION_UP);
        }
        else if (!IsTargetInFrontOfPlane(directionToTarget))
        {
            state = EPlaneState.TurningAround;
            return;
        }
        else
        {
            if (horizontalDeviation > verticalDeviation || !IsTargetInFrontOfPlane(directionToTarget))
            {
                planeController.SetPitchTorque(DIRECTION_NONE);
                if (horizontalAlignment > coneCosine)
                {
                    planeController.SetYawTorque(DIRECTION_NONE);
                }
                else
                {
                    planeController.SetYawTorque(Vector3.Cross(planeForwardDirection, directionToTarget).y > 0 ? DIRECTION_LEFT : DIRECTION_RIGHT);
                }
            }
            else
            {
                planeController.SetYawTorque(DIRECTION_NONE);
                if (verticalAlignment > coneCosine)
                {
                    planeController.SetPitchTorque(DIRECTION_NONE);
                }
                else
                {
                    Vector3 pitchCrossProduct = Vector3.Cross(planeForwardDirection, forwardUpProjection);
                    if (Vector3.Dot(pitchCrossProduct, planeRightDirection) > 0)
                    {
                        // Target is above, pitch up
                        planeController.SetPitchTorque(DIRECTION_DOWN);
                    }
                    else
                    {
                        // Target is below, pitch down
                        planeController.SetPitchTorque(DIRECTION_UP);
                    }
                }
            }
        }
    }

    private void LevelPlane()
    {
        
        if (Vector3.Dot(planeController.transform.up, Vector3.up) < 0.25f)
        {
            planeController.SetRollTorque(ROLL_CW);
        }
        state = EPlaneState.FollowPlayer;
    }

    private void TurnAround()
    {
        Vector3 directionToTarget = currentActiveTarget.transform.position - planeController.transform.position;
        directionToTarget.Normalize();

        Vector3 planeForwardDirection = planeController.transform.forward;
        Vector3 planeUpDirection = planeController.transform.up;

        Vector3 forwardRightProjection = directionToTarget - Vector3.Dot(directionToTarget, planeUpDirection) * planeUpDirection;
        forwardRightProjection.Normalize();
        float horizontalAlignment = Vector3.Dot(planeForwardDirection, forwardRightProjection);

        float coneAngle = 30;
        float coneCosine = MathF.Cos(Mathf.Deg2Rad * coneAngle);

        if (IsCapsized())
        {
            state = EPlaneState.Leveling;
            return;
        }
        if (IsTooSteepIncline())
        {
            planeController.SetPitchTorque(DIRECTION_DOWN);
        }
        else if (IsNosedive())
        {
            planeController.SetPitchTorque(DIRECTION_UP);
        }
        else
        {
            if (IsTargetInFrontOfPlane(directionToTarget))
            {
                state = EPlaneState.FollowPlayer;
                return;
            }
            else
            {
                planeController.SetYawTorque(DIRECTION_LEFT);
            }
            
        }
    }

    private float GetAscentAngle(float targetAltitude)
    {
        float angle;

        if (targetAltitude < 0)
        {
            return -GetAscentAngle(-targetAltitude);
        }

        if (targetAltitude >= 350)
        {
            angle = 70;
        }
        else if (targetAltitude >= 200 && targetAltitude < 350)
        {
            angle = 35 + (targetAltitude - 200) * (35 / 150);
        }
        else if (targetAltitude >= 30 && targetAltitude < 200)
        {
            angle = 20 + (targetAltitude - 30) * (15 / 170);
        }
        else // For targetAltitude < 30
        {
            angle = 20;
        }

        return angle;
    }


    private void SelectClosestTarget()
    {
        float closestDistance = float.MaxValue; // Start with the largest possible float value.
        PlaneController closestPlane = null;

        foreach (PlaneController targetPlane in targetPlanes)
        {
            float distanceToTarget = Vector3.Distance(planeController.transform.position, targetPlane.transform.position);
            if (distanceToTarget < closestDistance)
            {
                closestDistance = distanceToTarget;
                closestPlane = targetPlane;
            }
        }

        if (closestPlane != null && currentActiveTarget != closestPlane)
        {
            Debug.Log($"Current Active Target is {closestDistance.ToString("F0")} meters away", currentActiveTarget);
            currentActiveTarget = closestPlane;
            
        }

        state = EPlaneState.FollowPlayer;
    }

}
