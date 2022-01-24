using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStar
{
    private static float TileSize = 1.0f;
    private static int HeuristicConstant = 1;

    // Class to bind position, heuristics and parent node of the path
    internal class Node
    {
        public Node parent;
        public Vector3 pos;
        public int F, G, H;
        public Node(Vector3 position)
        {
            pos = position;
        }
        public Node(Node n)
        {
            parent = n.parent;
            pos = n.pos;
            F = n.F;
            G = n.G;
            H = n.H;
        }
        public override bool Equals(object obj)
        {
            if (obj is Node n)
                return pos == n.pos;
            return base.Equals(obj);
        }
        public static bool operator ==(Node a, Node b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(Node a, Node b)
        {
            return !Equals(a, b);
        }

        public override int GetHashCode()
        {
            return pos.GetHashCode();
        }
    }

    private static Node ExtractMin(List<Node> li)
    {
        Node lowest = li[0];
        for (int i = 1; i < li.Count; i++)
        {
            Node n = li[i];
            if (n.F < lowest.F)
                lowest = n;
        }
        li.Remove(lowest);
        return lowest;
    }

    private static Node[] OrthogonalNodes(Vector3 p)
    {
        return new Node[] {
            new Node(new Vector3(p.x, p.y - TileSize)), // Down
            new Node(new Vector3(p.x - TileSize, p.y)), // Left
            new Node(new Vector3(p.x + TileSize, p.y)), // Right
            new Node(new Vector3(p.x, p.y + TileSize)), // Up
        };
    }

    private static Node[] DiagonalNodes(Vector3 p)
    {
        return new Node[] {
            new Node(new Vector3(p.x - TileSize, p.y - TileSize)), // Down left
            new Node(new Vector3(p.x - TileSize, p.y + TileSize)), // Up Left
            new Node(new Vector3(p.x + TileSize, p.y + TileSize)), // Up Right
            new Node(new Vector3(p.x + TileSize, p.y - TileSize)), // Down Right
        };
    }

    public static Vector3[] ShortestPath(Vector3 start, Vector3 target)
    {
        start = VectorExtension.RoundVector(start);
        target = VectorExtension.RoundVector(target);

        // Get collider of tilemap
        var tilemap = GameObject.FindGameObjectWithTag("Enviroment").GetComponent<TilemapCollider2D>();
        if (tilemap == null)
        {
            Debug.LogError("Astar could not find tilemap collider, aborting!");
            return null;
        }
        if (tilemap.OverlapPoint(start))
        {
            Debug.LogError("Start is not valid, aborting!");
            throw new System.ArgumentException("Start is not valid, aborting!");
        }
        if (tilemap.OverlapPoint(target))
        {
            Debug.LogError("Target is not valid, aborting!");
            throw new System.ArgumentException("Target is not valid, aborting!");
        }

        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();
        List<Vector3> path = new List<Vector3>();

        Node startNode = new Node(start);
        Node targetNode = new Node(target);

        open.Add(startNode);

        while(open.Count > 0)
        {
            // Finds the node with the lowest cost in the list of open nodes. Breaks if the list is empty.
            Node centerNode = ExtractMin(open);

            // If it's the target node, break. Adds it to the list of closed nodes.
            if (centerNode.Equals(targetNode))
            {
                Node node = centerNode;
                while (node != startNode)
                {
                    path.Insert(0, node.pos);
                    node = node.parent;
                }
                break;
            }

            closed.Add(centerNode);

            // Orthogonal nodes
            Node[] potentialNodes = OrthogonalNodes(centerNode.pos);
            for (int i = 0; i < potentialNodes.Length; i++)
            {
                Node node = potentialNodes[i];
                if (!closed.Contains(node) && !tilemap.OverlapPoint(node.pos))
                {
                    if (!open.Contains(node) || centerNode.G + 10 < node.G)
                    {
                        node.parent = centerNode;
                        node.G = centerNode.G + 10;
                        node.H = (int)((Mathf.Abs(target.x - node.pos.x) + Mathf.Abs(target.y - node.pos.y)) * HeuristicConstant);
                        node.F = node.G + node.H;
                        if (!open.Contains(node))
                            open.Add(node);
                    }
                }
            }

            // Diagonal nodes
            potentialNodes = DiagonalNodes(centerNode.pos);
            for (int i = 0; i < potentialNodes.Length; i++)
            {
                Node node = potentialNodes[i];
                if (!closed.Contains(node) &&
                    !tilemap.OverlapPoint(node.pos) &&
                    !tilemap.OverlapPoint(new Vector2(node.pos.x, centerNode.pos.y)) &&
                    !tilemap.OverlapPoint(new Vector2(centerNode.pos.x, node.pos.y)))
                {
                    if (!open.Contains(node) || centerNode.G + 14 < node.G)
                    {
                        node.parent = centerNode;
                        node.G = centerNode.G + 14;
                        node.H = (int)((Mathf.Abs(target.x - node.pos.x) + Mathf.Abs(target.y - node.pos.y)) * HeuristicConstant);
                        node.F = node.G + node.H;
                        if (!open.Contains(node))
                            open.Add(node);
                    }
                }
            }
        }

        return path.ToArray();
    }
}
