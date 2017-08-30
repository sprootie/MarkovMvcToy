/*
 * Copyright (c) 2015 Globys, Inc., All Rights Reserved.
 */

namespace Markov.Data
{
  using System;

  static class Extensions
  {
    public static bool IsEndOfSentence(this string word)
    {
      word = word.Trim();
      if (word.Equals("Mr.", StringComparison.OrdinalIgnoreCase) || word.Equals("Mrs.", StringComparison.OrdinalIgnoreCase) || word.Equals("Ms.", StringComparison.OrdinalIgnoreCase) || word.Equals("Dr.", StringComparison.OrdinalIgnoreCase))
        return false;
      return word.EndsWith(".") || word.EndsWith("!") || word.EndsWith("?") || word.EndsWith(".\"") || word.EndsWith("?\"") || word.EndsWith("!\"");
    }

    public static bool IsEndOfLine(this string word)
    {
      return word.EndsWith("\n");
    }
  }
}