using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunCompute : MonoBehaviour {


    // struct
    struct Particle {
        public Vector3 position;
        public float hDeg;
        public float sDeg;
        public Color color;
        public Color colorBuffer;
        public float size;
    }

    /// <summary>
	/// Size in octet of the Particle struct.
    /// since float = 4 bytes...
    /// 4 floats = 16 bytes
	/// </summary>
	//private const int SIZE_PARTICLE = 24;
    private const int SIZE_PARTICLE = 56; // since property "life" is added...

    /// <summary>
    /// Number of Particle created in the system.
    /// </summary>
    private int particleCount = 10000;

    /// <summary>
    /// Material used to draw the Particle on screen.
    /// </summary>
    public Material material;

    /// <summary>
    /// Compute shader used to update the Particles.
    /// </summary>
    public ComputeShader computeShader;

    /// <summary>
    /// Id of the kernel used.
    /// </summary>
    private int mComputeShaderKernelID;

    /// <summary>
    /// Buffer holding the Particles.
    /// </summary>
    ComputeBuffer particleBuffer;

    /// <summary>
    /// Number of particle per warp.
    /// </summary>
    private const int WARP_SIZE = 256; // TODO?

    /// <summary>
    /// Number of warp needed.
    /// </summary>
    private int mWarpCount; // TODO?

    //public ComputeShader shader;

    // Use this for initialization

    /// <summary>
    /// Scriptable object it's data of stars.
    /// </summary>
    public StarDataAssets starDataAssets;

    public float alpha = 0.7f;

    public GameObject controllerObject;

    void Start() {

        InitComputeShader();

    }

    Quaternion quaternion_mul(Quaternion f, Quaternion s) {
        Quaternion q = Quaternion.identity;
        q.x = (f.w * s.x) + (f.x * s.w) + (f.y * s.z) + (-f.z * s.y);
        q.y = (f.w * s.y) + (f.y * s.w) + (f.z * s.x) + (-f.x * s.z);
        q.z = (f.w * s.z) + (f.z * s.w) + (f.x * s.y) + (-f.y * s.x);
        q.w = (f.w * s.w) + (-f.x * s.x) + (-f.y * s.y) + (-f.z * s.z);
        return q;
    }

    Vector3 vector3_mul(Quaternion rotation, Vector3 point) {
        float num1 = rotation.x * 2f;
        float num2 = rotation.y * 2f;
        float num3 = rotation.z * 2f;
        float num4 = rotation.x * num1;
        float num5 = rotation.y * num2;
        float num6 = rotation.z * num3;
        float num7 = rotation.x * num2;
        float num8 = rotation.x * num3;
        float num9 = rotation.y * num3;
        float num10 = rotation.w * num1;
        float num11 = rotation.w * num2;
        float num12 = rotation.w * num3;
        Vector3 vector3;
        vector3.x = (float)((1.0 - ((double)num5 + (double)num6)) * (double)point.x + ((double)num7 - (double)num12) * (double)point.y + ((double)num8 + (double)num11) * (double)point.z);
        vector3.y = (float)(((double)num7 + (double)num12) * (double)point.x + (1.0 - ((double)num4 + (double)num6)) * (double)point.y + ((double)num9 - (double)num10) * (double)point.z);
        vector3.z = (float)(((double)num8 - (double)num11) * (double)point.x + ((double)num9 + (double)num10) * (double)point.y + (1.0 - ((double)num4 + (double)num5)) * (double)point.z);
        return vector3;
    }

    void InitComputeShader() {

        particleCount = starDataAssets.starDataList.Count;

        mWarpCount = Mathf.CeilToInt((float)particleCount / WARP_SIZE);

        // initialize the particles
        Particle[] particleArray = new Particle[starDataAssets.starDataList.Count];

        for (int i = 0; i < particleCount; i++) {

            StarDataAssets.StarData starData = starDataAssets.starDataList[i];

            float hDeg = (360f / 24f) * (starData.ra.x + starData.ra.y / 60f + starData.ra.z / 3600f);

            int hsSgn = 1;

            float decX = starData.dec.x;

            if (decX < 0) {
                decX *= -1;
                hsSgn = -1;
            }

            float sDeg = (decX + starData.dec.y / 60f + starData.dec.z / 3600f) * hsSgn;

            particleArray[i].hDeg = hDeg;

            particleArray[i].sDeg = sDeg;

            Color col;
            col = starData.spectTypeColor;
            col.a = 1.0f - 0.25f * starData.vMag;

            particleArray[i].color = col;
            particleArray[i].colorBuffer = col;

            particleArray[i].size = 1f - starData.vMag;
        }

        // create compute buffer
        particleBuffer = new ComputeBuffer(particleCount, SIZE_PARTICLE);

        particleBuffer.SetData(particleArray);

        // find the id of the kernel
        mComputeShaderKernelID = computeShader.FindKernel("CSParticle");

        // bind the compute buffer to the shader and the compute shader
        computeShader.SetBuffer(mComputeShaderKernelID, "particleBuffer", particleBuffer);
        material.SetBuffer("particleBuffer", particleBuffer);
    }

    void OnRenderObject() {
        material.SetPass(0);
        Graphics.DrawProcedural(MeshTopology.Points, 1, particleCount);
    }

    void OnDestroy() {
        if (particleBuffer != null)
            particleBuffer.Release();
    }

    // Update is called once per frame
    void Update() {
        controllerObject.transform.Rotate(0f, 2.5f * Time.deltaTime, 0f);
        Vector3 rot = controllerObject.transform.rotation.eulerAngles;
        float[] rots = new float[3] { rot.x, rot.y, rot.z };
        computeShader.SetFloats("rot", rots);
        computeShader.SetFloats("mulAlpha", alpha);
        computeShader.Dispatch(mComputeShaderKernelID, mWarpCount, 1, 1);

        if (OVRInput.GetDown(OVRInput.Button.Three) || Input.GetKeyDown(KeyCode.UpArrow)) {
            alpha += 0.1f;
            if (alpha >= 1.3f) alpha = 1.3f;
        } else if (OVRInput.GetDown(OVRInput.Button.Four) || Input.GetKeyDown(KeyCode.DownArrow)) {
            alpha -= 0.1f;
            if (alpha <= 0.0f) alpha = 0.0f;
        }
    }
}