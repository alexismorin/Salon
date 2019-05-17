using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class salon2D : MonoBehaviour {

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
    public GameObject leftHand;
    public Material outputMaterial;
    public Camera exportCamera;
    public GameObject planarProjectionPrefab;

    // internals
    List<TrailRenderer> hairStrands = new List<TrailRenderer> ();
    List<Mesh> strandMeshes = new List<Mesh> ();
    MeshFilter exportTarget;
    Material[] currentHairType;
    GameObject currentStrand;
    GameObject currentStrandPlanarProjection;

    float triggerPress;
    float grabPress;
    bool lastTriggering;
    bool triggering;
    bool lastUndo;
    bool undo;

    float triggerPressRight;
    float grabPressRight;
    bool lastTriggeringRight;
    bool triggeringRight;
    bool lastUndoRight;
    bool undoRight;

    void Start () {

        exportTarget = GameObject.Find ("exportTarget").GetComponent<MeshFilter> ();
        ChangeHairType (shortHair);
    }

    void Undo () {
        if (hairStrands.Count > 0) {
            Destroy (hairStrands[hairStrands.Count - 1].gameObject);
            hairStrands.RemoveAt (hairStrands.Count - 1);
        }
    }

    void Update () {
        if (Input.GetKey ("escape")) {
            Application.Quit ();
        }

        if (Input.GetKeyDown (KeyCode.Space)) {
            ExportOBJ ();
        }

        if (Input.GetKeyDown (KeyCode.Backspace)) {
            Undo ();
        }

        if (Input.GetMouseButtonDown (0)) {
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast (ray, out hit)) {
                currentStrandPlanarProjection = GameObject.Instantiate (planarProjectionPrefab, hit.point, Quaternion.identity);
                currentStrandPlanarProjection.transform.LookAt (Camera.main.gameObject.transform);
                leftHand.transform.position = hit.point;
                StartCreatingStrand (leftHand);
            }

        }

        if (Input.GetMouseButton (0)) {
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast (ray, out hit)) {
                leftHand.transform.position = hit.point;
            }
        }

        if (Input.GetMouseButtonUp (0)) {
            StopCreatingStrand ();
            Destroy (currentStrandPlanarProjection);
        }

    }
    // Export hairdo to OBJ.
    public void ExportOBJ () {
        hairStrands.RemoveAll (x => x == null);
        strandMeshes = new List<Mesh> ();
        BakeHairdo ();
    }

    // callback to initialize the hair creation process
    void StartCreatingStrand (GameObject initiatingHand) {
        currentStrand = Instantiate (strandPrefab, initiatingHand.transform.position, initiatingHand.transform.rotation);
        currentStrand.transform.parent = initiatingHand.transform;
        Material randomStrandMaterial = currentHairType[Random.Range (0, currentHairType.Length)];
        currentStrand.GetComponent<hairStrand> ().init (randomStrandMaterial);
    }

    // callback once we release the trigger to create a strand and add it to the strand list
    void StopCreatingStrand () {
        if (currentStrand != null) {
            if (currentStrand.transform.parent != null) {
                currentStrand.transform.parent = null;
            }

            currentStrand.GetComponent<TrailRenderer> ().emitting = false;
            hairStrands.Add (currentStrand.GetComponent<TrailRenderer> ());
            currentStrand = null;
        }
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
            exportCamera.gameObject.transform.LookAt (hairStrands[i].transform);
            hairStrands[i].BakeMesh (bakedStrand, exportCamera, true);
            strandMeshes.Add (bakedStrand);
        }

        CombineInstance[] combine = new CombineInstance[strandMeshes.Count];
        for (int i = 0; i < combine.Length; i++) {
            combine[i].mesh = strandMeshes[i];
        }

        Mesh[] strandsArray = strandMeshes.ToArray ();

        exportedHairdo.CombineMeshes (combine, true, false);
        exportTarget.mesh = exportedHairdo;

        string timeText = System.DateTime.Now.ToString ("hh.mm.ss");
        MeshToFile (exportTarget, "hairdoExport " + timeText + ".obj");
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