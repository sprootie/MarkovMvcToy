/*
 * Copyright (c) 2015 Globys, Inc., All Rights Reserved.
 */

namespace Markov.Data
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  /// <summary>
  /// First-order markov chain generator.  Works on small-sized source text, but generated strings can be pretty non-sensical
  /// </summary>
  public class FirstOrderMarkovChain : IMarkovChain
  {
    private Dictionary<string, List<string>> _cache;
    private const string StartKeyString = "__START__";

    public FirstOrderMarkovChain()
    {
      Reset();
    }

    /// <summary>
    /// Clear the generator's corpus
    /// </summary>
    public void Reset()
    {
      _cache = new Dictionary<string, List<string>>();
    }

    /// <summary>
    /// The count of keys in the generator's corpus
    /// </summary>
    public int Count
    {
      get { return _cache.Count; }
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
      var textArr = text.Replace("\r\n", "\r\n ").Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries);

      // remove blank lines
      textArr = textArr.Where(e => !e.Equals("\r\n")).ToArray();

      if (0 == textArr.Length) return;

      // Seed the cache with the first word
      AddOrUpdateCache(StartKeyString, textArr[0]);

      // Now go through each word and add it to the previous word's node
      for (var i = 0; i < textArr.Length - 1; i++)
      {
        AddOrUpdateCache(FormatKey(textArr[i]), textArr[i + 1]);

        // If it's the beginning of a sentence, add the next word to the start node too
        if (textArr[i].IsEndOfLine())
        {
          AddOrUpdateCache(StartKeyString, textArr[i + 1]);
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
      var currentKey = StartKeyString;
      var outputBuffer = string.Empty;

      var rng = new Random();

      var sentenceCount = 0;

      // Return nothing if _cache is empty/uninitialized
      if (null == _cache || 2 > _cache.Count)
      {
        return outputBuffer;
      }

      // Generate the text
      while (sentenceCount < sentencesRequested)
      {

        // Follow a random node, append it to the string, and move to that node
        var rand = rng.Next(_cache[currentKey].Count);
        var nextWord = _cache[currentKey][rand];
        outputBuffer += nextWord;

        // No more nodes to follow? Start again. (Add a period to make things look better.)
        if (!_cache.ContainsKey(FormatKey(nextWord)))
        {
          currentKey = StartKeyString;
          if (!nextWord.IsEndOfSentence())
          {
            outputBuffer += ". ";
          }
          else
          {
            outputBuffer += " ";
          }
        }
        else
        {
          currentKey = FormatKey(nextWord);
          outputBuffer += " ";
        }

        if (nextWord.IsEndOfLine())
        {
          sentenceCount++;
        }
      }
      return outputBuffer;
    }

    #region Helpers
    private static string FormatKey(string word)
    {
      return word.ToLower().Trim();
    }

    private void AddOrUpdateCache(string lookupKey, string text)
    {
      if (!_cache.ContainsKey(lookupKey))
      {
        _cache.Add(lookupKey, new List<string>());
      }
      _cache[lookupKey].Add(text);
    }
    #endregion
  }
}