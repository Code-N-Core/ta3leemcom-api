using AutoMapper;
using PlatformAPI.Core.Const;
using PlatformAPI.Core.DTOs;
using PlatformAPI.Core.DTOs.Choose;
using PlatformAPI.Core.DTOs.Questions;
using PlatformAPI.Core.Interfaces;
using PlatformAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.Services
{
    public class QuestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly AttachmentService _attachmentService;


        public QuestionService(IUnitOfWork unitOfWork, IMapper mapper, AttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }
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
                    QuestionId = c.QuestionId,
                };
                answer.Add(a);
            }
            var ans = q.Chooses.Where(c => c.QuestionId == q.Id && c.IsCorrect == true).SingleOrDefault();
            ShowQuestionsOfQuiz s = new ShowQuestionsOfQuiz
            {
                id = q.Id,
                QuizId = q.QuizId,
                AnswerId = ans is not null ? ans.Id : 0,
                Answer = ans is not null ? ans.Content : null,
                Choices = answer,
                Content = q.Content,
                Explain = q.Explain,
                Mark = q.Mark,
                Type = q.Type,
                QuizTitle = q.Quiz.Title,
                attachmentPath = q.attachmentPath,
                attachmentType = q.attachmentType,

            };
            return s;
        }


        public async Task<Question> CreateQuestion(QDTO q)
        {
            var question = _mapper.Map<Question>(q);
            try
            {
                question.Type = q.Type == "اجباري" ? QuestionType.Mandatory : QuestionType.Optional;
                /* if (q.AttachFile != null)
                 {
                     // Example: Save the file to the server or cloud storage


                     // Return the file path or a URL that can be used later
                     var fileUrl = $"/uploads/{q.AttachFile.FileName}";

                     //return the type of the attachment
                     var attachmentType = _attachmentService.GetAttachmentType(q.AttachFile.FileName);
                     if (attachmentType == "unknown")
                         return BadRequest("Unsupported file type.");

                     question.attachmentPath = fileUrl;
                     question.attachmentType = attachmentType;

                 }*/
                await _unitOfWork.Question.AddAsync(question);
            }
            catch (Exception)
            {

                return null;
            }
            await _unitOfWork.CompleteAsync();
            return question;
        }
        public async Task<List<ShowQuestionsOfQuiz>> GetAllQuestionsOfQuiz(int id)
        {
            var Q = await _unitOfWork.Question.FindAllWithIncludes<Question>(q => q.QuizId == id,
              q => q.Chooses,
              q => q.Quiz
              );
            if (Q == null) return null;
            List<ShowQuestionsOfQuiz> show = new List<ShowQuestionsOfQuiz>();
            foreach (var q in Q)
            {
                var s = GetQuestionMap(q);
                show.Add(s);
            }
            return show;
        }
        public async Task<List<StudentAnswerSolveDTO>> GetStudentAnswers(int studentquizid)
        {
            var questionresults = await _unitOfWork.StudentAnswer.GetStudentAnswers(studentquizid);
            if (questionresults == null) return null;
            var results=new List<StudentAnswerSolveDTO>();
            foreach (var answer in questionresults)
            {
                var res=new StudentAnswerSolveDTO
                {
                    ChosenId=answer.ChosenOptionId,
                    Id=answer.Id,
                    IsChoosenCorrect=answer.IsCorrect,
                    questionId=answer.QuestionId,
                };
                results.Add(res);
            }
            return results;
        }
    }
}
