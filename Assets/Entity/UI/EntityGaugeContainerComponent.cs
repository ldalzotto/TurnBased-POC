using _ActionPoint;
using _Health;
using _RuntimeObject;
using UnityEngine;

namespace _Entity
{
    /// <summary>
    /// This component manages the layout of the <see cref="Entity"/> GUI. He is the root component of <see cref="Entity"/> UI.
    /// All GUI related to an <see cref="Entity"/> must be instanciated from this component.
    /// </summary>
    public class EntityGaugeContainerComponent : RuntimeComponent
    {
        private ActionPointGUIComponent m_instanciatedActionPointGUIComponent;
        private HealthGUIComponent m_instanciatedHealthGUIComponent;
        public void InstanciateActionPointGUI(ActionPointGUIComponent p_ActionPointGUIComponentPrefab)
        {
            m_instanciatedActionPointGUIComponent = GameObject.Instantiate(p_ActionPointGUIComponentPrefab, transform);
        }


        public void InstanciateHealthGUI(HealthGUIComponent p_healthGUIComponentPrefab)
        {
            m_instanciatedHealthGUIComponent = GameObject.Instantiate(p_healthGUIComponentPrefab, transform);
        }

        private void CalculateObjectOrder()
        {
            if (m_instanciatedHealthGUIComponent != null)
            {
                m_instanciatedHealthGUIComponent.transform.SetSiblingIndex(0);
            }

            if (m_instanciatedActionPointGUIComponent != null)
            {
                m_instanciatedActionPointGUIComponent.transform.SetSiblingIndex(1);
            }
        }

    }
}

