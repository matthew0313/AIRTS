using System;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public interface IMessageReceiver
{
    void ReceiveMessage(Entity speaker, string message);
}   