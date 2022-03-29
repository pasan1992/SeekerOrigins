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

    // Update is called once per frame
    private void Start()
    {
        if(m_image)
        {
            m_image.enabled = false;
        }
        
    }

    void Update()
    {
        if (target)
        {
            transform.position = Camera.main.WorldToScreenPoint(target.position) + m_offset;
        }
    }

    public void setMainImageStatus(bool enabled)
    {
        m_image.enabled = enabled;
    }
}
