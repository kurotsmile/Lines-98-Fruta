using Carrot;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Game_Shop : MonoBehaviour
{
    [Header("obj Main")]
    public Game_Handle game;
    public int index_buy_category_icon;

    [Header("Obj icon")]
    private string s_id_category_buy_temp = "";
    private string s_data_json_icon_category_offline = "";
    private Carrot_Box box_category = null;
    private Carrot_Box box_icon = null;
    private string s_name_categroy_temp = "";
    private Sprite[] list_icon_temp;
    private int length_icon = 6;
    public void On_load()
    {
        if (this.game.carrot.is_offline())
        {
            this.s_data_json_icon_category_offline = PlayerPrefs.GetString("s_data_json_icon_category_offline");
        }
    }

    public void Show()
    {
        this.List_category_icon();
    }

    private void List_category_icon()
    {
        if (this.s_data_json_icon_category_offline == "")
        {
            this.game.carrot.show_loading();
            StructuredQuery q = new("icon_category");
            q.Set_limit(20);
            this.game.carrot.server.Get_doc(q.ToJson(), Act_list_category_icon_done, Act_server_fail);
        }
        else
        {
            this.Act_load_list_category_icon(this.s_data_json_icon_category_offline);
        }
    }

    private void Act_list_category_icon_done(string s_data)
    {
        this.s_data_json_icon_category_offline = s_data;
        PlayerPrefs.SetString("s_data_json_icon_category_offline", s_data);
        this.Act_load_list_category_icon(s_data);
    }

    private void Act_server_fail(string s_error)
    {
        this.game.carrot.hide_loading();
        this.game.carrot.Show_msg("Error", s_error, Msg_Icon.Error);
    }

    private void Act_load_list_category_icon(string s_data)
    {
        this.game.carrot.hide_loading();
        Fire_Collection fc = new(s_data);

        if (!fc.is_null)
        {
            if (this.box_category != null) this.box_category.close();
            this.box_category = this.game.carrot.Create_Box();
            this.box_category.set_icon(this.game.carrot.icon_carrot_all_category);
            this.box_category.set_title("Bundle of object styles");

            for (int i = 0; i < fc.fire_document.Length; i++)
            {
                string s_status_buy = "free";
                IDictionary icon_data = fc.fire_document[i].Get_IDictionary();
                Carrot_Box_Item item_cat = this.box_category.create_item("item_icon");
                item_cat.set_icon(this.game.carrot.icon_carrot_download);

                if (icon_data["buy"] != null) s_status_buy = icon_data["buy"].ToString();

                if (icon_data["key"] != null)
                {
                    string s_key_cat = icon_data["key"].ToString();
                    item_cat.set_title(s_key_cat);
                    item_cat.set_tip(s_key_cat);

                    if (s_status_buy != "free")
                    {
                        if (PlayerPrefs.GetInt("is_buy_category_icon_" + s_key_cat, 0) == 1) s_status_buy = "free";

                        if (s_status_buy != "free")
                        {
                            if (PlayerPrefs.GetInt("is_buy_0", 0) == 1) s_status_buy = "free";
                        }
                    }

                    if (s_status_buy != "free")
                    {
                        Carrot_Box_Btn_Item btn_buy = item_cat.create_item();
                        btn_buy.set_icon(this.game.carrot.icon_carrot_buy);
                        btn_buy.set_color(this.game.carrot.color_highlight);
                        Destroy(btn_buy.GetComponent<Button>());

                        if (this.game.carrot.model_app == ModelApp.Publish)
                            item_cat.set_act(() => this.Act_buy_category(s_key_cat));
                        else
                            item_cat.set_act(() => this.View_list_icon_by_category_key(s_key_cat));
                    }
                    else
                    {
                        item_cat.set_act(() => this.View_list_icon_by_category_key(s_key_cat));
                    }
                }
            };
            this.box_category.update_color_table_row(); ;
        }
    }

    private void Act_buy_category(string s_id_category)
    {
        this.s_id_category_buy_temp = s_id_category;
        this.game.carrot.shop.buy_product(this.index_buy_category_icon);
    }

    public void Act_buy_category_success()
    {
        if (this.s_id_category_buy_temp != "")
        {
            this.View_list_icon_by_category_key(this.s_id_category_buy_temp);
            PlayerPrefs.SetInt("is_buy_category_icon_" + this.s_id_category_buy_temp, 1);
            this.s_id_category_buy_temp = "";
        }
    }

    private void View_list_icon_by_category_key(string s_key)
    {
        this.s_name_categroy_temp = s_key;
        this.game.carrot.show_loading();
        StructuredQuery q = new("icon");
        q.Add_where("category", Query_OP.EQUAL, s_key);
        q.Set_limit(this.length_icon);
        this.game.carrot.server.Get_doc(q.ToJson(), Act_view_list_icon_by_category_key_done,Act_server_fail);
    }

    private void Act_view_list_icon_by_category_key_done(string s_data)
    {
        Fire_Collection fc = new(s_data);

        if (!fc.is_null)
        {
            this.list_icon_temp = new Sprite[6];
            this.game.carrot.hide_loading();
            this.box_icon = this.game.carrot.show_grid();
            this.box_icon.set_title(this.s_name_categroy_temp);
            this.box_icon.set_icon(this.game.carrot.icon_carrot_all_category);
            this.box_icon.set_item_size(new Vector2(120, 120));

            for (int i = 0; i < fc.fire_document.Length; i++)
            {
                IDictionary icon_data = (IDictionary)fc.fire_document[i].Get_IDictionary();
                Carrot_Box_Item item_icon=this.box_icon.create_item("icon_item_" + i);
                Sprite sp_icon = this.game.carrot.get_tool().get_sprite_to_playerPrefs(icon_data["id"].ToString());
                if (sp_icon != null)
                {
                    item_icon.set_icon_white(sp_icon);
                    this.list_icon_temp[i]= sp_icon;
                }
                else
                {
                    if (icon_data["icon"] != null) this.game.carrot.get_img_and_save_playerPrefs(icon_data["icon"].ToString(), item_icon.img_icon, icon_data["id"].ToString());
                }
                item_icon.set_act(() => this.Select_icon_for_table());
            }
        }
    }

    private void Select_icon_for_table()
    {
        this.game.table_fruta.Change_list_icon_fruta(this.list_icon_temp);
        this.game.carrot.play_sound_click();
        if (box_category != null) box_category.close();
        if(box_icon!=null) box_icon.close();
    }

}
