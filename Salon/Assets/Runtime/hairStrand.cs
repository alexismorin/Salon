using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hairStrand : MonoBehaviour {

    public void init (Material strandMaterial) {
        GetComponent<TrailRenderer> ().material = strandMaterial;
    }
}