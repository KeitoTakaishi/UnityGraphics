using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GPUBoid))]
public class BoidsRender : MonoBehaviour {
    public Vector3 ObjectScale = new Vector3(.1f, .2f, .5f);
    public GPUBoid GPUBoidsScript;
    public Mesh InstanceMesh;
    public Material InstanceRenderMaterial;
    uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    ComputeBuffer argsBuffer;

    void Start()
    {
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

    }

	void Update () {
        RenderInstanceMesh();
	}

    void RenderInstanceMesh()
    {
        if (InstanceRenderMaterial == null || GPUBoidsScript == null)
        {
            return;
        }

        uint numIndices = (InstanceMesh != null) ?
            (uint)InstanceMesh.GetIndexCount(0) : 0;
        args[0] = numIndices;
        args[1] = (uint)GPUBoidsScript.GetMaxObjectNum();
        argsBuffer.SetData(args);

        InstanceRenderMaterial.SetBuffer("_BoidDataBuffer", GPUBoidsScript.GetBoidDataBuffer());
        InstanceRenderMaterial.SetVector("_ObjectScale", ObjectScale);

        var bounds = new Bounds
        (
            GPUBoidsScript.GetSimulationAreaCenter(),
            GPUBoidsScript.GetSimulationAreaSize()
        );

        Graphics.DrawMeshInstancedIndirect
            (InstanceMesh, 0, InstanceRenderMaterial, bounds, argsBuffer);
    }
}
