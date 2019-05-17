using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disableXR : MonoBehaviour {
    // Start is called before the first frame update
    void Start () {
        UnityEngine.XR.XRSettings.enabled = false;
    }

    // Update is called once per frame
    void Update () {

    }
}