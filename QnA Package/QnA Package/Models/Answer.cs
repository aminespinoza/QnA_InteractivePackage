using Newtonsoft.Json;

namespace QnA_Interactive.Models
{
    public class Answer
    {
        [JsonProperty(PropertyName = "answer")]
        public string QnAAnswer { get; set; }

        [JsonProperty(PropertyName = "score")]
        public double Score { get; set; }

        public static Answer HandleResponse(string receivedValue)
        {
            var response = JsonConvert.DeserializeObject<Rootobject>(receivedValue);
            Answer newAnswer = new Answer();
            newAnswer.QnAAnswer = response.answers[0].answer;
            newAnswer.Score = response.answers[0].score;
            return newAnswer;
        }
    }

    public class Rootobject
    {
        public QnAAnswer[] answers { get; set; }
    }

    public class QnAAnswer
    {
        public string answer { get; set; }
        public string[] questions { get; set; }
        public float score { get; set; }
    }
    public class AnswerHandler
    {
        public string id { get; set; }
    }
}

