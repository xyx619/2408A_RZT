using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class SingleItem : MonoBehaviour
{   
    public Vector3 startPos = new Vector3(0, 0, 0);// ��ʼλ��
    
    public float width;// ��Ԫ����

    public float height;// ��Ԫ��߶�

    public List<Sprite> sprites;// ���õľ���ͼƬ�б�
    
    public int type;// ��ǰ��Ԫ�������

    public SpriteRenderer itemSprite; // ������Ⱦ����������ʾ��Ԫ���ͼƬ

    public event Action<SingleItem> mouseButton;// ������¼�
    
    public int col;// ��Ԫ���������

    public int row;// ��Ԫ���������

    public GameObject block;// ������ʾѡ��״̬�Ŀ� 

    private void Start()
    {
        // ��ʼ��ʱ����Ϊδѡ��״̬
    }

    // ��ʼ����Ԫ��
    public void BirthItem(int row, int col)
    {
       
        SetItemIndex(row, col); // ���õ�Ԫ�����������
        
        SetItemWorldPos(row, col);// ���õ�Ԫ���ڳ����е�λ��
        
        SetItemSpritemRandom();// ������õ�Ԫ�����ʽͼƬ
    }

    // ע��������¼�
    public void RegisterMouseButtonAction(Action<SingleItem> itemAction)
    {
        mouseButton += itemAction;
    }

    // ���õ�Ԫ���ѡ��״̬
    public void SetSelect(bool value)
    {
        block.SetActive(value);
    }

    // �Ƚϵ�ǰ��Ԫ������һ����Ԫ���Ƿ�����
    public bool CompareTypeWith(SingleItem compareItem)
    {
        // ͬ�������������1  
        if (compareItem.col == col && Math.Abs(compareItem.row - row) == 1)
        {
            return true;
        }
        // ͬ�������������1  
        if (compareItem.row == row && Math.Abs(compareItem.col - col) == 1)
        {
            return true;
        }
        return false;
    }

    // ������õ�Ԫ�����ʽͼƬ
    private void SetItemSpritemRandom()
    {
        
        int index = UnityEngine.Random.Range(0, sprites.Count);// ���ѡ��һ��ͼƬ����
        
        type = index;// ���õ�Ԫ������

        itemSprite.sprite = sprites[index];// ���þ���ͼƬ
    }

    // ���õ�Ԫ�����������
    public void SetItemIndex(int r, int c)
    {   
        col = c;
        row = r;
    }

    // �ƶ���Ԫ��ָ��������λ��
    public void MoveTo(int r, int c, int t)
    {
        
        SetItemIndex(r, c);// ������������

        Vector3 pos = startPos + new Vector3(c * width, -r * height, 0);// ����Ŀ��λ��

        transform.DOMove(pos, t);// ʹ�ö����ƶ���Ŀ��λ�� 
    }

    // ���õ�Ԫ���ڳ����е�λ��
    private void SetItemWorldPos(int r, int c)
    {
        // ���㲢����λ��  
        transform.position = startPos + new Vector3(+c * width, -r * height, 0);
    }

    // ������¼�����  
    private void OnMouseDown()
    {
        if (mouseButton != null)
        {
            mouseButton(this);
        }
    }
}
