using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private Rigidbody2D turret;
    private float rotationDamping = 2f;
    private float torqueForce = 1f;

    private float maxRotationSpeed = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        turret = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(playerPosition);

        Vector3 turretPosition = Camera.main.ScreenToWorldPoint(turret.transform.position);

        Vector2 direction = (mouseWorldPosition - turretPosition).normalized;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float currentAngle = turret.rotation;

        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);
        if(Mathf.Abs(angleDifference) > 1f)
        {
            float torque = angleDifference * torqueForce;

            //Debug.Log(torque * rotationDamping);
            if(torque * rotationDamping > maxRotationSpeed)
                turret.AddTorque(maxRotationSpeed);
            else
                turret.AddTorque(torque * rotationDamping);
        }
        else
        {
            turret.angularVelocity = 0f;
        }
        
    }
}
