using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControls2D : MonoBehaviour {

    [SerializeField]
    float rotationSpeed = 1f;
    [SerializeField]
    Vector2 normalizedMousePosition;
    Vector2 targetMousePosition;
    bool isRotating;

    // Update is called once per frame

    Vector2 findNormalizedMousePosition () {
        float newXPos = (1f / Screen.width) * Input.mousePosition.x;
        float newYPos = (1f / Screen.height) * Input.mousePosition.y;
        return new Vector2 (newXPos, newYPos);
    }

    void Update () {
        if (Input.GetMouseButtonDown (1)) {
            normalizedMousePosition = findNormalizedMousePosition ();
            isRotating = true;
        }
        if (Input.GetMouseButtonUp (1)) {

            isRotating = false;
        }
        if (Input.GetMouseButton (1)) {

            targetMousePosition = findNormalizedMousePosition ();
            Vector3 rotationDelta = targetMousePosition - normalizedMousePosition;
            Vector3 targetRotation = new Vector3 (rotationDelta.y * -rotationSpeed, rotationDelta.x * rotationSpeed, 0f);
            this.transform.Rotate (targetRotation);
            Vector3 clampedRotation = new Vector3 (this.transform.localEulerAngles.x, this.transform.localEulerAngles.y, 0f);
            this.transform.localEulerAngles = clampedRotation;

        }

    }
}