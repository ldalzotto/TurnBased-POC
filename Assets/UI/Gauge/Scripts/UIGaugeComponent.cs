using UnityEngine;
using UnityEngine.UI;

namespace _UI._Gauge
{
    public class UIGaugeComponent : MonoBehaviour
    {
        public Color EmptyColor;
        public Color FillColor;

        #region Component Dependencies
        private Image m_image;
        #endregion

        private void Awake()
        {
            m_image = GetComponent<Image>();
            m_image.material = new Material(m_image.material);

            m_image.material.SetColor("_FillColor", FillColor);
            m_image.material.SetColor("_EmptyColor", EmptyColor);
        }

        public void SetFillRate(float p_fillRate)
        {
            m_image.material.SetFloat("_FillRate", p_fillRate);
        }

    }

}
