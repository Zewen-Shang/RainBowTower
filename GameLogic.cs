using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
enum MoveDir{
    FrontBack,
    LeftRight,
}

public class GameLogic : MonoBehaviour
{
    public Transform topPlate;
    Transform movingPlate;
    MoveDir dir = MoveDir.FrontBack;
    bool reverse = false; 
    public float speed = 2;
    bool lose = false;
    void Start()
    {
        
    }

    Color rainBowColor(float heigh){
        float rate = 2;
        float r = Mathf.Sin(heigh*rate);
        float g = Mathf.Sin(heigh*rate + 120 * Mathf.Deg2Rad);
        float b = Mathf.Sin(heigh*rate + 240 * Mathf.Deg2Rad);
        return new Color(r,g,b);
    }
    void generateNewPlate(){
            AddScore();
            movingPlate = Instantiate(topPlate);
            if(dir == MoveDir.FrontBack){
                dir = MoveDir.LeftRight;
                movingPlate.position = new Vector3(2,topPlate.position.y + 0.1f,topPlate.position.z);
            }else{
                dir = MoveDir.FrontBack;
                 movingPlate.position = new Vector3(topPlate.position.x,topPlate.position.y + 0.1f,-2);
            }
            MeshRenderer render = movingPlate.GetComponent<MeshRenderer>();
            Material mat = render.material;
            mat.color = rainBowColor(movingPlate.position.y);
            GameObject effectPre = (GameObject)Resources.Load("Prefab/NaissanceEffect2");
            Transform effect = Instantiate(effectPre).transform;
            effect.position = movingPlate.position;
            if(dir == MoveDir.FrontBack){
                effect.Rotate(new Vector3(90,0,0));
                effect.GetComponent<ParticleSystem>().startSize = Mathf.Sqrt(Mathf.Pow(movingPlate.localScale.x,2) + Mathf.Pow(movingPlate.localScale.y,2));
            }else{
                effect.GetComponent<ParticleSystem>().startSize = Mathf.Sqrt(Mathf.Pow(movingPlate.localScale.z,2) + Mathf.Pow(movingPlate.localScale.y,2));
            }

            Vector3 camPos = Camera.main.transform.position;
            Camera.main.transform.position = new Vector3(camPos.x,camPos.y + 0.1f,camPos.z);
    }

    void movePlate(){
        GameObject cashPre = (GameObject)Resources.Load("Prefab/AT_Field");
        Transform cashEffect = Instantiate(cashPre).transform;
        Vector3 d;
        if(dir == MoveDir.FrontBack){
            d = new Vector3(0,0,speed);
            if(!reverse && movingPlate.position.z > 2){
                reverse = !reverse;
                cashEffect.position = new Vector3(movingPlate.position.x,movingPlate.position.y,
                movingPlate.position.z + movingPlate.localScale.z/2);
            }else if(reverse && movingPlate.position.z < -2){
                reverse = !reverse;
                cashEffect.position = new Vector3(movingPlate.position.x,movingPlate.position.y,
                movingPlate.position.z - movingPlate.localScale.z/2);
            }
        }else{
            d = new Vector3(speed,0,0);
            if(!reverse && movingPlate.position.x > 2){
                reverse = !reverse;
                cashEffect.position = new Vector3(
                    movingPlate.position.x + movingPlate.localScale.x/2,
                    movingPlate.position.y,movingPlate.position.z
                );
            }else if(reverse && movingPlate.position.x < -2){
                reverse = !reverse;
                cashEffect.position = new Vector3(
                    movingPlate.position.x - movingPlate.localScale.x/2,
                    movingPlate.position.y,movingPlate.position.z
                );
            }
        }
        if(reverse){
            movingPlate.Translate(-1 * d*Time.deltaTime);
        }else{
            movingPlate.Translate(1 * d*Time.deltaTime);
        }
        
    } 

    void stopPlate(){
        GameObject plateEffect = Resources.Load<GameObject>("Prefab/Particle System");
        Instantiate(plateEffect,movingPlate.transform.position + new Vector3(0,0,0),movingPlate.transform.rotation);

        GameObject cutPre = (GameObject)Resources.Load("Prefab/CutPlate");

        Transform stayPlate = null;
        Transform cutPlate = null;
        float movFront = movingPlate.position.z + movingPlate.localScale.z/2;
        float movBack = movingPlate.position.z - movingPlate.localScale.z/2;
        float movLeft = movingPlate.position.x + movingPlate.localScale.x/2;
        float movRight = movingPlate.position.x - movingPlate.localScale.x/2;

        float topFront = topPlate.position.z + topPlate.localScale.z/2;
        float topBack = topPlate.position.z - topPlate.localScale.z/2;
        float topLeft = topPlate.position.x + topPlate.localScale.x/2;
        float topRight = topPlate.position.x - topPlate.localScale.x/2;
        //判断胜负
        if(dir == MoveDir.FrontBack){
            if(movFront < topBack || movBack > topFront){
                lose = true;
                return;
            }
        }else{
            if(movLeft < topRight || movRight > topLeft){
                lose = true;
                return;
            }
        }
        //切割
        if(dir == MoveDir.FrontBack){
            float cutFront,cutBack,stayFront,stayBack;
            if(movingPlate.position.z > topPlate.position.z){
                cutFront = movFront;
                cutBack = topFront;
                stayFront = topFront;
                stayBack = movBack;
            }else{
                cutFront = topBack;
                cutBack = movBack;
                stayFront = movFront;
                stayBack = topBack;
            }
            Destroy(movingPlate.gameObject);
            cutPlate = Instantiate(cutPre).transform;
            cutPlate.GetComponent<MeshRenderer>().material.color = rainBowColor(movingPlate.position.y);
            cutPlate.position = new Vector3(movingPlate.position.x,movingPlate.position.y,(cutFront + cutBack)/2);
            cutPlate.localScale = new Vector3(movingPlate.localScale.x,movingPlate.localScale.y,cutFront-cutBack);
            stayPlate = Instantiate(movingPlate);
            stayPlate.position = new Vector3(stayPlate.position.x,stayPlate.position.y,(stayFront + stayBack)/2);
            stayPlate.localScale = new Vector3(stayPlate.localScale.x,stayPlate.localScale.y,stayFront-stayBack);
        }else{
            float cutLeft,cutRight,stayLeft,stayRight;
            if(movingPlate.position.x > topPlate.position.x){
                cutLeft = movLeft;
                cutRight = topLeft;
                stayLeft = topLeft;
                stayRight = movRight;
            }else{
                cutLeft = topRight;
                cutRight = movRight;
                stayLeft = movLeft;
                stayRight = topRight;
            }
            Destroy(movingPlate.gameObject);
            cutPlate = Instantiate(cutPre).transform;
            cutPlate.GetComponent<MeshRenderer>().material.color = rainBowColor(movingPlate.position.y);
            cutPlate.position = new Vector3((cutLeft + cutRight)/2,movingPlate.position.y,movingPlate.position.z);
            cutPlate.localScale = new Vector3( cutLeft-cutRight,movingPlate.localScale.y,movingPlate.localScale.z);
            stayPlate = Instantiate(movingPlate);
            stayPlate.position = new Vector3( (stayLeft + stayRight)/2,stayPlate.position.y,stayPlate.position.z);
            stayPlate.localScale = new Vector3( stayLeft-stayRight,stayPlate.localScale.y,stayPlate.localScale.z);
        }
        topPlate = stayPlate;
        cutPlate.gameObject.AddComponent<Rigidbody>();
        movingPlate = null;
    } 
    void AddScore()
    {
        Text score = GameObject.Find("Text").GetComponent<Text>();
        score.text = (System.Convert.ToInt32(score.text) + 1).ToString();
    }
    void Update()
    {
        if(lose){
            Time.timeScale = 0;
        }
        if(Input.GetKeyDown(KeyCode.R)){
            Debug.Log("r");
            SceneManager.LoadScene(0);
            Time.timeScale = 1;
        }
        if(movingPlate == null){
            generateNewPlate();
        }
        movePlate();
        if(Input.GetButtonDown("Fire1")){
            stopPlate();
        };
    }
}
