using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossScript : MonoBehaviour
{
    //StatsBoss
    Rigidbody2D rb;
    EntityStats boss_stats;
    int direction = 1;
    bool s_attack;
    bool is_placed;
    bool simple_move = true;


    //SimpleAttack
    public List<GameObject> simple_guns;
    public GameObject projectile_s;
    public float bullet_s_speed;
    bool isAttacking = false;
    public int simple_attk_dmg;

    //MiniGunAttack
    public GameObject projectile_m;
    public List<GameObject> m_guns;
    bool isMiniGunAttacking = false;
    bool alreadyMAttk;
    public int minigun_attk_dmg;

    //LazerAttack
    public GameObject lazer_project;
    public List<GameObject> lazer_guns;
    bool alreadyLazerAttk;
    public int lazer_attk_dmg;
    private List<GameObject> activeLasers = new List<GameObject>();

    //Death
    public List<Transform> exp_points;
    public GameObject exp_anim;
    bool can_explode = true;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Infos iniciais
        rb = gameObject.GetComponent<Rigidbody2D>();
        boss_stats = gameObject.GetComponent<EntityStats>();
        s_attack = true;
        alreadyMAttk = false;
        alreadyLazerAttk = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Faz o movimento de entrada
        if (gameObject.transform.position.y != 3.03f)
        {
            StartMovement();
        }

        //Começa o boss
        else if (gameObject.transform.position.y == 3.03f)
        {
            //Atire apenas quando está em posição
            is_placed = true;

            //Movimento para os lados
            Movement();
            
            //Se a vida for menor ou igual a 800 ativa o ataque de minigun
            if (gameObject.GetComponent<EntityStats>().hp_ <= 800 && alreadyMAttk == false)
            {
                alreadyMAttk = true;
                isMiniGunAttacking = true;
                StartCoroutine(minigun_attack());
            }
            //Se a vida for menor ou igual a 400 ativa o ataque de laser
            if (gameObject.GetComponent<EntityStats>().hp_ <= 400 && alreadyLazerAttk == false)
            {
                alreadyLazerAttk = true;
                Lazer_attack();
            }
            //Inicia o ataque simples
            if (!isAttacking) // Garante que a corrotina só começa uma vez
            {
                isAttacking = true;
                StartCoroutine(simple_attack());
            }
            //Se o boss morrer para todos os ataques e desativa o laser
            if (boss_stats.hp_ <= 0)
            {
                StopAllAttacks();
                DestroyActiveLasers(); // Destrui todos os lasers restantes
            }

        }
    }

    //Movimentação
    void StartMovement()
    {
        transform.position = Vector2.MoveTowards(gameObject.transform.position, new Vector2(gameObject.transform.position.x, 3.03f), 1 * Time.deltaTime);
    }
    public void Movement()
    {
        if (simple_move == true)
        {
            rb.linearVelocityX = boss_stats.speed_ * direction;
        }
    }

    //Ataques

    //Simples
    IEnumerator simple_attack()
    {
        while (s_attack == true && is_placed == true)
        {
            //Espera 0,8 segundos e atira no canhao esquerdo, dps espera mais 0,8 e atira no direito. Repete a não ser que q uma bool se torne falsa.
            yield return new WaitForSeconds(0.8f);

            InventoryManager.Instance.SimpleFireSound();

            GameObject projectile_instance1 = Instantiate(projectile_s, simple_guns[0].transform.position, projectile_s.transform.rotation);
            projectile_instance1.GetComponent<Projectile_boss>().projectile_dmg = simple_attk_dmg;
            projectile_instance1.GetComponent<Rigidbody2D>().linearVelocity = Vector2.down * bullet_s_speed;
            Destroy(projectile_instance1, 1.5f);

            yield return new WaitForSeconds(0.8f);

            InventoryManager.Instance.SimpleFireSound();

            GameObject projectile_instance2 = Instantiate(projectile_s, simple_guns[1].transform.position, projectile_s.transform.rotation);
            projectile_instance2.GetComponent<Projectile_boss>().projectile_dmg = simple_attk_dmg;
            projectile_instance2.GetComponent<Rigidbody2D>().linearVelocity = Vector2.down * bullet_s_speed;
            Destroy(projectile_instance2, 1.5f);

        }
    }
    //MiniGun
    IEnumerator minigun_attack()
    {

        while (isMiniGunAttacking == true)
        {
            //Espera 0,3 segundo e atira para uma direção aleatoria em uma arma aleatória.
            yield return new WaitForSeconds(0.2f);

            int random_gun = Random.Range(0, 2);
            float random_fire = Random.Range(-8.8f, 8.8f);

            InventoryManager.Instance.MiniGunSound();

            GameObject projectile_instance = Instantiate(projectile_m, m_guns[random_gun].transform.position, Quaternion.identity);
            projectile_instance.GetComponent<MiniGun_boss>().projectile_dmg = minigun_attk_dmg;
            projectile_instance.GetComponent<Rigidbody2D>().AddForce(new Vector2(random_fire, -6), ForceMode2D.Impulse);
            Destroy(projectile_instance, 2);
        }
    }
    //LaserAttack
    void Lazer_attack()
    {
        //Spawna um lazer em cada arma.
        foreach (GameObject l_g in lazer_guns)
        {
            // Instancia na posicao que está o prefab para ficar perfeito com a arma.
            GameObject laser_instance = Instantiate(lazer_project, new Vector3(l_g.transform.position.x, l_g.transform.position.y - 3.22f, l_g.transform.position.z), Quaternion.identity);
            laser_instance.GetComponent<LazerGun>().projectile_dmg = lazer_attk_dmg;
            activeLasers.Add(laser_instance); // Adiciona o laser à lista de lasers ativos para depois destruir tambem
            StartCoroutine(Lazer_dynamic(laser_instance, l_g));
        }

        InventoryManager.Instance.LaserContinuosSound();

    }
    IEnumerator Lazer_dynamic(GameObject laser, GameObject gun)
    {
        //Para cada laser em cada arma será alternada a posição junto com o boss
        laser.transform.position = new Vector3(gun.transform.position.x, gun.transform.position.y - 3.22f, laser.transform.position.z);

        while (laser != null)
        {
            // Atualiza a posição do laser para seguir o movimento no eixo X do boss
            laser.transform.position = new Vector3(gun.transform.position.x, laser.transform.position.y, laser.transform.position.z);

            yield return null;
        }
    }

    public void DestroyActiveLasers()
    {
        foreach (GameObject laser in activeLasers)
        {
            Destroy(laser); // Destrói cada laser na lista
        }
        activeLasers.Clear(); // Limpa a lista após destruir os lasers
    }

    //Função pra parar todos os ataques
    public void StopAllAttacks()
    {
        s_attack = false;
        isMiniGunAttacking = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Trocar de direção ao colidir com a parede
        if (collision.gameObject.tag == "wall")
        {
            direction *= -1;
        }
    }

    //Death
    IEnumerator DeathMoment()
    {
        while (can_explode == true)
        {
            yield return new WaitForSeconds(0.2f);

            //Instancia uma explosao e som em cada lugar aleatorio e faz o inimigo se mover aleatoriamente para dar uma sensação de destruição

            InventoryManager.Instance.StartSound();

            int Random_point = Random.Range(0, exp_points.Count);

            GameObject exp_instance = Instantiate(exp_anim, exp_points[Random_point].transform.position, Quaternion.identity);

            float Random_move = Random.Range(-1, 2);

            gameObject.transform.position = new Vector2(Random_move, gameObject.transform.position.y);
        }
    }

    IEnumerator TimeOfDeath()
    {
        //Após 7 segundos de animação de morte ele cancela tudo, pausa o jogo, aparece a tela de vitoria e som.
        yield return new WaitForSeconds(7);

        can_explode = false;
        Destroy(this.gameObject);

        Time.timeScale = 0;
        InventoryManager.Instance.WinSound();
        InventoryManager.Instance.WinnerMoment();

    }

    public void Death()
    {
        //Para a musica de boss, para os laser, para o movimento e inicia a animação de morte.
        SpawnManager.Instance.finalboss_.Stop();
        InventoryManager.Instance.DisableLaserConti();
        simple_move = false;
        StartCoroutine(DeathMoment());

        StartCoroutine(TimeOfDeath());

    }

}
