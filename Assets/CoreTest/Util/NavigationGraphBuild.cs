using UnityEngine;
using System.Collections;
using _NavigationGraph;
using System.Collections.Generic;
using Unity.Mathematics;

namespace _Util
{
    public static class NavigationGraphBuild
    {
        public static void CreateGridNavigation(NavigationGraph p_navigationGraph, int XNumber, int ZNumber)
        {
            CreateGridNavigationStructure(XNumber, ZNumber, out Dictionary<int, Vector3> l_navigationNodes, out List<GridNavigationLink> l_navigationLinks);

            List<NavigationNode> l_instancedNavigationNodes = new List<NavigationNode>(l_navigationNodes.Count);

            for (int i = 0; i < l_navigationNodes.Count; i++)
            {
                NavigationNode l_instancedNavigationNode = NavigationGraph.instanciateAndAddNode(p_navigationGraph);
                l_instancedNavigationNode.LocalPosition = l_navigationNodes[i];
                l_instancedNavigationNodes.Add(l_instancedNavigationNode);
            }

            foreach (GridNavigationLink l_link in l_navigationLinks)
            {
                NavigationGraph.createLinkBetween(p_navigationGraph, l_instancedNavigationNodes[l_link.StartNode], l_instancedNavigationNodes[l_link.EndNode],
                        math.distance(l_instancedNavigationNodes[l_link.StartNode].LocalPosition, l_instancedNavigationNodes[l_link.EndNode].LocalPosition));
            }
        }

        struct GridNavigationLink
        {
            public int StartNode;
            public int EndNode;
        }

        private static void CreateGridNavigationStructure(int XNumber, int ZNumber, out Dictionary<int, Vector3> p_navigationNodes, out List<GridNavigationLink> p_navigationLinks)
        {
            int currentKey = 0;

            Dictionary<int, Dictionary<int, int>> l_XandZtoKeyLookupTable = new Dictionary<int, Dictionary<int, int>>();
            p_navigationNodes = new Dictionary<int, Vector3>();
            p_navigationLinks = new List<GridNavigationLink>();

            // Node creation
            for (int x = 0; x < XNumber; x++)
            {
                if (!l_XandZtoKeyLookupTable.ContainsKey(x)) { l_XandZtoKeyLookupTable[x] = new Dictionary<int, int>(); }

                for (int z = 0; z < ZNumber; z++)
                {
                    p_navigationNodes.Add(currentKey, new Vector3(x, 0, z));
                    l_XandZtoKeyLookupTable[x][z] = currentKey;
                    currentKey += 1;
                }
            }

            for (int x = 0; x < XNumber; x++)
            {
                for (int z = 0; z < ZNumber; z++)
                {
                    if (l_XandZtoKeyLookupTable.ContainsKey(x + 1))
                    {
                        p_navigationLinks.Add(new GridNavigationLink()
                        {
                            StartNode = l_XandZtoKeyLookupTable[x][z],
                            EndNode = l_XandZtoKeyLookupTable[x + 1][z]
                        });
                        p_navigationLinks.Add(new GridNavigationLink()
                        {
                            StartNode = l_XandZtoKeyLookupTable[x + 1][z],
                            EndNode = l_XandZtoKeyLookupTable[x][z]
                        });
                    }

                    if (l_XandZtoKeyLookupTable[x].ContainsKey(z - 1))
                    {
                        p_navigationLinks.Add(new GridNavigationLink()
                        {
                            StartNode = l_XandZtoKeyLookupTable[x][z],
                            EndNode = l_XandZtoKeyLookupTable[x][z - 1]
                        });
                        p_navigationLinks.Add(new GridNavigationLink()
                        {
                            StartNode = l_XandZtoKeyLookupTable[x][z - 1],
                            EndNode = l_XandZtoKeyLookupTable[x][z]
                        });
                    }
                }
            }
        }

    }

}

