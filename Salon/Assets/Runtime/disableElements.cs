using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disableElements : MonoBehaviour {

    public GameObject[] elementsToDisable;
    public GameObject[] elementsToEnable;
    public float delay;

    // Start is called before the first frame update
    void Start () {
        Invoke ("disable", delay);
    }

    // Update is called once per frame
    void disable () {
        for (int i = 0; i < elementsToDisable.Length; i++) {
            elementsToDisable[i].SetActive (false);
        }
        for (int i = 0; i < elementsToEnable.Length; i++) {
            elementsToEnable[i].SetActive (true);
        }
    }
}