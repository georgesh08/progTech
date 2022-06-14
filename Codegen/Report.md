## Отчет по лабораторной работе № 2
---

1. ### Создание сервера на Java
Для поднятия сервера на Java был использован шаблон серверного приложения с использованием *Spring Boot Framework*. В качестве сущностей используются *Владелец (Owner)* и *Кот (Cat)*. Классы используются как POJO. 

```java
// Cat.java
package tech.is.service.entities;

public class Cat {
    public int Id;
    public int Owner;
    public String Name;
    public String Color;
}

// Owner.java
package tech.is.service.entities;

import java.util.ArrayList;
import java.util.List;

public class Owner {
    public int Id;
    public String Name;
    public List<Integer> Cats = new ArrayList<>();
}
```


Сервер предоставляет Post и Get запросы, которые в свою очередь могут принимать данные как с помощью Body-request'ов, так и с помощью параметров, переданных в строке запроса.

Сущетсвует так же произвольная "база данных" для каждой из сущностей, представляющая собой массивы влдельцев и котов, которые существуют в runtime.

----

2. Парсер сервера

Парсер читает каждый файл сервера, предствляя код файла, как строку. Для получения семантической модели сервера нас иинтересуют файлы *Cat.java,Owner.java,CatsController.java,OwnersController.java*. 

Для получения данных из наших моделей были созданы DTO-классы:

```c#
// ArgDeclaration.cs
namespace JavaParser.Tools
{
    public struct ArgDeclaration
    {
        public string Type;
        public string Name;
    }
} 


// DtoConstructor.cs
using System.Collections.Generic;

namespace JavaParser.Tools
{
    public class DtoConstructor
    {
        public string ClassName;
        private List<Field> _objectFields = new();

        public void AddField(Field field)
        {
            _objectFields.Add(field);
        }

        public List<Field> Fields()
        {
            return _objectFields;
        }
    }
} 

// Field.cs
namespace JavaParser.Tools
{
    public class Field
    {
        public string Type;
        public string Name;
    }
} 

// MethodDeclaration.cs
using System.Collections.Generic;

namespace JavaParser.Tools
{
    public class MethodDeclaration
    {
        public string RequestType { get; set; }
        public string MethodName { get; set; }
        public string ReturnType { get; set; }
        public string Url { get; set; }
        public List<ArgDeclaration> ArgList = new();
    }
} 
```

Для извлечения интересующей нас информации были использованы регулярные выражения.

3. Кодогенерация

Для корректной работы клиента нам требуется генерация двух вещей: генерация сущностей, которые используются сервером; генерация клиента, который будет взаимодействовать с сервером.

Сначала была проанализирована модель сервера и по ней строилось синтаксическое дерево, которое отвечает за наш клиент. 




