using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectUI : MonoBehaviour
{
    // Start is called before the first frame update
    public SpriteRenderer MiniMapIcon;
    public Text NameTag;
    public GameObject InfoPlate;
    public void OnDistryedObject()
    {
        MiniMapIcon.gameObject.SetActive(false);
        NameTag.gameObject.SetActive(false);
        InfoPlate.gameObject.SetActive(false);
    }
}
