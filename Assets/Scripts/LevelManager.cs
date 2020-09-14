using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    Vector2 playerInitPosition;

    void Start()
    {
        playerInitPosition = FindObjectOfType<Fox>().transform.position;
    }

    public void Restart()
    {
        //1- Restart the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //2- Reset the player's position 
        //Save the player's initial position when game starts
        //When respawning simply reposit the player to that init position
        //Reset the player's movement speed
        //FindObjectOfType<Fox>().ResetPlayer();
        //FindObjectOfType<Fox>().transform.position = playerInitPosition;
        //Reset the life count

    }
}
