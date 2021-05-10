using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;
    float speed = 0.0f;
    public float walkSpeed = 8.0f;
    public float runSpeed = 12.0f;
    public float acceleration = 0.1f;
    public float deceleration = 0.5f;
    public float gravity = -9.81f;
    public float mass = 10f;
    public float jumpHeight = 3f;
    public float crouchDeltaHeight = 1f;
    public float startHeight;

    public float turnSmoothTime = 0.1f;

    public Transform groundCheck;
    public Transform mesh;
    public Transform cam;

    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public Material[] auraMaterialsGround;
    public Material[] auraMaterialsSpark;

    public GameObject auraGround;
    public GameObject auraSpark;

    int VelocityHash;
    Vector3 velocity;
    Vector3 direction;
    Vector3 moveDir;
    bool isOnGround;
    bool isCrouching;
    bool finishedTransition;
    float oldStepOffset;
    float turnSmoothVelocity;
    float dist;

    System.Random rand;

    IEnumerator ChangeLayerWeightSmooth(int layerIndex, float v_start, float v_end, float duration)
    {
        finishedTransition = false;
        float elapsed = 0.0f;
        float weight;
        while (elapsed < duration)
        {
            weight = Mathf.Lerp(v_start, v_end, elapsed / duration);
            animator.SetLayerWeight(layerIndex, weight);
            elapsed += Time.deltaTime;
            yield return null;
        }
        weight = v_end;
        animator.SetLayerWeight(layerIndex, weight);
        finishedTransition = true;
    }

    bool isFloatEqual(float a, float b, float epsilon)
    {
        return Mathf.Abs(a - b) <= (Mathf.Max(Mathf.Abs(a), Mathf.Abs(b)) * epsilon);
    }

    public void Awake()
    {
        oldStepOffset = controller.stepOffset;
        isCrouching = false;
        moveDir = new Vector3();
        rand = new System.Random();
        if (auraMaterialsGround.Length != 0 && auraMaterialsSpark.Length != 0)
        {
            int index = rand.Next(0, 3);
            auraGround.GetComponent<ParticleSystemRenderer>().material = auraMaterialsGround[index];
            auraSpark.GetComponent<ParticleSystemRenderer>().material = auraMaterialsSpark[index];
        }
    }

    private void Start()
    {
        VelocityHash = Animator.StringToHash("Velocity");
        startHeight = controller.height;
        dist = startHeight / 2;
    }

    void Update()
    {
        float newHeight = startHeight;
        isOnGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isOnGround && velocity.y < 0)
        {
            controller.stepOffset = oldStepOffset;
            velocity.y = -2f;
        } else
        {
            controller.stepOffset = 0f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, 
                                                    targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            if (speed < walkSpeed && !isFloatEqual(speed, walkSpeed, (float)1e-3))
            {
                speed += Time.deltaTime * acceleration;
            }

            if (Input.GetButton("Change Speeds") && speed < runSpeed && !isFloatEqual(speed, runSpeed, (float)1e-3))
            {
                speed += Time.deltaTime * acceleration;
            }

            if ( (!Input.GetButton("Change Speeds")) && speed > walkSpeed)
            {
                speed -= Time.deltaTime * deceleration;
            }

        } else if (speed > 0.0f)
        {
            speed -= Time.deltaTime * deceleration;
        } else if (speed < 0.0f)
        {
            speed = 0.0f;
        }

        animator.SetFloat(VelocityHash, speed);
        controller.Move(moveDir.normalized * speed * Time.deltaTime);


        if (Input.GetButtonDown("Crouch") && !isCrouching && isOnGround)
        {
            isCrouching = true;
            groundCheck.Translate(0, crouchDeltaHeight / 2, 0, Space.World);
            StartCoroutine(ChangeLayerWeightSmooth(1, 0.0f, 1.0f, 0.4f));

        }
        else if (Input.GetButtonUp("Crouch") && isCrouching)
        {
            isCrouching = false;
            groundCheck.Translate(0, -crouchDeltaHeight / 2, 0, Space.World);
            StartCoroutine(ChangeLayerWeightSmooth(1, 1.0f, 0.0f, 0.4f));
        }

        if (isCrouching)
        {
            newHeight = 0.5f * startHeight;
        }

        float lastHeight = controller.height;

        controller.height = Mathf.Lerp(controller.height, newHeight, 5.0f * Time.deltaTime);

        transform.position = new Vector3(transform.position.x,
            transform.position.y + ((controller.height - lastHeight) * 0.5f),
            transform.position.z);
        /*mesh.transform.position = new Vector3(mesh.transform.position.x,
            mesh.transform.position.y - ((controller.height - lastHeight) * 0.5f),
            mesh.transform.position.z);*/


        if (Input.GetButtonDown("Jump") && isOnGround && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -mass * gravity);
        }

        velocity.y += mass * gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

    }
}
