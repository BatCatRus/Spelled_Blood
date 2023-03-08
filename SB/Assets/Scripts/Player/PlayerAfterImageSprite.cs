using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*Этот код используется для создания эффекта "afterimage" для игрока в игре на Unity. 
 * "Afterimage" - это эффект, когда за движущимся объектом остается след, который постепенно исчезает.*/
public class PlayerAfterImageSprite : MonoBehaviour
{
    [SerializeField]
    private float activeTime = 0.1f;
    private float timeActivated;
    private float alpha;
    [SerializeField]
    private float alphaSet = 0.8f;
    [SerializeField]
    private float alphaDecay = 0.85f;

    private Transform player;

    private SpriteRenderer SR;
    private SpriteRenderer playerSR;

    private Color color;

    private void OnEnable()
    {
        SR = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerSR = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        SR.sprite = playerSR.sprite;
        transform.position = player.position;
        transform.rotation = player.rotation;
        timeActivated = Time.time;
    }

    private void Update()
    {
        alpha -= alphaDecay * Time.deltaTime;
        color = new Color(1f, 1f, 1f, alpha);
        SR.color = color;

        if (Time.time >= (timeActivated + activeTime))
        {
            PlayerAfterImagePool.Instance.AddToPool(gameObject);
        }

    }
}
/* В начале кода мы подключаем необходимые пространства имен: "System.Collections", "System.Collections.Generic" и "UnityEngine". 
 * Далее объявляются переменные класса: "activeTime" - время, которое "afterimage" будет активно, "timeActivated" - время, 
 * когда "afterimage" был активирован, "alpha" - прозрачность спрайта, "alphaSet" - начальное значение прозрачности, "alphaMultiplier" - множитель прозрачности, 
 * "player" - ссылка на игрока, "SR" - ссылка на компонент "SpriteRenderer" "afterimage", 
 * "PlayerSr" - ссылка на компонент "SpriteRenderer" игрока, "color" - цвет спрайта.
 * Затем в методе "OnEnable()" мы получаем ссылки на компоненты "SpriteRenderer" "afterimage" и игрока, 
 * устанавливаем начальное значение прозрачности и спрайт "afterimage" равным спрайту игрока, а также устанавливаем позицию и поворот "afterimage" равным позиции и повороту игрока в момент активации "afterimage".
 * В методе "Update()" мы изменяем значение прозрачности "afterimage" путем умножения начального значения прозрачности на множитель прозрачности, 
 * и устанавливаем цвет спрайта "afterimage" с помощью создания нового объекта "Color" с заданными значениями RGBA. 
 * Если время активации "afterimage" превышает время, на которое "afterimage" был задействован, то мы отправляем "afterimage" в пул объектов.*/
