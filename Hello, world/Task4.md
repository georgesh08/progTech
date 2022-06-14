4. Изучить инструменты для оценки производительности в C# и Java. Написать несколько алгоритмов сортировок (и взять стандартную) и запустить бенчмарки (в бенчмарках помимо времени выполнения проверить аллокации памяти). В отчёт написать про инструменты для бенчмаркинга, их особености, анализ результатов проверок.

### **Бенчмаркинг в С#**

Для бенчмаркинга в языке C# мною был использован пакет BenchmarkDotNet. Я создал отдельный класс, в котором пометил методы, которые должны отслеживаться бенчмарком и запустил тесты.

В качестве оценки производительности использовались встроенная сортировка, сортировка слиянием и сортировка пузырьком.

Код класса с методами соритровок:

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Benchmark
{
    [MemoryDiagnoser]
    public class BenchmarkTest
    {
        private static Random _rand = new Random();
        private static List<int> _arr1 = Enumerable.Range(0, 10).OrderBy(_ => _rand.Next()).ToList();
        private List<int> _arr2 = _arr1;
        private List<int> _arr3 = _arr1;

        [Benchmark]
        public void StandardSort()
        {
            _arr2.Sort();
        }

        [Benchmark]
        public void MergeSort()
        {
            MergeSort(_arr1, 0, _arr1.Count - 1);
        }

        private void MergeSort(List<int> arr, int lowIndex, int highIndex)
        {
            if (lowIndex < highIndex)
            {
                var middleIndex = (lowIndex + highIndex) / 2;
                MergeSort(arr, lowIndex, middleIndex);
                MergeSort(arr, middleIndex + 1, highIndex);
                Merge(arr, lowIndex, middleIndex, highIndex);
            }
        }

        private void Merge(List<int> array, int lowIndex, int middleIndex, int highIndex)
        {
            var left = lowIndex;
            var right = middleIndex + 1;
            var tempArray = new int[highIndex - lowIndex + 1];
            var index = 0;

            while (left <= middleIndex && right <= highIndex)
            {
                if (array[left] < array[right])
                {
                    tempArray[index] = array[left];
                    left++;
                }
                else
                {
                    tempArray[index] = array[right];
                    right++;
                }

                index++;
            }

            for (var i = left; i <= middleIndex; i++)
            {
                tempArray[index] = array[i];
                index++;
            }

            for (var i = right; i <= highIndex; i++)
            {
                tempArray[index] = array[i];
                index++;
            }

            for (var i = 0; i < tempArray.Length; i++)
            {
                array[lowIndex + i] = tempArray[i];
            }
        }

        [Benchmark]
        public void BubbleSort()
        {
            int tmp;
            for (int i = 0; i < _arr3.Count - 1; i++)
            {
                for (int j = i+1; j < _arr3.Count; j++)
                {
                    if (_arr3[i] > _arr3[j])
                    {
                        tmp = _arr3[i];
                        _arr3[i] = _arr3[j];
                        _arr3[j] = tmp;
                    }
                }
            }
        }
    }
}
```

Класс в котором запускаются бенчмарки:

```c#
using Benchmark;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<BenchmarkTest>();
```

Были произведены запуски при размере массива N = 10, 100 и 1000.

Результаты:

N = 10:

|       Method |      Mean |    Error |    StdDev |  Gen 0 | Allocated |
|------------- |----------:|---------:|----------:|-------:|----------:|
| StandardSort |  22.85 ns | 0.481 ns |  0.690 ns |      - |         - |
|    MergeSort | 253.66 ns | 5.117 ns | 13.208 ns | 0.1760 |     368 B |
|   BubbleSort |  73.41 ns | 1.000 ns |  0.936 ns |      - |         - |


N = 100:

|       Method |       Mean |     Error |    StdDev |  Gen 0 | Allocated |
|------------- |-----------:|----------:|----------:|-------:|----------:|
| StandardSort |   357.6 ns |   7.18 ns |   9.08 ns |      - |         - |
|    MergeSort | 4,101.3 ns |  67.42 ns |  63.07 ns | 2.4948 |   5,224 B |
|   BubbleSort | 6,584.7 ns | 128.85 ns | 218.80 ns |      - |         - |

N = 1000

|       Method |       Mean |      Error |     StdDev |   Gen 0 | Allocated |
|------------- |-----------:|-----------:|-----------:|--------:|----------:|
| StandardSort |   4.746 us |  0.0949 us |  0.1874 us |       - |         - |
|    MergeSort |  53.323 us |  0.8609 us |  0.7631 us | 30.7007 |  64,328 B |
|   BubbleSort | 597.957 us | 11.1561 us | 11.4565 us |       - |         - |

Можно заметить, что самым эффективным оказалася встроенная сортировка. При N = 10 сортировка пузырьком в среднем работает быстрее, чем сортировка слиянием, но при больших значениях N сортировка слиянием выигрывает по скорости работы, одна требует достаточно много дополнительной памяти.

### **Бенчмаркинг в Java**

Для проведения бенчмаркинга на Java нужно воспользоваться специальным пакетом - JMH (_Java Microbenchmark Harness_). Для этого я создал проект на Maven, где прописал эти строчки в pom.xml:

```java
    <dependencies>
        <dependency>
            <groupId>org.openjdk.jmh</groupId>
            <artifactId>jmh-core</artifactId>
            <version>1.20</version>
        </dependency>
        <dependency>
            <groupId>org.openjdk.jmh</groupId>
            <artifactId>jmh-generator-annprocess</artifactId>
            <version>1.20</version>
            <scope>provided</scope>
        </dependency>
    </dependencies>
```

Теперь мы можем обращаться к модулям из пакета для бенчмаркинга.

Для проведения тестов были написаны сами алгоритмы сортировок (_класс Sorts_):

```java
package Sortings;

import java.util.ArrayList;
import java.util.Comparator;

public class Sorts {
    public static void standardSort(ArrayList<Integer> list){
        list.sort(Comparator.naturalOrder());
    }

    public static void bubbleSort(ArrayList<Integer> list) {
        int temp = 0;
        for (int i = 0; i < list.size(); i++) {
            for (int j = 1; j < (list.size() - i); j++) {
                if (list.get(j - 1) > list.get(j)) {
                    temp = list.get(j - 1);
                    list.set(j - 1, list.get(j));
                    list.set(j, temp);
                }
            }
        }
    }

    public static void mergeSort(ArrayList<Integer> list){
        var n = list.size();
        if (n < 2) {
            return;
        }
        int mid = n / 2;
        ArrayList<Integer> l = new ArrayList<>();
        ArrayList<Integer> r = new ArrayList<>();

        for (int i = 0; i < mid; i++) {
            l.add(0);
        }

        for (int i = 0; i < n-mid; i++) {
            r.add(0);
        }

        for (int i = 0; i < mid; i++) {
            l.set(i, list.get(i));
        }
        for (int i = mid; i < n; i++) {
            list.set(i-mid, list.get(i));
        }
        mergeSort(l);
        mergeSort(r);

        merge(list, l, r, mid, n - mid);
    }

    public static void merge(
            ArrayList<Integer> a,
            ArrayList<Integer> l,
            ArrayList<Integer> r,
            int left, int right) {

        int i = 0, j = 0, k = 0;
        while (i < left && j < right) {
            if (l.get(i) <= r.get(j)) {
                a.set(k++, l.get(i++));
            }
            else {
                a.set(k++, r.get(j++));
            }
        }
        while (i < left) {
            a.set(k++, l.get(i++));
        }
        while (j < right) {
            a.set(k++, r.get(j++));
        }
    }
}
```

Класс, который занимается генерацией случайного массива на 100 элементов (_класс ArrayGenerator_):

```java
package Benchmark;

import java.util.ArrayList;
import java.util.Random;

public class ArrayGenerator {
    private static ArrayList<Integer> myArray = new ArrayList<Integer>();

    public static ArrayList<Integer> getRandArray(){
        Random rand = new Random();
        for(int i = 0; i <= 100; ++i){
            myArray.add(rand.nextInt());
        }
        return myArray;
    }
}
```

Класс для запуска самих бенчмарков (класс _TestLauncher_):

```java
package Benchmark;

import Sortings.Sorts;
import org.openjdk.jmh.annotations.*;

import java.util.ArrayList;
import java.util.concurrent.TimeUnit;

public class TestLauncher {

    @Benchmark
    @OutputTimeUnit(TimeUnit.NANOSECONDS)
    @BenchmarkMode(Mode.AverageTime)
    @Warmup(iterations = 0)
    @Measurement(iterations = 20)
    @Fork(value = 1, warmups = 2)
    public void startStandardSort(){
        ArrayList<Integer> myList = ArrayGenerator.getRandArray();
        Sorts.standardSort(myList);
    }

    @Benchmark
    @OutputTimeUnit(TimeUnit.NANOSECONDS)
    @BenchmarkMode(Mode.AverageTime)
    @Warmup(iterations = 0)
    @Measurement(iterations = 20)
    @Fork(value = 1, warmups = 2)
    public void startBubbleSort(){
        ArrayList<Integer> myList = ArrayGenerator.getRandArray();
        Sorts.bubbleSort(myList);
    }

    @Benchmark
    @OutputTimeUnit(TimeUnit.NANOSECONDS)
    @BenchmarkMode(Mode.AverageTime)
    @Warmup(iterations = 0)
    @Measurement(iterations = 20)
    @Fork(value = 1, warmups = 2)
    public void startMergeSort(){
        ArrayList<Integer> myList = ArrayGenerator.getRandArray();
        Sorts.mergeSort(myList);
    }
}
```

Код для запуска бенчмарков:

```java
package Benchmark;

import org.openjdk.jmh.runner.RunnerException;

import java.io.IOException;

public class Main {
    public static void main(String[] args) throws RunnerException, IOException {
        org.openjdk.jmh.Main.main(args);
    }
}
```

Прицип работы бенчмарков в Java и C# похож. Программа запускает помеченные методы несколько раз замеряя указанные параметры (_например:_ время работы, аллоцированная память) а потом считает среднее значение по каждому параметру для всех методов.

В результате работы бенчмарков я получил такие значения работы алгоритмов сортировки:

```
Result "Benchmark.TestLauncher.startStandardSort":
  4124940,140 ±(99.9%) 1765886,603 ns/op [Average]
  (min, avg, max) = (502120,050, 4124940,140, 7692003,788), stdev = 2033597,638
  CI (99.9%): [2359053,536, 5890826,743] (assumes normal distribution)

Result "Benchmark.TestLauncher.startMergeSort":
  23407176,163 ±(99.9%) 7881013,366 ns/op [Average]
  (min, avg, max) = (4468823,894, 23407176,163, 34698862,069), stdev = 9075786,713
  CI (99.9%): [15526162,797, 31288189,529] (assumes normal distribution)

Result "Benchmark.TestLauncher.startBubbleSort":
  252937630,352 ±(99.9%) 102956132,751 ns/op [Average]
  (min, avg, max) = (18441066,071, 252937630,352, 427195366,667), stdev = 118564435,586
  CI (99.9%): [149981497,601, 355893763,103] (assumes normal distribution)
```

По результатам можно сделать вывод, что стандартная сортировка оказалась самой быстрой, далее по скорости идет сортировка слиянием и на последнем месте сортировка пузырьком.