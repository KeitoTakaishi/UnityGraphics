using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ImageEffectBase_01 : MonoBehaviour {

    #region Field
    public Material mat;
    #endregion Field

    #region Method


    void Start () {
        
		if(!SystemInfo.supportsImageEffects || !this.mat || !this.mat.shader.isSupported)
        {
            Debug.Log("faild");
            base.enabled = false;
        }
        
	}


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, this.mat);
    }

    #endregion Method
}
