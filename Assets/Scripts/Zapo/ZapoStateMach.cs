using System.Collections.Generic;
using System.Diagnostics;


public enum GeneralStateType
{
    NOTREADY,
    READY,
    ACTIVE,
    BUSY,
}

[System.Serializable]
public class GeneralMach : ZapoStateMach<GeneralStateType>
{
}

//General
[System.Serializable]
public class ZapoState
{
    public object enumValue = null;
    public string enumName = "?";

    public int idx;
    public delegate void OnStateDelegate(object owningObject);
    public delegate bool CanEnterDelegate(object owningObject);
    public delegate void UpdateDelegate(float deltaTime, object owningObject);

    public OnStateDelegate OnEnter;
    public OnStateDelegate OnExit;
    public CanEnterDelegate CanEnter;
    public UpdateDelegate DoUpdate;

    public ZapoState(int i, object eVal, string eName)
    {
        enumValue = eVal;
        enumName = eName;
        idx = i;
        OnEnter = null;
        OnExit = null;
        CanEnter = DefaultEnter;
        DoUpdate = null;
    }

    public void Noop()
    {
        return;
    }

    public bool DefaultEnter(object owningObject)
    {
        return true;
    }
}

[System.Serializable]
public class ZapoStateMach<T> where T : struct, System.IConvertible, System.IComparable, System.IFormattable
{
    public System.Object owningObject;
    public delegate void OnChangeDelegate(int idx);
    public OnChangeDelegate OnChange;

    public ZapoState CurrentState;
    public ZapoState PreviousState;
    public ZapoState failedState;
    public List<ZapoState> stateList;
    public bool isInitialized = false;
    public float timeInState;
    public Dictionary<T, T> AdvanceMap = new Dictionary<T, T>();
    public Dictionary<T, T> WithdrawMap = new Dictionary<T, T>();

    public virtual void Initialize(System.Object own)
    {
        owningObject = own;
        T[] enumVals = ZapoEnumUtils.GetAllEnumValues<T>();
        stateList = new List<ZapoState>(enumVals.Length);

        for (int i = 0; i < enumVals.Length; ++i)
        {
            T enumValue = (T)enumVals.GetValue(i);
            string enumName = ZapoEnumUtils.GetEnumName<T>(enumValue);
            stateList.Add(new ZapoState(i, enumValue, enumName));
        }
        PreviousState = stateList[0];
        CurrentState = stateList[0];

        isInitialized = true;
        timeInState = 0.0f;
    }

    public ZapoState this[int i]
    {
        get { return stateList[i]; }
        set { }
    }

    public int GetPreviousStateIdx()
    {
        return PreviousState.idx;
    }

    public T CurrentStateEnum { get { return (T)(object)GetCurrentStateIdx(); } }
    public int GetCurrentStateIdx()
    {
        return CurrentState.idx;
    }
    public ZapoState GetStateByType(T type)
    {
        return stateList[(int)(System.ValueType)type];
    }

    public bool IsState(T tValue)
    {
        return isInitialized && CurrentState.enumValue.Equals(tValue);
    }

    public bool? SetState(T type)
    {
        return SetZapoState(GetStateByType(type));
    }
    public bool? Advance()
    {
        if (AdvanceMap.ContainsKey(CurrentStateEnum))
        {
            return SetState(AdvanceMap[CurrentStateEnum]);
        }
        return false;
    }
    public bool? Withdraw()
    {
        if (WithdrawMap.ContainsKey(CurrentStateEnum))
        {
            return SetState(WithdrawMap[CurrentStateEnum]);
        }
        return false;
    }


    public bool? SetZapoState(ZapoState nextState)
    {
        if (nextState == CurrentState)
        {
            return null;
        }

        if (nextState.CanEnter(owningObject))
        {
            failedState = null;
            PreviousState = CurrentState;
            CurrentState = nextState;

            if (PreviousState.OnExit != null) { PreviousState.OnExit(owningObject); }
            if (OnChange != null) { OnChange(PreviousState.idx); }
            if (CurrentState.OnEnter != null) { CurrentState.OnEnter(owningObject); }
            timeInState = 0.0f;
            return true;
        }
        failedState = nextState;
        return false;
    }

    public bool? SetLastState()
    {
        return SetZapoState(PreviousState);
    }

    public override string ToString()
    {
        return CurrentState.enumName;
    }

    public virtual void MachineUpdate(float deltaTime)
    {
        timeInState += deltaTime;
        if (CurrentState.DoUpdate != null)
        {
            CurrentState.DoUpdate(deltaTime, owningObject);
        }
    }

    public void AddEnterListener(ZapoState.OnStateDelegate deleg)
    {
        string input = deleg.Method.Name;
        input = input.Replace("On", "");
        T stateType = ZapoEnumUtils.GetEnumByName<T>(input);

        int state = GetStateByType(stateType).idx;
        if (stateList[state].OnEnter == null)
        {
            stateList[state].OnEnter = deleg;
        }
        else
        {
            stateList[state].OnEnter += deleg;
        }
    }

    public void AddExitListener(ZapoState.OnStateDelegate deleg)
    {
        string input = deleg.Method.Name;
        input = input.Replace("Off", "");
        T stateType = ZapoEnumUtils.GetEnumByName<T>(input);

        int state = GetStateByType(stateType).idx;
        if (stateList[state].OnExit == null)
        {
            stateList[state].OnExit = deleg;
        }
        else
        {
            stateList[state].OnExit += deleg;
        }
    }


    public void AddUpdateListener(ZapoState.UpdateDelegate deleg)
    {
        string input = deleg.Method.Name;
        input = input.Replace("Update", "");
        T stateType = ZapoEnumUtils.GetEnumByName<T>(input);

        int state = GetStateByType(stateType).idx;
        if (stateList[state].DoUpdate == null)
        {
            stateList[state].DoUpdate = deleg;
        }
        else
        {
            stateList[state].DoUpdate += deleg;
        }
    }

    public void AddChangeListener(OnChangeDelegate deleg)
    {
        if (OnChange == null)
        {
            OnChange = deleg;
        }
        else
        {
            OnChange += deleg;
        }
    }

}