using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class AstarNode
{
	public Vector3Int Position { get; set; }
	public AstarNode Parent { get; set; }
	public int GCost { get; set; }
	public int HCost { get; set; }
	public int FCost { get; set; }

	public AstarNode(Vector3Int position)
	{
		Position = position;
	}
}

public class Astar
{
	public static List<Vector3Int> FindPath(Vector3Int startPosition, Vector3Int endPosition)
	{
		List<Vector3Int> path = new List<Vector3Int>();
		AstarNode startNode = new AstarNode(startPosition);
		AstarNode endNode = new AstarNode(endPosition);

		List<AstarNode> openList = new List<AstarNode>();
		HashSet<AstarNode> closedSet = new HashSet<AstarNode>();

		openList.Add(startNode);

		while (openList.Count > 0)
		{
			if(closedSet.Count > 5000) 
				return null;
				
			AstarNode currentNode = openList[0];
			for (int i = 1; i < openList.Count; i++)
			{
				if (openList[i].FCost < currentNode.FCost || (openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost))
				{
					currentNode = openList[i];
				}
			}

			openList.Remove(currentNode);
			closedSet.Add(currentNode);

			if (currentNode.Position == endNode.Position)
			{
				// 경로를 역으로 추적
				AstarNode pathNode = currentNode;
				while (pathNode != null)
				{
					path.Insert(0, pathNode.Position);
					pathNode = pathNode.Parent;
				}
				
				break;
			}

			foreach (Vector3Int neighborPos in Astar.GetNeighborPositions(currentNode.Position))
			{
				if (MapManager.Instance.Map.ObstaclePositions.Contains(neighborPos) || closedSet.Contains(new AstarNode(neighborPos)))
				{
					if(neighborPos != endNode.Position) 
					{
						continue;
					}
				}

				int tentativeGCost = currentNode.GCost + GetDistance(currentNode.Position, neighborPos);

				AstarNode neighborNode = new AstarNode(neighborPos);
				neighborNode.Parent = currentNode;
				neighborNode.GCost = tentativeGCost;
				neighborNode.HCost = GetDistance(neighborPos, endNode.Position);
				neighborNode.FCost = neighborNode.GCost + neighborNode.HCost;

				if (!openList.Contains(neighborNode) || tentativeGCost < neighborNode.GCost)
				{
					openList.Add(neighborNode);
				}
			}
		}

		return path;
	}

	private static int GetDistance(Vector3Int posA, Vector3Int posB)
	{
		int distX = Mathf.Abs(posA.x - posB.x);
		int distY = Mathf.Abs(posA.y - posB.y);
		return distX + distY;
	}

	private static List<Vector3Int> GetNeighborPositions(Vector3Int position)
	{
		List<Vector3Int> neighbors = new List<Vector3Int>();

		// 상, 하, 좌, 우, 대각선 방향의 이웃 위치를 추가합니다.
		neighbors.Add(new Vector3Int(position.x, position.y + 1, position.z));
		neighbors.Add(new Vector3Int(position.x, position.y - 1, position.z));
		neighbors.Add(new Vector3Int(position.x - 1, position.y, position.z));
		neighbors.Add(new Vector3Int(position.x + 1, position.y, position.z));
		neighbors.Add(new Vector3Int(position.x - 1, position.y + 1, position.z));
		neighbors.Add(new Vector3Int(position.x + 1, position.y + 1, position.z));
		neighbors.Add(new Vector3Int(position.x - 1, position.y - 1, position.z));
		neighbors.Add(new Vector3Int(position.x + 1, position.y - 1, position.z));

		return neighbors;
	}
}