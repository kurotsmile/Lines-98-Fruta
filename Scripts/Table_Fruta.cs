using Carrot;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class F
{
    public int index_col;
    public int index_row;
    public bool is_can_move = true;

    public F(int x,int y)
    {
        this.index_col = x;
        this.index_row = y;
    }
}

public class Table_Fruta : MonoBehaviour
{
    public GameObject row_fruta_prefab;
    public Sprite[] sp_icon_fruta;
    public Text txt_scores;
    public Text txt_timer;
    public Slider slider_timer;
    private List<Row_Fruta> list_row;
    private List<Furta> list_prepare_big_fruta;
    private int count_fruta_open = 0;
    private int num_row = 7;
    private int num_col = 15;
    private int num_show = 3;
    private int scores = 0;
    private float timer = 0f;
    private Furta f_select;
    private Furta f_change;
    private Game_Handle game;
    private Carrot_Window_Msg msg = null;
    private bool is_play = false;

    public void load(Game_Handle g)
    {
        this.game = g;
        this.load_data();
        prepare_random_fruta(20, true);
    }

    private void load_data()
    {
        this.list_row = new List<Row_Fruta>();
        this.list_prepare_big_fruta = new List<Furta>();
        for (int i = 0; i < this.num_row; i++)
        {
            GameObject obj_fura = Instantiate(this.row_fruta_prefab);
            obj_fura.name = "Row_Fruta_" + i;
            obj_fura.transform.SetParent(this.transform);
            obj_fura.transform.localPosition = new Vector3(obj_fura.transform.localPosition.x, obj_fura.transform.localPosition.y, obj_fura.transform.localPosition.z);
            obj_fura.transform.localScale = new Vector3(1f, 1f, 1f);
            obj_fura.transform.localRotation = Quaternion.identity;
            Row_Fruta row = obj_fura.GetComponent<Row_Fruta>();
            row.index_row = i;
            row.load(this.num_col,this);
            list_row.Add(row);

        }
        prepare_random_fruta(this.num_show, true);
        this.show_random_fruta();
    }

    public void btn_reset()
    {
        this.is_play = false;
        this.game.carrot.play_sound_click();
        this.msg = this.game.carrot.show_msg("Play again", "Are you sure to reset the game and start over?", act_reset_yes, act_reset_no);
        this.msg.UI.get_list_btn()[2].gameObject.SetActive(false);
    }

    private void act_reset_no()
    {
        this.is_play = true;
        this.game.carrot.play_sound_click();
        this.msg.close();
    }

    private void act_reset_yes()
    {
        this.is_play = true;
        this.game.carrot.play_sound_click();
        this.reset();
        this.msg.close();
    }

    public void reset()
    {
        this.slider_timer.value = 500;
        this.is_play = true;
        this.count_fruta_open = 0;
        for (int i = 0; i < this.num_row; i++) Destroy(this.list_row[i].gameObject);
        this.load_data();
        this.game.update_graphic();
    }

    public void show_random_fruta()
    {
        if (this.list_prepare_big_fruta.Count > 0)
        {
            for (int i = 0; i < this.list_prepare_big_fruta.Count; i++)
            {
                Furta f_check = this.list_prepare_big_fruta[i];
                f_check.open();
                f_check.start_effect_big();
                this.check_diagonal(f_check);
            }
        }

        this.list_prepare_big_fruta = new List<Furta>();
        prepare_random_fruta(this.num_show, false);
    }

    private void prepare_random_fruta(int num_fruta, bool is_big, bool is_effect = false)
    {
        if (this.count_fruta_open < (this.num_row * this.num_col))
            for (int i = 0; i < num_fruta; i++)
            {
                int index_icon_type = UnityEngine.Random.Range(0, this.sp_icon_fruta.Length);
                Furta f = this.random_fruta(is_big, this.sp_icon_fruta[index_icon_type], index_icon_type);
                if (f != null)
                {
                    if (is_effect) this.game.create_effect(f.transform.position, 2);
                    if (is_big == false)
                    {
                        f.index_prepare = i;
                        f.start_effect_prepare();
                        this.list_prepare_big_fruta.Add(f);
                    }
                }
            }
        else
        {
            this.game.show_gameover();
        }
    }

    private Furta random_fruta(bool is_big, Sprite sp, int type)
    {
        if (this.count_fruta_open < (this.num_row * this.num_col))
        {
            int index_rand_row = UnityEngine.Random.Range(0, this.list_row.Count);
            Furta f_check = this.list_row[index_rand_row].show(is_big, sp, type);
            if (f_check == null)
            {
                return this.random_fruta(is_big, sp, type);
            }
            else
            {
                this.count_fruta_open++;
                return f_check;
            }
        }
        else
        {
            return null;
        }
    }

    public void select_fruta(Furta f)
    {
        if (this.f_select == null)
        {
            if (f.status == 0 || f.status == 1)
            {
                this.game.play_sound(1);
                return;
            }
            f.select();
            this.f_select = f;
            this.game.carrot.play_sound_click();
        }
        else
        {
            this.game.carrot.play_sound_click();
            if (this.f_select.index_col == f.index_col && this.f_select.index_row == f.index_row)
            {
                f.unSelect();
                this.f_select = null;
            }
            else if (f.status == 2)
            {
                this.f_select.unSelect();
                f.select();
                this.f_select = f;
            }
            else
            {
                if (this.check_move(this.f_select,f))
                {
                    this.f_change = f;
                    this.f_change.set_img(this.f_select.img_big.sprite);
                    this.f_change.type = this.f_select.type;
                    if (this.f_change.index_prepare != -1)
                    {
                        this.list_prepare_big_fruta.RemoveAt(this.f_change.index_prepare);
                        this.f_change.index_prepare = -1;
                    }
                    this.f_change.open();
                    this.f_select.close();
                    this.f_select = null;
                    this.game.carrot.delay_function(0.3f, this.act_check_true_and_create_fruta);
                }
                else
                {
                    f.start_effect_move_false();
                    this.game.play_sound(1);
                }
            }
        }
    }

    private void act_check_true_and_create_fruta()
    {
        this.check_diagonal(this.f_change);
        this.show_random_fruta();
    }

    private bool check_move(Furta f_start, Furta f_end)
    {
        bool check_f_start = this.check_furta_block_move(f_start);
        bool check_f_end = this.check_furta_block_move(f_end);
        bool check_f_block_col = this.check_f_block_col_all_row(f_start.index_col, f_end.index_col);
        bool check_f_block_row = this.check_f_block_row_all_col(f_start.index_row, f_end.index_row);

        if (check_f_start == true && check_f_end == true&& check_f_block_col==true&& check_f_block_row==true)
            return true;
        else
            return false;
    }

    private bool check_f_block_row_all_col(int index_row_start, int index_row_end)
    {
        if (index_row_start == index_row_end) return true;
        int row_start;
        int row_end;
        if (index_row_start > index_row_end)
        {
            row_start = index_row_end;
            row_end = index_row_start;
        }
        else
        {
            row_start = index_row_start;
            row_end = index_row_end;
        }

        for (int i = row_start; i < row_end; i++) if (this.list_row[i].check_full_block() == true) return false;

        return true;
    }

    private bool check_f_block_col_all_row(int index_col_start, int index_col_end)
    {
        if (index_col_start == index_col_end) return true;//cung cot
        int col_start;
        int col_end;
        if (index_col_start > index_col_end)
        {
            col_start = index_col_end;
            col_end = index_col_start;
        }
        else
        {
            col_start = index_col_start;
            col_end = index_col_end;
        }

        for (int i = col_start; i < col_end; i++) if (ckeck_is_col_not_full(i) == false) return false;

        return true;
    }

    private bool ckeck_is_col_not_full(int index_col)
    {
        int count_block=0;
        for(int i = 0; i < this.num_row; i++)
        {
            if (this.get_Furta(index_col, i).status == 2) count_block++;
        }
        if (count_block >= this.num_row)
            return false;
        else
            return true;
    }

    private bool check_furta_block_move(Furta f_start)
    {
        F f_top = new F(f_start.index_col, f_start.index_row-1);
        F f_bottom = new F(f_start.index_col, f_start.index_row+1);
        F f_left = new F(f_start.index_col-1, f_start.index_row);
        F f_right = new F(f_start.index_col+1, f_start.index_row);

        if (f_top.index_row < 0)
            f_top.is_can_move = false;
        else
            if (this.get_Furta(f_top.index_col, f_top.index_row).status == 2) f_top.is_can_move = false;

        if (f_bottom.index_row >= this.num_row) 
            f_bottom.is_can_move = false;
        else
            if (this.get_Furta(f_bottom.index_col, f_bottom.index_row).status == 2) f_bottom.is_can_move = false;

        if (f_left.index_col < 0) 
            f_left.is_can_move = false;
        else
            if (this.get_Furta(f_left.index_col, f_left.index_row).status == 2) f_left.is_can_move = false;

        if (f_right.index_col >= this.num_col) 
            f_right.is_can_move = false;
        else
            if (this.get_Furta(f_right.index_col, f_right.index_row).status == 2) f_right.is_can_move = false;

        if (f_top.is_can_move == false && f_bottom.is_can_move == false && f_left.is_can_move == false && f_right.is_can_move == false)
            return false;
        else
            return true;
    }

    private Furta get_Furta(int x,int y)
    {
        return this.list_row[y].get_furta_buy_index_col(x);
    }

    public void subtract_fruta(int num_fruta)
    {
        this.count_fruta_open = this.count_fruta_open - num_fruta;
        this.add_scores(num_fruta);
    }

    private void add_scores(int num_scores)
    {
        this.scores += num_scores;
        this.txt_scores.text = this.scores.ToString();
        this.slider_timer.value = 500;
    }

    public int get_scores()
    {
        return this.scores;
    }

    private void Update()
    {
        if (this.is_play)
        {
            this.timer += 1f * Time.deltaTime;
            this.txt_timer.text = FormatTime(this.timer);
            this.slider_timer.value = this.slider_timer.value - 0.1f;
            if (this.slider_timer.value <= 0)
            {
                this.slider_timer.value = 500;
                prepare_random_fruta(this.num_show, true, true);
                this.game.play_sound(3);
            }
        }
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        int milliseconds = (int)(1000 * (time - minutes * 60 - seconds));
        return string.Format("{0:00}:{1:00}:{2:0}", minutes, seconds, milliseconds);
    }

    public void stop()
    {
        this.is_play = false;
    }

    public bool get_status_play()
    {
        return this.is_play;
    }

    public float get_timer()
    {
        return this.timer;
    }

    private void check_diagonal(Furta f_check)
    {
        if (f_check.type == -1) return;
        List<Furta> list_furta = new List<Furta>();

        List<Furta> list_top_right_to_bottom_left = new List<Furta>();
        List<Furta> list_top_right = new List<Furta>();
        List<Furta> list_bottom_left = new List<Furta>();
        for (int i = 0; i < this.num_row; i++)
        {
            for (int j = 0; j < this.num_col; j++)
            {
                if (i + j == f_check.index_col + f_check.index_row)
                {
                    if (i < f_check.index_row)
                    {
                        if (this.list_row[i].get_furta_buy_index_col(j).type == f_check.type && this.list_row[i].get_furta_buy_index_col(j).status == 2)
                            list_top_right.Add(this.list_row[i].get_furta_buy_index_col(j));
                        else
                            list_top_right = new List<Furta>();
                    }

                    if (i > f_check.index_row && (this.list_row[i].get_furta_buy_index_col(j).type == f_check.type) && this.list_row[i].get_furta_buy_index_col(j).status == 2) list_bottom_left.Add(this.list_row[i].get_furta_buy_index_col(j));
                }
            }
        }
        list_top_right_to_bottom_left.AddRange(list_top_right);
        list_top_right_to_bottom_left.AddRange(list_bottom_left);

        List<Furta> list_bottom_right_top_left = new List<Furta>();
        List<Furta> list_bottom_right = new List<Furta>();
        List<Furta> list_top_left = new List<Furta>();
        for (int i = this.num_row - 1; i >= 0; i--)
        {
            for (int j = this.num_col - 1; j >= 0; j--)
            {
                if (i - j == f_check.index_row - f_check.index_col)
                {
                    if (i < f_check.index_row && (this.list_row[i].get_furta_buy_index_col(j).type == f_check.type) && this.list_row[i].get_furta_buy_index_col(j).status == 2) 
                        list_top_left.Add(this.list_row[i].get_furta_buy_index_col(j));

                    if (i > f_check.index_row)
                    {
                        if (this.list_row[i].get_furta_buy_index_col(j).type == f_check.type&& this.list_row[i].get_furta_buy_index_col(j).status==2)
                            list_bottom_right.Add(this.list_row[i].get_furta_buy_index_col(j));
                        else
                            list_bottom_right = new List<Furta>();
                    }
                }
            }
        }
        list_bottom_right_top_left.AddRange(list_bottom_right);
        list_bottom_right_top_left.AddRange(list_top_left);

        List<Furta> list_top_to_bottom = new List<Furta>();
        List<Furta> list_top = new List<Furta>();
        List<Furta> list_bottom = new List<Furta>();
        for (int index_start = f_check.index_row-1; index_start >= 0; index_start--)
        {
            if (this.list_row[index_start].get_furta_buy_index_col(f_check.index_col).type == f_check.type && this.list_row[index_start].get_furta_buy_index_col(f_check.index_col).status == 2)
                list_top.Add(this.list_row[index_start].get_furta_buy_index_col(f_check.index_col));
            else
                break;
        }

        for (int index_end = f_check.index_row+1; index_end < this.list_row.Count; index_end++)
        {
            if (this.list_row[index_end].get_furta_buy_index_col(f_check.index_col).type == f_check.type && this.list_row[index_end].get_furta_buy_index_col(f_check.index_col).status == 2)
                list_bottom.Add(this.list_row[index_end].get_furta_buy_index_col(f_check.index_col));
            else
                break;
        }
        list_top_to_bottom.AddRange(list_top);
        list_top_to_bottom.AddRange(list_bottom);

        List<Furta> list_left_to_right = new List<Furta>();
        List<Furta> list_left = new List<Furta>();
        List<Furta> list_right = new List<Furta>();
        for (int i_right= f_check.index_col+1; i_right < num_col; i_right++)
        {
            if (this.list_row[f_check.index_row].get_furta_buy_index_col(i_right).type == f_check.type && this.list_row[f_check.index_row].get_furta_buy_index_col(i_right).status == 2)
                list_right.Add(this.list_row[f_check.index_row].get_furta_buy_index_col(i_right));
            else break;
        }

        for (int i_left = f_check.index_col-1; i_left >= 0; i_left--)
        {
            if (this.list_row[f_check.index_row].get_furta_buy_index_col(i_left).type == f_check.type && this.list_row[f_check.index_row].get_furta_buy_index_col(i_left).status == 2)
                list_right.Add(this.list_row[f_check.index_row].get_furta_buy_index_col(i_left));
            else break;
        }
        list_left_to_right.AddRange(list_left);
        list_left_to_right.AddRange(list_right);

        if (list_bottom_right_top_left.Count>=4) list_furta.AddRange(list_bottom_right_top_left);
        if(list_top_right_to_bottom_left.Count>=4)  list_furta.AddRange(list_top_right_to_bottom_left);
        if(list_top_to_bottom.Count>=4)  list_furta.AddRange(list_top_to_bottom);
        if(list_left_to_right.Count>=4)  list_furta.AddRange(list_left_to_right);
        list_furta.Add(f_check);

        if (list_furta.Count >= 5)
        {
            for(int i=0;i< list_furta.Count;i++)
            {
                if (list_furta[i].type == 1 || list_furta[i].type == 4)
                    this.game.create_effect(list_furta[i].transform.position, 1);
                else
                    this.game.create_effect(list_furta[i].transform.position, 0);
                list_furta[i].close();
            }
            this.game.table_fruta.subtract_fruta(list_furta.Count);
            this.game.play_sound(0);
        }
    }

    public List<Row_Fruta> get_list_row()
    {
        return this.list_row;
    }

    public Furta get_f_select()
    {
        return this.f_change;
    }
}
