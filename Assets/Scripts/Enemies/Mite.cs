using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mite : Enemy
{
    private Rigidbody2D mite;
    private float visionDistance = 0.085f;
    private float moveSpeed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        mite = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 playerWorldPosition = Camera.main.ScreenToWorldPoint(playerPosition);

        
        Vector3 mitePosition = Camera.main.ScreenToWorldPoint(mite.transform.position);

        Debug.Log(Vector3.Distance(mitePosition, playerWorldPosition) + "\nVision: " + visionDistance);
        if(Vector2.Distance(mitePosition, playerWorldPosition) < visionDistance)
        {
            Debug.Log("move");
            Vector2 direction = (playerWorldPosition - mitePosition).normalized;

            if(direction.x > 0)
                mite.velocity = new Vector2(1 * moveSpeed, 0);
            else
                mite.velocity = new Vector2(-1 * moveSpeed, 0);

        }
        UpdateEnemy();
    }
}
