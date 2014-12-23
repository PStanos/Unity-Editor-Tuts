using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeMaker : MonoBehaviour
{
    private Vector3 dimensions = Vector3.one;

    void OnDrawGizmos()
    {
        // Set matrix to use correct rotation
        Gizmos.matrix = this.transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, dimensions);
    }

    void OnDrawGizmosSelected()
    {
    }

    public void Create()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = dimensions;
        cube.transform.position = this.transform.position;
        cube.transform.rotation = this.transform.rotation;
    }

    public void ResetSize()
    {
        dimensions = Vector3.one;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

#if UNITY_EDITOR
    public void DrawResizeHandles()
    {
        //UnityEditor.Handles.matrix = this.transform.localToWorldMatrix;

        float deltaX = 0.0f, deltaY = 0.0f, deltaZ = 0.0f;
        deltaX += (UnityEditor.Handles.ScaleSlider(dimensions.x, this.transform.position + this.transform.localToWorldMatrix.MultiplyVector(new Vector3(dimensions.x * 0.5f, 0.0f, 0.0f)), this.transform.right, this.transform.rotation, 1.0f, UnityEditor.HandleUtility.GetHandleSize(this.transform.position)) - dimensions.x);
        deltaX += (UnityEditor.Handles.ScaleSlider(dimensions.x, this.transform.position + this.transform.localToWorldMatrix.MultiplyVector(new Vector3(-dimensions.x * 0.5f, 0.0f, 0.0f)), -this.transform.right, this.transform.rotation, 1.0f, UnityEditor.HandleUtility.GetHandleSize(this.transform.position)) - dimensions.x);

        deltaY += (UnityEditor.Handles.ScaleSlider(dimensions.y, this.transform.position + this.transform.localToWorldMatrix.MultiplyVector(new Vector3(0.0f, dimensions.y * 0.5f, 0.0f)), this.transform.up, this.transform.rotation, 1.0f, UnityEditor.HandleUtility.GetHandleSize(this.transform.position)) - dimensions.y);
        deltaY += (UnityEditor.Handles.ScaleSlider(dimensions.y, this.transform.position + this.transform.localToWorldMatrix.MultiplyVector(new Vector3(0.0f, -dimensions.y * 0.5f, 0.0f)), -this.transform.up, this.transform.rotation, 1.0f, UnityEditor.HandleUtility.GetHandleSize(this.transform.position)) - dimensions.y);

        deltaZ += (UnityEditor.Handles.ScaleSlider(dimensions.z, this.transform.position + this.transform.localToWorldMatrix.MultiplyVector(new Vector3(0.0f, 0.0f, dimensions.z * 0.5f)), this.transform.forward, this.transform.rotation, 1.0f, UnityEditor.HandleUtility.GetHandleSize(this.transform.position)) - dimensions.z);
        deltaZ += (UnityEditor.Handles.ScaleSlider(dimensions.z, this.transform.position + this.transform.localToWorldMatrix.MultiplyVector(new Vector3(0.0f, 0.0f, -dimensions.z * 0.5f)), -this.transform.forward, this.transform.rotation, 1.0f, UnityEditor.HandleUtility.GetHandleSize(this.transform.position)) - dimensions.z);

        dimensions += new Vector3(deltaX, deltaY, deltaZ);
    }
#endif
}
