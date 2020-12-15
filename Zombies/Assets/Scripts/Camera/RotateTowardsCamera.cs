using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsCamera : MonoBehaviour
{
    /*
     * Rotate a GameObject sprite to always face the player 
     */

    Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Vector3 cameraPos = cam.transform.position;
        Vector3 mPos = transform.position;
        mPos.y = 0;

        Vector3 dirToCam = (cameraPos - mPos).normalized;
        float angleToCam = GetAngleFromVectorFloat(dirToCam);
        transform.eulerAngles = new Vector3(0f, -angleToCam + 90 + 180, 0f);
    }

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}
