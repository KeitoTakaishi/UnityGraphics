using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleComputeShader_Array : MonoBehaviour {

    public ComputeShader computeShader;
    int kernelIndex_KernelFunc_A;
    int kernelIndex_KernelFunc_B;

    ComputeBuffer intComputeBuffer;

    void Start () {
        //store kernel Index 
        this.kernelIndex_KernelFunc_A = this.computeShader.FindKernel("KernelFunction_A");
        //this.kernelIndex_KernelFunc_B = this.computeShader.FindKernel("KernelFunction_B");

        //init ComputeBuffer
        this.intComputeBuffer = new ComputeBuffer(4, sizeof(int));
        this.computeShader.SetBuffer(this.kernelIndex_KernelFunc_A, "intBuffer", this.intComputeBuffer);

        this.computeShader.SetInt("intValue", 1);

        //execute computeShader
        this.computeShader.Dispatch(this.kernelIndex_KernelFunc_A, 1, 1, 1);

        int[] result = new int[4];

        this.intComputeBuffer.GetData(result);

        Debug.Log("Result + kernelFuncA");

        for(int i = 0; i < 4; i++)
        {
            Debug.Log(result[i]);
        }

        this.intComputeBuffer.Release();

    }
}
