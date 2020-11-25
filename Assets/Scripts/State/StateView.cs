using UnityEngine;
using State;

public class StateView : MonoBehaviour
{
    //現在のUpdate関数と次のUpdate関数
    private StateUpdate currentUpdate, nextUpdate;

    private void Update()
    {
        //次の関数が設定させたら更新
        if (nextUpdate != null)
        {
            currentUpdate = nextUpdate;
            nextUpdate = null;
            currentUpdate(true);
        }
        else
            currentUpdate?.Invoke(false);
    }

    /// <summary>
    /// 次に処理するべき関数を受け取る
    /// </summary>
    /// <param name="nextUpdate">次の関数</param>
    public void SetState(StateUpdate nextUpdate)
    {
        this.nextUpdate = nextUpdate;
    }
}

