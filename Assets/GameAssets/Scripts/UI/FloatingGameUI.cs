using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingGameUI : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target;
    public Vector3 m_offset;
    public Image m_image;
    public float smoothness;

    private Vector3 m_targetPosition;

    // Update is called once per frame
    private void Start()
    {
        if(m_image)
        {
            m_image.enabled = false;
        }
        if (smoothness ==0)
        {
            smoothness = 1;
        }
        
    }

    void Update()
    {

        if (target)
        {
            if (Vector3.Distance(m_targetPosition, target.position) > 0.1f)
            {
                m_targetPosition = target.position;
            }

            var target_screen_pos = Camera.main.WorldToScreenPoint(m_targetPosition) + m_offset;
            transform.position= Vector3.Lerp(transform.position, target_screen_pos, smoothness);
        }
    }

    public void setMainImageStatus(bool enabled)
    {
        m_image.enabled = enabled;
    }
}
