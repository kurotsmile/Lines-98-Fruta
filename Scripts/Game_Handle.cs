using Carrot;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_Handle : MonoBehaviour
{
    [Header("Obj Main")]
    public Carrot.Carrot carrot;
    public Game_Shop shop;

    [Header("Obj Game")]
    public Table_Fruta table_fruta;
    public GameObject panel_gameplay;
    public GameObject panel_gamemain;
    public GameObject panel_gameover;
    public GameObject[] effect_prefab;
    public AudioSource[] sounds;

    [Header("GameOver")]
    public Text txt_gameover_scores;
    public Text txt_gameover_timer;
    public Text txt_gameover_hight_scores;
    public Text txt_gameover_hight_timer;
    public Text txt_main_hight_scores;
    public Text txt_main_hight_timer;

    private int hight_scores;
    private float hight_timer;

    void Start()
    {
        this.carrot.Load_Carrot(this.check_exit_app);
        this.carrot.shop.onCarrotPaySuccess += shop.OnPaySuccess;

        this.panel_gamemain.SetActive(true);
        this.panel_gameplay.SetActive(true);
        this.panel_gameover.SetActive(false);

        shop.On_load();
        table_fruta.load(this);

        this.update_graphic();
        this.update_and_show_hight_scores();
        this.update_and_show_hight_timer();

        this.carrot.game.load_bk_music(this.sounds[4]);
    }

    public void update_graphic()
    {
        this.carrot.delay_function(0.2f, act_update_graphic);
    }

    private void act_update_graphic()
    {
        this.table_fruta.GetComponent<VerticalLayoutGroup>().enabled = false;
        this.table_fruta.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
    }

    private void check_exit_app()
    {
        if (this.table_fruta.get_status_play())
        {
            this.btn_back_home();
            this.carrot.set_no_check_exit_app();
        }
        else if(this.panel_gameover.activeInHierarchy)
        {
            this.btn_back_home();
            this.carrot.set_no_check_exit_app();
        }
    }

    public void btn_play_now()
    {
        this.carrot.ads.show_ads_Interstitial();
        this.carrot.play_sound_click();
        this.panel_gamemain.SetActive(false);
        this.table_fruta.reset();
    }

    public void btn_show_setting()
    {
        this.carrot.ads.show_ads_Interstitial();
        Carrot_Box box_setting= this.carrot.Create_Setting();
        box_setting.set_act_before_closing(act_after_close_setting);
    }

    private void act_after_close_setting(List<string> item_change)
    {
        foreach(string s in item_change)
        {
            if(s== "list_bk_music") this.carrot.game.load_bk_music(this.sounds[4]);
        }
        this.carrot.ads.show_ads_Interstitial();
    }

    public void btn_gamereplay()
    {
        this.carrot.ads.show_ads_Interstitial();
        this.carrot.play_sound_click();
        this.panel_gameover.SetActive(false);
        this.table_fruta.reset();
    }

    public void btn_back_home()
    {
        this.carrot.ads.show_ads_Interstitial();
        this.table_fruta.stop();
        this.carrot.play_sound_click();
        this.panel_gamemain.SetActive(true);
        this.panel_gameover.SetActive(false);
    }

    public void show_gameover()
    {
        this.carrot.ads.show_ads_Interstitial();
        this.table_fruta.stop();
        int your_scores = this.table_fruta.get_scores();
        float your_timer = this.table_fruta.get_timer();
        if (your_scores > this.hight_scores)
        {
            PlayerPrefs.SetInt("hight_scores", your_scores);
            this.update_and_show_hight_scores();
        }

        if (your_timer > this.hight_timer)
        {
            PlayerPrefs.SetFloat("hight_timer", your_timer);
            this.update_and_show_hight_timer();
        }
        this.carrot.play_vibrate();
        this.carrot.game.update_scores_player(your_scores);
        this.txt_gameover_scores.text = this.table_fruta.get_scores().ToString();
        this.txt_gameover_timer.text = this.table_fruta.txt_timer.text;
        this.txt_gameover_hight_timer.text = this.table_fruta.FormatTime(this.table_fruta.get_timer());
        this.play_sound(2);
        this.panel_gameover.SetActive(true);
    }

    public void create_effect(Vector3 pos,int index_effect = 0)
    {
        GameObject effect_obj = Instantiate(this.effect_prefab[index_effect]);
        effect_obj.transform.SetParent(this.panel_gameplay.transform);
        effect_obj.transform.position = pos;
        effect_obj.transform.localScale = new Vector3(1f, 1f, 1f);
        Destroy(effect_obj, 1f);
    }

    public void play_sound(int index_sound=0)
    {
        if (this.carrot.get_status_sound()) this.sounds[index_sound].Play();
    }

    public void btn_user_carrot()
    {
        this.carrot.user.show_login();
    }

    public void btn_show_rate()
    {
        this.carrot.show_rate();
    }

    public void btn_show_top_player()
    {
        this.carrot.game.Show_List_Top_player();
    }

    private void update_and_show_hight_scores()
    {
        this.hight_scores = PlayerPrefs.GetInt("hight_scores", 0);
        this.txt_gameover_hight_scores.text = this.hight_scores.ToString();
        this.txt_main_hight_scores.text= this.hight_scores.ToString();
    }

    private void update_and_show_hight_timer()
    {
        this.hight_timer = PlayerPrefs.GetFloat("hight_timer", 0);
        this.txt_main_hight_timer.text = this.table_fruta.FormatTime(this.hight_timer);
        this.txt_gameover_hight_timer.text = this.table_fruta.FormatTime(this.hight_timer);
    }

    public void Btn_show_Shop()
    {
        carrot.ads.show_ads_Interstitial();
        carrot.play_sound_click();
        shop.Show();
    }
}
