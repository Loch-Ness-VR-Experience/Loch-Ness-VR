using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class VRMovementController : MonoBehaviour
{
    // Movement mode enum
    public enum MovementMode
    {
        Flying,
        Stationary
    }

    [Header("Movement Settings")]
    [SerializeField] private MovementMode currentMode = MovementMode.Flying;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float flyingSpeed = 5f;
    [SerializeField] private float waypointReachedDistance = 0.1f;
    [SerializeField] private float landingDuration = 2f;

    [Header("Look Settings")]
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;

    [Header("Events")]
    public UnityEvent onFlightStart;
    public UnityEvent onWaypointReached;
    public UnityEvent onLandingStart;
    public UnityEvent onLandingComplete;


    private int currentWaypointIndex = 0;
    private bool isLanding = false;
    private float verticalLookRotation = 0f;
    private Transform cameraTransform;

    private void Start()
    {

        cameraTransform = Camera.main.transform;

        if (waypoints.Length == 0) //If there's no waypoints, don't do anything
        {
            Debug.LogError("No waypoints assigned to VR Movement Controller!");
            return;
        }

        // Start in flying mode
        SetMovementMode(MovementMode.Flying);

        // Trigger flight start event
        onFlightStart.Invoke();
    }

    private void Update()
    {
        switch (currentMode)
        {
            case MovementMode.Flying:
                HandleFlyingMovement();
                break;

            case MovementMode.Stationary:
                HandleStationaryLooking();
                break;
        }

        // Test input for changing modes (can be removed or modified for VR input)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentMode == MovementMode.Flying)
                StartLanding();
            else
                SetMovementMode(MovementMode.Flying);
        }
    }

    private void HandleFlyingMovement()
    {
        if (isLanding || waypoints.Length == 0)
            return;

        // Get current waypoint
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Calculate direction and move towards the waypoint
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        transform.position += direction * flyingSpeed * Time.deltaTime;

        // Smoothly rotate towards the waypoint direction
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);

        // Check if we've reached the waypoint
        float distanceToWaypoint = Vector3.Distance(transform.position, targetWaypoint.position);
        if (distanceToWaypoint < waypointReachedDistance)
        {
            // Trigger waypoint reached event
            onWaypointReached.Invoke();

            // Move to next waypoint
            currentWaypointIndex++;

            // If we've reached the final waypoint, start landing
            if (currentWaypointIndex >= waypoints.Length)
            {
                StartLanding();
            }
        }
    }

    private void HandleStationaryLooking()
    {
        // In VR, head rotation would be handled by the VR system
        // This code is for testing in non-VR mode

        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        // Apply horizontal rotation to the entire object
        transform.Rotate(Vector3.up * mouseX);

        // Apply vertical rotation to the camera only
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
    }

    private void StartLanding()
    {
        if (isLanding)
            return;

        isLanding = true;
        onLandingStart.Invoke();

        // Start landing coroutine
        StartCoroutine(LandingSequence());
    }

    private IEnumerator LandingSequence()
    {
        // Store initial position and rotation
        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;

        // Define landing position (you may want to set this to a specific landing point)
        Vector3 landingPosition = new Vector3(initialPosition.x, 1.7f, initialPosition.z); // Height of average person's eyes
        Quaternion landingRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f); // Level rotation

        float elapsedTime = 0f;

        while (elapsedTime < landingDuration)
        {
            float t = elapsedTime / landingDuration;

            t = Mathf.SmoothStep(0f, 1f, t);

            // Interpolate position and rotation
            transform.position = Vector3.Lerp(initialPosition, landingPosition, t);
            transform.rotation = Quaternion.Slerp(initialRotation, landingRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we're exactly at the landing position and rotation
        transform.position = landingPosition;
        transform.rotation = landingRotation;

        // Set mode to stationary
        SetMovementMode(MovementMode.Stationary);

        // Trigger landing complete event
        onLandingComplete.Invoke();

        isLanding = false;
    }

    public void SetMovementMode(MovementMode mode)
    {
        currentMode = mode;

        // Reset waypoint index if returning to flying mode
        if (mode == MovementMode.Flying)
        {
            currentWaypointIndex = 0;
        }
    }
}