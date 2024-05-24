using UnityEngine;

public class InstantiatePrefabMessage
{
    public InstantiatePrefabMessage(GameObject prefab)
    {
        Prefab = prefab;
    }

    public GameObject Prefab { get; }
}
