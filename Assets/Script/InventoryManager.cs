using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{

    public static InventoryManager Instance { get; private set; }

    //Contador de pontos e misc
    public Text points_text;
    public int points_counter;
    int LastPoints_;
    int many_;
    bool already_final;


    //HP
    public Slider hp_bar;
    public Text hp_text;
    EntityStats player_stats;

    //HP boss
    public GameObject hp_bos;
    public Slider hp_boss;
    EntityStats boss_stats;
    public TMP_Text boss_text;

    //Winner
    public GameObject winScene_;

    //Sound
    public AudioSource impact_sound;
    public AudioSource fire_s_sound;
    public AudioSource minigun_sound;
    public AudioSource laser_continuo;
    public AudioSource win__;
    public AudioSource enemy_sound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_stats = GameObject.FindGameObjectWithTag("Player").GetComponent<EntityStats>();
        already_final = false;
        //Atualiza a vida
        RefreshHUD();
        LastPoints_ = 0;
        many_ = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Se somar +100 pontos 
        if (points_counter >= LastPoints_ + 100)
        {
            LastPoints_ = points_counter;
            many_++;
            //Roda os 60 segundos pre boss
            if (many_ == 4 && already_final == false)
            {
                SpawnManager.Instance.startfinal();
                already_final = true;
                return;
            }
            else if (many_ < 4)
            {
                //Aumenta as chances, dificultando o jogo
                SpawnManager.Instance.UpperChance();
                SpawnManager.Instance.spawn_time -= 0.6f;
                SpawnManager.Instance.spawn_chance += 0.1f;
            }
        }
    }
    //Adiciona os pontos do inimigo morto e atualiza o hud
    public void AddPoints(int p_)
    {
        points_counter += p_;
        RefreshHUD();
    }
    //Atualiza sliders, pontos, hp...
    public void RefreshHUD()
    {
        points_text.text = points_counter.ToString("00");
        hp_bar.maxValue = player_stats.hp_max;
        hp_bar.value = player_stats.hp_;
        hp_text.text = player_stats.hp_.ToString() + "/100";
        if (boss_stats != null)
        {
            hp_boss.maxValue = boss_stats.hp_max;
            hp_boss.value = boss_stats.hp_;
        }
    }
    //Acha o boss quando ele for spawnado.
    public void FindBoss()
    {
        boss_stats = GameObject.FindGameObjectWithTag("boss").GetComponent<EntityStats>();
    }
    //Botao pra jogar novamente
    public void PlayAgain()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("SampleScene");
    }
    //Botao pra ir para o menu
    public void Menu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("StartScene");
    }
    //Inicia a tela de vitoria
    public void WinnerMoment()
    {
        winScene_.SetActive(true);
    }
    //Som de impacto
    public void StartSound()
    {
        impact_sound.PlayOneShot(impact_sound.clip);
    }
    //Som de tiro boss
    public void SimpleFireSound()
    {
        fire_s_sound.PlayOneShot(fire_s_sound.clip);
    }
    //Som de minigun boss
    public void MiniGunSound()
    {
        minigun_sound.PlayOneShot(minigun_sound.clip);
    }
    //Som de laser boss
    public void LaserContinuosSound()
    {
        laser_continuo.Play();
    }
    //Desativa os laser
    public void DisableLaserConti()
    {
        laser_continuo.Stop();
    }
    //Som de vitória
    public void WinSound()
    {
        win__.PlayOneShot(win__.clip);
    }
    //Som do tiro inimigo
    public void Enemy_sound()
    {
        enemy_sound.PlayOneShot(enemy_sound.clip);
    }
    //Botao para sair do jogo
    public void QuitApplication()
    {
        Application.Quit();
    }
}
