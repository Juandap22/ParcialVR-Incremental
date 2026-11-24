using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    public GameStats Stats {get; private set;} = new GameStats();

    void Awake()
    {
        if(Instance == null){
            Instance = this;
        }
    }

    public void ChangeScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void AddFix()
    {
        Stats.Fixed++;
    }

    public void AddDamage()
    {
        Stats.Damaged++;
    }
}

public class GameStats
{
    public int Fixed;
    public int Damaged;
    public int Money;
    public int AllMoney;
}
