using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Botao que inicia o jogo
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    //Botao pra sair do jogo
    public void QuitApplication()
    {
        Application.Quit();
    }
}
