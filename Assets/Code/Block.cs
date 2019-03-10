using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public event System.Action<Block> OnBlockPresed;

    public Vector2Int coordinate;

    public void Initialization(Vector2Int startingCoordinate, Texture2D image)
    {
        coordinate = startingCoordinate;
        GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/Texture");
        GetComponent<MeshRenderer>().material.mainTexture = image;
    }
    
    private void OnMouseDown()
    {
        if (OnBlockPresed != null)
        {
            OnBlockPresed(this);
        }
    }
}
