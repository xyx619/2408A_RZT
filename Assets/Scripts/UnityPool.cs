using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class UnityPool : MonoBehaviour
{
    public SingleItem itemModel; // �������ɶ�����ж����ģ��  

    private ObjectPool<SingleItem> pool; // �����ʵ��

    int poolSize = 50; // ����صĳ�ʼ��С  

    int poolMaxSize = 100; // ����ص�����С  

    private void Awake()
    {
        // ��ʼ������أ�ָ����������ȡ���ͷź����ٶ���Ļص�����  
        pool = new ObjectPool<SingleItem>(OnCreate, OnGet, OnRelease, OnDes, true, poolSize, poolMaxSize);
    }

    // �Ӷ�����л�ȡһ������  
    public SingleItem Get()
    {
        return pool.Get();
    }

    // �������ͷŻض����  
    public void Release(SingleItem obj)
    {
        pool.Release(obj);
    }

    // ����������е��¶���  
    private SingleItem OnCreate()
    {
        SingleItem go = Instantiate(itemModel); // ʵ����һ���µĶ���  
        return go;
    }

    // ���Ӷ�����л�ȡ����ʱ����  
    private void OnGet(SingleItem obj)
    {
        obj.gameObject.SetActive(true); // �������  
    }

    // ���������ͷŻض����ʱ����  
    private void OnRelease(SingleItem obj)
    {
        obj.gameObject.SetActive(false); // ���ö���  
    }

    // ����������ٶ���ʱ����  
    private void OnDes(SingleItem obj)
    {
        Destroy(obj.gameObject); // ���ٶ���  
    }
}
