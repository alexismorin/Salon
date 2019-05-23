using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hairStrand : MonoBehaviour {

    GameObject lookTarget;
    bool looking = true;
    Vector3 initPos;
    // Start is called before the first frame update
    void Start () {
        initPos = transform.position;
        lookTarget = new GameObject ();
    }

    // Update is called once per frame
    void Update () {
        if (looking == true) {
            Vector3 newPos = new Vector3 (0f, transform.position.y, Mathf.Clamp (transform.position.z, -0.146f, 0.062f));
            lookTarget.transform.position = newPos;
            transform.LookAt (lookTarget.transform);
        }
    }

    public void init (Material strandMaterial) {
        GetComponent<TrailRenderer> ().material = strandMaterial;
    }

    public void end () {
        looking = false;
        Destroy (lookTarget);
    }
}