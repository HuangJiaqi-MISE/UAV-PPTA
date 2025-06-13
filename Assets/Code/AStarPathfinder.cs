using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder : MonoBehaviour
{
    public Transform startPoint, endPoint;
    GridManager grid;

    void Start()
    {
        grid = FindObjectOfType<GridManager>();
        if (grid == null) { Debug.LogError("GridManager not found"); return; }
        if (startPoint == null || endPoint == null) return;

        FindPath(startPoint.position, endPoint.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        if (startNode == null || targetNode == null) return;

        var open = new List<Node> { startNode };
        var closed = new HashSet<Node>();

        while (open.Count > 0)
        {
            Node cur = open[0];
            for (int i = 1; i < open.Count; i++)
                if (open[i].fCost < cur.fCost ||
                    open[i].fCost == cur.fCost && open[i].hCost < cur.hCost)
                    cur = open[i];

            open.Remove(cur);
            closed.Add(cur);

            if (cur == targetNode) { RetracePath(startNode, targetNode); return; }

            foreach (Node nb in grid.GetNeighbors(cur))
            {
                if (!nb.walkable || closed.Contains(nb)) continue;
                int newG = cur.gCost + GetDistance(cur, nb);
                if (newG < nb.gCost || !open.Contains(nb))
                {
                    nb.gCost = newG;
                    nb.hCost = GetDistance(nb, targetNode);
                    nb.parent = cur;
                    if (!open.Contains(nb)) open.Add(nb);
                }
            }
        }
    }

    // ⚡ 3 维曼哈顿距离
    int GetDistance(Node a, Node b)
    {
        return (Mathf.Abs(a.gridX - b.gridX) +
                Mathf.Abs(a.gridY - b.gridY) +
                Mathf.Abs(a.gridZ - b.gridZ)) * 10;
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node cur = endNode;
        while (cur != startNode)
        {
            path.Add(cur);
            cur = cur.parent;
        }
        path.Add(startNode);
        path.Reverse();

        Debug.Log($"🔢 3D 路径节点数：{path.Count}");
        foreach (var n in path)
            Debug.Log($"    节点：{n.worldPosition}");

        StartCoroutine(GetComponent<DroneMover>().FollowPath(path));
    }

}
