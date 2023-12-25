using UnityEngine;

public class DDOLSAudioStarter : DDOLSingleton
{
    [SerializeField]
    private AudioSource _prefab;

    protected override void Awake()
    {
        base.Awake();
        _prefab.Play();
    }
}