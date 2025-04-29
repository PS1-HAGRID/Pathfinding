using UnityEngine;

public class Chunk : MonoBehaviour
{
    public FlowField Fields;
    public Bounds bounds;

    float timer;
    [SerializeField] float FieldRefreshRate;
    void Start()
    {
        bounds = GetComponent<MeshRenderer>().bounds;

        if (TryGetComponent(out Fields))
        {
            RegenerateField();
        }
    }

    public Node[,] GetPath(Node pGoal)
    {
        if(Fields == null)
        {
            Debug.Log("no fields attached to chunk");
            return null;
        }

        return Fields.GetFlowField(pGoal);
    }

    public Node[] GetExits()
    {
        if (Fields == null)
        {
            Debug.Log("no fields attached to chunk");
            return null;
        }

        return Fields.GetExits();
    }

    private Bounds GetBounds()
    {
        return bounds;
    }

    public void ShowField(Node pNode)
    {
        Fields.PreviewFlowField(pNode);
    }

    public void RegenerateField()
    {
        Fields.Init(bounds);
    }
}
