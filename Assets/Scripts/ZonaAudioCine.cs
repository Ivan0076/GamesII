using UnityEngine;

public class ZonaAudioCine : MonoBehaviour
{
    public AudioSource audioCine;

    private void Start()
    {
        audioCine.volume = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioCine.volume = 1f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioCine.volume = 0f;
        }
    }
}