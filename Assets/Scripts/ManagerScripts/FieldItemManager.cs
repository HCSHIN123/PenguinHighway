using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IngameObject      // "ObjectPool"�̶�� �̸��� Ȥ�ö� ��ĥ��� ������ namespace ����
{
    public class ObjectPool<T>          // Bullet�� FieldItem�� ��� �����ϴ� ���� ���ǹǷ� ���׸�Ŭ������ ����
    {
        protected GameObject[] pool;             // ID�� ��ü�� �����ϱ� ���� GameObject �迭

        public ObjectPool(GameObject[] _pool)   // �����ڿ��� �迭�� �״�� �޾Ƽ� ���
        {
            pool = _pool;
        }

        // ID�� �������� ��ü�� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ�ϴ� �Լ�
        public void SetActive(bool _active, int _id)
        {
            if (pool == null || _id >= pool.Length) // ���� ó��: Ǯ�� �ʱ�ȭ���� �ʾҰų�, �߸��� ID�� ���
                return;

            pool[_id].SetActive(_active); // �ش� ID�� ��ü�� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ
        }

        // ID�� �������� Ư�� ��ü�� ��ȯ�ϴ� �Լ�
        public T Get(int _id)
        {
            if (pool == null || _id >= pool.Length) // ���� ó��
                return default(T); // �߸��� ID�� ��� �⺻�� ��ȯ

            GameObject obj = pool[_id]; // �ش� ID�� GameObject�� ������
            return obj.GetComponent<T>(); // GameObject���� T Ÿ���� ��ü��ȯ
        }
    }
}


public class FieldItemManager : MonoBehaviour
{
    private IngameObject.ObjectPool<Bullet> bulletPool;     // BulletǮ
    private IngameObject.ObjectPool<FieldItem> itemPool;    // ItemǮ
    private HashSet<int> remainingItemIDSet = new();        // ���� �����ִ� �������� ID�� �����ϴ� �ؽü�

    public GameObject[] tbullets;

    [SerializeField]
    private int count = 1;                                   // �� ������ ���� ������ ��
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
        GameObject[] itemPrefabs = Resources.LoadAll<GameObject>("Prefab/ItemCase_Prefab/");           // Resources���� ��� ������������ ��������
        GameObject[] items = new GameObject[itemPrefabs.Length * count];                               // ���� ������ ������ƴ �迭ũ�� �Ҵ�
        GameObject[] bullets = new GameObject[itemPrefabs.Length * count];                             // ���� �Ѿ� ������ŭ �迭ũ�� �Ҵ�
        tbullets = Resources.LoadAll<GameObject>("Prefab/Bullet_Prefab/");
        int id = 0;     // ID������ �� �����۵��� ����
        for (int i = 0; i < itemPrefabs.Length; i++)    // ��� ������ ������������ ����
        {
            for (int j = 0; j < count; j++)                 // ������ ������ŭ ����
            {
                // ������ ������Ʈ Instantiate
                GameObject go = Instantiate(itemPrefabs[i], Vector3.zero, itemPrefabs[i].transform.rotation);
                FieldItem item = go.GetComponent<FieldItem>();
                
                items[id] = go;
                int captureID = id;
                item.InitFieldItem(id, () => { BulletDie(captureID); }); // ���ٽ����� Bullet�� Ǯ���Լ� ����
                bullets[id] = item.bulletObject;     // �����ۿ� �´� Bullet ����

                remainingItemIDSet.Add(id);       // �����ִ� ���������� �߰�
                id++;
            }
        }

        this.bulletPool = new IngameObject.ObjectPool<Bullet>(bullets);     // BulletPool ����
        this.itemPool = new IngameObject.ObjectPool<FieldItem>(items);      // ItemPool ����
    }

    // �÷��̾ �������� �Ծ����� ȣ��Ǵ� �Լ�
    public void ItemDie(int _id)
    {
        pv.RPC("ItemDieNetwork", RpcTarget.All, _id);
    }
    [PunRPC]
    public void ItemDieNetwork(int _id)
    {
        itemPool.SetActive(false, _id);         // �ش� ������ ��Ȱ��ȭ
    }

    // Bullet�� ����� �� �ؼ� Polling�ϴ� �Լ�
    public void BulletDie(int _keyId)
    {
        bulletPool.SetActive(false, _keyId);    // Bullet��Ȱ��ȭ
        remainingItemIDSet.Add(_keyId);         // �ش�ID�� ��밡��Set�� �߰�
    }

    // ��û�� ID�� �ش��ϴ� Bullet�� �����ϴ� �Լ�
    public Bullet GetBullet(int _id)            // id�� �ش��ϴ� Bullet�� �����ϴ� �Լ�
    {
        Bullet bullet = bulletPool.Get(_id);    // Ǯ���� �޾ƿ´�
        bulletPool.SetActive(true, _id);        // Ȱ��ȭ

        return bullet;  // ��ȯ
    }

    public void CreateFieldItem(Vector3 _pos)
    {
        //return;
        if (!PhotonNetwork.IsMasterClient)      // �����ۻ����� ������ Ŭ���̾�Ʈ������ ���
            return;
        if (remainingItemIDSet.Count == 0)      // �����ִ� �������� ���ٸ� Return
            return;
       // HashSet�ڷᱸ���� ���������� �ݿ��Ͽ� ������ ù��° ��Ҹ� ���� 
        pv.RPC("ReleaseItem", RpcTarget.All, remainingItemIDSet.ToList()[0], _pos);    
    }

    [PunRPC]
    public void ReleaseItem(int _id, Vector3 _pos)  // ID�� �ش� �������� Ǯ���� ���� Ȱ��ȭ�ϰ� ��ġ �����ϴ� �Լ�
    {
        GameObject itemObj = itemPool.Get(_id).gameObject;     

        remainingItemIDSet.Remove(_id);          // �����ִ� �����ۿ��� ����
        itemObj.SetActive(true);                 // Ȱ��ȭ
        itemObj.transform.position = _pos;       // ��ġ ����
    }



    

}
