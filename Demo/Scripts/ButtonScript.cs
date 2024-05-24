using SlimeNull.UnityFramework;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [field: SerializeField]
    public GameObject Prefab { get; set; }

    public void SendInstantiatePrefabMessage()
    {
        MessageCenter.Instance.Broadcast(new InstantiatePrefabMessage(Prefab));
    }
}