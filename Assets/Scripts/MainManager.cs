using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using static Utils.Utility;

public class MainManager : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;

    private ExplainTextFactory explain;
    private NrealInputFactory input;
    private AudioFactory audio;
    private StateFactory state;
    private FloatingMenuFactory menu;
    private MainUsecase usecase;

    private List<GameObject> castles = new List<GameObject>();
    private List<GameObject> planes = new List<GameObject>();

    private Dictionary<string, int> nodeDictionary = new Dictionary<string, int>();
    
    #region 時代の構造体
    [Serializable]
    public struct Node
    {
        public int era;
        public Transform eraText;
        [TextArea(1, 3)]
        public string explain;
        public AudioClip vc;
        public AudioClip se;
        public Transform transform;
        public Transform panel;
        public Transform destination;
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
        usecase = new MainUsecase();
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
        Initialize();
        //audio.Controller.SetAudioVoice(audio.Controller.Model.Clip_OpeExplain);
        //yield return new WaitForSeconds(5f);
        explain.Controller.SetText("スタートボタンをクリックしましょう");
    }
    #endregion

    #region スタンバイ
    public void ST_Standby(bool isFirst) { }
    #endregion

    #region プレイ
    public void ST_Play(bool isFirst) { }
    #endregion

    #region 自由選択
    public void ST_FreeChoice(bool isFirst)
    {
        //初期処理
        if (isFirst)
        {
            DeActivate(menu.Controller.View.nodeIndicater);
            Activate(menu.Controller.View.nodeSelectEffect);
            //イニシャライズボタンのアクティブ化
            Activate(menu.Controller.View.rebootButton);
            //タグを変える
            for (int i = 0; i < nodes.Length; i++)
                nodes[i].transform.tag = "FreeChoiceNode";
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
        yield return usecase.PlaneLiftDown(node);
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
        Activate(warAnim);
        yield return usecase.ReleaseArrow(node.assets[2], node.assets[3]);

        //着火
        Activate(fire);
        yield return usecase.PlaneLiftDown(node);

        //鎮火、非アクティブ化
        DeActivate(fire);
        DeActivate(warAnim);

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
        yield return usecase.PlaneLiftUp(node);
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
        dissolveShere.localScale = Vector3.one * 0.0001f;
        SetStage(node.era, castles, planes);
        PlayAudioAndSetText(node);
        //落雷
        Activate(lightnig);
        Activate(cloud);
        DeActivate(fire);
        //落ちるまで待つ
        yield return new WaitForSeconds(1.74f);
        dissolveShere.localScale = Vector3.one * 0.334f;
        //火災
        Activate(fire);
        yield return new WaitForSeconds(2f);

        DeActivate(lightnig);
        DeActivate(cloud);
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
        yield return usecase.PlaneLiftUp(node);
        if (state.Controller.Model.GameState == ST_FreeChoice) yield break;
        UpdateNode();
    }
    #endregion

    #region クリック別処理
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
        StartCoroutine(usecase.On3DButtonDown(menu.Controller.View.startButton));
        Activate(menu.Controller.View.nodeIndicater);
        DeActivate(menu.Controller.View.nodeSelectEffect);
        //ノードインディケーターを初期位置まで移動
        StartCoroutine(usecase.ActivateNode(currentNode, menu.Controller.View));
        currentNode.transform.tag = "NextWaitNode";
        state.Controller.SetState(ST_Play);
    }

    public void OnRebootButtonClicked()
    {
        //決定のSEを流す
        audio.Controller.SetAudioSE(audio.Controller.Model.Clip_Select);
        StartCoroutine(usecase.On3DButtonDown(menu.Controller.View.rebootButton));
        Initialize();
    }
    #endregion

    #region 初期化
    public void Initialize()
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

        Alignment(menu.Controller.View.plane_UI, menu.Controller.View.planeStart);
        SetStage(currentNode.era, castles, planes);
        explain.Controller.SetText(string.Empty);
        Activate(menu.Controller.View.startButton);
        DeActivate(menu.Controller.View.nodeIndicater);
        DeActivate(menu.Controller.View.nodeSelectEffect);

        state.Controller.SetState(ST_Standby);
    }
    #endregion

    #region ドラッグ処理
    public void MoveContent()
    {
        Vector3 forward = contentRoot.position - Camera.main.transform.position;
        forward.y *= 0;
        contentRoot.rotation = Quaternion.LookRotation(forward);
        contentRoot.transform.position = input.Controller.View.GetRayTipPosition();
    }
    #endregion

    #region ノードのアップデート関連
    public void UpdateNode()
    {
        currentNode.transform.gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/Node_ActiveColor") as Material;
        int nextIndex = currentNode.myIndex + 1;
        if (nextIndex < nodes.Length)
        {
            currentNode = nodes[nextIndex];
            currentNode.transform.tag = "NextWaitNode";
            StartCoroutine(usecase.ActivateNode(currentNode, menu.Controller.View));
        }
        //最後のノードのイベントが終わったときはフリー選択モードとなる
        else
        {
            //パネルずらし
            menu.Controller.View.plane_UI.localPosition += Vector3.right;
            state.Controller.SetState(ST_FreeChoice);
        }
    }

    public void PlayAudioAndSetText(Node node)
    {
        audio.Controller.SetAudioSE(node.se);
        audio.Controller.SetAudioVoice(node.vc);
        explain.Controller.SetText(node.explain);
    }
    #endregion
}
