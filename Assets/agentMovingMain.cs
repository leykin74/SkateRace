using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class agentMovingMain : MonoBehaviour
{
    public List<Transform> target;
    public float distance;
    public float kickPower;
    public float hitAgentSpeed;

    NavMeshAgent agent;
    Animator anim;
    public float speedCorrection;
    public float fastAgentSpeed;

    public float speedIncrease;
    public float speedDecrease;
    public float maxSpeed;
    public float minSpeed;
    bool isFaster = false;
    public string EnemiesTag;
    public GameObject startingText;
    public GameObject pushingText;
    public GameObject imageRun;
    public GameObject imageStop;
    bool firstTap = true;
    //private NavMesh myMesh;
    public ParticleSystem FinParticle;
    public GameObject SpeedParticle;
    public int Place;
    float lastSpeed = 0f;
    public g_manager gameManager;
    Rigidbody rb;
    public float jumpPower;
    bool isJumping = false;
    public float horizontalSpeedMultiply;
    public float tramplinMinY;
    public Cinemachine.CinemachineVirtualCamera vcam;
    public GameObject clickToContinueBtn;
    Vector3 lastPosition;
    public float moveOutSpeedDevider;
    int currentTarget = -1;
    public float minJumpPower;
    float lastAngle = 0f;
    public float minAngle = 0.2f;
    public float angleSpeedDevider = 2f;
    public float outDistance = 0.1f;
    int isRotating = 0;
    public int maxRotating = 3;
    bool isRotatingLeft = false;
    bool isRotatingRight = false;
    public bool isFinished = false;
    public float returnOffset;
    bool canBeOut = true;
    public float canBeOutDelay;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>(); 
        setNewDistanation();
        // agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        // agent.CalculatePath(target[0].position, agent.path);
        anim = gameObject.GetComponentInChildren<Animator>();
        // agent.angularSpeed = 500;
        rb = GetComponent<Rigidbody>();
    }
 
    // Update is called once per frame
    void Update () {
        
        // Debug.Log(agent.obstacleAvoidanceType);

        // NavMeshHit hit;
        // if (!agent.Raycast(target.position, out hit, 0.1f))
        // {
        //     // Target is "visible" from our position.
        // }
        if (Input.GetMouseButtonDown(0)) {
            if (firstTap) {
                firstTap = false;
                startingText.SetActive(false);
                pushingText.SetActive(true);
                setImageRun(true);
            }
        }

        if (!isFaster) {
            if (Input.GetMouseButton(0) && agent.speed < maxSpeed) {
                agent.speed += speedIncrease;
                anim.SetBool("isPushing", true);
                setImageRun(true);
                // SpeedParticle.Play();
            } else if (agent.speed > minSpeed && !Input.GetMouseButton(0)) {
                agent.speed -= speedDecrease;
                anim.SetBool("isPushing", false);
                setImageRun(false);
                // SpeedParticle.Stop();
            }else if (agent.speed < minSpeed) {
                agent.speed = minSpeed;
                anim.SetBool("isPushing", false);
                setImageRun(false);
                // SpeedParticle.Stop();
            }
        }else{
            // SpeedParticle.Play();
            anim.SetBool("isPushing", true);
        }
        if (agent.speed > lastSpeed) {
            SpeedParticle.SetActive(true);
        } else {
            SpeedParticle.SetActive(false);
        }

        float newSpeed = agent.speed / speedCorrection;
        anim.speed = newSpeed > 1 ? newSpeed : 1f;

        

        // Debug.Log(transform.rotation.eulerAngles.y - lastAngle);
        if (agent.enabled)
            if (isRotatingLeft) {
                agent.Move(- transform.right * agent.speed / angleSpeedDevider * Time.deltaTime);
                agent.FindClosestEdge(out NavMeshHit hit);
                if (hit.distance < outDistance && maxSpeed - agent.speed < 0.1f && isRotating > maxRotating && canBeOut) {
                    moveAgentOut();
                }
                isRotating++;
            } else if (isRotatingRight) {
                agent.Move(transform.right * agent.speed / angleSpeedDevider * Time.deltaTime);
                agent.FindClosestEdge(out NavMeshHit hit);
                if (hit.distance < outDistance && maxSpeed - agent.speed < 0.1f && isRotating > maxRotating && canBeOut) {
                    moveAgentOut();
                }
                isRotating++;
            } else {
                isRotating = 0;
            }

        
        
        // Debug.Log(agent.destination);
        if (Vector3.Distance(transform.position, agent.destination) <= distance)
        {
            // Debug.Log("kkkkkkkkk target.Count " + target.Count + " currentTarget " + currentTarget);
            // if (target.Count > currentTarget)
            // {
            //     setNewDistanation();
            //     if (maxSpeed - agent.speed < 0.1f) {
            //         moveAgentOut();
            //     }
            // }
            // else
            // {
                agent.speed = 0;
                anim.SetBool("isPushing", false);
                gameManager.GameOver();
            // }
        }
        if (isJumping && transform.position.y <= tramplinMinY) {
            // rb.AddForce(transform.up * gravity, ForceMode.Force);
            Debug.Log("TramplinEnd");
            isJumping = false;
            agent.enabled = true;
            rb.isKinematic = true;
            setNewDistanation(false);
        }

        lastSpeed = agent.speed;
        lastAngle = transform.rotation.eulerAngles.y;
        // Debug.Log("y " + transform.position.y);
    }
    private void moveAgentOut() {
        canBeOut = false;
        Debug.Log("moveAgentOut");
        agent.FindClosestEdge(out NavMeshHit hit);
        Vector2 direction = new Vector2(agent.path.corners[0].x - transform.position.x, agent.path.corners[0].z - transform.position.z) * returnOffset;
        Debug.Log(direction);
        // lastPosition = new Vector3(transform.position.x + direction.x,transform.position.y, transform.position.z + direction.y);
        lastPosition = transform.position;
        agent.enabled = false;
        rb.isKinematic = false;
        rb.useGravity = true;
        float dir = isRotatingLeft ? -1 : 1;
        rb.AddRelativeForce(new Vector3(dir * agent.speed / moveOutSpeedDevider, jumpPower, agent.speed / moveOutSpeedDevider), ForceMode.Impulse);
        anim.SetBool("isHit", true);
        vcam.enabled = false;
        clickToContinueBtn.SetActive(true);
        pushingText.SetActive(false);
    }
    IEnumerator setOutEnableWIthDelay()
    {
        Debug.Log("setOutEnableWIthDelay");
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(canBeOutDelay);

        //After we have waited 5 seconds print the time again.
        canBeOut = true;
        Debug.Log("setOutEnableWIthDelay 222");
    }
    public void clickToContinue() {
        anim.SetBool("isHit", false);
        clickToContinueBtn.SetActive(false);
        pushingText.SetActive(true);
        transform.position = lastPosition;
        vcam.enabled = true;
        agent.enabled = true;
        rb.isKinematic = true;
        setNewDistanation();
        StartCoroutine(setOutEnableWIthDelay());
    }

    private void setImageRun(bool run) {
        imageRun.SetActive(run);
        imageStop.SetActive(!run);
    }

    private void setNewDistanation(bool nextTarget = true) {
        // if (target.Count > 0) {
            // if (nextTarget) currentTarget++;
            // agent.SetDestination(target[currentTarget].position);
            agent.SetDestination(target[0].position);
            // if (target.Count > 0 && removeTarget) target.RemoveAt(0);
        // } 
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == EnemiesTag) {
            // other.gameObject.GetComponent<NavMeshAgent>().enabled = false;
            // other.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            // other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * kickPower);
            other.gameObject.GetComponentInChildren<Animator>().SetBool("isHit", true);
            other.gameObject.GetComponent<NavMeshAgent>().speed = hitAgentSpeed;
        }
        if (other.tag == "faster" && Input.GetMouseButton(0)) {
            agent.speed += fastAgentSpeed;
            isFaster = true;
        }
        if (other.tag == "Finish" && Place == 0) {
            FinParticle.Play();
        }
        if(other.tag == "Out" && agent.speed >= maxSpeed) {
            gameObject.GetComponent<NavMeshAgent>().enabled = false;
        }
        // if (other.gameObject.tag == "Path" && isJumping) {
        //     Debug.Log("Path collided" + other.gameObject.name);
        //     isJumping = false;
        //     agent.enabled = true;
        //     rb.isKinematic = true;
        //     agent.SetDestination(target[0].position);
        // }
        if (other.tag == "LeftOutZone") {
            isRotatingLeft = true;
        }
        if (other.tag == "RightOutZone") isRotatingRight = true;
        if (other.tag == "Ground") {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.tag == "faster") {
            isFaster = false;
        }
        if (other.tag == "Finish") {
            pushingText.SetActive(false);
            isFinished = true;
        }
        if (other.tag == "Tramplin" && !isJumping && Input.GetMouseButton(0)) {
            Debug.Log("Tramplin");
            agent.enabled = false;
            rb.isKinematic = false;
            rb.useGravity = true;
            float jumpspeed = agent.speed * horizontalSpeedMultiply;
            if (jumpspeed < minJumpPower) jumpspeed = minJumpPower;
            rb.AddRelativeForce(new Vector3(0f, jumpPower, jumpspeed), ForceMode.Impulse);
            isJumping = true;
        }
        if (other.tag == "LeftOutZone") isRotatingLeft = false;
        if (other.tag == "RightOutZone") isRotatingRight = false;
    }
}
