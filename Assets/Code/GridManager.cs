using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("水平网格尺寸 (X, Z)")]
    public Vector3 gridWorldSize;    // X: 地面宽度, Z: 地面深度, Y 字段不用

    [Header("垂直网格层数 (Y 方向)")]
    public int verticalLayers = 5;    // 由你手动设置的层数

    [Header("节点半径")]
    public float nodeRadius = 1f;

    [Header("障碍物检测 LayerMask")]
    public LayerMask obstacleMask;

    private Node[,,] grid;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY, gridSizeZ;
    private float gridHeight;

    void Awake()
    {
        // 1. 计算每层直径
        nodeDiameter = nodeRadius * 2f;

        // 2. X/Z 轴网格尺寸由 gridWorldSize 决定，Y 轴由 verticalLayers 决定
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);
        gridSizeY = verticalLayers;
        gridHeight = gridSizeY * nodeDiameter;

        // 3. 构建网格
        CreateGrid();

        Debug.Log($"✅ GridManager 初始化完毕: X={gridSizeX}, Y={gridSizeY}, Z={gridSizeZ}");
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY, gridSizeZ];

        // 基准点：地面中心减去网格半宽、半高、半深
        Vector3 origin = transform.position
                       - new Vector3(
                            gridWorldSize.x / 2f,
                            gridHeight / 2f,
                            gridWorldSize.z / 2f
                         );

        for (int x = 0; x < gridSizeX; x++)
            for (int y = 0; y < gridSizeY; y++)
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Vector3 worldPoint = origin
                        + Vector3.right * (x * nodeDiameter + nodeRadius)
                        + Vector3.up * (y * nodeDiameter + nodeRadius)
                        + Vector3.forward * (z * nodeDiameter + nodeRadius);

                    bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, obstacleMask);
                    grid[x, y, z] = new Node(walkable, worldPoint, x, y, z);
                }
    }

    // 将一个世界坐标映射到最近的网格节点
    public Node NodeFromWorldPoint(Vector3 pos)
    {
        // 与 CreateGrid 同样的 origin 计算
        Vector3 origin = transform.position
                       - new Vector3(
                            gridWorldSize.x / 2f,
                            gridHeight / 2f,
                            gridWorldSize.z / 2f
                         );

        Vector3 local = pos - origin;

        int ix = Mathf.Clamp(Mathf.RoundToInt(local.x / nodeDiameter), 0, gridSizeX - 1);
        int iy = Mathf.Clamp(Mathf.RoundToInt(local.y / nodeDiameter), 0, gridSizeY - 1);
        int iz = Mathf.Clamp(Mathf.RoundToInt(local.z / nodeDiameter), 0, gridSizeZ - 1);

        return grid[ix, iy, iz];
    }

    // 获取周围 26 个邻居（含上下前后）
    public IEnumerable<Node> GetNeighbors(Node n)
    {
        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
                for (int dz = -1; dz <= 1; dz++)
                {
                    if (dx == 0 && dy == 0 && dz == 0) continue;
                    int nx = n.gridX + dx, ny = n.gridY + dy, nz = n.gridZ + dz;
                    if (nx >= 0 && nx < gridSizeX
                     && ny >= 0 && ny < gridSizeY
                     && nz >= 0 && nz < gridSizeZ)
                    {
                        yield return grid[nx, ny, nz];
                    }
                }
    }


}
