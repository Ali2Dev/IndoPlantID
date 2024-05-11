using Business.Abstract;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class ChatGPTManager : IChatGPTService
    {

        public async Task<string> GetResponse(string plantName)
        {

            var openAIService = new OpenAIService(new OpenAiOptions
            {
                ApiKey = "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"
            });
            string gptRequest = $"Bana {plantName} bitkisi hakkında bilgi verir misin";

            var completionResult = await openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = new List<ChatMessage>
            {
                ChatMessage.FromUser(gptRequest)
            },
                Model = Models.Gpt_3_5_Turbo_16k_0613,
                MaxTokens = 1000//optional
            });

            if (completionResult.Successful)
            {
                string response = completionResult.Choices.First().Message.Content;
                return response;
            }
            else
            {
                return "Servis hatası!";
            }
        }
    }
}

