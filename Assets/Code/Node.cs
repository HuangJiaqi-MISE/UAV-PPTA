using UnityEngine;

public class Node
{
    public Vector3 worldPosition;
    public bool walkable;

    public int gridX;   // 原来就有
    public int gridY;   // ⚡ 保留作高度索引用
    public int gridZ;   // ⚡ 新增：Z 轴索引

    public int gCost, hCost;
    public int fCost => gCost + hCost;
    public Node parent;

    // ⚡ 构造函数改为 3 维
    public Node(bool walkable, Vector3 worldPos, int x, int y, int z)
    {
        this.walkable = walkable;
        this.worldPosition = worldPos;
        gridX = x;
        gridY = y;   // 这里代表“层”索引
        gridZ = z;
    }
}
