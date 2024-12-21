/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public enum CloudScriptType
{
    OnPlayerRegister,
    OnPlayerLogin,
    SoftResetAccount,
    ResetAccount,
    DeletePlayer
}

public class CloudScriptManager : DesignPatterns.SingletonPersistent<CloudScriptManager>
{
    public delegate void OnSuccess(ExecuteCloudScriptResult r);
    public delegate void OnError(PlayFabError e);
    public delegate void OnGetIsGuest(bool isGuest);

    private bool bIsUpdatingPlayerInfo = false;

    [SerializeField] LoginManagement loginManager;

    public void ExecBasicCoudScriptFunction(CloudScriptType type, OnSuccess onSuccess = null, OnError onError = null)
    {
        // Convert type to function name
        string functionName = System.Enum.GetName(typeof(CloudScriptType), type);
        bool shouldUpdateInfo = type switch
        {
            CloudScriptType.OnPlayerRegister => true,
            CloudScriptType.OnPlayerLogin => true,
            _ => false
        };

        if (shouldUpdateInfo) bIsUpdatingPlayerInfo = true;
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = functionName,
                FunctionParameter = new { },
                GeneratePlayStreamEvent = true
            },
            (r) => {
                if (shouldUpdateInfo) bIsUpdatingPlayerInfo = false;
                var succFunc = onSuccess == null ? OnExecSucc : onSuccess;
                succFunc(r);
            },
            (e) => {
                var failFunc = onError == null ? OnExecFail : onError;
                failFunc(e);
            }
        );
    }

    public void ExecOpenBundle(ItemInstance item, OnSuccess onSuccess, OnError onError)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "OpenBundle",
            FunctionParameter = new
            {
                Item = item
            },
            GeneratePlayStreamEvent = true, 
        },
        r => onSuccess(r),
        e => onError(e));
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

    public void ExecOnGuestLogin(OnSuccess onSuccess, OnError onError)
    {
        bIsUpdatingPlayerInfo = true;

        // Reset account if a previous guest on PC exists
        // (double check in case Guest forcefully closes game)
        ExecGetIsNewAccount(isNewAccount =>
        {
            if (!isNewAccount)
            {
                // Soft reset account (just clear user data and inventory while keeping account)
                ExecBasicCoudScriptFunction(CloudScriptType.SoftResetAccount, null, onError);
            }

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "OnGuestLogin",
                FunctionParameter = new { },
                GeneratePlayStreamEvent = true
            },
            (r) => {
                bIsUpdatingPlayerInfo = false;
                OnExecSucc(r);
                onSuccess(r);
            },
            (e) => onError(e));

        }, onError);
    }

    public IEnumerator ExecGetIsGuest(OnGetIsGuest onSuccess, OnError onError)
    {
        // Do not get whether player is guest until info has been set
        yield return new WaitUntil(() => bIsUpdatingPlayerInfo == false);

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "GetIsGuest",
            FunctionParameter = new { },
            GeneratePlayStreamEvent = true
        },
        (r) => onSuccess((bool)r.FunctionResult),
        (e) => onError(e));
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
            FunctionParameter = new {
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
            FunctionParameter = new{},
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
*/