using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLines : MonoBehaviour {

    public StarLineDataAssets starLineDataAssets;

    static Material lineMaterial;

    bool isShowLine = false;

    public GameObject controllerObject;

    private void Start() {


    }

    private void Update() {
        if (OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.Space)) {
            if (isShowLine == false) {
                isShowLine = true;
            } else {
                isShowLine = false;
            }
        }
    }

    static void CreateLineMaterial() {
        if (!lineMaterial) {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    private void OnRenderObject() {
        if (isShowLine) {
            // Materialの初期化と設定、詳しくは↑のUnityのScriptReferenceを参照。
            CreateLineMaterial();

            lineMaterial.SetPass(0);

            // GL.PushMatrix()～GL.PopMatrix()の間に行われた、行列マトリクスの変更が外に漏れないようにPush&Pop。おまじない、おまじない
            GL.PushMatrix();

            foreach (StarLineDataAssets.StarLineData lineData in starLineDataAssets.starLineDataList) {
                if (getStarPos(lineData.hDegFirst, lineData.sDegFirst).x == 0f) {
                    break;
                }

                GL.Begin(GL.LINE_STRIP);
                GL.Color(new Color(1.0f, 1.0f, 1.0f, 0.1f));
                Vector3 rot = controllerObject.transform.rotation.eulerAngles;
                GL.Vertex(getStarPos(lineData.hDegFirst + rot.y, lineData.sDegFirst));
                GL.Color(new Color(1.0f, 1.0f, 1.0f, 0.1f));
                GL.Vertex(getStarPos(lineData.hDegSecond + rot.y, lineData.sDegSecond));
                GL.End();
            }

            GL.PopMatrix();
        }
    }

    Vector3 getStarPos(float hDeg, float sDeg) {
        Quaternion rotL = Quaternion.AngleAxis(hDeg, Vector3.up);
        Quaternion rotS = Quaternion.AngleAxis(sDeg, Vector3.right);

        Vector3 pos = rotL * rotS * Vector3.forward * 10;

        return pos;
    }
}
