using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ReBindingManager : MonoBehaviour
{
    [SerializeField]InputActionReference MoveActionRef;
    [SerializeField]InputActionReference DashActionRef;
    [SerializeField]InputActionReference LiftActionRef;
    [SerializeField]InputActionReference ThrowActionRef;
    [SerializeField]InputActionReference ScapeActionRef;
    [SerializeField]Text ForwordActionText;
    [SerializeField]Text BackwordActionText;
    [SerializeField]Text LeftActionText;
    [SerializeField]Text RightActionText;
    [SerializeField]Text DashActionText;
    [SerializeField]Text LiftActionText;
    [SerializeField]Text ThrowActionText;
    [SerializeField]Text ScapeActionText;
    PlayerInput playerInput;
    public AudioManager _am;
    // Start is called before the first frame update
    void Start()
    {
        playerInput = this.transform.GetComponent<PlayerInput>();
        _am = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        if (_am)
        {
            print(_am.name);
        }
        SetBindingBT();
    }

    //--------綁定按鍵-------
    public void SetBindingBT(){//初始化綁定按鈕顯示文字
        //----移動控制
        var vec = MoveActionRef.action.ChangeCompositeBinding("2DVector");
        var forword = vec.NextPartBinding("Up");
        var backword = vec.NextPartBinding("Down");
        var left = vec.NextPartBinding("Left");
        var right = vec.NextPartBinding("Right");
        var f = MoveActionRef.action.GetBindingDisplayString(forword.bindingIndex);
        var b = MoveActionRef.action.GetBindingDisplayString(backword.bindingIndex);
        var l = MoveActionRef.action.GetBindingDisplayString(left.bindingIndex);
        var r = MoveActionRef.action.GetBindingDisplayString(right.bindingIndex);
        var fs = InputControlPath.ToHumanReadableString(f,InputControlPath.HumanReadableStringOptions.OmitDevice);
        var bs = InputControlPath.ToHumanReadableString(b,InputControlPath.HumanReadableStringOptions.OmitDevice);
        var ls = InputControlPath.ToHumanReadableString(l,InputControlPath.HumanReadableStringOptions.OmitDevice);
        var rs = InputControlPath.ToHumanReadableString(r,InputControlPath.HumanReadableStringOptions.OmitDevice);
        ForwordActionText.text = fs;
        BackwordActionText.text = bs;
        LeftActionText.text = ls;
        RightActionText.text = rs;
        //----動作控制
        var dash = DashActionRef.action.bindings[0].effectivePath;//取得目前綁定的按鍵名
        var lift = LiftActionRef.action.bindings[0].effectivePath;
        var Throw = ThrowActionRef.action.bindings[0].effectivePath;
        var scap = ScapeActionRef.action.bindings[0].effectivePath;
        var dashBT = InputControlPath.ToHumanReadableString(dash,InputControlPath.HumanReadableStringOptions.OmitDevice);
        //將取得的按鍵名轉為可讀字串
        var liftBT = InputControlPath.ToHumanReadableString(lift,InputControlPath.HumanReadableStringOptions.OmitDevice);
        var throwBT = InputControlPath.ToHumanReadableString(Throw,InputControlPath.HumanReadableStringOptions.OmitDevice);
        var scapeBT = InputControlPath.ToHumanReadableString(scap,InputControlPath.HumanReadableStringOptions.OmitDevice);
        DashActionText.text = XboxController(dashBT);//設定為Button顯示字串
        LiftActionText.text = XboxController(liftBT);
        ThrowActionText.text = XboxController(throwBT);
        ScapeActionText.text = XboxController(scapeBT);
    }

    public void RebindingForword(){//重新綁定往前按鍵
        print("Rebinding");
        playerInput.SwitchCurrentActionMap("NotMe");
        ForwordActionText.text = ("請輸入...");
        var vec = MoveActionRef.action.ChangeCompositeBinding("2DVector");
        var forword = vec.NextPartBinding("Up");
            MoveActionRef.action.PerformInteractiveRebinding(forword.bindingIndex)
            .OnMatchWaitForAnother(0.1f)
            .WithCancelingThrough("<keyboard>/escape")
            .OnComplete(operation=>{
                var p = MoveActionRef.action.GetBindingDisplayString(forword.bindingIndex);
                var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
                string text = XboxController(s);
                ForwordActionText.text = $"{text}";
                print("done");
                Rebind();
                operation.Dispose();
                playerInput.SwitchCurrentActionMap("Player1");
            })
            .OnCancel(operation=>{
                var p = MoveActionRef.action.GetBindingDisplayString(forword.bindingIndex);
                var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
                string text = XboxController(s);
                ForwordActionText.text = $"{text}";
                print("done");
                Cancel();
                operation.Dispose();
                playerInput.SwitchCurrentActionMap("Player1");
            })
            .Start();
            
    }

    public void RebindingBackword(){//重新綁定往後按鍵
        print("Rebinding");
        playerInput.SwitchCurrentActionMap("NotMe");
        BackwordActionText.text = ("請輸入...");
        var vec = MoveActionRef.action.ChangeCompositeBinding("2DVector");
        var backword = vec.NextPartBinding("Down");
        MoveActionRef.action.PerformInteractiveRebinding(backword.bindingIndex)
        .OnMatchWaitForAnother(0.1f)
        .WithCancelingThrough("<keyboard>/escape")
        .OnComplete(operation=>{
            var p = MoveActionRef.action.GetBindingDisplayString(backword.bindingIndex);
            var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
            string text = XboxController(s);
            BackwordActionText.text = $"{text}";
            print("done");
            Rebind();
            operation.Dispose();
            playerInput.SwitchCurrentActionMap("Player1");
        })
        .OnCancel(operation=>{
            var p = MoveActionRef.action.GetBindingDisplayString(backword.bindingIndex);
            var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
            string text = XboxController(s);
            BackwordActionText.text = $"{text}";
            print("done");
            Cancel();
            operation.Dispose();
            playerInput.SwitchCurrentActionMap("Player1");
        })
        .Start();
    }

    public void RebindingLeft(){//重新綁定往左按鍵
        print("Rebinding");
        playerInput.SwitchCurrentActionMap("NotMe");
        LeftActionText.text = ("請輸入...");
        var vec = MoveActionRef.action.ChangeCompositeBinding("2DVector");
        var Left = vec.NextPartBinding("Left");
        MoveActionRef.action.PerformInteractiveRebinding(Left.bindingIndex)
        .OnMatchWaitForAnother(0.1f)
        .WithCancelingThrough("<keyboard>/escape")
        .OnComplete(operation=>{
            var p = MoveActionRef.action.GetBindingDisplayString(Left.bindingIndex);
            var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
            string text = XboxController(s);
            LeftActionText.text = $"{text}";
            print("done");
            Rebind();
            operation.Dispose();
            playerInput.SwitchCurrentActionMap("Player1");
        })
        .OnCancel(operation=>{
            var p = MoveActionRef.action.GetBindingDisplayString(Left.bindingIndex);
            var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
            string text = XboxController(s);
            LeftActionText.text = $"{text}";
            print("done");
            Cancel();
            operation.Dispose();
            playerInput.SwitchCurrentActionMap("Player1");
        })
        .Start();
    }

    public void RebindingRght(){//重新綁定往右按鍵
        print("Rebinding");
        playerInput.SwitchCurrentActionMap("NotMe");
        RightActionText.text = ("請輸入...");
        var vec = MoveActionRef.action.ChangeCompositeBinding("2DVector");
        var right = vec.NextPartBinding("Right");
        MoveActionRef.action.PerformInteractiveRebinding(right.bindingIndex)
        .OnMatchWaitForAnother(0.1f)
        .WithCancelingThrough("<keyboard>/escape")
        .OnComplete(operation=>{
            var p = MoveActionRef.action.GetBindingDisplayString(right.bindingIndex);
            var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
            print(p);
            string text = XboxController(s);
            RightActionText.text = $"{text}";
            print("done");
            Rebind();
            operation.Dispose();
            playerInput.SwitchCurrentActionMap("Player1");
        })
        .OnCancel(operation=>{
             var p = MoveActionRef.action.GetBindingDisplayString(right.bindingIndex);
            var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
            string text = XboxController(s);
            RightActionText.text = $"{text}";
            print("done");
            Cancel();
            operation.Dispose();
            playerInput.SwitchCurrentActionMap("Player1");
        })
        .Start();
    }

    public void RebindingDash(){//重新綁定Dash按鍵
        print("Rebinding");
        playerInput.SwitchCurrentActionMap("NotMe");
        DashActionText.text = ("請輸入...");
        Rebinding(DashActionRef,DashActionText);
    }

    public void RebindingLift(){//重新綁定Lift按鍵
        print("Rebinding");
        playerInput.SwitchCurrentActionMap("NotMe");
        LiftActionText.text = ("請輸入...");
        Rebinding(LiftActionRef,LiftActionText);
    }
    
    public void RebindingThrow(){//重新綁定Throw按鍵
        print("Rebinding");
        playerInput.SwitchCurrentActionMap("NotMe");
        ThrowActionText.text = ("請輸入...");
        Rebinding(ThrowActionRef,ThrowActionText);
    }

    public void RebindingScape(){//重新綁定Scape按鍵
        print("Rebinding");
        playerInput.SwitchCurrentActionMap("NotMe");
        ScapeActionText.text = ("請輸入...");
        Rebinding(ScapeActionRef,ScapeActionText);
    }
    
    public void Rebinding(InputActionReference reference,Text actionText){//重新綁定
        reference.action.PerformInteractiveRebinding()
        .OnMatchWaitForAnother(0.1f)
        .WithCancelingThrough("<keyboard>/escape")
        .OnComplete(operation=>{
            var p = operation.action.bindings[0].effectivePath;
            var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
            print(s);
            string text = XboxController(s);
            actionText.text = $"{text}";
            print("done");
            Rebind();
            operation.Dispose();
            playerInput.SwitchCurrentActionMap("Player1");
        })
        .OnCancel(operation=>{
            var p = operation.action.bindings[0].effectivePath;
            var s = InputControlPath.ToHumanReadableString(p,InputControlPath.HumanReadableStringOptions.OmitDevice);
            string text = XboxController(s);
            actionText.text = $"{text}";
            print("done");
            Cancel();
            operation.Dispose();
            playerInput.SwitchCurrentActionMap("Player1");
        })
        .Start();
        
    }

    public string XboxController(string input){//修正Xbox手把按鈕名稱
        string s = input;
        if (s == "Button South")
        {
            s = "A";
        }
        if (s == "Button North")
        {
            s = "Y";
        }
        if (s == "Button East")
        {
            s = "B";
        }
        if (s == "Button West")
        {
            s = "X";
        }

        return s;
    }

    public void SaveRebinding(){//儲存綁定後按鍵配置
        string AllActionBindingDate = playerInput.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("binding",AllActionBindingDate);
    }

//-------按鈕音效-------
    public void ClickRebindButton(){//按下綁定按鈕的音效
        _am.PlayAudio(9);
    }

    public void Rebind(){//綁定完成的音效
        
        _am.PlayAudio(10);
    }

    public void Cancel(){
        _am.PlayAudio(11);
    }
}
