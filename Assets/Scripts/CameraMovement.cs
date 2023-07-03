using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float speed = 0.008f;
    private float zoomSpeed = 10f;
    private float panSpeed = 0.003f;
    private float maxHeight = 400f;
    private float minHeight = 2f;
    private float heightScalar;
    private Vector3 p1;
    private Vector3 p2;

    private void Update()
    {
        heightScalar = transform.position.y / 2f; // Used to adjust speed of movement based on current camera height (faster for higher).

        CameraWASDMovement();
        CameraPanMovement();
    }
    private void CameraWASDMovement()
    {
        Vector3 move = new Vector3();

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetAxis("Vertical") != 0)
        { // Do forward movement logic.
            float verticalSpeed = heightScalar * speed * Input.GetAxis("Vertical"); // W & S key will activate this GetAxis.
            Vector3 forwardMove = transform.forward;

            forwardMove.y = 0; // If we dont remove y component, the camera will zoom at what its looking at
            forwardMove.Normalize(); // Normalize to keep it the same speed as horizontal movement
            forwardMove *= verticalSpeed;
            move += forwardMove;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetAxis("Horizontal") != 0)
        { // Do lateral movement logic.
            float horizontalSpeed = heightScalar * speed * Input.GetAxis("Horizontal"); // A & D key will activate this GetAxis.
            Vector3 lateralMove = horizontalSpeed * transform.right;

            move += lateralMove;
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0) // Do height movement logic
        { 
            // Log function used to slow down scroll speed as it reaches the top.
            float heightSpeed = Mathf.Log(transform.position.y) * -zoomSpeed * Input.GetAxis("Mouse ScrollWheel"); 

            // Bound our camera so we cannot go above or below height thresholds. 
            if ((transform.position.y + heightSpeed) > maxHeight) 
                heightSpeed = maxHeight - transform.position.y;
            else if ((transform.position.y + heightSpeed) < minHeight)
                heightSpeed = minHeight - transform.position.y;

            move += new Vector3(0, heightSpeed, 0);
        }

        if (move != Vector3.zero)
            transform.position += move;
    }
    private void CameraPanMovement()
    {
        if (Input.GetMouseButtonDown(2)) // Check if the middle mouse button has been pressed
            p1 = Input.mousePosition;

        if (Input.GetMouseButton(2)) // Check is the middle mouse button is being held down
        {
            p2 = Input.mousePosition;

            float dx = heightScalar * (p2 - p1).x * panSpeed;
            float dz = heightScalar * (p2 - p1).y * panSpeed; // No idea why we need y axis

            Vector3 move = new Vector3(-dx, 0, 0);
            move += new Vector3(0, 0, -dz);

            transform.position += move;
            p1 = p2;
        }
    }
}
