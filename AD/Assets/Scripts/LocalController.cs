using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
[RequireComponent(typeof(Camera))]
public class LocalController : MonoBehaviour
{
    float bobOffset; //naturally approaches 0. use for impacts and shit
    public float bobOffsetTarget = 0; //change this on impacts. determines how much up or down the camera goes
    public float bobSpeed = 3.8f; //how quickly the camera bobs when moving
    public float inAirBobSpeed = 0.4f; //how quickly the camera bobs when in air
    public float bobIntensity = 0.02f; //how much the camera bobs up and down
    public float lookSensitivity = 300f; //how much you look around
    public BodyScript currentBody; //the currently controlled body
    public List<BodyScript> switchableBodies = new List<BodyScript>(); //all the bodies you can switch to. this will get replaced in the future
    public int cbody = 0; //the current body, used in switching
    float currentBobSin; //idk, unused
    float bobTime; //current bob height and stuff
    float cameraRot; //idfk its copied from another game of mine
    Volume underwaterVol;
    GameObject distortionPlane;
    private void Start()
    {
        currentBody.OnJump.AddListener(CharacterJumped); //listeners for jumping and landing, for camera bob
        currentBody.OnLand.AddListener(CharacterLanded);
        underwaterVol = transform.GetChild(1).GetComponent<Volume>();
        underwaterVol.weight = 0f;
        distortionPlane = transform.GetChild(2).gameObject;
        distortionPlane.SetActive(false);
    }

    public void SwitchToBody(BodyScript body)
    {
        transform.rotation = Quaternion.identity; //reset camera rotation so its not broken
        currentBody.OnJump.RemoveListener(CharacterJumped);
        currentBody.OnLand.RemoveListener(CharacterLanded);
        currentBody = body;
        currentBody.OnJump.AddListener(CharacterJumped);
        currentBody.OnLand.AddListener(CharacterLanded);
    }
    private void Update()
    {
        if(currentBody.grounded)
        bobTime += Time.deltaTime * bobSpeed * currentBody.GetRelativeSpeed(); //if grounded, we add to the bobtime
        else
        {
            bobTime += Time.deltaTime * inAirBobSpeed; //if not grounded, we add using inairbobspeed
        }
        bobOffsetTarget = Mathf.Lerp(bobOffsetTarget, 0f, 5f * Time.deltaTime); //math for moving the camera up and down on impacts
        bobOffset = Mathf.Lerp(bobOffset, bobOffsetTarget, 5f * Time.deltaTime);
        if(Input.GetKeyDown(KeyCode.Space))
        {
            currentBody.Jump(); //jump
        }
        Vector2 curPos = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")); //current mouse position ig
        currentBody.Look(curPos * Time.deltaTime * lookSensitivity); //look around using the mouse position
        if(!currentBody.inWater)
        {
            cameraRot -= curPos.y * lookSensitivity * Time.deltaTime; //if not in water, do the usual look around
            if (cameraRot > 90) cameraRot = 90;
            if (cameraRot < -90) cameraRot = -90;
            transform.eulerAngles = new Vector3(cameraRot, currentBody.transform.eulerAngles.y, currentBody.transform.eulerAngles.z);
            underwaterVol.weight = Mathf.Lerp(underwaterVol.weight, 0f, Time.deltaTime * 10f);
            distortionPlane.SetActive(false);
        }
        else
        {
            transform.rotation = currentBody.transform.rotation; //if in water, match our camera position with the body
            if(Input.GetKey(KeyCode.Space))
            {
                currentBody.moveVector.y = 1f; //move up and down with space and shift
            }
            else if(Input.GetKey(KeyCode.LeftShift))
            {
                currentBody.moveVector.y = -1f;
            }
            else { currentBody.moveVector.y = 0f; }
            underwaterVol.weight = Mathf.Lerp(underwaterVol.weight, 1f, Time.deltaTime * 10f);
            distortionPlane.SetActive(true);

        }

        Cursor.lockState = CursorLockMode.Locked; //lock the cursor
        if(Input.GetKeyDown(KeyCode.C))
        {
            cbody++;
            if(cbody >= switchableBodies.Count) //if we press C, switch
            {
                cbody = 0;
            }
            SwitchToBody(switchableBodies[cbody]);
        }
        if(Input.GetKey(KeyCode.E))
        {
            currentBody.sideVector = 1f; //rotate around in water if we press Q or E
        }
        else if(Input.GetKey(KeyCode.Q))
        {
            currentBody.sideVector = -1f;
        }
        else currentBody.sideVector = 0f;
        if (Input.GetKeyDown(KeyCode.R) && !currentBody.inWater) currentBody.EnterWater(); //for debugging the water
        else if (Input.GetKeyDown(KeyCode.R) && currentBody.inWater) { currentBody.ExitWater(); transform.rotation = Quaternion.identity; }
    }
    private void FixedUpdate()
    {
        //print(Input.GetAxisRaw("Vertical"));
        currentBody.moveVector = ((Vector3.forward * Input.GetAxisRaw("Vertical")) + (Vector3.right * (Input.GetAxisRaw("Horizontal")))); //set the body's move vector
        transform.position = currentBody.transform.TransformPoint(currentBody.desiredCameraPosition + ((Vector3.up * Mathf.Sin(bobTime)) * bobIntensity) + (Vector3.up * bobOffset)); //move the camera to be where the body wants it to be
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
