using System.Text;

namespace AIServer.LlamaCpp.Models;

internal class Llama3Template
{
    public static string Compile(IEnumerable<Message> turns)
    {
        var sb = new StringBuilder();
        sb.Append("<|begin_of_text|>");

        foreach (var t in turns)
        {
            var role = t.Role switch
            {
                "system" => "system",
                "assistant" => "assistant",
                _ => "user"
            };

            sb.Append("<|start_header_id|>")
              .Append(role)
              .Append("<|end_header_id|>\n\n")
              .Append(t.Content)
              .Append("\n<|eot_id|>");
        }

        // Signal the model to continue as assistant
        sb.Append("<|start_header_id|>assistant<|end_header_id|>\n\n");
        return sb.ToString();
    }
}