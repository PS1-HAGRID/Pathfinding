using UnityEngine;

public class Agent : MonoBehaviour
{
    [SerializeField] Chunk _CurrentChunk;
    Vector3 _Velocity;
    [SerializeField] float _Speed = 1;
    bool isTravelling;
    Node _CurrentGoal;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            _CurrentGoal = GetExit(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        if (isTravelling)
        {
            _Velocity = GetVel(_Velocity, _CurrentGoal);
        }

        transform.position += _Velocity * _Speed * Time.deltaTime;
    }

    private Node GetExit(Vector3 pClickPosition)
    {
        Node[] lExits = _CurrentChunk.GetExits();

        Node lClosestExit = lExits[0];

        float lDist = float.PositiveInfinity;

        for(int i = 0; i < lExits.Length; i++)
        {
            float lNextDist = (pClickPosition - lExits[i].position).sqrMagnitude;

            if(lNextDist < lDist)
            {
                lDist = lNextDist;
                lClosestExit = lExits[i];
            }
        }
        
        isTravelling = true;
        _CurrentChunk.Fields.PreviewFlowField(lClosestExit);
        return lClosestExit;
    }

    Vector3 GetVel(Vector3 pVelocity,Node pGoal)
    {
        Bounds lBounds = _CurrentChunk.bounds;
        Vector2Int lGridPosition = new Vector2Int(Mathf.FloorToInt((transform.position.x - (_CurrentChunk.transform.position.x - lBounds.extents.x)) / pGoal.size), Mathf.FloorToInt((transform.position.y - (_CurrentChunk.transform.position.y - lBounds.extents.y)) / pGoal.size));
        Node[,] lGrid = _CurrentChunk.GetPath(pGoal);

        pVelocity = lGrid[lGridPosition.x, lGridPosition.y].bestDirection;

        Debug.Log(pVelocity);

        return pVelocity;
    }
}
