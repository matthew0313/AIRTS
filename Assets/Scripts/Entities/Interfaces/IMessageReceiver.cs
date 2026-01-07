using System;
using System.Collections.Generic;
using UnityEngine;

public interface IMessageReceiver
{
    void ReceiveMessage(Entity speaker, string message);
}   