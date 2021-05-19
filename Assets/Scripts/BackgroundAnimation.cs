using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimation : MonoBehaviour
{

    [SerializeField] GameObject trees;
    [SerializeField] GameObject frontMountains;
    [SerializeField] GameObject rearMountains;
    //const float TREE_LENGTH;
    //const float OTHER_LENGTH;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        var front = trees.GetComponentsInChildren<Transform>();
        var middle = frontMountains.GetComponentsInChildren<Transform>();
        var back = rearMountains.GetComponentsInChildren<Transform>();

        /*
        backgroundsA[4].transform.position = new Vector3(backgroundsA[4].transform.position.x - Time.deltaTime * 30, backgroundsA[4].transform.position.y, backgroundsA[4].transform.position.z);
        backgroundsB[4].transform.position = new Vector3(backgroundsB[4].transform.position.x - Time.deltaTime * 30, backgroundsB[4].transform.position.y, backgroundsB[4].transform.position.z);
        backgroundsC[4].transform.position = new Vector3(backgroundsC[4].transform.position.x - Time.deltaTime * 30, backgroundsC[4].transform.position.y, backgroundsC[4].transform.position.z);

        for(int i = 4; i > 1; i--)
        {
            backgroundsA[i].transform.position = new Vector3(backgroundsA[i].transform.position.x - Time.deltaTime * i*i * 5, backgroundsA[i].transform.position.y, backgroundsA[i].transform.position.z);
            backgroundsB[i].transform.position = new Vector3(backgroundsB[i].transform.position.x - Time.deltaTime * i*i * 5, backgroundsB[i].transform.position.y, backgroundsB[i].transform.position.z);
            backgroundsC[i].transform.position = new Vector3(backgroundsC[i].transform.position.x - Time.deltaTime * i*i * 5, backgroundsC[i].transform.position.y, backgroundsC[i].transform.position.z);
            if(backgroundsA[i].transform.position.x < -LENGTH)
            {
                backgroundsA[i].transform.position = new Vector3(backgroundsC[i].transform.position.x + LENGTH, backgroundsA[i].transform.position.y, backgroundsA[i].transform.position.z);
            }
            if (backgroundsB[i].transform.position.x < -LENGTH)
            {
                backgroundsB[i].transform.position = new Vector3(backgroundsA[i].transform.position.x + LENGTH, backgroundsB[i].transform.position.y, backgroundsB[i].transform.position.z);
            }
            if (backgroundsC[i].transform.position.x < -LENGTH)
            {
                backgroundsC[i].transform.position = new Vector3(backgroundsB[i].transform.position.x + LENGTH, backgroundsC[i].transform.position.y, backgroundsC[i].transform.position.z);
            }

        }
        */

    }
}