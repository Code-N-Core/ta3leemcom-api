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

        public static ShowQuestionsOfQuiz GetQuestionMap(Question q,bool IsTeacher)
        {
            List<ChooseDTO> answer = new List<ChooseDTO>();
            foreach (var c in q.Chooses)
            {
                var a = new ChooseDTO
                {
                    Id = c.Id,
                    Content = c.Content,
                    IsCorrect =IsTeacher==true? c.IsCorrect:null,
                    QuestionId = c.QuestionId,
                };
                answer.Add(a);
            }
            var ans = q.Chooses.Where(c => c.QuestionId == q.Id && c.IsCorrect == true).SingleOrDefault();
            ShowQuestionsOfQuiz s = new ShowQuestionsOfQuiz
            {
                id = q.Id,
                QuizId = q.QuizId,
                AnswerId = ans is not null && IsTeacher == true ? ans.Id : 0,
                Answer = ans is not null && IsTeacher ? ans.Content : null,
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


        public async Task<Question> CreateQuestion(QDTO q,List<ChooseDTO> lcso)
        {
            var question = _mapper.Map<Question>(q);

            try
            {
                question.Type = q.Type == "اجباري" ? QuestionType.Mandatory : QuestionType.Optional;

                #region AttachFile (Optional handling of file upload)
                /*
                if (q.AttachFile != null)
                {
                    // Example: Save the file to the server or cloud storage
                    var fileUrl = $"/uploads/{q.AttachFile.FileName}";

                    // Check for the type of the attachment using the attachment service
                    var attachmentType = _attachmentService.GetAttachmentType(q.AttachFile.FileName);
                    if (attachmentType == "unknown")
                        return null;  // Or handle as you like

                    question.attachmentPath = fileUrl;
                    question.attachmentType = attachmentType;
                }
                */
                #endregion

                await _unitOfWork.Question.AddAsync(question);

                // Commit the transaction
                await _unitOfWork.CompleteAsync();
                foreach (var model in lcso)
                {
                   var choice = _mapper.Map<Choose>(model);
                    choice.QuestionId = question.Id;
                    // Add the choice to the repository
                    await _unitOfWork.Choose.AddAsync(choice);
                }
                await _unitOfWork.CompleteAsync();

            }
            catch (Exception ex)
            {
                // Log the exception if needed
                // e.g., _logger.LogError(ex, "An error occurred while creating a question.");
                return null;  // Return null to indicate failure
            }

            return question;  // Return the question entity with any generated data (e.g., Id)
        }

        public async Task<List<ShowQuestionsOfQuiz>> GetAllQuestionsOfQuiz(int id,bool IsTeacher)
        {
            var Q = await _unitOfWork.Question.FindAllWithIncludes<Question>(q => q.QuizId == id,
              q => q.Chooses,
              q => q.Quiz
              );
            if (Q == null) return null;
            List<ShowQuestionsOfQuiz> show = new List<ShowQuestionsOfQuiz>();
            foreach (var q in Q)
            {
                var s = GetQuestionMap(q,IsTeacher);
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

        public async Task<bool> DeleteQuestionsWithChoises(int id)
        {
           
                var q = await _unitOfWork.Question.GetByIdAsync(id);
            if (q == null) return false;
                try
                {
                    var choices = await _unitOfWork.Choose.FindAllAsync(c => c.QuestionId == id);
                    foreach (var choice in choices)
                        await _unitOfWork.Choose.DeleteAsync(choice);
                    await _unitOfWork.Question.DeleteAsync(q);

                }
                catch (Exception ex)
                {

                    return false;
                }
                return true;


            }
        public async Task ModifiyQuiz(Question q,bool IsDeleted)
        {
            var quiz = (await _unitOfWork.Quiz.GetByIdAsync(q.QuizId));
            bool f = q.Type == QuestionType.Mandatory ? true : false;

            if (f)
            {
                quiz.Mark -= q.Mark;
            }
            else
            {
                quiz.Bounce -= q.Mark;
            }
            var anss = (await _unitOfWork.StudentAnswer.GetAllAsync()).Where(ans => ans.QuestionId == q.Id);
            foreach (var answer in anss)
            {
                if (answer.IsCorrect == true)
                {
                    var studentsolution = (await _unitOfWork.StudentQuiz.GetByIdAsync(answer.StudentQuizId));
                    if (f)
                    {
                        studentsolution.StudentMark -= q.Mark;
                    }
                    else
                    {
                        studentsolution.StudentBounce -= q.Mark;
                    }
                }
                answer.IsCorrect = false;
                if (IsDeleted)
                {
                   await _unitOfWork.StudentAnswer.DeleteAsync(answer);

                }
            }
           await _unitOfWork.CommitAsync();
        }
          
        }
    }

