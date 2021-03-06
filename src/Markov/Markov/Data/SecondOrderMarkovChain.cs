﻿/*
 * Copyright (c) 2015 Globys, Inc., All Rights Reserved.
 */

namespace Markov.Data
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  /// <summary>
  /// Second-order markov chain generator.  More readable than first order, but can have readability problems with source text containing lots of quotations.
  /// </summary>
  public class SecondOrderMarkovChain : IMarkovChain
  {
    private Dictionary<Tuple<string, string>, List<string>> _cache;
    private Tuple<string, string> _startKey;
    private const string StartKeyString = @"__START__";

    public SecondOrderMarkovChain()
    {
      Reset();
    }

    /// <summary>
    /// The count of keys in the generator's corpus
    /// </summary>
    public int Count
    {
      get { return _cache.Count; }
    }

    /// <summary>
    /// Clear the generator's corpus
    /// </summary>
    public void Reset()
    {
      _startKey = MakeLookupKey(StartKeyString, StartKeyString);
      _cache = new Dictionary<Tuple<string, string>, List<string>>();
    }

    /// <summary>
    /// Load file contents into the generator's corpus
    /// </summary>
    /// <param name="filePath"></param>
    public void LoadFromFile(string filePath)
    {
      if (System.IO.File.Exists(filePath))
      {
        var text = System.IO.File.ReadAllText(filePath);
        Load(text);
      }

    }

    /// <summary>
    /// Load a string into the generator's corpus
    /// </summary>
    /// <param name="text"></param>
    public void Load(string text)
    {
      // add a space after linebreaks before splitting, otherwise the split on space will join words on either side of linebreaks
      var textArr = text.Replace("\r\n", "\r\n ").Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries);

      // remove blank lines
      textArr = textArr.Where(e => !e.Equals("\r\n")).ToArray();

      if (0 == textArr.Length) return;

      // Seed the cache with the first two words
      AddOrUpdateCache(_startKey, textArr[0]);
      AddOrUpdateCache(ShiftLookupKey(_startKey, textArr[0]), textArr[1]);

      // Now go through each word and add it to the previous word's node
      for (var i = 1; i < textArr.Length - 1; i++)
      {
        var lookupKey = MakeLookupKey(textArr[i - 1], textArr[i]);
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
    private void AddOrUpdateCache(Tuple<string, string> lookupKey, string text)
    {
      if (!_cache.ContainsKey(lookupKey))
      {
        _cache.Add(lookupKey, new List<string>());
      }
      _cache[lookupKey].Add(text);
    }

    private static Tuple<string, string> MakeLookupKey(string str1, string str2)
    {
      return new Tuple<string, string>(str1.ToLower().Trim(), str2.ToLower().Trim());
    }

    private static Tuple<string, string> ShiftLookupKey(Tuple<string, string> lookupKey, string newWord)
    {
      return new Tuple<string, string>(lookupKey.Item2, newWord.ToLower().Trim());
    }
    #endregion
  }
}