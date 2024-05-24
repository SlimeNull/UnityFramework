using System.Collections;
using System.Collections.Generic;
using SlimeNull.UnityFramework;
using UnityEngine;

[MessageHandler]
public class InstantiatePrefabHandler : MessageHandler<InstantiatePrefabMessage>
{
    public override void HandleMessage(InstantiatePrefabMessage message)
    {
        Object.Instantiate(message.Prefab);
    }
}
