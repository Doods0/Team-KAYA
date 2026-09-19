using UnityEngine;

public class GroundTiler : MonoBehaviour
{
    [Header("Target & Setup")]
    [SerializeField] private Transform[] tiles = new Transform[9];

    [Header("Settings")]
    [SerializeField] private Vector2 tileSize;

    private void Update()
    {
        Vector3 player = GameUtils.instance.playerPosition;
        if (player == Vector3.zero) return;

        // Check each tile individually
        foreach (Transform tile in tiles)
        {
            if (tile == null) continue;

            float deltaX = player.x - tile.position.x;
            float deltaY = player.y - tile.position.y;

            if (Mathf.Abs(deltaX) >= tileSize.x * 1.5f)
            {
                float moveX = Mathf.Sign(deltaX) * tileSize.x * 3f;
                tile.position += new Vector3(moveX, 0f, 0f);
            }

            if (Mathf.Abs(deltaY) >= tileSize.y * 1.5f)
            {
                float moveY = Mathf.Sign(deltaY) * tileSize.y * 3f;
                tile.position += new Vector3(0f, moveY, 0f);
            }
        }
    }
}
