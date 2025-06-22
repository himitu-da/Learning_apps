using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;



public class Word_Dictionary : MonoBehaviour
{
    private TextAsset csvFile;
    private List<string> english = new List<string>();
    private List<string> japanese = new List<string>();

    private List<List<string>> word_set = new List<List<string>>();
    private List<List<string>> unshuffled_word_set = new List<List<string>>();

    List<List<string>> new_list = new List<List<string>>();
    public Boolean shuffle_flag = true;

    /*public Dictionary()
    {
        csvFile = Resources.Load("EnglishWordList600") as TextAsset;
        StringReader reader = new StringReader(csvFile.text);
        reader.ReadLine();
        while (reader.Peek() != -1)
        {
            string[] line = reader.ReadLine().Split(',');   //１行ずつ読む 
            english.Add(line[2]);
            japanese.Add(line[6]);
        }

    }*/

    public Word_Dictionary()
    {
        int num = PlayerPrefs.GetInt("Gold", 0);
        if (num == 0)
        {
            csvFile = Resources.Load("EnglishWordList600") as TextAsset;
            StringReader reader = new StringReader(csvFile.text);
            reader.ReadLine();
            while (reader.Peek() != -1)
            {
                string[] line = reader.ReadLine().Split(',');   //１行ずつ読む 
                word_set.Add(new List<string> { line[2], line[6] });
            }
        }
        else if(num == 1)
        {
            csvFile = Resources.Load("Gold1000") as TextAsset;
            StringReader reader = new StringReader(csvFile.text);
            reader.ReadLine();
            while (reader.Peek() != -1)
            {
                string[] line = reader.ReadLine().Split(',');   //１行ずつ読む 
                word_set.Add(new List<string> { line[1], line[0] });
            }
        }
        else
        {
            csvFile = Resources.Load("Gold1000s") as TextAsset;
            StringReader reader = new StringReader(csvFile.text);
            reader.ReadLine();
            while (reader.Peek() != -1)
            {
                string[] line = reader.ReadLine().Split(',');   //１行ずつ読む 
                word_set.Add(new List<string> { line[1], line[0] });
            }
        }
        unshuffled_word_set = word_set.ToList();
        shuffle();
    }

    public void shuffle()
    {
        shuffle_flag = false;
        List<List<string>> shuffled_list = word_set.ToList();
        new_list = new List<List<string>>();
        int list_count = shuffled_list.Count;
        for (int i = 0; i < list_count; i++)
        {
            int take_index = UnityEngine.Random.Range(0, list_count - i);
            new_list.Add(shuffled_list[take_index]);
            shuffled_list.RemoveAt(take_index);
        }
        shuffle_flag = true;
    }

    async public Task<(string, List<string>, int)> get_choice_quiz(int num_option)
    {
        await UniTask.WaitUntil(() => shuffle_flag);

        List<List<string>> options_list = new List<List<string>>();
        List<List<string>> before_list = new List<List<string>>();
        List<string> options = new List<string>();
        List<string> before_options = new List<string>();

        //設定項目による場合分け
        int check = PlayerPrefs.GetInt("Randamize_Quiz_Setting", 0);

        //正解となるものを取り出す
        if(check == 0) //ランダムのとき
        {
            before_list.Add(new_list[0]);
            before_options.Add(new_list[0][0]);
            new_list.RemoveAt(0);
        }
        else // 固定のとき
        {
            int quiz_count = PlayerPrefs.GetInt("Quiz_Count", 0);
            int serve_quiz_count = quiz_count % unshuffled_word_set.Count;
            print(quiz_count + "quiz_count");
            print(serve_quiz_count + "Below");
            before_list.Add(unshuffled_word_set[serve_quiz_count]);
            before_options.Add(unshuffled_word_set[serve_quiz_count][0]);
            PlayerPrefs.SetInt("Quiz_Count", quiz_count + 1);
        }
        string tmp = before_options[0].ToString();
        string quiz_sentence = before_list[0][1].ToString();

        //不正解となるものを取り出す
        while (before_list.Count < num_option)
        {
            List<string> list = new_list[0];
            Boolean confrict_frag = false;
            if (before_list.Count > 0)
            {
                foreach (List<string> strings in before_list) //訳文が同じだった時
                {
                    if (list[0] == strings[0])
                    {
                        confrict_frag = true;
                    }
                }
            }

            if (!confrict_frag)
            {
                before_list.Add(list);
                before_options.Add(list[0]);
            }
            new_list.RemoveAt(0);
        }

        int correct_index = 0;

        // 並べ替え
        for(int i = 0; i < num_option; i++)
        {
            int index = UnityEngine.Random.Range(0, num_option - i);
            options.Add(before_options[index]);
            if (before_options[index] == tmp)　//取り出したものが答えの場合
                correct_index = i;
            before_options.RemoveAt(index);
        }

        // 問題文（和訳）、選択肢（n択）、正解のインデックス
        return (quiz_sentence, options, correct_index);
    }

    /*public string GetRandomJapanese()
    {
        return japanese[UnityEngine.Random.Range(0, japanese.Count)];
    }

    // 引数: 日本語, 正解のインデックス, 選択肢数
    // 出力: 選択肢リスト(英語)
    public string[] GetJa2EnQuestion(string japanese_word, int correct_index, int nb_chioces)
    {
        int num = 0;
        string[] choices = new string[nb_chioces];

        // 使用済み単語を格納
        List<string> NG_list = new List<string>();
        NG_list.Add(english[japanese.IndexOf(japanese_word)]);

        for (int i = 0; i < nb_chioces; i++)
        {
            if (i == correct_index)
            {
                num = japanese.IndexOf(japanese_word);
                choices[i] = english[num];
            }
            else
            {
                do
                {
                    num = UnityEngine.Random.Range(0, english.Count);
                }
                while (NG_list.Contains(english[num]));

                choices[i] = english[num];
            }
            NG_list.Add(english[num]);
        }
        return choices;
    }*/
}
