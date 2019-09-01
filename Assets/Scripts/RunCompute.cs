using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunCompute : MonoBehaviour {


    // struct
    struct Particle {
        public Vector3 position;
        public Vector3 velocity;
        public float life;
    }

    /// <summary>
	/// Size in octet of the Particle struct.
    /// since float = 4 bytes...
    /// 4 floats = 16 bytes
	/// </summary>
	//private const int SIZE_PARTICLE = 24;
    private const int SIZE_PARTICLE = 28; // since property "life" is added...

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

    void Start() {

        InitComputeShader();

    }

    void InitComputeShader() {

        particleCount = starDataAssets.starDataList.Count;

        mWarpCount = Mathf.CeilToInt((float)particleCount / WARP_SIZE);

        // initialize the particles
        Particle[] particleArray = new Particle[starDataAssets.starDataList.Count];

        for (int i = 0; i < particleCount; i++) {

            StarDataAssets.StarData starData = starDataAssets.starDataList[i];

            float hDeg = (360f / 24f) * (starData.ra.x + starData.ra.y / 60f + starData.ra.z / 3600f);

            float signNumber = -1f;

            if (starData.dec.x < 0) {
                starData.dec.x *= -1;
                signNumber = 1f;
            }

            float sDeg = (starData.dec.x + starData.dec.y / 60f + starData.dec.z / 3600f) * signNumber;

            Quaternion rotL = Quaternion.AngleAxis(hDeg, Vector3.up);
            Quaternion rotS = Quaternion.AngleAxis(sDeg, Vector3.right);

            Vector3 pos = rotL * rotS * Vector3.forward * 100;

            particleArray[i].position = pos;

            particleArray[i].velocity = Vector3.zero;

            // Initial life value
            particleArray[i].life = 100;
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


    }

}