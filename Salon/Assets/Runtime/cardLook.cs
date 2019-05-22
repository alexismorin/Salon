using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardLook : MonoBehaviour {

    Transform lookTarget;
    // Start is called before the first frame update
    void Start () {
        lookTarget = GameObject.Find ("cardLookRoot").transform;
    }

    // Update is called once per frame
    void Update () {
        transform.LookAt (lookTarget);
    }
}