using UnityEngine;

public class Chunk : MonoBehaviour
{
    FlowField Fields;

    float timer;
    [SerializeField] float FieldRefreshRate;
    void Start()
    {
        if(TryGetComponent(out Fields))
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

    public void RegenerateField()
    {
        Fields.Init();
    }
}
