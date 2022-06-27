using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SingleTon : MonoBehaviour {

    #region 코어
    public  static SingleTon st { get { return instance; } }
    private static SingleTon instance;
    public  bool      isMontaDead = false;

    private void Awake() {
        if (instance) {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void OnDestroy() {
        Destroy(gameObject);
    }
    #endregion
    #region Manager
    [Header("Manager")]
    public SoundManagerEX exSM = null;
    #endregion
    [Header("Object")]
    #region Object
    public List<Player> playerPrefs = null;
    public Transform    playersParent = null;
    [System.NonSerialized]
    public Transform[]  players = null;
    public Transform    player
    {

        set
        {
            _player = value;
        }
        get { return _player; }
    }

    private Transform _player = null;
    #endregion
}
