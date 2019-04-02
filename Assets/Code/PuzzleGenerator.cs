using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
    #region InspectorFields

    public Texture2D image;
    [SerializeField] private int _blocksPerLine = 4;
    [SerializeField] private int _shuffleLenth = 20;
    [SerializeField] private float _defaultMoveDuration = 0.2f;
    [SerializeField] private float _shuffleMoveDuration = 0.1f;

    private Block _emptyBlock;
    private Queue<Block> _inputs;
    private Block[,] _blocks;
    private bool _blockIsMoving;
    private int _shuffleMovesRemaining;
    private Vector2Int _previousShuffleOffset;
    
    enum PuzzleState{Solved, Shuffling, InPlay}

    private PuzzleState _state;
    #endregion
    void Start()
    {
       // CreatePuzzle();
        Camera cam = Camera.main;
        float width = 2f * cam.orthographicSize * cam.aspect;
        cam.orthographicSize = _blocksPerLine / (2 * cam.aspect);
    }

    private void Update()
    {
        if (_state == PuzzleState.Solved && Input.GetKeyDown(KeyCode.Space))
        {
            StartShuffle();
        }
    }

    #region Methods

    public void CreatePuzzle(Texture2D image)
    {
        Debug.Log("CreatePuzzle");
        _blocks = new Block[_blocksPerLine,_blocksPerLine];
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
                block.OnFinishedMoving += OnBlockFinishedMoving;
                block.Initialization(new Vector2Int(x,y), imageSlicers[x,y]);
                _blocks[x, y] = block;
                if (y == 0 && x ==_blocksPerLine - 1)
                {
                    _emptyBlock = block;
                }
            }
        }
        _inputs = new Queue<Block>();
    }

    private void PlayerMoveBlockInput(Block blockToMove)
    {
        if (_state == PuzzleState.InPlay)
        {
            _inputs.Enqueue(blockToMove);
            MakeNextPlayerMove();
        }
    }

    private void MakeNextPlayerMove()
    {
        while (_inputs.Count > 0 && !_blockIsMoving)
        {          
            MoveBlock(_inputs.Dequeue(), _defaultMoveDuration);
        }
    }

    private void MoveBlock(Block blockToMove, float duration)
    {
        if ((blockToMove.coordinate - _emptyBlock.coordinate).sqrMagnitude == 1)
        {

            _blocks[blockToMove.coordinate.x, blockToMove.coordinate.y] = _emptyBlock;
            _blocks[_emptyBlock.coordinate.x, _emptyBlock.coordinate.y] = blockToMove;
            
            var targetCoordinate = _emptyBlock.coordinate;
            _emptyBlock.coordinate = blockToMove.coordinate;
            blockToMove.coordinate = targetCoordinate;
            var targetPosition = _emptyBlock.transform.position;
            _emptyBlock.transform.position = blockToMove.transform.position;
            blockToMove.MoveToPosition(targetPosition, duration);
            _blockIsMoving = true;
        }
    }

    private void OnBlockFinishedMoving()
    {
        _blockIsMoving = false;
        CheckIfSolved();
        if (_state == PuzzleState.InPlay)
        {
            MakeNextPlayerMove();
        }
        else if(_state == PuzzleState.Shuffling)
        {
            if (_shuffleMovesRemaining > 0)
            {
                MakeNextShuffleMove();
            }
            else
            {
                _state = PuzzleState.InPlay;
            }
        }      
    }

    private void MakeNextShuffleMove()
    {
        Vector2Int[] offsets = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };
        int randomIndex = Random.Range(0, offsets.Length);

        for (int i = 0; i < offsets.Length; i++)
        {
            Vector2Int offset = offsets[(randomIndex + i) % offsets.Length];
            if (offset != _previousShuffleOffset * -1)
            {                                        
                Vector2Int moveBlockCoord = _emptyBlock.coordinate + offset;
    
                if (moveBlockCoord.x >= 0 && moveBlockCoord.x < _blocksPerLine &&
                    moveBlockCoord.y >= 0 && moveBlockCoord.y < _blocksPerLine)
                {
                    MoveBlock(_blocks[moveBlockCoord.x, moveBlockCoord.y], _shuffleMoveDuration);
                    _shuffleMovesRemaining--;
                    _previousShuffleOffset = offset;
                    break;
                }
            }
        }     
    }

    public void StartShuffle()
    {
        _state = PuzzleState.Shuffling;
        _shuffleMovesRemaining = _shuffleLenth;
        _emptyBlock.gameObject.SetActive(false);
        MakeNextShuffleMove();
    }

    private void CheckIfSolved()
    {
        foreach (var it in _blocks)
        {
            if (!it.IsAtStartingCoord())
            {
                return;
            }
        }

        _state = PuzzleState.Solved;
        _emptyBlock.gameObject.SetActive(true);
    }

    #endregion
}
