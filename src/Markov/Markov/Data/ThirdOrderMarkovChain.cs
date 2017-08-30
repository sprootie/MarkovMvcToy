/*
 * Copyright (c) 2015 Globys, Inc., All Rights Reserved.
 */

namespace Markov.Data
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  /// <summary>
  /// Third-order markov chain generator.  Looks much more readable, but requires much greater source text for good randomization.
  /// </summary>
  public class ThirdOrderMarkovChain : IMarkovChain
  {
    private Dictionary<Tuple<string, string, string>, List<string>> _cache;
    private Tuple<string, string, string> _startKey;
    private const string StartKeyString = @"__START__";
    private string _lastFileLoaded = string.Empty;

    public ThirdOrderMarkovChain()
    {
      Reset();
    }

    /// <summary>
    /// The count of keys in the generator's corpus
    /// </summary>
    public int Count => _cache.Count;

      /// <summary>
    /// Clear the generator's corpus
    /// </summary>
    public void Reset()
    {
      _startKey = MakeLookupKey(StartKeyString, StartKeyString, StartKeyString);
      _cache = new Dictionary<Tuple<string, string, string>, List<string>>();
    }

    /// <summary>
    /// Load file contents into the generator's corpus.  Because of the potential size of the input corpus, this will
    /// reset any existing corpus before loading the new one.  It was also not reset/reload a file if it is the same
    /// file that was loaded last.
    /// </summary>
    /// <param name="filePath"></param>
    public void LoadFromFile(string filePath)
    {
      if (System.IO.File.Exists(filePath) && !_lastFileLoaded.Equals(filePath, StringComparison.CurrentCultureIgnoreCase))
      {
        Reset();
        var text = System.IO.File.ReadAllText(filePath);
        Load(text);
        _lastFileLoaded = filePath;
      }
    }

    /// <summary>
    /// Load a string into the generator's corpus.  This does not remove any existing corpus entries.
    /// </summary>
    /// <param name="text"></param>
    public void Load(string text)
    {
      // add a space after linebreaks before splitting, otherwise the split on space will join words on either side of linebreaks
      var textArr = text.Replace("\r\n", "\r\n ").Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries);

      // remove blank lines
      textArr = textArr.Where(e => !e.Equals("\r\n")).ToArray();

      if (0 == textArr.Length) return;

      // Seed the cache with the first partial keys
      AddOrUpdateCache(_startKey, textArr[0]);
      AddOrUpdateCache(ShiftLookupKey(_startKey, textArr[0]), textArr[1]);
      AddOrUpdateCache(ShiftLookupKey(ShiftLookupKey(_startKey, textArr[0]), textArr[1]), textArr[2]);

      // Now go through each word and add it to the previous word's node
      for (var i = 2; i < textArr.Length - 1; i++)
      {
        var lookupKey = MakeLookupKey(textArr[i - 2], textArr[i - 1], textArr[i]);
        AddOrUpdateCache(lookupKey, textArr[i + 1]);

        // If it's the beginning of a sentence, add the next word to the start node too
        if (textArr[i].IsEndOfLine())
        {
          AddOrUpdateCache(_startKey, textArr[i + 1]);
        }

        if (textArr[i - 1].IsEndOfLine())
        {
          AddOrUpdateCache(ShiftLookupKey(_startKey, textArr[i]), textArr[i + 1]);
        }

        if (textArr[i -2].IsEndOfLine())
        {
          AddOrUpdateCache(ShiftLookupKey(ShiftLookupKey(_startKey, textArr[i - 1]), textArr[i]), textArr[i + 1]);
        }
      }
    }

    /// <summary>
    /// Generate a random string.  Sentences are seperated by a double-space.
    /// </summary>
    /// <returns></returns>
    public string Generate(int sentencesRequested = 15)
    {
      // Start with the root node
      var currentWord = _startKey;
      var outputBuffer = string.Empty;

      var rng = new Random();

      var sentenceCount = 0;

      // Return nothing if _cache is empty/uninitialized
      if (null == _cache || 2 > _cache.Count)
      {
        return outputBuffer;
      }

      // Generate 300 words of text
      while (sentenceCount < sentencesRequested)
      {

        // Follow a random node, append it to the string, and move to that node
        var rand = rng.Next(_cache[currentWord].Count);
        var nextWord = _cache[currentWord][rand];
        outputBuffer += (string.IsNullOrEmpty(outputBuffer) ? string.Empty : " ") + nextWord;

        currentWord = ShiftLookupKey(currentWord, nextWord);

        if (nextWord.IsEndOfLine())
        {
          sentenceCount++;
        }
      }
      return outputBuffer;
    }

    #region Helpers
    private void AddOrUpdateCache(Tuple<string, string, string> lookupKey, string text)
    {
      if (!_cache.ContainsKey(lookupKey))
      {
        _cache.Add(lookupKey, new List<string>());
      }
      _cache[lookupKey].Add(text);
    }

    private static Tuple<string, string, string> MakeLookupKey(string str1, string str2, string str3)
    {
      return new Tuple<string, string, string>(str1.ToLower().Trim(), str2.ToLower().Trim(), str3.ToLower().Trim());
    }

    private static Tuple<string, string, string> ShiftLookupKey(Tuple<string, string, string> lookupKey, string newWord)
    {
      return new Tuple<string, string, string>(lookupKey.Item2, lookupKey.Item3, newWord.ToLower().Trim());
    }
    #endregion
  }
}