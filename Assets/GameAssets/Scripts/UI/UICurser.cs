using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICurser : MonoBehaviour
{
    private Camera cam;
    public GameObject obj;
    public Image innerCircle;
    private float m_aim_size = 1;
    public float recall_aim_offset = 0;
    private Vector3 starting_aim_scale;
    private Vector3 starting_inner_circle_size;
    public Text reload_text;
    private int current_weapon_accuracy;
    private bool is_player_moving = false;
    private Vector3 moving_offset = Vector3.zero;

    void Start()
    {
        cam = Camera.main;
        starting_aim_scale = obj.transform.localScale;
        starting_inner_circle_size = innerCircle.transform.localScale;
        Cursor.visible = false;
    }

    void OnGUI()
    {
        Cursor.visible = false;
        Vector3 point = new Vector3();
        Event currentEvent = Event.current;
        Vector2 mousePos = new Vector2();

        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;

        point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane*100));

        obj.transform.position = point;


        if(is_player_moving)
        {
            //moving_offset = Vector3.Lerp(moving_offset,starting_aim_scale *1.2f,1);
        }
        else
        {
           // moving_offset = Vector3.Lerp(moving_offset,Vector3.zero, 1);
        }


        if(reload_text.text == "R" || current_weapon_accuracy ==-1)
        {
            obj.transform.localScale = starting_aim_scale + starting_aim_scale*0.7f;
            innerCircle.enabled = false;
        }
        else
        {
            //innerCircle.enabled = true;
            //obj.transform.localScale = starting_aim_scale *(100 - current_weapon_accuracy) *0.025f*m_aim_size + starting_aim_scale + moving_offset;
            //innerCircle.transform.localScale = starting_inner_circle_size + starting_inner_circle_size*recall_aim_offset;
        }
        
    }


    public void Set_aim_Size(float aim_size)
    {
        if(aim_size < 0.3f)
        {
            aim_size = 0.3f;
        }
        this.m_aim_size = aim_size*1;
    }

    public void set_is_player_moving(bool moving)
    {
        is_player_moving = moving;
    }

    public IEnumerator recall_aim_size_change(float recall_size,float duration)
    {
        recall_aim_offset = recall_size;
        yield return new WaitForSeconds(duration);
        recall_aim_offset = 0;
        //float change_rate = recall_size / duration;
        //while (recall_aim_offset < recall_size)
        //{
        //    yield return new WaitForSeconds(Time.deltaTime);
        //    recall_aim_offset = Mathf.Lerp(recall_aim_offset, recall_size, 0.2f);
        //}
        //while (recall_aim_offset > 0)
        //{
        //    yield return new WaitForSeconds(Time.deltaTime);
        //    recall_aim_offset = Mathf.Lerp(recall_aim_offset, 0, 0.2f);
        //}
        //recall_aim_offset = 0;

    }

    public void set_text(string text)
    {
        reload_text.text = text;
    }

    public void set_current_weapon_accuracy(int accuracy)
    {
        current_weapon_accuracy = accuracy;
    }
}
