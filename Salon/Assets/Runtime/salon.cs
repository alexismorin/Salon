using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class salon : MonoBehaviour {

    // hair card materials, selected from random
    [Header ("Material Settings")]
    [SerializeField]
    Material[] shortHair;
    [SerializeField]
    Material[] longHair;
    [SerializeField]
    Material[] kinkyHair;
    [SerializeField]
    Material[] curlyHair;
    [SerializeField]
    Material[] flyawayHair;
    [Space (10)]

    [Header ("General Settings")]
    public GameObject strandPrefab;
    public GameObject playerHand;
    public bool leftHanded = true;
    public Material outputMaterial;

    // internals
    List<TrailRenderer> hairStrands = new List<TrailRenderer> ();
    List<Mesh> strandMeshes = new List<Mesh> ();
    MeshFilter exportTarget;
    Material[] currentHairType;

    GameObject currentStrand;
    float triggerPress;
    bool lastTriggering;
    bool triggering;

    void Start () {
        VRSafetyCheck ();
        exportTarget = GameObject.Find ("exportTarget").GetComponent<MeshFilter> ();
        ChangeHairType (shortHair);
    }

    void VRSafetyCheck () {
        XRDevice.SetTrackingSpaceType (TrackingSpaceType.RoomScale);
    }

    void Update () {
        lastTriggering = triggering;

        List<XRNodeState> xrNodes = new List<XRNodeState> ();
        InputTracking.GetNodeStates (xrNodes);

        for (int i = 0; i < xrNodes.Count; i++) {

            if (leftHanded) {
                if (xrNodes[i].nodeType == XRNode.LeftHand) {
                    Vector3 newHandPosition;
                    xrNodes[i].TryGetPosition (out newHandPosition);
                    playerHand.transform.localPosition = newHandPosition;

                    Quaternion newHandRotation;
                    xrNodes[i].TryGetRotation (out newHandRotation);
                    playerHand.transform.localRotation = newHandRotation;

                    InputDevices.GetDeviceAtXRNode (xrNodes[i].nodeType).TryGetFeatureValue (CommonUsages.trigger, out triggerPress);

                    if (triggerPress > 0.6f) {
                        triggering = true;
                    } else {
                        triggering = false;
                    }

                }

            } else {

            }

        }

        if (triggering == true && lastTriggering == false) {
            StartCreatingStrand ();
        }
        if (lastTriggering == true && triggering == false) {
            StopCreatingStrand ();
        }

    }
    // Export hairdo to OBJ.
    public void ExportOBJ () {
        BakeHairdo ();
    }

    // callback to initialize the hair creation process
    void StartCreatingStrand () {
        currentStrand = Instantiate (strandPrefab, playerHand.transform.position, playerHand.transform.rotation);
        currentStrand.transform.parent = playerHand.transform;
        Material randomStrandMaterial = currentHairType[Random.Range (0, currentHairType.Length)];
        currentStrand.GetComponent<hairStrand> ().init (randomStrandMaterial);
    }

    // callback once we release the trigger to create a strand and add it to the strand list
    void StopCreatingStrand () {
        currentStrand.transform.parent = null;
        hairStrands.Add (currentStrand.GetComponent<TrailRenderer> ());
        currentStrand = null;
    }

    // change hair type
    void ChangeHairType (Material[] newHairType) {
        currentHairType = newHairType;
    }

    // Bake strands into meshes and sends the geometry into the exporter
    void BakeHairdo () {

        Mesh exportedHairdo = new Mesh ();

        for (int i = 0; i < hairStrands.Count; i++) {
            Mesh bakedStrand = new Mesh ();
            hairStrands[i].BakeMesh (bakedStrand, true);
            strandMeshes.Add (bakedStrand);
        }

        CombineInstance[] combine = new CombineInstance[strandMeshes.Count];
        for (int i = 0; i < combine.Length; i++) {
            combine[i].mesh = strandMeshes[i];
        }

        Mesh[] strandsArray = strandMeshes.ToArray ();

        exportedHairdo.CombineMeshes (combine, true, false);
        exportTarget.mesh = exportedHairdo;

        MeshToFile (exportTarget, "hairdoExport.obj");
    }

    // courtesy of KeliHlodversson @ https://wiki.unity3d.com/index.php/ObjExporter
    static string MeshToString (MeshFilter mf) {
        Mesh m = mf.mesh;
        Material[] mats = mf.GetComponent<Renderer> ().materials;

        StringBuilder sb = new StringBuilder ();

        sb.Append ("g ").Append (mf.name).Append ("\n");
        foreach (Vector3 v in m.vertices) {
            sb.Append (string.Format ("v {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append ("\n");
        foreach (Vector3 v in m.normals) {
            sb.Append (string.Format ("vn {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append ("\n");
        foreach (Vector3 v in m.uv) {
            sb.Append (string.Format ("vt {0} {1}\n", v.x, v.y));
        }
        for (int material = 0; material < m.subMeshCount; material++) {
            sb.Append ("\n");
            sb.Append ("usemtl ").Append ("hairMaterial").Append ("\n");
            sb.Append ("usemap ").Append ("hairMaterial").Append ("\n");

            int[] triangles = m.GetTriangles (material);
            for (int i = 0; i < triangles.Length; i += 3) {
                sb.Append (string.Format ("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                    triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
            }
        }
        return sb.ToString ();
    }

    // courtesy of KeliHlodversson @ https://wiki.unity3d.com/index.php/ObjExporter
    static void MeshToFile (MeshFilter mf, string filename) {
        using (StreamWriter sw = new StreamWriter (filename)) {
            sw.Write (MeshToString (mf));
        }
    }
}