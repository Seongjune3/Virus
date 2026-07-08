using UnityEngine;

public class Star : MonoBehaviour
{
    public AudioClip collectSound;

    public void Boom()
    {
        AudioSource.PlayClipAtPoint(collectSound, Camera.main.transform.position);
    
        gameObject.SetActive(false);
    }
}