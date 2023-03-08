using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Этот код предназначен для оптимизации производительности приложения, так как создание и удаление объектов в процессе выполнения может приводить к задержкам в работе приложения.
//Вместо этого, пул объектов позволяет повторно использовать объекты, которые уже были созданы, что уменьшает нагрузку на процессор и память.
public class PlayerAfterImagePool : MonoBehaviour
{
    [SerializeField]
    private GameObject afterImagePrefab;

    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    public static PlayerAfterImagePool Instance { get; private set; }  

    private void Awake()
    {
        Instance = this;
        GrowPool();
    }

    private void GrowPool()
    {
        for (int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(afterImagePrefab);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }


    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }

    public GameObject GetFromPool()
    {
        if(availableObjects.Count == 0)
        {
            GrowPool();
        }

        var instance = availableObjects.Dequeue();
        instance.SetActive(true);
        return instance;
    }
}
/*Первые три строки кода используются для импорта необходимых пространств имен. Затем следует определение класса PlayerAfterImagePool, который наследует MonoBehaviour, чтобы использовать некоторые из функций жизненного цикла, предоставляемых Unity.

Поле afterImagePrefab - это префаб объекта, который будет использоваться для создания новых объектов пула. Queue availableObjects содержит доступные объекты, которые могут быть использованы для создания новых объектов при необходимости.

Свойство Instance создает статический экземпляр класса PlayerAfterImagePool, который может быть доступен из любого места в коде. Метод Awake() вызывается при создании объекта и устанавливает статический экземпляр Instance в текущий объект и вызывает метод GrowPool() для заполнения пула объектов.

Метод GrowPool() используется для добавления новых объектов в пул. Он создает 10 новых экземпляров префаба и добавляет их в очередь доступных объектов, используя метод AddToPool().

Метод AddToPool() используется для добавления новых объектов в очередь доступных объектов. Он деактивирует переданный ему объект, используя метод SetActive(false), и затем добавляет его в очередь доступных объектов.

Метод GetFromPool() используется для извлечения объекта из очереди доступных объектов. Если доступных объектов нет, вызывается метод GrowPool() для добавления новых объектов в пул. Затем извлекается первый объект из очереди доступных объектов, активируется с помощью метода SetActive(true) и возвращается.*/