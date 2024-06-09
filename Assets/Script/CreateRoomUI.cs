using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class CreateRoomUI : MonoBehaviour
{


    [SerializeField]
    private List<Image> crewImgs;
    [SerializeField]
    private List<Button> imposterCountButtons;
    [SerializeField]
    private List<Button> maxPlayerCountButtons;
    private CreateGameRoomData roomData;

    void Start()
    {
    for(int i=0; i<crewImgs.Count; i++)
        {
            Material material = Instantiate(crewImgs[i].material);
            crewImgs[i].material = material;
        }     
    roomData = new CreateGameRoomData() { imposterCount = 1,maxPlayerCount=10};
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CreateRoom()
    {
        var manager = BallRoomManager.singleton;
        //방 설정 작업처리
        //
        //
        manager.StartHost();

    }
    public class CreateGameRoomData
    {
        public int imposterCount;
        public int maxPlayerCount;
    }

    public void OnClick_CloseUI()
    {
        this.gameObject.SetActive(false);
    }
}
