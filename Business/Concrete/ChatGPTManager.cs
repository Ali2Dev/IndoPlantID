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
using Microsoft.Extensions.Configuration;

namespace Business.Concrete
{
    public class ChatGPTManager : IChatGPTService
    {
        private readonly IConfiguration _configuration;

        public ChatGPTManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        public async Task<string> GetResponse(string plantName)
        {


            var openAIService = new OpenAIService(new OpenAiOptions
            {
                ApiKey = _configuration["GPTService:Key"]
            });

            string gptRequest = $"{plantName} bitkisi hakkında bilgi verir misin ? Bitkinin adı yabancı ise Türkçedeki adını da ver. Vereceğin cevap bir sitede kullanılacağı için sanki birisi senden böyle bir şey istememiş gibi bir cevap ver. Yani 'tabii ki' , 'işte cevabın' gibi kelime ya da cümleler kurma.";

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


        public async Task<string> GetMaintenanceAndWatering(string plantName)
        {

            var openAIService = new OpenAIService(new OpenAiOptions
            {
                ApiKey = _configuration["GPTService:Key"]
            });

            string gptRequest = $"{plantName} bitkisininin bakımı ve sulaması nasıl yapılmalıdır ? İdeal yaşam alanı ve dikkat edilmesi gerekenler nedir ? Vereceğin cevap bir sitede kullanılacağı için sanki birisi senden böyle bir şey istememiş gibi bir cevap ver. Yani 'tabii ki' , 'işte cevabın', 'umarım yardımcı olabilmişimdir' gibi kelime ya da cümleler kurma. Sadece sorularn sorunun cevabını ver.";

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

