using KlikajZGlowa.Database;
using KlikajZGlowa.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KlikajZGlowa.Seeder
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            try
            {
                using var db = new Db();

                Console.WriteLine("Migrating database...");
                await db.Database.MigrateAsync();

                // seed users first
                if (!db.Users.Any())
                {
                    var hasher = new PasswordHasher<ApplicationUser>();

                    var authorUser = new ApplicationUser
                    {
                        UserName = "author@local",
                        Email = "author@local",
                        FirstName = "Jan",
                        LastName = "Kowalski",
                        // FullName is computed in ApplicationUser getter
                        EmailConfirmed = true
                    };
                    authorUser.PasswordHash = hasher.HashPassword(authorUser, "P@ssw0rd1");

                    var studentUser = new ApplicationUser
                    {
                        UserName = "student@local",
                        Email = "student@local",
                        FirstName = "Anna",
                        LastName = "Nowak",
                        EmailConfirmed = true
                    };
                    studentUser.PasswordHash = hasher.HashPassword(studentUser, "P@ssw0rd1");

                    db.Users.Add(authorUser);
                    db.Users.Add(studentUser);
                    await db.SaveChangesAsync();

                    // create related profile entities
                    var author = new Author
                    {
                        ApplicationUserId = authorUser.Id,
                        FirstName = authorUser.FirstName,
                        LastName = authorUser.LastName
                    };

                    var enrolled = new EnrolledUser
                    {
                        ApplicationUserId = studentUser.Id,
                        FirstName = studentUser.FirstName,
                        LastName = studentUser.LastName
                    };

                    db.Add(author);
                    db.Add(enrolled);
                    await db.SaveChangesAsync();

                    Console.WriteLine("Dane seedowane: dodano przykładowych użytkowników (author i student).");
                }

                if (!db.Courses.Any())
                {
                    var course = new Course
                    {
                        Title = "Przykładowy kurs",
                        Description = "Kurs dodany przez seeder"
                    };

                    var lesson = new Lesson
                    {
                        Title = "Wprowadzenie",
                        ContentArea = "Podstawy"
                    };

                    course.Lessons.Add(lesson);
                    db.Courses.Add(course);

                    await db.SaveChangesAsync();
                    Console.WriteLine("Dane seedowane: dodano przykładowy kurs i lekcję.");
                }
                else
                {
                    Console.WriteLine("Baza zawiera już kursy - pomijam seeding.");
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Błąd podczas seedowania: {ex}");
                return 1;
            }
        }
    }
}
