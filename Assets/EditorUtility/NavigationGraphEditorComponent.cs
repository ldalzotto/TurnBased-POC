
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector.Editor;

namespace _Navigation
{
    public struct NavigationGraphEditorComponentEditorData
    {
        public Mesh NodeMesh;
    }

    [ExecuteInEditMode]
    public class NavigationGraphEditorComponent : SerializedMonoBehaviour
    {
        [BoxGroup("NavigationGraph")]
        [InlineEditor()]
        public NavigationGraphAsset NavigationGraphAsset;

        [BoxGroup("NavigationGraphEditor")]
        public GameObject NavigationNodePrefab;

        [TabGroup("Graph generation")]
        [Button("CreateGrid")]
        private void CreateGrid()
        {
            if (NavigationGraphAsset != null)
            {
                int currentKey = NavigationGraphAsset.NavigationNodes.Keys.Count == 0 ? 0 : (NavigationGraphAsset.NavigationNodes.Keys.ToList().Max() + 1);

                Dictionary<int, Dictionary<int, int>> l_XandZtoKeyLookupTable = new Dictionary<int, Dictionary<int, int>>();

                // Node creation
                for (int x = 0; x < XNumber; x++)
                {
                    if (!l_XandZtoKeyLookupTable.ContainsKey(x)) { l_XandZtoKeyLookupTable[x] = new Dictionary<int, int>(); }

                    for (int z = 0; z < ZNumber; z++)
                    {
                        SerializedNavigationNode l_newNode = new SerializedNavigationNode();
                        l_newNode.LocalPosition = new Vector3(x, 0, z) * DistanceBetweenCells;
                        NavigationGraphAsset.NavigationNodes.Add(currentKey, l_newNode);
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
                            NavigationGraphAsset.NavigationLinks.Add(new SerializedNavigationLink()
                            {
                                StartNode = l_XandZtoKeyLookupTable[x][z],
                                EndNode = l_XandZtoKeyLookupTable[x + 1][z]
                            });
                            NavigationGraphAsset.NavigationLinks.Add(new SerializedNavigationLink()
                            {
                                StartNode = l_XandZtoKeyLookupTable[x + 1][z],
                                EndNode = l_XandZtoKeyLookupTable[x][z]
                            });
                        }

                        if (l_XandZtoKeyLookupTable[x].ContainsKey(z - 1))
                        {
                            NavigationGraphAsset.NavigationLinks.Add(new SerializedNavigationLink()
                            {
                                StartNode = l_XandZtoKeyLookupTable[x][z],
                                EndNode = l_XandZtoKeyLookupTable[x][z - 1]
                            });
                            NavigationGraphAsset.NavigationLinks.Add(new SerializedNavigationLink()
                            {
                                StartNode = l_XandZtoKeyLookupTable[x][z - 1],
                                EndNode = l_XandZtoKeyLookupTable[x][z]
                            });
                        }
                    }
                }

                CalculateLinkTravelCosts();

                EditorUtility.SetDirty(NavigationGraphAsset);
            }
        }

        [Button("Recalculate link travel costs")]
        private void CalculateLinkTravelCosts()
        {
            if (NavigationGraphAsset != null)
            {
                for (int i = 0; i < NavigationGraphAsset.NavigationLinks.Count; i++)
                {
                    SerializedNavigationLink l_navigationLink = NavigationGraphAsset.NavigationLinks[i];
                    SerializedNavigationNode l_startNodeSerialized = NavigationGraphAsset.NavigationNodes[l_navigationLink.StartNode];
                    SerializedNavigationNode l_endNodeSerialized = NavigationGraphAsset.NavigationNodes[l_navigationLink.EndNode];
                    float l_travelCost = Vector3.Distance(l_startNodeSerialized.LocalPosition, l_endNodeSerialized.LocalPosition);
                    l_navigationLink.TravelCost = l_travelCost;
                    NavigationGraphAsset.NavigationLinks[i] = l_navigationLink;
                }

                EditorUtility.SetDirty(NavigationGraphAsset);

            }

        }

        public void RemoveNodeAndItsLinks(int p_nodeID)
        {
            if (NavigationGraphAsset != null)
            {
                if (NavigationGraphAsset.NavigationNodes.ContainsKey(p_nodeID))
                {
                    NavigationGraphAsset.NavigationNodes.Remove(p_nodeID);

                    for (int i = NavigationGraphAsset.NavigationLinks.Count - 1; i >= 0; i--)
                    {
                        SerializedNavigationLink l_navigationLink = NavigationGraphAsset.NavigationLinks[i];
                        if (l_navigationLink.StartNode == p_nodeID || l_navigationLink.EndNode == p_nodeID)
                        {
                            NavigationGraphAsset.NavigationLinks.RemoveAt(i);
                        }
                    }
                }

                EditorUtility.SetDirty(NavigationGraphAsset);
            }
        }

        [TabGroup("Graph generation")]
        public int XNumber;
        [TabGroup("Graph generation")]
        public int ZNumber;
        [TabGroup("Graph generation")]
        public float DistanceBetweenCells;

        [TabGroup("Graph clear")]
        [Button("Clear graph")]
        private void ClearGraph()
        {
            if (NavigationGraphAsset != null)
            {
                NavigationGraphAsset.NavigationNodes.Clear();
                NavigationGraphAsset.NavigationLinks.Clear();
                EditorUtility.SetDirty(NavigationGraphAsset);
            }
        }

        private Dictionary<int, NavigationNodeEditorComponent> m_instanciatedNavigationNodeEditorComponents = new Dictionary<int, NavigationNodeEditorComponent>();
        private List<NavigationNodeEditorComponent> m_selectedNavigationNodeEditorComponents = new List<NavigationNodeEditorComponent>();
        private GameObject m_NavigationNodeContainerGameObject;

        private void OnDrawGizmosSelected()
        {
            if (NavigationGraphContainer.UniqueNavigationGraph != null)
            {
                Matrix4x4 l_localToWorldMatrix = transform.localToWorldMatrix;

                for (int i = 0; i < NavigationGraphContainer.UniqueNavigationGraph.NavigationNodes.Count; i++)
                {
                    Gizmos.DrawWireCube(l_localToWorldMatrix.MultiplyPoint(NavigationGraphContainer.UniqueNavigationGraph.NavigationNodes[i].LocalPosition),
                        l_localToWorldMatrix.MultiplyPoint(new Vector3(0.2f, 0.2f, 0.2f)));
                }

                var l_listOfNavigationNodes = NavigationGraphContainer.UniqueNavigationGraph.NodeLinksIndexedByStartNode.Values.GetEnumerator();
                while (l_listOfNavigationNodes.MoveNext())
                {
                    for (int i = 0; i < l_listOfNavigationNodes.Current.Count; i++)
                    {
                        Vector3 l_startPosition = l_localToWorldMatrix.MultiplyPoint(l_listOfNavigationNodes.Current[i].StartNode.LocalPosition);
                        Vector3 l_endPosition = l_localToWorldMatrix.MultiplyPoint(l_listOfNavigationNodes.Current[i].EndNode.LocalPosition);
                        GizmosHelper.DrawArrow(l_startPosition, l_endPosition, 0.2f);
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                if (NavigationGraphAsset != null && NavigationGraphAsset.NavigationLinks != null)
                {
                    Matrix4x4 l_localToWorldMatrix = transform.localToWorldMatrix;

                    foreach (var l_navigationNodeEntry in NavigationGraphAsset.NavigationNodes)
                    {
                        Vector3 l_nodePosition = l_localToWorldMatrix.MultiplyPoint(l_navigationNodeEntry.Value.LocalPosition);
                        Color l_oldColor = Gizmos.color;
                        if (m_instanciatedNavigationNodeEditorComponents.ContainsKey(l_navigationNodeEntry.Key)
                                && m_selectedNavigationNodeEditorComponents.Contains(m_instanciatedNavigationNodeEditorComponents[l_navigationNodeEntry.Key]))
                        {
                            Gizmos.color = Color.red;
                        }
                        Gizmos.DrawWireCube(l_nodePosition, new Vector3(0.2f, 0.2f, 0.2f));
                        Gizmos.color = l_oldColor;
                        // Handles.color = new Color(1.0f, 0.5f, 0.0f);
                        Handles.Label(l_nodePosition + (Vector3.up * 0.35f), l_navigationNodeEntry.Key.ToString());
                    }

                    foreach (SerializedNavigationLink l_navigationLink in NavigationGraphAsset.NavigationLinks)
                    {
                        Vector3 l_startPosition = l_localToWorldMatrix.MultiplyPoint(NavigationGraphAsset.NavigationNodes[l_navigationLink.StartNode].LocalPosition);
                        Vector3 l_endPosition = l_localToWorldMatrix.MultiplyPoint(NavigationGraphAsset.NavigationNodes[l_navigationLink.EndNode].LocalPosition);
                        GizmosHelper.DrawArrow(l_startPosition, l_endPosition, 0.2f);

                        /*
                        if (l_navigationLink.IsDoubleSided)
                        {
                            l_startPosition = l_localToWorldMatrix.MultiplyPoint(NavigationGraphAsset.NavigationNodes[l_navigationLink.EndNode].LocalPosition);
                            l_endPosition = l_localToWorldMatrix.MultiplyPoint(NavigationGraphAsset.NavigationNodes[l_navigationLink.StartNode].LocalPosition);
                            GizmosHelper.DrawArrow(l_startPosition, l_endPosition, 0.2f);
                        }
                        */
                    }
                }
            }
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                if (NavigationGraphAsset)
                {
                    if (m_NavigationNodeContainerGameObject == null)
                    {
                        m_NavigationNodeContainerGameObject = new GameObject("NavigationNodeContainer");
                        m_NavigationNodeContainerGameObject.transform.SetParent(transform);
                    }

                    if (m_instanciatedNavigationNodeEditorComponents.Count == 0)
                    {
                        InstanciatedNavigationNodes();
                    }

                    for (int i = 0; i < m_instanciatedNavigationNodeEditorComponents.Count; i++)
                    {
                        m_instanciatedNavigationNodeEditorComponents[i].Tick();
                    }
                }
            }
        }

        private void InstanciatedNavigationNodes()
        {
            if (NavigationNodePrefab != null)
            {
                foreach (var l_navigationNodeEntry in NavigationGraphAsset.NavigationNodes)
                {
                    m_instanciatedNavigationNodeEditorComponents.Add(l_navigationNodeEntry.Key, NavigationNodeEditorComponent.Instanciate(NavigationNodePrefab, m_NavigationNodeContainerGameObject.transform,
                                l_navigationNodeEntry.Key, l_navigationNodeEntry.Value, this, OnNavigationNodeMoved, OnNavigationNodeSelectionChanged));
                }
            }

        }

        private void OnNavigationNodeMoved(NavigationNodeEditorComponent l_movedNavigationNode)
        {
            SerializedNavigationNode l_updatedNode = NavigationGraphAsset.NavigationNodes[l_movedNavigationNode.GetSerializedNavigationNodeId()];
            l_updatedNode.LocalPosition = l_movedNavigationNode.transform.localPosition;
            NavigationGraphAsset.NavigationNodes[l_movedNavigationNode.GetSerializedNavigationNodeId()] = l_updatedNode;
            EditorUtility.SetDirty(NavigationGraphAsset);
        }

        private void OnNavigationNodeSelectionChanged(NavigationNodeEditorComponent l_selectionChangedNavigationnode, bool p_isSelected)
        {
            if (p_isSelected)
            {
                m_selectedNavigationNodeEditorComponents.Add(l_selectionChangedNavigationnode);
            }
            else
            {
                m_selectedNavigationNodeEditorComponents.Remove(l_selectionChangedNavigationnode);
            }
        }

        private void OnDisable()
        {
            m_instanciatedNavigationNodeEditorComponents.Clear();
            m_selectedNavigationNodeEditorComponents.Clear();
            GameObject.DestroyImmediate(m_NavigationNodeContainerGameObject);
        }
    }

    [ExecuteInEditMode]
    public class NavigationNodeEditorComponent : MonoBehaviour
    {
        public static NavigationNodeEditorComponent Instanciate(GameObject p_source, Transform p_parent, int p_serializedNavigationNodeId, SerializedNavigationNode p_serializedNavigationNode,
                          NavigationGraphEditorComponent p_navigationGraphEditorReference, Action<NavigationNodeEditorComponent> p_onPositionChangedCallback, Action<NavigationNodeEditorComponent, bool> p_onSelectionChangedCallback)
        {
            GameObject l_instanciatedGameObject = GameObject.Instantiate(p_source);
            NavigationNodeEditorComponent thiz = l_instanciatedGameObject.AddComponent<NavigationNodeEditorComponent>();
            thiz.gameObject.transform.parent = p_parent;
            thiz.m_lastFrameSelection = BoolVariable.New(false, (p_value) => { p_onSelectionChangedCallback(thiz, p_value); }, (p_value) => { p_onSelectionChangedCallback(thiz, p_value); });
            thiz.m_serializedNavigationNodeId = p_serializedNavigationNodeId;
            thiz.m_positionChangedCallback = p_onPositionChangedCallback;
            thiz.transform.localPosition = p_serializedNavigationNode.LocalPosition;
            thiz.m_lastFrameLocalPosition = thiz.transform.localPosition;
            thiz.m_navigationGraphEditorReference = p_navigationGraphEditorReference;
            thiz.Tick();
            return thiz;
        }
        public NavigationNodeEditorComponentLinks NavigationNodeEditorComponentLinks = NavigationNodeEditorComponentLinks.New();
        private int m_serializedNavigationNodeId;
        public int GetSerializedNavigationNodeId() { return m_serializedNavigationNodeId; }
        private NavigationGraphEditorComponent m_navigationGraphEditorReference;
        private Action<NavigationNodeEditorComponent> m_positionChangedCallback;
        private Vector3 m_lastFrameLocalPosition;

        private BoolVariable m_lastFrameSelection;

        [BoxGroup("NavigationLink Creation")]
        [LabelText("EndNode")]
        [SerializeField]
        private int m_navigationLinkCreation_endNode;

        [BoxGroup("NavigationLink Creation")]
        [Button("Create")]
        private void OnNavigationLinkCreation()
        {
            RefreshLinks();
            for (int i = 0; i < m_navigationGraphEditorReference.NavigationGraphAsset.NavigationLinks.Count; i++)
            {
                SerializedNavigationLink l_serializedNavigationLink = m_navigationGraphEditorReference.NavigationGraphAsset.NavigationLinks[i];
                if (l_serializedNavigationLink.StartNode == m_serializedNavigationNodeId && l_serializedNavigationLink.EndNode == m_navigationLinkCreation_endNode)
                {
                    return;
                }
            }

            m_navigationGraphEditorReference.NavigationGraphAsset.NavigationLinks.Add(SerializedNavigationLink.New(m_serializedNavigationNodeId, m_navigationLinkCreation_endNode));
            EditorUtility.SetDirty(m_navigationGraphEditorReference.NavigationGraphAsset);
            RefreshLinks();
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            m_lastFrameSelection.Set(Selection.Contains(gameObject));
            RefreshLinks();
        }

        private void OnNavigationNodeLinkRemoved(NavigationNodeEditorComponentLink p_NavigationNodeEditorComponentLink)
        {
            for (int i = 0; i < m_navigationGraphEditorReference.NavigationGraphAsset.NavigationLinks.Count; i++)
            {
                SerializedNavigationLink l_serializedNavigationLink = m_navigationGraphEditorReference.NavigationGraphAsset.NavigationLinks[i];
                if (l_serializedNavigationLink.StartNode == m_serializedNavigationNodeId && l_serializedNavigationLink.EndNode == p_NavigationNodeEditorComponentLink.TargetNodeID)
                {
                    m_navigationGraphEditorReference.NavigationGraphAsset.NavigationLinks.RemoveAt(i);
                    EditorUtility.SetDirty(m_navigationGraphEditorReference.NavigationGraphAsset);
                    RefreshLinks();
                    break;
                }
            }
        }

        public void Tick()
        {
            Vector3 l_currentLocalPosition = transform.localPosition;
            if (m_lastFrameLocalPosition != l_currentLocalPosition && m_positionChangedCallback != null)
            {
                m_positionChangedCallback.Invoke(this);
            }
            m_lastFrameLocalPosition = l_currentLocalPosition;
        }

        private void RefreshLinks()
        {
            if (m_lastFrameSelection.Get())
            {
                NavigationNodeEditorComponentLinks.NavigationNodeEditorComponentLink.Clear();
                for (int i = 0; i < m_navigationGraphEditorReference.NavigationGraphAsset.NavigationLinks.Count; i++)
                {
                    SerializedNavigationLink l_serializedNavigationLink = m_navigationGraphEditorReference.NavigationGraphAsset.NavigationLinks[i];
                    if (l_serializedNavigationLink.StartNode == m_serializedNavigationNodeId)
                    {
                        NavigationNodeEditorComponentLinks.NavigationNodeEditorComponentLink.Add(NavigationNodeEditorComponentLink.New(l_serializedNavigationLink.EndNode, OnNavigationNodeLinkRemoved));
                    }
                }
            }
        }
    }

    [Serializable]
    public struct NavigationNodeEditorComponentLinks
    {
        public static NavigationNodeEditorComponentLinks New()
        {
            NavigationNodeEditorComponentLinks thiz = new NavigationNodeEditorComponentLinks();
            thiz.NavigationNodeEditorComponentLink = new List<NavigationNodeEditorComponentLink>();
            return thiz;
        }
        public List<NavigationNodeEditorComponentLink> NavigationNodeEditorComponentLink;
    }

    [Serializable]
    public struct NavigationNodeEditorComponentLink : IEquatable<NavigationNodeEditorComponentLink>
    {
        public static NavigationNodeEditorComponentLink New(int p_targetNodeID, Action<NavigationNodeEditorComponentLink> p_onLinkRemovedCallback)
        {
            NavigationNodeEditorComponentLink thiz = new NavigationNodeEditorComponentLink();
            thiz.TargetNodeID = p_targetNodeID;
            thiz.m_onLinkRemovedCallback = p_onLinkRemovedCallback;
            return thiz;
        }

        public int TargetNodeID;
        private Action<NavigationNodeEditorComponentLink> m_onLinkRemovedCallback;

        [Button("Remove Link")]
        private void RemoveLink()
        {
            m_onLinkRemovedCallback(this);
        }

        public override bool Equals(object obj)
        {
            return obj is NavigationNodeEditorComponentLink link && Equals(link);
        }

        public bool Equals(NavigationNodeEditorComponentLink other)
        {
            return TargetNodeID == other.TargetNodeID;
        }

        public override int GetHashCode()
        {
            return -849401503 + TargetNodeID.GetHashCode();
        }
    }

}

public class BoolVariable
{
    public static BoolVariable New(bool p_initialValue, Action<bool> p_onSetToTrue, Action<bool> p_onSetToFalse)
    {
        BoolVariable l_boolVariable = new BoolVariable();
        l_boolVariable.m_onSetToTrue = p_onSetToTrue;
        l_boolVariable.m_onSetToFalse = p_onSetToFalse;
        l_boolVariable.m_value = !p_initialValue;
        l_boolVariable.Set(p_initialValue);
        return l_boolVariable;
    }

    private Action<bool> m_onSetToTrue;
    private Action<bool> m_onSetToFalse;
    private bool m_value;
    public void Set(bool p_value)
    {
        if (m_value && !p_value)
        {
            m_value = p_value;
            if (m_onSetToFalse != null) { m_onSetToFalse(m_value); }
        }
        else if (!m_value && p_value)
        {
            m_value = p_value;
            if (m_onSetToTrue != null) { m_onSetToTrue(m_value); }
        }
        else
        {
            m_value = p_value;
        }
    }

    public bool Get()
    {
        return m_value;
    }
}


public static class GizmosHelper
{
    public static void DrawArrow(Vector3 start, Vector3 end, float branchLength = 1.0f)
    {
        Quaternion l_globalOrientation = Quaternion.identity;
        // Quaternion.FromToRotation(end - start, Vector3.up);

        Quaternion l_leftBranch = l_globalOrientation * Quaternion.Euler(0.0f, 20.0f, 0.0f);
        Quaternion l_rightBranch = l_globalOrientation * Quaternion.Euler(0.0f, -20.0f, 0.0f);

        Gizmos.DrawLine(start, end);



        Gizmos.DrawLine(end, end + (l_leftBranch * (start - end).normalized * branchLength));
        Gizmos.DrawLine(end, end + (l_rightBranch * (start - end).normalized * branchLength));
    }
}

#endif

