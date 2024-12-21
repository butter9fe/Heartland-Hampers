using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public enum CloudScriptType
{
    OnUserRegister
}

public class CloudScriptManager : DesignPatterns.SingletonPersistent<CloudScriptManager>
{
    public delegate void OnSuccess(ExecuteCloudScriptResult r);
    public delegate void OnError(PlayFabError e);
    public delegate void OnGetBoxId(int boxId);

    public void ExecBasicCoudScriptFunction(CloudScriptType type, OnSuccess onSuccess = null, OnError onError = null)
    {
        // Convert type to function name
        string functionName = System.Enum.GetName(typeof(CloudScriptType), type);
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = functionName,
            FunctionParameter = new { },
            GeneratePlayStreamEvent = true
        },
            (r) => {
                var succFunc = onSuccess == null ? OnExecSucc : onSuccess;
                succFunc(r);
            },
            (e) => {
                var failFunc = onError == null ? OnExecFail : onError;
                failFunc(e);
            }
        );
    }

    public void ExecGetBoxID(OnGetBoxId onSuccess, OnError onError)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "GetCurrentBoxId",
            FunctionParameter = new { },
            GeneratePlayStreamEvent = true
        },
        (r) => onSuccess(System.Convert.ToInt32(r.FunctionResult.ToString())),
        (e) => onError(e));
    }

    void OnExecSucc(ExecuteCloudScriptResult r)
    {
        Debug.Log("Response from server: " + r.FunctionResult.ToString());
    }

    void OnExecFail(PlayFabError e)
    {
        Debug.LogError("Error from cloud script: " + e.GenerateErrorReport());
    }
}
