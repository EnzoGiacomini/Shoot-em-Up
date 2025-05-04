using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    public static SpawnManager Instance { get; private set; }

    //Spawnpoints e inimigos
    public List<Transform> spawnpoints;
    public List<GameObject> enemies;
    //Stats spawn
    public float spawn_time;
    public float spawn_chance;
    public float group_chance;
    public bool can_spawn;

    //FinalTime
    public GameObject survivealarm_;
    public GameObject timecounter;
    public TMP_Text time_count;
    float time_;
    bool time_enabled;

    //Boss
    public GameObject boss_;
    public Transform spawn_boss;

    //Sound
    public AudioSource ambient_;
    public AudioSource finalboss_;

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

    void Start()
    {
        can_spawn = true;
        //Inicia a corrotina que por ser um loop vai ser tocada até que seja falso ou encerre o jogo
        StartCoroutine(SpawnEnemies());
    }

    private void Update()
    {

    }

    //Spawna Inimigos
    IEnumerator SpawnEnemies()
    {

        yield return new WaitForSeconds(1);

        //Enquanto verdade, roda infinitamente
        while (can_spawn == true)
        {
            //Manda esperar os segundos dentro de WaitForSeconds para rodar novamente. Impede travamentos e spawn loucos
            yield return new WaitForSeconds(spawn_time);
            //Para cada spawnpoint
            foreach (Transform sp in spawnpoints)
            {
                if (Random.value < spawn_chance) // Chance de spawn baseada na probabilidade
                {
                    //Spawna um inimigo aleatorio
                    GameObject enemy_instance = Instantiate(enemies[Random.Range(0, enemies.Count)], sp.position, Quaternion.identity);
                    //Probabilidade de spawnar em grupo
                    if (Random.value < group_chance)
                    {
                        //Sorteia se vai ser grupos de 1,2 ou 3
                        int Num_group = Random.Range(1, 3);
                        //Spawna a quantidade sortida
                        for (int i = 0; i < Num_group; i++)
                        {
                            // Manda esperar 1 segundo para spawnar os novos inimigos do grupo, evitando, assim, que spawnem um em cima do outro
                            yield return new WaitForSeconds(1);
                            //Inimigo aleatorio
                            GameObject enemy_instance2 = Instantiate(enemies[Random.Range(0, enemies.Count)], sp.position, enemies[1].transform.rotation);
                            Destroy(enemy_instance2, 7.5f);
                        }

                    }

                    Destroy(enemy_instance, 7.5f);

                }
            }
        }
    }

    //Ativa o final de 1 minuto
    public void startfinal()
    {
        StartCoroutine(finaltime());
    }

    IEnumerator finaltime()
    {
        //Alerta de survive
        survivealarm_.SetActive(true);

        yield return new WaitForSeconds(4);
        //Desabilita o alerta
        survivealarm_.SetActive(false);
        //Inicia a contagem de segundos
        timecounter.SetActive(true);
        time_enabled = true;
        StartCoroutine(TimerCoroutine());

        yield return new WaitForSeconds(60);

        //60 segundos dps, para o spawn, tempo e musica e ambiente
        can_spawn = false;

        time_enabled = false;
        timecounter.SetActive(false);

        ambient_.Stop();
        //Espera 8 segundos pra ter certeza q vai matar todo mundo
        yield return new WaitForSeconds(8);

        //Encontra todos os inimigos vivos e mata
        GameObject[] enemies_alive = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies_alive)
        {
            Destroy(enemy);
        }
        //Toca musica do boss e spawna ele
        finalboss_.Play();

        Instantiate(boss_, spawn_boss.transform.position, Quaternion.identity);

        //Inicia o slider do boss
        InventoryManager.Instance.hp_bos.SetActive(true);
        InventoryManager.Instance.FindBoss();
        InventoryManager.Instance.RefreshHUD();


    }


    //Conta os segundos e minutos
    IEnumerator TimerCoroutine()
    {
        time_ = 0;
        while (time_enabled == true) 
        {
            int minutes = Mathf.FloorToInt(time_ / 60);
            int seconds = Mathf.FloorToInt(time_ % 60);

            time_count.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            yield return new WaitForSeconds(1); // Atualiza o tempo a cada segundo
            time_++;
        }
    }
    //Aumenta a chance de grupo
    public void UpperChance()
    {
        group_chance += 0.2f;
    }

}