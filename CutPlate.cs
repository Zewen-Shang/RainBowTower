using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutPlate : MonoBehaviour
{
    void Update()
    {
        
        Color a = GetComponent<MeshRenderer>().material.color; //获取颜色
        a.a -= Time.deltaTime * 0.5f; //a值逐渐减小
        //将改变后的颜色赋值给自身(颜色逐渐透明)
        GetComponent<MeshRenderer>().material.color = a; 
        if (a.a < 0) //当透明度小到一定时候
            Destroy(gameObject); //销毁自身
    }
}
