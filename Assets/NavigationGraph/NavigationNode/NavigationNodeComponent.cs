using _RuntimeObject;
using UnityEngine;

namespace _NavigationGraph
{
    /// <summary>
    /// Game representation of <see cref="_NavigationGraph.NavigationNode"/>.
    /// </summary>
    public class NavigationNodeComponent : RuntimeComponent
    {
        public NavigationNode AssociatedNavigationNode;

        public static GameObject instanciateFromPrefab(Transform p_parent, NavigationNode p_associatedNavigationNode, GameObject p_navigationNodeComponentPrefab)
        {
            GameObject l_instanciatedGameObject = GameObject.Instantiate(p_navigationNodeComponentPrefab);
            l_instanciatedGameObject.transform.SetParent(p_parent);
            l_instanciatedGameObject.transform.localPosition = p_associatedNavigationNode.LocalPosition;
            l_instanciatedGameObject.transform.localRotation = Quaternion.identity;
            NavigationNodeComponent l_navigationNodeComponent = l_instanciatedGameObject.GetComponent<NavigationNodeComponent>();
            if (l_navigationNodeComponent)
            {
                l_navigationNodeComponent.AssociatedNavigationNode = p_associatedNavigationNode;
            }
            return l_instanciatedGameObject;
        }

    }

}

