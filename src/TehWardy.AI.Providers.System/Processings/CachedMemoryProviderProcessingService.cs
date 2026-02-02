using TehWardy.AI.Providers.InMemory.Foundations;
using TehWardy.AI.Providers.InMemory.Processings;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Providers.System.Models;

namespace TehWardy.AI.Providers.System.Processings;

internal class CachedMemoryProviderProcessingService(ICachedMemoryProviderService cachedMemoryProviderService)
    : ICachedMemoryProviderProcessingService
{

    public ValueTask AddDocumentAsync(string indexName, dynamic metadata, RagDocument doc) =>
        cachedMemoryProviderService.AddDocumentAsync(indexName, metadata, doc);

    public ValueTask ClearAsync(string indexName) =>
        cachedMemoryProviderService.ClearAsync(indexName);

    public async ValueTask<List<RagDocument>> SearchAsync(EmbeddedSearchRequest embeddedSearchRequest)
    {
        ValidateSearchRequest(embeddedSearchRequest);

        if (embeddedSearchRequest.Embedding is null || embeddedSearchRequest.Embedding.Length == 0)
            return [];

        var topK = embeddedSearchRequest.TopK <= 0 ? 0 : embeddedSearchRequest.TopK;

        // Score candidates
        var scored = await ScoreAndSortIndex(embeddedSearchRequest);

        var resultCount = Math.Min(topK, scored.Count);

        return scored
            .Take(topK)
            .Select(i => i.doc)
            .ToList();
    }

    private async ValueTask<List<(float score, RagDocument doc)>> ScoreAndSortIndex(EmbeddedSearchRequest embeddedSearchRequest)
    {
        var docs = await cachedMemoryProviderService
            .GetDocumentsInIndexAsync(embeddedSearchRequest.IndexName);

        List<(float score, RagDocument doc)> scored = [];
        float? score;

        foreach (var doc in docs)
        {
            score = ComputeScore(embeddedSearchRequest, doc);

            if (score is not null)
                scored.Add(((float)score, doc.Document));
        }

        // Take topK by highest cosine
        scored.Sort((a, b) => b.score.CompareTo(a.score));

        return scored;
    }

    static void ValidateSearchRequest(EmbeddedSearchRequest embeddedSearchRequest)
    {
        if (embeddedSearchRequest is null)
            throw new ArgumentNullException(nameof(embeddedSearchRequest), "No request was provided, one is required.");
    }

    static float? ComputeScore(EmbeddedSearchRequest embeddedSearchRequest, StoredDocument documentToScore)
    {
        bool isGlobalDoc = documentToScore.Document.ConversationId is null;

        bool isPartOfCurrentConversation = string.Equals(
            documentToScore.Document.ConversationId,
            embeddedSearchRequest.ConversationId,
            StringComparison.Ordinal);

        // Conversation rule: match OR doc is global (null)
        if (!(isGlobalDoc || isPartOfCurrentConversation))
            return null;

        var score = CosineSimilarity(
            embeddedSearchRequest.Embedding,
            Normalise(embeddedSearchRequest.Embedding),
            documentToScore.Document.ContentVector);

        return score;
    }

    static float CosineSimilarity(float[] a, float aNorm, float[] b)
    {
        // cosine(a,b) = dot(a,b) / (||a|| * ||b||)
        var dot = 0f;
        var bSq = 0f;

        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            bSq += b[i] * b[i];
        }

        var bNorm = MathF.Sqrt(bSq);

        if (bNorm == 0f)
            return 0f;

        return dot / (aNorm * bNorm);
    }

    static float Normalise(float[] v)
    {
        var sum = 0f;

        for (int i = 0; i < v.Length; i++)
            sum += v[i] * v[i];

        return MathF.Sqrt(sum);
    }
}