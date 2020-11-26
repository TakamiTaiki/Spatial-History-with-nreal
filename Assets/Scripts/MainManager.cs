using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using static Utils.Utility;


public class MainManager : MonoBehaviour
{
    [SerializeField] private Transform planeStart;
    [SerializeField] private Transform contentRoot;

    private ExplainTextFactory explain;
    private NrealInputFactory input;
    private AudioFactory audio;
    private StateFactory state;
    private FloatingMenuFactory menu;

    private Vector3 shrinkScale = Vector3.one * 0.23f;
    private Vector3 enlargeScale = Vector3.one * 1.5f;
    private Quaternion eraTextRotation = Quaternion.Euler(0, 0, 0);


    private List<GameObject> castles = new List<GameObject>();
    private List<GameObject> planes = new List<GameObject>();

    private Dictionary<string, int> nodeDictionary = new Dictionary<string, int>();
    
    #region 時代の構造体
    [Serializable]
    public struct Node
    {
        [Header("時代")]
        public int era;
        [Header("年代テキスト")]
        public Transform eraText;
        [Header("説明"), TextArea(1, 3)]
        public string explain;
        [Header("ボイス")]
        public AudioClip vc;
        [Header("サウンドエフェクト")]
        public AudioClip se;
        [Header("Nodeのトランスフォーム")]
        public Transform transform;
        [Header("ディゾルブ用のパネル")]
        public Transform panel;
        [Header("パネルのターゲット位置")]
        public Transform destination;
        [Header("アセット")]
        public GameObject[] assets;
        [NonSerialized]
        public int myIndex;
        public UnityEvent process;
    }
    public Node[] nodes;
    private Node currentNode;
    #endregion

    void Start()
    {
        explain = GetComponent<ExplainTextFactory>();
        audio = GetComponent<AudioFactory>();
        state = GetComponent<StateFactory>();
        state.Controller.SetState(ST_Positioning);
        input = GetComponent<NrealInputFactory>();
        menu = GetComponent<FloatingMenuFactory>();
        GameObject castles_Root = GameObject.Find("/ContentRoot/BuildDissolve/Castles");
        foreach (Transform child in castles_Root.transform) castles.Add(child.gameObject);
        castles.RemoveAt(0);
        GameObject planes_Root = GameObject.Find("/ContentRoot/BuildDissolve/Planes");
        foreach (Transform child in planes_Root.transform) planes.Add(child.gameObject);
    }

    #region コンテンツの位置決め

    public void ST_Positioning(bool isFirst)
    {
        //初期処理
        if (isFirst)
        {
            StartCoroutine(Positioning());
        }
        //継続処理
        else
        {

        }
    }
    private IEnumerator Positioning()
    {
        GameObject positioningCanvas = Instantiate(Resources.Load("Prefabs/PositioningCanvas"), Camera.main.transform.position, Quaternion.identity) as GameObject;
        Image clickImageOff = positioningCanvas.transform.Find("ClickImageOff").GetComponent<Image>();
        Image clickImageOn = positioningCanvas.transform.Find("ClickImageOn").GetComponent<Image>();
        for (int i = 0; i < 8; i++)
        {
            yield return new WaitForSeconds(0.5f);
            clickImageOff.enabled = !clickImageOff.enabled;
            clickImageOn.enabled = !clickImageOn.enabled;
        }
        Destroy(positioningCanvas);
    }

    public void ST_SetPosition(bool isFirst)
    {
        //初期処理
        if (isFirst)
        {
            GameObject pin = GameObject.Find("/NRInput/Right/ControllerTracker/LaserRaycaster/Pin");
            GameObject player = GameObject.Find("/ContentRoot/BuildDissolve/Player1");
            contentRoot.position += pin.transform.position - player.transform.position;
            contentRoot.GetComponent<Animation>().Play("AdventPlayerAnim");
            Destroy(pin);
            state.Controller.SetState(ST_FirstHalfIntroduction);
        }
        //継続処理
        else
        {
        }
    }
    #endregion

    #region イントロダクション
    public void ST_FirstHalfIntroduction(bool isFirst)
    {
        if (isFirst)
        {
            StartCoroutine(FirstHalfIntroduction());
        }
        else
        {
        }
    }
    private IEnumerator FirstHalfIntroduction()
    {
        audio.Controller.SetAudioVoice(audio.Controller.Model.Clip_Intro);
        yield return new WaitWhile(() => audio.Controller.View_Voice.IsPlaying());
        explain.Controller.SetText("コントローラでクリックをしましょう");
    }

    public void ST_LatterHalfIntroduction(bool isFirst)
    {
        if (isFirst)
        {
            audio.Controller.View_Voice.Stop();
            explain.Controller.SetText(string.Empty);
            StartCoroutine(LatterHalfIntroduction());
        }
        else
        {

        }
    }

    private IEnumerator LatterHalfIntroduction()
    {
        audio.Controller.SetAudioSE(audio.Controller.Model.Clip_WorldMake);
        // オープニングアニメーション
        Animation openAnim = contentRoot.GetComponent<Animation>();
        openAnim.Play("OpeningDissolveAnim");
        yield return new WaitWhile(() => openAnim.isPlaying);
        Init();
        //audio.Controller.SetAudioVoice(audio.Controller.Model.Clip_OpeExplain);
        //yield return new WaitForSeconds(5f);
        explain.Controller.SetText("スタートボタンをクリックしましょう");
    }
    #endregion

    #region スタンバイ
    public void ST_Standby(bool isFirst)
    {
        //初期処理
        if (isFirst)
        {
        }
        //継続処理
        else
        {
            // スタートボタンを押すまでのスタンバイ状態
        }
    }
    #endregion

    #region プレイ
    public void ST_Play(bool isFirst)
    {
        //初期処理
        if (isFirst)
        {

        }
        //継続処理
        else
        {
            // コンテンツ中
        }
    }
    #endregion

    #region 自由選択
    public void ST_FreeChoice(bool isFirst)
    {
        //初期処理
        if (isFirst)
        {
        }
        //継続処理
        else
        {
            Alignment(menu.Controller.View.nodeSelectEffect, currentNode.transform.gameObject);
            GameObject target = input.Controller.View.GetRaycastHit();
            if (target == null) return;

            if (target.tag == "FreeChoiceNode" && currentNode.transform.gameObject.name != target.name)
            {
                currentNode = nodes[nodeDictionary[target.name]];
                ChangeObjColor(menu.Controller.View.nodeSelectEffect, Color.white);
                audio.Controller.SetAudioSE(audio.Controller.Model.Clip_PreSelect);
                explain.Controller.SetText(currentNode.explain);
            }
        }
    }
    #endregion

    #region 1585年
    public void Event_1585() { StartCoroutine(Process_1585(currentNode)); }
    IEnumerator Process_1585(Node node)
    {
        SetStage(node.era, castles, planes);
        PlayAudioAndSetText(node);
        yield return PlaneLiftDown(node);
        if (state.Controller.Model.GameState == ST_FreeChoice) yield break;
        UpdateNode();
    }
    #endregion

    #region 1615年
    public void Event_1615() { StartCoroutine(Process_1615(currentNode)); }
    IEnumerator Process_1615(Node node)
    {
        GameObject fire = node.assets[0], warAnim = node.assets[1];
        SetStage(node.era, castles, planes);
        PlayAudioAndSetText(node);
        //開戦
        warAnim.SetActive(true);
        yield return ReleaseArrow(node.assets[2], node.assets[3]);

        //着火
        fire.SetActive(true);
        yield return PlaneLiftDown(node);

        //鎮火、非アクティブ化
        fire.SetActive(false);
        warAnim.SetActive(false);

        if (state.Controller.Model.GameState == ST_FreeChoice) yield break;
        UpdateNode();
    }
    #endregion

    #region 1626年
    public void Event_1626() { StartCoroutine(Process_1626(currentNode)); }
    IEnumerator Process_1626(Node node)
    {
        SetStage(node.era, castles, planes);
        PlayAudioAndSetText(node);
        yield return PlaneLiftUp(node);
        if (state.Controller.Model.GameState == ST_FreeChoice) yield break;
        UpdateNode();
    }
    #endregion

    #region 1665年
    public void Event_1665() { StartCoroutine(Process_1665(currentNode)); }
    IEnumerator Process_1665(Node node)
    {
        GameObject lightnig = node.assets[0], cloud = node.assets[1], fire = node.assets[2];
        Transform dissolveShere = node.panel;
        SetStage(node.era, castles, planes);
        PlayAudioAndSetText(node);
        //落雷
        lightnig.SetActive(true);
        cloud.SetActive(true);
        //落ちるまで待つ
        yield return new WaitForSeconds(1.74f);
        dissolveShere.localScale = Vector3.one * 0.334f;
        //火災
        fire.SetActive(true);
        yield return new WaitForSeconds(2f);
        //非アクティブなど
        fire.SetActive(false);
        dissolveShere.localScale = Vector3.one * 0.0001f;
        lightnig.SetActive(false);
        cloud.SetActive(false);
        //castles[4].SetActive(false);
        if (state.Controller.Model.GameState == ST_FreeChoice) yield break;
        UpdateNode();
    }
    #endregion

    #region 1931年
    public void Event_1931() { StartCoroutine(Process_1931(currentNode)); }
    IEnumerator Process_1931(Node node)
    {
        SetStage(node.era, castles, planes);
        PlayAudioAndSetText(node);
        yield return PlaneLiftUp(node);
        if (state.Controller.Model.GameState == ST_FreeChoice) yield break;
        UpdateNode();
    }
    #endregion

    #region サブルーチン

    IEnumerator PlaneLiftUp(Node node)
    {
        Vector3 dir = Vector3.up * Time.deltaTime * 0.15f;
        while (node.panel.position.y < node.destination.position.y)
        {
            node.panel.Translate(dir);
            yield return null;
        }
    }

    IEnumerator PlaneLiftDown(Node node)
    {
        Vector3 dir = Vector3.down * Time.deltaTime * 0.15f;
        while (node.panel.position.y > node.destination.position.y)
        {
            node.panel.Translate(dir);
            yield return null;
        }
    }

    IEnumerator ActiveNextNode(Node node)
    {
        //縮小化
        yield return Shrink();
        //移動
        float t = 0;
        while (IsReaching(ref t, 0.001f, 0.1f))
        {
            menu.Controller.View.plane_UI.position = Vector3.Lerp(menu.Controller.View.plane_UI.position, node.transform.position, t);
            yield return null;
        }
        //拡大化
        yield return EnLarge(node);
    }

    IEnumerator Shrink()
    {
        float t = 0;
        while (IsReaching(ref t, 0.02f, 1))
        {
            menu.Controller.View.nodeIndicater.transform.localScale = Vector3.Lerp(menu.Controller.View.nodeIndicater.transform.localScale, shrinkScale, t);
            yield return null;
        }
    }

    IEnumerator EnLarge(Node node)
    {
        float t = 0;
        while (IsReaching(ref t, 0.02f, 1))
        {
            menu.Controller.View.nodeIndicater.transform.localScale = Vector3.Lerp(menu.Controller.View.nodeIndicater.transform.localScale, enlargeScale, t);
            yield return null;
        }

        t = 0;
        Activate(node.eraText.gameObject);
        while (IsReaching(ref t, 0.02f, 1))
        {
            node.eraText.localRotation = Quaternion.Lerp(node.eraText.localRotation, eraTextRotation, t);
            yield return null;
        }
    }

    IEnumerator On3DButtonDown(GameObject button)
    {
        ChangeObjColor(button, Color.green);
        yield return new WaitForSeconds(0.5f);
        ChangeObjColor(button, Color.white);
        button.SetActive(false);
    }

    IEnumerator ReleaseArrow(GameObject arrow1, GameObject arrow2)
    {
        Vector3 arrow1Pos = arrow1.transform.position;
        Vector3 arrow2Pos = arrow2.transform.position;
        yield return new WaitForSeconds(2f);
        arrow1.SetActive(true);
        arrow2.SetActive(true);
        for (int i = 0; i < 20; i++)
        {
            arrow1.transform.Translate(0, 0.001f, 0.001f);
            arrow2.transform.Translate(0, 0.001f, 0.001f);
            yield return null;
        }
        arrow1.SetActive(false);
        arrow2.SetActive(false);
        arrow1.transform.position = arrow1Pos;
        arrow2.transform.position = arrow2Pos;

    }

    #endregion

    public void RunAnimation_Auto()
    {
        //決定のSEを流す
        audio.Controller.SetAudioSE(audio.Controller.Model.Clip_Select);
        //各ノードに割り振られた処理を開始
        currentNode.process.Invoke();
        //タグを変えることによる選択できないようにする
        currentNode.transform.tag = "ActiveNode";
    }

    public void RunAnimation_Free()
    {
        //決定のSEを流す
        audio.Controller.SetAudioSE(audio.Controller.Model.Clip_Select);
        //セレクトエフェクトのカラーを決定色(Green)にする
        ChangeObjColor(menu.Controller.View.nodeSelectEffect, Color.green);
        currentNode.process.Invoke();
    }

    public void OnStartButtonClicked()
    {
        //決定のSEを流す
        audio.Controller.SetAudioSE(audio.Controller.Model.Clip_Select);
        StartCoroutine(On3DButtonDown(menu.Controller.View.startButton));
        SetNodeEffect(NodeEffect.AUTO);
        //ノードインディケーターを初期位置まで移動
        StartCoroutine(ActiveNextNode(nodes[0]));
        nodes[0].transform.tag = "NextWaitNode";
        state.Controller.SetState(ST_Play);
    }

    public void OnRebootButtonClicked()
    {
        //決定のSEを流す
        audio.Controller.SetAudioSE(audio.Controller.Model.Clip_Select);
        StartCoroutine(On3DButtonDown(menu.Controller.View.rebootButton));
        Init();
    }

    #region 初期化
    public void Init()
    {
        Material nodeInactiveMaterial = Resources.Load("Materials/Node_NoneActiveColor") as Material;
        //タグと索引の初期化
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].transform.tag = "InActiveNode";
            nodes[i].myIndex = i;
            nodeDictionary[nodes[i].transform.name] = i;
            nodes[i].transform.gameObject.GetComponent<Renderer>().material = nodeInactiveMaterial;
            nodes[i].eraText.localRotation = Quaternion.Euler(90, 0, 0);
            DeActivate(nodes[i].eraText.gameObject);
        }
        currentNode = nodes[0];

        Alignment(menu.Controller.View.plane_UI, planeStart);
        SetStage(currentNode.era, castles, planes);
        explain.Controller.SetText(string.Empty);
        Activate(menu.Controller.View.startButton);
        SetNodeEffect(NodeEffect.INIT);

        state.Controller.SetState(ST_Standby);
    }
    #endregion

    public void MoveContent()
    {
        Vector3 forward = contentRoot.position - Camera.main.transform.position;
        forward.y *= 0;
        contentRoot.rotation = Quaternion.LookRotation(forward);
        contentRoot.transform.position = input.Controller.View.GetRayTipPosition();
    }


    #region ノードのアップデート関連
    public void UpdateNode()
    {
        currentNode.transform.gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/Node_ActiveColor") as Material;
        int nextIndex = currentNode.myIndex + 1;
        if (nextIndex < nodes.Length)
        {
            currentNode = nodes[nextIndex];
            currentNode.transform.tag = "NextWaitNode";
            StartCoroutine(ActiveNextNode(currentNode));
        }
        //最後のノードのイベントが終わったときはフリー選択モードとなる
        else
        {
            //パネルずらし
            menu.Controller.View.plane_UI.localPosition += Vector3.right;
            FreeChoiceAvtivate();
        }
    }

    public void PlayAudioAndSetText(Node node)
    {
        audio.Controller.SetAudioSE(node.se);
        audio.Controller.SetAudioVoice(node.vc);
        explain.Controller.SetText(node.explain);
    }
    #endregion

    #region その他の関数
    /// <summary>
    /// ストーリーモードからフリーチョイスモードへ
    /// </summary>
    public void FreeChoiceAvtivate()
    {
        SetNodeEffect(NodeEffect.FREE);
        //イニシャライズボタンのアクティブ化
        menu.Controller.View.rebootButton.SetActive(true);
        //アップデートモードの変更
        state.Controller.SetState(ST_FreeChoice);
        //タグを変える
        for (int i = 0; i < nodes.Length; i++)
            nodes[i].transform.tag = "FreeChoiceNode";
    }

    public enum NodeEffect { INIT, AUTO, FREE }
    public void SetNodeEffect(NodeEffect ne)
    {
        switch (ne)
        {
            case NodeEffect.INIT:
                DeActivate(menu.Controller.View.nodeIndicater);
                DeActivate(menu.Controller.View.nodeSelectEffect);
                break;
            case NodeEffect.AUTO:
                Activate(menu.Controller.View.nodeIndicater);
                DeActivate(menu.Controller.View.nodeSelectEffect);
                break;
            case NodeEffect.FREE:
                DeActivate(menu.Controller.View.nodeIndicater);
                Activate(menu.Controller.View.nodeSelectEffect);
                break;
            default:
                break;
        }
    }

    private bool IsReaching(ref float t, float inc, float max) { return Mathf.Clamp01(t += inc) < max; }
    #endregion
}
