using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DroneMover : MonoBehaviour
{
    public float speed = 5f;
    LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.startWidth = lr.endWidth = 0.1f;
        lr.material = new Material(Shader.Find("Unlit/Color")) { color = Color.cyan };
    }

    public IEnumerator FollowPath(List<Node> path)
    {
        lr.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
            lr.SetPosition(i, path[i].worldPosition);

        foreach (Node n in path)
        {
            Vector3 tgt = n.worldPosition;
            while (Vector3.Distance(transform.position, tgt) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, tgt, speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
