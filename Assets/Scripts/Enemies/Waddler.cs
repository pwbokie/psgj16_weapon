using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waddler : Enemy
{
    private Rigidbody2D waddler;
    private float visionDistance = 0.085f;
    private float moveSpeed = 4f;
    private float wanderSpeed = 1.5f;

    private bool isWandering = false;
    private bool isChasing = false;




    private Transform target;           // Target position to move toward
    public float maxSpeed = 5f;        // Maximum movement speed
    public float acceleration = 2f;    // Speed increase per second
    public float decelerationTime = 0.5f; // Time to slow down
    private float currentSpeed = 0f;   // Current movement speed
    private bool isSlowingDown = false;
    private Vector2 targetPosition;    // The last known position of the player
    private bool hasTarget = false; 
    // Start is called before the first frame update
    void Start()
    {
        waddler = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 playerWorldPosition = Camera.main.ScreenToWorldPoint(playerPosition);
        target = GameObject.FindGameObjectWithTag("Player").transform;
        
        if (!hasTarget && target != null)
        {
            targetPosition = target.position; // Lock onto the player's position at the moment of detection
            hasTarget = true;
        }

        Vector3 waddlerPosition = Camera.main.ScreenToWorldPoint(waddler.transform.position);
        if(hasTarget)
        {
            float distance = Vector2.Distance(waddlerPosition, playerWorldPosition);
            Debug.Log(distance);
            if (distance < 0.8f && !isSlowingDown)
            {
                // Accelerate toward the target
                currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
                MoveTowardsTarget();
            }
            else if (!isSlowingDown)
            {
                // Start slowing down when close to the target
                StartCoroutine(SlowDown());
            }
        }
        

        UpdateEnemy();
    }

      private void MoveTowardsTarget()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        transform.position += (Vector3)(direction * currentSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DealDamage(collision.gameObject);
        }
    }

    private IEnumerator SlowDown()
    {
        isSlowingDown = true;
        float startSpeed = currentSpeed;
        float timer = 0f;

        while (timer < decelerationTime)
        {
            timer += Time.deltaTime;
            currentSpeed = Mathf.Lerp(startSpeed, 0, timer / decelerationTime);
            MoveTowardsTarget();
            yield return null;
        }

        currentSpeed = 0f;
        yield return new WaitForSeconds(3f);
        gameObject.transform.eulerAngles = Vector3.zero;
        isSlowingDown = false;
        hasTarget = false; // Reset for next detection
    }

}
