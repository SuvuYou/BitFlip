using System;
using UnityEngine;

public interface IContextData 
{
    public Action OnContextInjected { get; set; }
}

public abstract class ContextDataSO : ScriptableObject, IContextData 
{
    public Action OnContextInjected { get; set; }
}