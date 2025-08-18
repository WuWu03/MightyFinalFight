using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFrameWork.Map
{
    public static class AStar
    {
        private class Node : IComparable<Node>
        {
            public Node parent;
            public Vector2Int pos;
            public float f;
            public float g;
            public float h;
            public bool open = true;

            public Node(Node parent, Vector2Int pos, float h, float g)
            {
                this.parent = parent;
                this.pos = pos;
                this.f = g + h;
                this.g = g;
                this.h = h;
                this.open = true;
            }

            public int CompareTo(Node target)
            {
                if (target == null)
                {
                    return 0;
                }

                if (this.f == target.f)
                {
                    if (this.g == target.g)
                    {
                        return 0;
                    }

                    return this.g > target.g ? -1 : 1;
                }

                return this.f < target.f ? -1 : 1;
            }
        }

        /// <summary>
        /// 矩形网格不包含对角
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="mapTypes"></param>
        /// <param name="passTypes"></param>
        /// <returns></returns>

        public static List<Vector2Int> Find4(Vector2Int from, Vector2Int to, Dictionary<Vector2Int, int> mapTypes, List<int> passTypes)
        {
            static float getDistance(Vector2Int a, Vector2Int b)
            {
                float xDistance = Mathf.Abs(a.x - b.x);
                float yDistance = Mathf.Abs(a.y - b.y);
                return xDistance * xDistance + yDistance * yDistance;
            }

            static Vector2Int[] getNeighbors(Vector2Int pos)
            {
                Vector2Int[] neighbors = new Vector2Int[4]
                {
                    new(pos.x, pos.y + 1),
                    new(pos.x, pos.y - 1),
                    new(pos.x + 1, pos.y),
                    new(pos.x - 1, pos.y),
                };

                return neighbors;
            }

            return AStarPathFinding(from, to, mapTypes, passTypes, getDistance, getNeighbors);
        }


        /// <summary>
        /// 六边形/菱形网格
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="mapTypes"></param>
        /// <param name="passTypes"></param>
        /// <returns></returns>
        public static List<Vector2Int> Find6(Vector2Int from, Vector2Int to, Dictionary<Vector2Int, int> mapTypes, List<int> passTypes)
        {
            static float getDistance(Vector2Int a, Vector2Int b)
            {
                float xDistance = Mathf.Abs(a.x - b.x);
                float yDistance = Mathf.Abs(a.y - b.y);
                return xDistance * xDistance + yDistance * yDistance * 3;//相邻正六边形y轴距离为根号三，此处yDistance是按照相邻距离为1进行计算，因此要扩大根号三倍
            }

            static Vector2Int[] getNeighbors(Vector2Int pos)
            {
                Vector2Int[] neighbors = new Vector2Int[6]
                {
                    new(pos.x + 1, pos.y + 1),
                    new(pos.x - 1, pos.y + 1),
                    new(pos.x + 1, pos.y - 1),
                    new(pos.x - 1, pos.y - 1),
                    new(pos.x - 2, pos.y),
                    new(pos.x + 2, pos.y),
                };

                return neighbors;
            }

            return AStarPathFinding(from, to, mapTypes, passTypes, getDistance, getNeighbors);
        }

        /// <summary>
        /// 矩形网格包含对角
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="mapTypes"></param>
        /// <param name="passTypes"></param>
        /// <returns></returns>
        public static List<Vector2Int> Find8(Vector2Int from, Vector2Int to, Dictionary<Vector2Int, int> mapTypes, List<int> passTypes)
        {
            static float getDistance(Vector2Int a, Vector2Int b)
            {
                float xDistance = Mathf.Abs(a.x - b.x);
                float yDistance = Mathf.Abs(a.y - b.y);
                return xDistance * xDistance + yDistance * yDistance;
            }

            static Vector2Int[] getNeighbors(Vector2Int pos)
            {
                Vector2Int[] neighbors = new Vector2Int[8]
                {
                    new(pos.x - 1, pos.y + 1),
                    new(pos.x, pos.y + 1),
                    new(pos.x + 1, pos.y + 1),

                    new(pos.x + 1, pos.y),
                    new(pos.x - 1, pos.y),

                    new(pos.x + 1, pos.y - 1),
                    new(pos.x, pos.y - 1),
                    new(pos.x - 1, pos.y - 1),
                };

                return neighbors;
            }

            return AStarPathFinding(from, to, mapTypes, passTypes, getDistance, getNeighbors);
        }

        public static List<Vector2Int> CheckRange4(Vector2Int point, int range, Dictionary<Vector2Int, int> map, List<int> passTypes)
        {
            static Vector2Int[] getNeighbors(Vector2Int pos)
            {
                Vector2Int[] neighbors = new Vector2Int[4]
                {
                    new(pos.x, pos.y + 1),
                    new(pos.x, pos.y - 1),
                    new(pos.x + 1, pos.y),
                    new(pos.x - 1, pos.y),
                };

                return neighbors;
            }

            return CheckRange(point, range, map, passTypes, getNeighbors);
        }

        public static List<Vector2Int> CheckRange6(Vector2Int point, int range, Dictionary<Vector2Int, int> map, List<int> passTypes)
        {
            static Vector2Int[] getNeighbors(Vector2Int pos)
            {
                Vector2Int[] neighbors = new Vector2Int[6]
                {
                    new(pos.x + 1, pos.y + 1),
                    new(pos.x - 1, pos.y + 1),
                    new(pos.x + 1, pos.y - 1),
                    new(pos.x - 1, pos.y - 1),
                    new(pos.x - 2, pos.y),
                    new(pos.x + 2, pos.y),
                };

                return neighbors;
            }

            return CheckRange(point, range, map, passTypes, getNeighbors);
        }

        public static List<Vector2Int> CheckRange8(Vector2Int point, int range, Dictionary<Vector2Int, int> map, List<int> passTypes)
        {
            static Vector2Int[] getNeighbors(Vector2Int pos)
            {
                Vector2Int[] neighbors = new Vector2Int[8]
                {
                    new(pos.x - 1, pos.y + 1),
                    new(pos.x, pos.y + 1),
                    new(pos.x + 1, pos.y + 1),

                    new(pos.x + 1, pos.y),
                    new(pos.x - 1, pos.y),

                    new(pos.x + 1, pos.y - 1),
                    new(pos.x, pos.y - 1),
                    new(pos.x - 1, pos.y - 1),
                };

                return neighbors;
            }

            return CheckRange(point, range, map, passTypes, getNeighbors);
        }

        private static List<Vector2Int> AStarPathFinding(Vector2Int from, Vector2Int to, Dictionary<Vector2Int, int> map, List<int> passTypes, GameFrameWorkFloatAction<Vector2Int, Vector2Int> getDistance, GameFrameWorkTemplateAction<Vector2Int, Vector2Int[]> getNeighbors)
        {
            List<Vector2Int> results = new();

            if (from == to)
            {
                results.Add(from);
                return results;
            }

            List<Node> openList = new();
            List<Node> closeList = new();
            Node endNode = null;

            openList.Add(new Node(null, from, getDistance(from, to), 0));

            while (openList.Count > 0)
            {
                Node currNode = openList.Min();
                Vector2Int[] neigbors = getNeighbors(currNode.pos);

                openList.Remove(currNode);
                closeList.Add(currNode);

                endNode = closeList.Find(obj => obj.pos == to);

                if (endNode != null)
                {
                    break;
                }

                for (int i = 0; i < neigbors.Length; i++)
                {
                    FindDestNode(neigbors[i], to, currNode, map, passTypes, getDistance, openList, closeList);
                }
            }

            while (endNode != null)
            {
                results.Add(endNode.pos);
                endNode = endNode.parent;
            }

            results.Reverse();
            return results;
        }

        private static void FindDestNode(Vector2Int from, Vector2Int to, Node currNode, Dictionary<Vector2Int, int> map, List<int> passTypes, GameFrameWorkFloatAction<Vector2Int, Vector2Int> getDistance, List<Node> openList, List<Node> closeList)
        {
            if (!map.TryGetValue(from, out int passType) || !passTypes.Contains(passType))
            {
                return;
            }

            foreach (Node node in closeList)
            {
                if (node.pos == from)
                {
                    return;
                }
            }

            Node temp = null;

            foreach (Node node in openList)
            {
                if (node.pos == from)
                {
                    temp = node;
                    break;
                }
            }

            if (temp == null)
            {
                temp = new Node(currNode, from, currNode.g + 1, getDistance(from, to));
                openList.Add(temp);
            }
            else if (currNode.f > currNode.g + 1 + temp.h)
            {
                temp.g = currNode.g + 1;
                temp.f = temp.g + temp.h;
            }
        }


        private static List<Vector2Int> CheckRange(Vector2Int point, int range, Dictionary<Vector2Int, int> map, List<int> passTypes, GameFrameWorkTemplateAction<Vector2Int, Vector2Int[]> getNeighbors)
        {
            Queue<Vector2Int> queuePoints = new();
            Queue<int> queueRange = new();
            List<Vector2Int> results = new();

            queuePoints.Enqueue(point);
            queueRange.Enqueue(range);

            while (queuePoints.Count > 0)
            {
                Vector2Int currentPoint = queuePoints.Dequeue();
                int currentRange = queueRange.Dequeue();

                if (!map.TryGetValue(currentPoint, out int passType) || !passTypes.Contains(passType))
                {
                    continue;
                }

                if (currentRange < 0 || results.Contains(currentPoint))
                {
                    continue;
                }

                results.Add(currentPoint);
                Vector2Int[] neighbors = getNeighbors(currentPoint);

                for (int i = 0; i < neighbors.Length; i++)
                {
                    Vector2Int neighbor = neighbors[i];

                    if (results.Contains(neighbor))
                    {
                        continue;
                    }

                    queuePoints.Enqueue(neighbor);
                    queueRange.Enqueue(currentRange - 1);
                }
            }

            return results;
        }
    }
}