using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
    #region InspectorFields

    public Texture2D image;
    [SerializeField] private int _blocksPerLine = 4;

    private Block _emptyBlock;

    #endregion
    void Start()
    {
        CreatePuzzle();
        Camera cam = Camera.main;
        float width = 2f * cam.orthographicSize * cam.aspect;
        cam.orthographicSize = _blocksPerLine / (2 * cam.aspect);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Methods

    private void CreatePuzzle()
    {
        Texture2D[,] imageSlicers = ImageSlicer.GetSlicers(image, _blocksPerLine);
        for (int y = 0; y < _blocksPerLine; y++)
        {
            for (int x = 0; x < _blocksPerLine; x++)
            {
                var blockObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                blockObject.transform.position = -Vector2.one * (_blocksPerLine - 1) * 0.5f + new Vector2(x, y);
                blockObject.transform.parent = transform;
                var block = blockObject.AddComponent<Block>();
                block.OnBlockPresed += PlayerMoveBlockInput;
                block.Initialization(new Vector2Int(x,y), imageSlicers[x,y]);

                if (y == 0 && x ==_blocksPerLine - 1)
                {
                    blockObject.SetActive(false);
                    _emptyBlock = block;
                }
            }
        }
    }

    private void PlayerMoveBlockInput(Block blockTomove)
    {
        if ((blockTomove.coordinate - _emptyBlock.coordinate).sqrMagnitude == 1)
        {
            var targetCoordinate = _emptyBlock.coordinate;
            _emptyBlock.coordinate = blockTomove.coordinate;
            blockTomove.coordinate = targetCoordinate;
            var targetPosition = _emptyBlock.transform.position;
            _emptyBlock.transform.position = blockTomove.transform.position;
            blockTomove.transform.position = targetPosition;
        }
    }

    #endregion
}
