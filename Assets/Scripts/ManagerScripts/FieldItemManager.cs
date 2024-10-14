using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IngameObject      // "ObjectPool"이라는 이름이 혹시라도 겹칠까봐 별도의 namespace 선언
{
    public class ObjectPool<T>          // Bullet과 FieldItem을 모두 관리하는 데에 사용되므로 제네릭클래스로 선언
    {
        protected GameObject[] pool;             // ID로 객체를 관리하기 위한 GameObject 배열

        public ObjectPool(GameObject[] _pool)   // 생성자에서 배열을 그대로 받아서 사용
        {
            pool = _pool;
        }

        // ID를 기준으로 객체를 활성화 또는 비활성화하는 함수
        public void SetActive(bool _active, int _id)
        {
            if (pool == null || _id >= pool.Length) // 예외 처리: 풀이 초기화되지 않았거나, 잘못된 ID일 경우
                return;

            pool[_id].SetActive(_active); // 해당 ID의 객체를 활성화 또는 비활성화
        }

        // ID를 기준으로 특정 객체를 반환하는 함수
        public T Get(int _id)
        {
            if (pool == null || _id >= pool.Length) // 예외 처리
                return default(T); // 잘못된 ID일 경우 기본값 반환

            GameObject obj = pool[_id]; // 해당 ID의 GameObject를 가져옴
            return obj.GetComponent<T>(); // GameObject에서 T 타입의 객체반환
        }
    }
}


public class FieldItemManager : MonoBehaviour
{
    private IngameObject.ObjectPool<Bullet> bulletPool;     // Bullet풀
    private IngameObject.ObjectPool<FieldItem> itemPool;    // Item풀
    private HashSet<int> remainingItemIDSet = new();        // 현재 남아있는 아이템의 ID를 관리하는 해시셋

    public GameObject[] tbullets;

    [SerializeField]
    private int count = 1;                                   // 한 종류당 사용될 아이템 수
    public PhotonView pv;


    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    private int bidx = 0;
    public Bullet TEST_GetBullet()
    {
        bidx %= tbullets.Length;

        return Instantiate(tbullets[bidx++].GetComponent<Bullet>());
    }

    [PunRPC]
    public void InitFieldItemManager()
    {
        GameObject[] itemPrefabs = Resources.LoadAll<GameObject>("Prefab/ItemCase_Prefab/");           // Resources에서 모든 아이템프리펩 가져오기
        GameObject[] items = new GameObject[itemPrefabs.Length * count];                               // 사용될 아이템 갯수만틈 배열크기 할당
        GameObject[] bullets = new GameObject[itemPrefabs.Length * count];                             // 사용될 총알 갯수만큼 배열크기 할당
        tbullets = Resources.LoadAll<GameObject>("Prefab/Bullet_Prefab/");
        int id = 0;     // ID값으로 각 아이템들을 관리
        for (int i = 0; i < itemPrefabs.Length; i++)    // 모든 종류의 아이템프리펩 생성
        {
            for (int j = 0; j < count; j++)                 // 정해진 갯수만큼 생성
            {
                // 아이템 오브젝트 Instantiate
                GameObject go = Instantiate(itemPrefabs[i], Vector3.zero, itemPrefabs[i].transform.rotation);
                FieldItem item = go.GetComponent<FieldItem>();
                
                items[id] = go;
                int captureID = id;
                item.InitFieldItem(id, () => { BulletDie(captureID); }); // 람다식으로 Bullet에 풀링함수 전달
                bullets[id] = item.bulletObject;     // 아이템에 맞는 Bullet 저장

                remainingItemIDSet.Add(id);       // 남아있는 아이템으로 추가
                id++;
            }
        }

        this.bulletPool = new IngameObject.ObjectPool<Bullet>(bullets);     // BulletPool 정의
        this.itemPool = new IngameObject.ObjectPool<FieldItem>(items);      // ItemPool 정의
    }

    // 플레이어가 아이템을 먹었을때 호출되는 함수
    public void ItemDie(int _id)
    {
        pv.RPC("ItemDieNetwork", RpcTarget.All, _id);
    }
    [PunRPC]
    public void ItemDieNetwork(int _id)
    {
        itemPool.SetActive(false, _id);         // 해당 아이템 비활성화
    }

    // Bullet이 기능을 다 해서 Polling하는 함수
    public void BulletDie(int _keyId)
    {
        bulletPool.SetActive(false, _keyId);    // Bullet비활성화
        remainingItemIDSet.Add(_keyId);         // 해당ID를 사용가능Set에 추가
    }

    // 요청한 ID에 해당하는 Bullet을 제공하는 함수
    public Bullet GetBullet(int _id)            // id에 해당하는 Bullet을 제공하는 함수
    {
        Bullet bullet = bulletPool.Get(_id);    // 풀에서 받아온다
        bulletPool.SetActive(true, _id);        // 활성화

        return bullet;  // 반환
    }

    public void CreateFieldItem(Vector3 _pos)
    {
        //return;
        if (!PhotonNetwork.IsMasterClient)      // 아이템생성은 마스터 클라이언트에서만 담당
            return;
        if (remainingItemIDSet.Count == 0)      // 남아있는 아이템이 없다면 Return
            return;
       // HashSet자료구조의 무순서성을 반영하여 랜덤한 첫번째 요소를 생성 
        pv.RPC("ReleaseItem", RpcTarget.All, remainingItemIDSet.ToList()[0], _pos);    
    }

    [PunRPC]
    public void ReleaseItem(int _id, Vector3 _pos)  // ID로 해당 아이템을 풀에서 꺼내 활성화하고 위치 지정하는 함수
    {
        GameObject itemObj = itemPool.Get(_id).gameObject;     

        remainingItemIDSet.Remove(_id);          // 남아있는 아이템에서 제거
        itemObj.SetActive(true);                 // 활성화
        itemObj.transform.position = _pos;       // 위치 지정
    }



    

}
