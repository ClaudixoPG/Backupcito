using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnTime = 1.5f;
    public float time = 0.0f;
    public Player player;
    public TextMeshProUGUI liveText;

    // Update is called once per frame
    void Update()
    {
        CreateEnemy();
        UpdateCanvas();
    }

    void UpdateCanvas()
    {
        liveText.text = "Life: " + player.lives;
    }

    private void CreateEnemy()
    {
        time += Time.deltaTime;
        if (time > spawnTime)
        {
            Instantiate(enemyPrefab, new Vector3(Random.Range(-8.0f, 8.0f), 7.0f, 0), Quaternion.identity);
            time = 0.0f;
        }
    }


}
