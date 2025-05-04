using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    //Stats Enemy
    EntityStats enemy_stats;
    public List<Transform> guns;
    public GameObject projectile_;
    float cooldown_;
    public float speedbullet;
    
    //Melee
    public bool is_melee;
    int direction = 1;
    public float zigag_speed;
    private float initialX;
    private float zigzagRange = 0.5f;

    //Ranged Complexo
    public bool is_special;
    public float rotation_speed;

    //PowerUp
    public List<GameObject> powerups_;
    //Animação impact
    public GameObject impac_;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy_stats = gameObject.GetComponent<EntityStats>();
        cooldown_ = enemy_stats.atk_speed;
        //Guarda a posição inicial que spawnou
        initialX = transform.position.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
        if (is_melee == false)
        {
            //Roda o cooldown pra atirar
            CooldownOnShoot();
        }
        if (is_special)
        {
            //Fica rotacionando o inimigo
            transform.Rotate(0, 0, rotation_speed * Time.deltaTime);
        }
        if (gameObject.transform.position.y == -6)
        {
            //Destroy quando chegar fora da cena
            Destroy(this.gameObject);
        }

    }

    void Movement()
    {
        //Só desce
        if (is_melee == false)
        {
            transform.position = Vector2.MoveTowards(gameObject.transform.position, new Vector2(gameObject.transform.position.x, -6), enemy_stats.speed_ * Time.deltaTime);
        }
        else
        {
            // Movimento vertical constante para baixo
            transform.position += Vector3.down * enemy_stats.speed_ * Time.deltaTime;

            // Movimento horizontal de zigzag
            transform.position += Vector3.right * direction * zigag_speed * Time.deltaTime;

            //Altera o movimento quando chegar na posicao maxima

            if (transform.position.x >= initialX + zigzagRange)
            {
                direction = -1;
            }
            else if (transform.position.x <= initialX - zigzagRange)
            {
                direction = 1;
            }
        }
    }

    void CooldownOnShoot()
    {
        //Se o cooldown zerar atira e sai som
        if (cooldown_ > 0)
        {
            cooldown_ -= Time.deltaTime;
        }
        else if (cooldown_ <= 0)
        {
            InventoryManager.Instance.Enemy_sound();
            Shoot();
            cooldown_ = enemy_stats.atk_speed;
        }
    }

    void Shoot()
    {
        //Se for especial atira em direção dos canhoes
        if (is_special == true)
        {
            float angleStep = 360f / guns.Count; // Divide o círculo igualmente entre os tiros
            float startingAngle = transform.eulerAngles.z; // Pega a rotação atual do inimigo

            for (int i = 0; i < guns.Count; i++)
            {
                float angle = startingAngle + (angleStep * i); // Calcula o ângulo correto para cada tiro

                // Converte o ângulo para direção (vetor unitário)
                Vector2 shootDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

                GameObject projectile_instance = Instantiate(projectile_, guns[i].position, Quaternion.Euler(0, 0, angle));

                Rigidbody2D rb = projectile_instance.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = shootDirection * speedbullet;
                }

                Destroy(projectile_instance, 1.5f);
            }
        }
        else
        {
            //Só atira pra baixo
            foreach (Transform gun in guns)
            {
                GameObject projectile_instance = Instantiate(projectile_, gun.position, Quaternion.identity);
                projectile_instance.GetComponent<Rigidbody2D>().linearVelocity = Vector2.down * speedbullet;
                Destroy(projectile_instance, 1.5f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Se for melee causa mais dano ao colidir, som, animação.
            if (is_melee == true)
            {
                collision.gameObject.GetComponent<EntityStats>().TakeDamage(10);
                GameObject impac_instance = Instantiate(impac_, collision.ClosestPoint(transform.position), Quaternion.identity);
                impac_instance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                Destroy(this.gameObject);
            }
            else
            {
                //Dano, som e animação
                collision.gameObject.GetComponent<EntityStats>().TakeDamage(5);
                GameObject impac_instance = Instantiate(impac_, collision.ClosestPoint(transform.position), Quaternion.identity);
                impac_instance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                Destroy(this.gameObject);
            }
        }
    }

    public void PowerUpChance()
    {
        //Calcula a chance de spawnar powerup
        float chanceofpower = 0.10f;
        float num_power = Random.value;

        if (num_power <= chanceofpower)
        {
            //Spawna um powerup aleatorio
            int random_power = Random.Range(0, 5);
            Instantiate(powerups_[random_power], transform.position, Quaternion.identity);
        }
        //Calcula a chance de spawnar vida
        float chanceofhp = 0.15f;
        float hp_num = Random.value;

        if (hp_num <= chanceofhp)
        {
            //Spawna a vida
            Instantiate(powerups_[5], transform.position, Quaternion.identity);
        }
    }

}
