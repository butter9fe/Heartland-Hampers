using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public enum CloudScriptType
{
    OnPlayerRegister
}

public class CloudScriptManager : DesignPatterns.SingletonPersistent<CloudScriptManager>
{
    public delegate void OnSuccess(ExecuteCloudScriptResult r);
    public delegate void OnError(PlayFabError e);
    public delegate void OnGetIsGuest(bool isGuest);

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

    public void ExecGetUserDataAnotherPlayer(string playfabId, List<string> keys, OnSuccess onSuccess, OnError onError)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "GetUserDataOfPlayer",
            FunctionParameter = new
            {
                PlayfabId = playfabId,
                Keys = keys
            },
            GeneratePlayStreamEvent = true
        },
        r => onSuccess(r),
        e => onError(e));
    }

    public void ExecGetIsNewAccount(OnGetIsGuest onSuccess, OnError onError)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "GetIsNewAccount",
            FunctionParameter = new { },
            GeneratePlayStreamEvent = true
        },
        (r) => onSuccess((bool)r.FunctionResult),
        (e) => onError(e));
    }

    public void ExecSendFriendRequest(string friendPlayFabId, OnSuccess onSuccess, OnError onError)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "SendFriendRequest",
            FunctionParameter = new
            {
                FriendPlayFabId = friendPlayFabId
            },
            GeneratePlayStreamEvent = true
        },
        r => onSuccess(r),
        e => onError(e));
    }

    public void ExecAcceptFriendRequest(string friendPlayFabId, OnSuccess onSuccess, OnError onError)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "AcceptFriendRequest",
            FunctionParameter = new
            {
                FriendPlayFabId = friendPlayFabId
            },
            GeneratePlayStreamEvent = true
        },
        r => onSuccess(r),
        e => onError(e));
    }

    public void ExecRejectFriendRequest(string friendPlayFabId, OnSuccess onSuccess, OnError onError)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "RejectFriendRequest",
            FunctionParameter = new
            {
                FriendPlayFabId = friendPlayFabId
            },
            GeneratePlayStreamEvent = true
        },
        r => onSuccess(r),
        e => onError(e));
    }

    public void ExecSearchPlayers(string searchQuery, int searchType, OnSuccess onSuccess, OnError onError)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "SearchPlayers",
            FunctionParameter = new
            {
                SearchQuery = searchQuery,
                SearchType = searchType,
                MaxEntries = 20
            },
            GeneratePlayStreamEvent = true
        },
        r => onSuccess(r),
        e => onError(e)
        );
    }

    public void ExecCreateTitleGuild(string guildName, string guildDesc, OnSuccess onSuccess, OnError onError)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "CreateTitleGuild",
            FunctionParameter = new
            {
                GroupName = guildName,
                Headline = guildDesc
            },
            GeneratePlayStreamEvent = true
        },
        r => onSuccess(r),
        e => onError(e)
        );
    }

    public void ExecGetAllGuilds(OnSuccess onSuccess, OnError onError)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "GetAllGuilds",
            FunctionParameter = new { },
            GeneratePlayStreamEvent = true
        },
        r => onSuccess(r),
        e => onError(e)
        );
    }

    public void ExecGetOneGuild(PlayFab.GroupsModels.EntityKey groupKey, OnSuccess onSuccess, OnError onError)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "GetGuild",
            FunctionParameter = new
            {
                EntityKey = groupKey
            },
            GeneratePlayStreamEvent = true
        },
        r => onSuccess(r),
        e => onError(e)
        );
    }

    public void ExecGetGuildMembers(PlayFab.GroupsModels.EntityKey groupKey, string query, OnSuccess onSuccess, OnError onError)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "GetGuildMembers",
            FunctionParameter = new
            {
                EntityKey = groupKey,
                SearchQuery = query
            },
            GeneratePlayStreamEvent = true
        },
        r => onSuccess(r),
        e => onError(e));
    }

    public void ExecGetGuildApplications(PlayFab.GroupsModels.EntityKey groupKey, OnSuccess onSuccess, OnError onError)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "GetGuildApplications",
            FunctionParameter = new
            {
                EntityKey = groupKey
            },
            GeneratePlayStreamEvent = true
        },
        r => onSuccess(r),
        e => onError(e));
    }

    public void ExecRemoveMemberFromGuild(PlayFab.GroupsModels.EntityKey groupKey, PlayFab.GroupsModels.EntityKey memberKey, OnSuccess onSuccess, OnError onError)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "RemoveMemberFromGuild",
            FunctionParameter = new
            {
                GroupKey = groupKey,
                MemberKey = memberKey
            },
            GeneratePlayStreamEvent = true
        },
        r => onSuccess(r),
        e => onError(e));
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
