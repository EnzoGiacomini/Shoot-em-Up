using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    //Misc
    Rigidbody2D rb;
    EntityStats player_stats;
    public GameObject projectile_;
    public float speedbullet;
    float cooldown_;
    public GameObject shield_warning;

    //Armas e hud de powerup
    public List<Transform> guns;
    public List<GameObject> powup_hud;

    //PowerUp
    bool HasMultiShot;
    bool HasExplosionShot;
    //Cena de gameover
    public GameObject over_scene;

    //Sound
    public AudioSource laser_sound;
    public AudioSource powerUp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        player_stats = gameObject.GetComponent<EntityStats>();
        HasMultiShot = false;
        HasExplosionShot = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Movimento inicial para aparecer na tela
        if (gameObject.transform.position.y <= -3.49f)
        {
            transform.position = Vector2.MoveTowards(gameObject.transform.position, new Vector2(gameObject.transform.position.x, 3.03f), 1 * Time.deltaTime);
        }
        //Movimento e cooldown para tiro
            Movement();
            CooldownOnShoot();
        //Checa se o cooldown é 0 e pressionou space, para atirar
            if (Input.GetKey(KeyCode.Space) && cooldown_ <= 0)
            {
                //Atira normal
                if (HasMultiShot == false)
                {
                    laser_sound.PlayOneShot(laser_sound.clip);
                    Shoot();
                    cooldown_ = player_stats.atk_speed;
                }
                //Atira especial
                else if (HasMultiShot == true)
                {
                    laser_sound.PlayOneShot(laser_sound.clip);
                    MultiShoot();
                    cooldown_ = player_stats.atk_speed;
                }
            }
    }
    //Movimentação
    void Movement()
    {
        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        move.Normalize();
        move = move * player_stats.speed_ * Time.deltaTime;
        rb.linearVelocity = move;
    }
    //Cooldown de tiro
    void CooldownOnShoot()
    {
        if (cooldown_ > 0)
        {
            cooldown_ -= Time.deltaTime;
        }
    }

    void MultiShoot()
    {
        //Tiro Frontal
        GameObject projectile_instance = Instantiate(projectile_, gameObject.transform.position, projectile_.transform.rotation);
        projectile_instance.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * speedbullet;
        Destroy(projectile_instance, 1.5f);

        //Tiro Diagonal
        foreach (Transform gun in guns)
        {
            GameObject projectile_instance2 = Instantiate(projectile_, gameObject.transform.position, gun.transform.rotation);

            Projectile_Player projectileScript2 = projectile_instance2.GetComponent<Projectile_Player>();

            if (HasExplosionShot == true)
            {
                projectileScript2.canExplode = true;
            }
            else
            {
                projectileScript2.canExplode = false;
            }
            //Atira o projetil para a direção que a arma está apontada
            Vector2 shootDirection = (gun.position - transform.position).normalized;

            if (shootDirection.y < 0) shootDirection.y *= -1; // Garante que o tiro sempre vá para cima

            // Aumenta levemente a componente Y para subir mais
            shootDirection += new Vector2(0, 0.5f);
            shootDirection.Normalize(); // Normaliza para manter a velocidade constante

            projectile_instance2.GetComponent<Rigidbody2D>().linearVelocity = shootDirection * speedbullet;

            Destroy(projectile_instance2, 1.5f);
        }
    }

    void Shoot()
    {
        //Atira
        GameObject projectile_instance = Instantiate(projectile_, gameObject.transform.position, projectile_.transform.rotation);

        Projectile_Player projectileScript = projectile_instance.GetComponent<Projectile_Player>();

        if (HasExplosionShot == true)
        {
            projectileScript.canExplode = true;
        }
        else
        {
            projectileScript.canExplode = false;
        }

        projectile_instance.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * speedbullet;
        Destroy(projectile_instance, 1.5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Shield
        if (collision.gameObject.tag == "shield")
        {
            powerUp.PlayOneShot(powerUp.clip);
            player_stats.HasShield = true;
            shield_warning.SetActive(true);
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.35f);
            powup_hud[4].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            Destroy(collision.gameObject);
            Invoke("DisableShield", 5);
        }
        //Multishot
        if (collision.gameObject.tag == "multishot")
        {
            powerUp.PlayOneShot(powerUp.clip);
            HasMultiShot = true;
            powup_hud[3].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            Destroy(collision.gameObject);
            Invoke("DisableMultiShot", 5);
        }
        //MiniGun
        if (collision.gameObject.tag == "minigun")
        {
            powerUp.PlayOneShot(powerUp.clip);
            player_stats.atk_speed /= 10;
            powup_hud[2].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            Destroy(collision.gameObject);
            Invoke("DisableMiniGun", 5);
        }
        //Haste
        if (collision.gameObject.tag == "haste")
        {
            powerUp.PlayOneShot(powerUp.clip);
            player_stats.speed_ *= 2;
            powup_hud[1].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            Destroy(collision.gameObject);
            Invoke("DisableHaste", 5);
        }
        //ExplosionShot
        if (collision.gameObject.tag == "explosion")
        {
            powerUp.PlayOneShot(powerUp.clip);
            HasExplosionShot = true;
            powup_hud[0].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            Destroy(collision.gameObject);
            Invoke("DisableExplosionShot", 5);
        }
        //HP
        if (collision.gameObject.tag == "hp")
        {
            powerUp.PlayOneShot(powerUp.clip);
            // Calcula a cura sem ultrapassar o HP máximo
            float missingHP = player_stats.hp_max - player_stats.hp_;
            float healAmount = Mathf.Min(20, missingHP); // Cura o menor valor possível

            player_stats.hp_ += healAmount; // Aplica a cura real

            InventoryManager.Instance.RefreshHUD();

            Destroy(collision.gameObject);
        }
    }

    //Desativa os powerUps
    void DisableShield()
    {
        powup_hud[4].GetComponent<Image>().color = new Color(1, 1, 1, 0.254f);
        shield_warning.SetActive(false);
        player_stats.HasShield = false;
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    void DisableMultiShot()
    {
        powup_hud[3].GetComponent<Image>().color = new Color(1, 1, 1, 0.254f);
        HasMultiShot = false;
    }

    void DisableMiniGun()
    {
        powup_hud[2].GetComponent<Image>().color = new Color(1, 1, 1, 0.254f);
        player_stats.atk_speed *= 10;
    }

    void DisableHaste()
    {
        powup_hud[1].GetComponent<Image>().color = new Color(1, 1, 1, 0.254f);
        player_stats.speed_ /= 2;
    }

    void DisableExplosionShot()
    {
        powup_hud[0].GetComponent<Image>().color = new Color(1, 1, 1, 0.254f);
        HasExplosionShot = false;
    }

    //Tela de GameOver
    public void GameOver()
    {
        Time.timeScale = 0;
        over_scene.SetActive(true);
    }

}
