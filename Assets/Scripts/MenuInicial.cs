using UnityEngine;
using UnityEngine.SceneManagement; // NOVO: A chave para mudar de fase!

public class MenuInicial : MonoBehaviour
{
    public void Jogar()
    {
        // Coloque aqui o NOME EXATO da sua cena principal do jogo (com letras maiúsculas e minúsculas iguais)
        // Por exemplo: "SampleScene", "Fase1", "Corrida", etc.
        SceneManager.LoadScene("SampleScene"); 
    }
}