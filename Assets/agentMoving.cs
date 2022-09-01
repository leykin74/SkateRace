using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class agentMoving : MonoBehaviour
{
    public List<Transform> target;
    public float distance;
    public float maxSpeed;

    NavMeshAgent agent;
    Animator anim;
    public float speedCorrection;
    public GameObject[] numbers;
    public float fastAgentSpeed;
    Rigidbody rb;
    public float jumpPower;
    bool isJumping = false;
    public float horizontalSpeedMultiply;
    public float tramplinMinY;
    bool isRotatingLeft = false;
    bool isRotatingRight = false;
    public float angleSpeedDevider = 2f;
    public bool isFinished = false;
    //private NavMesh myMesh;
    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        setNewDistanation();
        // agent.CalculatePath(target[0].position, agent.path);
        anim = gameObject.GetComponentInChildren<Animator>();
        // agent.angularSpeed = 500;
        rb = GetComponent<Rigidbody>();
    }
 
    // Update is called once per frame
    void Update () {
        
        if (Input.GetMouseButtonDown(0)) {
            agent.speed = maxSpeed;
            anim.SetBool("isPushing", true);
        }
        float newSpeed = agent.speed / speedCorrection;
        anim.speed = newSpeed > 1 ? newSpeed : 1f;
        if (Vector3.Distance(transform.position, agent.destination) < distance)
        {
            if (target.Count > 0)
            {
                setNewDistanation();
            }
        }
        if (isJumping && transform.position.y <= tramplinMinY) {
            // rb.AddForce(transform.up * gravity, ForceMode.Force);
            isJumping = false;
            agent.enabled = true;
            rb.isKinematic = true;
            setNewDistanation(true);
        }
        if (agent.enabled) {
            if (isRotatingLeft) {
                agent.Move(- transform.right * agent.speed / angleSpeedDevider * Time.deltaTime);
            } else if (isRotatingRight) {
                agent.Move(transform.right * agent.speed / angleSpeedDevider * Time.deltaTime);
            }
        }
    }

    private void setNewDistanation(bool removeTarget = false) {
        if (target.Count > 0) {
            agent.SetDestination(target[0].position);
            // if (target.Count > 1 || removeTarget) target.RemoveAt(0);
        } 
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "faster") {
            agent.speed += fastAgentSpeed;
        }
        if (other.tag == "LeftOutZone") {
            isRotatingLeft = true;
        }
        if (other.tag == "RightOutZone") isRotatingRight = true;
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "faster") {
            agent.speed = maxSpeed;
        }
        if (other.gameObject.tag == "Tramplin" && !isJumping) {
            agent.enabled = false;
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddRelativeForce(new Vector3(0f, jumpPower, agent.speed * horizontalSpeedMultiply), ForceMode.Impulse);
            isJumping = true;
        }
        if (other.tag == "LeftOutZone") isRotatingLeft = false;
        if (other.tag == "RightOutZone") isRotatingRight = false;
        if (other.tag == "Finish") {
            isFinished = true;
        }
    }
}
