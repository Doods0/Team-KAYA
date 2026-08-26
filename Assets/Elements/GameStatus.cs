using UnityEngine;

public class GameStatus : MonoBehaviour
{
    public static GameStatus instance;

    public Transform playerTransform;

    public void Awake()
    {
        instance = this;
    }
}
