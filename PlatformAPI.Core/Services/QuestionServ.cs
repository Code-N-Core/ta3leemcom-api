using PlatformAPI.Core.DTOs.Choose;
using PlatformAPI.Core.DTOs.Questions;
using PlatformAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.Services
{
    public static class QuestionServ
    {
        public static ShowQuestionsOfQuiz GetQuestionMap(Question q)
        {
            List<ChooseDTO> answer = new List<ChooseDTO>();
            foreach (var c in q.Chooses)
            {
                var a = new ChooseDTO
                {
                    Id = c.Id,
                    Content = c.Content,
                    IsCorrect = c.IsCorrect,
                    QuestionId=c.QuestionId,
                };
                answer.Add(a);
            }
            var ans = q.Chooses.Where(c => c.QuestionId == q.Id && c.IsCorrect == true).SingleOrDefault();
            ShowQuestionsOfQuiz s = new ShowQuestionsOfQuiz
            {
                id = q.Id,
                QuizId = q.QuizId,
                AnswerId =ans is not null?ans.Id:0,
                Answer = ans is not null ? ans.Content : null,
                Choices = answer,
                Content = q.Content,
                Explain = q.Explain,
                Mark = q.Mark,
                Type = q.Type,
                QuizTitle = q.Quiz.Title,
                attachmentPath=q.attachmentPath,
                attachmentType=q.attachmentType,

            };
            return s;
        }
    }
}
