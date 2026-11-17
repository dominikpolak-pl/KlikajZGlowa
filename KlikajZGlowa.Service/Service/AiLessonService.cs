using AutoMapper;
using KlikajZGlowa.Database;
using KlikajZGlowa.Model;
using KlikajZGlowa.Service.Ai.Prompts;
using KlikajZGlowa.Service.DTO.Lesson;
using KlikajZGlowa.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace KlikajZGlowa.Service.Service
{
    public class AiLessonService : BaseService, IAiCourseService
    {
        private readonly ILlmClient _llmClient;
        public AiLessonService(Db dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILlmClient llmClient)
            : base(dbContext, mapper, httpContextAccessor) { _llmClient = llmClient ?? throw new ArgumentNullException(nameof(llmClient)); }

        public async Task<GeneratedLessonResultDto> GenerateLessonDraftAsync(GenerateLessonRequestDto request)
        {
            var currentUserId = GetCurrentUserId;
            if (string.IsNullOrEmpty(currentUserId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowany.");
            }

            var course = await _dbContext.Courses
                .Include(c => c.Authors)
                .FirstOrDefaultAsync(c => c.CourseId == request.CourseId);

            if (course == null)
            {
                throw new KeyNotFoundException("Nie znaleziono wskazanego kursu.");
            }

            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;
            var isCourseAuthor = course.Authors.Any(a => a.ApplicationUserId == currentUserId);

            if (!isAdmin && !isCourseAuthor)
            {
                throw new UnauthorizedAccessException("Brak uprawnień do edycji tego kursu.");
            }

            var systemPrompt = AiCoursePrompts.LessonGeneratorSystemPrompt;
            var userPrompt = AiCoursePrompts.BuildLessonUserPrompt(course.Title, course.Description, request.LessonTopic, request.AdditionalInstructions);

            string rawJsonResponse = await _llmClient.GetStructuredCompletionAsync(systemPrompt, userPrompt);

            rawJsonResponse = CleanRawJsonString(rawJsonResponse);
            var generatedLesson = JsonSerializer.Deserialize<GeneratedLessonResultDto>(rawJsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            });

            return generatedLesson ?? throw new InvalidOperationException("Nie udało się sparsować odpowiedzi modelu.");
        }
        private static string CleanRawJsonString(string json)
        {
            var cleaned = json.Trim();
            if (cleaned.StartsWith("```json")) cleaned = cleaned.Substring(7);
            if (cleaned.StartsWith("```")) cleaned = cleaned.Substring(3);
            if (cleaned.EndsWith("```")) cleaned = cleaned.Substring(0, cleaned.Length - 3);

            cleaned = cleaned.Trim();

            var sb = new System.Text.StringBuilder(cleaned.Length);
            bool insideString = false;
            bool isEscaped = false;

            for (int i = 0; i < cleaned.Length; i++)
            {
                char c = cleaned[i];

                if (c == '"' && !isEscaped)
                {
                    insideString = !insideString;
                    sb.Append(c);
                }
                else if (insideString)
                {
                    if (c == '\n')
                    {
                        sb.Append("\\n"); // zamiana surowego 0x0A na "\n"
                    }
                    else if (c == '\r')
                    {
                        // ignorujemy carriage return
                    }
                    else if (c == '\t')
                    {
                        sb.Append("\\t");
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    sb.Append(c);
                }

                isEscaped = (c == '\\' && !isEscaped);
            }

            return sb.ToString();
        }
    }
}
