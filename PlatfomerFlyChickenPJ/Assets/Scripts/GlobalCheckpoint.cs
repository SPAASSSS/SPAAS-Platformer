using UnityEngine;

public class GlobalCheckpoint : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.SetGlobalCheckpoint(transform.position);
    }
}