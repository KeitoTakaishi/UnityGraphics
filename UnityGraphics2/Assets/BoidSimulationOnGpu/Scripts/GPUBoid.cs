﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class GPUBoid : MonoBehaviour {
    [SerializeField]
    struct BoidData
    {
        public Vector3 Velocity;
        public Vector3 Position;
    }
    const int SIMULATION_BLOCK_SIZE = 256;
    [Range(256, 32768)]
    public int MaxObjectNum = 16384;

    public float CohesionNeighborhoodRadius = 2.0f;
    public float AlignmentNeighborhoodRadius = 2.0f;
    public float SeparateNeighborhoodRadius = 1.0f;

    public float MaxSpeed = 5.0f;
    public float MaxSteerForce = 0.5f;

    public float CohesionWeight = 1.0f;
    public float AlignmentWeight = 1.0f;
    public float SeparateWeight = 3.0f;
    public float AvoidWallWeight = 10.0f;

    public Vector3 WallCenter = Vector3.zero;
    public Vector3 WallSize = new Vector3(32.0f, 32.0f, 32.0f);

    public ComputeShader BoidCS;
    ComputeBuffer _boidForceBuffer;
    ComputeBuffer _boidDataBuffer;


    public ComputeBuffer GetBoidDataBuffer()
    {
        return this._boidForceBuffer != null ? this._boidDataBuffer : null;
    }

    public int GetMaxObjectNum()
    {
        return this.MaxObjectNum;
    }

    public Vector3 GetSimulationAreaCenter()
    {
        return this.WallCenter;
    }

    public Vector3 GetSimulationAreaSize()
    {
        return this.WallSize;
    }


    void Start()
    {
        // バッファを初期化
        InitBuffer();
    }

    void Update()
    {
        // シミュレーション
        Simulation();
    }

    void OnDestroy()
    {
        // バッファを破棄
        ReleaseBuffer();
    }

    void OnDrawGizmos()
    {
        // デバッグとしてシミュレーション領域をワイヤーフレームで描画
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(WallCenter, WallSize);
    }

    void InitBuffer()
    {
        _boidDataBuffer = new ComputeBuffer(MaxObjectNum, Marshal.SizeOf(typeof(BoidData)));
        _boidForceBuffer = new ComputeBuffer(MaxObjectNum, Marshal.SizeOf(typeof(Vector3)));

        var forceArr = new Vector3[MaxObjectNum];
        var boidDataArr = new BoidData[MaxObjectNum];

        for(var i = 0; i < MaxObjectNum; i++)
        {
            forceArr[i] = Vector3.zero;
            boidDataArr[i].Position = Random.insideUnitSphere * 1.0f;
            boidDataArr[i].Velocity = Random.insideUnitSphere * 1.0f;

            _boidDataBuffer.SetData(boidDataArr);
            _boidForceBuffer.SetData(forceArr);
            boidDataArr = null;
            forceArr = null;
            
        }

    }

    void Simulation()
    {
        ComputeShader cs = BoidCS;
        int id = -1;
        int threadGroupSize = Mathf.CeilToInt(MaxObjectNum / SIMULATION_BLOCK_SIZE);


        id = cs.FindKernel("ForceCS"); // カーネルIDを取得
        cs.SetInt("_MaxBoidObjectNum", MaxObjectNum);
        cs.SetFloat("_CohesionNeighborhoodRadius", CohesionNeighborhoodRadius);
        cs.SetFloat("_AlignmentNeighborhoodRadius", AlignmentNeighborhoodRadius);
        cs.SetFloat("_SeparateNeighborhoodRadius", SeparateNeighborhoodRadius);
        cs.SetFloat("_MaxSpeed", MaxSpeed);
        cs.SetFloat("_MaxSteerForce", MaxSteerForce);
        cs.SetFloat("_SeparateWeight", SeparateWeight);
        cs.SetFloat("_CohesionWeight", CohesionWeight);
        cs.SetFloat("_AlignmentWeight", AlignmentWeight);
        cs.SetVector("_WallCenter", WallCenter);
        cs.SetVector("_WallSize", WallSize);
        cs.SetFloat("_AvoidWallWeight", AvoidWallWeight);
        cs.SetBuffer(id, "_BoidDataBufferRead", _boidDataBuffer);
        cs.SetBuffer(id, "_BoidForceBufferWrite", _boidForceBuffer);
        cs.Dispatch(id, threadGroupSize, 1, 1); // ComputeShaderを実行

        // 操舵力から、速度と位置を計算
        id = cs.FindKernel("IntegrateCS"); // カーネルIDを取得
        cs.SetFloat("_DeltaTime", Time.deltaTime);
        cs.SetBuffer(id, "_BoidForceBufferRead", _boidForceBuffer);
        cs.SetBuffer(id, "_BoidDataBufferWrite", _boidDataBuffer);
        cs.Dispatch(id, threadGroupSize, 1, 1); // ComputeShaderを実行
    }

    void ReleaseBuffer()
    {
        if (_boidDataBuffer != null)
        {
            _boidDataBuffer.Release();
            _boidDataBuffer = null;
        }

        if (_boidForceBuffer != null)
        {
            _boidForceBuffer.Release();
            _boidForceBuffer = null;
        }
    }


}
