using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BodyScript : MonoBehaviour
{
    [Tooltip("If true, the body script will automatically setup the entire body, using the selected variables (mesh renderer, ragdoll(soon), etc.)")]
    public bool doSetup;
    bool didSetup; //true when the body finishes setup. its here so the body doesnt setup itself more that once
    [Tooltip("Change this to the mesh you want this to be, used for debugging usually. If null, a mesh renderer wont be addded.")]
    public Mesh bodyMesh;
    [Tooltip("Determines the dimensions of the capsule collider")]
    public float collHeight, collSize;
    [Tooltip("The body material (NOT FOR RAGDOLLS). Set this to a material with no bounciness and no friction, as the script handles it automatically")]
    public PhysicMaterial collMaterial;
    [Tooltip("The material used for the body mesh. Not used if the mesh is null")]
    public Material bodyMaterial;
    [Tooltip("The base movespeed of the creature. This can be affected by afflictions and whatnot, so you shouldn't use this directly.")]
    public float baseMoveSpeed, baseJumpSpeed;
    [Tooltip("The max magnitude of the horizontal speed. If the magnitude is above it, you cant accelerate anymore.")]
    public float maxHorizontalSpeed;
    [Tooltip("How quickly the entity slows down, multiplies the current speed. 0f is INSTANT slowdown, 1f is no slowdown.")]
    [Range(0f, 1f)]
    public float slowdown;
    [Tooltip("The mass of the main collider Rigidbody")]
    public float rbMass;
    [Tooltip("Determines which layers the script will consider as walkable (determines if the entity is grounded)")]
    public LayerMask walkableLayers;
    [Tooltip("If a camera was in this body, at what position should it be?")]
    public Vector3 desiredCameraPosition;


    public bool inWater, faceInWater; //determines if we are in or out of water
    [HideInInspector]
    public UnityEvent OnJump = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnLand = new UnityEvent();
    //here come all the hidden variables
    float jumpcool; //makes sure you cant jump 20 times a second. cooldown
    CapsuleCollider col; //the main collider
    Rigidbody rb; //the main rigidbody
    [HideInInspector]
    public bool grounded; //determines if the entity sees itself as grounded
    [HideInInspector]
    public Vector3 moveVector; //determines where the entity wants to move, in local space. eg. if Z is 1, the entity will move forward. if X is -1, the entity will move left. if Y is 1, the entity will try to swim upwards.
    public float sideVector; //when in water, rotate left and right
    private void Start()
    {
        if (doSetup && !didSetup)
        {
            didSetup = true; //make sure we dont do the setup twice
            if (bodyMesh != null) //if we set the bodmyesh, then we do shit with it
            {
                MeshRenderer mesh = gameObject.AddComponent<MeshRenderer>();
                MeshFilter filter = gameObject.AddComponent<MeshFilter>();
                filter.mesh = bodyMesh;
                mesh.material = bodyMaterial;
            }
            rb = gameObject.AddComponent<Rigidbody>(); //rb creation
            rb.mass = rbMass;
            col = gameObject.AddComponent<CapsuleCollider>(); //collider creation
            col.height = collHeight;
            col.radius = collSize;
            col.material = collMaterial;
            rb.freezeRotation = true;
        }
    }
    public void Jump()
    {
        if (grounded)
        {
            rb.AddForce(Vector3.up * baseJumpSpeed, ForceMode.Impulse); //if were grounded, jump
            OnJump.Invoke();
        }
    }
    public void EnterWater() //called when we enter the water
    {
        rb.freezeRotation = false;
        inWater = true;
        //rb.useGravity = false;
        rb.drag = 3;
        rb.angularDrag = 3f;
    }
    public void ExitWater() //called when we exit the water
    {
        rb.freezeRotation = true;
        inWater = false;
        //rb.useGravity = false;
        rb.drag = 0;
        rb.angularDrag = 0.05f;
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
    }
    public void Look(Vector2 dir) //used by AI/LocalController, makes the capsule look around
    {
        transform.Rotate(new Vector3(0, dir.x, 0), Space.Self);
        if(inWater)
        {
            transform.Rotate(new Vector3(-dir.y, 0, 0), Space.Self);
        }
    }
    private void FixedUpdate()
    {
        //print(GetRelativeSpeed());
        bool wasgrounded = grounded;
        grounded = Physics.Raycast(transform.position, Vector3.down, collHeight * 0.5f + 0.1f, walkableLayers); //if we are on the ground, we are grounded
        if (!wasgrounded && grounded)
        { 
            OnLand.Invoke(); //if we land, invoke the onland event
        }
        if (GetRelativeSpeed() > maxHorizontalSpeed)
        {
            DoSlowdown(); //slowdown if we are going faster than max move speed
        }
        else
        {
            rb.AddRelativeForce(moveVector * baseMoveSpeed); //if we arent, move according to the movevector
        }
        if(inWater)
        {
            rb.AddRelativeTorque(Vector3.forward * -sideVector * baseMoveSpeed * 0.08f); //if in water, do the water movement
            //rb.AddForce(phy)
        }
            
        if (moveVector.x == 0) DoSlowdown(false);
        if (moveVector.z == 0) DoSlowdown(true); //slowdown depending on the input
        // }
    }
    public void DoSlowdown() //global slowdown. use this when no keys are getting pressed
    {
        Vector3 vel = rb.velocity;
        vel.x *= slowdown;
        vel.z *= slowdown;
        rb.velocity = vel;
    }
    public void DoSlowdown(bool side) //slowdown for 1 specific side
    {
        Vector3 localVelocity = rb.transform.InverseTransformDirection(rb.velocity); //math
        if (!side) //dont ask
        {
            localVelocity.x *= slowdown;
            rb.velocity = rb.transform.TransformDirection(localVelocity);
        }
        else
        {
            localVelocity.z *= slowdown;
            rb.velocity = rb.transform.TransformDirection(localVelocity);
        }
    }
    public float GetRelativeSpeed() //use this instead of rb.velocity.magnitude. it factors in the speed of the surface we are standing on. 
    {
        return Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.y * rb.velocity.y + rb.velocity.z * rb.velocity.z); //factor in the speed of whatever were standing on later
    }
}
