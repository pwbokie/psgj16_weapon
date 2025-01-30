using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachment_Hookshot : MonoBehaviour
{
    public GameObject chainSegmentPrefab; // Prefab for chain links
    public int chainLength = 20; // Number of segments
    public float extendSpeed = 30f; // Speed of extension
    public float retractForce = 20f; // Force pulling the player

    private GameObject[] chainSegments;
    private Rigidbody2D playerRb;
    private bool isGrappling = false;

    void Start()
    {
        playerRb = GameObject.Find("Gun").GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        if (Input.GetMouseButton(1) && playerRb.GetComponent<PlayerController>().currentAmmo > 0)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            playerRb.GetComponent<PlayerController>().currentAmmo--;
            playerRb.GetComponent<PlayerController>().UpdateAmmoCount();

            FireGrapple(mousePos);
        }
    }

    public void FireGrapple(Vector2 target)
    {
        if (isGrappling) return;
        isGrappling = true;

        StartCoroutine(ExtendChain(target));
    }

    private IEnumerator ExtendChain(Vector2 target)
    {
        Vector2 startPos = transform.position + (Vector3)(Vector2.right * 0.5f);
        Vector2 direction = (target - startPos).normalized;
        float segmentDistance = Vector2.Distance(startPos, target) / chainLength;

        chainSegments = new GameObject[chainLength];

        for (int i = 0; i < chainLength; i++)
        {
            GameObject segment = Instantiate(chainSegmentPrefab, startPos + direction * segmentDistance * i, Quaternion.identity);
            Rigidbody2D segmentRb = segment.GetComponent<Rigidbody2D>();

            if (i > 0)
            {
                HingeJoint2D joint = segment.AddComponent<HingeJoint2D>();
                joint.connectedBody = chainSegments[i - 1].GetComponent<Rigidbody2D>();
            }
            else
            {
                segment.GetComponent<HingeJoint2D>().connectedBody = GetComponent<Rigidbody2D>();
            }

            chainSegments[i] = segment;
            segmentRb.velocity = direction * extendSpeed;

            if (i == chainLength - 1)
            {
                segmentRb.transform.localScale = new Vector3(5f, 5f, 5f);   
            }

            yield return new WaitForSeconds(0.02f); // Small delay for smooth extension
        }

        yield return new WaitForSeconds(0.2f);
        StartCoroutine(RetractPlayer());
    }

    private IEnumerator RetractPlayer()
    {
        GetComponent<ParticleSystem>().Play();
        float elapsedTime = 0f;
        while (Vector2.Distance(playerRb.position, chainSegments[chainLength - 1].transform.position) > 1f && elapsedTime < 1f)
        {
            Vector2 pullDirection = (chainSegments[chainLength - 1].transform.position - (Vector3)playerRb.position).normalized;
            playerRb.velocity = pullDirection * retractForce;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        CleanupChain();
    }

    private void CleanupChain()
    {
        GetComponent<ParticleSystem>().Stop();

        foreach (var segment in chainSegments)
        {
            Destroy(segment);
        }

        isGrappling = false;
    }
}
