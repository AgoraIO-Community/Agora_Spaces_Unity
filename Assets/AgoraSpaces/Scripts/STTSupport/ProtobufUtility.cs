// ----------
// ProtobufUtility.cs
// 
// Hu Yuhua(darkzero)
// 2023.01.24
// ----------
namespace AgoraSTTSample.Utility
{
    using System;
    using Unity.VisualScripting;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Net;
    using System.IO;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Google.Protobuf;
    using AgoraSTTSample.Protobuf;
    using System.Linq;

    public class ProtobufUtility
    {
        #region - Parse Protobuf Data to Text
        public static Text ParseProtobufData(byte[] data)
        {
            Text text = Text.Parser.ParseFrom(data);
            //Debug.Log(string.Format("ParseProtobufData {0}: {1}", text.Uid, text.Words));
            return text; 
        }
        #endregion


        #region - Parse STT Text to strings
        private static int lastSeqnum = -1;
        private static Dictionary<long, List<string>> finalLists = new Dictionary<long, List<string>>();
        private static Dictionary<long, List<string>> finalConfidenceLists = new Dictionary<long, List<string>>();
        private static Dictionary<long, string> finalTexts = new Dictionary<long, string>();
        private static Dictionary<long, string> finalTextConfidences = new Dictionary<long, string>();

        public delegate void FinalTextHandler(string finalText, string finalTextConfidence);

        public static (string, string, string, int, string) createStringWithText(Text sttText, FinalTextHandler handler)
        {
            (string, string, string, int, string) defaultReturnValue = ("", "", "", 0, "");

            if (sttText.Words == null || sttText.Words.Count == 0)
            {
                //Debug.Log("return defaultReturnValue.");
                return defaultReturnValue;
            }
            //lastSeqnum = sttText.Seqnum;

            int revUid = sttText.Uid;

            if (!finalLists.Keys.Contains(revUid) || finalLists[revUid] == null)
            {
                finalLists[revUid] = new List<string>();
            }
            List<string> finalList = finalLists[revUid];

            if (!finalConfidenceLists.Keys.Contains(revUid) || finalConfidenceLists[revUid] == null)
            {
                finalConfidenceLists[revUid] = new List<string>();
            }
            List<string> finalConfidenceList = finalConfidenceLists[revUid];

            if (!finalTexts.Keys.Contains(revUid) || finalTexts[revUid] == null)
            {
                finalTexts[revUid] = "";
            }
            string finalText = finalTexts[revUid];

            if (!finalTextConfidences.Keys.Contains(revUid) || finalTextConfidences[revUid] == null)
            {
                finalTextConfidences[revUid] = "";
            }
            string finalTextConf = finalTextConfidences[revUid];

            List<string> nonFinalList = new List<string>();
            List<string> nonFinalConfidenceList = new List<string>();

            foreach (Word word in sttText.Words) {
                if (word.IsFinal)
                {
                    finalList.Add(word.Text);
                    finalLists[revUid] = finalList;

                    finalConfidenceList.Add(string.Format("%.2f", (word.Confidence)));
                    finalConfidenceLists[revUid] = finalConfidenceList;

                    if (IsSentenceBoundaryWord(word.Text))
                    {
                        string text = WordsToText(finalList);
                        finalList.Clear();
                        finalLists[revUid] = finalList;

                        finalText += text;
                        finalTexts[revUid] = finalText;

                        string textConfidence = WordsToText(finalConfidenceList);
                        finalConfidenceList.Clear();
                        finalConfidenceLists[revUid] = finalConfidenceList;

                        finalTextConf += textConfidence;
                        finalTextConfidences[revUid] = finalTextConf;

                        handler(text, textConfidence);
                    }
                }
                else
                {
                    nonFinalList.Add(word.Text);
                    nonFinalConfidenceList.Add(string.Format("%.2f", (word.Confidence)));
                }
            }

            string currentFinalText = WordsToText(finalList);
            int currentFinalCount = currentFinalText.Length;

            string currentText = currentFinalText;
            int oldFinalCount = finalText.Length;
            currentText += WordsToText(nonFinalList);

            string wholeText = finalText + currentText;
            int wholeFinalCount = currentFinalCount + oldFinalCount;

            string currentConfidenceText = WordsToText(finalConfidenceList);
            currentConfidenceText += WordsToText(nonFinalConfidenceList);

            return (currentText, currentConfidenceText, wholeText, wholeFinalCount, currentFinalText);
        }

        private static bool IsPunctuationWord(string word)
        {
            List<String> chars = new List<String> {
                ",", "?", ".", "，", "？", "。"
            };
            return chars.Contains(word);
            //return (word == "," || word == "?" || word == "."); 
        }

        private static bool IsSentenceBoundaryWord(string word)
        {
            List<String> chars = new List<String> {
                "?", ".", "？", "。"
            };
            return chars.Contains(word);
            //return (word == "?" || word == ".");
        }

        private static string WordsToText(List<string> words)
        {
            string result = "";
            foreach (string word in words)
            {
                if (!IsPunctuationWord(word))
                {
                    result += " ";
                }
                result += word;
            }
            return result;
        }
        #endregion
    }
}