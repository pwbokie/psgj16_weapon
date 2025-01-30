using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;

public class Beamer : Enemy
{
    private Rigidbody2D beamer;
    public GameObject beam;
    private GameObject beamObject;
    private float visionDistance = 0.085f;
    private float beamRange = 0.065f;
    private float moveSpeed = 1.5f;
    private bool isFiring = false;
    public Sprite baseBeamer;
    private float distanceToPlayer;
    public Sprite spriter;
    private float targetAngle;
    private Vector2 direction;
    // Start is called before the first frame update
    void Start()
    {
        beamer = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 playerWorldPosition = Camera.main.ScreenToWorldPoint(playerPosition);

        
        Vector3 beamerPosition = Camera.main.ScreenToWorldPoint(beamer.transform.position);
        distanceToPlayer = Vector2.Distance(beamerPosition, playerWorldPosition);

        if(distanceToPlayer < visionDistance && !isFiring)
        {
            direction = (playerWorldPosition - beamerPosition).normalized;

            targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if(Math.Abs(direction.x) < 0.1f && Math.Abs(direction.y) < 1f)
                beamer.velocity = Vector2.zero;
            else if(direction.x > 0)
                beamer.velocity = new Vector2(1 * moveSpeed, 0);
            else
                beamer.velocity = new Vector2(-1 * moveSpeed, 0);

            if(distanceToPlayer < beamRange)
            {
                StartCoroutine(FireBeam());
            }
        }

        UpdateEnemy();


    }

    IEnumerator FireBeam()
    {
        isFiring = true;
        beamer.velocity = Vector2.zero;

        gameObject.GetComponent<SpriteRenderer>().sprite = spriter;
        
        yield return new WaitForSeconds(2f);
        float beamDistance = (distanceToPlayer > beamRange ? beamRange : distanceToPlayer) * 312.5f;
        RaycastHit2D hit = Physics2D.Raycast(beamer.position, direction, beamDistance, LayerMask.GetMask("Foreground"));
        Debug.Log(hit.distance);
        beam.GetComponent<SpriteRenderer>().size = new Vector2(Mathf.Ceil(beamDistance + hit.distance), 4);
        beamObject = Instantiate(beam, new Vector3(beamer.position.x, beamer.position.y + 0.2f), Quaternion.Euler(new Vector3(0, 0, targetAngle)), beamer.transform);

        
        if(hit.collider != null && hit.collider.gameObject.tag == "Player")
        {
            DealDamage(hit.collider.gameObject);
        }

        yield return new WaitForSeconds(1f);

        Destroy(beamObject);

        yield return new WaitForSeconds(2f);

        gameObject.GetComponent<SpriteRenderer>().sprite = baseBeamer;

        isFiring = false;
    }
}
