using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PurchaseBlock : MonoBehaviour
{
    public System.Action<bool> OnEnoughMoney;

    public abstract bool EnoughMoney { get; protected set; }
    
    protected abstract void CheckMoney();
}
