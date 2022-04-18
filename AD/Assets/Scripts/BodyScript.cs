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
    private void Start()
    {
        if(doSetup && !didSetup)
        {
            didSetup = true; //make sure we dont do the setup twice
            if(bodyMesh != null) //if we set the bodmyesh, then we do shit with it
            {
                MeshRenderer mesh = gameObject.AddComponent<MeshRenderer>();
                MeshFilter filter= gameObject.AddComponent<MeshFilter>();
                filter.mesh = bodyMesh;
                mesh.material = bodyMaterial;
            }
            rb = gameObject.AddComponent<Rigidbody>(); //rb creation
            rb.mass = rbMass;
            col = gameObject.AddComponent<CapsuleCollider>(); //collider creation
            col.height= collHeight;
            col.radius = collSize;
            col.material = collMaterial;
            rb.freezeRotation = true;
        }
    }
    public void Jump()
    {
        if(grounded)
        {
            rb.AddForce(Vector3.up * baseJumpSpeed, ForceMode.Impulse);
            OnJump.Invoke();
        }
    }
    private void FixedUpdate()
    {
        bool wasgrounded = grounded;
        grounded = Physics.Raycast(transform.position, Vector3.down, collHeight * 0.5f + 0.1f, walkableLayers); //if we are on the ground, we are grounded
        if(!wasgrounded && grounded)
        {
            OnLand.Invoke();
        }
        // if(grounded)
        // {
            //we do this for the 3 seperate directions, as unity is bullshit
            if(GetRelativeSpeed() > maxHorizontalSpeed)
            {
                DoSlowdown();
            }
            else
            rb.AddRelativeForce(moveVector * baseMoveSpeed);
            if(moveVector.x == 0) DoSlowdown(false);
            if(moveVector.y == 0) DoSlowdown(true); //slowdown depending on the input
        // }
    }
    public void DoSlowdown() //global slowdown. use this when no keys are getting pressed
    {
        Vector3 vel = rb.velocity;
        vel.x *= slowdown;
        vel.y *= slowdown;
        rb.velocity = vel;
    }
    public void DoSlowdown(bool side) 
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
        return rb.velocity.magnitude; //factor in the speed of whatever were standing on later
    }
}
