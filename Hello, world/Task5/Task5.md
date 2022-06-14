5. Используя инструменты dotTrace, dotMemory, всё-что-угодно-хоть-windbg, проанализировать работу написанного кода для бекапов. Необходимо написать сценарий, когда в цикле будет выполняться много запусков, будут создаваться и удаляться точки. Проверить два сценария: с реальной работой с файловой системой и без неё. В отчёте неоходимо проанализировать полученные результаты, сделать вывод о написанном коде. 

В качестве начальных условий для првоерки моей программы с бэкапами было установлено: два текстовых файла, которые являлись объектами джобы; тип хранения - sigle storage; алгоритм очистки точек - по их количству(максимальное значение - 5).

Было произведено 120 запусков алгоритма сохранения резервной копии.

Исходный код для запуска:

Код для работы с файловой системой:

```c#
using System.Collections.Generic;
using System.IO;
using Backups.RepositoryFolder;
using Backups.StorageAlgorithm;
using Backups.StorageType;
using BackupsExtra.PointRemover;
using BackupsExtra.Subjects;

namespace BackupsExtra
{
    internal class Program
    {
        private static void Main()
        {
            const string repositoryPath = "C:\\Users\\geo02\\Desktop\\MyRepository";
            var newRep = new Repository(repositoryPath);
            Directory.CreateDirectory(repositoryPath);
            var paths = new List<string>()
            {
                "C:\\Users\\geo02\\Desktop\\FilesToAdd\\File_1.txt",
                "C:\\Users\\geo02\\Desktop\\FilesToAdd\\File_2.txt",
            };
            IStorageAlgorithmType newAlgo = new SingleStorageAlgorithm(new FileSystemSaver());
            IPointRemover remover = new ByNumberPointRemover(5);
            var bj = new BackupJobSubject("Job1", newRep, newAlgo, paths, true);
            bj.AttachPointRemover(remover);
            for (int i = 0; i < 120; ++i)
            {
                bj.StartJob();
            }
        }
    }
}
```

Код для работы с рантаймом:

```c#
var newRep = new Repository("examplePath");
var paths = new List<string>()
{
    "examplePath1",
    "examplePath2",
};
IStorageAlgorithmType newAlgo = new SplitStorageAlgorithm(new VirtualStorageSaver());
var job = new BackupJob("job1", newAlgo, paths);
for (int i = 0; i < 120; ++i)
{
    job.StartJob(newRep);
}
```

Запустив BackupsExtra.exe из dotMemory я получил график расхода памяти моей программой. 

_\*смотреть вложение с получившимися графиками\*_

На картинке _memory.png_ показан график при работе с реальной файловой системой. _memory2.png_ показывает работу с файлами, которые хранятся в рантайме. Можно заметить, что работа с файлами из рантайма занимает в разы меньше памяти. 

Исходя из значений на графиках можно сделать вывод, что моя программа работает достаточно оптимально и не происходит утечек памяти.