// test script to see if the player will move and vr is working

using UnityEngine;

public class FlyForward : MonoBehaviour
{
    public float speed = 2.0f;  // flight speed

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
