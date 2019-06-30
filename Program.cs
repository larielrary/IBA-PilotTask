using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Timers;
using System.Runtime;

namespace Game_Words
{
    class Program
    {
        public static Timer myTimer;
        static void Main()
        {
            myTimer = new Timer();
            myTimer.Interval = 10000;
            myTimer.Elapsed += OnTimedEvent;    ///подписка на событие



            bool checkName = false;
            bool checkLenght = false;
            string firstPlayerNickname = string.Empty;
            string secondPlayerNickname = string.Empty;
            do
            {
                do
                {
                    Console.WriteLine("Enter the first player nickname");
                    firstPlayerNickname = Console.ReadLine();    ///ввод ника первого игрока
                    if (firstPlayerNickname.Length > 8 && firstPlayerNickname.Length < 20)///проверка на длину первого ника
                        checkLenght = true;
                    if (!checkLenght)
                    {
                        Console.WriteLine("You enter nickname with wrong lenght. Lenght can be 8-20 characters. Please press any key to re-enter...");
                        Console.ReadKey();
                    }
                }
                while (!checkLenght);
                checkLenght = false;
                do
                {
                    Console.WriteLine("Enter the second player nickname");
                    secondPlayerNickname = Console.ReadLine();  ///ввод ника второго игрока
                    if (secondPlayerNickname.Length > 8 && secondPlayerNickname.Length < 20)///проверка на длину второго ника
                        checkLenght = true;
                    if (!checkLenght)
                    {
                        Console.WriteLine("You enter nickname with wrong lenght. Lenght can be 8-20 characters. Please press any key to re-enter...");
                        Console.ReadKey();
                    }
                }
                while (!checkLenght);

                if (firstPlayerNickname != secondPlayerNickname)///проверка на равенство ников
                    checkName = true;
                if (!checkName)
                {
                    Console.WriteLine("You can't enter an equal nicknames. Please press any key to re-enter...");
                    Console.ReadKey();
                }
            }
            while (!checkName);
            string checkLanguage = string.Empty;
            bool successfulInput = false;
            do
            {
                Console.WriteLine("\nPleese, select language to play");
                Console.WriteLine("1. English");
                Console.WriteLine("2. Русский");
                checkLanguage = Console.ReadLine();
                if (checkLanguage == "1" || checkLanguage == "2") ///проверка языка
                    successfulInput = true;
                if (successfulInput == false)
                {
                    Console.WriteLine("You enter wrong language key. Please, press any key to re-enter...");
                    Console.ReadKey();
                }

            }
            while ((checkLanguage != "1") && (checkLanguage != "2"));
            Console.Clear();

            if (checkLanguage == "1") ProcessingInEnglish(firstPlayerNickname, secondPlayerNickname);
            if (checkLanguage == "2") ProcessingInRussian(firstPlayerNickname, secondPlayerNickname);

            Console.ReadLine();
        }
        static void ProcessingInEnglish(string firstPlayer, string secondPlayer) ///если игрок выбрал английский язык
        {
            string[] wordsFromFile = null;
            try
            {
                wordsFromFile = File.ReadAllLines("Ewords.txt", Encoding.GetEncoding(1251));   ///считывание из файла
            }
            catch (FileNotFoundException) ///если файл не найден
            {
                Console.WriteLine("File not found. Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(-1);
            }
            Console.WriteLine("Rules of the game. \nThe first player enters the word, its existence is checked in the dictionary. \nNext, the second player enters a word consisting of the letters of the source word. \nIf the player does not enter the word / word impossible to create from the source, the player lost.");
            Console.WriteLine();
            string startWord = string.Empty;
            bool successfulInput = false; ///для проверки успешного ввода слова
            bool firstPlayerMove = false;///ход первого игрока
            bool secondPlayerMove = false;///ход второго игрока
            Console.WriteLine($"{firstPlayer} moves\n");
            do
            {
                firstPlayerMove = true;
                

                Console.WriteLine("\nEnter a start word: ");
                startWord = Console.ReadLine();
                for (int i = 0; i < wordsFromFile.Length; i++)
                    if (startWord == wordsFromFile[i] && startWord.Length >= 8 && startWord.Length <= 30) ///проверка, есть ли введенное слово в файле, а также провера на длину слова
                        successfulInput = true;
                if (!successfulInput)
                {
                    Console.WriteLine("You enter a wrong word. Press any key to re-entry");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
            while (!successfulInput);
            firstPlayerMove = false;
            List<string> newWords = new List<string>();
            newWords.Add(startWord.ToString());///запоминаем в список, чтобы не было повторяющихся слов
            bool checkWord = true;
            secondPlayerMove = true;
            while (checkWord)
            {
                if (firstPlayerMove == true) Console.WriteLine($" {firstPlayer} moves");
                if (secondPlayerMove == true) Console.WriteLine($"{secondPlayer} moves");
                myTimer.Start();
                Console.WriteLine("Enter your new word: ");
                string newWord = Console.ReadLine();
                if (newWords.Contains(newWord))///если слово уже есть в списке, то все плохо - мы проиграли
                {
                    checkWord = false;
                    Console.WriteLine("You enter an existing word. Sorry, you lose");
                    break;
                }
                else
                {
                    var baseString = startWord.ToString().ToLower().GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());///проверка, можно ли составить новое слово из исходного
                    bool check = newWord.ToLower().GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count())
                        .All(x => baseString.ContainsKey(x.Key) && baseString[x.Key] >= x.Value);

                    Console.WriteLine(check ? "You can create a word." : "You can't create a word. You lose.\nPress any key to exit...");
                    if (!check) checkWord = false;
                    else newWords.Add(newWord);
                }
                myTimer.Stop();///остановка таймера
                if (firstPlayerMove == true)///переход хода
                {
                    firstPlayerMove = false;
                    secondPlayerMove = true;
                }
                else
                {
                    secondPlayerMove = false;
                    firstPlayerMove = true;
                }
            }
        }
        static void ProcessingInRussian(string firstPlayer, string secondPlayer)///русскоязычная версия
        {
            string[] wordsFromFile = null;
            try
            {
                wordsFromFile = File.ReadAllLines("words.txt", Encoding.GetEncoding(1251));///считывание
            }
            catch (FileNotFoundException)///файл не найден
            {
                Console.WriteLine("Файл не найден. Нажмите любую кнопку для выхода...");
                Console.ReadKey();
                Environment.Exit(-1);
            }
            Console.WriteLine("Правила игры. \nПервый игрок вводит слово, его существование проверяется в словаре. \nДалее второй игрок вводит слово, состоящее из букв исходного слова. \nЕсли же игрок не вводит слово / слово невозможно создать из исходного, игрок проиграл.");
            Console.WriteLine();
            wordsFromFile = File.ReadAllLines("words.txt", Encoding.GetEncoding(1251));
            string startWord = string.Empty;
            bool successfulInput = false;

            bool firstPlayerMove = false;///ход первого игрока
            bool secondPlayerMove = false;///ход второго игрока
            Console.WriteLine($"Ход игрока {firstPlayer}\n");
            do
            {
                firstPlayerMove = true;
                

                Console.WriteLine("\nВведите первое слово: ");
                startWord = Console.ReadLine();
                for (int i = 0; i < wordsFromFile.Length; i++)
                    if (startWord == wordsFromFile[i] && startWord.Length >= 8 && startWord.Length <= 30) ///проверка, есть ли введенное слово в файле, а также провера на длину слова
                        successfulInput = true;
                if (!successfulInput)
                {
                    Console.WriteLine("Вы ввели неверное слово. Нажмите любую кнопку для повторного ввода");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
            while (!successfulInput);
            firstPlayerMove = false;
            List<string> newWords = new List<string>();

            newWords.Add(startWord.ToString());
            bool checkWord = true;

            secondPlayerMove = true;
            while (checkWord)
            {
                if (firstPlayerMove == true) Console.WriteLine($"Ход игрока {firstPlayer}");
                if (secondPlayerMove == true) Console.WriteLine($"Ход игрока {secondPlayer}");
                myTimer.Start();
                Console.WriteLine("Введите ваше следующее слово: ");
                string newWord = Console.ReadLine();
                if (newWords.Contains(newWord))
                {
                    checkWord = false;
                    Console.WriteLine("Вы ввели существующее слово. Извините, вы проиграли");
                    break;
                }
                else
                {
                    var baseString = startWord.ToString().ToLower().GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());///можно ли составить новое слово из исходного
                    bool check = newWord.ToLower().GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count())
                        .All(x => baseString.ContainsKey(x.Key) && baseString[x.Key] >= x.Value);

                    Console.WriteLine(check ? "Вы можете создать слово." : "Вы не можете создать слово. Вы проиграли.\nНажмите любую кнопку для выхода...");
                    if (!check) checkWord = false;
                    else newWords.Add(newWord);
                }
                myTimer.Stop();///остановка таймера
                if (firstPlayerMove == true)///переход хода
                {
                    firstPlayerMove = false;
                    secondPlayerMove = true;
                }
                else
                {
                    secondPlayerMove = false;
                    firstPlayerMove = true;
                }
            }

        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Time is up! Game over. Press any key to return to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}