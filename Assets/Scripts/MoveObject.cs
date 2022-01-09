using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MoveObject : MonoBehaviour
{

    //Добавляет нулевое поле на котором можно указать вектор кординат Y X Z
    [SerializeField] private Vector3 movePosition;
    //
    [SerializeField] [Range(0,2)] private float moveSpeed;
    //Добавляет поле с ползунком в котором 0 начало кординат 1 - 100% пройденных кординат
    [SerializeField] [Range(0,1)] private float moveProgress;
    //Создаем стартовое поле в которое загоним стартовую позицию обьекта
    private Vector3 startPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        //во время запуска загоняем стартовую позицию обьекта в поле startPos
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        moveProgress = Mathf.PingPong(Time.time * moveSpeed, 1);
        
        Vector3 offset = movePosition * moveProgress;
        transform.position = startPosition + offset;
    }
}
