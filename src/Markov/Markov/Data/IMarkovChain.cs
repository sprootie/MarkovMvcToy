namespace Markov.Data
{
    public interface IMarkovChain
    {
        int Count { get; }
        void Reset();
        void LoadFromFile(string filePath);
        void Load(string text);
        string Generate(int sentencesRequested);
    }
}