using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class ItemManager : MonoBehaviour
{
    public int col; // 列数

    public int row; // 行数

    public int time; // 交换时间

    private int XiaoChuCount = 0; // 消除的数量  

    private UnityPool itemPool; // Unity对象池，用于管理游戏对象的复用
    private SingleItem[,] itemList; // 存储所有游戏元素的二维数组
    private List<SingleItem> deleteList; // 存储需要消除的元素列表

    private SingleItem itemFitst; // 当前选中的第一个元素

    // Start is called before the first frame update  
    void Start()
    {
        deleteList = new List<SingleItem>(); // 初始化消除列表
        itemPool = GetComponent<UnityPool>(); // 获取对象池组件
        itemList = new SingleItem[row, col]; // 初始化游戏元素数组

        // 初始化游戏元素
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                SingleItem item = NewItem(i, j);
                itemList[i, j] = item;
            }
        }

        // 检查是否有可以消除的元素
        bool canDelete = CheckDelete();
        if (canDelete)
        {
            DeleteItem(); // 执行消除  
        }
    }

    // 创建新元素
    SingleItem NewItem(int row, int col)
    {
        SingleItem item = itemPool.Get(); // 从对象池获取新元素
        item.transform.SetParent(transform); // 设置父对象
        item.BirthItem(row, col); // 初始化元素位置

        // 注册鼠标点击事件
        Action<SingleItem> itemAction = SelectedItem;
        item.RegisterMouseButtonAction(itemAction);
        return item;
    }

    // 处理选中元素的逻辑
    private void SelectedItem(SingleItem item)
    {
        if (itemFitst == null) // 如果没有选中第一个元素
        {
            itemFitst = item;  
            itemFitst.SetSelect(true); // 设置选中状态
        }
        else
        {
            itemFitst.SetSelect(false); // 取消第一个元素的选中状态
            if (item.CompareTypeWith(itemFitst)) // 如果两个元素类型相同
            {
                StartCoroutine(DelayTimeExchange(item, itemFitst)); // 延时交换
            }

            itemFitst = null; // 重置选中状态
        }
    }

    // 交换两个元素的位置
    private void Exchange(SingleItem item1, SingleItem item2)
    {
        Debug.Log("010000");
        Debug.Log("123123");
        // 更新元素在数组中的位置  
        itemList[item1.row, item1.col] = item2;
        itemList[item2.row, item2.col] = item1;

        // 交换行列索引  
        int temp1_col = item1.col, temp1_row = item1.row;
        int temp2_col = item2.col, temp2_row = item2.row;

        item1.SetItemIndex(temp2_row, temp2_col);
        item2.SetItemIndex(temp1_row, temp1_col);

        // 执行移动动画
        item1.MoveTo(item1.row, item1.col, time);
        item2.MoveTo(item2.row, item2.col, time);
    }

    // 延时交换两个元素
    IEnumerator DelayTimeExchange(SingleItem item1, SingleItem item2)
    {
        Exchange(item1, item2); // 交换元素  
        yield return new WaitForSeconds(time); // 等待交换动画完成  

        bool canDelete = CheckDelete(); // 检查是否有可以消除的元素  
        if (canDelete)
        {
            DeleteItem(); // 执行消除
            XiaoChuCount++; // 增加消除计数  
            if (XiaoChuCount >= 3) // 如果连续消除次数达到3次  
            {
                XiaoChuCount = 0;
                Camera.main.transform.DOShakePosition(0.5f, 0.5f, 10, 90, false, true); // 相机抖动效果  
            }
        }
        else
        {
            Exchange(item1, item2); // 如果没有可以消除的元素，交换回原位置  
        }
    }

    // 执行消除逻辑
    private void DeleteItem()
    {
        int count = deleteList.Count;
        for (int i = 0; i < count; i++)
        {
            SingleItem item = deleteList[i];
            int tmpR = item.row, tmpC = item.col;
            itemPool.Release(item); // 释放元素到对象池  
            itemList[tmpR, tmpC] = null; // 清空数组中的位置  

            // 将上方的元素依次下移  
            for (int j = tmpR - 1; j >= 0; j--)
            {
                SingleItem itemTmp = itemList[j, tmpC];
                itemList[j + 1, itemTmp.col] = itemTmp;
                itemList[j, tmpC] = null;
                itemTmp.SetItemIndex(j + 1, itemTmp.col);
                itemTmp.MoveTo(itemTmp.row, itemTmp.col, time);
            }

            // 在顶部生成新元素  
            SingleItem newItem = itemPool.Get();
            newItem.transform.SetParent(transform);
            newItem.BirthItem(-1, item.col);
            newItem.MoveTo(0, newItem.col, time);
            itemList[0, newItem.col] = newItem;
        }

        deleteList = new List<SingleItem>(); // 清空消除列表  
        StartCoroutine(CheckDeleteNext()); // 检查下一次消除  
    }

    // 检查下一次是否可以消除  
    IEnumerator CheckDeleteNext()
    {
        yield return new WaitForSeconds(time);
        bool canDelete = CheckDelete();
        if (canDelete)
        {
            DeleteItem();
        }
    }

    // 将元素添加到消除列表  
    private void AddToDeleteList(SingleItem newItem)
    {
        int index = deleteList.FindIndex(item => item.row == newItem.row && item.col == newItem.col);
        if (index == -1)
        {
            deleteList.Add(newItem);
        }
    }

    // 检查是否有可以消除的元素
    private bool CheckDelete()
    {
        bool canDelete = false;

        for (int c = 0; c < col; c++)
        {
            for (int r = 0; r < row; r++)
            {
                // 检查垂直方向是否有三个相同的元素  
                if (r < row - 2 && itemList[r, c] != null && itemList[r + 1, c] != null && itemList[r + 2, c] != null)
                {
                    if (itemList[r, c].type == itemList[r + 1, c].type && itemList[r, c].type == itemList[r + 2, c].type)
                    {
                        AddToDeleteList(itemList[r, c]);
                        AddToDeleteList(itemList[r + 1, c]);
                        AddToDeleteList(itemList[r + 2, c]);
                        canDelete = true;
                    }
                }

                // 检查水平方向是否有三个相同的元素  
                if (c < col - 2 && itemList[r, c] != null && itemList[r, c + 1] != null && itemList[r, c + 2] != null)
                {
                    if (itemList[r, c].type == itemList[r, c + 1].type && itemList[r, c].type == itemList[r, c + 2].type)
                    {
                        AddToDeleteList(itemList[r, c]);
                        AddToDeleteList(itemList[r, c + 1]);
                        AddToDeleteList(itemList[r, c + 2]);
                        canDelete = true;
                    }
                }
            }
        }
        return canDelete;
    }
}
