using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;

public class CSVReader
{
    // @붙이면 \한번으로 인식
    //@심벌을 사용하여 보다 자연스럽게 패스 지정
    //string filename = "C:\\Temp\\1.txt";
    //string filename = @"C:\Temp\1.txt";
    //https://regexper.com/#%5C%5B%28%5B%5E%2C%5D%5D%29%5C%5D

    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();
        TextAsset data = Resources.Load(file) as TextAsset;
        //TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>("a" + file);

        //Regex.Split 라인별 쪼개기
        //문장 먼저 끊기 (개행 처리된 부분 찾아서 먼저 쪼갬)
        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        
        //쪼갠 줄을 SPLIT_RE 패턴으로 또 쪼갬
        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {


            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                //trim은 공백을 없애는거 TrinStart에서 부터 공백 제거 시작해서 TrimEnd 까지 진행
                //Replaces는 바꿔주는거, \\를 아무것도 없는것으로 바까줌
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }
}
