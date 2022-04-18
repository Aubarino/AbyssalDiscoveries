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
    public float lookSensitivity;
    public BodyScript currentBody;
    public List<BodyScript> switchableBodies = new List<BodyScript>();
    public int cbody = 0;
    float currentBobSin;
    float bobTime;
    float cameraRot;
    private void Start()
    {
        currentBody.OnJump.AddListener(CharacterJumped);
        currentBody.OnLand.AddListener(CharacterLanded);
    }

    public void SwitchToBody(BodyScript body)
    {
        currentBody = body;
    }
    private void Update()
    {
        if(currentBody.grounded)
        bobTime += Time.deltaTime * bobSpeed * currentBody.GetRelativeSpeed();
        else
        {
            bobTime += Time.deltaTime * inAirBobSpeed;
        }
        bobOffsetTarget = Mathf.Lerp(bobOffsetTarget, 0f, 5f * Time.deltaTime);
        bobOffset = Mathf.Lerp(bobOffset, bobOffsetTarget, 5f * Time.deltaTime);
        if(Input.GetKeyDown(KeyCode.Space))
        {
            currentBody.Jump();
        }
        Vector2 curPos = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        currentBody.transform.Rotate(new Vector3(0, curPos.x * lookSensitivity * Time.deltaTime, 0), Space.Self);
        cameraRot -= curPos.y * lookSensitivity * Time.deltaTime;
        if (cameraRot > 90) cameraRot = 90;
        if (cameraRot < -90) cameraRot = -90;
        transform.eulerAngles = new Vector3(cameraRot, currentBody.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
        Cursor.lockState = CursorLockMode.Locked;
        if(Input.GetKeyDown(KeyCode.C))
        {
            cbody++;
            if(cbody >= switchableBodies.Count)
            {
                cbody = 0;
            }
            SwitchToBody(switchableBodies[cbody]);
        }
    }
    private void FixedUpdate()
    {
        print(Input.GetAxisRaw("Vertical"));
        currentBody.moveVector = Vector3.forward * Input.GetAxisRaw("Vertical") + Vector3.right * Input.GetAxisRaw("Horizontal");
        transform.position = currentBody.transform.TransformPoint(currentBody.desiredCameraPosition + Vector3.up * Mathf.Sin(bobTime) * bobIntensity + Vector3.up * bobOffset);
    }
    void CharacterJumped() //called when the character jumps
    {
        bobOffsetTarget -= 1f;
    }
    void CharacterLanded() //called when the character lands
    {
        bobOffsetTarget -= 1f;
    }
}
