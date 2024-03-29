using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Furta : MonoBehaviour
{
    public Image img_big;
    public Image img_small;
    public Image img_Effect_move_false;
    public int status = 0;
    public int type = -1;
    public Animator anim;
    public int index_col;
    public int index_row;
    public int index_prepare=-1;

    public void click()
    {
        GameObject.Find("Game").GetComponent<Game_Handle>().table_fruta.select_fruta(this);
    }

    public void select()
    {
        anim.enabled = true;
        this.anim.Play("Fruta_select");
        this.status = 3;
    }

    public void unSelect()
    {
        anim.enabled = false;
        this.img_big.transform.localScale = new Vector3(1f, 1f, 1f);
        this.status = 2;
    }

    public void close()
    {
        anim.enabled = false;
        this.status = 0;
        this.type = -1;
        this.index_prepare = -1;
        this.img_big.transform.localScale = new Vector3(1f, 1f, 1f);
        this.img_small.gameObject.SetActive(false);
        this.img_big.gameObject.SetActive(false);
        this.img_Effect_move_false.gameObject.SetActive(false);
    }

    public void open()
    {
        this.status = 2;
        this.index_prepare = -1;
        this.img_small.gameObject.SetActive(false);
        this.img_big.gameObject.SetActive(true);
    }

    public void open_small()
    {
        this.status = 1;
        this.img_small.gameObject.SetActive(true);
        this.img_big.gameObject.SetActive(false);
    }

    public void set_img(Sprite sp)
    {
        this.img_big.sprite = sp;
        this.img_small.sprite = sp;
    }

    public void start_effect_big()
    {
        this.anim.enabled = true;
        this.anim.Play("Fruta_effect_big");
        this.img_big.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
    }

    public void start_effect_move_false()
    {
        this.anim.enabled = true;
        this.anim.Play("Fruta_effect_move_false");
        this.img_Effect_move_false.gameObject.SetActive(true);
    }

    public void start_effect_prepare()
    {
        this.anim.enabled = true;
        this.anim.Play("Fruta_effect_prepare");
    }

    public void stop_effect_prepare()
    {
        this.anim.enabled = false;
        this.img_small.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void stop_effect_big()
    {
        this.anim.enabled = false;
        this.img_big.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void stop_effect_move_false()
    {
        this.anim.enabled = false;
        this.img_Effect_move_false.gameObject.SetActive(false);
    }

    public Vector2 ToVector2()
    {
        return new Vector2(index_col, index_row);
    }
}
