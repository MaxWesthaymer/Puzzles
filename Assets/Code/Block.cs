using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public event System.Action<Block> OnBlockPresed;
    public event System.Action OnFinishedMoving;

    public Vector2Int coordinate;

    private Vector2Int _startingCoordinate;

    public void Initialization(Vector2Int startingCoordinate, Texture2D image)
    {
        this._startingCoordinate = startingCoordinate;
        coordinate = startingCoordinate;
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Block");
        GetComponent<MeshRenderer>().material.mainTexture = image;
    }
    
    private void OnMouseDown()
    {
        if (OnBlockPresed != null)
        {
            OnBlockPresed(this);
        }
    }

    public void MoveToPosition(Vector2 target, float duration)
    {
        StartCoroutine(AnimateMove(target, duration));
    }

    private IEnumerator AnimateMove(Vector2 target, float duration)
    {
        var initialPos = transform.position;
        var percent = 0f;
        while (percent < 1)
        {
            percent += Time.deltaTime / duration;
            transform.position = Vector2.Lerp(initialPos, target, percent);
            yield return null;
        }

        if (OnFinishedMoving != null)
        {
            OnFinishedMoving();
        }
    }

    public bool IsAtStartingCoord()
    {
        return coordinate == _startingCoordinate;
    }
}
