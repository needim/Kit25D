using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class VeryBasicFollowCam : MonoBehaviour
{
    public Transform target;
    public float minY = -0.73f;

    public void LateUpdate()
    {
        Vector3 targetPos = target.position;
        targetPos.z = transform.position.z;
        targetPos.y = minY;
        transform.position = Vector3.Lerp(transform.position, targetPos, 2f);
    }
}