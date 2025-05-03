using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

// Keeps the camera's roll axis (Z) upright.
public class CameraRotation : MonoBehaviour
{
    public GameObject cam;

    void Update()
    {
        float x_angle = cam.transform.eulerAngles.x;
        float y_angle = cam.transform.eulerAngles.y;
        Quaternion target = Quaternion.Euler(x_angle, y_angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 5.0f);
    }
}
