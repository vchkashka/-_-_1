# Компилятор

## Описание
Программа предназначена для разработчиков и студентов, изучающих программирование, и обеспечивает простую 
компиляцию кода без сложных настроек. Она включает функционал для редактирования и написания кода, упрощая тем самым 
работу с кодом.

## Функции:
- Создание, открытие файлов, сохранение изменений;
- Отмена и повтор действий, редактирование текста;
- Подсветка синтаксиса;
- Анализ кода;
- Возможность изменения языка интерфейса;
- Возмость изменения размера шрифта;
- Вывод ошибок.

## Вариант 37
Объявление массива символов с инициализацией строковой константой на языке С:
const char str[] = "Hello World";

## Постановка задачи
Константы — это элементы данных, значения которых известны и не изменяются в процессе выполнения программы.
В языке C для объявления массива символов с инициализацией строковой константой используется ключевое слово const.
Формат записи:
const char имя_массива[] = "значение";
Примеры:
1. Обычная строка - символьный массив: const char str[] = "Hello World";
2. Строка с символами Unicode (UTF-8) поддерживает символы кириллицы или других алфавитов: const char greeting[] = "Привет, мир!";
3. Объявление массива фиксированного размера: const char buffer[10] = "Example"; 

## Примеры допустимых строк
Согласно будущей автоматной грамматике G[‹Def›] синтаксический анализатор (парсер) строковых констант будет считать верными следующие записи:
- const char text123[] = "Sample";
- const char _greeting[] = "Привет, мир!"
- const char word[6] = "Hello";

 ## Диаграмма состояний сканера
![Диаграмма](https://github.com/user-attachments/assets/0b7134dd-4fbe-4540-bcec-278398e83a18)

Рисунок 1 - Диаграмма состояний сканера

 ## Тестовые примеры
 На рисунках 2-5 представлены тестовые примеры запуска разработанного сканера.
 
![image](https://github.com/user-attachments/assets/55e990f2-4814-4241-b119-3c8dd7a788ca)

Рисунок 2 - Тестовый пример 1


![image](https://github.com/user-attachments/assets/1b8319b4-8014-4542-a764-99b5a0376775)

Рисунок 3 - Тестовый пример 2


![image](https://github.com/user-attachments/assets/d3101847-7623-48b4-8c0a-97b6caba5d55)

Рисунок 4 - Тестовый пример 3


![image](https://github.com/user-attachments/assets/77fbb70a-2e98-49bc-93eb-21f625116125)

Рисунок 5 - Тестовый пример 4



## Установка
Для установки и запуска проекта выполните следующие шаги:

1. Установите Microsoft Visual Studio, если у вас ее нет;
2. Скачайте репозиторий или клонируйте в Visual Studio;
3. Откройте скачанную папку и запустите файл лаба_компиляторы_1.slnлибо откройте клонированный проект в Visual Studio;
4. Запустите проект.

## Автор
Топорова Е.В.
