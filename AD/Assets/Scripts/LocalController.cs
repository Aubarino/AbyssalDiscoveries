using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LocalController : MonoBehaviour
{
    float bobOffset; //naturally approaches 0. use for impacts and shit
    public float bobOffsetTarget; //change this on impacts
    public float bobSpeed;
    public float inAirBobSpeed;
    public float bobIntensity;
    public BodyScript currentBody;
    float currentBobSin;
    float bobTime;
    private void Update()
    {
        if(currentBody.grounded)
        bobTime += Time.deltaTime * bobSpeed * currentBody.GetRelativeSpeed();
        else
        {
            bobTime += Time.deltaTime * bobSpeed * currentBody.GetRelativeSpeed();
        }
        bobOffsetTarget = Mathf.Lerp(bobOffsetTarget, 0f, 5f * Time.deltaTime);
        bobOffset = Mathf.SmoothStep(bobOffset, bobOffsetTarget, 5f * Time.deltaTime);
    }
    private void FixedUpdate()
    {
        print(Input.GetAxisRaw("Vertical"));
        currentBody.moveVector = Vector3.forward * Input.GetAxisRaw("Vertical") + Vector3.right * Input.GetAxisRaw("Horizontal");
        transform.position = currentBody.transform.TransformPoint(currentBody.desiredCameraPosition + Vector3.up * Mathf.Sin(bobTime) * bobIntensity + Vector3.up * bobOffset);
    }
}
