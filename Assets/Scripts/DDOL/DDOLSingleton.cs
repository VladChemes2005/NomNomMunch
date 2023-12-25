using System.Collections.Generic;

public class DDOLSingleton : DontDestroyOnLoad
{
    private static HashSet<string> _instanced = new();
    protected override void Awake()
    {
        if (_instanced.Contains(gameObject.name))
            Destroy(gameObject);

        _instanced.Add(gameObject.name);
        base.Awake();
    }
}
