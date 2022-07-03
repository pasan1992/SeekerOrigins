using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMateriaProperty : MonoBehaviour
{
    // Start is called before the first frame update
    public void SetMaterialEmmision(bool enabled, Material mat)
    {
        if(enabled)
        {
            mat.EnableKeyword("_EMISSION");
        }
        else{
            mat.DisableKeyword("_EMISSION");
        }
        
    }
}
