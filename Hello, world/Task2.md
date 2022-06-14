2. Написать немного кода на Scala и F# с использованием уникальных возможностей языка - Pipe operator, Discriminated Union и т.д. Вызвать написанный код из обычных соответствующих ООП языков (Java и С#) и посмотреть во что превращается написанный раннее код после декомпиляции в них. 

### **Запуск Scala из Java**

Поскольку код на языке Scala перед выполнением компилируется в байт-код, точно так же как и код на Java. Поэтому в одном проекте можно использовать классы написанные, как на Javaб так и на Scala. Создав проект на Java, я добавил в него Scala Framework и добавил Scala класс.

Напишем функцию с использованием pipe operator на Scala. 

```java
import scala.util.chaining._

class MyClass {

  def plus1(i: Int) : Int = i + 1
  def double(i: Int) : Int = i * 2
  def square(i: Int) : Int = i * i
  def doSomethingInPipe(i : Int) : Int = i.pipe(plus1).pipe(double) // method with pipe operator
  def doSomething(i : Int) : Int = double(plus1(i)) // classical presentation without pipe

}
```

Как уже упоминалось, механизм работы у языков одинаковый (поскольку оба работают на JVM), то мы можем создать экземпляр MyClass в классе Java и получить значение x:

```java
public class Main {

    public static void main(String[] args) {
        MyClass tmp = new MyClass();
        System.out.println(tmp.doSomethingInPipe(4));
    }
}
```

Сначала мы прибавим 1 к 4, а потом умножим полученный результат на 2. Получим на консоли число 10.

При декомпиляции Scala в Java мы можем заметить наши объявленные функции. Прочитать и понять некоторые из них не составляет труда, но функция составелнная с помощью pipe operator читается уже сложнее:

```java
public int doSomethingInPipe(final int i) {
       var10000 = .MODULE$;
      Integer var3 = (Integer).MODULE$.scalaUtilChainingOps(BoxesRunTime.boxToInteger(i));
      Integer var2 = (Integer)var10000.scalaUtilChainingOps(scala.util.ChainingOps..MODULE$.pipe$extension(var3, (ix) -> {
         return this.plus1(ix);
      }));
      return BoxesRunTime.unboxToInt(scala.util.ChainingOps..MODULE$.pipe$extension(var2, (ix) -> {
         return this.double(ix);
      }));
   }
```

Можно заметить, что вычисления производятся поэтапно, создавая прмежуточные переменные _var2_ и _var3_. Так же можно предположить, что происходит boxing и unboxing переменных в процессе выполнения.

### **Запуск F# из C#**

Механизм взаимодействия C# и F# имеет логику, описанную выше. Оба языка базируются на платформе .NET и код преобразуется в промежуточное состояние - IL код. Поэтому в одном решении мы можем создать два проекта: один на языке C#, второй - на F#. И дальше ссылаясь на F# проект запустить его из C#.

Примеры использования pipe forward operator и discriminated union в F#:

```c#
module BusinessLogic
open System
let pi = 3.141592654

type Shape =
    | Rectangle of width : float * length : float
    | Circle of radius : float
    
let area myShape =
    match myShape with
    | Circle radius -> pi * radius * radius
    | Rectangle (h, w) -> h * w

let square x = x * x
let toStr (x : int) = x.ToString()
let rev (x : string) = String(Array.rev (x.ToCharArray()))
let result x = x |> square |> toStr |> rev
```

Вызов методов из C# и полученные результаты:

```c#
using System;

// example of pipe operator from C#
var examplePipe = BusinessLogic.result(25);
Console.WriteLine(examplePipe);

// example of discriminated union
var shape = BusinessLogic.Shape.NewCircle(14);
Console.WriteLine(BusinessLogic.area(shape));

// Вывод в консоли:
526
615,752160184
```

В декомпилированном коде можно заметить перегруженные функции CompareTo, GetHashCode Equals и др. Так же были сгенерированы getter'ы и setter'ы для полей объектов в discriminated union. 