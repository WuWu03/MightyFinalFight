using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFrameWork.Map
{
    public static class AStar
    {
        private delegate float GetDistance(Vector2Int a, Vector2Int b);//获取h值(距离)
        private delegate Vector2Int[] GetNeighbors(Vector2Int pos);//获取周边的点

        private class Node : IComparable
        {
            public Node parent;
            public Vector2Int pos;
            public float f;
            public float h;
            public float g;
            public bool open = true;

            public Node(Node parent, Vector2Int pos, float h, float g)
            {
                this.parent = parent;
                this.pos = pos;
                this.h = h;
                this.g = g;
                this.f = h + g;
                this.open = true;
            }

            public int CompareTo(object obj)
            {
                if (obj == null)
                {
                    return 1;
                }

                Node temp = obj as Node;
                return this.f > temp.f ? 1 : -1;
            }
        }

        /// <summary>
        /// 四边形astar
        /// </summary>

        public static List<Vector2Int> Find4(Vector2Int from, Vector2Int to, Dictionary<Vector2Int, int> mapTypes, List<int> passTypes)
        {
            GetDistance getDistance = delegate (Vector2Int a, Vector2Int b)
            {
                float xDistance = Mathf.Abs(a.x - b.x);
                float yDistance = Mathf.Abs(a.y - b.y);
                return xDistance * xDistance + yDistance * yDistance;
            };

            GetNeighbors getNeighbors = delegate (Vector2Int pos)
            {
                Vector2Int[] neighbors = new Vector2Int[4]
                {
                    new Vector2Int(pos.x, pos.y + 1),
                    new Vector2Int(pos.x, pos.y - 1),
                    new Vector2Int(pos.x + 1, pos.y),
                    new Vector2Int(pos.x - 1, pos.y),
                };

                return neighbors;
            };

            return AStarPathFinding(from, to, mapTypes, passTypes, getDistance, getNeighbors);
        }

        /// <summary>
        /// 四边形astar允许对角移动
        /// </summary>
        public static List<Vector2Int> Find8(Vector2Int from, Vector2Int to, Dictionary<Vector2Int, int> mapTypes, List<int> passTypes)
        {
            GetDistance getDistance = delegate (Vector2Int a, Vector2Int b)
            {
                float xDistance = Mathf.Abs(a.x - b.x);
                float yDistance = Mathf.Abs(a.y - b.y);
                return xDistance * xDistance + yDistance * yDistance;
            };

            GetNeighbors getNeighbors = delegate (Vector2Int pos)
            {
                Vector2Int[] neighbors = new Vector2Int[8]
                {
                    new Vector2Int(pos.x - 1, pos.y + 1),
                    new Vector2Int(pos.x, pos.y + 1),
                    new Vector2Int(pos.x + 1, pos.y + 1),

                    new Vector2Int(pos.x + 1, pos.y),
                    new Vector2Int(pos.x - 1, pos.y),

                    new Vector2Int(pos.x + 1, pos.y - 1),
                    new Vector2Int(pos.x, pos.y - 1),
                    new Vector2Int(pos.x - 1, pos.y - 1),
                };

                return neighbors;
            };

            return AStarPathFinding(from, to, mapTypes, passTypes, getDistance, getNeighbors);
        }
        /*六边形x轴排列astar
         * 00    20    40
         * 
         *    11    31 
         *   
         * 02    22    42    
         * 
         *    13    33
         *    
         * 04    24    44
         */
        public static List<Vector2Int> Find6X(Vector2Int from, Vector2Int to, Dictionary<Vector2Int, int> mapTypes, List<int> passTypes)
        {
            GetDistance getDistance = delegate (Vector2Int a, Vector2Int b)
            {
                float xDistance = Mathf.Abs(a.x - b.x);
                float yDistance = Mathf.Abs(a.y - b.y);
                return xDistance * xDistance + yDistance * yDistance * 3;//相邻正六边形y轴距离为根号三，此处yDistance是按照相邻距离为1进行计算，因此要扩大根号三倍
            };

            GetNeighbors getNeighbors = delegate (Vector2Int pos)
            {
                Vector2Int[] neighbors = new Vector2Int[6]
                {
                    new Vector2Int(pos.x + 1, pos.y + 1),
                    new Vector2Int(pos.x - 1, pos.y + 1),
                    new Vector2Int(pos.x + 1, pos.y - 1),
                    new Vector2Int(pos.x - 1, pos.y - 1),
                    new Vector2Int(pos.x - 2, pos.y),
                    new Vector2Int(pos.x + 2, pos.y),
                };

                return neighbors;
            };

            return AStarPathFinding(from, to, mapTypes, passTypes, getDistance, getNeighbors);
        }

        /*六边形y轴排列astar
         * 00      20      40
         *     11      31  
         * 02      22      42        
         *     13      33
         * 04      24      44    
         */
        public static List<Vector2Int> Find6Y(Vector2Int from, Vector2Int to, Dictionary<Vector2Int, int> mapTypes, List<int> passTypes)
        {
            GetDistance getDistance = delegate (Vector2Int a, Vector2Int b)
            {
                float xDistance = Mathf.Abs(a.x - b.x);
                float yDistance = Mathf.Abs(a.y - b.y);
                return xDistance * xDistance * 3 + yDistance * yDistance;//相邻正六边形x轴距离为根号三，此处xDistance是按照相邻距离为1进行计算，因此要扩大根号三倍
            };

            GetNeighbors gn = delegate (Vector2Int pos)
            {
                Vector2Int[] neighbors = new Vector2Int[6]
                {
                    new Vector2Int(pos.x + 1, pos.y + 1),
                    new Vector2Int(pos.x - 1, pos.y + 1),
                    new Vector2Int(pos.x + 1, pos.y - 1),
                    new Vector2Int(pos.x - 1, pos.y - 1),
                    new Vector2Int(pos.x, pos.y - 2),
                    new Vector2Int(pos.x, pos.y + 2)
                };

                return neighbors;
            };

            return AStarPathFinding(from, to, mapTypes, passTypes, getDistance, gn);
        }

        public static List<Vector2Int> CheckRange4(Vector2Int point, int range, Dictionary<Vector2Int, int> map, List<int> passTypes)
        {
            GetNeighbors getNeighbors = delegate (Vector2Int pos)
            {
                Vector2Int[] neighbors = new Vector2Int[4]
                {
                    new Vector2Int(pos.x, pos.y + 1),
                    new Vector2Int(pos.x, pos.y - 1),
                    new Vector2Int(pos.x + 1, pos.y),
                    new Vector2Int(pos.x - 1, pos.y),
                };

                return neighbors;
            };

            return CheckRange(point, range, map, passTypes, getNeighbors);
        }

        public static List<Vector2Int> CheckRange8(Vector2Int point, int range, Dictionary<Vector2Int, int> map, List<int> passTypes)
        {
            GetNeighbors getNeighbors = delegate (Vector2Int pos)
            {
                Vector2Int[] neighbors = new Vector2Int[8]
                {
                    new Vector2Int(pos.x - 1, pos.y + 1),
                    new Vector2Int(pos.x, pos.y + 1),
                    new Vector2Int(pos.x + 1, pos.y + 1),

                    new Vector2Int(pos.x + 1, pos.y),
                    new Vector2Int(pos.x - 1, pos.y),

                    new Vector2Int(pos.x + 1, pos.y - 1),
                    new Vector2Int(pos.x, pos.y - 1),
                    new Vector2Int(pos.x - 1, pos.y - 1),
                };

                return neighbors;
            };

            return CheckRange(point, range, map, passTypes, getNeighbors);
        }

        public static List<Vector2Int> CheckRange6X(Vector2Int point, int range, Dictionary<Vector2Int, int> map, List<int> passTypes)
        {
            GetNeighbors getNeighbors = delegate (Vector2Int pos)
            {
                Vector2Int[] neighbors = new Vector2Int[6]
                {
                    new Vector2Int(pos.x + 1, pos.y + 1),
                    new Vector2Int(pos.x - 1, pos.y + 1),
                    new Vector2Int(pos.x + 1, pos.y - 1),
                    new Vector2Int(pos.x - 1, pos.y - 1),
                    new Vector2Int(pos.x - 2, pos.y),
                    new Vector2Int(pos.x + 2, pos.y),
                };

                return neighbors;
            };

            return CheckRange(point, range, map, passTypes, getNeighbors);
        }

        public static List<Vector2Int> CheckRange6Y(Vector2Int point, int range, Dictionary<Vector2Int, int> map, List<int> passTypes)
        {
            GetNeighbors getNeighbors = delegate (Vector2Int pos)
            {
                Vector2Int[] neighbors = new Vector2Int[6]
                {
                    new Vector2Int(pos.x + 1, pos.y + 1),
                    new Vector2Int(pos.x - 1, pos.y + 1),
                    new Vector2Int(pos.x + 1, pos.y - 1),
                    new Vector2Int(pos.x - 1, pos.y - 1),
                    new Vector2Int(pos.x, pos.y - 2),
                    new Vector2Int(pos.x, pos.y + 2)
                };
                return neighbors;
            };

            return CheckRange(point, range, map, passTypes, getNeighbors);
        }

        private static List<Vector2Int> AStarPathFinding(Vector2Int from, Vector2Int to, Dictionary<Vector2Int, int> map, List<int> passTypes, GetDistance getDistance, GetNeighbors getNeighbors)
        {
            List<Vector2Int> results = new List<Vector2Int>();

            if (from == to)
            {
                results.Add(from);
                return results;
            }

            Queue<Node> nodes = new Queue<Node>();
            List<Node> openList = new List<Node>();
            Node current = null;

            nodes.Enqueue(new Node(null, from, getDistance(from, to), 0));

            while (nodes.Count > 0)
            {
                current = nodes.Dequeue();

                if (current.pos == to)
                {
                    break;
                }

                current.open = false;

                if (!openList.Contains(current))
                {
                    openList.Add(current);
                }

                Vector2Int[] neighbors = getNeighbors(current.pos);

                for (int i = 0; i < neighbors.Length; i++)
                {
                    if (!map.ContainsKey(neighbors[i]) || !passTypes.Contains(map[neighbors[i]]))
                    {
                        continue;
                    }

                    Node temp = openList.Find(obj => obj.pos == neighbors[i]);

                    if (temp == null)
                    {
                        temp = new Node(current, neighbors[i], getDistance(neighbors[i], to), current.g + 1);
                        openList.Add(temp);
                    }
                    else if (temp.open && temp.g > current.g + 1)
                    {
                        temp.g = current.g + 1;
                        temp.f = temp.h + temp.g;
                        temp.parent = current;
                    }
                }

                nodes.Enqueue(openList.FindAll(obj => obj.open).Min());
            }

            while (current != null)
            {
                results.Add(current.pos);
                current = current.parent;
            }

            results.Reverse();
            return results;
        }

        private static List<Vector2Int> CheckRange(Vector2Int point, int range, Dictionary<Vector2Int, int> map, List<int> passTypes, GetNeighbors getNeighbors)
        {
            Queue<Vector2Int> queuePoints = new Queue<Vector2Int>();
            Queue<int> queueRange = new Queue<int>();
            List<Vector2Int> results = new List<Vector2Int>();

            queuePoints.Enqueue(point);
            queueRange.Enqueue(range);

            while (queuePoints.Count > 0)
            {
                Vector2Int currentPoint = queuePoints.Dequeue();
                int currentRange = queueRange.Dequeue();

                if (!map.ContainsKey(currentPoint) || !passTypes.Contains(map[currentPoint]))
                {
                    continue;
                }

                if (currentRange < 0)
                {
                    continue;
                }

                if (!results.Contains(currentPoint))
                {
                    results.Add(currentPoint);
                }

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