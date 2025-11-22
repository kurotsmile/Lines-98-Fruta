using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row_Fruta : MonoBehaviour
{
    public GameObject furta_prefab;
    public List<Furta> list_fruta;
    private int num_col;
    public int index_row;
    public void load(int num_col,Table_Fruta tf)
    {
        this.num_col = num_col;
        this.list_fruta = new List<Furta>();
        for(int i = 0; i < num_col; i++)
        {
            GameObject obj_fura = Instantiate(this.furta_prefab);
            obj_fura.name = "Fruta_" + i;
            obj_fura.transform.SetParent(this.transform);
            obj_fura.transform.localScale = new Vector3(1f, 1f, 1f);
            obj_fura.transform.localRotation = Quaternion.identity;
            Furta f = obj_fura.GetComponent<Furta>();
            f.index_row = index_row;
            f.index_col = i;
            this.list_fruta.Add(f);
        }
    }

    public Furta show(bool is_big,Sprite sp,int type)
    {
        if (this.check_full_create())
            return null;
        else
        {
            int index_show = Random.Range(0, this.list_fruta.Count);
            if (this.list_fruta[index_show].status == 0)
            {
                if (is_big)
                    this.list_fruta[index_show].open();
                else
                    this.list_fruta[index_show].open_small();

                this.list_fruta[index_show].type = type;
                this.list_fruta[index_show].set_img(sp);
                return this.list_fruta[index_show];
            }
            else
            {
                return this.show(is_big,sp,type);
            }   
        }
    }

    private bool check_full_create()
    {
        int count_full = 0;
        for (int i = 0; i < this.num_col; i++) if (this.list_fruta[i].status == 1|| this.list_fruta[i].status == 2) count_full++;

        if (count_full >= this.list_fruta.Count)
            return true;
        else
            return false;
    }

    public bool check_full_block()
    {
        for (int i = 0; i < this.num_col; i++) if (this.list_fruta[i].status != 2) return false;
        return true;
    }

    public Furta get_furta_buy_index_col(int index_col)
    {
        return this.list_fruta[index_col];
    }
}
