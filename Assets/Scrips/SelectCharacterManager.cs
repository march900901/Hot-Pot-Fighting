using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using Photon.Realtime;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class SelectCharacterManager : MonoBehaviourPunCallbacks
{
    [SerializeField]Text textRoomName;
    [SerializeField]Text textPlayerList;
    [SerializeField]Button buttonStartGame;
    // [SerializeField]InputActionReference MoveActionRef;
    // [SerializeField]InputActionReference DashActionRef;
    // [SerializeField]InputActionReference LiftActionRef;
    // [SerializeField]InputActionReference ThrowActionRef;
    // [SerializeField]InputActionReference ScapeActionRef;
    // [SerializeField]Text ForwordActionText;
    // [SerializeField]Text BackwordActionText;
    // [SerializeField]Text LeftActionText;
    // [SerializeField]Text RightActionText;
    // [SerializeField]Text DashActionText;
    // [SerializeField]Text LiftActionText;
    // [SerializeField]Text ThrowActionText;
    // [SerializeField]Text ScapeActionText;
    
    public int CharacterIndex = 0;
    public List<GameObject> CharacterList = new List<GameObject>();
    public Transform GeneratPoint;
    public string selectCharacterName;
    public Text text_Confirm;
    public bool CanStart;
    public int CanStartPlayer;
    public DoTween PlayerListAni;
    public DoTween NextButton;
    public DoTween PreviousButton;
    public GameObject GameMode;
    public GameObject PanelStory;
    DoTween ButtonMoveIn;
    PhotonView _pv;
    PlayerInput playerInput;
    AudioManager _am;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {//如果沒連上伺服器的話就回到Lobby場景
            SceneManager.LoadScene("Lobby");
        }else{//如果有連上伺服器就把房間名稱UI設為房間名稱，並刷新玩家列表
            textRoomName.text="Room: " + PhotonNetwork.CurrentRoom.Name;
            UpDatePlayerList();
        }
        buttonStartGame.interactable=false;//禁用開始按鈕
        //在GeneratPoint生成角色列表第0個角色
        GameObject Character = Instantiate(CharacterList[0],GeneratPoint.position,new Quaternion(0,180,0,0));
        Character.name = CharacterList[0].name;//將生成的角色名字改成跟角色列表一樣，防止出現culon等字樣，方便後續操作
        ReSetCharacter(Character);//將角色控制、數據等腳本刪除，防止報錯
        selectCharacterName = Character.name;//將所selectCharcterName設為當前所選角色名字
        text_Confirm.enabled = false;//將UI初始化
        _pv = this.transform.GetComponent<PhotonView>();
        playerInput = this.transform.GetComponent<PlayerInput>();
        //SetBindingBT();
        _am = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        PlayerListAni.PlayerListAni();
        ButtonMoveIn = this.gameObject.GetComponent<DoTween>();
        ButtonMoveIn.StartCoroutine("ButtonMoveIn");
        NextButton.ScaleButton(0.5f);
        PreviousButton.ScaleButton(0.5f);
        GameMode.active = PhotonNetwork.IsMasterClient;
        PanelStory.transform.GetComponent<DoTween>().PanelIn();
    }

    public override void OnMasterClientSwitched(Player newMasterClient){//當房主切換的時候
        //buttonStartGame.interactable=PhotonNetwork.IsMasterClient;
    }

    public void UpDatePlayerList(){//一個一個印出PlayerList的玩家名字
        StringBuilder sb = new StringBuilder();
        foreach (var kvp in PhotonNetwork.CurrentRoom.Players)
        {
            sb.AppendLine("> " + kvp.Value.NickName);
        }
        textPlayerList.text=sb.ToString();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer){//當玩家加入房間時更新玩家列表
        UpDatePlayerList();
        UpdateCanStart();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer){//當玩家離開房間時更新玩家列表
        UpDatePlayerList();
        print("LeftRoom");
    }

    public void OnClickStart(){//按下Start按紐時
        CallRpcSetGameMode(GameMode.gameObject.GetComponent<GameModeText>().index);
        
        PhotonNetwork.CurrentRoom.IsVisible = false;
    }

    public void OnClickHome(){
        PhotonNetwork.Disconnect();
    }
    
    public void OnClickLeaveRoom(){//按下LeaveRoom按鈕時
        _am.PlayAudio(16);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom(){//當離開房間時
        
        SceneManager.LoadScene("Lobby");
    }
//---------選角區--------
    public void AddCharaterIndex(){//當CharacterIndex在角色列表範圍內時，CharacterIndex+1
        if (CharacterIndex <= CharacterList.Count-2)
        {
            CharacterIndex ++;
        }else{
            CharacterIndex = 0;
        }
        text_Confirm.enabled = false;
        if (CanStart == true)
        {//取消確認
            CanStartPlayer --;
            CallRpcCancelConfirm();
        }
        CanStart = false;
        UpdateCharacter();//刷新角色預覽
        UpdateCanStart();
    }

    public void ReduceCharacterIndex(){//當CharacterIndex在角色列表範圍內時，CharacterIndex-1
        if (CharacterIndex >0)
        {
            CharacterIndex--;
        }else{
            CharacterIndex = CharacterList.Count-1;
        }
        text_Confirm.enabled = false;
        if (CanStart == true)
        {//取消確認
            CanStartPlayer --;
            CallRpcCancelConfirm();
        }
        CanStart = false;
        UpdateCharacter();//刷新角色預覽
        UpdateCanStart();
    }

    public void ReSetCharacter(GameObject gameObject){//刪除角色物件中可能報錯的腳本
        Destroy(gameObject.GetComponent<PlayerContaller>());
        Destroy(gameObject.GetComponent<PlayerData>());
        Destroy(gameObject.GetComponent<PlayerInput>());
    }

    public void UpdateCharacter(){//刷新角色預覽
        Destroy(GameObject.FindGameObjectWithTag("Player"));//刪除原本的角色物件
        //依照CharacterIndex生成新的角色物件
        GameObject Character = Instantiate(CharacterList[CharacterIndex],GeneratPoint.position,new Quaternion(0,180,0,0));
        //將生成的角色物件名字改成跟角色列表的一樣，預防clone等字出現，方便之後操作
        Character.name = CharacterList[CharacterIndex].name;
        ReSetCharacter(Character);//重製角色物件
        Animator animator = Character.GetComponentInChildren<Animator>();
        if (animator)
        {
            animator.SetTrigger("choose");
        }
        //將所selectCharcterName設為當前所選角色名字
        selectCharacterName = Character.name;
    }

    public void ConfirmCharacter(){//確認所選角色
        PlayerPrefs.SetString("CharacterName",selectCharacterName);
        text_Confirm.enabled = true;
        CanStart = true;
        CallRpcDoConfirm();
        DoConfirm();
    }

    public void CallRpcCancelConfirm(){//呼叫RPC執行取消確認
        _pv.RPC("RpcCancelConfirm",RpcTarget.Others);
    }
    [PunRPC]
    public void RpcCancelConfirm(){//呼叫RPV執行取消確認
        CanStartPlayer --;
        UpdateCanStart();
    }
    void DoConfirm(){
        print("DoConfirm");
        CanStartPlayer ++;
        UpdateCanStart();
    } 

    public void CallRpcDoConfirm(){//呼叫RPC執行確認選角
        _pv.RPC("RpcDoConfirm",RpcTarget.Others);
    }

    [PunRPC]
    void RpcDoConfirm(PhotonMessageInfo info){//RPC接收訊號執行DoConfirm
        DoConfirm();
    }

    public void UpdateCanStart(){//更新CanStart數值
        print(PhotonNetwork.CurrentRoom.PlayerCount);
        if (CanStartPlayer >= PhotonNetwork.CurrentRoom.PlayerCount)
        {//如果房間內玩家都準備好了才可以按開始按鈕
            print("Can Start");
            buttonStartGame.interactable=PhotonNetwork.IsMasterClient;
        }else{
            print("Can't Start");
            buttonStartGame.interactable = false;
        }
    }

    public void CallRpcSetGameMode(int gameMode){
        _pv.RPC("RpcSetGameMode",RpcTarget.All,gameMode);
    }

    [PunRPC]
    public void RpcSetGameMode(int gameMode,PhotonMessageInfo info){
        PlayerPrefs.SetInt("GameMode",gameMode);
        SceneManager.LoadScene("Game");
    }

// //--------綁定按鍵-------
//     public void SetBindingBT(){//初始化綁定按鈕顯示文字
//         //----移動控制
//         var vec = MoveActionRef.action.ChangeCompositeBinding("2DVector");
//         var forword = vec.NextPartBinding("Up");
//         var backword = vec.NextPartBinding("Down");
//         var left = vec.NextPartBinding("Left");
//         var right = vec.NextPartBinding("Right");
//         var f = MoveActionRef.action.GetBindingDisplayString(forword.bindingIndex);
//         var b = MoveActionRef.action.GetBindingDisplayString(backword.bindingIndex);
//         var l = MoveActionRef.action.GetBindingDisplayString(left.bindingIndex);
//         var r = MoveActionRef.action.GetBindingDisplayString(right.bindingIndex);
//         var fs = InputControlPath.ToHumanReadableString(f,InputControlPath.HumanReadableStringOptions.OmitDevice);
//         var bs = InputControlPath.ToHumanReadableString(b,InputControlPath.HumanReadableStringOptions.OmitDevice);
//         var ls = InputControlPath.ToHumanReadableString(l,InputControlPath.HumanReadableStringOptions.OmitDevice);
//         var rs = InputControlPath.ToHumanReadableString(r,InputControlPath.HumanReadableStringOptions.OmitDevice);
//         ForwordActionText.text = fs;
//         BackwordActionText.text = bs;
//         LeftActionText.text = ls;
//         RightActionText.text = rs;
//         //----動作控制
//         var dash = DashActionRef.action.bindings[0].effectivePath;//取得目前綁定的按鍵名
//         var lift = LiftActionRef.action.bindings[0].effectivePath;
//         var Throw = ThrowActionRef.action.bindings[0].effectivePath;
//         var scap = ScapeActionRef.action.bindings[0].effectivePath;
//         var dashBT = InputControlPath.ToHumanReadableString(dash,InputControlPath.HumanReadableStringOptions.OmitDevice);
//         //將取得的按鍵名轉為可讀字串
//         var liftBT = InputControlPath.ToHumanReadableString(lift,InputControlPath.HumanReadableStringOptions.OmitDevice);
//         var throwBT = InputControlPath.ToHumanReadableString(Throw,InputControlPath.HumanReadableStringOptions.OmitDevice);
//         var scapeBT = InputControlPath.ToHumanReadableString(scap,InputControlPath.HumanReadableStringOptions.OmitDevice);
//         DashActionText.text = XboxController(dashBT);//設定為Button顯示字串
//         LiftActionText.text = XboxController(liftBT);
//         ThrowActionText.text = XboxController(throwBT);
//         ScapeActionText.text = XboxController(scapeBT);
//     }

//     public void RebindingForword(){//重新綁定往前按鍵
//         print("Rebinding");
//         playerInput.SwitchCurrentActionMap("NotMe");
//         ForwordActionText.text = ("請輸入...");
//         var vec = MoveActionRef.action.ChangeCompositeBinding("2DVector");
//         var forword = vec.NextPartBinding("Up");
//             MoveActionRef.action.PerformInteractiveRebinding(forword.bindingIndex)
//             .OnMatchWaitForAnother(0.1f)
//             .WithCancelingThrough("<keyboard>/escape")
//             .OnComplete(operation=>{
//                 var p = MoveActionRef.action.GetBindingDisplayString(forword.bindingIndex);
//                 var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
//                 string text = XboxController(s);
//                 ForwordActionText.text = $"{text}";
//                 print("done");
//                 Rebind();
//                 operation.Dispose();
//                 playerInput.SwitchCurrentActionMap("Player1");
//             })
//             .OnCancel(operation=>{
//                 var p = MoveActionRef.action.GetBindingDisplayString(forword.bindingIndex);
//                 var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
//                 string text = XboxController(s);
//                 ForwordActionText.text = $"{text}";
//                 print("done");
//                 Cancel();
//                 operation.Dispose();
//                 playerInput.SwitchCurrentActionMap("Player1");
//             })
//             .Start();
            
//     }

//     public void RebindingBackword(){//重新綁定往後按鍵
//         print("Rebinding");
//         playerInput.SwitchCurrentActionMap("NotMe");
//         BackwordActionText.text = ("請輸入...");
//         var vec = MoveActionRef.action.ChangeCompositeBinding("2DVector");
//         var backword = vec.NextPartBinding("Down");
//         MoveActionRef.action.PerformInteractiveRebinding(backword.bindingIndex)
//         .OnMatchWaitForAnother(0.1f)
//         .WithCancelingThrough("<keyboard>/escape")
//         .OnComplete(operation=>{
//             var p = MoveActionRef.action.GetBindingDisplayString(backword.bindingIndex);
//             var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
//             string text = XboxController(s);
//             BackwordActionText.text = $"{text}";
//             print("done");
//             Rebind();
//             operation.Dispose();
//             playerInput.SwitchCurrentActionMap("Player1");
//         })
//         .OnCancel(operation=>{
//             var p = MoveActionRef.action.GetBindingDisplayString(backword.bindingIndex);
//             var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
//             string text = XboxController(s);
//             BackwordActionText.text = $"{text}";
//             print("done");
//             Cancel();
//             operation.Dispose();
//             playerInput.SwitchCurrentActionMap("Player1");
//         })
//         .Start();
//         Rebind();
//     }

//     public void RebindingLeft(){//重新綁定往左按鍵
//         print("Rebinding");
//         playerInput.SwitchCurrentActionMap("NotMe");
//         LeftActionText.text = ("請輸入...");
//         var vec = MoveActionRef.action.ChangeCompositeBinding("2DVector");
//         var Left = vec.NextPartBinding("Left");
//         MoveActionRef.action.PerformInteractiveRebinding(Left.bindingIndex)
//         .OnMatchWaitForAnother(0.1f)
//         .WithCancelingThrough("<keyboard>/escape")
//         .OnComplete(operation=>{
//             var p = MoveActionRef.action.GetBindingDisplayString(Left.bindingIndex);
//             var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
//             string text = XboxController(s);
//             LeftActionText.text = $"{text}";
//             print("done");
//             Rebind();
//             operation.Dispose();
//             playerInput.SwitchCurrentActionMap("Player1");
//         })
//         .OnCancel(operation=>{
//             var p = MoveActionRef.action.GetBindingDisplayString(Left.bindingIndex);
//             var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
//             string text = XboxController(s);
//             LeftActionText.text = $"{text}";
//             print("done");
//             Cancel();
//             operation.Dispose();
//             playerInput.SwitchCurrentActionMap("Player1");
//         })
//         .Start();
//         Rebind();
//     }

//     public void RebindingRght(){//重新綁定往右按鍵
//         print("Rebinding");
//         playerInput.SwitchCurrentActionMap("NotMe");
//         RightActionText.text = ("請輸入...");
//         var vec = MoveActionRef.action.ChangeCompositeBinding("2DVector");
//         var right = vec.NextPartBinding("Right");
//         MoveActionRef.action.PerformInteractiveRebinding(right.bindingIndex)
//         .OnMatchWaitForAnother(0.1f)
//         .WithCancelingThrough("<keyboard>/escape")
//         .OnComplete(operation=>{
//             var p = MoveActionRef.action.GetBindingDisplayString(right.bindingIndex);
//             var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
//             print(p);
//             string text = XboxController(s);
//             RightActionText.text = $"{text}";
//             print("done");
//             Rebind();
//             operation.Dispose();
//             playerInput.SwitchCurrentActionMap("Player1");
//         })
//         .OnCancel(operation=>{
//              var p = MoveActionRef.action.GetBindingDisplayString(right.bindingIndex);
//             var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
//             string text = XboxController(s);
//             RightActionText.text = $"{text}";
//             print("done");
//             Cancel();
//             operation.Dispose();
//             playerInput.SwitchCurrentActionMap("Player1");
//         })
//         .Start();
//         Rebind();
//     }

//     public void RebindingDash(){//重新綁定Dash按鍵
//         print("Rebinding");
//         playerInput.SwitchCurrentActionMap("NotMe");
//         DashActionText.text = ("請輸入...");
//         Rebinding(DashActionRef,DashActionText);
//     }

//     public void RebindingLift(){//重新綁定Lift按鍵
//         print("Rebinding");
//         playerInput.SwitchCurrentActionMap("NotMe");
//         LiftActionText.text = ("請輸入...");
//         Rebinding(LiftActionRef,LiftActionText);
//     }
    
//     public void RebindingThrow(){//重新綁定Throw按鍵
//         print("Rebinding");
//         playerInput.SwitchCurrentActionMap("NotMe");
//         ThrowActionText.text = ("請輸入...");
//         Rebinding(ThrowActionRef,ThrowActionText);
//     }

//     public void RebindingScape(){//重新綁定Scape按鍵
//         print("Rebinding");
//         playerInput.SwitchCurrentActionMap("NotMe");
//         ScapeActionText.text = ("請輸入...");
//         Rebinding(ScapeActionRef,ScapeActionText);
//     }
    
//     public void Rebinding(InputActionReference reference,Text actionText){//重新綁定
//         reference.action.PerformInteractiveRebinding()
//         .OnMatchWaitForAnother(0.1f)
//         .WithCancelingThrough("<keyboard>/escape")
//         .OnComplete(operation=>{
//             var p = operation.action.bindings[0].effectivePath;
//             var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
//             print(s);
//             string text = XboxController(s);
//             actionText.text = $"{text}";
//             print("done");
//             Rebind();
//             operation.Dispose();
//             playerInput.SwitchCurrentActionMap("Player1");
//         })
//         .OnCancel(operation=>{
//             var p = operation.action.bindings[0].effectivePath;
//             var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
//             string text = XboxController(s);
//             actionText.text = $"{text}";
//             print("done");
//             Cancel();
//             operation.Dispose();
//             playerInput.SwitchCurrentActionMap("Player1");
//         })
//         .Start();
        
//     }

//     public string XboxController(string input){//修正Xbox手把按鈕名稱
//         string s = input;
//         if (s == "Button South")
//         {
//             s = "A";
//         }
//         if (s == "Button North")
//         {
//             s = "Y";
//         }
//         if (s == "Button East")
//         {
//             s = "B";
//         }
//         if (s == "Button West")
//         {
//             s = "X";
//         }

//         return s;
//     }

//     public void SaveRebinding(){//儲存綁定後按鍵配置
//         string AllActionBindingDate = playerInput.actions.SaveBindingOverridesAsJson();
//         PlayerPrefs.SetString("binding",AllActionBindingDate);
//     }

//-------按鈕音效-------
    // public void ClickRebindButton(){//按下綁定按鈕的音效
    //     _am.PlayAudio(9);
    // }

    // public void Rebind(){//綁定完成的音效
    //     _am.PlayAudio(10);
    // }

    // public void Cancel(){
    //     _am.PlayAudio(11);
    // }
}
