using System.Collections.Generic;
using UnityEngine;

public class FlowField : MonoBehaviour
{
    [SerializeField] private Node CurrentDestination;
    public enum Dir
    {
        Left, Right, Top, Bottom, TopRight, BottomRight, TopLeft, BottomLeft
    }

    public Dictionary<Dir, Vector2Int> lDirections = new Dictionary<Dir, Vector2Int>();
    public Dictionary<Node, Node[,]> lPaths = new Dictionary<Node, Node[,]>();

    public LayerMask WallLayer;

    [SerializeField] private float _CellSize = 1;

    [SerializeField] private GameObject _DebugSquare;

    private Node[][,] _Fields;
    private Node[] _Exits;

    public Bounds _Bounds;

    bool isInit = false;

    public void Init(Bounds pBounds)
    {
        InitDictionnary();
        _Fields = GenerateFields(pBounds);
        isInit = true;
    }

    public void Update()
    {
        if(!isInit)
        {
            return;
        }

        if (CurrentDestination.position != Vector3.zero)
        {
            ShowFlowField(lPaths[CurrentDestination]);
        }
    }

    public Node PreviewFlowField(Node pNode)
    {
        return CurrentDestination = pNode;
    }

    public Node[,] GetFlowField(Node pNode)
    {
        return lPaths[pNode];
    }

    private void InitDictionnary()
    {
        lDirections[Dir.Left] = new Vector2Int(-1,0);
        lDirections[Dir.Right] = new Vector2Int(1,0);
        lDirections[Dir.Top] = new Vector2Int(0, 1);
        lDirections[Dir.Bottom] = new Vector2Int(0, -1);
        lDirections[Dir.TopRight] = new Vector2Int(1, 1);
        lDirections[Dir.BottomRight] = new Vector2Int(1, -1);
        lDirections[Dir.TopLeft] = new Vector2Int(-1, 1);
        lDirections[Dir.BottomLeft] = new Vector2Int(-1, -1);
    }

    private Node[][,] GenerateFields(Bounds pBounds)
    {
        _Bounds = pBounds;

        Node[,] lStartingGrid = GenerateGrid();

        _Exits = QueryExits(lStartingGrid);

        Node[][,] lFields = new Node[_Exits.Length][,];

        if(_Exits.Length == 0)
        {
            Debug.Log("no exits");
            return null;
        }

        for (int i = 0; i < _Exits.Length; i++)
        {
            Node[,] lGrid = (Node[,])lStartingGrid.Clone();
            lFields[i] = GenerateField(lGrid, _Exits[i]);
            lPaths[_Exits[i]] = lFields[i];
        }

        return lFields;
    }

    public Node[] GetExits()
    {
        return _Exits;
    }

    private Node[] QueryExits(Node[,] pField)
    {
        List<Node> lExits = new List<Node>();
        foreach (Node lNode in pField) 
        { 
            if(lNode.cost == -2)
            {
                lExits.Add(lNode);
            }
        }

        return lExits.ToArray();
    }

    private Node[,] GenerateGrid()
    {
        int lNumCellsX = Mathf.CeilToInt(_Bounds.size.x / _CellSize);
        int lNumCellsY = Mathf.CeilToInt(_Bounds.size.y / _CellSize);

        Node[,] lField = new Node[lNumCellsX, lNumCellsY];

        for (int i = 0; i < lNumCellsX; i++) 
        {
            for (int j = 0; j < lNumCellsY; j++) 
            { 
                Node lCurrentNode = new Node();

                Vector3 lPos = new Vector3(
                    (i + 0.5f) * _CellSize,
                    (j + 0.5f) * _CellSize,
                    0);

                lCurrentNode.position = lPos + transform.position - _Bounds.extents;
                lCurrentNode.size = _CellSize;
                lCurrentNode.cost = 0;
                lCurrentNode.gridPosition = new Vector2Int(i,j);

                if (i == 0 || i == lNumCellsX - 1 || j == 0 || j == lNumCellsY - 1)
                {
                    if (!CheckNodeOccupancy(lCurrentNode))
                    {
                        lCurrentNode.cost = -2;
                    }
                }

                lField[i, j] = lCurrentNode;
            }
        }

        return lField;
    }

    private bool CheckNodeOccupancy(Node pCurrentNode)
    {
        return Physics.CheckBox(pCurrentNode.position, Vector3.one * pCurrentNode.size * 0.5f, transform.rotation, WallLayer);
    }

    private Node[,] GenerateField(Node[,] pGrid,Node pGoalNode)
    {
        for(int i = 0;i < pGrid.GetLength(0); i++)
        {
            for(int j = 0; j < pGrid.GetLength(1); j++)
            {
                if (CheckNodeOccupancy(pGrid[i, j]))
                {
                    pGrid[i, j].cost = -1;
                }
            }
        }

        Queue<Node> lQueue = new Queue<Node>();
        pGoalNode.cost = 0;
        lQueue.Enqueue(pGoalNode);

        while (lQueue.Count > 0)
        {
            Node lCurrent = lQueue.Dequeue();

            foreach (Vector2Int lDir in lDirections.Values)
            {
                Vector2Int lNextGridPos = lCurrent.gridPosition;
                lNextGridPos += lDir;

                if (lNextGridPos.x < 0 || lNextGridPos.x > pGrid.GetLength(0) - 1 || lNextGridPos.y < 0 || lNextGridPos.y > pGrid.GetLength(1) - 1)
                {
                    continue;
                }

                Node lNeightborCell = pGrid[lNextGridPos.x, lNextGridPos.y];

                if (lNeightborCell.cost == 0)
                {
                    lNeightborCell.cost = lCurrent.cost + 1;
                    lNeightborCell.bestDirection = -(Vector2)lDir;

                    lQueue.Enqueue(lNeightborCell);
                }

                pGrid[lNextGridPos.x, lNextGridPos.y] = lNeightborCell;
            }
        }

        return pGrid;
    }

    private void ShowFlowField(Node[,] pField)
    {
        foreach (Node node in pField) 
        {
            Debug.DrawRay(node.position, node.bestDirection * node.size, Color.blue);
        }
    }
}

public struct Node
{
    public float size;
    public Vector3 position;
    public Vector2Int gridPosition;
    public float cost;
    public Vector3 bestDirection;
}