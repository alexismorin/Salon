using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControls2D : MonoBehaviour {

    [SerializeField]
    float rotationSpeed = 1f;
    [SerializeField]
    Vector2 normalizedMousePosition;
    Vector2 normalizedMousePositionZoom;
    Vector2 targetMousePosition;
    Vector2 targetMousePositionZoom;
    bool isRotating;
    float currentZoom = -0.4f;
    bool isZooming;

    // Update is called once per frame

    Vector2 findNormalizedMousePosition () {
        float newXPos = (1.0f / Screen.width) * Input.mousePosition.x;
        float newYPos = (1.0f / Screen.height) * Input.mousePosition.y;
        return new Vector2 (newXPos, newYPos);
    }

    void Update () {

        Vector3 newZoomPosition = new Vector3 (0f, 0f, currentZoom);
        transform.GetChild (0).localPosition = newZoomPosition;

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

        if (Input.GetMouseButtonDown (2)) {
            normalizedMousePositionZoom = findNormalizedMousePosition ();
            isZooming = true;
        }
        if (Input.GetMouseButtonUp (2)) {

            isZooming = false;
        }
        if (Input.GetMouseButton (2)) {

            targetMousePositionZoom = findNormalizedMousePosition ();
            print (normalizedMousePositionZoom - targetMousePositionZoom);
            float zoomDelta = normalizedMousePositionZoom.y - targetMousePositionZoom.y;
            currentZoom += zoomDelta * 0.05f;
        }

    }
}