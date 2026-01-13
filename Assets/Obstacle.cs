using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float obstacleSpeed = 2f; // units per second, moves right->left

    private void Update()
    {
        transform.position += Vector3.left * obstacleSpeed * Time.deltaTime;
    }

    public void SetSpeed(float s)
    {
        obstacleSpeed = s;
    }
}
