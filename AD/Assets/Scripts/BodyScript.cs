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
    [Tooltip("The name of the creature. Right now, used in doing the limb setup")]
    public string CreatureName;
    public bool dead = false;
    public bool isRagdoll = false;
    public float stunTime; //if above 0, you cant unragdoll. goes down over time


    public bool inWater, faceInWater; //determines if we are in or out of water
    [HideInInspector]
    public UnityEvent OnJump = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnLand = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnDamage = new UnityEvent();
    //here come all the hidden variables
    float jumpcool; //makes sure you cant jump 20 times a second. cooldown
    CapsuleCollider col; //the main collider
    Rigidbody rb; //the main rigidbody
    [HideInInspector]
    public bool grounded; //determines if the entity sees itself as grounded
    [HideInInspector]
    public Vector3 moveVector; //determines where the entity wants to move, in local space. eg. if Z is 1, the entity will move forward. if X is -1, the entity will move left. if Y is 1, the entity will try to swim upwards.
    public float sideVector; //when in water, rotate left and right
    public List<Limb> limbs = new List<Limb>();
    public List<AirAreaManager> nodes = new List<AirAreaManager>();
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
        if (CreatureName == "Human")
        {
            limbs.Add(new Limb("Head", "head"));
            limbs.Add(new Limb("Torso", "torso"));
            limbs.Add(new Limb("Right Arm", "rarm"));
            limbs.Add(new Limb("Left Arm", "larm"));
            limbs.Add(new Limb("Right Leg", "rleg"));
            limbs.Add(new Limb("Left Leg", "lreg"));
        }
        nodes.AddRange(FindObjectsOfType<AirAreaManager>());
    }
    public void Damaged()
    {
        float vit = GetVitality();
        if (vit <= 100f && !isRagdoll)
        {
            //do ragdoll
            EnterRagdoll();
        }
        if (vit <= 0f)
        {
            Death();
        }
        OnDamage.Invoke();
    }
    public void EnterRagdoll()
    {
        if (!isRagdoll)
        {
            isRagdoll = true;
            rb.freezeRotation = false;
        }
    }
    public void ExitRagdoll()
    {
        if (isRagdoll && stunTime < 0f && !dead && GetVitality() > 100f)
        {
            isRagdoll = false;
            rb.freezeRotation = true;
        }
    }
    public void Death()
    {
        if (!dead)
        {
            dead = true;
            if (!isRagdoll)
            {
                //do ragdoll
                EnterRagdoll();
            }
        }

    }
    public void Jump()
    {
        if (grounded && !isRagdoll)
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
        if (!isRagdoll)
            rb.freezeRotation = true;
        inWater = false;
        //rb.useGravity = false;
        rb.drag = 0;
        rb.angularDrag = 0.05f;
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
    }
    public void Look(Vector2 dir) //used by AI/LocalController, makes the capsule look around
    {
        if (!isRagdoll)
        {
            transform.Rotate(new Vector3(0, dir.x, 0), Space.Self);
            if (inWater)
            {
                transform.Rotate(new Vector3(-dir.y, 0, 0), Space.Self);
            }
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
        if (GetRelativeSpeed() > maxHorizontalSpeed && !isRagdoll)
        {
            DoSlowdown(); //slowdown if we are going faster than max move speed
        }
        else if (!isRagdoll)
        {
            rb.AddRelativeForce(moveVector * baseMoveSpeed); //if we arent, move according to the movevector
        }
        if (inWater && !isRagdoll)
        {
            rb.AddRelativeTorque(Vector3.forward * -sideVector * baseMoveSpeed * 0.08f); //if in water, do the water movement
            //rb.AddForce(phy)
        }

        if (moveVector.x == 0) DoSlowdown(false);
        if (moveVector.z == 0) DoSlowdown(true); //slowdown depending on the input
        // }
        bool inair = false;
        bool indeepwater = true;
        foreach (AirAreaManager air in nodes)
        {
            if (air.col.bounds.Contains(transform.position))
            {
                indeepwater = false;
                if (transform.position.y > air.WaterLevelY + air.transform.position.y)
                {
                    inair = true;
                }
                break;
            }
        }
        if (!inair && !inWater) EnterWater();
        if (inair && inWater) { ExitWater(); }
        if (indeepwater) GiveAffliction("barotrauma", Time.fixedDeltaTime * 10f);
        else if (HasAffliction("barotrauma", 0f)) GiveAffliction("barotrauma", -Time.fixedDeltaTime * 20f);
        if (!inair) GiveAffliction("hypoxemia", Time.fixedDeltaTime * 1.8f);
        else if (HasAffliction("hypoxemia", 0f)) GiveAffliction("hypoxemia", -Time.fixedDeltaTime * 10f);
        stunTime -= Time.fixedDeltaTime;
        if (isRagdoll)
        {
            ExitRagdoll();
        }
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
    public float GetVitality() //calculates the vitality, based on all limb afflictions
    {
        float f = 200f;
        foreach (Limb limb in limbs)
        {
            foreach (Affliction aff in limb.afflictions)
            {
                if (aff.prefab.scaleVitality)
                {
                    f -= Mathf.Lerp(0f, 200f, aff.strength / aff.prefab.maxStrength);
                }
                else
                {
                    f -= Mathf.Lerp(aff.prefab.vitalityReduction.x, aff.prefab.vitalityReduction.y, aff.strength / aff.prefab.maxStrength);

                }
            }
        }
        return f;
    }
    public bool HasAffliction(string identifier, float minStrength)
    {
        foreach (Limb limb in limbs)
        {
            foreach (Affliction aff in limb.afflictions)
            {
                if (aff.prefab.identifier == identifier && aff.strength >= minStrength) return true;
            }
        }
        return false;
    }
    public bool HasAfflictionLimb(string identifier, string limbid, float minStrength)
    {
        foreach (Affliction aff in GetLimb(limbid).afflictions)
        {
            if (aff.prefab.identifier == identifier && aff.strength >= minStrength) return true;
        }
        return false;
    }
    public void GiveAffliction(string identifier, float strength)
    {
        if (!HasAfflictionLimb(identifier, limbs[0].identifier, 0f))
        {
            limbs[0].afflictions.Add(new Affliction(strength, AfflictionDatabase.GetAffliction(identifier)));
        }
        else
        {
            foreach (Affliction aff in limbs[0].afflictions)
            {
                if (aff.prefab.identifier == identifier) aff.strength += strength;
                if (aff.strength > aff.prefab.maxStrength) aff.strength = aff.prefab.maxStrength;
                if (aff.strength <= 0f) { limbs[0].afflictions.Remove(aff); break; }
            }
        }
        Damaged();
    }
    public List<Affliction> GetAllAfflictions()
    {
        List<Affliction> list = new List<Affliction>();
        foreach (Limb limb in limbs)
        {
            foreach (Affliction aff in limb.afflictions)
            {
                list.Add(aff);
            }
        }
        return list;
    }
    public Limb GetLimb(string identifier)
    {
        foreach (Limb limb in limbs)
        {
            if (limb.identifier == identifier) return limb;
        }
        return null;
    }
}
public class Limb
{
    public string name { get; private set; }
    public string identifier;
    public List<Affliction> afflictions = new List<Affliction>();

    public Limb(string nm, string id)
    {
        name = nm;
        identifier = id;
    }
}
public class Affliction
{
    public float strength;
    public AfflictionPrefab prefab;
    public Affliction(float str, AfflictionPrefab pref)
    {
        strength = str;
        prefab = pref;
    }
}
