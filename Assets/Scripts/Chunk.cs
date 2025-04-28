using UnityEngine;

public class Chunk : MonoBehaviour
{
    FlowField Fields;
    void Start()
    {
        if(TryGetComponent(out Fields))
        {
            Fields.Init();
        }
    }

    void Update()
    {
        
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
}
