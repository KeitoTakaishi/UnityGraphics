using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleComputeShader_Texture : MonoBehaviour {

    public GameObject plane_A;

    public ComputeShader computeShader;

    RenderTexture renderTexture_A;

    int kernelIndex_KernelFunction_A;

    struct ThreadSize
    {
        public int x;
        public int y;
        public int z;

        public ThreadSize(uint x, uint y, uint z)
        {
            this.x = (int)x;
            this.y = (int)y;
            this.z = (int)z;
        }
    }

    ThreadSize kernelThreadSize_KrnelFunction_A;


    void Start () {
        this.renderTexture_A = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32);
        //textureへの書き込みを有効にする
        this.renderTexture_A.enableRandomWrite = true;
        renderTexture_A.Create();

        this.kernelIndex_KernelFunction_A = this.computeShader.FindKernel("KernelFunction_A");
        uint threadSizeX, threadSizeY, threadSizeZ;

        //threadSizeの取得
        this.computeShader.GetKernelThreadGroupSizes(this.kernelIndex_KernelFunction_A
            , out threadSizeX, out threadSizeY, out threadSizeZ);

        this.kernelThreadSize_KrnelFunction_A = new ThreadSize(threadSizeX, threadSizeY, threadSizeZ);

        this.computeShader.SetTexture(this.kernelIndex_KernelFunction_A, "textureBuffer", this.renderTexture_A);

    }
	
	void Update () {

        this.computeShader.SetFloat("time", Time.realtimeSinceStartup);
        this.computeShader.Dispatch(this.kernelIndex_KernelFunction_A,
                                    this.renderTexture_A.width / this.kernelThreadSize_KrnelFunction_A.x,
                                    this.renderTexture_A.height / this.kernelThreadSize_KrnelFunction_A.y,
                                    this.kernelThreadSize_KrnelFunction_A.z);

        plane_A.GetComponent<Renderer>().material.mainTexture = this.renderTexture_A;



    }
}
