using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class SingleItem : MonoBehaviour
{   
    public Vector3 startPos = new Vector3(0, 0, 0);// 起始位置
    
    public float width;// 单元格宽度

    public float height;// 单元格高度

    public List<Sprite> sprites;// 可用的精灵图片列表
    
    public int type;// 当前单元格的类型

    public SpriteRenderer itemSprite; // 精灵渲染器，用于显示单元格的图片

    public event Action<SingleItem> mouseButton;// 鼠标点击事件
    
    public int col;// 单元格的列索引

    public int row;// 单元格的行索引

    public GameObject block;// 用于显示选中状态的块 

    private void Start()
    {
        // 初始化时设置为未选中状态
    }

    // 初始化单元格
    public void BirthItem(int row, int col)
    {
       
        SetItemIndex(row, col); // 设置单元格的行列索引
        
        SetItemWorldPos(row, col);// 设置单元格在场景中的位置
        
        SetItemSpritemRandom();// 随机设置单元格的样式图片
    }

    // 注册鼠标点击事件
    public void RegisterMouseButtonAction(Action<SingleItem> itemAction)
    {
        mouseButton += itemAction;
    }

    // 设置单元格的选中状态
    public void SetSelect(bool value)
    {
        block.SetActive(value);
    }

    // 比较当前单元格与另一个单元格是否相邻
    public bool CompareTypeWith(SingleItem compareItem)
    {
        // 同列且行索引相差1  
        if (compareItem.col == col && Math.Abs(compareItem.row - row) == 1)
        {
            return true;
        }
        // 同行且列索引相差1  
        if (compareItem.row == row && Math.Abs(compareItem.col - col) == 1)
        {
            return true;
        }
        return false;
    }

    // 随机设置单元格的样式图片
    private void SetItemSpritemRandom()
    {
        
        int index = UnityEngine.Random.Range(0, sprites.Count);// 随机选择一个图片索引
        
        type = index;// 设置单元格类型

        itemSprite.sprite = sprites[index];// 设置精灵图片
    }

    // 设置单元格的行列索引
    public void SetItemIndex(int r, int c)
    {   
        col = c;
        row = r;
    }

    // 移动单元格到指定的行列位置
    public void MoveTo(int r, int c, int t)
    {
        
        SetItemIndex(r, c);// 更新行列索引

        Vector3 pos = startPos + new Vector3(c * width, -r * height, 0);// 计算目标位置

        transform.DOMove(pos, t);// 使用动画移动到目标位置 
    }

    // 设置单元格在场景中的位置
    private void SetItemWorldPos(int r, int c)
    {
        // 计算并设置位置  
        transform.position = startPos + new Vector3(+c * width, -r * height, 0);
    }

    // 鼠标点击事件触发  
    private void OnMouseDown()
    {
        if (mouseButton != null)
        {
            mouseButton(this);
        }
    }
}
