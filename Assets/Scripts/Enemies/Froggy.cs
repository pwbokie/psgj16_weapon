using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Froggy : Enemy
{
    private Rigidbody2D froggy;
    private float distanceToPlayer;
    private float visionDistance = 0.095f;
    private float moveSpeed = 1f;
    private bool isLeaping = false;
    private float maxVelocity = 4f;
    private float gravityScale;
    private Vector2 direction;
    
    // Start is called before the first frame update
    void Start()
    {
        froggy = GetComponent<Rigidbody2D>();
        gravityScale = froggy.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 playerWorldPosition = Camera.main.ScreenToWorldPoint(playerPosition);

        
        Vector3 froggyPosition = Camera.main.ScreenToWorldPoint(froggy.transform.position);

        distanceToPlayer = Vector2.Distance(froggyPosition, playerWorldPosition);

        if(distanceToPlayer < visionDistance && !isLeaping)
        {
            direction = (playerWorldPosition - froggyPosition).normalized;

            StartCoroutine(Leap(playerWorldPosition, 2f));
        }

        UpdateEnemy();
    }

    IEnumerator Leap(Vector2 targetPosition, float leapHeight)
    {
        isLeaping = true;
        // Calculate the distance between the current position and the target
        Vector2 startPosition = froggy.position;
        float distanceX = startPosition.x - targetPosition.x;
        float distanceY = targetPosition.y - startPosition.y;


          // Calculate the total time for the leap
        float timeToApex = Mathf.Sqrt(-2 * leapHeight / (Physics2D.gravity.y * gravityScale));
        float timeToDescend = Mathf.Sqrt(2 * (distanceY - leapHeight) / (Physics2D.gravity.y * gravityScale));
        float totalTime = timeToApex + timeToDescend;

        // Calculate the required horizontal and vertical velocities
        float velocityX = distanceX / totalTime;
        float velocityY = Mathf.Sqrt(-2 * Physics2D.gravity.y * gravityScale * leapHeight);

        // Apply the velocity to the Rigidbody2D
        Debug.Log(Mathf.Abs(velocityX * -2));
        if ( direction.x > 0)
            froggy.velocity = new Vector2((velocityX * 2) < Mathf.Abs(maxVelocity) ? velocityX * 2 : maxVelocity, velocityY);
        else
            froggy.velocity = new Vector2(Mathf.Abs(velocityX * -2) < Mathf.Abs(maxVelocity) ? velocityX * -2 : -maxVelocity, velocityY);

        froggy.angularVelocity = 360f;
        yield return new WaitForSeconds(5f);
        froggy.transform.eulerAngles = Vector3.zero;
        yield return new WaitForSeconds(2f);
        isLeaping = false;
    }
}
