using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using QnA_Interactive.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace QnA_Interactive
{
    public class QnAHelper
    {
        enum ResponseType
        {
            TXT,  //Plain text
            HRC,  //Hero Card
            VDC,  //Video Card
            CRC,  //Carrusel Card
            ADC   //Audio card
        }

        string knowledgeBaseId = string.Empty;
        string authorizationKey = string.Empty;
        string qnaMakerName = string.Empty;

        public QnAHelper(string qnaName, string knowledgeBase, string authorizationToken)
        {
            knowledgeBaseId = knowledgeBase;
            authorizationKey = authorizationToken;
            qnaMakerName = qnaName;
        }

        public void CreateAnswer(string query, ITurnContext context)
        {
            string receivedQuestion = string.Format("{{\"question\":\"{0}\"}}", query);
            string urlBase = string.Format("https://{0}.azurewebsites.net/qnamaker/knowledgebases/{1}/generateAnswer", qnaMakerName, knowledgeBaseId);

            var client = new RestClient(urlBase);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", authorizationKey);
            request.AddParameter("undefined", receivedQuestion, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            Answer receivedAnswer = Answer.HandleResponse(response.Content);
            HandleMultimediaResponse(receivedAnswer.QnAAnswer, receivedAnswer.Score, context, query);
        }

        private async void HandleMultimediaResponse(string stringAnswer, double scoreAnswer, ITurnContext context, string receivedQuestion)
        {
            if (scoreAnswer == 0)
            {
                HandleWrongAnswer(context, receivedQuestion);
            }
            else
            {
                string[] responseArray = stringAnswer.Split('|');

                ResponseType contentType = (ResponseType)Enum.Parse(typeof(ResponseType), responseArray[0]);

                switch (contentType)
                {
                    case ResponseType.TXT:
                        await context.SendActivityAsync(responseArray[1]);
                        break;
                    case ResponseType.HRC:
                        GenerateHeroCard(context, responseArray);
                        break;
                    case ResponseType.VDC:
                        GenerateVideoCard(context, responseArray);
                        break;
                    case ResponseType.CRC:
                        GenerateCarouselCard(context, responseArray);
                        break;
                    case ResponseType.ADC:
                        GenerateAudioCard(context, responseArray);
                        break;
                    default:
                        await context.SendActivityAsync("Estoy probando");
                        break;
                }
            }
        }

        private void HandleWrongAnswer(ITurnContext context, string receivedQuestion)
        {
            var postBody = $"{{\"message\": \"{receivedQuestion}\"}}";
            var logicAppUrl = "https://prod-00.southcentralus.logic.azure.com:443/workflows/42ee53dacac0417ea5f070aaf80f0fce/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=DN3ckgFLCs-ujMNtWGEMQHJ5ia6sbHOmacPvDFhALmE";

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.Headers.Add("Content-Type", "application/json");
                client.UploadString(logicAppUrl, postBody);
            }

            context.SendActivityAsync("No tengo una respuesta para esta pregunta, gracias por hacerla porque me permitirá aprender más, ya he enviado un correo a mi administrador para agregarla a mi base de datos.");
        }

        private void GenerateHeroCard(ITurnContext context, string[] responseArray)
        {
            Activity reply = context.Activity.CreateReply();

            string answer = responseArray[1];
            string imageUrl = responseArray[2];
            string url = responseArray[2];

            HeroCard card = new HeroCard
            {
                Text = answer,
            };
            /*card.Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.OpenUrl, "Ver imagen en tamaño real", value: url)
                };*/
            card.Images = new List<CardImage>
                {
                    new CardImage( url = imageUrl.Trim())
                };

            reply.Attachments.Add(card.ToAttachment());
            context.SendActivityAsync(reply);
        }

        private void GenerateCarouselCard(ITurnContext context, string[] responseArray)
        {
            Activity reply = context.Activity.CreateReply();
            List<Attachment> listAttachment = new List<Attachment>();

            for (int i = 1; i < responseArray.Length; i = i + 3)
            {
                string Title = responseArray[i];
                string Text = responseArray[i + 1];
                string Image = responseArray[i + 2];

                HeroCard myCard = new HeroCard
                {
                    Title = responseArray[i],
                    Text = responseArray[i + 1],
                };

                List<CardImage> imageList = new List<CardImage>();
                List<CardAction> buttonsList = new List<CardAction>();
                CardImage characterImage = new CardImage(responseArray[i + 2].Trim());
                imageList.Add(characterImage);
                myCard.Images = imageList;

                listAttachment.Add(myCard.ToAttachment());
            }
            reply.Attachments.Clear();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = listAttachment;
            context.SendActivityAsync(reply);
        }

        private void GenerateVideoCard(ITurnContext context, string[] responseArray)
        {
            Activity reply = context.Activity.CreateReply();
            VideoCard myCard = new VideoCard
            {
                Title = responseArray[1],
                Text = responseArray[2],
                Autostart = true,
                Media = new List<MediaUrl>
                 {
                  new MediaUrl()
                    {
                        Url = responseArray[3].Trim()
                    }
                 }
            };

            reply.Attachments.Add(myCard.ToAttachment());
            context.SendActivityAsync(reply);
        }

        private void GenerateAudioCard(ITurnContext context, string[] responseArray)
        {
            Activity reply = context.Activity.CreateReply();

            AudioCard myCard = new AudioCard
            {
                Title = responseArray[1],
                Text = responseArray[2],
                Autostart = true,
                Media = new List<MediaUrl>
                {
                    new MediaUrl()
                    {
                        Url = responseArray[3].Trim()
                    }
                }
            };

            reply.Attachments.Add(myCard.ToAttachment());
            context.SendActivityAsync(reply);
        }
    }
}
